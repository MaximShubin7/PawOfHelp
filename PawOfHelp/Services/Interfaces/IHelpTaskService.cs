// Services/Interfaces/IHelpTaskService.cs
using PawOfHelp.DTOs.HelpTask;

namespace PawOfHelp.Services.Interfaces;

public interface IHelpTaskService
{
    Task<HelpTaskResponseDto> CreateHelpTaskAsync(Guid creatorId, CreateHelpTaskDto dto);
    Task<HelpTaskResponseDto> UpdateHelpTaskAsync(Guid taskId, Guid userId, UpdateHelpTaskDto dto);
    Task DeleteHelpTaskAsync(Guid taskId, Guid userId);
    Task<HelpTaskResponseDto> GetHelpTaskByIdAsync(Guid taskId);
    Task<HelpTaskListResponseDto> GetFeedTasksAsync(Guid userId, HelpTaskFilterDto filter, int offset, int limit);
    Task<HelpTaskListResponseDto> GetTasksByCreatorAsync(Guid creatorId, int offset, int limit);
    Task<HelpTaskListResponseDto> GetTasksByWorkerAsync(Guid workerId, int offset, int limit);
    Task CompleteAndDeleteTaskAsync(Guid taskId, Guid userId);
}