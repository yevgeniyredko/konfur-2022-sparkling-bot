using Konfur2022SparklingBot.Common.EventLoop;
using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;
using Konfur2022SparklingBot.Services;

namespace Konfur2022SparklingBot.Background;

public class MakeNewPairsBackgroundService : PeriodicalBackgroundService
{
    private readonly SparklingBotSettings _settings;
    private readonly EventLoop _eventLoop;
    private readonly PairRepository _pairRepository;
    private readonly UserRepository _userRepository;
    private readonly EventHandlerService _eventHandlerService;

    public MakeNewPairsBackgroundService(
        SparklingBotSettings settings,
        EventLoop eventLoop,
        PairRepository pairRepository,
        UserRepository userRepository,
        EventHandlerService eventHandlerService,
        PeriodicalBackgroundServiceParameters parameters)
        : base(parameters)
    {
        _settings = settings;
        _eventLoop = eventLoop;
        _pairRepository = pairRepository;
        _userRepository = userRepository;
        _eventHandlerService = eventHandlerService;
    }

    protected override async Task RunAsync(CancellationToken stoppingToken) => _eventLoop.Push(RunInternalAsync);

    private async Task RunInternalAsync()
    {
        var remainingPairsCount = _settings.MaxPairsCount - await _pairRepository.CountAsync(DateTime.UtcNow.Subtract(_settings.PairFinishTime));
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

            User? secondUser = null;
            foreach (var secondCandidate in matched)
            {
                if (await _pairRepository.SelectAsync(user.Id, secondCandidate.Id) != null)
                {
                    continue;
                }

                secondUser = secondCandidate;
                break;
            }

            if (secondUser == null)
            {
                continue;
            }

            await _eventHandlerService.HandlePairFoundAsync(user, secondUser);

            remainingPairsCount--;
        }
    }
}