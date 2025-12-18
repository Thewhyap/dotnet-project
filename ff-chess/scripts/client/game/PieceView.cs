using Godot;
using System;
using FFChessShared;

public partial class PieceView : Node2D
{
	private Piece _piece;
	private Sprite2D _sprite;
	private const float SquareSize = 80f; // Ajustez selon votre taille de case

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
		var imagePath = $"res://assets/pieces/{_piece.Color}_{_piece.Type}.png";
		_sprite.Texture = GD.Load<Texture2D>(imagePath);

		// Positionne la pièce sur la case
		Position = new Vector2(
			_piece.Square.File * SquareSize,
			_piece.Square.Rank * SquareSize
		);
	}
}
