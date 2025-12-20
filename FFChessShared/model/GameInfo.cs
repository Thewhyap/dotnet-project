using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GameInfo
{
    [Key(0)] public required Guid GameId { get; set; }
    [Key(1)] public required string GameName { get; set; }
    [Key(2)] public required MatchStatus Status { get; set; }
    [Key(3)] public required string WhitePlayerName { get; set; }
    [Key(4)] public required string BlackPlayerName { get; set; }
}
