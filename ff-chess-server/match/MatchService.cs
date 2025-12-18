using FFChessShared;

namespace Server.Match;

public class MatchService
{
    private readonly Dictionary<Guid, GameSession> _sessions = new();

    public MatchService CreateGame(Player white, Player black)
    {
        var state = GameInitializer.CreateInitialState();
        var session = new GameSession(white, black, state);
        _sessions.Add(session.Id, session);
        return session;
    }

    public GameSession? GetGame(Guid id)
    {
        return _sessions.TryGetValue(id, out var session) ? session : null;
    }
}

