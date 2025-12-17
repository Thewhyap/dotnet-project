using Godot;
using System;
using System.Collections.Generic;
using FFChess.data;

public partial class LobbyScreen : Control
{
	private VBoxContainer _lobbyList;
	private PackedScene _lobbyItemScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 _lobbyList = GetNode<VBoxContainer>("LobbyList");
		_lobbyItemScene = GD.Load<PackedScene>("res://scripts/client/lobby/LobbyItemScene.tscn");

		LoadLobbies();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	 private void LoadLobbies()
	{
		foreach (var lobby in GetLobbiesFromServer())
		{
			var item = _lobbyItemScene.Instantiate<LobbyItemScene>();
			_lobbyList.AddChild(item);
			item.SetData(lobby.Name, lobby.Players, lobby.MaxPlayers);
		}
		GD.Print("Lobbies loaded.");
	}

	private List<LobbyData> GetLobbiesFromServer()
	{
		return new List<LobbyData>
		{
			new LobbyData("Lobby 1", 4, 8),
			new LobbyData("Lobby 2", 2, 8)
			 /* TODO add server data */
		};
	}
}
