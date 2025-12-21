using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class GameUpdate
{
    [Key(0)] public GameState State { get; set; } = null!;
    [Key(1)] public TurnStatus TurnStatus { get; set; } = TurnStatus.WaitingMove;

    public GameUpdate() { }

    public GameUpdate(GameState state, TurnStatus turnStatus)
    {
        State = state;
        TurnStatus = turnStatus;
    }
}
