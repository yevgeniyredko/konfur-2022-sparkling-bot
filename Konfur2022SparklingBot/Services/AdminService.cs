using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;

namespace Konfur2022SparklingBot.Services;

public class AdminService
{
    private readonly SparklingBotSettings _settings;
    private readonly UserRepository _userRepository;
    private readonly PairRepository _pairRepository;

    public AdminService(SparklingBotSettings settings, UserRepository userRepository, PairRepository pairRepository)
    {
        _settings = settings;
        _userRepository = userRepository;
        _pairRepository = pairRepository;
    }

    public async Task<string> BuildAsync()
    {
        var users = await _userRepository.SelectAllAsync();
        var pairs = await _pairRepository.SelectAllAsync();
        var activePairs = pairs.Where(p => !p.IsDeleted);

        return $@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset=""utf-8"">
        <title>Konfur 2022 Sparkling Bot admin</title>
    </head>
    <body>
        <h1>Users</h1>
        <table border=""1"">
            <thead>
                <tr>
                    <td>Id</td>
                    <td>ChatId</td>
                    <td>Name</td>
                    <td>IsMan</td>
                    <td>Q2</td>
                    <td>WantsMan</td>
                    <td>State</td>
                    <td>PairsCount</td>
                </tr>
            </thead>
            <tbody>
                {string.Join(Environment.NewLine, users.Select(BuildUserRow))}
            </tbody>
        </table>

        <h1>Accepted pairs</h1>
        <table border=""1"">
            <thead>
                <tr>
                    <td>Id</td>
                    <td>FirstUserId</td>
                    <td>FirstUserAccepted</td>
                    <td>SecondUserId</td>
                    <td>SecondUserAccepted</td>
                    <td>CreationDate</td>
                    <td>StartDate</td>
                    <td>EndDate</td>
                    <td>IsDeleted</td>
                </tr>
            </thead>
            <tbody>
                {string.Join(Environment.NewLine, activePairs.Select(BuildPairRow))}
            </tbody>
        </table>

        <h1>Pairs</h1>
        <table border=""1"">
            <thead>
                <tr>
                    <td>Id</td>
                    <td>FirstUserId</td>
                    <td>FirstUserAccepted</td>
                    <td>SecondUserId</td>
                    <td>SecondUserAccepted</td>
                    <td>CreationDate</td>
                    <td>StartDate</td>
                    <td>EndDate</td>
                    <td>IsDeleted</td>
                </tr>
            </thead>
            <tbody>
                {string.Join(Environment.NewLine, pairs.Select(BuildPairRow))}
            </tbody>
        </table>
    </body>
</html>
";
    }

    private static string BuildUserRow(User user)
    {
        return $@"
                <tr>
                    <td>{user.Id}</td>
                    <td>{user.ChatId}</td>
                    <td>{user.Name}</td>
                    <td>{user.IsMan}</td>
                    <td>{user.Question2}</td>
                    <td>{user.WantMan}</td>
                    <td>{user.State}</td>
                    <td>{user.PairsCount}</td>
                </tr>
";
    }
    
    private static string BuildPairRow(Pair pair)
    {
        return $@"
                <tr>
                    <td>{pair.Id}</td>
                    <td>{pair.FirstUserId}</td>
                    <td>{pair.FirstUserAccepted}</td>
                    <td>{pair.SecondUserId}</td>
                    <td>{pair.SecondUserAccepted}</td>
                    <td>{pair.CreationDate}</td>
                    <td>{pair.StartDate}</td>
                    <td>{pair.EndDate}</td>
                    <td>{pair.IsDeleted}</td>
                </tr>
";
    }
}