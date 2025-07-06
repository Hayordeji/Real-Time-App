using Qdrant.Client.Grpc;
using Service.DTO.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IQdrantService
    {
        Task<bool> CreateCollection(string collectionName, uint vectorSize);
        Task AddVectorsToCollection(string collectionName, List<PointStruct> embeddings);
        //Task AddVectorsToCollection(string collectionName, List<OpenAIEmbeddingResponseDto> embeddings);

        Task<List<string>> SearchVector (string collectionName, List<float> vector, int limit = 5);

    }
}
