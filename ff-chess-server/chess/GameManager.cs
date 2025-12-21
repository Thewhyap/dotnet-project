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
        if (GameHelper.IsOffBoard(state.Board, move.From) || GameHelper.IsOffBoard(state.Board, move.To))
        {
            Console.WriteLine("Unable to move, move is off board");
            return false;
        }
        
		var piece = state.Board.Cells[move.From.X, move.From.Y];

        if (piece == null)
        {
            Console.WriteLine("Unable to move, no piece found at position");
            return false;
        }

        if(piece.Color != state.CurrentTurn)
        {
            return false;
        }

        if(!PieceRuleRegistry.GetRule(piece.Type, piece.Color).IsMoveLegal(state, move))
        {
            return false;
        }

        Game.TurnStatus = RuleHelper.MoveAction(state, move);

        state.Board.MovePiece(move.From, move.To);

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
