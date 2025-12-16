using Godot;
using System;

public partial class NetworkServer : Node
{
	private ENetMultiplayerPeer _peer;

	public override void _Ready()
	{
		_peer = new ENetMultiplayerPeer();
		_peer.CreateServer(7777, 100);

		Multiplayer.MultiplayerPeer = _peer;

		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;

		GD.Print("Server started on port 7777");
	}

	private void OnPeerConnected(long id)
	{
		GD.Print($"Client connected: {id}");
	}

	private void OnPeerDisconnected(long id)
	{
		GD.Print($"Client disconnected: {id}");
	}
	
}
