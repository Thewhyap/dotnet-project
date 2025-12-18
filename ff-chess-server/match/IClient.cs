using FFChessShared;

namespace Server.Match;

public interface IClient
{
    void Send(GameState state);
}