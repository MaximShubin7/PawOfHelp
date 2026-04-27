// Services/Interfaces/IPostService.cs
using PawOfHelp.DTOs.Post;

namespace PawOfHelp.Services.Interfaces;

public interface IPostService
{
    Task<PostResponseDto> CreatePostAsync(Guid organizationId, CreatePostDto dto);
    Task<PostResponseDto> UpdatePostAsync(Guid postId, Guid organizationId, UpdatePostDto dto);
    Task DeletePostAsync(Guid postId, Guid organizationId);
    Task<PostListResponseDto> GetOrganizationPostsAsync(Guid organizationId, int offset, int limit);
    Task<PostResponseDto?> GetLatestPostByOrganizationAsync(Guid organizationId);
}