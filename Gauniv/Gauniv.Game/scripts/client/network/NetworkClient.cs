using Godot;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;

public partial class NetworkClient : Node
{
	private TcpClient _tcpClient;
	private NetworkStream _networkStream;
	private bool _isConnected = false;
	
	private static readonly MessagePackSerializerOptions Options = 
		MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

	[Signal]
	public delegate void OnMessageReceivedEventHandler(byte[] data);

	public async Task ConnectToServer(string host, int port)
	{
		try
		{
			_tcpClient = new TcpClient();
			await _tcpClient.ConnectAsync(host, port);
			_networkStream = _tcpClient.GetStream();
			_isConnected = true;
			GD.Print($"Connected to server at {host}:{port}");

			_ = ListenForMessages();
		}
		catch (Exception e)
		{
			GD.PrintErr($"Connection failed: {e.Message}");
		}
	}

	private async Task ListenForMessages()
	{
		try
		{
			while (_isConnected)
			{
				var lengthBuffer = new byte[4];
				var bytesRead = await ReadExactAsync(_networkStream, lengthBuffer, 0, 4);
				
				if (bytesRead != 4)
				{
					_isConnected = false;
					GD.Print("Connection to server closed");
					break;
				}

				if (!BitConverter.IsLittleEndian)
					Array.Reverse(lengthBuffer);

				var length = BitConverter.ToInt32(lengthBuffer, 0);
				
				if (length <= 0 || length > 10_000_000)
				{
					GD.PrintErr($"Invalid message length: {length}");
					_isConnected = false;
					break;
				}

				// Read the actual message
				var messageBuffer = new byte[length];
				bytesRead = await ReadExactAsync(_networkStream, messageBuffer, 0, length);
				
				if (bytesRead != length)
				{
					GD.PrintErr($"Failed to read complete message. Expected {length}, got {bytesRead}");
					_isConnected = false;
					break;
				}

				GD.Print($"[NETWORK] Received message: length={length}");
				EmitSignal(SignalName.OnMessageReceived, messageBuffer);
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error listening for messages: {e.Message}");
			_isConnected = false;
		}
	}

	private async Task<int> ReadExactAsync(NetworkStream stream, byte[] buffer, int offset, int count)
	{
		var total = 0;
		while (total < count)
		{
			var n = await stream.ReadAsync(buffer, offset + total, count - total);
			if (n == 0)
				return total; // Connection closed
			total += n;
		}
		return total;
	}

	public async Task SendMessageAsync<T>(T message)
	{
		if (!_isConnected || _networkStream == null)
		{
			GD.PrintErr("[NETWORK] Cannot send message: not connected");
			return;
		}

		try
		{
			var messageBytes = MessagePackSerializer.Serialize(message, Options);
			var length = messageBytes.Length;
		
			// Envoyer d'abord la longueur (4 bytes)
			var lengthBytes = BitConverter.GetBytes(length);
			if (!BitConverter.IsLittleEndian)
				Array.Reverse(lengthBytes);
		
			await _networkStream.WriteAsync(lengthBytes, 0, 4);
			await _networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
			await _networkStream.FlushAsync();
		
			GD.Print($"[NETWORK] Sent message: length={length}");
		}
		catch (Exception e)
		{
			GD.PrintErr($"[NETWORK] Error sending message: {e.Message}");
			_isConnected = false;
		}
	}

	public void SendMessage<T>(T message)
	{
		// Wrapper synchrone pour la compatibilité
		_ = SendMessageAsync(message);
	}


	public override void _ExitTree()
	{
		_isConnected = false;
		_networkStream?.Dispose();
		_tcpClient?.Dispose();
	}
}
