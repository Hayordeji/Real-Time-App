using Qdrant.Client.Grpc;
using Service.DTO.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapper
{
    public static class EmbeddingStoreMapper
    {
        public static List<PointStruct> ToEmbeddingStoreDto(this OpenAIEmbeddingResponseDto body)
        {

            //return new EmbeddingStoreDto
            //{
            //    Id = Guid.NewGuid(),
            //    Vector = body.Data.FirstOrDefault()?.embedding ?? new List<float>(),
            //    Payload = new List<Dictionary<string, object>>
            //    {
            //        new Dictionary<string, object>
            //        {
            //            ["username"] = "ayodeji"
            //        }
            //    }
            //};
            var newEmbedding = new Embedding();
            newEmbedding = body.Data.FirstOrDefault();
            var vector = newEmbedding.embedding.ToArray();
            
            return new List<PointStruct>
            {
                new PointStruct
                {
                    Id = Guid.NewGuid(),
                    Vectors = vector,
                    Payload =
                    {
                        ["username"] = "Shoga",
                        ["age"] = "352"

                    }
                }
                
            };
        }

        public static List<PointStruct> ToEmbeddingsStoreDto(this List<OpenAIEmbeddingResponseDto> body)
        {

            List<PointStruct> points = new List<PointStruct>();
           
            foreach (var dto in body)
            {
                var newEmbedding = new Embedding();
                newEmbedding = dto.Data.FirstOrDefault();
                var vector = newEmbedding.embedding.ToArray();
                var newPoint = new PointStruct
                {
                    Id = Guid.NewGuid(),
                    Vectors = vector,
                    Payload =
                    {
                        ["description"] = "Testing the list",
                    }
                };
                points.Add(newPoint);
            }


            return points;
        }
    }
}
