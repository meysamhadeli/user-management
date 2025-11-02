using FluentValidation;
using Shouldly;
using UserManagement.Users.Feautures.CheckingUsernameAvailability.V1;

namespace UserManagement.Users.Tests.Features.CheckUsernameAvailability;

public class CheckUsernameAvailabilityCommandValidatorTests
{
    private readonly CheckUsernameAvailabilityCommandValidator _validator;

    public CheckUsernameAvailabilityCommandValidatorTests()
    {
        _validator = new CheckUsernameAvailabilityCommandValidator();
    }

    [Fact]
    public void Validate_WithValidUsername_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("validuser123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUsername_FailsValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username is required");
    }

    [Fact]
    public void Validate_WithNullUsername_FailsValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(null!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username is required");
    }

    [Fact]
    public void Validate_WithWhitespaceUsername_FailsValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("   ");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username is required");
    }

    [Fact]
    public void Validate_WithTooShortUsername_FailsValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("ab"); // 2 characters

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username must be at least 3 characters");
    }

    [Fact]
    public void Validate_WithTooLongUsername_FailsValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(new string('a', 51)); // 51 characters

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username must not exceed 50 characters");
    }

    [Theory]
    [InlineData("user@name")]    // @ symbol not allowed
    [InlineData("user-name")]    // dash not allowed
    [InlineData("user name")]    // space not allowed
    [InlineData("user$name")]    // $ symbol not allowed
    [InlineData("user#name")]    // # symbol not allowed
    [InlineData("user%name")]    // % symbol not allowed
    [InlineData("user/name")]    // / symbol not allowed
    [InlineData("user\\name")]   // \ symbol not allowed
    [InlineData("user|name")]    // | symbol not allowed
    [InlineData("user+name")]    // + symbol not allowed
    [InlineData("user=name")]    // = symbol not allowed
    public void Validate_WithInvalidCharacters_FailsValidation(string invalidUsername)
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(invalidUsername);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error =>
            error.ErrorMessage == "Username can only contain letters, numbers, underscores and periods");
    }

    [Theory]
    [InlineData("user_name")]        // underscore allowed
    [InlineData("user.name")]        // period allowed
    [InlineData("user123")]          // numbers allowed
    [InlineData("UserName")]         // mixed case allowed
    [InlineData("user.name_123")]    // combination of allowed characters
    [InlineData("test_user.name")]   // complex but valid pattern
    public void Validate_WithValidCharacters_PassesValidation(string validUsername)
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(validUsername);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithExactMinimumLength_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("usr"); // 3 characters

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithExactMaximumLength_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(new string('a', 50)); // 50 characters

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithOnlyUnderscores_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("___");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithOnlyPeriods_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("...");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithOnlyNumbers_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithOnlyLetters_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("abc");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithConsecutiveSpecialCharacters_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("user..name__test");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithSpecialCharactersAtStart_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(".username");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithSpecialCharactersAtEnd_PassesValidation()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("username_");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithMultipleValidationErrors_ReturnsAllErrors()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("a@"); // Too short and invalid character

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username must be at least 3 characters");
        result.Errors.ShouldContain(error =>
            error.ErrorMessage == "Username can only contain letters, numbers, underscores and periods");
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(25)]
    [InlineData(49)]
    [InlineData(50)]
    public void Validate_WithLengthInValidRange_PassesValidation(int length)
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(new string('a', length));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(51)]
    [InlineData(100)]
    public void Validate_WithLengthOutsideValidRange_FailsValidation(int length)
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(new string('a', length));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }
}