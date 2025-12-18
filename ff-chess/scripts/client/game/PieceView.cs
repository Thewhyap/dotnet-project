using Godot;
using System;
using FFChessShared;

public partial class PieceView : Node2D
{
	private Piece _piece;
	private Sprite2D _sprite;
	private const float SquareSize = 80f; // Ajustez selon votre taille de case
	private const string Variant = "1";

	public override void _Ready()
	{
		_sprite = new Sprite2D();
		AddChild(_sprite);
	}

	public void SetPiece(Piece piece)
	{
		_piece = piece;
		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		// Charge l'image selon le type et la couleur
		var imagePath = $"res://assets/textures/pawns/{_piece.Color.ToString().ToLower()}_{_piece.Type.ToString().ToLower()}_{Variant}.png";
		_sprite.Texture = GD.Load<Texture2D>(imagePath);
	}

	public void setCoordinates(int x, int y)
	{
		Position = new Vector2(x, y);
	}
}
