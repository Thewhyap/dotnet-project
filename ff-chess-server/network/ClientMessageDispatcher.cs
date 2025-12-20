using FFChessShared;
using MessagePack;
using Server.Match;

namespace Server.Network;

public static class ClientMessageDispatcher
{
    private static readonly MatchService _matchService = new();

    public static async Task DispatchAsync(Player sender, byte[] data)
    {
        var message = MessagePackSerializer.Deserialize<ClientMessage>(data);

        if (message.PlayerId != sender.Id)
        {
            await sender.SendError("Invalid player id");
            return;
        }

        switch (message)
        {
            case ClientCreateGame:
                _matchService.CreateAndJoinGame(sender);
                break;

            case ClientJoinGame join:
                _matchService.JoinGame(sender, join.GameId);
                break;

            case ClientMove move:
                _matchService.TryMakeMove(sender, move.GameId, move.Move);
                break;

            case ClientPromotion promo:
                _matchService.TryPromote(sender, promo.GameId, promo.PromotionChoice);
                break;

            case ClientQuitGame quit:
                _matchService.QuitGame(sender, quit.GameId);
                break;
        }
    }
}
