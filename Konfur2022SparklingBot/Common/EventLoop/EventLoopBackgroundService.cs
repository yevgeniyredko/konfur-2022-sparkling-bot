using Konfur2022SparklingBot.Common.HostedServices;

namespace Konfur2022SparklingBot.Common.EventLoop;

public class EventLoopBackgroundService : PeriodicalBackgroundService
{
    private readonly EventLoop _eventLoop;

    public EventLoopBackgroundService(EventLoop eventLoop, PeriodicalBackgroundServiceParameters parameters) : base(parameters)
    {
        _eventLoop = eventLoop;
    }

    protected override TimeSpan Interval => TimeSpan.FromSeconds(1);

    protected override async Task RunAsync(CancellationToken stoppingToken) => await _eventLoop.ExecuteCurrentEventsAsync();
}