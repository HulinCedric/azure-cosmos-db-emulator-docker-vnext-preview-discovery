using System.Text.Json;
using Microsoft.Azure.Cosmos;

namespace CosmosDbEmulatorDiscovery;

public static class CosmosClientFactory
{
    public static CosmosClient CreateCosmosClient(
        string connectionString,
        Action<CosmosClientOptions>? clientOptions = null)
    {
        var options = CosmosClientOptions();

        clientOptions?.Invoke(options);

        return new CosmosClient(connectionString, options);
    }

    private static CosmosClientOptions CosmosClientOptions()
        => new()
        {
            // only for emulator (High availability)
            LimitToEndpoint = true,

            // only for emulator (not performant (http vs tcp))
            ConnectionMode = ConnectionMode.Gateway,

            // not work with emulator
            //  AllowBulkExecution = true,
        };
}