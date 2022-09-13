using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;
using Konfur2022SparklingBot.Services;

namespace Konfur2022SparklingBot.Background;

public class MakeNewPairsBackgroundService : PeriodicalBackgroundService
{
    private readonly SparklingBotSettings _settings;
    private readonly PairRepository _pairRepository;
    private readonly UserRepository _userRepository;
    private readonly MessageSenderService _messageSenderService;

    public MakeNewPairsBackgroundService(
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
        return;
        var remainingPairsCount = _settings.MaxPairsCount - await _pairRepository.CountAsync();
        if (remainingPairsCount <= 0)
        {
            return;
        }

        while (remainingPairsCount > 0)
        {
            var users = await _userRepository.SelectAllAsync(UserState.WaitingForPair);
            if (users.Count < 2)
            {
                break;
            }

            var user = users.First();

            var matched = await _userRepository.SelectMatchedAsync(user);
            var secondUser = matched.FirstOrDefault();
            if (secondUser == null)
            {
                continue;
            }

            await _userRepository.UpdateStateAsync(user.Id, UserState.Pairing);
            await _userRepository.UpdateStateAsync(secondUser.Id, UserState.Pairing);
            await _pairRepository.CreateAsync(user.Id, secondUser.Id);
            await _messageSenderService.NotifyPairFoundAsync(user, secondUser);

            remainingPairsCount--;
        }
    }
}