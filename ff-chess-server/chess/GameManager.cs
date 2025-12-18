using FFChessShared;
using System;

namespace Server.Chess;

public class GameManager
{
    private readonly GameState state;

    public GameManager()
    {
        state = new GameState(GameHelper.InitializeBoard());
    }

    // Returns true if the move was successful, false otherwise
    public bool Move(ChessMove move)
	{
        if (GameHelper.IsOffBoard(state.Board, move.From) || GameHelper.IsOffBoard(state.Board, move.To))
            return false;
        
		var piece = state.Board.Cells[move.From.X, move.From.Y];

        if (!piece.HasValue)
            return false;

        if(piece.Value.Color != state.CurrentTurn)
            return false;

        if(!piece.Value.IsMoveLegal(state, move))
            return false;

        RuleHelper.MoveAction(state, move);

        state.Board.MovePiece(move.From, move.To);

        GameResult? result = RuleHelper.CheckWinCondition(state);

        if (result.HasValue)
        {
            EndGame();
        }
        else
        {
            NextTurn();
        }

        return true;
    }

    private void NextTurn()
    {
        state.CurrentTurn = state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    private void EndGame()
    {
        //TODO
    }
}
