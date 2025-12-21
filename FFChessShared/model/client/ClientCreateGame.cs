using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ClientCreateGame : ClientMessage
{
    // Dummy property to help MessagePack distinguish this type from base ClientMessage
    // Without this, MessagePack can't deserialize to the correct derived type
    [Key(1)] public byte MessageTypeDiscriminator { get; set; } = 0;
}

