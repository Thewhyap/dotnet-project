using Godot;
using System;

/*
Rôle
- Connexion au serveur
- Envoi des intentions joueur
- Réception des états (lobby / partie)
- Le client ne décide jamais si un coup est valide.
*/
public partial class NetworkClient : Node
{
	private ENetMultiplayerPeer _peer;

	public void Connect(string ip = "127.0.0.1", int port = 7777)
	{
		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(ip, port);

		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Connecting to server...");
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
