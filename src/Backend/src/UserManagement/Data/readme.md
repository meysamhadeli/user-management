dotnet ef migrations add initial --context UserManagementDbContext -o "Data\Migrations"
dotnet ef database update --context UserManagementDbContext
