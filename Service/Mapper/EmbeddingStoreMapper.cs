using Qdrant.Client.Grpc;
using Service.DTO.Embedding;


namespace Service.Mapper
{
    public static class EmbeddingStoreMapper
    {
        public static PointStruct ToEmbeddingStoreDto(this OpenAIEmbeddingResponseDto body, string text)
        {

            var newEmbedding = new Embedding();
            newEmbedding = body.Data.FirstOrDefault();
            var vector = newEmbedding.embedding.ToArray();
            
            return new PointStruct
                {
                    Id = Guid.NewGuid(),
                    Vectors = vector,
                    Payload =
                    {
                        ["text"] = text
                    }   
            };
        }

        public static List<float> ToEmbeddingQueryDto(this OpenAIEmbeddingResponseDto body)
        {

            var newEmbedding = new Embedding();
            newEmbedding = body.Data.FirstOrDefault();
            var vector = newEmbedding.embedding.ToArray();
               
            return vector.ToList();
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
