using Data;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

string monitoredContainerName = "monitored";
string leasesContainerName = "leases";
string partitionKeyPath = "/id";
string databaseId = "changefeed-basic";

try
{
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

    string connectionString = "AccountEndpoint=https://chips-cosmos.documents.azure.com:443/;AccountKey=kC8s84B0zcC0Sx70hmR4znRhUBQL702LXXBfZlwPBDXFHzHU84ptaUUhFCLvBQ9pCyP0zhaUHOYfGjI4dFUzcw==;";
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new ArgumentNullException("Missing ConnectionString");
    }

    Console.WriteLine("Starting demo...");

    using (CosmosClient client = new CosmosClient(connectionString))
    {
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
        await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(monitoredContainerName, partitionKeyPath));
        await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(leasesContainerName, partitionKeyPath));

        Console.WriteLine("Containers created...");

        Container leaseContainer = client.GetContainer(databaseId, leasesContainerName);
        Container monitoredContainer = client.GetContainer(databaseId, monitoredContainerName);
        ChangeFeedProcessor changeFeedProcessor = monitoredContainer
            .GetChangeFeedProcessorBuilder<ToDoItem>("changeFeedBasic", HandleChangesAsync)
                .WithInstanceName(Environment.MachineName)
                .WithLeaseContainer(leaseContainer)
                .Build();
        Console.WriteLine($"Pod's name: {Environment.MachineName}");

        Console.WriteLine("Starting Change Feed Processor...");
        await changeFeedProcessor.StartAsync();
        Console.WriteLine("Change Feed Processor started.");

        while (true)
        {
            await Task.Delay(1000);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.WriteLine("End of demo.");
}

static async Task HandleChangesAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<ToDoItem> changes, CancellationToken cancellationToken)
{
    Console.WriteLine($"Started handling changes for lease {context.LeaseToken}...");
    Console.WriteLine($"Change Feed request consumed {context.Headers.RequestCharge} RU.");
    // SessionToken if needed to enforce Session consistency on another client instance
    Console.WriteLine($"SessionToken ${context.Headers.Session}");

    // We may want to track any operation's Diagnostics that took longer than some threshold
    if (context.Diagnostics.GetClientElapsedTime() > TimeSpan.FromSeconds(1))
    {
        Console.WriteLine($"Change Feed request took longer than expected. Diagnostics:" + context.Diagnostics.ToString());
    }

    foreach (ToDoItem item in changes)
    {
        Console.WriteLine($"\tDetected item with id {item.id}, created at {item.creationTime}.");
        // Simulate work
        await Task.Delay(10000);
    }
}