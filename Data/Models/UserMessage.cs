using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class UserMessage
    {
        [Key]
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;


    }
}
