using Godot;
using System;
using System.Collections.Generic;

// Gestion des lobbies (avant match)
public partial class LobbyService : Node
{
	public int _nextLobbyId = 1;
	public Dictionary<int, Lobby> _lobbies = new();

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
		var arr = new Godot.Collections.Array();
		foreach (var lobby in _lobbies.Values)
		{
			var dict = new Godot.Collections.Dictionary
			{
				["id"] = lobby.Id,
				["name"] = lobby.Name
			};
			arr.Add(dict);
		}
		return Json.Stringify(arr);
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
