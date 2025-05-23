using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Message
{
    public class MessageGetDto
    {
        public Guid MessageId { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

    }
}
