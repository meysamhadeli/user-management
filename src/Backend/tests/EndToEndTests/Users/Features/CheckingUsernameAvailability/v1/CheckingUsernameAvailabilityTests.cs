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
using UserManagement.Users.Feautures.CheckingUsernameAvailability.V1;
using UserManagement.Users.Models;

namespace UserManagement.Users.Tests.Features.CheckUsernameAvailability;

public class CheckUsernameAvailabilityEndToEndTests(SharedFixture<Program, UserManagementDbContext> sharedFixture)
    : UserManagementEndToEndTestBase(sharedFixture)
{
    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameDoesNotExist_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("nonexistinguser");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameExists_ReturnsNotAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).WithUserName("existinguser").Generate();
        await CreateUserAsync(existingUser);

        var request = new CheckUsernameAvailabilityRequestDto("existinguser");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameIsDeletedUser_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var deletedUser = new UserFake(company.Id).WithUserName("deleteduser").AsDeleted().Generate();
        await CreateUserAsync(deletedUser);

        var request = new CheckUsernameAvailabilityRequestDto("deleteduser");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue(); // Should be available because user is deleted
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameIsTooShort_ReturnsBadRequest()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("ab"); // 2 characters
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameIsTooLong_ReturnsBadRequest()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto(new string('a', 51)); // 51 characters
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameIsNull_ReturnsBadRequest()
    {
        // Arrange
        var checkUsernameRoute = Constants.Routes.User.CheckUsernameAvailability;

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenUsernameHasInvalidCharacters_ReturnsBadRequest()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("user@name"); // @ symbol not allowed
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithValidSpecialCharacters_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("user.name_123");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenDatabaseIsEmpty_ReturnsAvailable()
    {
        // Arrange - No users created
        var request = new CheckUsernameAvailabilityRequestDto("anyusername");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithMinimumLengthUsername_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("usr"); // Exactly 3 characters
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithMaximumLengthUsername_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto(new string('a', 50)); // Exactly 50 characters
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithOnlyUnderscores_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("___");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithOnlyPeriods_ReturnsAvailable()
    {
        // Arrange
        var request = new CheckUsernameAvailabilityRequestDto("...");
        var checkUsernameRoute = $"{Constants.Routes.User.CheckUsernameAvailability}?username={request.Username}";

        // Act
        var response = await SharedFixture.GuestClient.GetAsync(checkUsernameRoute, CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadFromJsonAsync<CheckUsernameAvailabilityResponseDto>(
            cancellationToken: CancellationToken
        );

        responseContent.ShouldNotBeNull();
        responseContent.IsAvailable.ShouldBeTrue();
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
