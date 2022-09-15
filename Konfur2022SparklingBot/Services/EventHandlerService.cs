using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;

namespace Konfur2022SparklingBot.Services;

public class EventHandlerService
{
    private readonly MessageSenderService _messageSenderService;
    private readonly UserRepository _userRepository;
    private readonly PairRepository _pairRepository;

    public EventHandlerService(
        MessageSenderService messageSenderService,
        UserRepository userRepository,
        PairRepository pairRepository)
    {
        _messageSenderService = messageSenderService;
        _userRepository = userRepository;
        _pairRepository = pairRepository;
    }

    public async Task HandleErrorAsync(long chatId)
    {
        await _messageSenderService.SendInvalidMessageAsync(chatId);
    }

    public async Task HandleUserAddedAsync(string username, long chatId, string name)
    {
        var user = await _userRepository.SelectAsync(username) ?? new User {Id = username};
        if (user.State is (UserState.Swimming or UserState.Pairing))
        {
            await _messageSenderService.SendInvalidMessageAsync(chatId);
            return;
        }

        await _messageSenderService.SendFirstMessageAsync(chatId);
        user.ChatId = chatId;
        user.Name = name;
        user.State = UserState.ConfirmingName;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.QueryNameConfirmationAsync(user);
    }

    public async Task HandleUserDeletedAsync(string username)
    {
        var user = await _userRepository.SelectAsync(username);
        if (user == null)
        {
            return;
        }

        await _userRepository.UpdateStateAsync(user.Id, UserState.Deleted);
    }
    
    public async Task HandleUserMessageAsync(string username, long chatId, string? text)
    {
        var user = await _userRepository.SelectAsync(username);
        if (user == null)
        {
            await _messageSenderService.SendInternalErrorAsync(chatId);
            return;
        }

        switch (user.State)
        {
            case UserState.ConfirmingName:
                await HandleConfirmingNameAsync(user, text);
                break;
            case UserState.Renaming:
                await HandleNameSetAsync(user, text);
                break;
            case UserState.AnsweringQ1:
                await HandleQuestion1AnsweredAsync(user, text);
                break;
            case UserState.AnsweringQ2:
                await HandleQuestion2AnsweredAsync(user, text);
                break;
            case UserState.AnsweringQ3:
                await HandleQuestion3AnsweredAsync(user, text);
                break;
            case UserState.WaitingForPair:
                break;
            case UserState.Pairing:
                await HandlePairingAnswerAsync(user, text);
                break;
            case UserState.Swimming:
                break;
            case UserState.Deleted:
                await _messageSenderService.SendInternalErrorAsync(chatId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(user.State), user.State, null);
        }
    }

    public async Task HandlePairFoundAsync(User user1, User user2)
    {
        await _userRepository.UpdateStateAsync(user1.Id, UserState.Pairing);
        await _userRepository.IncrementPairsCountAsync(user1.Id);
        await _userRepository.UpdateStateAsync(user2.Id, UserState.Pairing);
        await _userRepository.IncrementPairsCountAsync(user2.Id);
        await _pairRepository.CreateAsync(user1.Id, user2.Id);
        await _messageSenderService.NotifyPairFoundAsync(user1, user2);
    }

    public async Task HandleNonAcceptedPairAsync(Pair pair)
    {
        var user1 = await _userRepository.SelectAsync(pair.FirstUserId);
        var user2 = await _userRepository.SelectAsync(pair.SecondUserId);

        await _messageSenderService.NotifyPairCanceledAsync(user1, user2);
        await _userRepository.UpdateStateAsync(pair.FirstUserId, UserState.WaitingForPair);
        await _userRepository.UpdateStateAsync(pair.SecondUserId, UserState.WaitingForPair);
        await _pairRepository.DeleteAsync(pair.Id);
    }
    
    private async Task HandleConfirmingNameAsync(User user, string? text)
    {
        if (text is not (TextConstants.Ok or TextConstants.Change))
        {
            await _messageSenderService.SendInvalidMessageAsync(user.ChatId);
            return;
        }

        if (text == TextConstants.Ok)
        {
            await GoToQuestion1Async(user);
            return;
        }

        user.State = UserState.Renaming;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.SendNewNameQuestionAsync(user);
    }

    private async Task HandleNameSetAsync(User user, string? text)
    {
        if (text == null)
        {
            await _messageSenderService.SendInvalidMessageAsync(user.ChatId);
            return;
        }

        user.Name = text;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.NotifyNameChangedAsync(user);
        await GoToQuestion1Async(user);
    }

