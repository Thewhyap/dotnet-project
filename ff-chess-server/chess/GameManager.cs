using FFChessShared;
using System;

namespace Server.Chess;

public class GameManager
{
    public readonly Game game;
    private readonly GameState state;

    public GameManager()
    {
        game = new Game(new GameState(GameHelper.InitializeBoard()), LobbyNameGenerator.GenerateLobbyName());
        state = game.GameState;
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

        game.TurnStatus = RuleHelper.MoveAction(state, move);

        state.Board.MovePiece(move.From, move.To);

        if (game.TurnStatus == TurnStatus.WaitingPromotion)
        {
            SendGame();
            return;
        }

        game.TurnStatus = RuleHelper.CheckWinCondition(state);

        if (game.TurnStatus == TurnStatus.WinWhite || game.TurnStatus == TurnStatus.WinBlack || game.TurnStatus == TurnStatus.Draw)
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
        game.Status = MatchStatus.Closed;
        SendGame();
    }

    public void EndGameWithWin(PieceColor winningColor)
    {
        game.Status = MatchStatus.Closed;
        game.TurnStatus = winningColor == PieceColor.White ? TurnStatus.WinWhite : TurnStatus.WinBlack;
        SendGame();
    }
}
