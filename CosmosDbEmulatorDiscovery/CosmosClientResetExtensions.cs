using Microsoft.Azure.Cosmos;

namespace CosmosDbEmulatorDiscovery;

public static class CosmosClientResetExtensions
{
    public static async Task ResetAllContainers(this CosmosClient cosmosClient)
    {
        var databases = await Databases(cosmosClient);

        var containers = await Containers(databases);

        foreach (var container in containers)
        {
            await container.ResetContainerAsync();
        }
    }

    private static async Task<List<Database>> Databases(CosmosClient cosmosClient)
    {
        var databases = new List<Database>();

        using var databaseIterator = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
        while (databaseIterator.HasMoreResults)
        {
            foreach (var databaseProperties in await databaseIterator.ReadNextAsync())
            {
                databases.Add(cosmosClient.GetDatabase(databaseProperties.Id));
            }
        }

        return databases;
    }

    private static async Task<List<Container>> Containers(List<Database> databases)
    {
        var containers = new List<Container>();
        foreach (var database in databases)
        {
            using var containerIterator = database.GetContainerQueryIterator<ContainerProperties>();
            while (containerIterator.HasMoreResults)
            {
                foreach (var containerProperties in await containerIterator.ReadNextAsync())
                {
                    containers.Add(database.GetContainer(containerProperties.Id));
                }
            }
        }

        return containers;
    }
}