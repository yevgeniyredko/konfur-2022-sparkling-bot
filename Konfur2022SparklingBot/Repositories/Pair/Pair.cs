namespace Konfur2022SparklingBot.Repositories.Pair;

public class Pair
{
    public string Id { get; set; }
    public string FirstUserId { get; set; }
    public bool? FirstUserAccepted { get; set; }
    public string SecondUserId { get; set; }
    public bool? SecondUserAccepted { get; set; }
    public long CreationDate { get; set; }
    public long? StartDate { get; set; }
    public long? EndDate { get; set; }
    public bool IsDeleted { get; set; }
}