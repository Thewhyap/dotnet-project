using Godot;
using System;
using System.Linq;

public partial class Bootstrap : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var args = OS.GetCmdlineArgs();

		if (args.Contains("--server"))
		{
			GetTree().ChangeSceneToFile("res://scripts/server/server_root.tscn");
		}
		else
		{
			GetTree().ChangeSceneToFile("res://scenes/client_root.tscn");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
