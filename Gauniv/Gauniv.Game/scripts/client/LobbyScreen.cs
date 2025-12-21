using Godot;
using System.Collections.Generic;
using FFChessShared;


public partial class LobbyScreen : Control
{
	
	private VBoxContainer _lobbyList;
	private Button _createLobbyButton;
	private PackedScene _lobbyItemScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 _lobbyList = GetNode<VBoxContainer>("VBoxContainer/LobbyList");
		 _createLobbyButton = GetNode<Button>("VBoxContainer/ActionButtons/CreateLobbyButton");
		 _createLobbyButton.Pressed += OnCreateLobbyButtonPressed;
		 _lobbyItemScene = GD.Load<PackedScene>("res://scripts/client/lobby/LobbyItemScene.tscn");
		 
		 // Request the list of games from the server when the lobby screen loads
		 RequestGamesList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	/**
	 * Request the games list from the server
	 */
	private void RequestGamesList()
	{
		GD.Print("LobbyScreen: Requesting games list from server...");
		var gameUpdater = GetGameUpdater();
		gameUpdater.SendRequestGamesList();
	}
	
	/*
	 * Display the list of game lobbies
	 */
	 public void DisplayGamesLobbies(GameInfo[] lobbies)
	{
		// Clear existing lobby items before displaying new ones
		ClearLobbyList();
		
		GD.Print($"LobbyScreen: Displaying {lobbies.Length} game lobbies");
		
		foreach (var gameLobby in lobbies)
		{
			var item = _lobbyItemScene.Instantiate<LobbyItemScene>();
			_lobbyList.AddChild(item);
			item.SetData(gameLobby);
		}
	}
	
	/**
	 * Clear all lobby items from the list
	 */
	private void ClearLobbyList()
	{
		foreach (Node child in _lobbyList.GetChildren())
		{
			child.QueueFree();
		}
	}
	 

	private void OnCreateLobbyButtonPressed()
	{
		// The sever will create a new Lobby and return it's data.
		GD.Print("Create Lobby button pressed.");
		var gameUpdater = GetGameUpdater();
		gameUpdater.SendCreateGameRequest();
	}
	
	/**
	 * Helper to get the GameUpdaterServer node
	 */
	private GameUpdaterServer GetGameUpdater()
	{
		return GetNode<GameUpdaterServer>("/root/ClientRoot/GameUpdaterServer");
	}
}
