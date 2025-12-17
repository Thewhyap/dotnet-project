using Godot;
using System;

public partial class MatchService : Node
{
	private int _nextMatchId = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void create_match(Lobby lobby)
	{
		var match = new MatchSession
		{
			Name = $"Match_{_nextMatchId++}"
		};

		GetNode("MatchSessions").AddChild(match);
		match.Initialize(lobby);
	}

	public void close_match(long matchId)
	{
		
	}

	public void route_action(long peerId,long action) // TODO define action type
	{
		
	}
}
