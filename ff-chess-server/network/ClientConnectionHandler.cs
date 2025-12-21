using System.Net.Sockets;
using FFChessShared;
using Server.Match;
using MessagePack;
using Server.Chess;

namespace Server.Network;

public static class ClientConnectionHandler
    {
        public static async Task HandleAsync(TcpClient tcpClient)
        {
            Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} connected");
            tcpClient.NoDelay = true; // reduce buffering delays
            var stream = tcpClient.GetStream();

            Player? player = null;

            try
            {
                // Créer immédiatement le joueur sans attendre de message d'auth
                player = new Player(NicknameGenerator.GenerateNickname(), tcpClient);
                PlayerRegistry.Register(player);
                
                player.SetTcpClient(tcpClient);
                await player.SendPlayerInfo();
                Console.WriteLine($"[ClientHandler] Sent PlayerInfo to {player.PlayerInfo.PlayerName}");

                while (true)
                {
                    var bytes = await MessageReader.ReceiveAsync(tcpClient);
                    if (bytes.Length == 0)
                    {
                        Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnected");
                        break;
                    }

                    Console.WriteLine($"Received {bytes.Length} bytes from {tcpClient.Client.RemoteEndPoint}");
                    await ClientMessageDispatcher.DispatchAsync(player, bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Handler error for {tcpClient.Client.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnected");
                if (player != null)
                {
                    MatchService.Instance.HandleDisconnect(player);
                }
                tcpClient.Close();
            }
        }
    }
