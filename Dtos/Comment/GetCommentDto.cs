using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dtos.Comment
{
    public class GetCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; } 
        public string Username { get; set; } = string.Empty; 
    }
}