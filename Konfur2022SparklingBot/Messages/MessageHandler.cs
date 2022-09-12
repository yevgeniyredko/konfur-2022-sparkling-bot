using Konfur2022SparklingBot.DataAccess.Pair;
using Konfur2022SparklingBot.DataAccess.User;

namespace Konfur2022SparklingBot.Messages;

public class MessageHandler
{
    private readonly UserRepository _userRepository;
    private readonly PairRepository _pairRepository;

    public MessageHandler(UserRepository userRepository, PairRepository pairRepository)
    {
        _userRepository = userRepository;
        _pairRepository = pairRepository;
    }

    public async Task HandleUserAddedAsync()
    {
        throw new NotImplementedException();
    }

    public async Task HandleNameSetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task HandleQuestion1AnsweredAsync()
    {
        throw new NotImplementedException();
    }

    public async Task HandleQuestion2AnsweredAsync()
    {
        throw new NotImplementedException();
    }

    public async Task HandleQuestion3AnsweredAsync()
    {
        throw new NotImplementedException();
    }

    public async Task HandleAcceptPairAsync()
    {
        throw new NotImplementedException();
    }

    public async Task HandleRejectPairAsync()
    {
        throw new NotImplementedException();
    }
    
    public async Task HandlePairEndRequestAsync()
    {
        throw new NotImplementedException();
    }
}