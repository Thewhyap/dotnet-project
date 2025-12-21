using Server.Network;

namespace Gauniv.GameServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        int port = 8080;
        var server = new TcpGameServer(port: port);
        Console.WriteLine($"FFChess server running on port {port}...");
        await server.StartAsync();
    }
}
