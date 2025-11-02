using BuildingBlocks.Web;
using Microsoft.Extensions.Configuration;

namespace TestsShared.Helpers;

public static class ConfigurationHelper
{
    private static readonly IConfigurationRoot ConfigurationRoot;

    static ConfigurationHelper()
    {
        ConfigurationRoot = BuildConfiguration();
    }

    public static TOptions BindOptions<TOptions>()
        where TOptions : new()
    {
        return ConfigurationRoot.GetOptions<TOptions>(typeof(TOptions).Name);
    }

    public static IConfigurationRoot BuildConfiguration()
    {
        var rootPath = Directory.GetCurrentDirectory();

        return new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}