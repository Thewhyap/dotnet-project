using Godot;
using System;

public partial class MatchSession : Node
{
	public long WhitePlayer;
	public long BlackPlayer;

	public void Initialize(Lobby lobby)
	{
		WhitePlayer = lobby.White.Value;
		BlackPlayer = lobby.Black.Value;

		AddChild(new ChessGame());
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
