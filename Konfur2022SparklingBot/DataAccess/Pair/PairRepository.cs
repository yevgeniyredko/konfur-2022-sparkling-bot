namespace Konfur2022SparklingBot.DataAccess.Pair;

public class PairRepository
{
    public async Task<int> CountAsync()
    {
        throw new NotImplementedException();
    }
    
    public async Task<Guid> CreateAsync(Guid firstUserId, Guid secondUserId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task AcceptAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task RejectAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<List<Pair>> FindStartedBeforeAsync(DateTime dateTime)
    {
        throw new NotImplementedException();
    }
}