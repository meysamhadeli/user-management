using Shouldly;
using UserManagement.Users.Feautures.CompletingUserRegistration.V1;

namespace UnitTests.Users.Features.CompletingUserRegistration.v1;

public class CompleteUserRegistrationValidatorTests
{
    private readonly CompleteUserRegistrationValidator _validator;

    public CompleteUserRegistrationValidatorTests()
    {
        _validator = new CompleteUserRegistrationValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
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
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCompanyId_FailsValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.Empty,
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
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Company is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidFirstName_FailsValidation(string invalidFirstName)
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: invalidFirstName,
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "First name is required");
    }

    [Fact]
    public void Validate_WithTooLongFirstName_FailsValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: new string('a', 101), // 101 characters
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "First name must not exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidLastName_FailsValidation(string invalidLastName)
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: invalidLastName,
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Last name is required");
    }

    [Fact]
    public void Validate_WithTooLongLastName_FailsValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: new string('a', 101), // 101 characters
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Last name must not exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidUsername_FailsValidation(string invalidUsername)
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: invalidUsername,
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

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
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "ab", // 2 characters
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

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
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: new string('a', 51), // 51 characters
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username must not exceed 50 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidPassword_FailsValidation(string invalidPassword)
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: invalidPassword,
            PasswordRepetition: invalidPassword,
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Password is required");
    }

    [Fact]
    public void Validate_WithTooShortPassword_FailsValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "12345", // 5 characters
            PasswordRepetition: "12345",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Password must be at least 6 characters");
    }

    [Fact]
    public void Validate_WithMismatchedPasswords_FailsValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "differentpassword",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "Passwords do not match");
    }

    [Fact]
    public void Validate_WithNullEmail_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: null,
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithEmptyEmail_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithoutAcceptingTermsOfService_FailsValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: false,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "You must accept the terms of service");
    }

    [Fact]
    public void Validate_WithoutAcceptingPrivacyPolicy_FailsValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: false
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "You must accept the privacy policy");
    }

    [Fact]
    public void Validate_WithNeitherTermsNorPrivacyPolicyAccepted_FailsValidationWithBothErrors()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: false,
            AcceptPrivacyPolicy: false
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage == "You must accept the terms of service");
        result.Errors.ShouldContain(error => error.ErrorMessage == "You must accept the privacy policy");
    }

    [Fact]
    public void Validate_WithMultipleValidationErrors_ReturnsAllErrors()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.Empty,
            FirstName: "",
            LastName: "",
            UserName: "ab", // Too short
            Password: "123", // Too short
            PasswordRepetition: "different", // Mismatched
            Email: "john.doe@example.com",
            AcceptTermsOfService: false,
            AcceptPrivacyPolicy: false
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(8); // All fields have errors
        result.Errors.ShouldContain(error => error.ErrorMessage == "Company is required");
        result.Errors.ShouldContain(error => error.ErrorMessage == "First name is required");
        result.Errors.ShouldContain(error => error.ErrorMessage == "Last name is required");
        result.Errors.ShouldContain(error => error.ErrorMessage == "Username must be at least 3 characters");
        result.Errors.ShouldContain(error => error.ErrorMessage == "Password must be at least 6 characters");
        result.Errors.ShouldContain(error => error.ErrorMessage == "Passwords do not match");
        result.Errors.ShouldContain(error => error.ErrorMessage == "You must accept the terms of service");
        result.Errors.ShouldContain(error => error.ErrorMessage == "You must accept the privacy policy");
    }

    [Fact]
    public void Validate_WithExactMinimumLengthPassword_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "123456", // Exactly 6 characters
            PasswordRepetition: "123456",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithExactMaximumLengthFirstName_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: new string('a', 100), // Exactly 100 characters
            LastName: "Doe",
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithExactMaximumLengthLastName_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: new string('a', 100), // Exactly 100 characters
            UserName: "johndoe",
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithExactMinimumLengthUsername_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: "usr", // Exactly 3 characters
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithExactMaximumLengthUsername_PassesValidation()
    {
        // Arrange
        var command = new CompleteUserRegistrationCommand(
            CompanyId: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            UserName: new string('a', 50), // Exactly 50 characters
            Password: "password123",
            PasswordRepetition: "password123",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}