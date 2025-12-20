using FFChessShared;
using Server.Chess;

namespace Server.Match;

public class MatchSession(GameManager gameManager)
{
    private static readonly Random _rand = new();

    public Player? WhitePlayer { get; private set; }
    public Player? BlackPlayer { get; private set; }
    private readonly HashSet<Player> _viewers = new();

    public GameManager GameManager { get; private set; } = gameManager;

    public void AddPlayer(Player player)
    {
        if (!WhitePlayer.HasValue && !BlackPlayer == null)
        {
            if (_rand.Next(2) == 0)
            {
                WhitePlayer.Value = player;
                player.SendGameJoined(gameManager.Game, PieceColor.White);
            }
            else
            {
                BlackPlayer.Value = player;
                player.SendGameJoined(gameManager.Game, PieceColor.Black);
            }
        }
        else if (!WhitePlayer.HasValue)
        {
            WhitePlayer.Value = player;
            player.SendGameJoined(gameManager.Game, PieceColor.White);
        }
        else if (!BlackPlayer.HasValue)
        {
            BlackPlayer.Value = player;
            player.SendGameJoined(gameManager.Game, PieceColor.Black);
        }
        else
        {
            _viewers.Add(player);
            player.SendGameJoined(gameManager.Game, null);
        }

        await player.SendPlayerInfo();
    }

    public void RemovePlayer(Player player)
    {
        if (player == WhitePlayer)
        {
            WhitePlayer = null;
            if (GameManager.Game.Status == MatchStatus.InGame)
                GameManager.EndGameWithWin(PieceColor.Black);
        }
        else if (player == BlackPlayer)
        {
            BlackPlayer = null;
            if (GameManager.Game.Status == MatchStatus.InGame)
                GameManager.EndGameWithWin(PieceColor.White);
        }
        else
        {
            _viewers.Remove(player);
        }
    }

    public bool TryPromote(Player player, PieceType promotionChoice)
    {
        if (!IsPlayerTurn(player))
            return false;

        if (GameManager.Game.TurnStatus != TurnStatus.WaitingPromotion)
            return false;

        var result = GameManager.Promote(promotionChoice);

        BroadcastGameState();

        return result;
    }

    public bool TryMakeMove(Player player, ChessMove move)
    {
        if (!IsPlayerTurn(player))
            return false;

        if (GameManager.Game.TurnStatus != TurnStatus.WaitingMove)
            return false;

        var result = GameManager.Move(move);

        BroadcastGameState();

        return result;
    }

    private bool IsPlayerTurn(Player player)
    {
        PieceColor currentTurn = GameManager.Game.GameState.CurrentTurn;
        if (currentTurn == PieceColor.White)
            return player == WhitePlayer;

        return player == BlackPlayer;
    }

    private void BroadcastGameState()
    {
        WhitePlayer?.SendGameState(GameManager.Game.GameState, GameManager.Game.TurnStatus);
        BlackPlayer?.SendGameState(GameManager.Game.GameState, GameManager.Game.TurnStatus);
        foreach (var viewer in _viewers)
            viewer.SendGameState(GameManager.Game.GameState, GameManager.Game.TurnStatus);
    }

    private void BroadcastGameInfo()
    {
        GameInfo gameInfo = GameManager.Game.GameInfo;

        WhitePlayer?.SendGameInfo(GameManager.Game.GameState, GameManager.Game.TurnStatus);
        BlackPlayer?.SendGameInfo(GameManager.Game.GameState, GameManager.Game.TurnStatus);
        foreach (var viewer in _viewers)
            viewer.SendGameInfo(GameManager.Game.GameState, GameManager.Game.TurnStatus);
    }
}
