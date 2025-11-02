using IntegrationTests.Fakes;
using IntegrationTests.Fakes.UserManagement.Users.Tests.Fakes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TestsShared.Fixtures;
using UserManagement;
using UserManagement.Companies.Models;
using UserManagement.Data;
using UserManagement.Industries.Models;
using UserManagement.Users.Exceptions;
using UserManagement.Users.Feautures.CompletingUserRegistration.V1;
using UserManagement.Users.Models;

namespace IntegrationTests.Users.Features.CompleteUserRegistration.v1;

public class CompleteUserRegistrationTests(SharedFixture<Program, UserManagementDbContext> sharedFixture)
    : UserManagementIntegrationTestBase(sharedFixture)
{
    [Fact]
    internal async Task CompleteUserRegistration_WithValidData_CreatesUserSuccessfully()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CompleteUserRegistrationCommandFake(company.Id).Generate();

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldNotBe(Guid.Empty);

        var createdUser = await GetUserAsync(result.UserId);
        createdUser.ShouldNotBeNull();
        createdUser.UserName.ShouldBe(command.UserName);
        createdUser.Email.ShouldBe(command.Email);
        createdUser.CompanyId.ShouldBe(command.CompanyId);
        createdUser.FirstName.ShouldBe(command.FirstName);
        createdUser.LastName.ShouldBe(command.LastName);
    }

    [Fact]
    internal async Task CompleteUserRegistration_WithNonExistentCompany_ThrowsCompanyNotFoundException()
    {
        // Arrange
        var nonExistentCompanyId = Guid.NewGuid();
        var command = new CompleteUserRegistrationCommandFake(nonExistentCompanyId).Generate();

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<CompanyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CompleteUserRegistration_WithExistingUsername_ThrowsUsernameAlreadyExistsException()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).Generate();
        await CreateUserAsync(existingUser);

        var command = new CompleteUserRegistrationCommandFake(company.Id)
            .WithUserName(existingUser.UserName)
            .Generate();

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<UsernameAlreadyExistsException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CompleteUserRegistration_WithExistingEmail_ThrowsEmailAlreadyExistsException()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var existingUser = new UserFake(company.Id).Generate();
        await CreateUserAsync(existingUser);

        var command = new CompleteUserRegistrationCommandFake(company.Id).WithEmail(existingUser.Email).Generate();

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<EmailAlreadyExistsException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CompleteUserRegistration_WithoutAcceptingTerms_ThrowsTermsNotAcceptedException()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CompleteUserRegistrationCommandFake(company.Id).WithoutTermsAcceptance().Generate();

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<TermsNotAcceptedException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CompleteUserRegistration_WithoutAcceptingPrivacyPolicy_ThrowsTermsNotAcceptedException()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CompleteUserRegistrationCommandFake(company.Id).WithoutPrivacyPolicyAcceptance().Generate();

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
        >();

        // Act & Assert
        await Should.ThrowAsync<TermsNotAcceptedException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    internal async Task CompleteUserRegistration_WithNullEmail_CreatesUserSuccessfully()
    {
        // Arrange
        var industry = new IndustryFake().Generate();
        await CreateIndustryAsync(industry);

        var company = new CompanyFake(industry.Id).Generate();
        await CreateCompanyAsync(company);

        var command = new CompleteUserRegistrationCommandFake(company.Id).WithEmail(null).Generate();

        var handler = Scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CompleteUserRegistrationCommand, CompleteUserRegistrationResult>
        >();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldNotBe(Guid.Empty);

        var createdUser = await GetUserAsync(result.UserId);
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

    private async Task<User?> GetUserAsync(Guid userId)
    {
        return await SharedFixture.ExecuteEfDbContextAsync(async db =>
            await db.Users.FirstOrDefaultAsync(u => u.Id == userId)
        );
    }
}