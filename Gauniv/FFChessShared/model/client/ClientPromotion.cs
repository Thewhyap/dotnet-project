using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ClientPromotion : ClientMessage
{
    [Key(1)] public Guid GameId { get; set; }
    [Key(2)] public required PieceType PromotionChoice { get; set; }
}
