using Godot;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;

public partial class NetworkClient : Node
{
	private TcpClient _tcpClient;
	private NetworkStream _networkStream;
	private bool _isConnected = false;

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
		byte[] buffer = new byte[4096];
		try
		{
			while (_isConnected)
			{
				int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
				if (bytesRead == 0)
				{
					_isConnected = false;
					break;
				}

				byte[] data = new byte[bytesRead];
				Array.Copy(buffer, data, bytesRead);
				EmitSignal(SignalName.OnMessageReceived, data);
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error listening for messages: {e.Message}");
			_isConnected = false;
		}
	}

	public async void SendMessage<T>(T payload) where T : class
	{
		if (!_isConnected) return;

		try
		{
			byte[] data = MessagePackSerializer.Serialize(payload);
			await _networkStream.WriteAsync(data, 0, data.Length);
			await _networkStream.FlushAsync();
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error sending message: {e.Message}");
		}
	}

	public override void _ExitTree()
	{
		_isConnected = false;
		_networkStream?.Dispose();
		_tcpClient?.Dispose();
	}
}
