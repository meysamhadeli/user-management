using Microsoft.EntityFrameworkCore;
using Shouldly;
using UserManagement.Data;
using UserManagement.Users.Feautures.CheckingEmailAvailability.V1;
using UserManagement.Users.Models;

namespace UnitTests.Users.Features.CheckEmailAvailability.v1;

public class CheckEmailAvailabilityCommandValidatorTests
{
    private readonly UserManagementDbContext _dbContext;
    private readonly CheckEmailAvailabilityCommandValidator _validator;

    public CheckEmailAvailabilityCommandValidatorTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UserManagementDbContext(options);

        // Seed test data
        SeedTestData();

        _validator = new CheckEmailAvailabilityCommandValidator();
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
    public void Validator_WithValidEmail_PassesValidation()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("valid.email@example.com");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validator_WithInvalidEmailFormat_FailsValidation()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("invalid-email-format");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Validator_WithNullEmail_PassesValidation()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand(null!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue(); // Validator allows null/empty due to When condition
    }

    [Fact]
    public void Validator_WithEmptyEmail_PassesValidation()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue(); // Validator allows null/empty due to When condition
    }

    [Fact]
    public void Validator_WithWhitespaceEmail_PassesValidation()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("   ");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue(); // Validator allows null/empty due to When condition
    }
}