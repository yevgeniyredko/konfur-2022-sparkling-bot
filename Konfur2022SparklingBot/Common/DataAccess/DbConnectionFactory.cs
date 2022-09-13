using Microsoft.Data.Sqlite;

namespace Konfur2022SparklingBot.Common.DataAccess;

public class DbConnectionFactory
{
    private readonly SparklingBotSettings _settings;

    public DbConnectionFactory(SparklingBotSettings settings) => _settings = settings;

    public SqliteConnection Create() => new(_settings.SqLiteConnectionString);
}