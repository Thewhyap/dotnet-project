using FFChessShared;

namespace Server.Chess;

public class Knight(PieceColor color) : PieceBase(PieceType.Knight, color)
{
    public override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool ignored)
	{
        int deltaX = move.To.X - move.From.X;
        int deltaY = move.To.Y - move.From.Y;

        return ((Math.Abs(deltaX) == 2 && Math.Abs(deltaY) == 1) || (Math.Abs(deltaX) == 1 && Math.Abs(deltaY) == 2));
    }
}
