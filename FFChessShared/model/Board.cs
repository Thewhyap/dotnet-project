using MessagePack;

namespace FFChessShared;

[MessagePackObject]
public class Board
{
    public Board() { }

    public Board(int size)
    {
        Size = size;
        Cells = new PieceData?[size, size];

        WhiteBackRow = 0;
        WhitePawnRow = 1;
        BlackBackRow = size - 1;
        BlackPawnRow = size - 2;
        RookKingSideColumn = size - 1;
        RookQueenSideColumn = 0;
        KnightKingSideColumn = size - 2;
        KnightQueenSideColumn = 1;
        BishopKingSideColumn = size - 3;
        BishopQueenSideColumn = 2;
        KingColumn = size - 5;
        QueenColumn = size - 4;
    }

    [Key(0)] public PieceData?[,] Cells { get; } = null!;

    [IgnoreMember] public int Size { get; set; }

    [IgnoreMember] public readonly int WhiteBackRow;
    [IgnoreMember] public readonly int WhitePawnRow;
    [IgnoreMember] public readonly int BlackBackRow;
    [IgnoreMember] public readonly int BlackPawnRow;
    [IgnoreMember] public readonly int RookKingSideColumn;
    [IgnoreMember] public readonly int RookQueenSideColumn;
    [IgnoreMember] public readonly int KnightKingSideColumn;
    [IgnoreMember] public readonly int KnightQueenSideColumn;
    [IgnoreMember] public readonly int BishopKingSideColumn;
    [IgnoreMember] public readonly int BishopQueenSideColumn;
    [IgnoreMember] public readonly int KingColumn;
    [IgnoreMember] public readonly int QueenColumn;

    public void MovePiece(ChessSquare from, ChessSquare to)
    {
        Cells[to.X, to.Y] = Cells[from.X, from.Y];
        Cells[from.X, from.Y] = null;
    }
}