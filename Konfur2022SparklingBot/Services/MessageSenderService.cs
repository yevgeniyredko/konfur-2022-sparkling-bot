using Konfur2022SparklingBot.Repositories.User;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = Konfur2022SparklingBot.Repositories.User.User;

namespace Konfur2022SparklingBot.Services;

public class MessageSenderService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly UserRepository _userRepository;

    public MessageSenderService(
        ITelegramBotClient telegramBotClient,
        UserRepository userRepository)
    {
        _telegramBotClient = telegramBotClient;
        _userRepository = userRepository;
    }

    public async Task SendInvalidMessageAsync(long chatId)
    {
        await _telegramBotClient.SendTextMessageAsync(new ChatId(chatId), "Неверный формат ответа");
    }

    public async Task SendInternalErrorAsync(long chatId)
    {
        await _telegramBotClient.SendTextMessageAsync(new ChatId(chatId), "Внутренняя ошибка");
    }

    public async Task SendNewNameQuestionAsync(User user)
    {
        await _telegramBotClient.SendTextMessageAsync(new ChatId(user.ChatId), "Введи имя");
    }
    
    public async Task QueryNameConfirmationAsync(User user)
    {
        await QueryAsync(
            user.ChatId,
            $@"Привет, {user.Name}. Если хочешь поменять имя, нажми ""Поменять"". Если всё устраивает, нажми ""Ок""",
            TextConstants.Change,
            TextConstants.Ok);
    }

    public async Task QueryQuestion1(User user)
    {
        await QueryAsync(user.ChatId, TextConstants.Question1, TextConstants.Question1Answer1, TextConstants.Question1Answer2);
    }
    
    public async Task QueryQuestion2(User user)
    {
        await QueryAsync(user.ChatId, TextConstants.Question2, TextConstants.Question2Answer1, TextConstants.Question2Answer2);
    }
    
    public async Task QueryQuestion3(User user)
    {
        await QueryAsync(user.ChatId, TextConstants.Question3, TextConstants.Question3Answer1, TextConstants.Question3Answer2);
    }

    private async Task QueryAsync(long chatId, string question, string answer1, string answer2)
    {
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(chatId),
            question,
            replyMarkup: new ReplyKeyboardMarkup(
                new []
                {
                    new KeyboardButton(answer1),
                    new KeyboardButton(answer2),
                }));
    }
    
    public async Task NotifyNameChangedAsync(User user)
    {
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user.ChatId),
            $@"Имя сохранено как {user.Name}");
    }

    public async Task NotifyPairFindingAsync(User user)
    {
        await _telegramBotClient.SendTextMessageAsync(
            new ChatId(user.ChatId),
            $@"Ожидается пара");
    }
    
    public async Task NotifyPairFinishedAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task NotifyPairCanceledAsync(string username)
    {
        throw new NotImplementedException();
    }
    
    public async Task NotifyPairFoundAsync(User user1, User user2)
    {
        throw new NotImplementedException();
    }
}