using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class PlayerInfo
{
    [Key(0)]
    public required Guid PlayerId { get; set; }

    [Key(1)]
    public required string PlayerName { get; set; }

    public PlayerInfo(string playerName)
    {
        PlayerId = Guid.NewGuid();
        PlayerName = playerName;
    }

    public PlayerInfo() { }
}
