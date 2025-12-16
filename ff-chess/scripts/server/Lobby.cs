using Godot;
using System;
using System.Collections.Generic;
using DefaultNamespace;

public partial class Lobby : Node
{
	public long Id;
	public string Name;
	public long? White;
	public long? Black;
	public List<long> Spectators = new();
	public LobbyState State;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
