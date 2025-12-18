using FFChessShared;

namespace Server.Chess;

public class KingRule : PieceRuleBase
{
	protected override bool IsSpecificMoveLegal(GameState state, ChessMove move)
	{
		//TODO
		return true;
	}
}
