using Microsoft.EntityFrameworkCore;
using TestsShared.Fixtures;

namespace TestsShared.TestBases;

public class EndToEndTestBase<TEntryPoint, TContext>(SharedFixture<TEntryPoint, TContext> sharedFixture)
    : IntegrationTestBase<TEntryPoint, TContext>(sharedFixture)
    where TEntryPoint : class
    where TContext : DbContext;
