namespace Server.Chess;

public static class NicknameGenerator
{
    private static readonly string[] Nicknames = {
        "John Doe",
        "Toby Lerone",
        "Robyn Banks",
        "Sum Ting Wong",
        "Lee King",
        "Joe King",
        "Gerard Menvussa",
        "Arthur Luth",
        "Mike Litoris",
        "Chris.P Chicken",
        "Jean Voilassosse",
        "Jean Philippe-Herbien",
        "Frank N. Stein",
        "Chester Minit",
        "Felix Austead",
        "Horace Cope",
        "Ken Talke",
        "Lee Thargic",
        "Lee Vitoff",
        "Libby Doe",
        "Lipin Jection",
        "Lois Price",
        "Mack Aroney",
        "Sam Sung",
        "Milene Micotton",
        "Leny Bare",
        "Theo Thiste",
        "Ben Dover",
        "Hugh Jass",
        "Noah Pinion",
        "Ivana Tinkle",
        "Willie Stroker",
        "Harry Pitts",
        "Terry Aki",
        "Justin Case",
        "Crystal Clear",
        "Drew Peacock",
        "Ella Vator",
        "Anna Graham",
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