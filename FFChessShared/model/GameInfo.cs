using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GameInfo
{
    [Key(0)] public required Guid GameId { get; set; }
    [Key(1)] public required string GameName { get; set; }
    [Key(2)] public required MatchStatus Status { get; set; }
}
