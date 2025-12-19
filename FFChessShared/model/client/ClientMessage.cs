using MessagePack;

namespace FFChessShared;

[MessagePackObject]
[Union(0, typeof(ClientMove))]
[Union(1, typeof(ClientPromotion))]
[Union(2, typeof(ClientJoinGame))]
[Union(3, typeof(ClientCreateGame))]
[Union(4, typeof(ClientQuitGame))]
public abstract class ClientMessage
{
    [Key(0)] public Guid PlayerId { get; set; }
}