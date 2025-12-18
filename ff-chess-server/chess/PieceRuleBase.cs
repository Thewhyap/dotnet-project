using FFChessShared;

namespace Server.Chess;

public abstract class PieceRuleBase
{
	public bool IsLegalMove(GameState state, ChessMove move)
	{
		if (IsOffBoard(state.Board, move.From) || IsOffBoard(state.Board, move.To))
			return false;
		
		var piece = state.Board.Cells[move.From.X, move.From.Y];
		if (!piece.HasValue || piece.Value.Color != state.CurrentTurn)
			return false;
		
		var targetPiece = state.Board.Cells[move.To.X, move.To.Y];
		if (targetPiece.HasValue && targetPiece.Value.Color == state.CurrentTurn)
			return false;

		return IsSpecificMoveLegal(state, move);
	}
	
	private bool IsOffBoard(Board board, ChessSquare chessSquare)
	{
		return (chessSquare.X < 0 || chessSquare.X > board.Size) && (chessSquare.Y < 0 || chessSquare.Y > board.Size);
	}
	
	protected abstract bool IsSpecificMoveLegal(GameState state, ChessMove move);
}
