using System.Net.Sockets;

namespace Server.Network;

public static class MessageReader
{
    public static async Task<byte[]> ReceiveAsync(TcpClient client)
    {
        var stream = client.GetStream();

        byte[] lengthBytes = new byte[4];
        await ReadExactAsync(stream, lengthBytes, 4);
        int length = BitConverter.ToInt32(lengthBytes, 0);

        byte[] buffer = new byte[length];
        await ReadExactAsync(stream, buffer, length);

        return buffer;
    }

    private static async Task ReadExactAsync(NetworkStream stream, byte[] buffer, int size)
    {
        int read = 0;
        while (read < size)
        {
            int r = await stream.ReadAsync(buffer, read, size - read);
            if (r == 0)
                throw new Exception("Disconnected");
            read += r;
        }
    }
}
