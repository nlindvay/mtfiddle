using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mtfiddle;

public class MongoDbProvider
{
    public static readonly string Receives = "Receives";
    public static readonly string Orders = "Orders";
    public static readonly string Transactions = "Transactions";
    public static readonly string Events = "Events";

    protected readonly IMongoDatabase _db;

    protected readonly ILogger<MongoDbProvider> _logger;

    public MongoDbProvider(IMongoDatabase db, ILogger<MongoDbProvider> logger)
    {
        _db = db;
        _logger = logger;
    }

    public void Initialize()
    {
        _logger.LogInformation($"Initialize collections...");
        GetCollection<Receive>(Receives);
        GetCollection<Order>(Orders);
        GetCollection<Transaction>(Transactions);
        GetCollection<Event>(Events);
    }


    public IMongoCollection<T> GetCollection<T>(string name)
    {
        if (!_db.ListCollectionNames(new ListCollectionNamesOptions { Filter = new BsonDocument("name", name) }).Any())
        {
            _logger.LogInformation("Create collection: {Name}", name);
            _db.CreateCollection(name);
        }

        _logger.LogInformation("Get collection: {Name}", name);
        return _db.GetCollection<T>(name);
    }

}