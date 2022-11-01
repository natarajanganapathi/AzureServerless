namespace Incubation.AzConf.Common;
public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
    public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
    // you can also update your API info as well here
    public override OpenApiInfo Info { get; set; } = new OpenApiInfo { Title = "Azure Conference Event" };
}