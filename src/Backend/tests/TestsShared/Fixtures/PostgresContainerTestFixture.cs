using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using TestsShared.Helpers;

namespace TestsShared.Fixtures;

public class PostgresContainerFixture : IAsyncLifetime
{
    public PostgresContainerOptions PostgresContainerOptions { get; }
    public PostgreSqlContainer PostgresContainer { get; }
    public int HostPort => PostgresContainer.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort);
    public string ConnectionString => PostgresContainer.GetConnectionString();
    public int TcpContainerPort => PostgreSqlBuilder.PostgreSqlPort;

    public PostgresContainerFixture()
    {
        PostgresContainerOptions = ConfigurationHelper.BindOptions<PostgresContainerOptions>();
        ArgumentNullException.ThrowIfNull(PostgresContainerOptions);

        var postgresContainerBuilder = new PostgreSqlBuilder()
            .WithDatabase(PostgresContainerOptions.DatabaseName)
            .WithCleanUp(true)
            .WithName(PostgresContainerOptions.Name)
            .WithImage(PostgresContainerOptions.ImageName);

        PostgresContainer = postgresContainerBuilder.Build();
    }

    public async Task InitializeAsync()
    {
        await PostgresContainer.StartAsync();
    }

    public async Task ResetDbAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(PostgresContainer.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            var checkpoint = await Respawner.CreateAsync(
                connection,
                new RespawnerOptions { DbAdapter = DbAdapter.Postgres }
            );

            // https://github.com/jbogard/Respawn/issues/108
            // https://github.com/jbogard/Respawn/pull/115 - fixed
            await checkpoint.ResetAsync(connection)!;
        }
        catch (Exception e)
        {
        }
    }

    public async Task DisposeAsync()
    {
        await PostgresContainer.StopAsync();
        await PostgresContainer.DisposeAsync();
    }
}

public sealed class PostgresContainerOptions
{
    public string Name { get; set; } = "postgres_" + Guid.NewGuid();
    public string ImageName { get; set; } = "postgres:latest";
    public string DatabaseName { get; set; } = "test_db";
}