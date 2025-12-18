using FFChessShared;

namespace Server.Chess;

public static class RuleHelper
{
    public static MoveAction(GameState state, ChessMove move)
    {
        Board board = state.Board;

        IPiece piece = board.Cells[move.From.X, move.From.Y].Value;
        IPiece? targetPiece = board.Cells[move.From.X, move.From.Y];


        if (piece.Type == PieceType.Pawn || targetPiece.HasValue)
        {
            state.DrawMoveClock = 0;
        }
        else
        {
            state.DrawMoveClock += 1;
        }

        if (piece.Type == PieceType.Rook)
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

        if (piece.Type == PieceType.King)
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

        if (piece.Type == PieceType.Pawn)
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
                    //TODO make white user choose a piece between Bishop, Rook, Knight and Queen
                    // and then replace pawn ChessSquare by the selected piece
                }
            }

            if(state.CurrentTurn == PieceColor.Black)
            {
                if (move.To.Y == 0)
                {
                    //TODO make black user choose a piece between Bishop, Rook, Knight and Queen
                    // and then replace pawn ChessSquare by the selected piece
                }
            }   
        }
        else
        {
            state.EnPassantTarget = null;
        }
    }

    
    // This is not optimized at all (and that does not take into account repetition)
    public GameResult? CheckWinCondition(GameState state)
    {
        if (!OpponentHasLegalMove(state))
        {
            // Check for checkmate first
            if(IsKingAttacked(state, true))
            {
                return state.CurrentTurn == PieceColor.White ? GameResult.White : GameResult.Black;
            }
            // Stalemate
            return GameResult.Draw;
        }

        // 50 turn rule
        if (state.DrawMoveClock >= 50)
            return GameResult.Draw;

        var opponentPiecesPosition = GameHelper.GetOpponentPiecesPosition(state);
        var playerPiecesPosition = GameHelper.GetPiecesPosition(state);

        var opponentPieces = GetPiecesFromPosition(opponentPiecesPosition);
        var playerPieces = GetPiecesFromPosition(playerPiecesPosition);

        // Insufficient material
        if (IsInsufficientMaterial(playerPieces) || IsInsufficientMaterial(opponentPieces))
            return GameResult.Draw;
        
        return false;
    }

    private static bool OpponentHasLegalMove(GameState state)
    {
        var opponentPiecesPosition = GameHelper.GetOpponentPiecesPosition(state);
        
        foreach(var opponentPos in opponentPiecesPosition)
        {
            IPiece piece = state.Board.Cells[opponentPos.X, opponentPos.Y].Value;
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
        PieceColor color = isOpponentKing ? state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White : state.CurrentTurn;
        var opponentPiecesPosition = GameHelper.GetPiecesPosition(state.Board, color);
        ChessSquare kingPosition = GameHelper.GetKingPosition(state.Board, color);

        foreach (var opponentPos in opponentPiecesPosition)
        {
            var kingAttack = new ChessMove(opponentPos, kingPosition);
            IPiece piece = state.Board.Cells[opponentPos.X, opponentPos.Y].Value
            if (piece.IsSpecificMoveLegal(state, kingAttack))
                return false;
        }
    }

    private static List<IPiece> GetPiecesFromPosition(Board board, List<ChessSquare> piecePositions)
    {
        var pieces = new List<IPiece>();

        foreach (var pos in piecePositions)
        {
            var piece = board.Cells[pos.X, pos.Y];
            if (piece.HasValue)
                pieces.Add(piece.Value);
        }

        return pieces;
    }

    private static bool IsInsufficientMaterial(List<IPiece> pieces)
    {
        // Only king
        if (pieces.Count < 2)
            return true;

        // King + Bishop OR King + Knight
        if (pieces.Count == 2)
        {
            bool hasMinor =
                pieces.Any(p => p.Type == PieceType.Bishop || p.Type == PieceType.Knight);

            return hasMinor;
        }

        // King + 2 Knights
        if (pieces.Count == 3)
        {
            int knightCount = pieces.Count(p => p.Type == PieceType.Knight);

            return knightCount == 2;
        }

        return false;
    }
}
