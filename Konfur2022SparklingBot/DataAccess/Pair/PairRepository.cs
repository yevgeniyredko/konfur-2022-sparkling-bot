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
    
    public async Task<List<Pair>> FindBeforeAsync(DateTime dateTime)
    {
        throw new NotImplementedException();
    }
}