using BuildingBlocks.Utils;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using UserManagement.Companies.Models;
using UserManagement.Data;
using UserManagement.Users.Exceptions;
using UserManagement.Users.Feautures.CompletingUserRegistration.V1;
using UserManagement.Users.Models;

namespace UnitTests.Users.Features.CompletingUserRegistration.v1;

public class CompleteUserRegistrationHandlerTests : IDisposable
{
    private readonly UserManagementDbContext _dbContext;
    private readonly CompleteUserRegistrationHandler _handler;

    public CompleteUserRegistrationHandlerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UserManagementDbContext(options);

        // Seed test data
        SeedTestData();

        _handler = new CompleteUserRegistrationHandler(_dbContext);
    }

    private void SeedTestData()
    {
        // Add a test company
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            IndustryId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Version = 1
        };
        _dbContext.Companies.Add(company);

        // Add a test user for duplicate testing
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Existing",
            LastName = "User",
            UserName = "existinguser",
            Email = "existing@example.com",
            PasswordHash = "hashed_password",
            CompanyId = company.Id,
            CreatedAt = DateTime.UtcNow,
            Version = 1
        };
        _dbContext.Users.Add(existingUser);

        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesUserSuccessfully()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldNotBe(Guid.Empty);

        // Verify user was saved to database
        var createdUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == result.UserId);
        createdUser.ShouldNotBeNull();
        createdUser.FirstName.ShouldBe(command.FirstName);
        createdUser.LastName.ShouldBe(command.LastName);
        createdUser.UserName.ShouldBe(command.UserName);
        createdUser.Email.ShouldBe(command.Email);
        createdUser.CompanyId.ShouldBe(command.CompanyId);

        // Verify entity properties are set
        createdUser.Id.ShouldNotBe(Guid.Empty);
        createdUser.CreatedAt.ShouldNotBeNull();
        createdUser.IsDeleted.ShouldBeFalse();

        // Verify password was hashed
        createdUser.PasswordHash.ShouldNotBe(command.Password);
        PasswordHasher.VerifyPassword(command.Password, createdUser.PasswordHash).ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentCompany_ThrowsCompanyNotFoundException()
    {
        // Arrange
        var nonExistentCompanyId = Guid.NewGuid();
        var command = new CompleteUserRegistrationCommand(
            CompanyId: nonExistentCompanyId,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act & Assert
        var exception = await Should.ThrowAsync<CompanyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain(nonExistentCompanyId.ToString());
    }

    [Fact]
    public async Task Handle_WithExistingUsername_ThrowsUsernameAlreadyExistsException()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var existingUser = await _dbContext.Users.FirstAsync(u => u.UserName == "existinguser");

        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: existingUser.UserName, // Duplicate username
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "newemail@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act & Assert
        var exception = await Should.ThrowAsync<UsernameAlreadyExistsException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain(existingUser.UserName);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ThrowsEmailAlreadyExistsException()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var existingUser = await _dbContext.Users.FirstAsync(u => u.Email == "existing@example.com");

        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "newusername",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: existingUser.Email, // Duplicate email
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act & Assert
        var exception = await Should.ThrowAsync<EmailAlreadyExistsException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain(existingUser.Email!);
    }

    [Fact]
    public async Task Handle_WithoutAcceptingTermsOfService_ThrowsTermsNotAcceptedException()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: false, // Not accepted
            AcceptPrivacyPolicy: true
        );

        // Act & Assert
        await Should.ThrowAsync<TermsNotAcceptedException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_WithoutAcceptingPrivacyPolicy_ThrowsTermsNotAcceptedException()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: false // Not accepted
        );

        // Act & Assert
        await Should.ThrowAsync<TermsNotAcceptedException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_WithNullEmail_DoesNotCheckForEmailDuplicatesAndCreatesUser()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: null, // Null email
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldNotBe(Guid.Empty);

        var createdUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == result.UserId);
        createdUser.ShouldNotBeNull();
        createdUser.Email.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WithEmptyEmail_DoesNotCheckForEmailDuplicatesAndCreatesUser()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "", // Empty email
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldNotBe(Guid.Empty);

        var createdUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == result.UserId);
        createdUser.ShouldNotBeNull();
        createdUser.Email.ShouldBe("");
    }

    [Fact]
    public async Task Handle_WithDeletedCompany_ThrowsCompanyNotFoundException()
    {
        // Arrange
        var deletedCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Deleted Company",
            IndustryId = Guid.NewGuid(),
            IsDeleted = true, // Marked as deleted
            CreatedAt = DateTime.UtcNow,
            Version = 1
        };
        _dbContext.Companies.Add(deletedCompany);
        await _dbContext.SaveChangesAsync();

        var command = new CompleteUserRegistrationCommand(
            CompanyId: deletedCompany.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act & Assert
        await Should.ThrowAsync<CompanyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_WithDeletedUser_AllowsUsernameReuse()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();

        // Add a deleted user with the same username we want to use
        var deletedUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Deleted",
            LastName = "User",
            UserName = "deleteduser",
            Email = "deleted@example.com",
            PasswordHash = "hashed_password",
            CompanyId = company.Id,
            IsDeleted = true, // Marked as deleted
            CreatedAt = DateTime.UtcNow,
            Version = 1
        };
        _dbContext.Users.Add(deletedUser);
        await _dbContext.SaveChangesAsync();

        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "deleteduser", // Same username as deleted user
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should succeed since the existing user is deleted
        result.ShouldNotBeNull();
        result.UserId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenDatabaseOperationFails_ThrowsException()
    {
        // Arrange
        var company = await _dbContext.Companies.FirstAsync();
        var command = new CompleteUserRegistrationCommand(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Simulate database failure by disposing context
        _dbContext.Dispose();

        // Act & Assert
        await Should.ThrowAsync<Exception>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}