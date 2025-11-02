using System.Net;
using System.Net.Http.Json;
using EndToEndTests;
using IntegrationTests.Fakes;
using IntegrationTests.Fakes.UserManagement.Users.Tests.Fakes;
using Shouldly;
using TestsShared.Fixtures;
using UserManagement.Companies.Models;
using UserManagement.Data;
using UserManagement.Industries.Models;
using UserManagement.Users.Feautures.CheckingEmailAvailability.V1;
using UserManagement.Users.Models;

namespace UserManagement.Users.Tests.Features.CheckEmailAvailability;

public class CheckEmailAvailabilityEndToEndTests(SharedFixture<Program, UserManagementDbContext> sharedFixture)
    : UserManagementEndToEndTestBase(sharedFixture)
{
    [Fact]
    internal async Task CheckEmailAvailability_WhenEmailDoesNotExist_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckEmailAvailabilityRequestDto("nonexisting@example.com");
        var checkEmailRoute = $"{Constants.Routes.User.CheckEmailAvailability}?email={request.Email}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckEmailAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WhenEmailExists_ReturnsNotAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).WithEmail("existing@example.com").Generate();
        await CreateUserAsync(existingUser);

        var request = new CheckEmailAvailabilityRequestDto("existing@example.com");
        var checkEmailRoute = $"{Constants.Routes.User.CheckEmailAvailability}?email={request.Email}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckEmailAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WhenEmailIsDeletedUser_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var deletedUser = new UserFake(company.Id).WithEmail("deleted@example.com").AsDeleted().Generate();
        await CreateUserAsync(deletedUser);

        var request = new CheckEmailAvailabilityRequestDto("deleted@example.com");
        var checkEmailRoute = $"{Constants.Routes.User.CheckEmailAvailability}?email={request.Email}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckEmailAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue(); // Should be available because user is deleted
    }

    [Fact]
    internal async Task CheckEmailAvailability_WhenEmailIsInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var request = new CheckEmailAvailabilityRequestDto("invalid-email-format");
        var checkEmailRoute = $"{Constants.Routes.User.CheckEmailAvailability}?email={request.Email}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CheckEmailAvailability_WhenEmailIsNull_ReturnsBadRequest()
    {
        // Arrange - Note: For [AsParameters] with GET, null might be handled differently
        // This tests the behavior when email parameter is missing or null
        var checkEmailRoute = Constants.Routes.User.CheckEmailAvailability;

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithSpecialCharactersEmail_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckEmailAvailabilityRequestDto("user.name+tag@example.com");
        var checkEmailRoute = $"{Constants.Routes.User.CheckEmailAvailability}?email={request.Email}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckEmailAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WhenDatabaseIsEmpty_ReturnsAvailable()
    {
        // Arrange - No users created
        var request = new CheckEmailAvailabilityRequestDto("any@example.com");
        var checkEmailRoute = $"{Constants.Routes.User.CheckEmailAvailability}?email={request.Email}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckEmailAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithNullEmailInDatabase_DoesNotAffectAvailability()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        // Create a user with null email
        var userWithNullEmail = new UserFake(company.Id).WithEmail(null).Generate();
        await CreateUserAsync(userWithNullEmail);

        var request = new CheckEmailAvailabilityRequestDto("test@example.com");
        var checkEmailRoute = $"{Constants.Routes.User.CheckEmailAvailability}?email={request.Email}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkEmailRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckEmailAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue(); // Should be available since null emails don't count
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