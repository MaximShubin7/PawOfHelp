// Services/PostService.cs (исправленные методы)
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Post;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class PostService : IPostService
{
    private readonly AppDbContext _context;
    private readonly IImageKitService _imageKitService;

    public PostService(AppDbContext context, IImageKitService imageKitService)
    {
        _context = context;
        _imageKitService = imageKitService;
    }

    public async Task<PostResponseDto> CreatePostAsync(Guid organizationId, CreatePostDto dto)
    {
        var organization = await _context.OrganizationDetails
            .FirstOrDefaultAsync(o => o.UserId == organizationId);

        if (organization == null)
            throw new Exception("Организация не найдена");

        var post = new Post
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        if (dto.Photos != null && dto.Photos.Any())
        {
            foreach (var photo in dto.Photos)
            {
                var photoUrl = await _imageKitService.UploadImageAsync(photo, "posts", post.Id.ToString());
                if (photoUrl != null)
                {
                    _context.PostPhotos.Add(new PostPhoto
                    {
                        PostId = post.Id,
                        PhotoUrl = photoUrl
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        return await GetPostResponseAsync(post.Id);
    }

    public async Task<PostResponseDto> UpdatePostAsync(Guid postId, Guid organizationId, UpdatePostDto dto)
    {
        var post = await _context.Posts
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == postId && p.OrganizationId == organizationId);

        if (post == null)
            throw new Exception("Пост не найден или доступ запрещён");

        if (dto.Title != null)
            post.Title = dto.Title;

        if (dto.Description != null)
            post.Description = dto.Description;

        if (dto.NewPhotos != null && dto.NewPhotos.Any())
        {
            foreach (var photo in dto.NewPhotos)
            {
                var photoUrl = await _imageKitService.UploadImageAsync(photo, "posts", post.Id.ToString());
                if (photoUrl != null)
                {
                    _context.PostPhotos.Add(new PostPhoto
                    {
                        PostId = post.Id,
                        PhotoUrl = photoUrl
                    });
                }
            }
        }

        await _context.SaveChangesAsync();

        return await GetPostResponseAsync(post.Id);
    }

    public async Task DeletePostAsync(Guid postId, Guid organizationId)
    {
        var post = await _context.Posts
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == postId && p.OrganizationId == organizationId);

        if (post == null)
            throw new Exception("Пост не найден или доступ запрещён");

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }

    public async Task<PostListResponseDto> GetOrganizationPostsAsync(Guid organizationId, int offset, int limit)
    {
        var organizationExists = await _context.OrganizationDetails.AnyAsync(o => o.UserId == organizationId);
        if (!organizationExists)
            throw new Exception("Организация не найдена");

        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.Posts
            .Include(p => p.Photos)
            .Where(p => p.OrganizationId == organizationId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync();

        var posts = await query
            .Skip(offset)
            .Take(limit)
            .Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                PhotoUrls = p.Photos.Select(ph => ph.PhotoUrl).ToList(),
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new PostListResponseDto
        {
            Posts = posts,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<PostResponseDto?> GetLatestPostByOrganizationAsync(Guid organizationId)
    {
        var post = await _context.Posts
            .Include(p => p.Photos)
            .Where(p => p.OrganizationId == organizationId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        if (post == null)
            return null;

        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            PhotoUrls = post.Photos.Select(ph => ph.PhotoUrl).ToList(),
            CreatedAt = post.CreatedAt
        };
    }

    private async Task<PostResponseDto> GetPostResponseAsync(Guid postId)
    {
        var post = await _context.Posts
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            throw new Exception("Пост не найден");

        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            PhotoUrls = post.Photos.Select(ph => ph.PhotoUrl).ToList(),
            CreatedAt = post.CreatedAt
        };
    }
}