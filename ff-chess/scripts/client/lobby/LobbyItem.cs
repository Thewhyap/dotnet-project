using Godot;
using System;

public partial class LobbyItem : Control
{
	 private Label _nameLabel;
	private Label _playersLabel;
	private Label _spectatorsLabel;
	private Button _joinButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 _nameLabel = GetNode<Label>("HBoxContainer/NameLabel");
		_playersLabel = GetNode<Label>("HBoxContainer/PlayersLabel");
		_spectatorsLabel = GetNode<Label>("HBoxContainer/SpectatorLabel");
		_joinButton = GetNode<Button>("HBoxContainer/JoinButton");
		
		_joinButton.Pressed += OnJoinPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	  public void SetData(string name, int players, int maxPlayers)
	{
		_nameLabel.Text = name;
		_playersLabel.Text = $"{players}/{maxPlayers}";
	}

	private void OnJoinPressed()
	{
		GD.Print($"Joining: {_nameLabel.Text}");
	}
}
