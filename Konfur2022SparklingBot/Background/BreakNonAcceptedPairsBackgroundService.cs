using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;
using Konfur2022SparklingBot.Services;

namespace Konfur2022SparklingBot.Background;

public class BreakNonAcceptedPairsBackgroundService : PeriodicalBackgroundService
{
    private readonly SparklingBotSettings _settings;
    private readonly PairRepository _pairRepository;
    private readonly UserRepository _userRepository;
    private readonly MessageSenderService _messageSenderService;

    public BreakNonAcceptedPairsBackgroundService(
        SparklingBotSettings settings,
        PairRepository pairRepository,
        UserRepository userRepository,
        MessageSenderService messageSenderService,
        PeriodicalBackgroundServiceParameters parameters)
        : base(parameters)
    {
        _settings = settings;
        _pairRepository = pairRepository;
        _userRepository = userRepository;
        _messageSenderService = messageSenderService;
    }

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        var pairs = await _pairRepository.SelectCreatedNonStartedBeforeAsync(DateTime.UtcNow.Subtract(_settings.PairAcceptTtl));

        foreach (var pair in pairs)
        {
            await _messageSenderService.NotifyPairCanceledAsync(pair.FirstUserId);
            await _messageSenderService.NotifyPairCanceledAsync(pair.SecondUserId);
            await _userRepository.UpdateStateAsync(pair.FirstUserId, UserState.WaitingForPair);
            await _userRepository.UpdateStateAsync(pair.SecondUserId, UserState.WaitingForPair);
            await _pairRepository.DeleteAsync(pair.Id);
        }
    }
}