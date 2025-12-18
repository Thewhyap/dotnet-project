namespace FFChessShared;

public readonly struct Game
{
	public string Name { get; }
	public GameState GameState { get; }
	public MatchStatus Status { get; }

	public Game(string name,GameState gameState, MatchStatus status)
	{
		Name = name;
		GameState = gameState;
		Status = status;
	}
	
	/**
	 * Creates a default game with initial game state and empty status
	 */
	public Game()
	{
		Name = "Default_Game_Name";
		GameState = GameState.CreateDefault();
		Status = new MatchStatus();
	}
}
