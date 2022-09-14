namespace Konfur2022SparklingBot.Common.HostedServices;

public class PeriodicalBackgroundServiceParameters
{
    public ILoggerFactory LoggerFactory { get; }

    public PeriodicalBackgroundServiceParameters(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
    }
}

public abstract class PeriodicalBackgroundService : BackgroundService
{
    private readonly ILogger _logger;

    protected PeriodicalBackgroundService(PeriodicalBackgroundServiceParameters parameters)
        => _logger = parameters.LoggerFactory.CreateLogger(GetType());

    protected virtual TimeSpan Interval => TimeSpan.FromSeconds(1);

    protected abstract Task RunAsync(CancellationToken stoppingToken);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}