using Godot;
using System;

public partial class Lobby : Node
{
	private long Id;
	private string Name;
	private long? White;
	private long? Black;
	private List<long> Spectators = new();
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
