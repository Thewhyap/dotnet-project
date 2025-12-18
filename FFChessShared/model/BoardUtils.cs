namespace FFChessShared;

/**
 * Utility class for creating and manipulating chess boards
 */
public static class BoardUtils
{
	
	public const int StandardBoardSize = 8;

	/**
	 * Creates a standard chess board with pieces in starting positions
	 */
	public static Board CreateStandardBoard()
	{
		var board = new Board(StandardBoardSize);
		
		// Placer les pièces blanches (ligne 1 et 2)
		PlaceBackRow(board, 0, PieceColor.White);
		PlacePawns(board, 1, PieceColor.White);
		
		// Placer les pièces noires (ligne 7 et 8)
		PlaceBackRow(board, 7, PieceColor.Black);
		PlacePawns(board, 6, PieceColor.Black);
		
		return board;
	}

	/**
	 * Creates an empty chess board of the given size
	 */
	public static Board CreateEmptyBoard(int size = StandardBoardSize)
	{
		return new Board(size);
	}

	/**
	 * Place a piece at the given position
	 */
	public static void PlacePiece(Board board, int row, int col, Piece piece)
	{
		if (IsValidPosition(board, row, col))
		{
			board.Cells[row, col] = piece;
		}
	}

	/**
	 * Get the piece at the given position, or null if empty or invalid
	 */
	public static Piece? GetPieceAt(Board board, int row, int col)
	{
		if (IsValidPosition(board, row, col))
		{
			return board.Cells[row, col];
		}
		return null;
	}

	/**
	 * Check if the given position is valid on the board
	 */
	public static bool IsValidPosition(Board board, int row, int col)
	{
		return row >= 0 && row < board.Size && col >= 0 && col < board.Size;
	}

	/**
	 * Place the back row pieces on the given row
	 */
	private static void PlaceBackRow(Board board, int row, PieceColor color)
	{
		// Tours
		board.Cells[row, 0] = new Piece(PieceType.Rook, color);
		board.Cells[row, 7] = new Piece(PieceType.Rook, color);
		
		// Cavaliers
		board.Cells[row, 1] = new Piece(PieceType.Knight, color);
		board.Cells[row, 6] = new Piece(PieceType.Knight, color);
		
		// Fous
		board.Cells[row, 2] = new Piece(PieceType.Bishop, color);
		board.Cells[row, 5] = new Piece(PieceType.Bishop, color);
		
		// Dame et Roi
		board.Cells[row, 3] = new Piece(PieceType.Queen, color);
		board.Cells[row, 4] = new Piece(PieceType.King, color);
	}

	/**
	 * Place the pawns on the given row
	 */
	private static void PlacePawns(Board board, int row, PieceColor color)
	{
		for (int col = 0; col < StandardBoardSize; col++)
		{
			board.Cells[row, col] = new Piece(PieceType.Pawn, color);
		}
	}
}

