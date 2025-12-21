using FFChessShared;

namespace Server.Chess;

public static class PieceRuleRegistry
{
    public static PieceBase GetRule(PieceType type, PieceColor color)
        => type switch
        {
            PieceType.Pawn => new Pawn(color),
            PieceType.Knight => new Knight(color),
            PieceType.Bishop => new Bishop(color),
            PieceType.Rook => new Rook(color),
            PieceType.Queen => new Queen(color),
            PieceType.King => new King(color),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}