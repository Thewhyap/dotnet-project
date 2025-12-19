using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;
using System.Collections.Concurrent;
using FFChessShared;
using Server.Match;

namespace Server.Network;

public class TcpGameServer
{
    private TcpListener _listener;

    public async Task StartAsync()
    {
        _listener.Start();
        while (true)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(tcpClient);
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        try
        {
            while (true)
            {
                byte[] lengthBytes = new byte[4];
                await ReadExactAsync(stream, lengthBytes, 0, 4);
                int length = BitConverter.ToInt32(lengthBytes, 0);

                byte[] messageBytes = new byte[length];
                await ReadExactAsync(stream, messageBytes, 0, length);

                await HandleClientMessageAsync(messageBytes);
            }
        }
        catch (Exception ex)
        {
            // TODO Gestion des erreurs, logs, fermeture client...
        }
    }

    public async Task HandleClientMessageAsync(byte[] messageBytes)
    {
        var command = MessagePackSerializer.Deserialize<ClientCommand>(messageBytes);

        if (!PlayerRegistry.TryGetPlayer(command.PlayerId, out var player))
        {
            await SendErrorAsync(player, "Token invalide");
            return;
        }

        switch (command.Command)
        {
            case "MakeMove":
                var move = MessagePackSerializer.Deserialize<ChessMove>(command.Payload);
                var match = GetMatchForPlayer(player);
                if (match == null)
                {
                    await SendErrorAsync(player, "Partie introuvable");
                    return;
                }

                bool ok = match.TryMakeMove(player, move);
                if (!ok)
                {
                    await SendErrorAsync(player, "Coup invalide ou pas ton tour");
                    return;
                }
                break;

                // TODO Autres commandes...
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        var player = new Player(tcpClient);

        PlayerRegistry.Register(player);

        while (true)
        {
            var messageBytes = await ReceiveMessageAsync(tcpClient.GetStream());
            await HandleClientMessageAsync(messageBytes);
        }
    }

}
