using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ClientQuitGame : ClientMessage
{
    [Key(1)] public Guid GameId { get; set; }
    [Key(2)] public int MessageType { get; set; } = 1; // Discriminator: 1 = Quit (vs 0 = Join)
}