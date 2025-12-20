namespace Server.Chess;

public static class NicknameGenerator
{
    private static readonly string[] Nicknames = {
        "John Doe",
        "Gerard Menvussa",
        "Arthur Luth",
        "Mike Litoris",
        "Chris.P Chicken",
        "Jean Voilassosse",
        "Jean Philippe-Herbien",
        "Milene Micotton",
        "Leny Bare",
        "Theo Thiste",
        "Ben Dover",
        "Hugh Jass",
        "Ivana Tinkle",
        "Willie Stroker",
        "Harry Pitts",
        "Terry Aki",
        "Justin Case",
        "Crystal Clear",
        "Drew Peacock",
        "Ella Vator",
        "Frank N. Stein",
        "Holly Wood",
        "Lou Natic",
        "Moe Lester",
        "Phil McCraken",
        "Sal Ami",
        "Will Power",
        "Mike Rotch",
        "Master Baiter",
    };
    
    
    public static string GenerateNickname()
    {
        var random = new Random();
        var name = Nicknames[random.Next(Nicknames.Length)];
        return $"{name}";
    }
}