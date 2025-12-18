namespace FFChessShared;

/// <summary>
/// Utilitaire pour initialiser et configurer des plateaux d'échec
/// </summary>
public static class BoardUtils
{
	/// <summary>
	/// Taille standard d'un plateau d'échec
	/// </summary>
	public const int StandardBoardSize = 8;

	/// <summary>
	/// Crée un plateau d'échec standard (8x8) avec toutes les pièces en position initiale
	/// </summary>
	/// <returns>Un plateau d'échec configuré</returns>
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

	/// <summary>
	/// Crée un plateau d'échec vide d'une taille donnée
	/// </summary>
	/// <param name="size">La taille du plateau (8 pour standard)</param>
	/// <returns>Un plateau d'échec vide</returns>
	public static Board CreateEmptyBoard(int size = StandardBoardSize)
	{
		return new Board(size);
	}

	/// <summary>
	/// Place une pièce sur le plateau
	/// </summary>
	/// <param name="board">Le plateau</param>
	/// <param name="row">La ligne (0-7)</param>
	/// <param name="col">La colonne (0-7)</param>
	/// <param name="piece">La pièce à placer</param>
	public static void PlacePiece(Board board, int row, int col, Piece piece)
	{
		if (IsValidPosition(board, row, col))
		{
			board.Cells[row, col] = piece;
		}
	}

	/// <summary>
	/// Récupère une pièce à une position donnée
	/// </summary>
	/// <param name="board">Le plateau</param>
	/// <param name="row">La ligne</param>
	/// <param name="col">La colonne</param>
	/// <returns>La pièce ou null si la case est vide</returns>
	public static Piece? GetPieceAt(Board board, int row, int col)
	{
		if (IsValidPosition(board, row, col))
		{
			return board.Cells[row, col];
		}
		return null;
	}

	/// <summary>
	/// Vérifie si une position est valide sur le plateau
	/// </summary>
	/// <param name="board">Le plateau</param>
	/// <param name="row">La ligne</param>
	/// <param name="col">La colonne</param>
	/// <returns>True si la position est valide</returns>
	public static bool IsValidPosition(Board board, int row, int col)
	{
		return row >= 0 && row < board.Size && col >= 0 && col < board.Size;
	}

	/// <summary>
	/// Place la rangée arrière de pièces (tours, cavaliers, fous, dame, roi)
	/// </summary>
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

	/// <summary>
	/// Place la rangée de pions
	/// </summary>
	private static void PlacePawns(Board board, int row, PieceColor color)
	{
		for (int col = 0; col < StandardBoardSize; col++)
		{
			board.Cells[row, col] = new Piece(PieceType.Pawn, color);
		}
	}
}

