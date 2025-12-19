using FFChessShared;

namespace Server.Chess;

public class Knight(PieceColor color) : PieceBase(PieceType.Knight, color)
{
    public override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool ignored)
	{
        int deltaX = to.X - from.X;
        int deltaY = to.Y - from.Y;

        return ((Math.Abs(deltaX) == 2 && Math.Abs(deltaY) == 1) || (Math.Abs(deltaX) == 1 && Math.Abs(deltaY) == 2));
    }
}
