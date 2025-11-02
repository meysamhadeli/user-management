using TestsShared.Fixtures;
using UserManagement;
using UserManagement.Data;

namespace EndToEndTests;

[CollectionDefinition(Name)]
public class UserManagementEndToEndTestCollection : ICollectionFixture<SharedFixture<Program, UserManagementDbContext>>
{
    public const string Name = "User Management EndToEnd Test";
}
