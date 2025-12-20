using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GameJoined
{
    [Key(0)] public Guid GameId { get; set; }
    [Key(1)] public PieceColor? AssignedColor { get; set; }
    [Key(2)] public GameUpdate InitialGameState { get; set; }
}
