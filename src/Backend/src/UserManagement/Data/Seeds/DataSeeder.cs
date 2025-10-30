using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Data.Seeds;

public class DataSeeder : IDataSeeder
{
    private readonly UserManagementDbContext _userManagementDbContext;

    public DataSeeder(UserManagementDbContext userManagementDbContext)
    {
        _userManagementDbContext = userManagementDbContext;
    }

    public async Task SeedAllAsync()
    {
        var pendingMigrations = await _userManagementDbContext.Database.GetPendingMigrationsAsync();

        if (!pendingMigrations.Any())
        {
            await SeedIndustriesAsync();
            await SeedCompaniesAsync();
        }
    }

    private async Task SeedIndustriesAsync()
    {
        if (!await _userManagementDbContext.Industries.AnyAsync())
        {
            await _userManagementDbContext.Industries.AddRangeAsync(InitialData.Industries);
            await _userManagementDbContext.SaveChangesAsync();
        }
    }

    private async Task SeedCompaniesAsync()
    {
        if (!await _userManagementDbContext.Companies.AnyAsync())
        {
            await _userManagementDbContext.Companies.AddRangeAsync(InitialData.Companies);
            await _userManagementDbContext.SaveChangesAsync();
        }
    }
}
