using Dapper;
using Konfur2022SparklingBot.Common.DataAccess;

namespace Konfur2022SparklingBot.Repositories.Pair;

public class PairRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public PairRepository(DbConnectionFactory dbConnectionFactory) => _dbConnectionFactory = dbConnectionFactory;

    public async Task<int> CountAsync(DateTime dateTime)
    {
        await using var conn = _dbConnectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(SelectCountSql, new { EndDate = dateTime });
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

    public async Task SetFirstUserAcceptedAsync(string pairId, bool answer)
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(
            SetFirstUserAcceptedSql,
            new { Id = pairId, FirstUserAccepted = answer });
    }

    public async Task SetSecondUserAcceptedAsync(string pairId, bool answer)
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(
            SetSecondUserAcceptedSql,
            new { Id = pairId, SecondUserAccepted = answer });
    }

    public async Task SetStartedAsync(string pairId, DateTime dateTime)
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(
            SetStartedSql,
            new { Id = pairId, StartDate = dateTime.Ticks });
    }

    public async Task SetEndedAsync(string pairId, DateTime dateTime)
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(
            SetEndedSql,
            new { Id = pairId, EndDate = dateTime.Ticks });
    }
    
    public async Task<Pair> SelectAsync(string id)
    {
        await using var conn = _dbConnectionFactory.Create();
        return await conn.QuerySingleAsync<Pair>(
            SelectSql,
            new { Id = id });
    }

    public async Task<Pair?> SelectAsync(string firstUserId, string secondUserId)
    {
        await using var conn = _dbConnectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<Pair>(
            SelectByUsersSql,
            new { User1 = firstUserId, User2 = secondUserId });
    }
    
    public async Task<Pair?> SelectNonStartedAsync(string userId)
    {
        await using var conn = _dbConnectionFactory.Create();
        return await conn.QuerySingleAsync<Pair>(
            SelectNonStartedSql,
            new { UserId = userId });
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
WHERE IsDeleted=FALSE AND (EndDate IS NULL OR EndDate>@EndDate)
";

    private const string SelectSql = @"
SELECT Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted
FROM pairs
WHERE Id=@Id
";

    private const string SelectByUsersSql = @"
SELECT Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted
FROM pairs
WHERE (FirstUserId = @User1 AND SecondUserId = @User2) OR (FirstUserId = @User2 AND SecondUserId = @User1)
";

    private const string SelectNonStartedSql = @"
SELECT Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted
FROM pairs
WHERE IsDeleted=FALSE AND (FirstUserId=@UserId OR SecondUserId=@UserId) AND StartDate IS NULL
";

    private const string SelectAllSql = @"
SELECT Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted
FROM pairs
";

    private const string SelectCreatedNonStartedBeforeSql = @"
SELECT Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted
FROM pairs
WHERE IsDeleted=FALSE AND StartDate IS NULL AND CreationDate<@CreationDate
";
    
    private const string SelectStartedBeforeSql = @"
SELECT Id, FirstUserId, FirstUserAccepted, SecondUserId, SecondUserAccepted, CreationDate, StartDate, EndDate, IsDeleted
FROM pairs
WHERE IsDeleted=FALSE AND StartDate<@StartDate AND EndDate IS NULL
";

    private const string SetStartedSql = @"
UPDATE pairs
SET StartDate=@StartDate
WHERE Id=@Id
";
    
    private const string SetEndedSql = @"
UPDATE pairs
SET EndDate=@EndDate
WHERE Id=@Id
";
    
    private const string SetFirstUserAcceptedSql = @"
UPDATE pairs
SET FirstUserAccepted=@FirstUserAccepted
WHERE Id=@Id
";
    
    private const string SetSecondUserAcceptedSql = @"
UPDATE pairs
SET SecondUserAccepted=@SecondUserAccepted
WHERE Id=@Id
";
}