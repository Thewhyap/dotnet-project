using Godot;
using System.Collections.Generic;
using Server.Lobbies;

namespace Server.Match;

public partial class MatchService : Node
{
	private long _nextMatchId = 1;
	private readonly Dictionary<long, MatchSession> _matches = new();

	public MatchSession CreateFromLobby(Lobby lobby)
	{
		var match = new MatchSession(
			_nextMatchId++,
			lobby.White!.Value,
			lobby.Black!.Value
		);

		_matches[match.MatchId] = match;
		AddChild(match);

		lobby.StartGame();
		return match;
	}

	public MatchSession? Get(long id)
		=> _matches.GetValueOrDefault(id);

	public void Close(long id)
	{
		if (_matches.TryGetValue(id, out var match))
		{
			match.QueueFree();
			_matches.Remove(id);
		}
	}
}
