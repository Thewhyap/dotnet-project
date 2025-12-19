using FFChessShared;

namespace Server.Chess;

public abstract class PieceBase(PieceType type, PieceColor color)
{
    public PieceData pieceData = new(type, color);

    public bool IsMoveLegal(GameState state, ChessMove move)
    {
        // Check that you don't attack your own piece

        var targetPiece = state.Board.Cells[move.To.X, move.To.Y];
        if (targetPiece.HasValue && targetPiece.Value.Color == state.CurrentTurn)
            return false;

        // Check if your king is not attacked at the end of your turn

        Board boardAfterMove = state.Board.Clone();
        boardAfterMove.MovePiece(move.From, move.To);
        GameState afterMoveState = new GameState(boardAfterMove, state.CurrentTurn);

        RuleHelper.IsKingAttacked(afterMoveState);

        // Check that you respect the rules of the piece you are playing with

        return IsSpecificMoveLegal(state, move);
    }

    public abstract bool IsSpecificMoveLegal(GameState state, ChessMove move, bool inRoque = false);
}
