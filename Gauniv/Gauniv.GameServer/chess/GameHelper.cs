using FFChessShared;

namespace Server.Chess;

public static class GameHelper
{
    public static ChessSquare GetKingPosition(GameState state)
    {
        return GetKingPosition(state.Board, state.CurrentTurn);
    }

    public static ChessSquare GetOpponentKingPosition(GameState state)
    {
        return GetKingPosition(state.Board, state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White);
    }

    public static ChessSquare GetKingPosition(Board board, PieceColor color)
    {
        for (int x = 0; x < board.Size; x++)
        {
            for (int y = 0; y < board.Size; y++)
            {
                PieceData? piece = board.Cells[x, y];
                if (piece != null && piece.Type == PieceType.King && piece.Color == color)
                    return new ChessSquare(x, y);
            }
        }

        throw new InvalidOperationException($"King of color {color} not found on the board.");
    }

    public static List<ChessSquare> GetOpponentPiecesPosition(GameState state)
    {
        return GetPiecesPosition(state.Board, state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White);
    }

    public static List<ChessSquare> GetPiecesPosition(GameState state)
    {
        return GetPiecesPosition(state.Board, state.CurrentTurn);
    }

    public static List<ChessSquare> GetPiecesPosition(Board board, PieceColor color)
    {
        var opponentPiecesPosition = new List<ChessSquare>();

        for (int x = 0; x < board.Size; x++)
        {
            for (int y = 0; y < board.Size; y++)
            {
                PieceData? piece = board.Cells[x, y];
                if (piece != null)
                {
                    if (piece.Color == color)
                        opponentPiecesPosition.Add(new ChessSquare(x, y));
                }
            }
        }

        return opponentPiecesPosition;
    }

    public static Board InitializeBoard()
    {
        var board = new Board(8);

        // Pawns
        for (int x = 0; x < 8; x++)
        {
            board.Cells[x, board.WhitePawnRow] = new PieceData(PieceType.Pawn, PieceColor.White);
            board.Cells[x, board.BlackPawnRow] = new PieceData(PieceType.Pawn, PieceColor.Black);
        }

        // Rooks
        board.Cells[board.RookQueenSideColumn, board.WhiteBackRow] = new PieceData(PieceType.Rook, PieceColor.White);
        board.Cells[board.RookKingSideColumn, board.WhiteBackRow] = new PieceData(PieceType.Rook, PieceColor.White);
        board.Cells[board.RookQueenSideColumn, board.BlackBackRow] = new PieceData(PieceType.Rook, PieceColor.Black);
        board.Cells[board.RookKingSideColumn, board.BlackBackRow] = new PieceData(PieceType.Rook, PieceColor.Black);

        // Knights
        board.Cells[board.KnightQueenSideColumn, board.WhiteBackRow] = new PieceData(PieceType.Knight, PieceColor.White);
        board.Cells[board.KnightKingSideColumn, board.WhiteBackRow] = new PieceData(PieceType.Knight, PieceColor.White);
        board.Cells[board.KnightQueenSideColumn, board.BlackBackRow] = new PieceData(PieceType.Knight, PieceColor.Black);
        board.Cells[board.KnightKingSideColumn, board.BlackBackRow] = new PieceData(PieceType.Knight, PieceColor.Black);

        // Bishops
        board.Cells[board.BishopQueenSideColumn, board.WhiteBackRow] = new PieceData(PieceType.Bishop, PieceColor.White);
        board.Cells[board.BishopKingSideColumn, board.WhiteBackRow] = new PieceData(PieceType.Bishop, PieceColor.White);
        board.Cells[board.BishopQueenSideColumn, board.BlackBackRow] = new PieceData(PieceType.Bishop, PieceColor.Black);
        board.Cells[board.BishopKingSideColumn, board.BlackBackRow] = new PieceData(PieceType.Bishop, PieceColor.Black);

        // Queens
        board.Cells[board.QueenColumn, board.WhiteBackRow] = new PieceData(PieceType.Queen, PieceColor.White);
        board.Cells[board.QueenColumn, board.BlackBackRow] = new PieceData(PieceType.Queen, PieceColor.Black);

        // Kings
        board.Cells[board.KingColumn, board.WhiteBackRow] = new PieceData(PieceType.King, PieceColor.White);
        board.Cells[board.KingColumn, board.BlackBackRow] = new PieceData(PieceType.King, PieceColor.Black);

        return board;
    }

    public static bool IsOffBoard(Board board, ChessSquare chessSquare)
    {
        return (chessSquare.X < 0 || chessSquare.X > board.Size) && (chessSquare.Y < 0 || chessSquare.Y > board.Size);
    }
}
