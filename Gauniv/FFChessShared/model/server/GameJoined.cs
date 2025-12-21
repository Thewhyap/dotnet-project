using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GameJoined
{
    [Key(0)] public Guid GameId { get; set; }
    [Key(1)] public PieceColor? AssignedColor { get; set; } = null;
    [Key(2)] public GameUpdate InitialGameState { get; set; } = null!;

    public GameJoined() { }

    public GameJoined(Guid gameId, PieceColor? color, GameUpdate update)
    {
        GameId = gameId;
        AssignedColor = color;
        InitialGameState = update;
    }
}
