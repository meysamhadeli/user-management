using BuildingBlocks.Web;
using UserManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedInfrastructure();

var app = builder.Build();

app.UserSharedInfrastructure();
app.MapMinimalEndpoints();

app.Run();

namespace UserManagement
{
    public partial class Program
    {
    }
}