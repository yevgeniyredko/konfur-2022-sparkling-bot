using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;

namespace Konfur2022SparklingBot.Services;

public class MessageHandlerService
{
    private readonly MessageSenderService _messageSenderService;
    private readonly UserRepository _userRepository;
    private readonly PairRepository _pairRepository;

    public MessageHandlerService(
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
        user.ChatId = chatId;
        user.Name = name;
        user.State = UserState.ConfirmingName;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.QueryNameConfirmationAsync(user);
    }

    public async Task HandleMessageAsync(string username, long chatId, string? text)
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
                break;
            case UserState.Swimming:
                break;
            case UserState.Deleted:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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

        user.Question1 = text == TextConstants.Question1Answer1;
        await _userRepository.UpdateAsync(user);
        await GoToQuestion2Async(user);
    }

    private async Task GoToQuestion2Async(User user)
    {
        user.State = UserState.AnsweringQ2;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.QueryQuestion2(user);
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
    }

    private async Task GoToQuestion3Async(User user)
    {
        user.State = UserState.AnsweringQ3;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.QueryQuestion3(user);
    }
    
    private async Task HandleQuestion3AnsweredAsync(User user, string? text)
    {
        if (text is not (TextConstants.Question3Answer1 or TextConstants.Question3Answer2))
        {
            await _messageSenderService.SendInvalidMessageAsync(user.ChatId);
            return;
        }

        user.Question3 = text == TextConstants.Question3Answer1;
        await _userRepository.UpdateAsync(user);
        await GoToWaitingForPairAsync(user);
    }

    private async Task GoToWaitingForPairAsync(User user)
    {
        user.State = UserState.WaitingForPair;
        await _userRepository.UpdateAsync(user);
        await _messageSenderService.NotifyPairFindingAsync(user);
    }
    
    private async Task HandleAcceptPairAsync()
    {
        throw new NotImplementedException();
    }

    private async Task HandleRejectPairAsync()
    {
        throw new NotImplementedException();
    }

    private async Task HandlePairEndRequestAsync()
    {
        throw new NotImplementedException();
    }
}