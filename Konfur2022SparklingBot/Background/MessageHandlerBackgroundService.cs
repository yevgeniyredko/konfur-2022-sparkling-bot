using Konfur2022SparklingBot.Common.EventLoop;
using Konfur2022SparklingBot.Services;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Konfur2022SparklingBot.Background;

public class MessageHandlerBackgroundService : BackgroundService
{
    private readonly ILogger<MessageHandlerBackgroundService> _logger;
    private readonly EventLoop _eventLoop;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly EventHandlerService _eventHandlerService;

    public MessageHandlerBackgroundService(
        ILogger<MessageHandlerBackgroundService> logger,
        EventLoop eventLoop,
        ITelegramBotClient telegramBotClient,
        EventHandlerService eventHandlerService)
    {
        _logger = logger;
        _eventLoop = eventLoop;
        _telegramBotClient = telegramBotClient;
        _eventHandlerService = eventHandlerService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _telegramBotClient.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            new ReceiverOptions(),
            stoppingToken);
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        => _eventLoop.Push(() => HandleUpdateInternalAsync(update));

    private async Task HandleUpdateInternalAsync(Update update)
    {
        _logger.LogInformation(JsonConvert.SerializeObject(update));

        try
        {
            var myChatMember = update.MyChatMember;
            if (myChatMember != null && myChatMember.NewChatMember.Status == ChatMemberStatus.Kicked)
            {
                await _eventHandlerService.HandleUserDeletedAsync(update.MyChatMember.Chat.Username);
                return;
            }

            var message = update.Message;
            if (message?.From?.Username == null)
            {
                return;
            }

            var chatId = message.Chat.Id;

            if (message.Text == null)
            {
                await _eventHandlerService.HandleErrorAsync(chatId);
                return;
            }

            if (message.Text == "/start")
            {
                await _eventHandlerService.HandleUserAddedAsync(
                    message.From.Username,
                    chatId,
                    (message.From.FirstName + " " + message.From.LastName).TrimEnd());
                return;
            }

            await _eventHandlerService.HandleUserMessageAsync(message.From.Username, chatId, message.Text);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }
    }
    
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "");
        return Task.CompletedTask;
    }
}