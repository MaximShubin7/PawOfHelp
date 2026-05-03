using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;

namespace PawOfHelp.BackgroundServices;

public class ExpiredTasksCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;

    public ExpiredTasksCleanupService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var cutoffDate = DateTime.UtcNow.AddDays(-7);

                var expiredTasks = await context.HelpTasks
                    .Where(t => t.EndedAt <= cutoffDate)
                    .ToListAsync(stoppingToken);

                if (expiredTasks.Any())
                {
                    context.HelpTasks.RemoveRange(expiredTasks);
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception)
            {
                
            }

            await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
        }
    }
}