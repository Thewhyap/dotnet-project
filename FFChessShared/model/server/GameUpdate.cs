using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GameUpdate
{
    [Key(0)] public required GameState State { get; set; }
    [Key(1)] public required TurnStatus TurnStatus { get; set; }
}
