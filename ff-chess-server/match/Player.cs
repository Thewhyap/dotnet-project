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

    private TcpClient _tcpClient = tcpClient;

    public async Task Send<T>(T message)
    {
        var stream = _tcpClient.GetStream();

        byte[] data = MessagePackSerializer.Serialize(message);
        byte[] lengthPrefix = BitConverter.GetBytes(data.Length);

        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await stream.WriteAsync(data, 0, data.Length);
    }

    public Task SendPlayerInfo()
    => Send(PlayerInfo);

    public Task SendGameInfo(GameInfo gameInfo)
        => Send(gameInfo);

    public Task SendGameUpdate(GameState gameState, TurnStatus turnStatus)
        => Send(new GameUpdate(gameState, turnStatus));

    public Task SendGameJoined(Game game, PieceColor? color)
        => Send(new GameJoined
        {
            GameId = game.GameId,
            AssignedColor = color,
            InitialGameState = new GameUpdate(game.GameState, game.TurnStatus)
        });

    public void SetTcpClient(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
    }
}
