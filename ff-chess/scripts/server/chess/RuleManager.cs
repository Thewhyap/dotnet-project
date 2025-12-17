using Shared;

namespace Server.Chess;

public class RuleManager
{
	public bool IsLegal(BoardState board, ChessMove move, bool isWhiteTurn)
	{
		// TODO: règles complètes
		if (!move.From.IsValid() || !move.To.IsValid())
			return false;

		return true;
	}
}
