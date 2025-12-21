using FFChessShared;
using System;

namespace Server.Chess;

public class Queen(PieceColor color) : PieceBase(PieceType.Queen, color)
{
    public override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool ignored)
	{
        int deltaX = move.To.X - move.From.X;
        int deltaY = move.To.Y - move.From.Y;

        // Check if the move is horizontal, vertical or diagonal
        if (((deltaY != 0 && deltaX != 0) || (Math.Abs(deltaX) != Math.Abs(deltaY))))
            return false;

        int stepX = deltaX == 0 ? 0 : (deltaX > 0 ? 1 : -1);
        int stepY = deltaY == 0 ? 0 : (deltaY > 0 ? 1 : -1);

        int x = move.From.X + stepX;
        int y = move.From.Y + stepY;

        // Check if a piece blocks the path
        while (x != move.To.X || y != move.To.Y)
        {
            if (state.Board.Cells[x, y] != null)
                return false;

            x += stepX;
            y += stepY;
        }

        return true;
    }
}
