using Humanizer;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var pgUsername = builder.AddParameter("pg-username", "postgres", secret: true);
var pgPassword = builder.AddParameter("pg-password", "postgres", secret: true);

// 1. Database Services
var postgres = builder
    .AddPostgres("postgres")
    .WithImage("postgres:latest")
    .WithUserName(pgUsername)
    .WithPassword(pgPassword)
    .WithEndpoint(
        "tcp",
        e =>
        {
            e.Port = 5432;
            e.TargetPort = 5432;
            e.IsProxied = true;
            e.IsExternal = false;
        });

if (builder.ExecutionContext.IsPublishMode)
{
    postgres.WithDataVolume("postgres-data").WithLifetime(ContainerLifetime.Persistent);
}

var userManagementDb = postgres.AddDatabase(name: nameof(UserManagement).Kebaberize(), databaseName: "user_management_db");

// 2. Apis
var userManagementApi = builder
    .AddProject<UserManagement>("user-management-api")
    .WithReference(userManagementDb)
    .WaitFor(userManagementDb)
    .WithExternalHttpEndpoints()
    .WithEndpoint(
        "http",
        endpoint =>
        {
            endpoint.Port = 5001;
        })
    .WithEndpoint(
        "https",
        endpoint =>
        {
            endpoint.Port = 5000;
        });

// 3. Angular App
builder.AddNpmApp(
        name: "user-management-ui-app",
        workingDirectory: "../../../../Ui/user-management-ui",
        scriptName: "start:win")
    .WithReference(userManagementApi)
    .WaitFor(userManagementApi)
    .WithHttpEndpoint(
        env: "PORT",
        port: 4200,
        name: "user-management-ui-http")
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();