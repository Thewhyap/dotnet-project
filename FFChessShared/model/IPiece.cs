namespace FFChessShared;

public interface IPiece
{
    PieceType Type { get; }
    PieceColor Color { get; }

    bool IsMoveLegal(GameState state, ChessMove move);

    bool IsSpecificMoveLegal(GameState state, ChessMove move, bool inRoque = false);
}
