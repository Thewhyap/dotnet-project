using Godot;
using FFChess.scripts.client;
using FFChess.scripts.client.game;
using FFChessShared;
using System.Collections.Generic;

namespace FFChess.scripts.client.game;

public partial class GameScreen : Control
{
	// Model
	private GameState _gameState;
	private GameInfo _gameInfo;
	private TurnStatus _turnStatus;
	private PieceColor _playerColor;
	
	// Nodes
	private ChessBoardView _boardView;
	private Button _quitButton;
	private Label _gameStatusLabel;
	private Label _roleLabel;
	private Label _viewerCountLabel;
	private GameResultModal _gameResultModal;
	private PawnPromotionModal _pawnPromotionModal;
	private Vector2 _selectedPiecePosition = Vector2.Zero; 
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
		_playerColor = PieceColor.White; //TODO EG set a real pieceColor
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
		if (_turnStatus == TurnStatus.Draw || _turnStatus == TurnStatus.WinBlack || _turnStatus == TurnStatus.WinWhite)
		{
			_gameResultModal.ShowResult(_turnStatus);
			_resultModalShown = true;
		}
		
		// Handle the pawn promotion modal display
		if (_turnStatus == TurnStatus.WaitingPromotion || _gameState.CurrentTurn == _playerColor)
		{
			_pawnPromotionModal.ShowPromotionModal();
			ShowPawnPromotionModal = true;
		}
	}
	
	public void SetGameState(GameState gameState)
	{
		_gameState = gameState;
		RenderBoard();
	}
	
	public void SetTurnStatus(TurnStatus turnStatus)
	{
		_turnStatus = turnStatus;
		RenderBoard();
	}
	

	public void SetGameInfo(GameInfo gameInfo)
	{
		_gameInfo = gameInfo;
		RenderBoard();
	}
	
	private void OnViewportSizeChanged()
	{
		// Recalculate square size and re-render board
		GameConstants.CalculateSquareSizeFromViewport(GetViewportRect().Size);
		_boardView.ClearBoard();
		RenderBoard();
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
			GD.Print($"Case cliquée à ({x}, {y}) - TODO EG Move pawn from ({_selectedPiecePosition.X}, {_selectedPiecePosition.Y})");
			var gameUpdater = getGameUpdaterServer();
			ChessSquare from = new ChessSquare((int) _selectedPiecePosition.X, (int) _selectedPiecePosition.Y);
			ChessSquare to = new ChessSquare(x, y);
			ChessMove move = new ChessMove(from, to);
			gameUpdater.SendMovePieceRequest(move);
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
		
		for (int y = 0; y < _gameState.Board.Cells.GetLength(0); y++)
		{
			for (int x = 0; x < _gameState.Board.Cells.GetLength(1); x++)
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
				var maybePiece = _gameState.Board.Cells[y, x];
				if (maybePiece != null)
				{
					PieceData piece = (PieceData) maybePiece;
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
		GD.Print("Quit button pressed");
		var gameUpdater = getGameUpdaterServer();
		gameUpdater.SendQuitGameRequest();
	}
	
	private void OnPawnPromotionPieceSelected(PieceType pieceType)
	{
		GD.Print($"Pawn promotion piece selected in GameScreen: {pieceType}");
		var gameUpdater = getGameUpdaterServer();
		gameUpdater.SendPromoteRequest(pieceType);
	}
	
	private void OnGameResultModalOkPressed()
	{
		GD.Print("Game result modal closed - TODO EG implement what happens after game ends");
		// TODO EG: Return to lobby, restart game, etc.
	}

	private void UpdateRoleDisplay()
	{
		_roleLabel.Text = "Your role: TODO EG"; // TODO EG set role based on player info
	}
	
	private void UpdateViewerCountDisplay()
	{
		_roleLabel.Text = "Viewer count : TODO EG"; // TODO EG get viewer count from game info
	}

	private GameUpdaterServer getGameUpdaterServer()
	{
		return GetNode<GameUpdaterServer>("/root/ClientRoot/GameUpdaterServer");
	}
}
