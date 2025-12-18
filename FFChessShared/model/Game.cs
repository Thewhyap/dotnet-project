namespace FFChessShared;

public readonly struct Game
{
	public GameState GameState { get; }
	public MatchStatus Status { get; }

	public Game(GameState gameState, MatchStatus status)
	{
		GameState = gameState;
		Status = status;
	}
}
