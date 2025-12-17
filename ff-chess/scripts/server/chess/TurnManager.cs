namespace Server.Chess;

public class TurnManager
{
	public bool IsWhiteTurn { get; private set; } = true;

	public void Next()
	{
		IsWhiteTurn = !IsWhiteTurn;
	}
}
