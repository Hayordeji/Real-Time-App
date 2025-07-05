using LangChain.Splitters.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Qdrant.Client.Grpc;
using Service.DTO.Embedding;
using Service.Interface;
using Service.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientService _httpClient;
        public EmbeddingService(IConfiguration config, IHttpClientService httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<OpenAIEmbeddingResponseDto> CreateEmbedding(string text, int dimensions)
        {
            try
            {      
                //CREATE TEH REQUEST BODY
                var data = new EmbeddingCreateDto()
                {
                    input = text,
                    model = "text-embedding-3-small",
                    dimensions = dimensions
                };
                //SET THE AUTHORIZATION HEADER
                _httpClient.SetAuthorizationHeader("bearer", $"{_config["OpenAI:APIKey"]}");
                var response = await _httpClient.PostAsync<EmbeddingCreateDto>("https://api.openai.com/v1/embeddings", data);
                response.EnsureSuccessStatusCode();

                //READ THE RESPONSE CONTENT
                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedReponse = JsonConvert.DeserializeObject<OpenAIEmbeddingResponseDto>(responseContent);
                
                return deserializedReponse;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateEmbedding", ex);
            }
           
        }

        public async Task<List<string>> ChunkText()
        {

            string filePath = "C:\\Users\\ayode\\source\\repos\\AI_Chatbot\\Service\\Helpers\\MockDataSarahChen.txt";
            string data = File.ReadAllText(filePath);

            var textSplitter = new RecursiveCharacterTextSplitter(
            chunkSize: 450,
            chunkOverlap: 200,
            separators: new[] { "\n\n", "\n", " ", "" }
            );

            var chunks = textSplitter.SplitText(data);
            return chunks.ToList();
        }

        public async Task<List<OpenAIEmbeddingResponseDto>> CreateEmbeddings(List<string> texts, int dimensions)
        {
            try
            {
                List<OpenAIEmbeddingResponseDto> responses = new List<OpenAIEmbeddingResponseDto>();
                foreach (string text in texts)
                {
                    
                    //CREATE TEH REQUEST BODY
                    var data = new EmbeddingCreateDto()
                    {
                        input = text,
                        model = "text-embedding-3-small",
                        dimensions = dimensions
                    };

                    //SET THE AUTHORIZATION HEADER
                    _httpClient.SetAuthorizationHeader("bearer", $"{_config["OpenAI:APIKey"]}");
                    var response = await _httpClient.PostAsync<EmbeddingCreateDto>("https://api.openai.com/v1/embeddings", data);
                    response.EnsureSuccessStatusCode();

                    //READ THE RESPONSE CONTENT
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedReponse = JsonConvert.DeserializeObject<OpenAIEmbeddingResponseDto>(responseContent);
                    responses.Add(deserializedReponse);
                }

                return responses;

            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateEmbedding", ex);
            }
        }
    }
}
