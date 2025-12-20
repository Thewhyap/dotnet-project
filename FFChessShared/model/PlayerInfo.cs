using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class PlayerInfo
{
    [Key(0)]
    public required Guid PlayerId { get; set; }

    [Key(1)]
    public required string PlayerName { get; set; }

    [Key(2)]
    public required PieceColor? AssignedColor { get; set; }
}
