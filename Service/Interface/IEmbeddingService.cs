using Service.DTO.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IEmbeddingService
    {
        Task<OpenAIEmbeddingResponseDto> CreateEmbedding(string text, int dimensions);
        Task<List<OpenAIEmbeddingResponseDto>> CreateEmbeddings(List<string> texts, int dimensions);
        Task<List<string>> ChunkText();


    }
}
