namespace Konfur2022SparklingBot.Common.HostedServices;

public abstract class PeriodicalBackgroundService : BackgroundService
{
    protected virtual TimeSpan Interval => TimeSpan.FromMinutes(1);

    protected abstract Task RunAsync(CancellationToken stoppingToken);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunAsync(stoppingToken);
            await Task.Delay(Interval, stoppingToken);
        }
    }
}