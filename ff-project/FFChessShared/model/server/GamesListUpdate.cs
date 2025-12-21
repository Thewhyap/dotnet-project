using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GamesListUpdate
{
    [Key(0)]
    public List<GameInfo> Games { get; set; } = new();

    public GamesListUpdate() { }

    public GamesListUpdate(List<GameInfo> games)
    {
        Games = games;
    }
}
