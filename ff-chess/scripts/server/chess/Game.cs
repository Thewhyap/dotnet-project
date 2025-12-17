using Godot;
using Shared;

namespace Server.Chess;

public partial class Game : Node
{
	private readonly BoardState _board = new();
	private readonly TurnManager _turns = new();
	private readonly RuleManager _rules = new();

	public bool CanPlay(long peerId, long white, long black)
	{
		return (_turns.IsWhiteTurn && peerId == white)
			|| (!_turns.IsWhiteTurn && peerId == black);
	}

	public bool Play(ChessMove move)
	{
		if (!_rules.IsLegal(_board, move, _turns.IsWhiteTurn))
			return false;

		_board.Apply(move);
		_turns.Next();
		return true;
	}
}
