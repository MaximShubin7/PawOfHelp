// Services/Interfaces/IAvailabilityService.cs
namespace PawOfHelp.Services.Interfaces;

public interface IAvailabilityService
{
    Task<bool> IsUserAvailableForTaskAsync(Guid userId, DateTime startedAt, DateTime endedAt);
    Task<List<Guid>> FilterTasksByUserAvailabilityAsync(Guid userId, List<Guid> taskIds);
}