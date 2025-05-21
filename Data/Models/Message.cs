using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public string? SenderId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }


    }
}
