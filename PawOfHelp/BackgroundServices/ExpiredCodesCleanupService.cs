// BackgroundServices/ExpiredCodesCleanupService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;

namespace PawOfHelp.BackgroundServices;

public class ExpiredCodesCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;

    public ExpiredCodesCleanupService(IServiceProvider services)
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

                var codesToDelete = await context.VerificationCodes
                    .Where(vc => vc.ExpiresAt < DateTime.UtcNow || vc.AttemptsLeft <= 0)
                    .ToListAsync(stoppingToken);

                if (codesToDelete.Any())
                {
                    context.VerificationCodes.RemoveRange(codesToDelete);
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception)
            {
                
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}