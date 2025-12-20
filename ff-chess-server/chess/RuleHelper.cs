using FFChessShared;

namespace Server.Chess;

public static class RuleHelper
{
    public static PieceBase? GetPieceRuleFromSquare(Board board, ChessSquare square)
    {
        PieceBase? piece = board.Cells[square.X, square.Y];
        if (!piece.HasValue)
            return null;
        return PieceRuleRegistry.GetRule(piece.Value);
    }

    public static TurnStatus MoveAction(GameState state, ChessMove move)
    {
        Board board = state.Board;

        PieceBase piece = GetPieceRuleFromSquare(board, move).Value; // We know that its not null because MoveAction is called after a move has been validated
        PieceBase? targetPiece = GetPieceRuleFromSquare(board, move);


        if (piece.PieceData.Type == PieceType.Pawn || targetPiece.HasValue)
        {
            state.DrawMoveClock = 0;
        }
        else
        {
            state.DrawMoveClock += 1;
        }

        if (piece.PieceData.Type == PieceType.Rook)
        {
            if (state.CurrentTurn == PieceColor.White)
            {
                int row = board.WhiteBackRow;
                if (move.From.X == board.RookKingSideColumn && move.From.Y == row)
                    state.WhiteCanCastleKingSide = false;

                if (move.From.X == board.RookQueenSideColumn && move.From.Y == row)
                    state.WhiteCanCastleQueenSide = false;
            }

            if (state.CurrentTurn == PieceColor.Black)
            {
                int row = board.BlackBackRow;
                if (move.From.X == board.RookKingSideColumn && move.From.Y == row)
                    state.BlackCanCastleKingSide = false;

                if (move.From.X == board.RookQueenSideColumn && move.From.Y == row)
                    state.BlackCanCastleQueenSide = false;
            }
        }

        if (piece.PieceData.Type == PieceType.King)
        {
            if (state.CurrentTurn == PieceColor.White)
            {
                state.WhiteCanCastleQueenSide = false;
                state.WhiteCanCastleKingSide = false;
            }

            if (state.CurrentTurn == PieceColor.Black)
            {
                state.BlackCanCastleQueenSide = false;
                state.BlackCanCastleKingSide = false;
            }
        }

        if (piece.PieceData.Type == PieceType.Pawn)
        {
            int deltaY = move.To.Y - move.From.Y;
            int direction = deltaY > 0 ? 1 : -1;
            if (Math.Abs(deltaY) == 2)
            {
                state.EnPassantTarget = new ChessSquare(move.From.X, move.From.Y + direction);
            }
            else
            {
                state.EnPassantTarget = null;
            }

            if(state.CurrentTurn == PieceColor.White)
            {
                if (move.To.Y == board.Size - 1)
                {
                    return TurnStatus.WaitingPromotion;
                }
            }

            if(state.CurrentTurn == PieceColor.Black)
            {
                if (move.To.Y == 0)
                {
                    return TurnStatus.WaitingPromotion;
                }
            }   
        }
        else
        {
            state.EnPassantTarget = null;
        }

        return TurnStatus.WaitingMove;
    }

    
    // This is not optimized at all (and that does not take into account repetition)
    public static TurnStatus CheckWinCondition(GameState state)
    {
        if (!OpponentHasLegalMove(state))
        {
            // Check for checkmate first
            if(IsKingAttacked(state, true))
            {
                return state.CurrentTurn == PieceColor.White ? TurnStatus.WinWhite : TurnStatus.WinBlack;
            }
            // Stalemate
            return TurnStatus.Draw;
        }

        // 50 turn rule
        if (state.DrawMoveClock >= 50)
            return TurnStatus.Draw;

        var opponentPiecesPosition = GameHelper.GetOpponentPiecesPosition(state);
        var playerPiecesPosition = GameHelper.GetPiecesPosition(state);

        var opponentPieces = GetPiecesFromPosition(opponentPiecesPosition);
        var playerPieces = GetPiecesFromPosition(playerPiecesPosition);

        // Insufficient material
        if (IsInsufficientMaterial(playerPieces) || IsInsufficientMaterial(opponentPieces))
            return TurnStatus.Draw;
        
        return TurnStatus.WaitingMove;
    }

    private static bool OpponentHasLegalMove(GameState state)
    {
        var opponentPiecesPosition = GameHelper.GetOpponentPiecesPosition(state);
        var opponentPieces = GetPiecesFromPosition(opponentPiecesPosition);

        foreach (var piece in opponentPieces)
        {
            for (int x = 0; x < board.Size; x++)
            {
                for (int y = 0; y < board.Size; y++)
                {
                    if(piece.IsMoveLegal(state, new ChessMove(x, y)))
                        return true;
                }
            }
        }
        return false;
    }

    private static bool IsKingAttacked(GameState state, bool isOpponentKing = false)
    {
        PieceColor kingColor = isOpponentKing ? state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White : state.CurrentTurn;
        PieceColor attackersColor = kingColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        var opponentPiecesPosition = GameHelper.GetPiecesPosition(state.Board, attackersColor);
        ChessSquare kingPosition = GameHelper.GetKingPosition(state.Board, kingColor);

        foreach (var opponentPos in opponentPiecesPosition)
        {
            var kingAttack = new ChessMove(opponentPos, kingPosition);
            PieceBase piece = GetPieceRuleFromSquare(state.Board, opponentPos).Value;
            if (piece.IsSpecificMoveLegal(state, kingAttack))
                return true;
        }

        return false;
    }

    private static List<PieceBase> GetPiecesFromPosition(Board board, List<ChessSquare> piecePositions)
    {
        var pieces = new List<PieceBase>();

        foreach (var pos in piecePositions)
        {
            var piece = GetPieceRuleFromSquare(board, pos);
            if (piece.HasValue)
                pieces.Add(piece.Value);
        }

        return pieces;
    }

    private static bool IsInsufficientMaterial(List<PieceBase> pieces)
    {
        // Only king
        if (pieces.Count < 2)
            return true;

        // King + Bishop OR King + Knight
        if (pieces.Count == 2)
        {
            bool hasMinor =
                pieces.Any(p => p.PieceData.Type == PieceType.Bishop || p.Type == PieceType.Knight);

            return hasMinor;
        }

        // King + 2 Knights
        if (pieces.Count == 3)
        {
            int knightCount = pieces.Count(p => p.PieceData.Type == PieceType.Knight);

            return knightCount == 2;
        }

        return false;
    }
}
