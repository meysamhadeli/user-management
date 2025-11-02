using UserManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedInfrastructure();

var app = builder.Build();

app.UserSharedInfrastructure();

app.Run();

namespace UserManagement
{
    public partial class Program { }
}
