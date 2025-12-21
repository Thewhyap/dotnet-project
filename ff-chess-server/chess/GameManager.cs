using FFChessShared;
using System;

namespace Server.Chess;

public class GameManager
{
    public readonly Game Game;
    private readonly GameState state;

    public GameManager()
    {
        Game = new Game(new GameState(GameHelper.InitializeBoard()), LobbyNameGenerator.GenerateLobbyName());
        state = Game.GameState;
    }

    public bool Move(ChessMove move)
	{
        Console.WriteLine($"[GameManager] Attempting move from ({move.From.X}, {move.From.Y}) to ({move.To.X}, {move.To.Y})");
        
        if (GameHelper.IsOffBoard(state.Board, move.From) || GameHelper.IsOffBoard(state.Board, move.To))
        {
            Console.WriteLine("[GameManager] Move rejected: off board");
            return false;
        }
        
		var piece = state.Board.Cells[move.From.X, move.From.Y];

        if (piece == null)
        {
            Console.WriteLine("[GameManager] Move rejected: no piece at source");
            return false;
        }

        Console.WriteLine($"[GameManager] Piece: {piece.Type} {piece.Color}, Current turn: {state.CurrentTurn}");

        if(piece.Color != state.CurrentTurn)
        {
            Console.WriteLine("[GameManager] Move rejected: wrong color");
            return false;
        }

        var targetPiece = state.Board.Cells[move.To.X, move.To.Y];
        if (targetPiece != null)
        {
            Console.WriteLine($"[GameManager] Target square has piece: {targetPiece.Type} {targetPiece.Color}");
        }

        if(!PieceRuleRegistry.GetRule(piece.Type, piece.Color).IsMoveLegal(state, move))
        {
            Console.WriteLine("[GameManager] Move rejected: illegal move");
            return false;
        }

        Console.WriteLine("[GameManager] Move is legal, executing...");
        Game.TurnStatus = RuleHelper.MoveAction(state, move);

        state.Board.MovePiece(move.From, move.To);
        Console.WriteLine($"[GameManager] Move completed successfully");

        if (Game.TurnStatus == TurnStatus.WaitingPromotion)
        {
            return true;
        }

        Game.TurnStatus = RuleHelper.CheckWinCondition(state);

        if (RuleHelper.IsGameOver(Game))
        {
            EndGame();
        }
        else
        {
            NextTurn();
        }

        return true;
    }

    public bool Promote(PieceType promotionPiece)
    {
        var result = false;
        var piecesPosition = GameHelper.GetPiecesPosition(state);
        int promotionRow = state.CurrentTurn == PieceColor.White ? state.Board.BlackBackRow : state.Board.WhiteBackRow;
        foreach (var piecePos in piecesPosition)
        {
            PieceData? piece = state.Board.Cells[piecePos.X, piecePos.Y];
            if (piece != null && piece.Type == PieceType.Pawn && piecePos.Y == promotionRow)
            {
                state.Board.ChangePiece(piecePos, promotionPiece);
                result = true;
            }
        }

        Game.TurnStatus = RuleHelper.CheckWinCondition(state);

        if (RuleHelper.IsGameOver(Game))
        {
            EndGame();
        }
        else
        {
            NextTurn();
        }

        return result;
    }

    private void NextTurn()
    {
        state.CurrentTurn = state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    private void EndGame()
    {
        Game.Status = MatchStatus.Closed;
    }

    public void EndGameWithWin(PieceColor? winningColor)
    {
        Game.Status = MatchStatus.Closed;
        Game.TurnStatus = winningColor != null ? winningColor == PieceColor.White ? TurnStatus.WinWhite : TurnStatus.WinBlack : TurnStatus.Draw;
    }
}
