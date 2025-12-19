using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class Games
{
    [Key(0)] public List<Game> CurrentGames { get; set; } = new List<Game>();
}
