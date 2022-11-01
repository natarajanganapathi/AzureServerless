[assembly: FunctionsStartup(typeof(Incubation.AzConf.Startup))]

namespace Incubation.AzConf;
public class Startup : FunctionsStartup
{
    private bool IsDevelopment =>
        string.Equals(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
        .AddSingleton<DbContext>()
        .AddSingleton<JsonSerializer>()
        .AddScoped<ParticipantRepository>()
        .AddScoped<QuestionBankRepository>()
        .AddScoped<LeaderBoardRepository>()
        .AddScoped<CompositeService>()
        ;
        var context = builder.GetContext(); 
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        FunctionsHostBuilderContext context = builder.GetContext();
        var config = builder
            .ConfigurationBuilder
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true)
            .AddEnvironmentVariables()
            // .AddAzureKeyVault(new Uri(KeyVaultEndpointUrl), new DefaultAzureCredential())
            .Build();
    }

    // private static string KeyVaultEndpointUrl => "https://<YourKeyVaultName>.vault.azure.net";
}
