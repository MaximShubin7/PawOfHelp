// Services/CommentService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Comment;
using PawOfHelp.DTOs.Public;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CommentResponseDto> CreateCommentAsync(Guid senderId, CreateCommentDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new Exception("Рейтинг должен быть от 1 до 5");

        if (senderId == dto.RecipientId)
            throw new Exception("Нельзя оставить комментарий самому себе");

        var recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.RecipientId);
        if (recipient == null)
            throw new Exception("Получатель не найден");

        var existingComment = await _context.Comments
            .FirstOrDefaultAsync(c => c.SenderId == senderId && c.RecipientId == dto.RecipientId);
        if (existingComment != null)
            throw new Exception("Вы уже оставили комментарий этому пользователю");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Rating = dto.Rating,
            Description = dto.Description,
            SenderId = senderId,
            RecipientId = dto.RecipientId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);

        var recipientUser = await _context.Users.FindAsync(dto.RecipientId);
        if (recipientUser != null)
        {
            recipientUser.SumRating += dto.Rating;
            recipientUser.CountRating += 1;
        }

        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(senderId);

        return new CommentResponseDto
        {
            Id = comment.Id,
            Rating = comment.Rating,
            Description = comment.Description,
            Sender = new PublicProfileDto
            {
                Id = senderId,
                Name = sender?.Name ?? string.Empty
            },
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<CommentResponseDto> UpdateCommentAsync(Guid commentId, Guid userId, UpdateCommentDto dto)
    {
        var comment = await _context.Comments
            .Include(c => c.Sender)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
            throw new Exception("Комментарий не найден");

        if (comment.SenderId != userId)
            throw new Exception("Вы можете редактировать только свои комментарии");

        var oldRating = comment.Rating;

        if (dto.Rating.HasValue)
        {
            if (dto.Rating.Value < 1 || dto.Rating.Value > 5)
                throw new Exception("Рейтинг должен быть от 1 до 5");
            comment.Rating = dto.Rating.Value;
        }

        if (dto.Description != null)
            comment.Description = dto.Description;

        if (dto.Rating.HasValue && dto.Rating.Value != oldRating)
        {
            var recipient = await _context.Users.FindAsync(comment.RecipientId);
            if (recipient != null)
            {
                recipient.SumRating = recipient.SumRating - oldRating + dto.Rating.Value;
            }
        }

        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(comment.SenderId);

        return new CommentResponseDto
        {
            Id = comment.Id,
            Rating = comment.Rating,
            Description = comment.Description,
            Sender = new PublicProfileDto
            {
                Id = comment.SenderId,
                Name = sender?.Name ?? string.Empty
            },
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
            throw new Exception("Комментарий не найден");

        if (comment.SenderId != userId)
            throw new Exception("Вы можете удалять только свои комментарии");

        var recipient = await _context.Users.FindAsync(comment.RecipientId);
        if (recipient != null)
        {
            recipient.SumRating -= comment.Rating;
            recipient.CountRating -= 1;
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<CommentListResponseDto> GetUserCommentsAsync(Guid userId, int offset, int limit)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new Exception("Пользователь не найден");

        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.Comments
            .Include(c => c.Sender)
            .Where(c => c.RecipientId == userId)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync();

        var comments = await query
            .Skip(offset)
            .Take(limit)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Rating = c.Rating,
                Description = c.Description,
                Sender = new PublicProfileDto
                {
                    Id = c.SenderId,
                    Name = c.Sender != null ? c.Sender.Name : string.Empty
                },
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return new CommentListResponseDto
        {
            Comments = comments,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }
}