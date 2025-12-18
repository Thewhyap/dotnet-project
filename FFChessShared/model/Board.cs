namespace FFChessShared;

public readonly struct Board
{
	public Piece?[,] Cells { get; }

	public int Size => Cells.GetLength(0);

	public Board(int size)
	{
		Cells = new Piece?[size, size];
	}
}
