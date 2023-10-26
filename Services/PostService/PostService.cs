using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Data;

namespace Backend.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<ServiceResponse<List<GetPostDto>>> AddPost(AddPostDto newPost)
        {
            var serviceResponse = new ServiceResponse<List<GetPostDto>>();
            var post = _mapper.Map<Post>(newPost);
            post.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            post.UserId = GetUserId();
            post.Username = post.User!.Username;
            post.CreatedAt = DateTime.Now;
            
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Posts.Where(p => p.User!.Id == GetUserId()).Select(p => _mapper.Map<GetPostDto>(p)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetPostDto>>> DeletePost(int postId)
        {
            var serviceResponse = new ServiceResponse<List<GetPostDto>>();

            try{
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.User!.Id == GetUserId());

            if(post is null){
                throw new Exception("Post not found.");
            }

            var commentsToDelete = _context.Comments.Where(c => c.PostId == postId);

            _context.Comments.RemoveRange(commentsToDelete);
            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Posts.Where(p => p.User!.Id == GetUserId()).Select(p => _mapper.Map<GetPostDto>(p)).ToListAsync();
            } catch(Exception ex){
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetPostDto>>> GetAllPosts()
        {
            var serviceResponse = new ServiceResponse<List<GetPostDto>>();
            var dbPosts = await _context.Posts.ToListAsync();
            serviceResponse.Data = dbPosts.Select(p => _mapper.Map<GetPostDto>(p)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetPostDto>> GetPostById(int postId)
        {
            var serviceResponse = new ServiceResponse<GetPostDto>();
            var dbPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            serviceResponse.Data = _mapper.Map<GetPostDto>(dbPost);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetPostDto>> UpdatePost(int postId,UpdatePostDto updatedPost)
        {
            var serviceResponse = new ServiceResponse<GetPostDto>();

            try{
            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId);

            if(post is null || post.User!.Id != GetUserId()){
                throw new Exception("Post not found.");
            }

            post.Title = updatedPost.Title;
            post.Content = updatedPost.Content;

            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetPostDto>(post);
            } catch(Exception ex){
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
        
    }
}