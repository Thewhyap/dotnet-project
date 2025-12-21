using Godot;
using System;
using FFChessShared;

public partial class LobbyItemScene : Control
{
	private GameInfo _gameInfo;
	private Label _nameLabel;
	private Label _statusLabel;
	private Button _joinButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("LobbyItem ready.");
		_nameLabel = GetNode<Label>("HBoxContainer/NameLabel");
		_statusLabel = GetNode<Label>("HBoxContainer/StatusLabel");
		_joinButton = GetNode<Button>("HBoxContainer/JoinButton");
		
		_joinButton.Pressed += OnJoinPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetData(GameInfo gameInfo)
	{
		if (_nameLabel == null || _statusLabel == null || _joinButton == null)
		{
			GD.PrintErr("Labels not initialized yet!");
			return;
		}
		this._gameInfo = gameInfo;
		_nameLabel.Text = gameInfo.GameName;
		_statusLabel.Text = getReadableGameStatus(gameInfo.Status);
	}

	private void OnJoinPressed()
	{
		GD.Print($"Joining: {_gameInfo.GameName}, ID: {_gameInfo.GameId}");
		var gameUpdater = GetNode<GameUpdaterServer>("/root/ClientRoot/GameUpdaterServer");
		gameUpdater.SendJoinGameRequest(_gameInfo.GameId);
	}
	
	private string getReadableGameStatus(MatchStatus status)
	{
		switch (status)
		{
			case MatchStatus.Waiting:
				return "Waiting for players";
			case MatchStatus.InGame:
				return "In progress";
			case MatchStatus.Closed:
				return "Finished";
			default:
				return "Unknown state";
		}
	}
}
