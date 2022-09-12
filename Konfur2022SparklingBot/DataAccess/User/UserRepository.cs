namespace Konfur2022SparklingBot.DataAccess.User;

public class UserRepository
{
    public async Task<List<User>> FindAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> FindAllAsync(UserState state)
    {
        throw new NotImplementedException();
    }
    
    public async Task<List<User>> FindMatchedAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task ChangeStateAsync(Guid id, UserState userState)
    {
        throw new NotImplementedException();
    }
}