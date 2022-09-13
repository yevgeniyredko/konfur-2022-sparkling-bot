using Dapper;
using Konfur2022SparklingBot.Common.DataAccess;

namespace Konfur2022SparklingBot.Repositories.Pair;

public class PairRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public PairRepository(DbConnectionFactory dbConnectionFactory) => _dbConnectionFactory = dbConnectionFactory;

    public async Task<int> CountAsync()
    {
        await using var conn = _dbConnectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(SelectCountSql);
    }
    
    public async Task CreateAsync(string firstUserId, string secondUserId)
    {
        var pair = new Pair
        {
            Id = Guid.NewGuid().ToString(),
            FirstUserId = firstUserId,
            SecondUserId = secondUserId,
            CreationDate = DateTime.UtcNow.Ticks
        };

        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(InsertSql, pair);
    }

    public async Task DeleteAsync(string id)
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(DeleteSql, new { Id = id });
    }

    public async Task AcceptAsync(string userId)
    {
        await SetAcceptedAsync(userId, true);
    }

    public async Task RejectAsync(string userId)
    {
        await SetAcceptedAsync(userId, false);
    }

    public async Task<Pair> SelectAsync(string firstUserId, string secondUserId)
    {
        await using var conn = _dbConnectionFactory.Create();
        return await conn.QuerySingleAsync<Pair>(
            SelectByUsersSql,
            new { FirstUserId = firstUserId, SecondUserId = secondUserId });
    }

    public async Task<List<Pair>> SelectAllAsync()
    {
        await using var conn = _dbConnectionFactory.Create();
        var pairs = await conn.QueryAsync<Pair>(SelectAllSql);
        return pairs.ToList();
    }

    public async Task<List<Pair>> SelectCreatedNonStartedBeforeAsync(DateTime dateTime)
    {
        await using var conn = _dbConnectionFactory.Create();
        var pairs = await conn.QueryAsync<Pair>(
            SelectCreatedNonStartedBeforeSql,
            new { CreationDate = dateTime.Ticks });
        return pairs.ToList();
    }

    public async Task<List<Pair>> SelectStartedBeforeAsync(DateTime dateTime)
    {
        await using var conn = _dbConnectionFactory.Create();
        var pairs = await conn.QueryAsync<Pair>(
            SelectStartedBeforeSql,
            new { StartDate = dateTime.Ticks });
        return pairs.ToList();
    }

    public async Task BootstrapAsync()
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(CreateTableSql);
    }

    private async Task SetAcceptedAsync(string userId, bool accepted)
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(
            SetFirstUserAcceptedSql,
            new { FirstUserId = userId, FirstUserAccepted = accepted });
        await conn.ExecuteAsync(
            SetSecondUserAcceptedSql,
            new { SecondUserId = userId, SecondUserAccepted = accepted });
    }

    private const string CreateTableSql = @"
CREATE TABLE IF NOT EXISTS pairs (
    Id TEXT NOT NULL PRIMARY KEY,
    FirstUserId TEXT NOT NULL,
    FirstUserAccepted BOOLEAN,
    SecondUserId TEXT NOT NULL,
    SecondUserAccepted BOOLEAN,
    CreationDate INTEGER NOT NULL,
    StartDate INTEGER,
    EndDate INTEGER,
    IsDeleted BOOLEAN NOT NULL
)";

    private const string InsertSql = @"
INSERT INTO pairs(Id, FirstUserId, SecondUserId, CreationDate, IsDeleted)
    VALUES(@Id, @FirstUserId, @SecondUserId, @CreationDate, @IsDeleted)
";

    private const string DeleteSql = @"
UPDATE pairs
SET IsDeleted=TRUE
WHERE Id=@Id
";
    private const string SelectCountSql = @"
SELECT COUNT (Id)
FROM pairs
WHERE IsDeleted=FALSE AND FirstUserAccepted=TRUE AND SecondUserAccepted=TRUE AND StartDate <> NULL AND EndDate=NULL
";

    private const string SelectByUsersSql = @"
SELECT (Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted)
FROM pairs
WHERE FirstUserId=@FirstUserId AND SecondUserId=@SecondUserId
";

    private const string SelectAllSql = @"
SELECT (Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted)
FROM pairs
";

    private const string SelectCreatedNonStartedBeforeSql = @"
SELECT (Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted)
FROM pairs
WHERE StartDate=NULL AND CreationDate<@CreationDate
";
    
    private const string SelectStartedBeforeSql = @"
SELECT (Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted)
FROM pairs
WHERE StartDate<@StartDate
";

    private const string SetFirstUserAcceptedSql = @"
UPDATE pairs
SET FirstUserAccepted=@FirstUserAccepted
WHERE FirstUserId=@FirstUserId AND FirstUserAccepted=NULL
";
    
    private const string SetSecondUserAcceptedSql = @"
UPDATE pairs
SET SecondUserAccepted=@SecondUserAccepted
WHERE SecondUserId=@SecondUserId AND SecondUserAccepted=NULL
";
}