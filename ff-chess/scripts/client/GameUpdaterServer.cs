using Godot;
using System;
using FFChessShared;
using MessagePack;

public partial class GameUpdaterServer : Node
{
    private NetworkClient _networkClient;

    public override async void _Ready()
    {
        _networkClient = GetNode<NetworkClient>("NetworkClient");
        _networkClient.OnMessageReceived += OnMessageReceived;

        await _networkClient.ConnectToServer("localhost", 5000);
    }

    private void OnMessageReceived(byte[] data)
    {
        try
        {
            // GameState update
            try
            {
                var gameState = MessagePackSerializer.Deserialize<GameState>(data);
                HandleGameStateUpdate(gameState);
                return;
            }
            catch { }
            
            // GameInfo message
            try
            {
                var gameInfo = MessagePackSerializer.Deserialize<GameInfo>(data);
                HandleGameInfo(gameInfo);
                return;
            }
            catch { }
            
            // GameInfo[] message (list of games)
            try
            {
                var gameList = MessagePackSerializer.Deserialize<GameInfo[]>(data);
                HandleGameList(gameList);
                return;
            }
            catch { }
            
            

            GD.PrintErr("Unknown message type received");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error deserializing message: {e.Message}");
        }
    }

    private void HandleGameStateUpdate(GameState state)
    {
        GD.Print($"Game state updated: {state}");
        GetNode<SceneRouter>("/root/ClientRoot/GameUpdaterServer/SceneRouter").UpdateGame(state);
    }

    private void HandleGameInfo(GameInfo info)
    {
        GD.Print($"Game info received: {info.GameName}");
    }
    
    private void HandleGameList(GameInfo[] games)
    {
        GD.Print($"Received list of {games.Length} games from server.");
        // Here you would typically update the lobby UI with the received game list
        
        var lobbyScreen = GetNode<LobbyScreen>("/root/ClientRoot/GameUpdaterServer/LobbyScreen");
        lobbyScreen.DisplayGamesLobbies(games);
    }

    public void SendJoinGameRequest(Guid gameId)
    {
        var payload = new ClientJoinGame() { GameId = gameId };
        _networkClient.SendMessage(payload);
    }
    
    public void SendGetGamesRequest()
    {
        GD.Print("Requesting lobbies from server...");
        var request = new ClientGetGames(); // TODO EG define a specific request message
        _networkClient.SendMessage(request);
        GD.Print("Lobbies loaded.");
    }
    
    public void SendCreateGameRequest()
    {
        var payload = new ClientCreateGame();
        _networkClient.SendMessage(payload);
    }
    
    public void SendMovePieceRequest(ChessMove move)
    {
        _networkClient.SendMessage(move);
    }
}