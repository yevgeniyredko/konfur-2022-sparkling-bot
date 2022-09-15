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

        return users.OrderBy(x => x.PairsCount)
            .Where(x => x.Id != user.Id && x.State == UserState.WaitingForPair && HasCorrectAnswers(x, user)).ToList();

        bool HasCorrectAnswers(User user1, User user2) =>
            (
                (user1.IsMan && user2.IsMan && user1.WantMan && user2.WantMan)
                || (!user1.IsMan && !user2.IsMan && !user1.WantMan && !user2.WantMan)
                || (user1.IsMan && !user2.IsMan && !user1.WantMan && user2.WantMan)
                || (!user1.IsMan && user2.IsMan && user1.WantMan && !user2.WantMan)
            )
            && (user1.Question2 == user2.Question2);
    }
}