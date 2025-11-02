using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestsShared.Fixtures;

namespace TestsShared.TestBases;

public abstract class IntegrationTestBase<TEntryPoint, TContext> : IAsyncLifetime
    where TEntryPoint : class
    where TContext : DbContext
{
    private IServiceScope? _serviceScope;

    protected CancellationToken CancellationToken => CancellationToken.None;

    // Build Service Provider here
    protected IServiceScope Scope => _serviceScope ??= SharedFixture.ServiceProvider.CreateScope();
    protected SharedFixture<TEntryPoint, TContext> SharedFixture { get; }

    protected IntegrationTestBase(SharedFixture<TEntryPoint, TContext> sharedFixture)
    {
        SharedFixture = sharedFixture;
        CancellationToken.ThrowIfCancellationRequested();

        // we should not build a factory service provider with getting ServiceProvider in SharedFixture construction to having capability for override
        SharedFixture.WithTestConfigureServices(SetupTestConfigureServices);
        SharedFixture.WithTestConfigureAppConfiguration(
            (context, configurationBuilder) =>
            {
                SetupTestConfigureAppConfiguration(context, context.Configuration, context.HostingEnvironment);
            }
        );
        SharedFixture.WithTestConfiguration(SetupTestConfiguration);
        SharedFixture.AddOverrideEnvKeyValues(OverrideEnvKeyValues);
        SharedFixture.AddOverrideInMemoryConfig(OverrideInMemoryConfig);

        // Note: building service provider here or InitializeAsync
    }

    // we use IAsyncLifetime in xunit instead of constructor when we have an async operation
    public virtual async Task InitializeAsync() { }

    public virtual async Task DisposeAsync()
    {
        // cleanup data and messages in each test
        await SharedFixture.CleanupAsync(CancellationToken);

        Scope.Dispose();
    }

    protected virtual void SetupTestConfigureServices(IServiceCollection services) { }

    protected virtual void SetupTestConfigureAppConfiguration(
        WebHostBuilderContext webHostBuilderContext,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment
    )
    { }

    protected virtual void SetupTestConfiguration(IConfiguration configurations) { }

    protected virtual void OverrideEnvKeyValues(IDictionary<string, string> keyValues) { }

    protected virtual void OverrideInMemoryConfig(IDictionary<string, string> keyValues) { }
}