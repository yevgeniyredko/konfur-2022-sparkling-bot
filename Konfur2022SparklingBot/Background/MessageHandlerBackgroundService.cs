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
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly MessageHandlerService _messageHandlerService;

    public MessageHandlerBackgroundService(
        ILogger<MessageHandlerBackgroundService> logger,
        ITelegramBotClient telegramBotClient,
        MessageHandlerService messageHandlerService)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
        _messageHandlerService = messageHandlerService;
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
    {
        _logger.LogInformation(JsonConvert.SerializeObject(update));

        try
        {
            var message = update.Message;
            if (message?.From?.Username == null)
            {
                return;
            }

            var chatId = message.Chat.Id;

            if (message.Text == null)
            {
                await _messageHandlerService.HandleErrorAsync(chatId);
                return;
            }

            if (message.Text == "/start")
            {
                await _messageHandlerService.HandleUserAddedAsync(
                    message.From.Username,
                    chatId,
                    (message.From.FirstName + " " + message.From.LastName).TrimEnd());
                return;
            }

            await _messageHandlerService.HandleMessageAsync(message.From.Username, chatId, message.Text);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }
    }

    private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        => _logger.LogError(exception, "");
}