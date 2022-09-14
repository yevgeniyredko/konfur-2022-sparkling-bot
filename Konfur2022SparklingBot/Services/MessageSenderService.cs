using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = Konfur2022SparklingBot.Repositories.User.User;

namespace Konfur2022SparklingBot.Services;

public class MessageSenderService
{
    private readonly ITelegramBotClient _telegramBotClient;

    public MessageSenderService(ITelegramBotClient telegramBotClient) => _telegramBotClient = telegramBotClient;

    public async Task SendInvalidMessageAsync(long chatId)
        => await _telegramBotClient.SendTextMessageAsync(new ChatId(chatId), TextConstants.InvalidAnswer);

    public async Task SendInternalErrorAsync(long chatId)
        => await _telegramBotClient.SendTextMessageAsync(new ChatId(chatId), TextConstants.InternalError);

    public async Task QueryNameConfirmationAsync(User user)
        => await QueryAsync(user.ChatId, TextConstants.Hello(user.Name), TextConstants.Change, TextConstants.Ok);

    public async Task SendNewNameQuestionAsync(User user) =>
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user.ChatId),
            TextConstants.EnterName,
            replyMarkup: new ReplyKeyboardRemove());

    public async Task NotifyNameChangedAsync(User user)
        => await _telegramBotClient.SendTextMessageAsync(new ChatId(user.ChatId), TextConstants.NameSaved(user.Name));

    public async Task QueryQuestion1(User user)
        => await QueryAsync(user.ChatId, TextConstants.Question1, TextConstants.Question1Answer1, TextConstants.Question1Answer2);

    public async Task QueryQuestion2(User user)
        => await QueryAsync(user.ChatId, TextConstants.Question2, TextConstants.Question2Answer1, TextConstants.Question2Answer2);

    public async Task QueryQuestion3(User user)
        => await QueryAsync(user.ChatId, TextConstants.Question3, TextConstants.Question3Answer1, TextConstants.Question3Answer2);

    public async Task NotifyPairFindingAsync(User user) =>
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user.ChatId),
            TextConstants.PairFinding,
            replyMarkup: new ReplyKeyboardRemove());

    public async Task NotifyPairFoundAsync(User user1, User user2)
    {
        await QueryAsync(
            user1.ChatId,
            TextConstants.PairFound(user2),
            TextConstants.Yes,
            TextConstants.No);
        
        await QueryAsync(
            user2.ChatId,
            TextConstants.PairFound(user1),
            TextConstants.Yes,
            TextConstants.No);
    }

    public async Task NotifyWaitingForPairAnswerAsync(User user) =>
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user.ChatId),
            TextConstants.WaitingForPairAnswer,
            replyMarkup: new ReplyKeyboardRemove());

    public async Task NotifyYouRejectedAsync(User user) =>
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user.ChatId),
            TextConstants.YouRejectedPair,
            replyMarkup: new ReplyKeyboardRemove());

    public async Task NotifyOtherRejectedAsync(User user) =>
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user.ChatId),
            TextConstants.OtherRejectedPair,
            replyMarkup: new ReplyKeyboardRemove());

    public async Task NotifyPairStartedAsync(User user1, User user2)
        => await NotifyTwoUsersAsync(user1, user2, TextConstants.Match);

    public async Task NotifyPairCanceledAsync(User user1, User user2)
        => await NotifyTwoUsersAsync(user1, user2, TextConstants.PairCanceled);

    public async Task NotifyPairFinishedAsync(User user1, User user2)
        => await NotifyTwoUsersAsync(user1, user2, TextConstants.PairFinished);

    private async Task QueryAsync(long chatId, string question, string answer1, string answer2) =>
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(chatId),
            question,
            replyMarkup: new ReplyKeyboardMarkup(
                new []
                {
                    new KeyboardButton(answer1),
                    new KeyboardButton(answer2),
                }));

    private async Task NotifyTwoUsersAsync(User user1, User user2, string message)
    {
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user1.ChatId),
            message,
            replyMarkup: new ReplyKeyboardRemove());
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user2.ChatId),
            message,
            replyMarkup: new ReplyKeyboardRemove());
    }
}