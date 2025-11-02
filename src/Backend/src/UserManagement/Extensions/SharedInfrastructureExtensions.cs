using System.Reflection;
using Aspire.ServiceDefaults;
using BuildingBlocks.EFCore;
using BuildingBlocks.OpenApi;
using BuildingBlocks.ProblemDetails;
using BuildingBlocks.Web;
using Figgle;
using FluentValidation;
using UserManagement.Data;
using UserManagement.Data.Seeds;

namespace UserManagement.Extensions;

public static class SharedInfrastructureExtensions
{
    public static WebApplicationBuilder AddSharedInfrastructure(this WebApplicationBuilder builder)
    {
        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.AddServiceDefaults();

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddAspnetOpenApi();
        builder.Services.AddCustomVersioning();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCustomMediatR(assembly: Assembly.GetExecutingAssembly());

        builder.AddMinimalEndpoints(assemblies: Assembly.GetExecutingAssembly());
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.AddCustomDbContext<UserManagementDbContext>(nameof(UserManagement));
        builder.Services.AddScoped<IDataSeeder, DataSeeder>();

        builder.Services.AddProblemDetails();

        builder.Services.AddCors(options =>
                                 {
                                     options.AddPolicy(
                                         name: appOptions.CorsPolicyName,
                                         policy =>
                                         {
                                             policy
                                                 .AllowAnyOrigin()
                                                 .AllowAnyHeader()
                                                 .AllowAnyMethod();
                                         });
                                 });

        return builder;
    }

    public static WebApplication UserSharedInfrastructure(this WebApplication app)
    {
        var appOptions = app.Configuration.GetOptions<AppOptions>(nameof(AppOptions));

        app.MapDefaultEndpoints();

        app.UseCustomProblemDetails();

        app.UseForwardedHeaders();

        app.UseCors(appOptions.CorsPolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (app.Environment.IsDevelopment())
        {
            app.UseAspnetOpenApi();
        }

        app.UseMigration<UserManagementDbContext>();

        app.MapMinimalEndpoints();

        return app;
    }
}