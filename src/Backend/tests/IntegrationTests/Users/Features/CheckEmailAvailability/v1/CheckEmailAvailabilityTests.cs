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
using UserManagement.Users.Feautures.CheckingEmailAvailability.V1;
using UserManagement.Users.Models;

namespace IntegrationTests.Users.Features.CheckEmailAvailability.v1;

public class CheckEmailAvailabilityTests(SharedFixture<Program, UserManagementDbContext> sharedFixture)
    : UserManagementIntegrationTestBase(sharedFixture)
{
    [Fact]
    internal async Task CheckEmailAvailability_WithNonExistingEmail_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CheckEmailAvailabilityCommand("nonexisting@example.com");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithExistingEmail_ReturnsNotAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).WithEmail("existing@example.com").Generate();
        await CreateUserAsync(existingUser);

        var command = new CheckEmailAvailabilityCommand("existing@example.com");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeFalse();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithDeletedUser_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var deletedUser = new UserFake(company.Id).WithEmail("deleted@example.com").AsDeleted().Generate();
        await CreateUserAsync(deletedUser);

        var command = new CheckEmailAvailabilityCommand("deleted@example.com");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue(); // Should be available because user is deleted
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithNullEmail_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand(null!);

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithEmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithWhitespaceEmail_ThrowsArgumentException()
    {
        // Arrange
        var command = new CheckEmailAvailabilityCommand("   ");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithSpecialCharactersEmail_ReturnsAvailable()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CheckEmailAvailabilityCommand("user.name+tag@example.com");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WhenDatabaseIsEmpty_ReturnsAvailable()
    {
        // Arrange - No users created
        var command = new CheckEmailAvailabilityCommand("any@example.com");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
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

        var command = new CheckEmailAvailabilityCommand("test@example.com");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert - Should be available since null emails don't count as existing
        result.ShouldNotBeNull();
        result.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    internal async Task CheckEmailAvailability_WithEmptyEmailInDatabase_DoesNotAffectAvailability()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        // Create a user with empty email
        var userWithEmptyEmail = new UserFake(company.Id).WithEmail("").Generate();
        await CreateUserAsync(userWithEmptyEmail);

        var command = new CheckEmailAvailabilityCommand("test@example.com");

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CheckEmailAvailabilityCommand, CheckEmailAvailabilityResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert - Should be available since empty emails don't count as existing
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