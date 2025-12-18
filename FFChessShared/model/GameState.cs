using FFChessShared;

public class GameState
{
    public Board Board { get; }
    public PieceColor CurrentTurn { get; set; }

    public bool WhiteCanCastleKingSide { get; set; }
    public bool WhiteCanCastleQueenSide { get; set; }
    public bool BlackCanCastleKingSide { get; set; }
    public bool BlackCanCastleQueenSide { get; set; }
    public int DrawMoveClock {  get; set; }

    public ChessSquare? EnPassantTarget { get; set; }

    public GameState( Board board, PieceColor turn = PieceColor.White )
    {
        Board = board;
        CurrentTurn = turn;
        WhiteCanCastleKingSide = true;
        WhiteCanCastleQueenSide = true;
        BlackCanCastleKingSide = true;
        BlackCanCastleQueenSide = true;
        EnPassantTarget = null;
    }
}
