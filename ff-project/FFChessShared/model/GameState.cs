using MessagePack;
using FFChessShared;

[MessagePackObject]
public class GameState
{
    public GameState() { }
    public GameState(Board board, PieceColor turn = PieceColor.White)
    {
        Board = board;
        CurrentTurn = turn;
    }
    [Key(0)] public Board Board { get; set; } = null!;
    [Key(1)] public PieceColor CurrentTurn { get; set; } = PieceColor.White;

    [IgnoreMember] public bool WhiteCanCastleKingSide { get; set; } = true;
    [IgnoreMember] public bool WhiteCanCastleQueenSide { get; set; } = true;
    [IgnoreMember] public bool BlackCanCastleKingSide { get; set; } = true;
    [IgnoreMember] public bool BlackCanCastleQueenSide { get; set; } = true;
    [IgnoreMember] public int DrawMoveClock { get; set; } = 0;

    [IgnoreMember] public ChessSquare? EnPassantTarget { get; set; } = null;
}
