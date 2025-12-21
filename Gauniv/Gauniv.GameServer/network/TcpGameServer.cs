using System.Net;
using System.Net.Sockets;

namespace Server.Network;

public class TcpGameServer
{
    private readonly TcpListener _listener;

    public TcpGameServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public async Task StartAsync()
    {
        _listener.Start();

        while (true)
        {
            var tcpClient = await _listener.AcceptTcpClientAsync();
            _ = ClientConnectionHandler.HandleAsync(tcpClient);
        }
    }
}
