using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public string Username { get; set; } = string.Empty;
        public Post? Post { get; set; }
        public User? User { get; set; }
    }
}