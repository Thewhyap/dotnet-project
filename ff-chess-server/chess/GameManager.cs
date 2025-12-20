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

    public void SendGame()
    {
        //TODO
    }

    public void Move(ChessMove move)
	{
        if (GameHelper.IsOffBoard(state.Board, move.From) || GameHelper.IsOffBoard(state.Board, move.To))
        {
            SendGame();
            return;
        }
        
		var piece = state.Board.Cells[move.From.X, move.From.Y];

        if (!piece.HasValue)
        {
            SendGame();
            return;
        }

        if(piece.Value.Color != state.CurrentTurn)
        {
            SendGame();
            return;
        }

        if(!piece.Value.IsMoveLegal(state, move))
        {
            SendGame();
            return;
        }

        Game.TurnStatus = RuleHelper.MoveAction(state, move);

        state.Board.MovePiece(move.From, move.To);

        if (Game.TurnStatus == TurnStatus.WaitingPromotion)
        {
            SendGame();
            return;
        }

        Game.TurnStatus = RuleHelper.CheckWinCondition(state);

        if (Game.TurnStatus == TurnStatus.WinWhite || Game.TurnStatus == TurnStatus.WinBlack || Game.TurnStatus == TurnStatus.Draw)
        {
            EndGame();
        }
        else
        {
            NextTurn();
        }
    }

    private void NextTurn()
    {
        state.CurrentTurn = state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
        SendGame();
    }

    private void EndGame()
    {
        Game.Status = MatchStatus.Closed;
        SendGame();
    }

    public void EndGameWithWin(PieceColor winningColor)
    {
        Game.Status = MatchStatus.Closed;
        Game.TurnStatus = winningColor == PieceColor.White ? TurnStatus.WinWhite : TurnStatus.WinBlack;
        SendGame();
    }
}
