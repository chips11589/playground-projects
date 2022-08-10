using Data;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

string databaseId = "changefeed-basic";
string monitoredContainerName = "monitored";
string partitionKeyPath = "/id";

IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

string connectionString = "AccountEndpoint=https://chips-cosmos.documents.azure.com:443/;AccountKey=kC8s84B0zcC0Sx70hmR4znRhUBQL702LXXBfZlwPBDXFHzHU84ptaUUhFCLvBQ9pCyP0zhaUHOYfGjI4dFUzcw==;";
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new ArgumentNullException("Missing ConnectionString");
}

using (CosmosClient client = new CosmosClient(connectionString))
{
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
    await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(monitoredContainerName, partitionKeyPath));

    Container monitoredContainer = client.GetContainer(databaseId, monitoredContainerName);

    while (true)
    {
        await GenerateItemsAsync(10, monitoredContainer);
        Console.WriteLine("Items generated...");
        await Task.Delay(1);
    }
}

static async Task GenerateItemsAsync(int itemsToInsert, Container container)
{
    for (int i = 0; i < itemsToInsert; i++)
    {
        string id = Guid.NewGuid().ToString();
        await container.CreateItemAsync(new ToDoItem()
        {
            id = id,
            creationTime = DateTime.UtcNow
        }, new PartitionKey(id));
    }
}