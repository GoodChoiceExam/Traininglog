namespace FitLife.TrainingLog.Api.Services;

// BackgroundService der logger "Heartbeat" hvert minut så vi kan se i Grafana at containeren lever
public class HeartbeatService(ILogger<HeartbeatService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Heartbeat");
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
