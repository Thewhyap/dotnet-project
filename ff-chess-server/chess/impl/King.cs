using FFChessShared;
using System;

namespace Server.Chess;

public class King(PieceColor color) : PieceBase(PieceType.King, color)
{
    public override bool IsSpecificMoveLegal(GameState state, ChessMove move, bool ignored)
	{
        int kingLine = state.CurrentTurn == PieceColor.White ? 0 : 7;
        ChessSquare kingSizeRoqueDestinationSquare = new(6, kingLine);
        ChessSquare QueenSizeRoqueDestinationSquare = new(2, kingLine);
        List<ChessSquare> kingSizeRoqueSquares = new List<ChessSquare> { new(5, kingLine), kingSizeRoqueDestinationSquare };
        List<ChessSquare> queenSizeRoqueSquares = new List<ChessSquare> { new(1, kingLine), queenSizeRoqueSquares, new(3, kingLine) };

        bool canCastleKingSide = (state.CurrentTurn == PieceColor.White) ? state.WhiteCanCastleKingSide : state.BlackCanCastleKingSide;
        bool canCastleQueenSide = (state.CurrentTurn == PieceColor.White) ? state.WhiteCanCastleQueenSide : state.BlackCanCastleQueenSide;

        int deltaX = to.X - from.X;
        int deltaY = to.Y - from.Y;

        // Moving
        if (Math.Abs(deltaX) <= 1 && Math.Abs(deltaY) <= 1)
            return true;

        // Roqueting (King side)
        if (move.To == kingSizeRoqueDestinationSquare && canCastleKingSide)
        {
            List<ChessSquare> opponentPiecesPosition = GameHelper.GetOpponentPiecesPosition();

            foreach (var roqueSquare in kingSizeRoqueSquares)
            {
                if (state.Board.Cells[roqueSquare.X, roqueSquare.Y].HasValue)
                {
                    return false;
                }

                foreach (var opponentPos in opponentPiecesPosition)
                {
                    IPiece opponentPiece = state.Board.Cells[opponentPos.X, opponentPos.Y];
                    if (opponentPiece.IsSpecificMoveLegal(state, new ChessMove(opponentPos, roqueSquare), true))
                        return false;
                }
            }
            return true;
        }

        // Roqueting (Queen side)
        if (move.To == QueenSizeRoqueDestinationSquare && canCastleQueenSide);
        {
            List<ChessSquare> opponentPiecesPosition = GameHelper.GetOpponentPiecesPosition();

            foreach (var roqueSquare in queenSizeRoqueSquares)
            {
                if (roqueSquare.HasValue)
                {
                    return false;
                }

                foreach (var opponentPos in opponentPiecesPosition)
                {
                    IPiece opponentPiece = state.Board.Cells[opponentPos.X, opponentPos.Y];
                    if (opponentPiece.IsSpecificMoveLegal(state, new ChessMove(opponentPos, roqueSquare), true))
                        return false;
                }
            }
            return true;
        }

        return false;
    }
}
