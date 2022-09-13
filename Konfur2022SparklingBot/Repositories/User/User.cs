namespace Konfur2022SparklingBot.Repositories.User;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public long ChatId { get; set; }
    public bool Question1 { get; set; }
    public bool Question2 { get; set; }
    public bool Question3 { get; set; }
    public UserState State { get; set; }
    public int PairsCount { get; set; }
}