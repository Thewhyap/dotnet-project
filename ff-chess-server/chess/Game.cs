namespace FFChessShared;

public class Game
{
    public Game() { }

    public Game(GameState gameState, string name)
    {
        GameState = gameState;
        Name = name;
    }

    public Guid GameId { get; set; }
    public GameState GameState { get; set; } = null!;
    public MatchStatus Status { get; set; } = MatchStatus.Waiting;
    public TurnStatus TurnStatus { get; set; } = TurnStatus.WaitingMove;
    public bool HasWhite { get; set; } = false;
    public bool HasBlack { get; set; } = false;
    public string Name { get; set; } = null!;
}
