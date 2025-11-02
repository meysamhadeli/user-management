using TestsShared.Fixtures;
using TestsShared.TestBases;
using UserManagement;
using UserManagement.Data;

namespace EndToEndTests;

[Collection(UserManagementEndToEndTestCollection.Name)]
public class UserManagementEndToEndTestBase(SharedFixture<Program, UserManagementDbContext> sharedFixture)
    : EndToEndTestBase<Program, UserManagementDbContext>(sharedFixture);