// Services/Interfaces/ICommentService.cs
using PawOfHelp.DTOs.Comment;

namespace PawOfHelp.Services.Interfaces;

public interface ICommentService
{
    Task<CommentResponseDto> CreateCommentAsync(Guid senderId, CreateCommentDto dto);
    Task<CommentResponseDto> UpdateCommentAsync(Guid commentId, Guid userId, UpdateCommentDto dto);
    Task DeleteCommentAsync(Guid commentId, Guid userId);
    Task<CommentListResponseDto> GetUserCommentsAsync(Guid userId, int offset, int limit);
}