using Godot;
using System.Collections.Generic;

namespace Server.Lobbies;

public partial class Lobby : Node
{
	public long Id { get; }
	public string Name { get; }

	public long? White { get; private set; }
	public long? Black { get; private set; }

	public List<long> Spectators { get; } = new();
	public LobbyState State { get; private set; } = LobbyState.Waiting;

	public Lobby(long id, string name)
	{
		Id = id;
		Name = name;
	}
	
	public void StartGame()
	{
		if (State != LobbyState.Ready)
			return;

		State = LobbyState.InGame;
	}

	public bool JoinAsPlayer(long peerId)
	{
		if (White == null)
		{
			White = peerId;
			UpdateState();
			return true;
		}

		if (Black == null)
		{
			Black = peerId;
			UpdateState();
			return true;
		}

		return false;
	}

	public void JoinAsSpectator(long peerId)
	{
		if (!Spectators.Contains(peerId))
			Spectators.Add(peerId);
	}

	private void UpdateState()
	{
		if (White != null && Black != null)
			State = LobbyState.Ready;
	}
	
	public void Close()
	{
		State = LobbyState.Closed;
	}
}
