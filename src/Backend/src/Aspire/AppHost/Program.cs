using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("docker-compose");

// 1. Database Services
var pgUsername = builder.AddParameter("pg-username", "postgres", secret: true);
var pgPassword = builder.AddParameter("pg-password", "postgres", secret: true);

var postgres = builder
    .AddPostgres("postgres", pgUsername, pgPassword)
    .WithImage("postgres:latest")
    .WithEndpoint(
        "tcp",
        e =>
        {
            e.Port = 5432;
            e.TargetPort = 5432;
            e.IsProxied = true;
            e.IsExternal = false;
        }
    )
    .WithArgs("-c", "wal_level=logical", "-c", "max_prepared_transactions=10");

if (builder.ExecutionContext.IsPublishMode)
{
    postgres.WithDataVolume("postgres-data").WithLifetime(ContainerLifetime.Persistent);
}

// 2. Apis
var userManagement = builder
    .AddProject<UserManagement>("user-management")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpEndpoint(port: 5001, name: "user-management-http")
    .WithHttpsEndpoint(port: 5000, name: "user-management-https");

await builder.Build().RunAsync();