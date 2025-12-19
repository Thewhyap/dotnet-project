using FFChessShared;

namespace Server.Match;

public class MatchService
{
    private readonly Dictionary<Guid, MatchSession> _sessions = new();
    private readonly Dictionary<string, Player> _connectedPlayersByIP = new();

    public MatchSession CreateGame(Player creator)
    {
        var initialState = GameInitializer.CreateInitialState();
        var match = new MatchSession(creator, initialState);
        _sessions.Add(match.Id, match);
        creator.AssignedColor = PieceColor.White;
        return match;
    }

    public bool JoinGame(Guid matchId, Player player)
    {
        if (!_sessions.TryGetValue(matchId, out var match)) return false;

        if (!match.CanJoinAsPlayer())
        {
            match.AddViewer(player);
            return true; // joined as viewer
        }

        var assignedColor = match.AssignColorToPlayer(player);
        player.AssignedColor = assignedColor;

        match.StartGameIfReady();
        return true;
    }

    public void PlayerDisconnects(Player player)
    {
        var match = _sessions.Values.FirstOrDefault(m => m.WhitePlayer == player || m.BlackPlayer == player);
        if (match != null)
        {
            match.RemovePlayer(player);
        }
        // Gérer suppression joueur, notify etc.
    }

    public IEnumerable<MatchSession> ListMatches() => _sessions.Values;
}
