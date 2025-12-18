using Godot;
using System;
using FFChess.scripts.client;
using FFChess.scripts.client.game;

namespace FFChess.scripts.client.game;

public partial class SquareView : Node2D
{
	private bool _isBlack;
	private const string Variant = "1";
	private Sprite2D _sprite;
	private int _gridX;
	private int _gridY;
	private Rect2 _clickRect;
	
	public event Action<int, int> SquareClicked;
	
	public SquareView() { }
	
	public SquareView(bool isBlack, int gridX, int gridY)
	{
		_isBlack = isBlack;
		_gridX = gridX;
		_gridY = gridY;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = new Sprite2D();
		_sprite.Texture = LoadTexture();
		_sprite.Centered = false;
		
		// Scale the sprite to fit within in the given square size
		float textureWidth = _sprite.Texture.GetWidth();
		float textureHeight = _sprite.Texture.GetHeight();
		float scaleX = GameConstants.SquareSize / textureWidth;
		float scaleY = GameConstants.SquareSize / textureHeight;
 
		_sprite.Scale = new Vector2(scaleX, scaleY);
		AddChild(_sprite);
		
		// Define clickable rectangle
		_clickRect = new Rect2(Vector2.Zero, new Vector2(GameConstants.SquareSize, GameConstants.SquareSize));
		
		GD.Print($"SquareView created at ({_gridX}, {_gridY})");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			// Convert global mouse position to local position
			Vector2 localMousePos = GetLocalMousePosition();
			
			// Check if click is within this square's bounds
			if (_clickRect.HasPoint(localMousePos))
			{
				GD.Print($"Square clicked at ({_gridX}, {_gridY})");
				SquareClicked?.Invoke(_gridX, _gridY);
				GetTree().Root.SetInputAsHandled();
			}
		}
	}

	public void setCoordinates(float x, float y)
	{
		Position = new Vector2(x, y);
	}
	
	private Texture2D LoadTexture()
	{
		var color = _isBlack ? "black" : "white";
		var imagePath = $"res://assets/textures/squares/{color}_square_{Variant}.png";
		Texture2D texture = GD.Load<Texture2D>(imagePath);
		if (texture == null)
		{
			throw new Exception("Failed to load texture at path: " + imagePath);
		}
		return texture;
	}
}
