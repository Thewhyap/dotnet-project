using Godot;
using System;
using FFChessShared;
using MessagePack;

public partial class GameUpdaterServer : Node
{
    private NetworkClient _networkClient;
    private Guid _playerId = Guid.Empty;

    public override async void _Ready()
    {
        _networkClient = GetParent().GetNode<NetworkClient>("NetworkClient");
        if (_networkClient == null)
        {
            GD.PrintErr("Network client not found");
        }
        _networkClient.OnMessageReceived += OnMessageReceived;

        await _networkClient.ConnectToServer("localhost", 8080);
    }

    private void OnMessageReceived(byte[] data)
    {
        try
        {
            // PlayerInfo message (sent first by server)
            try
            {
                var playerInfo = MessagePackSerializer.Deserialize<PlayerInfo>(data);
                HandlePlayerInfo(playerInfo);
                return;
            }
            catch { }

            // GameUpdate
            try
            {
                var gameUpdate = MessagePackSerializer.Deserialize<GameUpdate>(data);
                HandleGameStateUpdate(gameUpdate);
                return;
            }
            catch { }

            try
            {
                var gameJoined = MessagePackSerializer.Deserialize<GameJoined>(data);
                HandleGameJoined(gameJoined);
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

    private void HandleGameStateUpdate(GameUpdate gameUpdate)
    {
        GD.Print($"Game state updated: {gameUpdate.State}");
        GetSceneRouterNode().UpdateGame(gameUpdate);
    }

    private void HandlePlayerInfo(PlayerInfo playerInfo)
    {
        _playerId = playerInfo.PlayerId;
        GD.Print($"Received player info. PlayerId: {_playerId}, Name: {playerInfo.PlayerName}");
    }

    private void HandleGameInfo(GameInfo info)
    {
        GD.Print($"Game info received: {info.GameName}");
    }

    private void HandleGameJoined(GameJoined gameJoined)
    {
        GetSceneRouterNode().InitGame(gameJoined);
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
        var payload = new ClientJoinGame() 
        { 
            PlayerId = _playerId,
            GameId = gameId 
        };
        _networkClient.SendMessage(payload);
    }
    
    public void SendCreateGameRequest()
    {
        GD.Print("Sending create game request...");
        var payload = new ClientCreateGame()
        {
            PlayerId = _playerId
        };
        _networkClient.SendMessage(payload);
        GD.Print("Sent.");
    }
    
    public void SendMovePieceRequest(ChessMove move)
    {
        ClientMove moveMessage = new ClientMove
        {
            PlayerId = _playerId,
            Move = move
        };
        _networkClient.SendMessage(moveMessage);
    }

    public void SendQuitGameRequest(Guid gameId)
    {
        var message = new ClientQuitGame
        {
            PlayerId = _playerId,
            GameId = gameId
        };
        _networkClient.SendMessage(message);
    }
    
    public void SendPromoteRequest(Guid gameId, PieceType promotionChoice)
    {
        ClientPromotion message = new ClientPromotion
        {
            PlayerId = _playerId,
            GameId =  gameId,
            PromotionChoice = promotionChoice
        };
        _networkClient.SendMessage(message);
    }

    private SceneRouter GetSceneRouterNode()
    {
        return GetNode<SceneRouter>("/root/ClientRoot/GameUpdaterServer/SceneRouter");
    }
}