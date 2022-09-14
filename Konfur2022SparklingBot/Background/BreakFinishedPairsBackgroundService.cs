using Konfur2022SparklingBot.Common.EventLoop;
using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Services;

namespace Konfur2022SparklingBot.Background;

public class BreakFinishedPairsBackgroundService : PeriodicalBackgroundService
{
    private readonly SparklingBotSettings _settings;
    private readonly EventLoop _eventLoop;
    private readonly PairRepository _pairRepository;
    private readonly EventHandlerService _eventHandlerService;

    public BreakFinishedPairsBackgroundService(
        SparklingBotSettings settings,
        EventLoop eventLoop,
        PairRepository pairRepository,
        EventHandlerService eventHandlerService,
        PeriodicalBackgroundServiceParameters parameters)
        : base(parameters)
    {
        _settings = settings;
        _eventLoop = eventLoop;
        _pairRepository = pairRepository;
        _eventHandlerService = eventHandlerService;
    }

    protected override async Task RunAsync(CancellationToken stoppingToken) => _eventLoop.Push(RunInternalAsync);

    private async Task RunInternalAsync()
    {
        var pairs = await _pairRepository.SelectStartedBeforeAsync(DateTime.UtcNow.Subtract(_settings.PairTime));

        foreach (var pair in pairs)
        {
            await _eventHandlerService.HandleFinishedPairAsync(pair);
        }
    }
}