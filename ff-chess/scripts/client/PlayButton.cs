using Godot;
using System;

public partial class PlayButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()	
	{
		Pressed += OnButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnButtonPressed()
	{
		GetNode<SceneRouter>("/root/ClientRoot/GameUpdaterServer/SceneRouter").LoadLobby();
	}
}
