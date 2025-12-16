using Godot;
using System;

public partial class Lobby : Node
{
	private Long LobbyId
	private String Nom
	private Player White;
	private Player Black
	private List<Spectator> Spectators;
	private LobbyState State;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
