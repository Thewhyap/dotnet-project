namespace Shared;

public readonly struct ChessMove
{
	public readonly ChessSquare From;
	public readonly ChessSquare To;
	
	public ChessMove(ChessSquare from, ChessSquare to)
	{
		From = from;
		To = to;
	}
}
