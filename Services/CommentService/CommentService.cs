using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos.Comment;

namespace Backend.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<List<GetCommentDto>>> AddComment(AddCommentDto newComment, int postId)
        {
            var serviceResponse = new ServiceResponse<List<GetCommentDto>>();
            try{
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if(post is null){
                throw new Exception("Post not found.");
            }

            var comment = _mapper.Map<Comment>(newComment);
            comment.PostId = postId;
            comment.Post = post;
            comment.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            comment.Username = comment.User!.Username;
            comment.CreatedAt = DateTime.Now;
            
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Comments.Where(c => c.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCommentDto>(c)).ToListAsync();
            }catch(Exception ex){
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCommentDto>>> DeleteComment(int postId, int commentId)
        {
            var serviceResponse = new ServiceResponse<List<GetCommentDto>>();

            try{
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.User!.Id == GetUserId());

            if(comment is null){
                throw new Exception("Comment not found.");
            }

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Comments.Where(c => c.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCommentDto>(c)).ToListAsync();
            } catch(Exception ex){
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCommentDto>>> GetAllComments(int postId)
        {
            var serviceResponse = new ServiceResponse<List<GetCommentDto>>();
            var dbComments = await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
            serviceResponse.Data = dbComments.Select(c => _mapper.Map<GetCommentDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCommentDto>> UpdateComment(int postId, int commentId, UpdateCommentDto updatedComment)
        {
            var serviceResponse = new ServiceResponse<GetCommentDto>();

            try{
            var comment = await _context.Comments.Include(c => c.User).FirstAsync(c => c.Id == commentId);

            if(comment is null || comment.User!.Id != GetUserId()){
                throw new Exception("Comment not found.");
            }

            comment.Content = updatedComment.Content;

            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetCommentDto>(comment);
            } catch(Exception ex){
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCommentDto>> GetCommentById(int postId, int commentId)
        {
           var serviceResponse = new ServiceResponse<GetCommentDto>();

            try{
            var comment = await _context.Comments.Include(c => c.User).FirstAsync(c => c.Id == commentId);

            if(comment is null){
                throw new Exception("Comment not found.");
            }
            serviceResponse.Data = _mapper.Map<GetCommentDto>(comment);} catch(Exception ex){
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}