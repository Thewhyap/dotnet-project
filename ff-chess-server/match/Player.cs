using FFChessShared;

namespace Server.Match;

public class Player : IClient
{
    public Guid Id { get; } = Guid.NewGuid();
    public PieceColor Color { get; }

    public Player(PieceColor color)
    {
        Color = color;
    }

    public void Send(GameState state)
    {
        // send over WebSocket / TCP / SignalR
    }
}
