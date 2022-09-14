using Konfur2022SparklingBot.Repositories.User;

namespace Konfur2022SparklingBot.Services;

public static class TextConstants
{
    public const string Yes = "Да";
    public const string No = "Нет";

    public const string Cancel = "Отмена";
    
    public const string Ok = "Ок";
    public const string Change = "Поменять";

    public const string InvalidAnswer = "Неверный формат ответа, попробуй снова";
    public const string InternalError = "Внутренняя ошибка. Если ты это увидел, то всё очень плохо, нажми /start";

    public static string Hello(string name)
        => $@"Привет, {name}! Если хочешь поменять имя, нажми ""{Change}"". Если всё устраивает, нажми ""{Ok}""";
    
    public const string EnterName = "Введи имя";
    public static string NameSaved(string name) => $@"Имя сохранено как {name}";
    
    public const string Question1 = "Вопрос 1";
    public const string Question1Answer1 = "Ответ 1";
    public const string Question1Answer2 = "Ответ 2";
    
    public const string Question2 = "Вопрос 2";
    public const string Question2Answer1 = "Ответ 1";
    public const string Question2Answer2 = "Ответ 2";
    
    public const string Question3 = "Вопрос 3";
    public const string Question3Answer1 = "Ответ 1";
    public const string Question3Answer2 = "Ответ 2";

    public const string PairFinding = @"Ожидается пара";
    public static string PairFound(User user) => $"Ваша пара {user.Name} (@{user.Id}). Вы согласны?";

    public const string WaitingForPairAnswer = @"Ожидаем ответа от пары";
    public const string YouRejectedPair = @"Пара отклонена, ожидаем новую";
    public const string OtherRejectedPair = @"Пара отклонила вас, ожидаем новую";

    public const string Match = $@"Мэтч состоялся, подходите к лодке и начинайе общение. Для досрочной отмены нажмите {Cancel}";

    public const string PairCanceled = "Вы не успели согласиться. Поиск следующей пары";
    public const string PairFinished = "Время вышло. Успейте вернуться в течение 5 минут";
}