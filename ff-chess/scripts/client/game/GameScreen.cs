using Godot;
using System;
using FFChess.scripts.client;
using FFChessShared;

public partial class GameScreen : Control
{
	private Game _gameModel;
	private ChessBoardView _boardView;
	private readonly int _squareSize = 80;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 _gameModel = new Game();
		_boardView = GetNode<ChessBoardView>("ChessBoardView");
		RenderBoard();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnSquareClicked(ChessSquare square)
	{
		// Logique de coup
	}
	
	private void RenderBoard()
	{
		
		// Remove existing pieces
		_boardView.ClearPieces();
		
		for (int y = 0; y < _gameModel.GameState.Board.Cells.GetLength(0); y++)
		{
			for (int x = 0; x < _gameModel.GameState.Board.Cells.GetLength(1); x++)
			{	
				// Scale coordinates
				var scaledX = x * GameConstants.SquareSize;
				var scaledY = y * GameConstants.SquareSize;
				
				// Handle the square background
				var isBlack = (x + y) % 2 == 1;
				var squareView = new SquareView(isBlack);
				squareView.setCoordinates(scaledX, scaledY);
				_boardView.AddChild(squareView);
				
				// Handle the piece on the square
				var maybePiece = _gameModel.GameState.Board.Cells[y, x];
				if (maybePiece != null)
				{
					var piece = maybePiece;
					var pieceView = new PieceView();
					pieceView.SetPiece(piece);
					pieceView.setCoordinates(scaledX, scaledY);
					_boardView.AddChild(pieceView);
					GD.Print("Rendering piece: " + piece.Type );
				}
			}
		}
		
	}
}
