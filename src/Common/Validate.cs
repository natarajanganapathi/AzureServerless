namespace Incubation.AzConf.Common;
public static class Validate
{
    public static async Task<Boolean> HasRegistered(string email)
    {
        var client = new RestClient($"https://prod-azconfdev-api.azurewebsites.net/api/v1.0/Ticket/AttendeeValidation?EmailId={email}");
        var request = new RestRequest();
        request.AddHeader("x-api-key", "RwBsAG8AYgBhAGwAQQB6AEMAbwBuAGYARABlAHYA");
        var response = await client.GetAsync<Boolean>(request);
        return response;
    }
}