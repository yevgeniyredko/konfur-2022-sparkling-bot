using Konfur2022SparklingBot.Repositories.User;

namespace Konfur2022SparklingBot.Services;

public static class TextConstants
{
    public const string Yes = "Да";
    public const string No = "Нет";

    public const string Ok = "Ок";
    public const string Change = "Сменить имя";

    public const string InvalidAnswer = "О-оу. Я вижу тут какого-то гангстера. Поправь ответ, я люблю, когда люди выбирают из предложенных мной ответов";
    public const string InternalError = "Внутренняя ошибка. Если ты это увидел, то всё очень плохо. Попробуй нажать /start, а если не поможет, напиши @yevgeniyredko";

    public const string FirstMessage =
        "Привет! Сейчас тебе придет три вопроса о тебе и твоих предпочтениях. Когда другой человек ответит также, бот предложит вам совершить путешествие по озеру. Согласись на предложенную пару или подожди другого шанса";
    public static string Hello(string name)
        => $@"Нажми {Ok}, если ты {name} или смени имя, если хочешь побыть инкогнито.";
    
    public const string EnterName = "Введи имя";
    public static string NameSaved(string name) => $@"Имя сохранено как {name}";
    
    public const string Question1 = "Укажи свой пол";
    public const string Question1Answer1 = "Юноша";
    public const string Question1Answer2 = "Девушка";
    
    public const string Question2 = "Ты любишь игристое?";
    public const string Question2Answer1 = Yes;
    public const string Question2Answer2 = No;
    
    public const string Question3 = "Хочешь прокатиться с мальчиком или с девочкой?";
    public const string Question3Answer1 = "С мальчиком";
    public const string Question3Answer2 = "С девочкой";

    public const string PairFinding = @"Минутку, бот ищет твою половинку";
    public static string PairFound(User user) => $"Мы нашли твой мэтч. Это {user.Name} (@{user.Id}). Пойдете кататься?";

    public const string WaitingForPairAnswer = @"Принял! Жду подтверждения от половинки";
    public const string YouRejectedPair = @"Cердечко разбито, ищем другую пару. Но придется подождать, потому что ты в конце очереди";
    public const string OtherRejectedPair = @"Похоже, это не твоя судьба. Ищем дальше.";

    public static readonly string Match = 
        @"Поздравляем! Сегодня два сердечка нашли друг друга. Выходите во внутренний двор и идите в сторону набережной. Там вас встретит инструктор и волонтер, который порадует пледом, бокалами и игристым"
        + Environment.NewLine
        + Environment.NewLine
        + @"Если вы нашли пару, а все лодки заняты, обратитесь к волонтеру. Он запишет ваши контакты и сообщит, когда лодки освободятся";

    public const string PairCanceled = "Похоже, вы не хотите кататься или сейчас не можете. Перенес вас в конец очереди. Скоро пришлю новую пару";
}