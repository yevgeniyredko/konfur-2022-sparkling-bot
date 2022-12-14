using Dapper;
using Konfur2022SparklingBot.Common.DataAccess;

namespace Konfur2022SparklingBot.Repositories.User;

public class UserRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public UserRepository(DbConnectionFactory dbConnectionFactory) => _dbConnectionFactory = dbConnectionFactory;

    public async Task<List<User>> SelectAllAsync()
    {
        await using var conn = _dbConnectionFactory.Create();
        var users = await conn.QueryAsync<User>(SelectAllSql);
        return users.ToList();
    }

    public async Task<User?> SelectAsync(string id)
    {
        await using var conn = _dbConnectionFactory.Create();
        var user = await conn.QuerySingleOrDefaultAsync<User>(SelectByIdSql, new { Id = id });
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(UpsertSql, user);
    }
    
    public async Task BootstrapAsync()
    {
        await using var conn = _dbConnectionFactory.Create();
        await conn.ExecuteAsync(CreateTableSql);
    }
    
    private const string CreateTableSql = @"
CREATE TABLE IF NOT EXISTS users (
    Id TEXT NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    ChatId INTEGER NOT NULL,
    IsMan BOOLEAN,
    Question2 BOOLEAN,
    WantMan BOOLEAN,
    State INTEGER NOT NULL,
    PairsCount INTEGER
)";

    private const string UpsertSql = @"
INSERT INTO users(Id, Name, ChatId, IsMan, Question2, WantMan, State, PairsCount)
    VALUES(@Id, @Name, @ChatId, @IsMan, @Question2, @WantMan, @State, @PairsCount)
ON CONFLICT(Id) DO UPDATE
    SET Name=@Name, 
        ChatId=@ChatId,
        IsMan=@IsMan,
        Question2=@Question2,
        WantMan=@WantMan,
        State=@State,
        PairsCount=@PairsCount;
";

    private const string SelectAllSql = @"
SELECT Id, Name, ChatId, IsMan, Question2, WantMan, State, PairsCount FROM users
";
    
    private const string SelectByIdSql = SelectAllSql + @" WHERE Id=@Id";
}