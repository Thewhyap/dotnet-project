using FFChessShared;

namespace Server.Chess;

public class QueenRule : PieceRuleBase
{
	protected override bool IsSpecificMoveLegal(GameState state, ChessMove move)
	{
		//TODO
		return true;
	}
}
