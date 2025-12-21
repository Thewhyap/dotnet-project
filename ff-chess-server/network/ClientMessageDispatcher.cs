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

            // Détection manuelle des types de messages si la désérialisation retourne le type de base
            if (message.GetType() == typeof(ClientMessage))
            {
                // ClientRequestGamesList: array de 1 élément (PlayerId uniquement), longueur ~39
                if (data.Length >= 38 && data.Length <= 42 && data[0] == 0x91) // 0x91 = array de 1
                {
                    try
                    {
                        var request = MessagePackSerializer.Deserialize<ClientRequestGamesList>(data, Options);
                        if (request.PlayerId != Guid.Empty)
                        {
                            Console.WriteLine($"[Dispatcher] Detected ClientRequestGamesList");
                            await MatchService.Instance.SendGamesList(sender);
                            return;
                        }
                    }
                    catch { }
                }
                
                // ClientCreateGame: array de 2 éléments (PlayerId + discriminator=0), longueur ~40
                if (data.Length == 40 && data[data.Length - 1] == 0)
                {
                    Console.WriteLine($"[Dispatcher] Detected ClientCreateGame based on message structure");
                    await MatchService.Instance.CreateAndJoinGame(sender);
                    return;
                }
                
                // ClientPromotion: array de 3 éléments (PlayerId + GameId + PieceType), longueur ~78
                if (data.Length >= 75 && data.Length <= 85 && data[0] == 0x93) // 0x93 = array de 3
                {
                    try
                    {
                        var promo = MessagePackSerializer.Deserialize<ClientPromotion>(data, Options);
                        if (promo.GameId != Guid.Empty)
                        {
                            Console.WriteLine($"[Dispatcher] Detected ClientPromotion for game {promo.GameId}, choice: {promo.PromotionChoice}");
                            await MatchService.Instance.TryPromote(sender, promo.GameId, promo.PromotionChoice);
                            return;
                        }
                    }
                    catch { }
                }
                
                // ClientMove: array de 3 éléments (PlayerId + GameId + Move), longueur variable
                if (data.Length >= 80 && data[0] == 0x93) // 0x93 = array de 3
                {
                    try
                    {
                        var move = MessagePackSerializer.Deserialize<ClientMove>(data, Options);
                        if (move.GameId != Guid.Empty && move.Move != null)
                        {
                            Console.WriteLine($"[Dispatcher] Detected ClientMove for game {move.GameId}");
                            await MatchService.Instance.TryMove(sender, move.GameId, move.Move);
                            return;
                        }
                    }
                    catch { }
                }
                
                // ClientQuitGame: array de 3 éléments (PlayerId + GameId + MessageType=1)
                // MUST be tested BEFORE ClientJoinGame to avoid confusion
                if (data.Length >= 75 && data.Length <= 85 && data[0] == 0x93) // 0x93 = array de 3
                {
                    try
                    {
                        var quit = MessagePackSerializer.Deserialize<ClientQuitGame>(data, Options);
                        if (quit.GameId != Guid.Empty && quit.MessageType == 1)
                        {
                            Console.WriteLine($"[Dispatcher] Detected ClientQuitGame for game {quit.GameId}");
                            await MatchService.Instance.QuitGame(sender, quit.GameId);
                            return;
                        }
                    }
                    catch { }
                }
                
                // ClientJoinGame: array de 2 éléments (PlayerId + GameId), pas de discriminator
                if (data.Length >= 70 && data.Length <= 80 && data[0] == 0x92) // 0x92 = array de 2
                {
                    try
                    {
                        var join = MessagePackSerializer.Deserialize<ClientJoinGame>(data, Options);
                        if (join.GameId != Guid.Empty)
                        {
                            Console.WriteLine($"[Dispatcher] Detected ClientJoinGame for game {join.GameId}");
                            await MatchService.Instance.JoinGame(sender, join.GameId);
                            return;
                        }
                    }
                    catch { }
                }
                
                Console.WriteLine($"[Dispatcher] ERROR: Unknown message type: ClientMessage (length={data.Length})");
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

                case ClientRequestGamesList:
                    Console.WriteLine($"[Dispatcher] Dispatching ClientRequestGamesList");
                    await MatchService.Instance.SendGamesList(sender);
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
