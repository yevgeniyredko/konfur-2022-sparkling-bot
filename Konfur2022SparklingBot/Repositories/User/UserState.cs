namespace Konfur2022SparklingBot.Repositories.User;

public enum UserState
{
    ConfirmingName = 0,
    Renaming = 1,
    AnsweringQ1 = 2,
    AnsweringQ2 = 3,
    AnsweringQ3 = 4,
    WaitingForPair = 5,
    Pairing = 6,
    Swimming = 7,
    Deleted = 8,
}