using Godot;
using System;
using FFChess.scripts.client;

public partial class ChessBoardView : Control
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Center the board on screen
		CenterBoard();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void ClearBoard()
	{
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}
	}
	
	public void ClearPieces()
	{
		
	}
	
	private void CenterBoard()
	{
		// Calculate board size
		float boardSize = 8 * GameConstants.SquareSize;
		
		// Get viewport center
		Vector2 viewportCenter = GetViewportRect().GetCenter();
		
		// Position board so it's centered horizontally and starts from top
		Position = new Vector2(viewportCenter.X - boardSize / 2f, 0);
	}
}
