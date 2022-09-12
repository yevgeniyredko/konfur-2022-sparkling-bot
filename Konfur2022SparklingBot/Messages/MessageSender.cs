using Konfur2022SparklingBot.DataAccess.User;

namespace Konfur2022SparklingBot.Messages;

public class MessageSender
{
    private readonly UserRepository _userRepository;

    public MessageSender(UserRepository userRepository) => _userRepository = userRepository;

    public async Task NotifyPairEndedAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task NotifyPairFoundAsync(User user1, User user2)
    {
        throw new NotImplementedException();
    }
}