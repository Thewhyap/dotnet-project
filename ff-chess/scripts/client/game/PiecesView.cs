using Godot;
using System;
using FFChessShared;

public partial class PiecesView : Control
{
	private Piece _model;
	
	public void SetPiece(Piece piece)
	{
		_model = piece;
		UpdateVisuals();
	}
	
	private void UpdateVisuals()
	{
		// Positionnement, texture selon le type/couleur
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
}
