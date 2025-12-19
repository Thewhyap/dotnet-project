using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class PieceData
{
    public PieceData() { }
    public PieceData(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
    }
    [Key(0)] public PieceType Type { get; set; } = PieceType.Pawn;
    [Key(1)] public PieceColor Color { get; set; } = PieceColor.White;
}