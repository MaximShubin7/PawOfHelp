using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;

namespace PawOfHelp.BackgroundServices;

public class RejectedResponsesCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;

    public RejectedResponsesCleanupService(IServiceProvider services)
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

                var rejectedResponses = await context.Responses
                    .Include(r => r.Status)
                    .Where(r => r.Status.Name == "Отклонен" && r.CreatedAt <= cutoffDate)
                    .ToListAsync(stoppingToken);

                if (rejectedResponses.Any())
                {
                    context.Responses.RemoveRange(rejectedResponses);
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