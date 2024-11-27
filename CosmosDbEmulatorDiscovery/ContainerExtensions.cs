using Microsoft.Azure.Cosmos;

namespace CosmosDbEmulatorDiscovery;

public static class ContainerExtensions
{
    /// <summary>
    ///     <c>ReadThroughputAsync</c> extension method that handle serverless accounts.
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static async Task<int?> ReadThroughputOrDefaultAsync(this Container container)
    {
        try
        {
            return await container.ReadThroughputAsync();
        }
        catch (CosmosException e) when (e.Message.Contains(
                                            "Reading or replacing offers is not supported for serverless accounts."))
        {
            return default;
        }
        catch (CosmosException e) when (e.Message.Contains("Have not implemented Query on Offer."))
        {
            return default;
        }
    }

    /// <summary>
    ///     Useful to quickly reset a container.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         WARNING: When used within the same <c>CosmosClient</c> instance, it may cause connection issues.
    ///     </para>
    ///     <para>
    ///         For example, the scenario :
    ///         <list type="number">
    ///             <item>New <c>CosmosClient</c> instance is created.</item>
    ///             <item>The container is reset.</item>
    ///             <item>
    ///                 The container <c>GetItemLinqQueryable</c>, <c>GetItemQueryIterator</c> or
    ///                 <c>GetItemQueryStreamIterator</c> is called.
    ///             </item>
    ///         </list>
    ///         Issue a <c>CosmosException</c> with the message "Stale cache for rid 'mdkxANr0XvI='".
    ///     </para>
    /// </remarks>
    /// <seealso href="https://github.com/Azure/azure-cosmos-dotnet-v3/issues/3092" />
    public static async Task ResetContainerAsync(this Container container)
    {
        var containerResponse = await container.ReadContainerAsync();
        var containerProperties = containerResponse.Resource;

        var throughput = await container.ReadThroughputOrDefaultAsync();

        await container.DeleteContainerAsync();
        await container.Database.CreateContainerAsync(containerProperties, throughput);
    }
}