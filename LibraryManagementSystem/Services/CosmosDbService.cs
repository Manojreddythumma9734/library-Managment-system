using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using System.Collections.Generic;

public interface ICosmosDbService
{
    Task AddItemAsync<T>(T item, string containerName);
    Task<T> GetItemAsync<T>(string id, string containerName);
    Task<IEnumerable<T>> GetItemsAsync<T>(string query, string containerName);
    Task UpdateItemAsync<T>(string id, T item, string containerName);
}

public class CosmosDbService : ICosmosDbService
{
    private readonly CosmosClient _cosmosClient;
    private readonly Database _database;

    public CosmosDbService(string account, string key, string databaseName)
    {
        _cosmosClient = new CosmosClient(account, key);
        _database = _cosmosClient.GetDatabase(databaseName);
    }

    public async Task AddItemAsync<T>(T item, string containerName)
    {
        var container = _database.GetContainer(containerName);
        await container.CreateItemAsync(item, new PartitionKey(typeof(T).Name));
    }

    public async Task<T> GetItemAsync<T>(string id, string containerName)
    {
        var container = _database.GetContainer(containerName);
        var response = await container.ReadItemAsync<T>(id, new PartitionKey(typeof(T).Name));
        return response.Resource;
    }

    public async Task<IEnumerable<T>> GetItemsAsync<T>(string query, string containerName)
    {
        var container = _database.GetContainer(containerName);
        var iterator = container.GetItemQueryIterator<T>(new QueryDefinition(query));
        List<T> results = new List<T>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task UpdateItemAsync<T>(string id, T item, string containerName)
    {
        var container = _database.GetContainer(containerName);
        await container.UpsertItemAsync(item, new PartitionKey(typeof(T).Name));
    }
}
