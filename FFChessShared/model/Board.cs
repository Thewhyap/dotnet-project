namespace FFChessShared;

public readonly struct Board
{
	public Piece?[,] Cells { get; }

	public int Size => Cells.GetLength(0);

	public Board(int size)
	{
		Cells = new Piece?[size, size];
	}
	
	public List<Piece> GetAllPieces()
	{
		var pieces = new List<Piece>();
		
		for (int row = 0; row < Size; row++)
		{
			for (int col = 0; col < Size; col++)
			{
				if (Cells[row, col] is Piece piece)
				{
					pieces.Add(piece);
				}
			}
		}
}
