using Dapper;
using Konfur2022SparklingBot.Repositories.Pair;
using Konfur2022SparklingBot.Repositories.User;

namespace Konfur2022SparklingBot.Common.DataAccess;

public class DbBootstrapHostedService : IHostedService
{
    private readonly UserRepository _userRepository;
    private readonly PairRepository _pairRepository;

    public DbBootstrapHostedService(UserRepository userRepository, PairRepository pairRepository)
    {
        _userRepository = userRepository;
        _pairRepository = pairRepository;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _userRepository.BootstrapAsync();
        await _pairRepository.BootstrapAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


    

}