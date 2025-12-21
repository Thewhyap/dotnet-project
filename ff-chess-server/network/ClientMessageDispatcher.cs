using FFChessShared;
using MessagePack;
using MessagePack.Resolvers;
using Server.Match;

namespace Server.Network;

public static class ClientMessageDispatcher
{
    private static readonly MessagePackSerializerOptions Options = 
        MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

    public static async Task DispatchAsync(Player sender, byte[] data)
    {
        Console.WriteLine($"[Dispatcher] Received {data.Length} bytes from {sender.PlayerInfo.PlayerName} (PlayerId: {sender.PlayerInfo.PlayerId})");
        Console.WriteLine($"[Dispatcher] Raw bytes: {BitConverter.ToString(data)}");
        
        try
        {
            // Tentative de désérialisation
            var message = MessagePackSerializer.Deserialize<ClientMessage>(data, Options);
            Console.WriteLine($"[Dispatcher] Deserialized message type: {message.GetType().Name}");
            Console.WriteLine($"[Dispatcher] Message PlayerId: {message.PlayerId}");
            Console.WriteLine($"[Dispatcher] Sender PlayerId: {sender.PlayerInfo.PlayerId}");

            if (message.PlayerId != sender.PlayerInfo.PlayerId)
            {
                Console.WriteLine($"[Dispatcher] ERROR: PlayerId mismatch! Ignoring message.");
                return;
            }

            // Si le type de base est retourné mais que c'est un array de 2 éléments avec le dernier = 0
            // alors c'est probablement un ClientCreateGame
            if (message.GetType() == typeof(ClientMessage) && data.Length == 40 && data[data.Length - 1] == 0)
            {
                Console.WriteLine($"[Dispatcher] Detected ClientCreateGame based on message structure");
                Console.WriteLine($"[Dispatcher] Dispatching ClientCreateGame");
                await MatchService.Instance.CreateAndJoinGame(sender);
                return;
            }

            switch (message)
            {
                case ClientCreateGame:
                    Console.WriteLine($"[Dispatcher] Dispatching ClientCreateGame");
                    await MatchService.Instance.CreateAndJoinGame(sender);
                    break;

                case ClientJoinGame join:
                    Console.WriteLine($"[Dispatcher] Dispatching ClientJoinGame for game {join.GameId}");
                    await MatchService.Instance.JoinGame(sender, join.GameId);
                    break;

                case ClientMove move:
                    Console.WriteLine($"[Dispatcher] Dispatching ClientMove for game {move.GameId}");
                    await MatchService.Instance.TryMove(sender, move.GameId, move.Move);
                    break;

                case ClientPromotion promo:
                    Console.WriteLine($"[Dispatcher] Dispatching ClientPromotion for game {promo.GameId}");
                    await MatchService.Instance.TryPromote(sender, promo.GameId, promo.PromotionChoice);
                    break;

                case ClientQuitGame quit:
                    Console.WriteLine($"[Dispatcher] Dispatching ClientQuitGame for game {quit.GameId}");
                    await MatchService.Instance.QuitGame(sender, quit.GameId);
                    break;

                default:
                    Console.WriteLine($"[Dispatcher] ERROR: Unknown message type: {message.GetType().Name}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Dispatcher] ERROR: Exception during dispatch: {ex.Message}");
            Console.WriteLine($"[Dispatcher] Stack trace: {ex.StackTrace}");
        }
    }
}
