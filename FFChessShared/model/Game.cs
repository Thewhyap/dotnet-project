using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class Game
{
    public Game() { }

    public Game(GameState gameState, string name)
    {
        GameState = gameState;
        Name = name;
    }

    [Key(0)] public Guid GameId { get; set; }
    [Key(1)] public GameState GameState { get; set; } = null!;
    [Key(2)] public MatchStatus Status { get; set; } = MatchStatus.Waiting;
    [Key(3)] public TurnStatus TurnStatus { get; set; } = TurnStatus.WaitingMove;
    [Key(4)] public bool HasWhite { get; set; } = false;
    [Key(5)] public bool HasBlack { get; set; } = false;
    [Key(6)] public string Name { get; set; } = null!;
}
