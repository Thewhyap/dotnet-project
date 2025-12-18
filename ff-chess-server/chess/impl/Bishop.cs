using FFChessShared;
using System;

namespace Server.Chess;

public class Bishop : PieceBase
{
    public Bishop(PieceColor color) : base(PieceType.Bishop, color) { }

    protected override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool ignored)
	{
        int deltaX = to.X - from.X;
        int deltaY = to.Y - from.Y;

        // Check if the move is diagonal
        if (Math.Abs(deltaY) != Math.Abs(deltaX))
			return false;

        int stepX = deltaX > 0 ? 1 : -1;
        int stepY = deltaY > 0 ? 1 : -1;

        int x = from.X + stepX;
        int y = from.Y + stepY;

        // Check if a piece blocks the path
        while (x != to.X || y != to.Y)
        {
            if (state.Board[x, y] != null)
                return false;

            x += stepX;
            y += stepY;
        }

        return true;
	}
}
