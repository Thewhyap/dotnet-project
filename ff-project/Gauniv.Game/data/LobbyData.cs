namespace Gauniv.Game.data;

public class LobbyData
{
    public string Name { get; set; }
    public int Players { get; set; }
    public int MaxPlayers { get; set; }

    public LobbyData(string name, int players, int maxPlayers)
    {
        Name = name;
        Players = players;
        MaxPlayers = maxPlayers;
    }
}
