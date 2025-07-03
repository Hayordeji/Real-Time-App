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
        Task<OpenAIEmbeddingResponseDto> CreateEmbedding(List<string> texts, int dimensions);

    }
}
