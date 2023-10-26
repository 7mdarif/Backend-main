using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Services.PostService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("posts")]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> Get() 
        {
            return Ok(await _postService.GetAllPosts());
        }

        [HttpGet("posts/{postId}")]
        public async Task<ActionResult<ServiceResponse<GetPostDto>>> GetSingle(int postId) 
        {
            return Ok(await _postService.GetPostById(postId));
        }
        [Authorize]
        [HttpPost("post")]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> AddPost(AddPostDto newPost)
        {
            return Ok(await _postService.AddPost(newPost));
        }
        [Authorize]
        [HttpPut("post/{postId}")]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> UpdatePost(int postId, UpdatePostDto updatedPost)
        {
            var response  = await _postService.UpdatePost(postId, updatedPost);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [Authorize]
        [HttpDelete("post/{postId}")]
        public async Task<ActionResult<ServiceResponse<GetPostDto>>> DeletePost(int postId) 
        {
            var response  = await _postService.DeletePost(postId);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}