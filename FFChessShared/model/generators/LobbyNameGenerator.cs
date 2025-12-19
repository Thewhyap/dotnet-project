namespace FFChessShared.generators;

public class LobbyNameGenerator
{
    private static readonly string[] Names = {
        "Paris", 
        "Amesterdam",
        "Berlin",
        "Madrid",
        "Rome",
        "Vienna",
        "Prague",
        "Lisbon",
        "Dublin",
        "Copenhagen",
        "Helsinki",
        "Oslo",
        "Stockholm",
        "Warsaw",
        "Budapest",
        "Brussels",
        "Athens",
        "Zurich",
        "Edinburgh",
    };
    
    
    public static string GenerateLobbyName()
    {
        var random = new Random();
        var name = Names[random.Next(Names.Length)];
        var number = random.Next(100, 999);
        return $"{name}-{number}";
    }
}