using Godot;
using Server.Chess;
using Shared;

namespace Server.Match;

public partial class MatchSession : Node
{
	public long MatchId { get; }
	public long White { get; }
	public long Black { get; }

	public Game Game { get; }

	public MatchSession(long matchId, long white, long black)
	{
		MatchId = matchId;
		White = white;
		Black = black;

		Game = new Game();
		AddChild(Game);
	}

	public bool HandleMove(long peerId, ChessMove move)
	{
		if (!Game.CanPlay(peerId, White, Black))
			return false;

		return Game.Play(move);
	}
}
