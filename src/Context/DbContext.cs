namespace Incubation.AzConf.Context;
public class DbContext
{
    private readonly IMongoDatabase _database;
    public DbContext(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionString"];
        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        var client = new MongoClient(settings);
        var mongoDatabase = configuration["DatabaseName"];
        _database = client.GetDatabase(mongoDatabase);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}