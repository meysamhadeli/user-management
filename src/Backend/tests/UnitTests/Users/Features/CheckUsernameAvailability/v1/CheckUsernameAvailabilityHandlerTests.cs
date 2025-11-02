using Microsoft.EntityFrameworkCore;
using Shouldly;
using UserManagement.Data;
using UserManagement.Users.Feautures.CheckingUsernameAvailability.V1;
using UserManagement.Users.Models;

namespace UnitTests.Users.Features.CheckUsernameAvailability.v1;

public class CheckUsernameAvailabilityHandlerTests : IDisposable
{
    private readonly UserManagementDbContext _dbContext;
    private readonly CheckUsernameAvailabilityHandler _handler;

    public CheckUsernameAvailabilityHandlerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UserManagementDbContext(options);

        // Seed test data
        SeedTestData();

        _handler = new CheckUsernameAvailabilityHandler(_dbContext);
    }

    private void SeedTestData()
    {
        // Add test users with various username scenarios
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
                UserName = "CaseSensitiveUser", // Mixed case username
                Email = "case@example.com",
                PasswordHash = "hashed_password",
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow,
                Version = 1
            },
            new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Special",
                LastName = "Chars",
                UserName = "user.name_123", // Username with special allowed characters
                Email = "special@example.com",
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
    public async Task Handle_WithNonExistingUsername_ReturnsAvailable()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("nonexistinguser");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithExistingUsername_ReturnsNotAvailable()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("existinguser");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WithDeletedUserUsername_ReturnsAvailable()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("deleteduser");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue(); // Should be available because user is deleted
    }

    [Fact]
    public async Task Handle_WithNullUsername_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(null!);

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain("Value cannot be null");
    }

    [Fact]
    public async Task Handle_WithEmptyUsername_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("");

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'command.Username')");
    }

    [Fact]
    public async Task Handle_WithWhitespaceUsername_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("   ");

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldContain("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'command.Username')");
    }

    [Fact]
    public async Task Handle_WithUsernameContainingAllowedSpecialCharacters_WorksCorrectly()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("user.name_123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should not be available (exists in seed data)
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeFalse();
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
        var emptyHandler = new CheckUsernameAvailabilityHandler(emptyDbContext);

        var command = new CheckUsernameAvailabilityCommand("anyusername");

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
        var command = new CheckUsernameAvailabilityCommand("existinguser");

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
        var command = new CheckUsernameAvailabilityCommand("testuser");
        var cancellationToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.Handle(command, cancellationToken)
        );
    }

    [Fact]
    public async Task Handle_WithVeryLongUsername_WorksCorrectly()
    {
        // Arrange
        var longUsername = "a".Repeat(100); // Very long username
        var command = new CheckUsernameAvailabilityCommand(longUsername);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithMixedCaseNewUsername_ReturnsAvailable()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("NewMixedCaseUser");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithNumbersOnlyUsername_WorksCorrectly()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("123456");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithUnderscoreOnlyUsername_WorksCorrectly()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("___");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithPeriodOnlyUsername_WorksCorrectly()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("...");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Should be available
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithComplexUsernamePattern_WorksCorrectly()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("user.name_123.MIXEDcase");

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

// Extension method for string repetition
public static class StringExtensions
{
    public static string Repeat(this string text, int count)
    {
        return string.Concat(Enumerable.Repeat(text, count));
    }
}