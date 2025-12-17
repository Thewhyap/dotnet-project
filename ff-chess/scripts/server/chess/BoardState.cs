using Shared;

namespace Server.Chess;

public class BoardState
{
	public int[,] Board { get; } = new int[8, 8];

	public void Apply(ChessMove move)
	{
		var piece = Board[move.From.X, move.From.Y];
		Board[move.To.X, move.To.Y] = piece;
		Board[move.From.X, move.From.Y] = 0;
	}
}
