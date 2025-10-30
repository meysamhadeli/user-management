using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserManagement.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserManagementDbContext>
    {
        public UserManagementDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<UserManagementDbContext>();

            builder.UseNpgsql("Server=localhost;Port=5432;Database=user_management_db;User Id=postgres;Password=postgres;Include Error Detail=true")
                .UseSnakeCaseNamingConvention();
            return new UserManagementDbContext(builder.Options);
        }
    }
}