namespace Shared;

public readonly struct Games
{
	public Game[] CurrentGames { get; }

	public Games(Game[] currentGames)
	{
		CurrentGames = currentGames;
	}
}
