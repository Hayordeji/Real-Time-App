using Qdrant.Client.Grpc;
using Qdrant.Client;
using Service.Interface;
using Grpc.Net.Client;
using Grpc.Core.Interceptors;
using Service.DTO.Embedding;
using Service.Mapper;
using System;
using LangChain.Splitters.Text;

namespace Service.Implementation
{
    public class QdrantService : IQdrantService
    {
        private readonly QdrantClient _client;
        private readonly IEmbeddingService _embeddingService;
        public QdrantService(QdrantClient client, IEmbeddingService embeddingService)
        {
            _client = client;
            _embeddingService = embeddingService;
            var channel = GrpcChannel.ForAddress($"https://localhost:6334", new GrpcChannelOptions
            {
                HttpHandler = new WinHttpHandler
                {
                    ServerCertificateValidationCallback =
                    CertificateValidation.Thumbprint("<certificate thumbprint>")
                }
            });
            var callInvoker = channel.Intercept(metadata =>
            {
                metadata.Add("api-key", "<api key>");
                return metadata;
            });

            var grpcClient = new QdrantGrpcClient(callInvoker);
        }

        public async Task AddVectorsToCollection(string collectionName,List<PointStruct> embeddings)
        {
            try
            {
                //var  mappedEmbedding = embeddings.ToEmbeddingStoreDto();
                await _client.UpsertAsync(collectionName, embeddings);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        

        //public async Task AddVectorsToCollection(string collectionName, List<PointStruct> embeddings)
        //{
        //    try
        //    {
        //        var mappedEmbeddings = embeddings.ToEmbeddingsStoreDto();
        //        await _client.UpsertAsync(collectionName, mappedEmbeddings);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<bool> CreateCollection(string collectionName, uint vectorSize)
        {
            try
            {
                await _client.CreateCollectionAsync(collectionName, new VectorParams
                {
                    Size = vectorSize,
                    Distance = Distance.Cosine
                });

                return true;
            }
            catch (Exception)
            {

                return false ;
            }
            
        }

        public async Task<List<string>> SearchVector(string collectionName, List<float> vector, int limit)
        {
            float[] vectorArray = vector.ToArray(); // Convert List<float> to ReadOnlyMemory<float>
            ReadOnlyMemory<float> queryVector = vectorArray;

            // return the 5 closest points
            var points = await _client.SearchAsync(
              collectionName,
              queryVector,
              limit : (ulong)limit,
              scoreThreshold : 0.3f // Optional: Set a score threshold for filtering results
              );
            List<string> texts = new List<string>();
            foreach (ScoredPoint point in points)
            {
                texts.Add(point.Payload["text"].StringValue);
            }

            return texts;
        }


       
    }
}
