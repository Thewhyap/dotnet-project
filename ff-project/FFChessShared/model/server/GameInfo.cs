using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GameInfo
{
    [Key(0)] public Guid GameId { get; set; }
    [Key(1)] public string GameName { get; set; } = string.Empty;
    [Key(2)] public MatchStatus Status { get; set; } = MatchStatus.Waiting;

    [Key(3)] public string WhitePlayerName { get; set; } = "Waiting for opponent";
    [Key(4)] public string BlackPlayerName { get; set; } = "Waiting for opponent";

    public GameInfo() { }

    public GameInfo(Guid gameId, string gameName, MatchStatus matchStatus, string whitePlayerName, string blackPlayerName)
    {
        GameId = gameId;
        GameName = gameName;
        Status = matchStatus;
        WhitePlayerName = whitePlayerName;
        BlackPlayerName = blackPlayerName;
    }
}
