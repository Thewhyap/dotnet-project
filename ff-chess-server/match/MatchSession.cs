using FFChessShared;

namespace Server.Match;

public class MatchSession
{
    public Guid Id { get; } = Guid.NewGuid();

    public GameState State { get; }
    public Player WhitePlayer { get; }
    public Player BlackPlayer { get; }

    private readonly List<IClient> _viewers = new();

    public MatchSession(Player white, Player black, GameState initialState)
    {
        WhitePlayer = white;
        BlackPlayer = black;
        State = initialState;
    }

    public bool TryMakeMove(Player player, ChessMove move)
    {
        if (!IsPlayerTurn(player))
            return false;

        if (!GameRules.IsMoveLegal(State, move))
            return false;

        GameRules.ApplyMove(State, move);
        BroadcastState();
        return true;
    }

    private bool IsPlayerTurn(Player player)
    {
        return (State.CurrentTurn == PieceColor.White && player == WhitePlayer)
            || (State.CurrentTurn == PieceColor.Black && player == BlackPlayer);
    }

    public void AddViewer(IClient viewer)
    {
        _viewers.Add(viewer);
        viewer.Send(State);
    }

    private void BroadcastState()
    {
        WhitePlayer.Send(State);
        BlackPlayer.Send(State);
        foreach (var v in _viewers)
            v.Send(State);
    }
}
