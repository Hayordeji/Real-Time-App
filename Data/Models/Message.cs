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

        //TO DO (USE THIS FOR AUTHENTICATION)
        public string? SenderId { get; set; }
        public string? SenderName { get; set; }

        public string? ReceipientId { get; set; }
        public string? ReceipientName { get; set; }

        //GROUP NAVIGATION
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }
        public Group? GroupDetails { get; set; }

        public List<MessageReceipient>? Receipients { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }


    }
}
