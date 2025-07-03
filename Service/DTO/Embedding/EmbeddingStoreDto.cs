using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Embedding
{
    public class EmbeddingStoreDto
    {
        public Guid Id { get; set; } = new Guid();
        public List<float> Vector { get; set; }
        public List<Dictionary<string, object>>? Payload { get; set; }
    }
}
