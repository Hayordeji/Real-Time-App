using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class UserConnection
    {
        [Key]
        public int Id { get; set; }
        public string ChatRoom { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
