using System.Text.Json;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using static FluentAssertions.FluentActions;

namespace CosmosDbEmulatorDiscovery;

public class ContainerShould : IClassFixture<CosmosDbFixture>, IAsyncLifetime
{
    private readonly string _connectionString;
    private Container _container = null!;

    public ContainerShould(CosmosDbFixture fixture) => _connectionString = fixture.GetConnectionString();

    public async Task InitializeAsync()
    {
        var client = CosmosClientFactory.CreateCosmosClient(
            _connectionString,
            options => options.UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

        Database database = await client.CreateDatabaseIfNotExistsAsync("database");
        _container = await database.CreateContainerIfNotExistsAsync("container", "/id");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_item()
        => await Invoking(async () => await _container.CreateItemAsync(new Item("1", "Hello")))
            .Should()
            .NotThrowAsync();

    [Fact]
    public async Task Create_item_with_diacritic()
        => await Invoking(async () => await _container.CreateItemAsync(new Item("2", "Halló")))
            .Should()
            .NotThrowAsync();

    private record Item(string id, string name);
}