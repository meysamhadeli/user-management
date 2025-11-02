using TestsShared.Fixtures;
using UserManagement;
using UserManagement.Data;

namespace IntegrationTests;

[CollectionDefinition(Name)]
public class UserManagementIntegrationTestCollection
    : ICollectionFixture<SharedFixture<Program, UserManagementDbContext>>
{
    public const string Name = "User Management Integration Test";
}
