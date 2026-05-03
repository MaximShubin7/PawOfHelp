// Services/Interfaces/IResponseService.cs
using PawOfHelp.DTOs.Response;

namespace PawOfHelp.Services.Interfaces;

public interface IResponseService
{
    Task<ResponseResponseDto> CreateResponseAsync(Guid senderId, CreateResponseDto dto);
    Task<ResponseResponseDto> UpdateResponseStatusAsync(Guid responseId, Guid userId, UpdateResponseStatusDto dto);
    Task DeleteResponseAsync(Guid responseId, Guid userId);
    Task RemoveWorkerAsync(Guid taskId, Guid userId, Guid workerId);
    Task<ResponseListResponseDto> GetResponsesByTaskAsync(Guid taskId, Guid userId, int offset, int limit);
    Task<ResponseListResponseDto> GetResponsesByCreatorAsync(Guid creatorId, int offset, int limit);
    Task<ResponseListResponseDto> GetResponsesBySenderAsync(Guid senderId, int offset, int limit);
}