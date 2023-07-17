using Microsoft.Extensions.Hosting;

namespace TT.Services;

public class DeliveryExpirationService : BackgroundService
{
    private readonly IDeliveryService _deliveryService;

    public DeliveryExpirationService(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _deliveryService.CheckAndExpireDeliveries();

            // Wait for a certain interval before running again
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}