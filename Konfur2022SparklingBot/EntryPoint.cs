using Konfur2022SparklingBot;
using Konfur2022SparklingBot.Background;
using Konfur2022SparklingBot.Common.DataAccess;
using Konfur2022SparklingBot.Common.EventLoop;
using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;
using Konfur2022SparklingBot.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsecrets.json");

builder.Services.AddSingleton<SparklingBotSettings>();

builder.Services
    .AddSingleton<ITelegramBotClient>(s =>
    {
        var settings = s.GetService<SparklingBotSettings>()!;
        return new TelegramBotClient(new TelegramBotClientOptions(settings.TelegramApiKey));
    });

builder.Services
    .AddSingleton<DbConnectionFactory>()
    .AddHostedService<DbBootstrapHostedService>()
    .AddSingleton<UserRepository>()
    .AddSingleton<PairRepository>();

builder.Services
    .AddSingleton<AdminService>()
    .AddSingleton<EventHandlerService>()
    .AddSingleton<MessageSenderService>();

builder.Services
    .AddSingleton<PeriodicalBackgroundServiceParameters>()
    .AddSingleton<EventLoop>()
    .AddHostedService<EventLoopBackgroundService>()
    .AddHostedService<MessageHandlerBackgroundService>()
    .AddHostedService<MakeNewPairsBackgroundService>()
    .AddHostedService<BreakNonAcceptedPairsBackgroundService>();

var app = builder.Build();

app.MapGet("/", async (AdminService b) => Results.Content(await b.BuildAsync(), "text/html"));

app.Run();