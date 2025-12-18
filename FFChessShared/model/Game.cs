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
	
	/**
	 * Creates a default game with initial game state and empty status
	 */
	public Game()
	{
		GameState = GameState.CreateDefault();
		Status = new MatchStatus();
	}
}
