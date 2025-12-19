using Godot;
using FFChessShared;

namespace FFChess.scripts.client.game;

public partial class PawnPromotionModal : Control
{
	private Button _queenButton;
	private Button _rookButton;
	private Button _bishopButton;
	private Button _knightButton;
	
	public event System.Action<PieceType> PieceSelected;

	public override void _Ready()
	{
		_queenButton = GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/QueenButton");
		_rookButton = GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/RookButton");
		_bishopButton = GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/BishopButton");
		_knightButton = GetNode<Button>("PanelContainer/VBoxContainer/HBoxContainer/KnightButton");
		
		_queenButton.Pressed += () => OnPieceSelected(PieceType.Queen);
		_rookButton.Pressed += () => OnPieceSelected(PieceType.Rook);
		_bishopButton.Pressed += () => OnPieceSelected(PieceType.Bishop);
		_knightButton.Pressed += () => OnPieceSelected(PieceType.Knight);
		
		// Start hidden
		Visible = false;
	}

	public void ShowPromotionModal()
	{
		Visible = true;
	}

	private void OnPieceSelected(PieceType pieceType)
	{
		GD.Print($"Promotion: Selected {pieceType}");
		PieceSelected?.Invoke(pieceType);
		Visible = false;
	}
}

