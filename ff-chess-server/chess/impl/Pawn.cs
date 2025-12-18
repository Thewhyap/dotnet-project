using FFChessShared;
using System;

namespace Server.Chess;

public class Pawn : PieceBase
{
    public Pawn(PieceColor color) : base(PieceType.Pawn, color) { }

    protected override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool inRoque)
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
			return !state.Board.Cells[to.X, to.Y].HasValue;

		// Moving straight (first time)
		if (deltaX == 0 && deltaY == 2 * direction && (from.Y == startRow))
			return !state.Board.Cells[to.X, to.Y].HasValue && !state.Board.Cells[to.X, from.Y + direction].HasValue;

        // Taking piece (and en-passant)
        if (Math.Abs(deltaX) == 1 && deltaY == direction)
			return state.Board.Cells[to.X, to.Y].HasValue || move.To == state.EnPassantTarget || inRoque;
		
        return false;
	}
}
