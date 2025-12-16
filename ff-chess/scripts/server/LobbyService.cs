using Godot;
using System;

// Gestion des lobbies (avant match)
public partial class LobbyService : Node
{
	private int _nextLobbyId = 1;
	private Dictionary<int, Lobby> _lobbies = new();

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void CreateLobby(string name)
	{
		int id = _nextLobbyId++;

		_lobbies[id] = new Lobby
		{
			Id = id,
			Name = name
		};

		Rpc(nameof(SyncLobbies), SerializeLobbies());
	}

	[Rpc]
	private void SyncLobbies(string json)
	{
		// client side
	}

	private string SerializeLobbies()
	{
		return Json.Stringify(_lobbies.Values);
	}
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
