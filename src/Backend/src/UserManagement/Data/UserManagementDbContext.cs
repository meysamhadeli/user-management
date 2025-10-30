using BuildingBlocks.EFCore;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using UserManagement.Companies.Models;
using UserManagement.Industries.Models;
using UserManagement.Users.Models;

namespace UserManagement.Data;

public sealed class UserManagementDbContext : AppDbContextBase
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<UserManagementDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<Industry> Industries => Set<Industry>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}