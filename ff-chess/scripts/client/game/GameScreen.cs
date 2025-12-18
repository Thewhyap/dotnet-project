using Godot;
using FFChess.scripts.client;
using FFChess.scripts.client.game;
using FFChessShared;

namespace FFChess.scripts.client.game;

public partial class GameScreen : Control
{
	private Game _gameModel;
	private ChessBoardView _boardView;
	private Vector2 _selectedPiecePosition = Vector2.Zero; // Stocke les coordonnées du pion sélectionné
	private bool _hasPieceSelected = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameModel = new Game();
		_boardView = GetNode<ChessBoardView>("ChessBoardView");
		
		// Calculate square size based on viewport height
		GameConstants.CalculateSquareSizeFromViewport(GetViewportRect().Size);
		
		RenderBoard();
		
		GetViewportRect();
		GetViewport().SizeChanged += OnViewportSizeChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnViewportSizeChanged()
	{
		// Recalculate square size and re-render board
		GameConstants.CalculateSquareSizeFromViewport(GetViewportRect().Size);
		_boardView.ClearBoard();
		RenderBoard();
	}
	
	public void OnSquareClicked(ChessSquare square)
	{
		// Logique de coup
	}
	
	private void OnPieceClicked(int x, int y)
	{
		// GD.Print($"GameScreen: OnPieceClicked called with ({x}, {y})");
		_selectedPiecePosition = new Vector2(x, y);
		_hasPieceSelected = true;
		GD.Print($"Select pawn : ({x}, {y})");
	}
	
	private void OnSquareClickedHandler(int x, int y)
	{
		if (_hasPieceSelected)
		{
			GD.Print($"Case cliquée à ({x}, {y}) - TODO Move pawn from ({_selectedPiecePosition.X}, {_selectedPiecePosition.Y})");
			_hasPieceSelected = false;
		}
		else
		{
			GD.Print($"Square clicked at ({x}, {y}) -No pawn selected");
		}
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
			var squareView = new SquareView(isBlack, x, y);
			squareView.setCoordinates(scaledX, scaledY);
			squareView.SquareClicked += OnSquareClickedHandler;
			_boardView.AddChild(squareView);
				
				// Handle the piece on the square
				var maybePiece = _gameModel.GameState.Board.Cells[y, x];
				if (maybePiece != null)
				{
					Piece piece = (Piece) maybePiece;
					var pieceView = new PieceView();
					pieceView.SetPiece(piece);
					pieceView.setCoordinates(scaledX, scaledY);
					pieceView.SetGridCoordinates(x, y);
					pieceView.PieceClicked += OnPieceClicked;
					_boardView.AddChild(pieceView);
					// GD.Print("Rendering piece: " + piece.Type );
				}
			}
		}
		
	}
}
