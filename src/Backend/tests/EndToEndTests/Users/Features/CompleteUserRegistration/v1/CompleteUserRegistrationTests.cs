using System.Net;
using System.Net.Http.Json;
using BuildingBlocks.Utils;
using IntegrationTests.Fakes;
using IntegrationTests.Fakes.UserManagement.Users.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using TestsShared.Fixtures;
using UserManagement;
using UserManagement.Companies.Models;
using UserManagement.Data;
using UserManagement.Industries.Models;
using UserManagement.Users.Feautures.CompletingUserRegistration.V1;
using UserManagement.Users.Models;

namespace EndToEndTests.Users.Features.CompleteUserRegistration.v1;

public class CompleteUserRegistrationEndToEndTests(SharedFixture<Program, UserManagementDbContext> sharedFixture)
    : UserManagementEndToEndTestBase(sharedFixture)
{
    [Fact]
    internal async Task CompleteUserRegistration_WhenRequestIsValid_CreatesUserSuccessfully()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var request = new CompleteUserRegistrationRequestDto(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "Password123!",
            PasswordRepetition: "Password123!",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        var registerRoute = Constants.Routes.User.CompleteRegistration;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(
            registerRoute,
            request,
            cancellationToken: CancellationToken
        );

        // Assert - Validate response
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Verify the user exists in the database
        var createdUser = await SharedFixture.ExecuteEfDbContextAsync(async db =>
        {
            return await db.Users.Include(u => u.Company).FirstOrDefaultAsync(u => u.UserName == request.UserName);
        });

        createdUser.ShouldNotBeNull();
        createdUser.FirstName.ShouldBe(request.FirstName);
        createdUser.LastName.ShouldBe(request.LastName);
        createdUser.UserName.ShouldBe(request.UserName);
        createdUser.Email.ShouldBe(request.Email);
        createdUser.CompanyId.ShouldBe(request.CompanyId);
        createdUser.Company.ShouldNotBeNull();
        createdUser.Company.Id.ShouldBe(company.Id);

        // Verify password was hashed
        createdUser.PasswordHash.ShouldNotBe(request.Password);
        PasswordHasher.VerifyPassword(request.Password, createdUser.PasswordHash).ShouldBeTrue();
    }

    [Fact]
    internal async Task CompleteUserRegistration_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCompanyId = Guid.NewGuid();

        var request = new CompleteUserRegistrationRequestDto(
            CompanyId: nonExistentCompanyId,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "Password123!",
            PasswordRepetition: "Password123!",
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        var registerRoute = Constants.Routes.User.CompleteRegistration;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(
            registerRoute,
            request,
            cancellationToken: CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    internal async Task CompleteUserRegistration_WhenUsernameAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).WithUserName("existinguser").Generate();
        await CreateUserAsync(existingUser);

        var request = new CompleteUserRegistrationRequestDto(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "existinguser", // Duplicate username
            Password: "Password123!",
            PasswordRepetition: "Password123!",
            Email: "different@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        var registerRoute = Constants.Routes.User.CompleteRegistration;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(
            registerRoute,
            request,
            cancellationToken: CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    internal async Task CompleteUserRegistration_WhenEmailAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).WithEmail("existing@example.com").Generate();
        await CreateUserAsync(existingUser);

        var request = new CompleteUserRegistrationRequestDto(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "newuser",
            Password: "Password123!",
            PasswordRepetition: "Password123!",
            Email: "existing@example.com", // Duplicate email
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        var registerRoute = Constants.Routes.User.CompleteRegistration;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(
            registerRoute,
            request,
            cancellationToken: CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    internal async Task CompleteUserRegistration_WhenTermsNotAccepted_ReturnsBadRequest()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var request = new CompleteUserRegistrationRequestDto(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "Password123!",
            PasswordRepetition: "Password123!",
            Email: "john.doe@example.com",
            AcceptTermsOfService: false, // Not accepted
            AcceptPrivacyPolicy: true
        );

        var registerRoute = Constants.Routes.User.CompleteRegistration;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(
            registerRoute,
            request,
            cancellationToken: CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CompleteUserRegistration_WhenValidationFails_ReturnsBadRequest()
    {
        // Arrange - Invalid request with empty first name and mismatched passwords
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var request = new CompleteUserRegistrationRequestDto(
            CompanyId: company.Id,
            FirstName: "", // Invalid - empty
            LastName: "Doe",
            UserName: "ab", // Invalid - too short
            Password: "123", // Invalid - too short
            PasswordRepetition: "different", // Invalid - mismatched
            Email: "john.doe@example.com",
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        var registerRoute = Constants.Routes.User.CompleteRegistration;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(
            registerRoute,
            request,
            cancellationToken: CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CompleteUserRegistration_WithNullEmail_CreatesUserSuccessfully()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var request = new CompleteUserRegistrationRequestDto(
            CompanyId: company.Id,
            FirstName: "John",
            LastName: "Doe",
            UserName: "johndoe",
            Password: "Password123!",
            PasswordRepetition: "Password123!",
            Email: null, // Null email
            AcceptTermsOfService: true,
            AcceptPrivacyPolicy: true
        );

        var registerRoute = Constants.Routes.User.CompleteRegistration;

        // Act
        var response = await SharedFixture.GuestClient.PostAsJsonAsync(registerRoute, request, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Verify the user was created with null email
        var createdUser = await SharedFixture.ExecuteEfDbContextAsync(async db =>
        {
            return await db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        });

        createdUser.ShouldNotBeNull();
        createdUser.Email.ShouldBeNull();
    }

    private async Task CreateIndustryAsync(Industry industry)
    {
        await SharedFixture.ExecuteEfDbContextAsync(async db =>
        {
            await db.Industries.AddAsync(industry);
            await db.SaveChangesAsync();
        });
    }

    private async Task CreateCompanyAsync(Company company)
    {
        await SharedFixture.ExecuteEfDbContextAsync(async db =>
        {
            await db.Companies.AddAsync(company);
            await db.SaveChangesAsync();
        });
    }

    private async Task CreateUserAsync(User user)
    {
        await SharedFixture.ExecuteEfDbContextAsync(async db =>
        {
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        });
    }
}