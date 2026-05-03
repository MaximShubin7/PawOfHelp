// Services/ResponseService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Public;
using PawOfHelp.DTOs.Response;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class ResponseService : IResponseService
{
    private readonly AppDbContext _context;

    public ResponseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseResponseDto> CreateResponseAsync(Guid senderId, CreateResponseDto dto)
    {
        var task = await _context.HelpTasks
            .FirstOrDefaultAsync(t => t.Id == dto.TaskId);

        if (task == null)
            throw new Exception("Задача не найдена");

        if (task.CreatorId == senderId)
            throw new Exception("Нельзя откликнуться на свою задачу");

        var existingResponse = await _context.Responses
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.SenderId == senderId && r.TaskId == dto.TaskId);

        if (existingResponse != null)
        {
            if (existingResponse.Status.Name == "Отклонен")
            {
                _context.Responses.Remove(existingResponse);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Вы уже откликнулись на эту задачу");
            }
        }

        var pendingStatus = await _context.Statuses
            .FirstOrDefaultAsync(s => s.Name == "На рассмотрении");

        if (pendingStatus == null)
            throw new Exception("Статус 'На рассмотрении' не найден");

        var response = new Response
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            TaskId = dto.TaskId,
            StatusId = pendingStatus.Id
        };

        _context.Responses.Add(response);
        task.CountResponses++;

        await _context.SaveChangesAsync();

        return await GetResponseByIdAsync(response.Id);
    }

    public async Task<ResponseResponseDto> UpdateResponseStatusAsync(Guid responseId, Guid userId, UpdateResponseStatusDto dto)
    {
        var response = await _context.Responses
            .Include(r => r.HelpTask)
            .Include(r => r.Sender)
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.Id == responseId);

        if (response == null)
            throw new Exception("Отклик не найден");

        if (response.HelpTask.CreatorId != userId)
            throw new Exception("Только создатель задачи может изменять статус отклика");

        var status = await _context.Statuses
            .FirstOrDefaultAsync(s => s.Name == dto.Status);

        if (status == null)
            throw new Exception($"Статус '{dto.Status}' не найден. Доступные статусы: 'На рассмотрении', 'Принят', 'Отклонен'");

        response.StatusId = status.Id;

        if (status.Name == "Принят")
        {
            var existingWorker = await _context.Workers
                .FirstOrDefaultAsync(w => w.TaskId == response.TaskId && w.UserId == response.SenderId);

            if (existingWorker == null)
            {
                _context.Workers.Add(new Worker
                {
                    TaskId = response.TaskId,
                    UserId = response.SenderId
                });
            }
        }
        else if (status.Name == "Отклонен")
        {
            var existingWorker = await _context.Workers
                .FirstOrDefaultAsync(w => w.TaskId == response.TaskId && w.UserId == response.SenderId);

            if (existingWorker != null)
            {
                _context.Workers.Remove(existingWorker);
            }
        }

        await _context.SaveChangesAsync();

        return await GetResponseByIdAsync(response.Id);
    }

    public async Task DeleteResponseAsync(Guid responseId, Guid userId)
    {
        var response = await _context.Responses
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.Id == responseId);

        if (response == null)
            throw new Exception("Отклик не найден");

        if (response.SenderId != userId)
            throw new Exception("Вы можете удалить только свой отклик");

        if (response.Status.Name == "Принят")
            throw new Exception("Нельзя удалить принятый отклик. Сначала открепитесь от задачи.");

        var task = await _context.HelpTasks.FindAsync(response.TaskId);
        if (task != null && task.CountResponses > 0)
        {
            task.CountResponses--;
        }

        _context.Responses.Remove(response);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveWorkerAsync(Guid taskId, Guid userId, Guid workerId)
    {
        var task = await _context.HelpTasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new Exception("Задача не найдена");

        if (task.CreatorId != userId && workerId != userId)
            throw new Exception("У вас нет прав для удаления этого исполнителя");

        var worker = await _context.Workers
            .FirstOrDefaultAsync(w => w.TaskId == taskId && w.UserId == workerId);

        if (worker == null)
            throw new Exception("Исполнитель не найден");

        var response = await _context.Responses
            .FirstOrDefaultAsync(r => r.TaskId == taskId && r.SenderId == workerId);

        if (response != null)
        {
            var rejectedStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.Name == "Отклонен");

            if (rejectedStatus != null)
            {
                response.StatusId = rejectedStatus.Id;
            }

            if (task.CountResponses > 0)
            {
                task.CountResponses--;
            }
        }

        _context.Workers.Remove(worker);
        await _context.SaveChangesAsync();
    }

    private async Task<ResponseResponseDto> GetResponseByIdAsync(Guid responseId)
    {
        var response = await _context.Responses
            .Include(r => r.Sender)
            .Include(r => r.HelpTask)
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.Id == responseId);

        if (response == null)
            throw new Exception("Отклик не найден");

        return new ResponseResponseDto
        {
            Id = response.Id,
            Sender = new PublicProfileDto
            {
                Id = response.Sender?.Id ?? Guid.Empty,
                Name = response.Sender?.Name ?? string.Empty
            },
            TaskId = response.TaskId,
            TaskTitle = response.HelpTask?.Title ?? string.Empty,
            Status = response.Status?.Name ?? "Неизвестно",
            CreatedAt = response.CreatedAt
        };
    }

    public async Task<ResponseListResponseDto> GetResponsesByTaskAsync(Guid taskId, Guid userId, int offset, int limit)
    {
        var task = await _context.HelpTasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null)
            throw new Exception("Задача не найдена");

        if (task.CreatorId != userId)
            throw new Exception("Вы не можете просматривать отклики на эту задачу");

        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.Responses
            .Where(r => r.TaskId == taskId);

        var totalCount = await query.CountAsync();

        var responses = await query
            .Include(r => r.Sender)
            .Include(r => r.Status)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var result = new List<ResponseResponseDto>();
        foreach (var response in responses)
        {
            result.Add(new ResponseResponseDto
            {
                Id = response.Id,
                Sender = new PublicProfileDto
                {
                    Id = response.Sender?.Id ?? Guid.Empty,
                    Name = response.Sender?.Name ?? string.Empty
                },
                TaskId = response.TaskId,
                TaskTitle = response.HelpTask?.Title ?? string.Empty,
                Status = response.Status?.Name ?? "Неизвестно",
                CreatedAt = response.CreatedAt
            });
        }

        return new ResponseListResponseDto
        {
            Responses = result,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<ResponseListResponseDto> GetResponsesByCreatorAsync(Guid creatorId, int offset, int limit)
    {
        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.Responses
            .Where(r => r.HelpTask.CreatorId == creatorId);

        var totalCount = await query.CountAsync();

        var responses = await query
            .Include(r => r.Sender)
            .Include(r => r.Status)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var result = new List<ResponseResponseDto>();
        foreach (var response in responses)
        {
            result.Add(new ResponseResponseDto
            {
                Id = response.Id,
                Sender = new PublicProfileDto
                {
                    Id = response.Sender?.Id ?? Guid.Empty,
                    Name = response.Sender?.Name ?? string.Empty
                },
                TaskId = response.TaskId,
                TaskTitle = response.HelpTask?.Title ?? string.Empty,
                Status = response.Status?.Name ?? "Неизвестно",
                CreatedAt = response.CreatedAt
            });
        }

        return new ResponseListResponseDto
        {
            Responses = result,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<ResponseListResponseDto> GetResponsesBySenderAsync(Guid senderId, int offset, int limit)
    {
        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.Responses
            .Where(r => r.SenderId == senderId);

        var totalCount = await query.CountAsync();

        var responses = await query
            .Include(r => r.Sender)
            .Include(r => r.HelpTask)
            .Include(r => r.Status)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var result = new List<ResponseResponseDto>();
        foreach (var response in responses)
        {
            result.Add(new ResponseResponseDto
            {
                Id = response.Id,
                Sender = new PublicProfileDto
                {
                    Id = response.Sender?.Id ?? Guid.Empty,
                    Name = response.Sender?.Name ?? string.Empty
                },
                TaskId = response.TaskId,
                TaskTitle = response.HelpTask?.Title ?? string.Empty,
                Status = response.Status?.Name ?? "Неизвестно",
                CreatedAt = response.CreatedAt
            });
        }

        return new ResponseListResponseDto
        {
            Responses = result,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }
}