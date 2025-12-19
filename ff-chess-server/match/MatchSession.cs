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

    public bool AddPlayer(Player player)
    {
        bool isPlayer = true;

        if (WhitePlayer == null && BlackPlayer == null)
        {
            if (_rand.Next(2) == 0)
            {
                WhitePlayer = player;
                player.AssignedColor = PieceColor.White;
            }
            else
            {
                BlackPlayer = player;
                player.AssignedColor = PieceColor.Black;
            }
        }
        else if (WhitePlayer == null)
        {
            WhitePlayer = player;
            player.AssignedColor = PieceColor.White;
        }
        else if (BlackPlayer == null)
        {
            BlackPlayer = player;
            player.AssignedColor = PieceColor.Black;
        }
        else
        {
            player.AssignedColor = null;
            _viewers.Add(player);
            isPlayer = false;
        }

        await player.SendPlayerInfo();

        return isPlayer;
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
        if (player.AssignedColor == null)
            return false; // Viewer

        return player.AssignedColor == GameManager.Game.GameState.CurrentTurn;
    }

    private void BroadcastGameState(string? message = null)
    {
        WhitePlayer?.SendGameState(GameManager.Game.GameState, GameManager.Game.TurnStatus, message);
        BlackPlayer?.SendGameState(GameManager.Game.GameState, GameManager.Game.TurnStatus, message);
        foreach (var viewer in _viewers)
            viewer.SendGameState(GameManager.Game.GameState, GameManager.Game.TurnStatus, message);
    }
}
