using FFChessShared;
using MessagePack;
using Server.Match;

namespace Server.Network;

public static class ClientMessageDispatcher
{
    public static async Task DispatchAsync(Player sender, byte[] data)
    {
        var message = MessagePackSerializer.Deserialize<ClientMessage>(data);

        if (message.PlayerId != sender.PlayerInfo.PlayerId)
        {
            return;
        }

        switch (message)
        {
            case ClientCreateGame:
                await MatchService.Instance.CreateAndJoinGame(sender);
                break;

            case ClientJoinGame join:
                await MatchService.Instance.JoinGame(sender, join.GameId);
                break;

            case ClientMove move:
                await MatchService.Instance.TryMove(sender, move.GameId, move.Move);
                break;

            case ClientPromotion promo:
                await MatchService.Instance.TryPromote(sender, promo.GameId, promo.PromotionChoice);
                break;

            case ClientQuitGame quit:
                await MatchService.Instance.QuitGame(sender, quit.GameId);
                break;
        }
    }
}
