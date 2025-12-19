using Godot;
using FFChessShared;

namespace FFChess.scripts.client.game;

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

	public void ShowResult(MatchStatus status)
	{
		string title = status switch
		{
			MatchStatus.WhiteWon => "White Won!",
			MatchStatus.BlackWon => "Black Won!",
			MatchStatus.Draw => "Draw!",
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

