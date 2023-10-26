using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Dtos.Comment;
using Backend.Services.CommentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("post/{postId}/comments")]
        public async Task<ActionResult<ServiceResponse<List<GetCommentDto>>>> Get(int postId) 
        {
            return Ok(await _commentService.GetAllComments(postId));
        }

        [HttpGet("post/{postId}/comment/{commentId}")]
        public async Task<ActionResult<ServiceResponse<GetCommentDto>>> GetComment(int postId, int commentId) 
        {
            return Ok(await _commentService.GetCommentById(postId, commentId));
        }

        [Authorize]
        [HttpPost("post/{postId}/comment")]
        public async Task<ActionResult<ServiceResponse<List<GetCommentDto>>>> AddComment(AddCommentDto newComment, int postId)
        {
            return Ok(await _commentService.AddComment(newComment, postId));
        }
        [Authorize]
        [HttpPut("post/{postId}/comment/{commentId}")]
        public async Task<ActionResult<ServiceResponse<List<GetCommentDto>>>> UpdateComment(int postId, int commentId, UpdateCommentDto updatedComment)
        {
            var response  = await _commentService.UpdateComment(postId, commentId, updatedComment);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [Authorize]
        [HttpDelete("post/{postId}/{commentId}")]
        public async Task<ActionResult<ServiceResponse<GetCommentDto>>> DeleteComment(int postId, int commentId) 
        {
            var response  = await _commentService.DeleteComment(postId, commentId);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}