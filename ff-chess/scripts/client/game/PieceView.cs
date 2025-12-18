using Godot;
using System;
using FFChess.scripts.client;
using FFChess.scripts.client.game;
using FFChessShared;

namespace FFChess.scripts.client.game;

public partial class PieceView : Node2D
{
	private Piece _piece;
	private Sprite2D _sprite;
	private const string Variant = "2";
	private int _gridX;
	private int _gridY;
	private Rect2 _clickRect;
	
	public event Action<int, int> PieceClicked;

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
		
		GD.Print($"PieceView created at ({_gridX}, {_gridY}) with SquareSize: {GameConstants.SquareSize}");
	}

	public void SetPiece(Piece piece)
	{
		_piece = piece;
		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		
	}

	public void setCoordinates(float x, float y)
	{
		Position = new Vector2(x, y);
	}
	
	public void SetGridCoordinates(int x, int y)
	{
		_gridX = x;
		_gridY = y;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			// Convert global mouse position to local position
			Vector2 localMousePos = GetLocalMousePosition();
			
			// Check if click is within this piece's bounds
			if (_clickRect.HasPoint(localMousePos))
			{
				GD.Print($"Piece clicked at ({_gridX}, {_gridY})");
				PieceClicked?.Invoke(_gridX, _gridY);
				GetTree().Root.SetInputAsHandled();
			}
		}
	}
	
	private Texture2D LoadTexture()
	{
		var imagePath = $"res://assets/textures/pawns/{_piece.Color.ToString().ToLower()}_{_piece.Type.ToString().ToLower()}_{Variant}.png";
		Texture2D texture = GD.Load<Texture2D>(imagePath);
		if (texture == null)
		{
			throw new Exception("Failed to load texture at path: " + imagePath);
		}
		return texture;
	}
}
