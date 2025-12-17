using Godot;
using System;

public partial class ClientRoot : Node2D
{
	private SceneRouter _sceneRouter;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CallDeferred(nameof(LoadInitialScene));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void LoadInitialScene()
	{
		GetTree().ChangeSceneToFile("res://scenes/client_root.tscn");
	}
}
