using FFChessShared;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MessagePack;
using Server.Chess;

namespace Server.Match;

public class Player(string name, TcpClient tcpClient)
{
    public PlayerInfo PlayerInfo { get; set; } = new PlayerInfo(name);

    private readonly TcpClient _tcpClient = tcpClient;

    public async Task SendPlayerInfo()
    {
        var stream = _tcpClient.GetStream();
        byte[] data = MessagePackSerializer.Serialize(PlayerInfo);
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }

    public async Task SendGameJoined(Game game, PieceColor? color)
    {
        var update = new GameUpdate
        {
            State = game.GameState,
            TurnStatus = game.TurnStatus
        };

        var join = new GameJoined
        {
            GameId = game.GameId,
            AssignedColor = color,
            InitialGameState = update
        };

        var stream = _tcpClient.GetStream();
        byte[] data = MessagePackSerializer.Serialize(join);
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }

    public async Task SendGameUpdate(GameState gameState, TurnStatus turnStatus)
    {
        var update = new GameUpdate
        {
            State = gameState,
            CurrentTurnStatus = turnStatus
        };

        var stream = _tcpClient.GetStream();
        byte[] data = MessagePackSerializer.Serialize(update);
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }

    public async Task SendGameInfo(GameInfo gameInfo)
    {
        var stream = _tcpClient.GetStream();
        byte[] data = MessagePackSerializer.Serialize(gameInfo);
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }
}
