using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ClientJoinGame : ClientMessage
{
    [Key(1)] public Guid GameId { get; set; }
}