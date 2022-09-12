using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.DataAccess.Pair;
using Konfur2022SparklingBot.DataAccess.User;
using Konfur2022SparklingBot.Handlers;

namespace Konfur2022SparklingBot.Services;

public class BreakOutdatedPairsService : PeriodicalBackgroundService
{
    private readonly SparklingBotSettings _settings;
    private readonly PairRepository _pairRepository;
    private readonly UserRepository _userRepository;
    private readonly MessageSender _messageSender;

    public BreakOutdatedPairsService(
        SparklingBotSettings settings,
        PairRepository pairRepository,
        UserRepository userRepository,
        MessageSender messageSender)
    {
        _settings = settings;
        _pairRepository = pairRepository;
        _userRepository = userRepository;
        _messageSender = messageSender;
    }

    protected override async Task RunAsync(CancellationToken stoppingToken)
    {
        var pairs = await _pairRepository.FindBeforeAsync(DateTime.UtcNow.Add(_settings.PairTime));

        foreach (var pair in pairs)
        {
            await _messageSender.NotifyPairEndedAsync(pair.FirstUserId);
            await _messageSender.NotifyPairEndedAsync(pair.SecondUserId);
            await _userRepository.ChangeStateAsync(pair.FirstUserId, UserState.WaitingForPair);
            await _userRepository.ChangeStateAsync(pair.SecondUserId, UserState.WaitingForPair);
            await _pairRepository.DeleteAsync(pair.Id);
        }
    }
}