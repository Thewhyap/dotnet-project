using Godot;
using System;
using FFChess.scripts.client;
using FFChessShared;

public partial class PieceView : Node2D
{
	private Piece _piece;
	private Sprite2D _sprite;
	private const string Variant = "2";

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
