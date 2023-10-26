using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dtos.Comment
{
    public class UpdateCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }
}