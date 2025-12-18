using Godot;
using System;
using FFChess.scripts.client.game;
using FFChessShared;

// Navigation des scenes
public partial class SceneRouter : Node
{
  private CanvasLayer _screenRoot;
  private bool _initialLoaded = false;
  private string _currentScreenPath = null;

  private const string MAIN_MENU = "res://scenes/main_menu_screen.tscn";
  private const string LOBBY = "res://scenes/lobby_screen.tscn";
  private const string GAME = "res://scenes/game_screen.tscn";
  private const string SPECTATOR = "res://scenes/spectator_screen.tscn";
  
  /**
   * Pending game to pass to the game screen once loaded
   */
  private Game ?_pendingGame;

  public override void _Ready()
  {
		CallDeferred(nameof(LoadInitialScene));
  }

  private void ClearScreen()
  {
	if (_screenRoot == null) return;
	foreach (Node child in _screenRoot.GetChildren())
	{
	  // don't remove this router or any other SceneRouter instances
	  if (child == this || child is SceneRouter) continue;
	  child.QueueFree();
	}
	_currentScreenPath = null;
  }

  private void LoadScreen(string path)
  {
	if (_screenRoot == null)
	{
	  GD.PrintErr("UI root is null, aborting LoadScreen(", path, ")");
	  return;
	}

	if (path == _currentScreenPath) return;

	ClearScreen();

	PackedScene scene = GD.Load<PackedScene>(path);
	if (scene == null)
	{
	  GD.PrintErr("Unable to load scene at path: ", path, "\n file not found.");
	  return;
	}

	GD.Print("Loading screen: ", path);
	Control screen = scene.Instantiate<Control>();
  
	_screenRoot.AddChild(screen);
  
	// Defer removal to allow _Ready to complete, then remove nested routers
	CallDeferred(nameof(RemoveNestedRouters), screen);
  
	screen.AnchorLeft = 0;
	screen.AnchorTop = 0;
	screen.AnchorRight = 1;
	screen.AnchorBottom = 1;

	_currentScreenPath = path;
  }

  private void RemoveNestedRouters(Node node)
  {
	if (node == null) return;
  
	var children = new System.Collections.Generic.List<Node>(node.GetChildren());
	foreach (Node child in children)
	{
	  if (child is SceneRouter router && router != this)
	  {
		GD.Print("Removing nested SceneRouter from loaded screen.");
		router.QueueFree();
	  }
	  else
	  {
		RemoveNestedRouters(child);
	  }
	}
  }


  public override void _Process(double delta)
  {
  }

  public void LoadMainMenu(){ LoadScreen(MAIN_MENU); }
  public void LoadLobby(){ LoadScreen(LOBBY); }

  public void LoadGame(Game game)
  {
	  LoadScreen(GAME);
	  _pendingGame = game;
	  CallDeferred(nameof(PassGameToScreen));
  }
  public void LoadSpectator(){ LoadScreen(SPECTATOR); }

  public void LoadInitialScene()
  {
	if (_initialLoaded) return;
	_initialLoaded = true;

	// Try current scene first, then parent fallback
	_screenRoot = GetTree().CurrentScene?.GetNodeOrNull<CanvasLayer>("UI")
				  ?? GetParent()?.GetNodeOrNull<CanvasLayer>("UI");

	if (_screenRoot == null)
	{
	  // If a CanvasLayer isn't present, try Control (useful during scene setup)
	  var maybeControl = GetTree().CurrentScene?.GetNodeOrNull<Control>("UI")
						 ?? GetParent()?.GetNodeOrNull<Control>("UI");
	  if (maybeControl != null)
	  {
		GD.Print("Found UI as Control, using its parent CanvasLayer if available.");
	  }
	  GD.PrintErr("UI CanvasLayer not found. Aborting initial load.");
	  return;
	}

	GD.Print("UI root found: ", _screenRoot.GetPath());
	LoadMainMenu();
  }
  
  private void PassGameToScreen()
  {
	  if (_screenRoot == null || _screenRoot.GetChildCount() == 0) return;
	
	  Control screen = _screenRoot.GetChild<Control>(_screenRoot.GetChildCount() - 1);

	  if (_pendingGame == null)
	  {
		  throw new InvalidOperationException("No pending game to pass to GameScreen, please set _pendingGame before calling PassGameToScreen.");
	  }
	  
	  if (screen is GameScreen gameScreen)
	  {
		  gameScreen.SetGame((Game) _pendingGame);
		  _pendingGame = null;
	  }
  }
}
