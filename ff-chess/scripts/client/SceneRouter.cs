using Godot;
using System;

// Navigation des scenes
public partial class SceneRouter : Node
{
	 private CanvasLayer _screenRoot;

	private const string MAIN_MENU = "res://scenes/main_menu_screen.tscn";
	private const string LOBBY = "res://scenes/lobby_screen.tscn";
	private const string GAME = "res://scenes/game_screen.tscn";
	private const string SPECTATOR = "res:/scenes/spectator_screen.tscn";
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Not able to load immediately
		CallDeferred(nameof(LoadInitialScene));
	}
	
	 private void ClearScreen()
	{
		if (_screenRoot == null) return;
		foreach (Node child in _screenRoot.GetChildren())
		{
			child.QueueFree();
		}
	}
	
	private void LoadScreen(string path)
	{
		if (_screenRoot == null)
		{
			GD.PrintErr("UI root is null, aborting LoadScreen(", path, ")");
			return;
		}

		ClearScreen();

		PackedScene scene = GD.Load<PackedScene>(path);
		if (scene == null)
		{
			GD.PrintErr("Failed to load scene at path: ", path);
			return;
		}

		GD.Print("Loading screen: ", path);
		Control screen = scene.Instantiate<Control>();
		_screenRoot.AddChild(screen);
		screen.AnchorLeft = 0;
		screen.AnchorTop = 0;
		screen.AnchorRight = 1;
		screen.AnchorBottom = 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void LoadMainMenu(){
		LoadScreen(MAIN_MENU);
	}
	public void LoadLobby(){
		LoadScreen(LOBBY);
	}
	public void LoadGame(){
		LoadScreen(GAME);
	}
	public void LoadSpectator(){
		LoadScreen(SPECTATOR);
	}
	
	public void LoadInitialScene()
	{
		_screenRoot = GetParent().GetNode<CanvasLayer>("UI");
		LoadMainMenu();
	}
}
