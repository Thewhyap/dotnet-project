using System.Net.Sockets;
using FFChessShared;
using Server.Match;
using MessagePack;

namespace Server.Network;

public static class ClientConnectionHandler
{
    public static async Task HandleAsync(TcpClient tcpClient)
    {
        var player = new Player(tcpClient);

        PlayerRegistry.Register(player);
        await player.SendPlayerInfo();

        try
        {
            while (true)
            {
                var bytes = await MessageReader.ReceiveAsync(tcpClient);
                await ClientMessageDispatcher.DispatchAsync(player, bytes);
            }
        }
        catch
        {
            PlayerRegistry.Unregister(player);
            tcpClient.Close();
        }
    }
}
