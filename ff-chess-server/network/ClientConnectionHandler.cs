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
                var initialBytes = await MessageReader.ReceiveAsync(tcpClient);
                if (initialBytes.Length == 0)
                {
                    Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnected before auth");
                    tcpClient.Close();
                    return;
                }

                var auth = MessagePackSerializer.Deserialize<ClientMessage>(initialBytes);
                if (auth.PlayerId != Guid.Empty && PlayerRegistry.TryGetPlayer(auth.PlayerId, out var existingPlayer))
                {
                    player = existingPlayer;
                    MatchService.Instance.CancelDisconnect(player);
                }
                else
                {
                    player = new Player(NicknameGenerator.GenerateNickname(), tcpClient);
                    PlayerRegistry.Register(player);
                }

                player.SetTcpClient(tcpClient);
                await player.SendPlayerInfo();

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
