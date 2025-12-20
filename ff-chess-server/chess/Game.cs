namespace FFChessShared;

public class Game(GameState gameState, string name)
{
    public Guid GameId { get; set; }
    public GameState GameState { get; set; } = gameState;
    public MatchStatus Status { get; set; } = MatchStatus.Waiting;
    public TurnStatus TurnStatus { get; set; } = TurnStatus.WaitingMove;
    public string Name { get; set; } = name;
}
