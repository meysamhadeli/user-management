using Microsoft.EntityFrameworkCore;
using Shouldly;
using UserManagement.Data;
using UserManagement.Users.Feautures.CheckingEmailAvailability.V1;
using UserManagement.Users.Models;

namespace UnitTests.Users.Features.CheckEmailAvailability.v1;

public class CheckEmailAvailabilityHandlerTests : IDisposable
{
    private readonly UserManagementDbContext _dbContext;
    private readonly CheckEmailAvailabilityHandler _handler;

    public CheckEmailAvailabilityHandlerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UserManagementDbContext(options);

        // Seed test data
        SeedTestData();

        _handler = new CheckEmailAvailabilityHandler(_dbContext);
    }

    private void SeedTestData()
    {
        // Add test users with various email scenarios
        var companyId = Guid.NewGuid();

        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Existing",
                LastName = "User",
                UserName = "existinguser",
                Email = "existing@example.com",
                PasswordHash = "hashed_password",
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Deleted",
                LastName = "User",
                UserName = "deleteduser",
                Email = "deleted@example.com",
                PasswordHash = "hashed_password",
                CompanyId = companyId,
                IsDeleted = true, // Soft deleted user
                CreatedAt = DateTime.UtcNow,
                Version = 1
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Case",
                LastName = "Sensitive",
                UserName = "casesensitive",
                Email = "CaseSensitive@Example.COM", // Mixed case email
                PasswordHash = "hashed_password",
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "No",
                LastName = "Email",
                UserName = "noemail",
                Email = null, // User with null email
                PasswordHash = "hashed_password",
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Empty",
                LastName = "Email",
                UserName = "emptyemail",
                Email = "", // User with empty email
                PasswordHash = "hashed_password",
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            }
        };

        _dbContext.Users.AddRange(users);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task Handle_WithNonExistingEmail_ReturnsAvailable()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("nonexisting@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ReturnsNotAvailable()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("existing@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WithDeletedUserEmail_ReturnsAvailable()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("deleted@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue(); // Should be available because user is deleted
    }

    [Fact]
    public async Task Handle_WithNullEmail_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand(null!);

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain("Value cannot be null");
    }

    [Fact]
    public async Task Handle_WithEmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("");

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'command.Email')");
    }

    [Fact]
    public async Task Handle_WithWhitespaceEmail_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("   ");

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'command.Email')");
    }

    [Fact]
    public async Task Handle_WithNullEmailInDatabase_DoesNotAffectAvailabilityCheck()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("newemail@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available since null emails don't count as existing
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithEmptyEmailInDatabase_DoesNotAffectAvailabilityCheck()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("anothernew@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available since empty emails don't count as existing
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithSpecialCharactersInEmail_WorksCorrectly()
    {
        // Arrange
        var specialEmail = "test.email+tag@example.com";
        var command = new CheckEmailAvailabilityCommand(specialEmail);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }


    [Fact]
    public async Task Handle_WhenDatabaseIsEmpty_ReturnsAvailable()
    {
        // Arrange
        // Create a fresh context with no data
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var emptyDbContext = new UserManagementDbContext(options);
        var emptyHandler = new CheckEmailAvailabilityHandler(emptyDbContext);

        var command = new CheckEmailAvailabilityCommand("any@example.com");

        // Act
        var result = await emptyHandler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();

        emptyDbContext.Dispose();
    }

    [Fact]
    public async Task Handle_WithMultipleCalls_ReturnsConsistentResults()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("existing@example.com");

        // Act - Call multiple times
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);
        var result3 = await _handler.Handle(command, CancellationToken.None);

        // Assert - All calls should return the same result
        result1.IsAvailable.ShouldBe(result2.IsAvailable);
        result2.IsAvailable.ShouldBe(result3.IsAvailable);
        result1.IsAvailable.ShouldBeFalse(); // Should consistently return false
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("test@example.com");
        var cancellationToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.Handle(command, cancellationToken)
        );
    }

    [Fact]
    public async Task Handle_WithVeryLongEmail_WorksCorrectly()
    {
        // Arrange
        var longEmail = $"{"a".Repeat(100)}@{"b".Repeat(100)}.{"c".Repeat(10)}"; // Very long email
        var command = new CheckEmailAvailabilityCommand(longEmail);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}

public static class StringExtensions
{
    public static string Repeat(this string text, int count)
    {
        return string.Concat(Enumerable.Repeat(text, count));
    }
}