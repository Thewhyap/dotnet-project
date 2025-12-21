using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ChessSquare
{
	public ChessSquare() { }
	public ChessSquare(int x, int y)
	{
		X = x; Y = y;
	}

	[Key(0)] public int X { get; set; } = 0;
	[Key(1)] public int Y { get; set; } = 0;
}
