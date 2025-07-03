using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Embedding
{
    public class EmbeddingCreateDto
    {
        public string input { get; set; }
        public string model { get; set; }
        public int dimensions { get; set; }
    }
}
