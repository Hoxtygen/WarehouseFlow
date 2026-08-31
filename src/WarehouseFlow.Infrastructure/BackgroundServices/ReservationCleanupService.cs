using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Interfaces;

namespace WarehouseFlow.Infrastructure.BackgroundServices;

public sealed class ReservationCleanupService : BackgroundService
{
    private readonly ILogger<ReservationCleanupService> logger;
    private readonly IServiceProvider serviceProvider;
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(3);

    public ReservationCleanupService(
        ILogger<ReservationCleanupService> logger,
        IServiceProvider serviceProvider
    )
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Reservation cleanup service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                await orderService.ExpireReservationsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error occurred during reservation cleanup.");
            }

            await Task.Delay(CleanupInterval, stoppingToken);
        }

        logger.LogInformation("Reservation cleanup service stopping.");
    }
}