    private async Task GoToQuestion1Async(User user)
    {
        user.State = UserState.AnsweringQ1;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.QueryQuestion1(user);
    }

    private async Task HandleQuestion1AnsweredAsync(User user, string? text)
    {
        if (text is not (TextConstants.Question1Answer1 or TextConstants.Question1Answer2))
        {
            await _messageSenderService.SendInvalidMessageAsync(user.ChatId);
            return;
        }

        user.IsMan = text == TextConstants.Question1Answer1;
        await _userRepository.UpdateAsync(user);
        await GoToQuestion2Async(user);
        
        async Task GoToQuestion2Async(User user)
        {
            user.State = UserState.AnsweringQ2;
            await _userRepository.UpdateAsync(user);
            await _messageSenderService.QueryQuestion2(user);
        }
    }

    private async Task HandleQuestion2AnsweredAsync(User user, string? text)
    {
        if (text is not (TextConstants.Question2Answer1 or TextConstants.Question2Answer2))
        {
            await _messageSenderService.SendInvalidMessageAsync(user.ChatId);
            return;
        }

        user.Question2 = text == TextConstants.Question2Answer1;
        await _userRepository.UpdateAsync(user);
        await GoToQuestion3Async(user);
        
        async Task GoToQuestion3Async(User user)
        {
            user.State = UserState.AnsweringQ3;
            await _userRepository.UpdateAsync(user);
            await _messageSenderService.QueryQuestion3(user);
        }
    }
    
    private async Task HandleQuestion3AnsweredAsync(User user, string? text)
    {
        if (text is not (TextConstants.Question3Answer1 or TextConstants.Question3Answer2))
        {
            await _messageSenderService.SendInvalidMessageAsync(user.ChatId);
            return;
        }

        user.WantMan = text == TextConstants.Question3Answer1;
        await _userRepository.UpdateAsync(user);
        await GoToWaitingForPairAsync(user);
        
        async Task GoToWaitingForPairAsync(User user)
        {
            user.State = UserState.WaitingForPair;
            await _userRepository.UpdateAsync(user);
            await _messageSenderService.NotifyPairFindingAsync(user);
        }
    }

    private async Task HandlePairingAnswerAsync(User user, string? text)
    {
        if (text is not (TextConstants.Yes or TextConstants.No))
        {
            await _messageSenderService.SendInvalidMessageAsync(user.ChatId);
            return;
        }

        var pair = await _pairRepository.SelectNonStartedAsync(user.Id);
        if (pair == null)
        {
            await _messageSenderService.SendInternalErrorAsync(user.ChatId);
            return;
        }
        
        var isFirstUser = pair.FirstUserId == user.Id;
        var accepted = text == TextConstants.Yes;

        var otherUser = await _userRepository.SelectAsync(isFirstUser ? pair.SecondUserId : pair.FirstUserId);
        
        await (isFirstUser
            ? _pairRepository.SetFirstUserAcceptedAsync(pair.Id, accepted) 
            : _pairRepository.SetSecondUserAcceptedAsync(pair.Id, accepted)
        );

        pair = await _pairRepository.SelectAsync(pair.Id);

        if (!accepted)
        {
            await _messageSenderService.NotifyYouRejectedAsync(user);
            await _messageSenderService.NotifyOtherRejectedAsync(otherUser);
            await _userRepository.UpdateStateAsync(user.Id, UserState.WaitingForPair);

            otherUser.PairsCount--;
            otherUser.State = UserState.WaitingForPair;
            await _userRepository.UpdateAsync(otherUser);
            await _pairRepository.DeleteAsync(pair.Id);
            return;
        }

        var otherUserAccepted = isFirstUser ? pair.SecondUserAccepted : pair.FirstUserAccepted;
        if (otherUserAccepted == null)
        {
            await _messageSenderService.NotifyWaitingForPairAnswerAsync(user);
            return;
        }

        if (otherUserAccepted == false)
        {
            return;
        }

        await _messageSenderService.NotifyPairStartedAsync(user, otherUser);
        await _userRepository.UpdateStateAsync(user.Id, UserState.Swimming);
        await _userRepository.UpdateStateAsync(otherUser.Id, UserState.Swimming);
        await _pairRepository.SetStartedAsync(pair.Id, DateTime.UtcNow);
    }
}