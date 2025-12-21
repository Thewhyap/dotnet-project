using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class ClientRequestGamesList : ClientMessage
{
    // Empty - just a request with PlayerId from base class
}

