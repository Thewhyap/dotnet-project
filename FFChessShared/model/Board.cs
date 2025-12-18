namespace FFChessShared;

public readonly struct Board
{
    public IPiece?[,] Cells { get; }

    public int Size => Cells.GetLength(0);

    public readonly int WhiteBackRow = 0;
    public readonly int WhitePawnRow = 1;
    public readonly int BlackBackRow;
    public readonly int BlackPawnRow;
    public readonly int RookKingSideColumn;
    public readonly int RookQueenSideColumn = 0;
    public readonly int KnightKingSideColumn;
    public readonly int KnightQueenSideColumn = 1;
    public readonly int BishopKingSideColumn;
    public readonly int BishopQueenSideColumn = 2;
    public readonly int KingColumn;
    public readonly int QueenColumn;

    public Board(int size)
    {
        Cells = new IPiece?[size, size];

        BlackBackRow = size - 1;
        BlackPawnRow = size - 2;
        RookKingSideColumn = size - 1;
        KnightKingSideColumn = size - 2;
        BishopKingSideColumn = size - 3;
        QueenColumn = size - 4;
        KingColumn = size - 5;
    }

    public void MovePiece(ChessSquare from, ChessSquare to)
    {
        Cells[to.X, to.Y] = Cells[from.X, from.Y];
        Cells[from.X, from.Y] = null;
    }
}
