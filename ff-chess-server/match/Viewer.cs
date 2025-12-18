using FFChessShared;

namespace Server.Match;

public class Viewer : IClient
{
    public void Send(GameState state)
    {
        // receive updates only
    }
}