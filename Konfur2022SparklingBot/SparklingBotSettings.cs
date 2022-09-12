namespace Konfur2022SparklingBot;

public class SparklingBotSettings
{
    private readonly IConfiguration _config;

    public SparklingBotSettings(IConfiguration config) => _config = config;

    public string SqLiteConnectionString => _config.GetValue<string>("SqLiteConnectionString");

    public string TelegramApiKey => _config.GetValue<string>("TelegramApiKey");

    public int MaxPairsCount => _config.GetValue<int>("MaxPairsCount");

    public TimeSpan PairTime => _config.GetValue<TimeSpan>("PairTime");

    public TimeSpan PairAcceptTtl => _config.GetValue<TimeSpan>("PairAcceptTtl");
}