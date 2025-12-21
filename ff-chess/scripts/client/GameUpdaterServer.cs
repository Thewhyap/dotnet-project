using Godot;
using System;
using FFChessShared;
using MessagePack;
using MessagePack.Resolvers;

public partial class GameUpdaterServer : Node
{
    private NetworkClient _networkClient;
    private Guid _playerId = Guid.Empty;
    private GameJoined _pendingGameJoined; // For deferred initialization
    private bool _isQuitting = false; // Flag to prevent rejoining after quit
    
    private static readonly MessagePackSerializerOptions Options = 
        MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

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
            // If not authenticated yet, the first message MUST be PlayerInfo
            if (_playerId == Guid.Empty)
            {
                try
                {
                    var playerInfo = MessagePackSerializer.Deserialize<PlayerInfo>(data, Options);
                    if (playerInfo.PlayerId != Guid.Empty)
                    {
                        HandlePlayerInfo(playerInfo);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Failed to deserialize initial PlayerInfo: {ex.Message}");
                }
            }

            // Try GameJoined first (most specific)
            try
            {
                var gameJoined = MessagePackSerializer.Deserialize<GameJoined>(data, Options);
                if (gameJoined.GameId != Guid.Empty) // Validation
                {
                    HandleGameJoined(gameJoined);
                    return;
                }
            }
            catch { }

            // GameUpdate
            try
            {
                var gameUpdate = MessagePackSerializer.Deserialize<GameUpdate>(data, Options);
                if (gameUpdate.State != null) // Validation
                {
                    HandleGameStateUpdate(gameUpdate);
                    return;
                }
            }
            catch { }
            
            // ServerGameQuit - Player has quit the game
            // IMPORTANT: Test BEFORE GameInfo because they both have GameId
            try
            {
                var gameQuit = MessagePackSerializer.Deserialize<ServerGameQuit>(data, Options);
                if (gameQuit.GameId != Guid.Empty && !string.IsNullOrEmpty(gameQuit.Reason))
                {
                    HandleGameQuit(gameQuit);
                    return;
                }
            }
            catch { }
            
            // GamesListUpdate message
            try
            {
                var gamesListUpdate = MessagePackSerializer.Deserialize<GamesListUpdate>(data, Options);
                if (gamesListUpdate.Games != null) // Validation
                {
                    HandleGameList(gamesListUpdate.Games.ToArray());
                    return;
                }
            }
            catch { }
            
            // GameInfo[] message (list of games)
            try
            {
                var gameList = MessagePackSerializer.Deserialize<GameInfo[]>(data, Options);
                if (gameList != null && gameList.Length > 0) // Validation
                {
                    HandleGameList(gameList);
                    return;
                }
            }
            catch { }
            
            // GameInfo message - check it's not PlayerInfo by verifying it has more than 2 fields
            try
            {
                var gameInfo = MessagePackSerializer.Deserialize<GameInfo>(data, Options);
                // GameInfo has GameName which PlayerInfo doesn't have
                if (gameInfo.GameId != Guid.Empty && !string.IsNullOrEmpty(gameInfo.GameName))
                {
                    HandleGameInfo(gameInfo);
                    return;
                }
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
        GD.Print($"[GameUpdater] GameInfo received: {info.GameName}, GameId: {info.GameId}, Status: {info.Status}");
        GD.Print($"[GameUpdater] White: {info.WhitePlayerName}, Black: {info.BlackPlayerName}");
        
        // Check if we should auto-join this game
        // NEVER auto-join - only manual join from lobby!
        GD.Print("[GameUpdater] GameInfo processed - NOT auto-joining");
    }

    private void HandleGameJoined(GameJoined gameJoined)
    {
        // Ignore GameJoined if we're quitting - prevents rejoining after quit
        if (_isQuitting)
        {
            GD.Print($"[GameUpdater] Ignoring GameJoined (quitting in progress): {gameJoined.GameId}");
            return;
        }
        
        GD.Print($"[GameUpdater] ========== GAME JOINED ==========");
        GD.Print($"[GameUpdater] GameId: {gameJoined.GameId}, AssignedColor: {gameJoined.AssignedColor}");
        GD.Print($"[GameUpdater] CurrentScene: {GetTree().CurrentScene?.Name}");
        
        // Load the game screen first
        var gameInfo = new GameInfo 
        { 
            GameId = gameJoined.GameId,
            GameName = "Chess Game",
            WhitePlayerName = gameJoined.AssignedColor == PieceColor.White ? "You" : "Opponent",
            BlackPlayerName = gameJoined.AssignedColor == PieceColor.Black ? "You" : "Opponent",
            Status = MatchStatus.InGame
        };
        
        GetSceneRouterNode().LoadGame(gameInfo);
        
        // Store for deferred initialization
        _pendingGameJoined = gameJoined;
        CallDeferred(nameof(InitGameDeferred));
    }
    
    private void InitGameDeferred()
    {
        if (_pendingGameJoined != null)
        {
            GetSceneRouterNode().InitGame(_pendingGameJoined);
            _pendingGameJoined = null;
        }
    }
    
    private void HandleGameList(GameInfo[] games)
    {
        GD.Print($"Received list of {games.Length} games from server.");
        
        // Only update lobby screen if it exists (we're on the lobby screen)
        var lobbyScreen = GetNodeOrNull<LobbyScreen>("/root/ClientRoot/UI/LobbyScreen");
        if (lobbyScreen != null)
        {
            lobbyScreen.DisplayGamesLobbies(games);
        }
        else
        {
            GD.Print("Not on lobby screen, skipping lobby update");
        }
    }
    
    private void HandleGameQuit(ServerGameQuit gameQuit)
    {
        GD.Print($"[GameUpdater] Game quit confirmed. GameId: {gameQuit.GameId}, Reason: {gameQuit.Reason}");
        
        // Clear any pending game joined data to prevent rejoining
        _pendingGameJoined = null;
        
        // Clear game data from GameScreen if it exists
        var gameScreen = GetNodeOrNull<GameScreen>("/root/ClientRoot/UI/GameScreen");
        if (gameScreen != null)
        {
            GD.Print("[GameUpdater] Clearing game data from GameScreen");
            gameScreen.clearGameData();
        }
        
        // Return to main menu - SceneRouter is a child of this node
        var sceneRouter = GetNode<SceneRouter>("SceneRouter");
        if (sceneRouter != null)
        {
            GD.Print("[GameUpdater] Returning to main menu...");
            sceneRouter.LoadMainMenu();
        }
        else
        {
            GD.PrintErr("[GameUpdater] SceneRouter not found, cannot return to main menu");
        }
        
        // Reset quitting flag - we're now at main menu and can join games again
        _isQuitting = false;
        GD.Print("[GameUpdater] Quit complete - ready for new games");
    }

    public void SendJoinGameRequest(Guid gameId)
    {
        if (_playerId == Guid.Empty)
        {
            GD.PrintErr("[GameUpdater] Cannot send JoinGame: not authenticated yet");
            return;
        }
        
        GD.Print($"[GameUpdater] Sending JOIN request for game {gameId}");  // ← Ajouté
        var payload = new ClientJoinGame() 
        { 
            PlayerId = _playerId,
            GameId = gameId 
        };
        _networkClient.SendMessage(payload);
        GD.Print($"[GameUpdater] JOIN request sent for game {gameId}");  // ← Ajouté
    }
    
    public void SendRequestGamesList()
    {
        if (_playerId == Guid.Empty)
        {
            GD.PrintErr("[GameUpdater] Cannot request games list: not authenticated yet");
            return;
        }
        
        GD.Print("Requesting games list from server...");
        var payload = new ClientRequestGamesList()
        {
            PlayerId = _playerId
        };
        _networkClient.SendMessage(payload);
    }
    
    public void SendCreateGameRequest()
    {
        if (_playerId == Guid.Empty)
        {
            GD.PrintErr("[GameUpdater] Cannot send CreateGame: not authenticated yet. Please wait...");
            return;
        }
        
        GD.Print("Sending create game request...");
        var payload = new ClientCreateGame()
        {
            PlayerId = _playerId
        };
        _networkClient.SendMessage(payload);
        GD.Print("Sent.");
    }
    
    public void SendMovePieceRequest(Guid gameId, ChessMove move)
    {
        if (_playerId == Guid.Empty)
        {
            GD.PrintErr("[GameUpdater] Cannot send Move: not authenticated yet");
            return;
        }
        
        if (gameId == Guid.Empty)
        {
            GD.PrintErr("[GameUpdater] Cannot send Move: invalid GameId");
            return;
        }
        
        ClientMove moveMessage = new ClientMove
        {
            PlayerId = _playerId,
            GameId = gameId,
            Move = move
        };
        _networkClient.SendMessage(moveMessage);
    }

    public void SendQuitGameRequest(Guid gameId)
    {
        if (_playerId == Guid.Empty)
        {
            GD.PrintErr("[GameUpdater] Cannot send QuitGame: not authenticated yet");
            return;
        }
        
        // Set flag to prevent rejoining the game while quitting
        _isQuitting = true;
        GD.Print("[GameUpdater] Quitting game - ignoring further GameJoined messages");
        
        var message = new ClientQuitGame
        {
            PlayerId = _playerId,
            GameId = gameId
        };
        _networkClient.SendMessage(message);
    }
    
    public void SendPromoteRequest(Guid gameId, PieceType promotionChoice)
    {
        if (_playerId == Guid.Empty)
        {
            GD.PrintErr("[GameUpdater] Cannot send Promote: not authenticated yet");
            return;
        }
        
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