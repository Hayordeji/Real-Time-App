using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class GroupMember
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string? UserId { get; set; }
        public string? ConnectionId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
        // Navigation properties
        public Group Group { get; set; } = null!;

    }
}
