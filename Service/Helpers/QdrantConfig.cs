using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public class QdrantConfig
    {
        public string Endpoint { get; set; }/*= "http://localhost:6333";*/
        public string? ApiKey { get; set; }
        public int Timeout { get; set; } = 30;
        public int MaxRetries { get; set; } = 3;
    }
}
