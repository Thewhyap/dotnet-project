using System.Net.Sockets;

namespace Server.Network;

public static class MessageReader
{
    public static async Task<byte[]> ReceiveAsync(TcpClient client, CancellationToken ct = default)
    {
        var stream = client.GetStream();
        stream.ReadTimeout = 5000; // 5s to surface stalled reads

        // Example: first read a 4-byte length prefix (MessagePack often uses framing externally).
        var lenBuf = new byte[4];
        var read = await ReadExactAsync(stream, lenBuf, 0, 4, ct);
        if (read == 0)
        {
            Console.WriteLine($"[MessageReader] Remote {client.Client.RemoteEndPoint} closed before length prefix");
            return Array.Empty<byte>();
        }

        // Convertir en little-endian si nécessaire
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(lenBuf);
            
        var length = BitConverter.ToInt32(lenBuf, 0);
        if (length <= 0 || length > 10_000_000)
        {
            Console.WriteLine($"[MessageReader] Invalid length {length} from {client.Client.RemoteEndPoint}");
            return Array.Empty<byte>();
        }

        var payload = new byte[length];
         await ReadExactAsync(stream, payload,0, length,ct);
        

        Console.WriteLine($"[MessageReader] Read {length} bytes from {client.Client.RemoteEndPoint}");
        return payload;
    }

    private static async Task<int> ReadExactAsync(NetworkStream stream, byte[] buffer, int offset, int count, CancellationToken ct)
    {
        var total = 0;
        while (total < count)
        {
            int n;
            try
            {
                n = await stream.ReadAsync(buffer.AsMemory(offset + total, count - total), ct);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[MessageReader] IO exception during read: {ex.Message}");
                return 0;
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("[MessageReader] Stream disposed during read");
                return 0;
            }

            if (n == 0)
            {
                // Remote closed.
                return 0;
            }

            total += n;
            Console.WriteLine($"[MessageReader] Progress {total}/{count}");
        }

        return total;
    }
}
