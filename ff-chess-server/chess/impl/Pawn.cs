using FFChessShared;
using System;

namespace Server.Chess;

public class Pawn(PieceColor color) : PieceBase(PieceType.Pawn, color)
{
    public override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool inRoque)
	{
		Board board = state.Board;
		int direction = (state.CurrentTurn == PieceColor.White) ? 1 : -1;
		int startRow = (state.CurrentTurn == PieceColor.White) ? 1 : 6;
		var from = move.From;
		var to = move.To;
		int deltaX = to.X - from.X;
		int deltaY = to.Y - from.Y;

		// Moving straight
		if (deltaX == 0 && deltaY == direction)
			return state.Board.Cells[to.X, to.Y] == null;

		// Moving straight (first time)
		if (deltaX == 0 && deltaY == 2 * direction && (from.Y == startRow))
			return state.Board.Cells[to.X, to.Y] == null && state.Board.Cells[to.X, from.Y + direction] == null;

        // Taking piece (and en-passant)
        if (Math.Abs(deltaX) == 1 && deltaY == direction)
			return state.Board.Cells[to.X, to.Y] != null || move.To == state.EnPassantTarget || inRoque;
		
        return false;
	}
}
