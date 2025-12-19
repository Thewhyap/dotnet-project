using FFChessShared;

namespace Server.Chess;

public class Rook(PieceColor color) : PieceBase(PieceType.Rook, color)
{
    public override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool ignored)
    {
        int deltaX = to.X - from.X;
        int deltaY = to.Y - from.Y;

        // Check if the move is either horizontal or vertical
        if (deltaY != 0 && deltaX != 0)
            return false;

        int stepX = deltaX == 0 ? 0 : (deltaX > 0 ? 1 : -1);
        int stepY = deltaY == 0 ? 0 : (deltaY > 0 ? 1 : -1);

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
