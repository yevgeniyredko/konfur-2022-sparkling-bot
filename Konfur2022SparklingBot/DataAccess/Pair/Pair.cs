namespace Konfur2022SparklingBot.DataAccess.Pair;

public class Pair
{
    public Guid Id { get; set; }
    public Guid FirstUserId { get; set; }
    public bool? FirstUserAccepted { get; set; }
    public Guid SecondUserId { get; set; }
    public bool? SecondUserAccepted { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}