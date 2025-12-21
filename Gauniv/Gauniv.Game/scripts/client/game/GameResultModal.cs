using Godot;
using FFChessShared;

namespace Gauniv.Game.scripts.client.game;

public partial class GameResultModal : Control
{
	private Label _titleLabel;
	private Button _okButton;
	
	public event System.Action OkPressed;

	public override void _Ready()
	{
		_titleLabel = GetNode<Label>("PanelContainer/VBoxContainer/TitleLabel");
		_okButton = GetNode<Button>("PanelContainer/VBoxContainer/OkButton");
		
		_okButton.Pressed += OnOkPressed;
		
		// Start hidden
		Visible = false;
	}

	public void ShowResult(TurnStatus status)
	{
		string title = status switch
		{
			TurnStatus.WinWhite => "White Won!",
			TurnStatus.WinBlack => "Black Won!",
			TurnStatus.Draw => "Draw!",
			_ => "Game Over"
		};
		
		_titleLabel.Text = title;
		Visible = true;
	}

	private void OnOkPressed()
	{
		Visible = false;
		OkPressed?.Invoke();
	}
}
