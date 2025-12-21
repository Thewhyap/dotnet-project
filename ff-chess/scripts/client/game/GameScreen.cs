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
	private PieceColor ?_playerColor; // An observer has no color
	private string _playerName;
	private string _playerRole;
	
	// Nodes
	private ChessBoardView _boardView;
	private Button _quitButton;
	private Label _gameStatusLabel;
	private Label _roleLabel;
	private Label _turnStatusLabel;
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
		_boardView = GetNode<ChessBoardView>("HBoxContainer/ChessBoardView");
		_quitButton = GetNode<Button>("HBoxContainer/VBoxContainer/QuitButton");
		_roleLabel = GetNode<Label>("HBoxContainer/VBoxContainer/RoleLabel");
		_turnStatusLabel = GetNode<Label>("HBoxContainer/VBoxContainer/TurnStatusLabel");
		_gameStatusLabel = GetNode<Label>("HBoxContainer/VBoxContainer/GameStatusLabel");
		_gameResultModal = GetNode<GameResultModal>("GameResultModal");
		_pawnPromotionModal = GetNode<PawnPromotionModal>("PawnPromotionModal");
		
		// Calculate square size based on viewport height
		GameConstants.CalculateSquareSizeFromViewport(GetViewportRect().Size);
		_quitButton.Pressed += HandleQuitButtonPressed;
		_gameResultModal.OkPressed += OnGameResultModalOkPressed;
		_pawnPromotionModal.PieceSelected += OnPawnPromotionPieceSelected;
		
		// Don't render board in _Ready - wait for game state to be set
		// RenderBoard();
		
		GetViewportRect();
		GetViewport().SizeChanged += OnViewportSizeChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Handle the game result modal display
		if (_turnStatus == TurnStatus.Draw || _turnStatus == TurnStatus.WinBlack || _turnStatus == TurnStatus.WinWhite)
		{
			if (!_resultModalShown)
			{
				_gameResultModal.ShowResult(_turnStatus);
				_resultModalShown = true;
			}
		}
		
		// Handle the pawn promotion modal display - only when waiting for promotion
		if (_turnStatus == TurnStatus.WaitingPromotion && !ShowPawnPromotionModal)
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
	
	public void SetPlayerColor(PieceColor pieceColor)
	{
		_playerColor = pieceColor;
		UpdateRoleDisplay();
	}
	
	public void SetPlayerName(string playerName)
	{
		_playerName = playerName;
	}

	public void SetGameInfo(GameInfo gameInfo)
	{
		_gameInfo = gameInfo;
		// Don't render here - wait for SetGameState to be called with actual game state
		// RenderBoard();
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
		if (_playerColor == null)
		{
			GD.Print("Obeserver can't play.");
			return;
		}
		
		// Check if clicking on the same piece that's already selected
		if (_hasPieceSelected && _selectedPiecePosition.X == x && _selectedPiecePosition.Y == y)
		{
			// Unselect the piece
			if (_currentSelectedPieceView != null)
			{
				_currentSelectedPieceView.SetSelected(false);
			}
			_hasPieceSelected = false;
			_currentSelectedPieceView = null;
			GD.Print($"Unselected pawn : ({x}, {y})");
			return;
		}
		
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
			if (pieceView.GetPieceData().Color != _playerColor)
			{
				GD.Print("Illegal move : You can't move a pawn which is not yours");
				return;
			}
			
			_currentSelectedPieceView = pieceView;
			_currentSelectedPieceView.SetSelected(true);
		}
		
		GD.Print($"Select pawn : ({x}, {y})");
	}
	
	private void OnSquareClickedHandler(int x, int y)
	{
		if (_hasPieceSelected)
		{
			GD.Print($"Square clicked at ({x}, {y}) - Should move pawn to ({_selectedPiecePosition.X}, {_selectedPiecePosition.Y})");
			var gameUpdater = getGameUpdaterServer();
			ChessSquare from = new ChessSquare((int) _selectedPiecePosition.X, (int) _selectedPiecePosition.Y);
			ChessSquare to = new ChessSquare(x, y);
			ChessMove move = new ChessMove(from, to);
			gameUpdater.SendMovePieceRequest(_gameInfo.GameId, move);
			_hasPieceSelected = false;
		}
		else
		{
			GD.Print($"Square clicked at ({x}, {y}) -No pawn selected");
		}
	}
	
	private void RenderBoard()
	{
		// Don't render if game state is not initialized yet
		if (_gameState == null || _gameState.Board == null || _gameState.Board.Cells == null)
		{
			GD.Print("GameState not initialized yet, skipping render");
			return;
		}
		
		// Don't render if board view is not initialized yet (before _Ready())
		if (_boardView == null)
		{
			GD.Print("BoardView not initialized yet, skipping render");
			return;
		}
		
		// Update UI 
		UpdateRoleDisplay();
		UpdateTurnStatusDisplay();
		
		// Remove existing pieces AND squares (clear everything)
		_boardView.ClearBoard();
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
				var maybePiece = _gameState.Board.Cells[x, y];
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
		GD.Print("[GameScreen] Quit button pressed - Leaving game");
		var gameUpdater = getGameUpdaterServer();
		gameUpdater.SendQuitGameRequest(_gameInfo.GameId);
		GD.Print("[GameScreen] Quit request sent to server, waiting for confirmation...");
		
		// Don't load main menu immediately - wait for server confirmation
		// The HandleGameQuit() in GameUpdaterServer will handle the transition
	}
	
	private void OnPawnPromotionPieceSelected(PieceType pieceType)
	{
		GD.Print($"Pawn promotion piece selected in GameScreen: {pieceType}");
		var gameUpdater = getGameUpdaterServer();
		gameUpdater.SendPromoteRequest(_gameInfo.GameId, pieceType);
	}
	
	private void OnGameResultModalOkPressed()
	{
		GD.Print("Game result modal closed - TODO EG implement what happens after game ends");
		// TODO EG: Return to lobby, restart game, etc.
	}

	private void UpdateRoleDisplay()
	{
		if (_roleLabel == null)
			return;
			
		var role = "Observer";
		if (_playerColor != null)
		{
			role = _playerColor == PieceColor.Black ? "Player (Black)" : "Player (White)";
		}
		
		_roleLabel.Text =  "Your role: "+role;
	}
	
	private void UpdateTurnStatusDisplay()
	{
		if (_turnStatusLabel == null)
			return;
			
		var message = GetTurnStatusMessage();
		_turnStatusLabel.Text = message;
	}

	private string GetTurnStatusMessage()
	{
		return _turnStatus switch
		{
			TurnStatus.WaitingMove => GetWaitingMoveMessage(),
			TurnStatus.WaitingPromotion => GetWaitingPromotionMessage(),
			TurnStatus.Draw => "Draw! - Game ended in a draw",
			TurnStatus.WinBlack => GetVictoryMessage(PieceColor.Black),
			TurnStatus.WinWhite => GetVictoryMessage(PieceColor.White),
			_ => "Unknown game status"
		};
	}
	
	private string GetVictoryMessage(PieceColor winnerColor)
	{
		string colorName = winnerColor == PieceColor.White ? "White" : "Black";
		
		if (_playerColor == null)
		{
			return $"{colorName} won!";
		}
		
		if (winnerColor == _playerColor)
		{
			return $"🎉 {colorName} (You) won! Congratulations!";
		}
		else
		{
			return $"{colorName} (Opponent) won! Better luck next time!";
		}
	}

	private string GetWaitingMoveMessage()
	{
		if (_gameState == null)
			return "Waiting for game state...";
		
		// Obtenir le nom de la couleur actuelle
		string currentColorName = _gameState.CurrentTurn == PieceColor.White ? "White" : "Black";
		
		// Vérifier si c'est notre tour
		bool isMyTurn = _gameState.CurrentTurn == _playerColor;
		
		if (isMyTurn)
		{
			return $"Waiting for {currentColorName} (You) to move...";
		}
		else
		{
			return $"Waiting for {currentColorName} (Opponent) to move...";
		}
	}

	private string GetWaitingPromotionMessage()
	{
		if (_gameState == null)
			return "Waiting for game state...";
		
		// Obtenir le nom de la couleur actuelle
		string currentColorName = _gameState.CurrentTurn == PieceColor.White ? "White" : "Black";
		
		// Vérifier si c'est notre tour
		bool isMyTurn = _gameState.CurrentTurn == _playerColor;
		
		if (isMyTurn)
		{
			return $"Waiting for {currentColorName} (You) to promote a pawn...";
		}
		else
		{
			return $"Waiting for {currentColorName} (Opponent) to promote a pawn...";
		}
	}
	
	public void clearGameData()
	{
		_gameState = null;
		_gameInfo = null;
		_turnStatus = TurnStatus.WaitingMove;
		_playerColor = null;
		_playerName = string.Empty;
		_playerRole = string.Empty;
		_boardView.ClearBoard();
		_pieceViewMap.Clear();
		_currentSelectedPieceView = null;
		_hasPieceSelected = false;
		_resultModalShown = false;
		ShowPawnPromotionModal = false;
	}

	private GameUpdaterServer getGameUpdaterServer()
	{
		return GetNode<GameUpdaterServer>("/root/ClientRoot/GameUpdaterServer");
	}
	
	private SceneRouter GetSceneRouterNode()
	{
		return GetNode<SceneRouter>("/root/ClientRoot/GameUpdaterServer/SceneRouter");
	}
}
