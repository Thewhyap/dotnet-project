using FFChessShared;
using System.Collections.Concurrent;

namespace Server.Match;

public class MatchService
{
    public static MatchService Instance { get; } = new();

    private readonly ConcurrentDictionary<Guid, MatchSession> _sessions = new();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _pendingDisconnects = new();

    private static readonly TimeSpan DisconnectTimeout = TimeSpan.FromSeconds(15);

    public async Task<MatchSession> CreateAndJoinGame(Player creator)
    {
        Console.WriteLine($"Creating game, by {creator}");
        var session = new MatchSession();
        _sessions[session.GameManager.Game.GameId] = session;

        await session.AddPlayer(creator);
        await BroadcastGamesList();

        return session;
    }

    public async Task<bool> JoinGame(Player player, Guid gameId)
    {
        Console.WriteLine($"Joining game, by {gameId}");
        if (!_sessions.TryGetValue(gameId, out var session))
            return false;

        await session.AddPlayer(player);
        await BroadcastGamesList();

        return true;
    }

    public async Task<bool> QuitGame(Player player, Guid gameId)
    {
        Console.WriteLine($"Quitting game, by {gameId}");
        if (!_sessions.TryGetValue(gameId, out var session))
            return false;

        if (!session.ContainsPlayer(player))
            return false;

        await session.RemovePlayer(player);
        _sessions.TryRemove(gameId, out _);

        await BroadcastGamesList();
        return true;
    }

    public async Task<bool> TryMove(Player player, Guid gameId, ChessMove move)
    {
        Console.WriteLine($"Trying move, by {move}");
        if (!_sessions.TryGetValue(gameId, out var session))
            return false;

        if (!session.ContainsPlayer(player))
            return false;

        bool result = await session.TryMakeMove(player, move);

        if (IsSessionOver(session))
        {
            _sessions.TryRemove(gameId, out _);
            await BroadcastGamesList();
        }

        return result;
    }

    public async Task<bool> TryPromote(Player player, Guid gameId, PieceType promotion)
    {
        Console.WriteLine($"Trying promote, by {promotion}");
        if (!_sessions.TryGetValue(gameId, out var session))
            return false;

        if (!session.ContainsPlayer(player))
            return false;

        bool result = await session.TryPromote(player, promotion);

        if (IsSessionOver(session))
        {
            _sessions.TryRemove(gameId, out _);
            await BroadcastGamesList();
        }

        return result;
    }

    public MatchSession? GetGame(Guid gameId)
        => _sessions.TryGetValue(gameId, out var session) ? session : null;

    public IEnumerable<GameInfo> GetGameInfos()
        => _sessions.Values.Select(s => s.ToGameInfo());

    public async Task BroadcastGamesList()
    {
        var update = new GamesListUpdate(GetGameInfos().ToList());

        var tasks = PlayerRegistry
            .GetAllPlayers()
            .Select(p => p.Send(update));

        await Task.WhenAll(tasks);
    }

    public bool IsSessionOver(MatchSession session)
        => session.GameManager.Game.Status == MatchStatus.Closed;

    public void HandleDisconnect(Player player)
    {
        var cts = new CancellationTokenSource();
        _pendingDisconnects[player.PlayerInfo.PlayerId] = cts;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(DisconnectTimeout, cts.Token);
                await FinalizeDisconnect(player);
            }
            catch (TaskCanceledException) { }
        });
    }

    public void CancelDisconnect(Player player)
    {
        if (_pendingDisconnects.TryRemove(player.PlayerInfo.PlayerId, out var cts))
            cts.Cancel();
    }

    private async Task FinalizeDisconnect(Player player)
    {
        foreach (var session in _sessions.Values)
        {
            if (!session.ContainsPlayer(player))
                continue;

            await QuitGame(player, session.GameManager.Game.GameId);
            break;
        }

        PlayerRegistry.Unregister(player.PlayerInfo.PlayerId);
    }
}

