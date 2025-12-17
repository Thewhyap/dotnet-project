using Godot;
using Server.Lobbies;
using Server.Match;

namespace Server.Network;

public partial class NetworkServer : Node
{
	public static NetworkServer Instance { get; private set; }

	public LobbyService LobbyService { get; private set; }
	public MatchService MatchService { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		LobbyService = new LobbyService();
		MatchService = new MatchService();

		AddChild(LobbyService);
		AddChild(MatchService);
	}
}
