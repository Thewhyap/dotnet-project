using Godot;
using System.Collections.Generic;

namespace Server.Lobbies;

public partial class LobbyService : Node
{
	private long _nextId = 1;
	private readonly Dictionary<long, Lobby> _lobbies = new();

	public Lobby CreateLobby(string name)
	{
		var lobby = new Lobby(_nextId++, name);
		_lobbies[lobby.Id] = lobby;
		AddChild(lobby);
		return lobby;
	}

	public Lobby? GetLobby(long id)
		=> _lobbies.GetValueOrDefault(id);

	public void RemoveLobby(long id)
	{
		if (_lobbies.TryGetValue(id, out var lobby))
		{
			lobby.QueueFree();
			_lobbies.Remove(id);
		}
	}
}
