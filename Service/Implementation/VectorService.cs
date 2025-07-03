using Microsoft.Extensions.Logging;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using Service.Interface;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class VectorService : IVectorService
    {
        private readonly QdrantClient _client;
        private readonly ILogger<VectorService> _logger;
        public VectorService(QdrantClient client, ILogger<VectorService> logger)
        {
            _client = client;
            _logger = logger;
        }
        public async Task InitializeCollectionsAsync()
        {
            try
            {
                await _client.CreateCollectionAsync("testcollectionayodeji", new VectorParams
                {
                    Size = 312,
                    Distance = Distance.Cosine
                });
                _logger.LogInformation($"Created collection 'testcollectionayodeji' with vector size 312");
                await CreateCollectionIfNotExistAsync("documents", 384);
                await CreateCollectionIfNotExistAsync("images", 512);
                await CreateCollectionIfNotExistAsync("products", 768);

            }
            catch (Exception ex)
            {

            }
        }

        private async Task CreateCollectionIfNotExistAsync(string collectionName, uint vectorSize)
        {
            try
            {
                //THIS IS WHERE I STOPPED, I AM TRYING TO FIND IF COLLECTION EXISTS
                var collections = await _client.ListCollectionsAsync();
                if (collections.Contains(collectionName))
                {
                    _logger.LogInformation($"Collection {collectionName} already exists.");
                    return;
                }
                await _client.CreateCollectionAsync(collectionName, new VectorParams
                {
                    Size = vectorSize,
                    Distance = Distance.Cosine
                });
                _logger.LogInformation($"Created collection '{collectionName}' with vector size {vectorSize}");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create collection '{collectionName}'");
                throw;
            }
        }
    }
}
