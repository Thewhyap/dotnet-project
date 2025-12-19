using FFChessShared;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;

namespace Server.Match;

public class Player(string name, TcpClient tcpClient)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public PieceColor? AssignedColor { get; set; }

    private readonly TcpClient _tcpClient = tcpClient;

    public async Task SendPlayerInfo()
    {
        var info = new PlayerInfo
        {
            PlayerId = Id,
            PlayerName = Name,
            AssignedColor = AssignedColor
        };

        var stream = _tcpClient.GetStream();
        byte[] data = MessagePackSerializer.Serialize(info);
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }

    public async Task SendGameState(GameState gameState, TurnStatus turnStatus, string? message = null)
    {
        var update = new GameStateUpdate
        {
            State = gameState,
            CurrentTurnStatus = turnStatus,
            InfoMessage = message
        };

        var stream = _tcpClient.GetStream();
        byte[] data = MessagePackSerializer.Serialize(update);
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }
}
