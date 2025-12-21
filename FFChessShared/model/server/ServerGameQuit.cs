using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ServerGameQuit
{
    [Key(0)] public Guid GameId { get; set; }
    [Key(1)] public string Reason { get; set; } = "";

    public ServerGameQuit() { }

    public ServerGameQuit(Guid gameId, string reason)
    {
        GameId = gameId;
        Reason = reason;
    }
}

