using IntegrationTests.Fakes;
using IntegrationTests.Fakes.UserManagement.Users.Tests.Fakes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TestsShared.Fixtures;
using UserManagement;
using UserManagement.Companies.Models;
using UserManagement.Data;
using UserManagement.Industries.Models;
using UserManagement.Users.Feautures.CheckingUsernameAvailability.V1;
using UserManagement.Users.Models;

namespace IntegrationTests.Users.Features.CheckUsernameAvailability.v1;

public class CheckUsernameAvailabilityTests(SharedFixture<Program, UserManagementDbContext> sharedFixture)
    : UserManagementIntegrationTestBase(sharedFixture)
{
    [Fact]
    internal async Task CheckUsernameAvailability_WithNonExistingUsername_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CheckUsernameAvailabilityCommand("nonexistingusername");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithExistingUsername_ReturnsNotAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).WithUserName("existinguser").Generate();
        await CreateUserAsync(existingUser);

        var command = new CheckUsernameAvailabilityCommand("existinguser");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithDeletedUser_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var deletedUser = new UserFake(company.Id).WithUserName("deleteduser").AsDeleted().Generate();
        await CreateUserAsync(deletedUser);

        var command = new CheckUsernameAvailabilityCommand("deleteduser");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue(); // Should be available because user is deleted
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithNullUsername_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand(null!);

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithEmptyUsername_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithWhitespaceUsername_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckUsernameAvailabilityCommand("   ");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WithSpecialCharactersUsername_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CheckUsernameAvailabilityCommand("user.name_123");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckUsernameAvailability_WhenDatabaseIsEmpty_ReturnsAvailable()
    {
        // Arrange - No users created
        var command = new CheckUsernameAvailabilityCommand("anyusername");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckUsernameAvailabilityCommand, CheckUsernameAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
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