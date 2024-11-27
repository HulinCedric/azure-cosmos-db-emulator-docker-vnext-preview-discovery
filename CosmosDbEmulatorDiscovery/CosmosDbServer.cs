using DotNet.Testcontainers.Builders;
using Microsoft.Azure.Cosmos;
using Testcontainers.CosmosDb;

namespace CosmosDbEmulatorDiscovery;

/// <seealso href="https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux" />
public class CosmosDbServer
{
    private readonly CosmosDbContainer _cosmosDbContainer = new CosmosDbBuilder()
        .WithPortBinding(8081, true)
        .WithPortBinding(1234, true)
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview")
        .WithEnvironment("PROTOCOL", "https")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Now listening on"))
        .Build();

    public async ValueTask DisposeAsync() => await _cosmosDbContainer.DisposeAsync();

    public CosmosClient CreateCosmosClient(Action<CosmosClientOptions>? clientOptions = null)
        => CosmosClientFactory.CreateCosmosClient(GetConnectionString(), clientOptions);

    public async Task StartAsync() => await _cosmosDbContainer.StartAsync();

    public async Task ResetAsync()
    {
        using var cosmosClient = CreateCosmosClient();

        await cosmosClient.ResetAllContainers();
    }

    public async Task InitializeAsync(Func<CosmosClient, Task> options)
    {
        using var client = CreateCosmosClient();

        await options(client);
    }

    /// <remarks>
    ///     To ignore SSL Certificate please suffix connectionstring with "DisableServerCertificateValidation=True;".
    ///     When CosmosClientOptions.HttpClientFactory is used, SSL certificate needs to be handled appropriately.
    ///     NOTE: DO NOT use the `DisableServerCertificateValidation` flag in production (only for emulator)
    /// </remarks>
    public string GetConnectionString()
        => $"{_cosmosDbContainer.GetConnectionString()};DisableServerCertificateValidation=true";
}