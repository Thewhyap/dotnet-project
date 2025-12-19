using System.Collections.Concurrent;

namespace Server.Match;

public class PlayerRegistry
{
    private static ConcurrentDictionary<Guid, Player> _players = new();

    public static void Register(Player player)
    {
        _players[player.Id] = player;
    }

    public static bool TryGetPlayer(Guid playerId, out Player player)
    {
        return _players.TryGetValue(playerId, out player);
    }

    public static void Unregister(Guid playerId)
    {
        _players.TryRemove(playerId, out _);
    }
}
