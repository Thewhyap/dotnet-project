using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ClientMove : ClientMessage
{
    [Key(1)] public Guid GameId { get; set; }
    [Key(2)] public required ChessMove Move { get; set; }
}
