using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.DataAccess.Pair;
using Konfur2022SparklingBot.DataAccess.User;
using Konfur2022SparklingBot.Messages;

namespace Konfur2022SparklingBot.Services;

public class MakeNewPairsService : PeriodicalBackgroundService
{
    private readonly SparklingBotSettings _settings;
    private readonly PairRepository _pairRepository;
    private readonly UserRepository _userRepository;
    private readonly MessageSender _messageSender;

    public MakeNewPairsService(
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
        var remainingPairsCount = _settings.MaxPairsCount - await _pairRepository.CountAsync();
        if (remainingPairsCount <= 0)
        {
            return;
        }

        while (remainingPairsCount > 0)
        {
            var users = await _userRepository.FindAllAsync(UserState.WaitingForPair);
            if (users.Count < 2)
            {
                break;
            }

            var user = users.First();

            var matched = await _userRepository.FindMatchedAsync(user);
            var secondUser = matched.FirstOrDefault();
            if (secondUser == null)
            {
                continue;
            }
                
            await _userRepository.ChangeStateAsync(user.Id, UserState.Pairing);
            await _userRepository.ChangeStateAsync(secondUser.Id, UserState.Pairing);
            await _pairRepository.CreateAsync(user.Id, secondUser.Id);
            await _messageSender.NotifyPairFoundAsync(user, secondUser);

            remainingPairsCount--;
        }
    }
}