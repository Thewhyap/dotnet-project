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
        var stream = tcpClient.GetStream();

        // Initial client auth message (containing PlayerId or empty)
        var initialBytes = await MessageReader.ReceiveAsync(tcpClient);

        var auth = MessagePackSerializer.Deserialize<ClientMessage>(initialBytes);

        Player? player;

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

        try
        {
            while (true)
            {
                var bytes = await MessageReader.ReceiveAsync(tcpClient);
                Console.WriteLine($"Received {bytes.Length} bytes from {tcpClient.Client.RemoteEndPoint}");
                await ClientMessageDispatcher.DispatchAsync(player, bytes);
            }
        }
        catch
        {
            Console.WriteLine($"Client {tcpClient.Client.RemoteEndPoint} disconnected");
            MatchService.Instance.HandleDisconnect(player);
            tcpClient.Close();
        }
    }
}
