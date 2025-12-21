using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ChessMove
{
	public ChessMove() { }
	public ChessMove(ChessSquare from, ChessSquare to)
	{
		From = from; To = to; 
	}

	[Key(0)] public ChessSquare From { get; set; } = null!;
	[Key(1)] public ChessSquare To { get; set; } = null!;
}
