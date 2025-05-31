using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Message
{
    public class ChatMessageDto
    {
        public AuthorRole Role { get; set; }
        public string Content { get; set; }
    }
}
