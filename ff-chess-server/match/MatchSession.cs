using FFChessShared;
using Server.Chess;

namespace Server.Match;

public class MatchSession()
{
    private static readonly Random _rand = new();

    public Player? WhitePlayer { get; private set; }
    public Player? BlackPlayer { get; private set; }
    private readonly HashSet<Player> _viewers = new();

    public GameManager GameManager { get; private set; } = new();

    public async Task AddPlayer(Player player)
    {
        Console.WriteLine($"[MatchSession] Adding player {player.PlayerInfo.PlayerName} to game {GameManager.Game.GameId}");
        
        if (WhitePlayer == null && BlackPlayer == null)
        {
            if (_rand.Next(2) == 0)
            {
                WhitePlayer = player;
                Console.WriteLine($"[MatchSession] Player assigned as White");
                await player.SendGameJoined(GameManager.Game, PieceColor.White);
            }
            else
            {
                BlackPlayer = player;
                Console.WriteLine($"[MatchSession] Player assigned as Black");
                await player.SendGameJoined(GameManager.Game, PieceColor.Black);
            }
        }
        else if (WhitePlayer == null)
        {
            WhitePlayer = player;
            Console.WriteLine($"[MatchSession] Player assigned as White");
            await player.SendGameJoined(GameManager.Game, PieceColor.White);
        }
        else if (BlackPlayer == null)
        {
            BlackPlayer = player;
            Console.WriteLine($"[MatchSession] Player assigned as Black");
            await player.SendGameJoined(GameManager.Game, PieceColor.Black);
        }
        else
        {
            _viewers.Add(player);
            Console.WriteLine($"[MatchSession] Player assigned as Viewer");
            await player.SendGameJoined(GameManager.Game, null);
        }

        Console.WriteLine($"[MatchSession] Broadcasting game info...");
        await BroadcastGameInfo();
    }

    public async Task RemovePlayer(Player player)
    {
        if (player == WhitePlayer)
        {
            WhitePlayer = null;
            if (GameManager.Game.Status == MatchStatus.InGame)
            {
                GameManager.EndGameWithWin(PieceColor.Black);
            }
            else
            {
                GameManager.EndGameWithWin(null);
            }
        }
        else if (player == BlackPlayer)
        {
            BlackPlayer = null;
            if (GameManager.Game.Status == MatchStatus.InGame)
            {
                GameManager.EndGameWithWin(PieceColor.White);
            }
            else
            {
                GameManager.EndGameWithWin(null);
            }
        }
        else
        {
            _viewers.Remove(player);
        }

        await BroadcastGameState();
    }

    public async Task<bool> TryPromote(Player player, PieceType promotionChoice)
    {
        if (!IsPlayerTurn(player))
            return false;

        if (GameManager.Game.TurnStatus != TurnStatus.WaitingPromotion)
            return false;

        if (GameManager.Game.TurnStatus != TurnStatus.WaitingPromotion)
            return false;

        var result = GameManager.Promote(promotionChoice);

        await BroadcastGameState();

        return result;
    }

    public async Task<bool> TryMakeMove(Player player, ChessMove move)
    {
        if (!IsPlayerTurn(player))
            Console.WriteLine("Unable to move, not player's turn");
            return false;

        if (GameManager.Game.TurnStatus != TurnStatus.WaitingMove)
            Console.WriteLine("Unable to move, not waiting for move");
            return false;

        if (GameManager.Game.TurnStatus != TurnStatus.WaitingMove)
            Console.WriteLine("Unable to move, game not waiting for move");
            return false;

        var result = GameManager.Move(move);

        await BroadcastGameState();

        return result;
    }

    private bool IsPlayerTurn(Player player)
    {
        PieceColor currentTurn = GameManager.Game.GameState.CurrentTurn;
        if (currentTurn == PieceColor.White)
            return player == WhitePlayer;

        return player == BlackPlayer;
    }

    private async Task BroadcastGameState()
    {
        var tasks = new List<Task>();

        if (WhitePlayer != null)
            tasks.Add(WhitePlayer.SendGameUpdate(GameManager.Game.GameState, GameManager.Game.TurnStatus));
        if (BlackPlayer != null)
            tasks.Add(BlackPlayer.SendGameUpdate(GameManager.Game.GameState, GameManager.Game.TurnStatus));

        foreach (var viewer in _viewers)
            tasks.Add(viewer.SendGameUpdate(GameManager.Game.GameState, GameManager.Game.TurnStatus));

        await Task.WhenAll(tasks);
    }

    private async Task BroadcastGameInfo()
    {
        GameInfo gameInfo = ToGameInfo();

        var tasks = new List<Task>();

        if (WhitePlayer != null)
            tasks.Add(WhitePlayer.SendGameInfo(gameInfo));
        if (BlackPlayer != null)
            tasks.Add(BlackPlayer.SendGameInfo(gameInfo));

        foreach (var viewer in _viewers)
            tasks.Add(viewer.SendGameInfo(gameInfo));

        await Task.WhenAll(tasks);
    }

    public GameInfo ToGameInfo()
    {
        string whitePlayerName = WhitePlayer != null ? WhitePlayer.PlayerInfo.PlayerName : "Waiting for opponent";
        string blackPlayerName = BlackPlayer != null ? BlackPlayer.PlayerInfo.PlayerName : "Waiting for opponent";
        return new(GameManager.Game.GameId, GameManager.Game.Name, GameManager.Game.Status, whitePlayerName, blackPlayerName);
    }

    public bool ContainsPlayer(Player player)
    {
        return WhitePlayer == player
            || BlackPlayer == player
            || _viewers.Contains(player);
    }
}
