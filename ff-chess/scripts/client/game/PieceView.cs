using Godot;
using System;
using FFChessShared;

public partial class PieceView : Node2D
{
	private Piece _piece;
	private Sprite2D _sprite;
	private const float SquareSize = 80f; // Ajustez selon votre taille de case
	private const string Variant = "2";

	public override void _Ready()
	{
		_sprite = new Sprite2D();
		_sprite.Texture = LoadTexture();
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

	public void setCoordinates(int x, int y)
	{
		Position = new Vector2(x * SquareSize, y * SquareSize);
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
