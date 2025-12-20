using Server.Network;

namespace FFChessServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var server = new TcpGameServer(port: 8080);
        await server.StartAsync();
    }
}
