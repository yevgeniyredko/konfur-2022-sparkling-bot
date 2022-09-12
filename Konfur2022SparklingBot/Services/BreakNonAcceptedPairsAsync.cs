using Konfur2022SparklingBot.Common.HostedServices;
using Konfur2022SparklingBot.DataAccess.Pair;

namespace Konfur2022SparklingBot.Services;

public class BreakNonAcceptedPairsAsync : PeriodicalBackgroundService
{
    private readonly PairRepository _pairRepository;

    public BreakNonAcceptedPairsAsync(PairRepository pairRepository)
    {
        _pairRepository = pairRepository;
    }

    protected override Task RunAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}