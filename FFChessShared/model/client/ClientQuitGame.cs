using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ClientQuitGame : ClientMessage
{
    [Key(1)] public Guid GameId { get; set; }
}