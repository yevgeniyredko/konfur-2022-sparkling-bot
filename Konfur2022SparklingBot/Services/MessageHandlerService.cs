using Konfur2022SparklingBot.Messages;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Konfur2022SparklingBot.Services;

public class MessageHandlerService : BackgroundService
{
    private readonly ILogger<MessageHandlerService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly MessageHandler _messageHandler;

    public MessageHandlerService(
        ILogger<MessageHandlerService> logger,
        ITelegramBotClient telegramBotClient,
        MessageHandler messageHandler)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
        _messageHandler = messageHandler;
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
        
    }

    private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        => _logger.LogError(exception, "");
}