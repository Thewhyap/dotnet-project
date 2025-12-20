using FFChessShared;
using Server.Match;

namespace Server.Messaging;

public static class MessageSender
{
    public static void Send(Player player, object message)
        => player.SendAsync(message);

    public static void SendGameState(MatchSession match)
    {
        var msg = new GameStateUpdate
        {
            State = match.GameManager.Game.GameState,
            CurrentTurnStatus = match.GameManager.Game.TurnStatus
        };

        match.WhitePlayer?.SendAsync(msg);
        match.BlackPlayer?.SendAsync(msg);

        foreach (var v in match.Viewers)
            v.SendAsync(msg);
    }

    public static void BroadcastGamesList(IEnumerable<GameInfo> games)
    {
        var msg = new GamesListUpdate
        {
            Games = games.ToList()
        };

        foreach (var p in PlayerRegistry.GetAllPlayers())
            p.SendAsync(msg);
    }
}
