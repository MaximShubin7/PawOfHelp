// Services/AvailabilityService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly AppDbContext _context;

    public AvailabilityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsUserAvailableForTaskAsync(Guid userId, DateTime startedAt, DateTime endedAt)
    {
        var userAvailabilities = await _context.UserAvailabilities
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Availability)
            .Select(ua => ua.Availability.Name)
            .ToListAsync();

        var taskDayOfWeek = startedAt.DayOfWeek.ToString();
        var taskHour = startedAt.Hour;

        bool isDayAvailable = userAvailabilities.Contains(taskDayOfWeek);

        string timeOfDay = taskHour switch
        {
            >= 6 and < 12 => "Утро",
            >= 12 and < 18 => "День",
            >= 18 and < 24 => "Вечер",
            _ => "Ночь"
        };

        bool isTimeAvailable = userAvailabilities.Contains(timeOfDay);

        return isDayAvailable && isTimeAvailable;
    }

    public async Task<List<Guid>> FilterTasksByUserAvailabilityAsync(Guid userId, List<Guid> taskIds)
    {
        var userAvailabilities = await _context.UserAvailabilities
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Availability)
            .Select(ua => ua.Availability.Name)
            .ToListAsync();

        var availableTasks = new List<Guid>();

        foreach (var taskId in taskIds)
        {
            var task = await _context.HelpTasks
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) continue;

            var taskDayOfWeek = task.StartedAt.DayOfWeek.ToString();
            var taskHour = task.StartedAt.Hour;

            bool isDayAvailable = userAvailabilities.Contains(taskDayOfWeek);

            string timeOfDay = taskHour switch
            {
                >= 6 and < 12 => "Утро",
                >= 12 and < 18 => "День",
                >= 18 and < 24 => "Вечер",
                _ => "Ночь"
            };

            bool isTimeAvailable = userAvailabilities.Contains(timeOfDay);

            if (isDayAvailable && isTimeAvailable)
            {
                availableTasks.Add(taskId);
            }
        }

        return availableTasks;
    }
}