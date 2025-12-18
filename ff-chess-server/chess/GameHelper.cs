using FFChessShared;

namespace Server.Chess;

public static class GameHelper
{
    public ChessSquare GetKingPosition(GameState state)
    {
        return GetKingPosition(state.Board, state.CurrentTurn);
    }

    public ChessSquare GetOpponentKingPosition(GameState state)
    {
        return GetKingPosition(state.Board, state.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White);
    }

    public ChessSquare GetKingPosition(Board board, PieceColor color)
    {
        for (int x = 0; x < board.Size; x++)
        {
            for (int y = 0; y < board.Size; y++)
            {
                IPiece? piece = board.Cells[x, y];
                if (piece.HasValue && piece.Value.Type == PieceType.King && piece.Value.Color == color)
                    return new ChessSquare(x, y);
            }
        }

        throw new InvalidOperationException($"King of color {color} not found on the board.");
    }

    public static List<ChessSquare> GetOpponentPiecesPosition(GameState state)
    {
        return GetPiecesPosition(state.Board, state.CurrentTurn == PiecePosition.White ? PiecePosition.Black : PiecePosition.White);
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
                Piece? piece = board.Cells[x, y];
                if (piece.HasValue)
                {
                    if (piece.Value.Color == color)
                        opponentPiecesPosition.Add(new ChessSquare(x, y));
                }
            }
        }

        return opponentPiecesPosition;
    }

    public static bool IsOffBoard(Board board, ChessSquare chessSquare)
    {
        return (chessSquare.X < 0 || chessSquare.X > board.Size) && (chessSquare.Y < 0 || chessSquare.Y > board.Size);
    }

    public static Board InitializeBoard()
    {
        var board = new Board(8);

        // Pawns
        for (int x = 0; x < 8; x++)
        {
            board.Cells[x, board.WhitePawnRow] = new Pawn(PieceColor.White);
            board.Cells[x, board.BlackPawnRow] = new Pawn(PieceColor.Black);
        }

        // Rooks
        board.Cells[board.RookQueenSideColumn, board.WhiteBackRow] = new Rook(PieceColor.White);
        board.Cells[board.RookKingSideColumn, board.WhiteBackRow] = new Rook(PieceColor.White);
        board.Cells[board.RookQueenSideColumn, board.BlackBackRow] = new Rook(PieceColor.Black);
        board.Cells[board.RookKingSideColumn, board.BlackBackRow] = new Rook(PieceColor.Black);

        // Knights
        board.Cells[board.KnightQueenSideColumn, board.WhiteBackRow] = new Knight(PieceColor.White);
        board.Cells[board.KnightKingSideColumn, board.WhiteBackRow] = new Knight(PieceColor.White);
        board.Cells[board.KnightQueenSideColumn, board.BlackBackRow] = new Knight(PieceColor.Black);
        board.Cells[board.KnightKingSideColumn, board.BlackBackRow] = new Knight(PieceColor.Black);

        // Bishops
        board.Cells[board.BishopQueenSideColumn, board.WhiteBackRow] = new Bishop(PieceColor.White);
        board.Cells[board.BishopKingSideColumn, board.WhiteBackRow] = new Bishop(PieceColor.White);
        board.Cells[board.BishopQueenSideColumn, board.BlackBackRow] = new Bishop(PieceColor.Black);
        board.Cells[board.BishopKingSideColumn, board.BlackBackRow] = new Bishop(PieceColor.Black);

        // Queens
        board.Cells[board.QueenColumn, board.WhiteBackRow] = new Queen(PieceColor.White);
        board.Cells[board.QueenColumn, board.BlackBackRow] = new Queen(PieceColor.Black);

        // Kings
        board.Cells[board.KingColumn, board.WhiteBackRow] = new King(PieceColor.White);
        board.Cells[board.KingColumn, board.BlackBackRow] = new King(PieceColor.Black);

        return board;
    }

    private bool IsOffBoard(Board board, ChessSquare chessSquare)
    {
        return (chessSquare.X < 0 || chessSquare.X > board.Size) && (chessSquare.Y < 0 || chessSquare.Y > board.Size);
    }
}
