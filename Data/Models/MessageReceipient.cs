﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class MessageReceipient
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public string? ReceipientId { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public string? ReceipientName { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        // Navigation properties
        public virtual Message Message { get; set; } = null!;
    }
}
