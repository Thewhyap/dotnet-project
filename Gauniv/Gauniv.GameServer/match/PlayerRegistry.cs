using System.Collections.Concurrent;

namespace Server.Match;

public class PlayerRegistry
{
    private static ConcurrentDictionary<Guid, Player> _players = new();

    public static void Register(Player player)
    {
        _players[player.PlayerInfo.PlayerId] = player;
    }

    public static bool TryGetPlayer(Guid playerId, out Player player)
    {
        return _players.TryGetValue(playerId, out player!);
    }

    public static void Unregister(Guid playerId)
    {
        _players.TryRemove(playerId, out _);
    }

    public static IEnumerable<Player> GetAllPlayers()
    {
        return _players.Values;
    }
}
