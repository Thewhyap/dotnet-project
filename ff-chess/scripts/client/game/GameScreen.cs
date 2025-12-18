using Godot;
using System;
using FFChessShared;

public partial class GameScreen : Control
{
	private Game _gameModel;
	private ChessBoardView _boardView;
	
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
		// Efface les pièces affichées précédemment
		_boardView.ClearPieces();
	
		// Récupère toutes les pièces du modèle partagé
		var allPieces = _gameModel.GameState.Board.GetAllPieces();


		for (int y = 0; y < _gameModel.GameState.Board.Cells.GetLength(0); y++)
		{
			for (int x = 0; x < _gameModel.GameState.Board.Cells.GetLength(1); x++)
			{
				var maybePiece = _gameModel.GameState.Board.Cells[y, x];
				if (maybePiece != null)
				{
					var piece = maybePiece.Value;
					var pieceView = new PieceView();
					pieceView.SetPiece(piece);
					pieceView.setCoordinates(x, y);
					_boardView.AddChild(pieceView);
					GD.Print("Rendering piece: " + piece.Type );
				}
			}
		}
		
	}
}
