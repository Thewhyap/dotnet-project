using FFChessShared;

namespace Server.Chess;

public static class PieceRuleRegistry
{
    private static readonly Dictionary<PieceType, PieceBase> _rules =
        new()
        {
            { PieceType.Pawn, new Pawn() },
            { PieceType.Knight, new Knight() },
            { PieceType.Bishop, new Bishop() },
            { PieceType.Rook, new Rook() },
            { PieceType.Queen, new Queen() },
            { PieceType.King, new King() }
        };

    public static PieceBase GetRule(PieceType type)
        => _rules[type];
}