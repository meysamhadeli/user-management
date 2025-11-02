using TestsShared.Fixtures;
using TestsShared.TestBases;
using UserManagement;
using UserManagement.Data;

namespace IntegrationTests;

[Collection(UserManagementIntegrationTestCollection.Name)]
public class UserManagementIntegrationTestBase(
    SharedFixture<Program, UserManagementDbContext> sharedFixture
) : IntegrationTestBase<Program, UserManagementDbContext>(sharedFixture);
