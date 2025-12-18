using Godot;
using System;
using System.Collections.Generic;
using FFChess.data;
using FFChessShared;
using FFChessShared.generators;

public partial class LobbyScreen : Control
{
	
	private VBoxContainer _lobbyList;
	private Button _createLobbyButton;
	private PackedScene _lobbyItemScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 _lobbyList = GetNode<VBoxContainer>("CenterContainer/VBoxContainer/LobbyList");
		 _createLobbyButton = GetNode<Button>("CenterContainer/VBoxContainer/ActionButtons/CreateLobbyButton");
		 _createLobbyButton.Pressed += OnCreateLobbyButtonPressed;
		 _lobbyItemScene = GD.Load<PackedScene>("res://scripts/client/lobby/LobbyItemScene.tscn");
		 
		 LoadLobbies();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	 private void LoadLobbies()
	{
		foreach (var gameLobby in GetLobbiesFromServer())
		{
			var item = _lobbyItemScene.Instantiate<LobbyItemScene>();
			_lobbyList.AddChild(item);
			item.SetData(gameLobby);
		}
		GD.Print("Lobbies loaded.");
	}

	private List<Game> GetLobbiesFromServer()
	{
		// TODO replace with actual server call
		return new List<Game>
		{
			new Game(),
			new Game()
			 
		};
	}

	private void OnCreateLobbyButtonPressed()
	{
		// TODO implement lobby creation logic (Server call)
		// The sever will create a new Lobby and return it's data.
		GD.Print("Create Lobby button pressed.");
		
		// Simulate the game returned from server
		var gameFromServer = new Game();
		GetNode<SceneRouter>("/root/ClientRoot/SceneRouter").LoadGame(gameFromServer);
	}
}
