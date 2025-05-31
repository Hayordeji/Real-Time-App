using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data.Models
{
    public class AIChatHistory
    {
        [Key]
        public Guid Id { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public string? ChatHistory { get; set; }
    }
}
