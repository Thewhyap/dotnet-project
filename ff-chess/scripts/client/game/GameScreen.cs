using Godot;
using FFChess.scripts.client;
using FFChess.scripts.client.game;
using FFChessShared;
using System.Collections.Generic;

namespace FFChess.scripts.client.game;

public partial class GameScreen : Control
{
	private Game _game;
	private ChessBoardView _boardView;
	private Button _quitButton;
	private Label _gameStatusLabel;
	private Label _roleLabel;
	private Label _viewerCountLabel;
	private GameResultModal _gameResultModal;
	private PawnPromotionModal _pawnPromotionModal;
	private Vector2 _selectedPiecePosition = Vector2.Zero; // Stocke les coordonnées du pion sélectionné
	private bool _hasPieceSelected = false;
	
	// Handle piece selection highlighting
	private PieceView _currentSelectedPieceView = null; // Ref to the selected piece view
	private Dictionary<Vector2, PieceView> _pieceViewMap = new();
	
	// Track if result modal has been shown to avoid showing it repeatedly
	private bool _resultModalShown = false;
	
	// Flag to control pawn promotion modal display
	public bool ShowPawnPromotionModal = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_game = new Game();
		_boardView = GetNode<ChessBoardView>("HBoxContainer/ChessBoardView");
		_quitButton = GetNode<Button>("HBoxContainer/VBoxContainer/QuitButton");
		_roleLabel = GetNode<Label>("HBoxContainer/VBoxContainer/RoleLabel");
		_viewerCountLabel = GetNode<Label>("HBoxContainer/VBoxContainer/ViewerCountLabel");
		_gameStatusLabel = GetNode<Label>("HBoxContainer/VBoxContainer/GameStatusLabel");
		_gameResultModal = GetNode<GameResultModal>("GameResultModal");
		_pawnPromotionModal = GetNode<PawnPromotionModal>("PawnPromotionModal");
		
		// Calculate square size based on viewport height
		GameConstants.CalculateSquareSizeFromViewport(GetViewportRect().Size);
		_quitButton.Pressed += HandleQuitButtonPressed;
		_gameResultModal.OkPressed += OnGameResultModalOkPressed;
		_pawnPromotionModal.PieceSelected += OnPawnPromotionPieceSelected;
		
		RenderBoard();
		
		GetViewportRect();
		GetViewport().SizeChanged += OnViewportSizeChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Handle the game result modal display
		// if (!_resultModalShown && _game.Status is MatchStatus.WhiteWon or MatchStatus.BlackWon or MatchStatus.Draw) TODO UNCOMMENT
		if(false)
		{
			_gameResultModal.ShowResult(_game.Status);
			_resultModalShown = true;
		}
		
		// Handle the pawn promotion modal display
		if (ShowPawnPromotionModal)
		{
			_pawnPromotionModal.ShowPromotionModal();
			ShowPawnPromotionModal = false;
		}
	}
	
	public void SetGame(Game game)
	{
		_game = game;
		RenderBoard();
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
		// Unselect previous piece if any
		if (_currentSelectedPieceView != null)
		{
			_currentSelectedPieceView.SetSelected(false);
		}
		
		// Select the the new piece
		_selectedPiecePosition = new Vector2(x, y);
		_hasPieceSelected = true;
		
		// Update the selected piece view for the highlighting
		Vector2 key = new Vector2(x, y);
		if (_pieceViewMap.TryGetValue(key, out var pieceView))
		{
			_currentSelectedPieceView = pieceView;
			_currentSelectedPieceView.SetSelected(true);
		}
		
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
		
		// Update UI 
		UpdateRoleDisplay();
		UpdateViewerCountDisplay();
		
		// Remove existing pieces
		_boardView.ClearPieces();
		_pieceViewMap.Clear();
		_currentSelectedPieceView = null;
		_hasPieceSelected = false;
		
		for (int y = 0; y < _game.GameState.Board.Cells.GetLength(0); y++)
		{
			for (int x = 0; x < _game.GameState.Board.Cells.GetLength(1); x++)
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
				var maybePiece = _game.GameState.Board.Cells[y, x];
				if (maybePiece != null)
				{
					Piece piece = (Piece) maybePiece;
					var pieceView = new PieceView();
					pieceView.SetPiece(piece);
					pieceView.setCoordinates(scaledX, scaledY);
					pieceView.SetGridCoordinates(x, y);
					pieceView.PieceClicked += OnPieceClicked;
					_boardView.AddChild(pieceView);
					_pieceViewMap[new Vector2(x, y)] = pieceView;
					// GD.Print("Rendering piece: " + piece.Type );
				}
			}
		}
		
	}

	private void HandleQuitButtonPressed()
	{
		GD.Print("Quit button pressed - TODO implement quit logic");
		//TODO implement quit logic
	}
	
	private void OnPawnPromotionPieceSelected(PieceType pieceType)
	{
		GD.Print($"Pawn promotion piece selected in GameScreen: {pieceType}");
		// TODO: Use this piece type to complete the pawn promotion
	}
	
	private void OnGameResultModalOkPressed()
	{
		GD.Print("Game result modal closed - TODO implement what happens after game ends");
		// TODO: Return to lobby, restart game, etc.
	}

	private void UpdateRoleDisplay()
	{
		_roleLabel.Text = "Your role: TODO"; // TODO set role based on player info
	}
	
	private void UpdateViewerCountDisplay()
	{
		_roleLabel.Text = "Viewer count : TODO"; // TODO get viewer count from game info
	}
}
