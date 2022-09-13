using Konfur2022SparklingBot;
using Konfur2022SparklingBot.Background;
using Konfur2022SparklingBot.Common.DataAccess;
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
    .AddSingleton<MessageHandlerService>()
    .AddSingleton<MessageSenderService>();

builder.Services
    .AddSingleton<PeriodicalBackgroundServiceParameters>()
    .AddHostedService<MessageHandlerBackgroundService>()
    .AddHostedService<MakeNewPairsBackgroundService>()
    .AddHostedService<BreakNonAcceptedPairsBackgroundService>()
    .AddHostedService<BreakFinishedPairsBackgroundService>();

var app = builder.Build();

app.MapGet("/", async (AdminService b) => await b.BuildAsync());

app.Run();