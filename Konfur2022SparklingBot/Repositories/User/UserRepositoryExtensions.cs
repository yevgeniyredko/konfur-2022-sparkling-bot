namespace Konfur2022SparklingBot.Repositories.User;

public static class UserRepositoryExtensions
{
    public static async Task UpdateStateAsync(this UserRepository source, string id, UserState state)
    {
        var user = await source.SelectAsync(id);
        if (user == null)
        {
            return;
        }

        user.State = state;
        await source.UpdateAsync(user);
    }

    public static async Task IncrementPairsCountAsync(this UserRepository source, string id)
    {
        var user = await source.SelectAsync(id);
        if (user == null)
        {
            return;
        }

        user.PairsCount++;
        await source.UpdateAsync(user);
    }
    
    public static async Task<List<User>> SelectAllAsync(this UserRepository source, UserState state)
    {
        var users = await source.SelectAllAsync();
        return users.Where(x => x.State == state).ToList();
    }

    public static async Task<List<User>> SelectMatchedAsync(this UserRepository source, User user)
    {
        var users = await source.SelectAllAsync();

        return users.Where(x => x.Id != user.Id && x.State == UserState.WaitingForPair && HasSimilarAnswers(x, user)).ToList();

        bool HasSimilarAnswers(User user1, User user2) =>
            (user1.Question1 == user2.Question1 && user1.Question2 == user2.Question2)
            || (user1.Question1 == user2.Question1 && user1.Question3 == user2.Question3)
            || (user1.Question2 == user2.Question2 && user1.Question3 == user2.Question3);
    }
}