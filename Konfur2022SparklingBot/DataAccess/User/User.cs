namespace Konfur2022SparklingBot.DataAccess.User;

public class User
{
    public Guid Id { get; set; }
    public Guid Name { get; set; }
    public string TelegramId { get; set; }
    public string ChatId { get; set; }
    public bool Question1 { get; set; }
    public bool Question2 { get; set; }
    public bool Question3 { get; set; }
    public UserState State { get; set; }
    public int PairsCount { get; set; }
}