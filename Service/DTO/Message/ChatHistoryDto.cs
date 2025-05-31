using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Message
{
    public class ChatHistoryDto
    {
       public List<ChatMessageDto> Messages { get; set; }
    }
}
