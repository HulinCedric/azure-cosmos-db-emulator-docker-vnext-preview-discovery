namespace CosmosDbEmulatorDiscovery;

public sealed class CosmosDbFixture : IAsyncLifetime
{
    private readonly CosmosDbServer _cosmosDb = new();

    public async Task InitializeAsync() => await _cosmosDb.StartAsync();

    public async Task DisposeAsync() => await _cosmosDb.DisposeAsync();

    public async Task ResetAsync() => await _cosmosDb.ResetAsync();

    public string GetConnectionString() => _cosmosDb.GetConnectionString();
}