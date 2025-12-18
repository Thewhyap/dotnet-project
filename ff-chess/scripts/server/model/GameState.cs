namespace Shared;

public readonly struct GameState
{
	public Board Board { get; }
	public PieceColor CurrentTurn { get; }
	
	public bool WhiteCanCastleKingSide { get; }
	public bool WhiteCanCastleQueenSide { get; }
	public bool BlackCanCastleKingSide { get; }
	public bool BlackCanCastleQueenSide { get; }

	public ChessSquare? EnPassantTarget { get; }

	public GameState(
		Board board,
		PieceColor currentTurn,
		bool whiteCastleK,
		bool whiteCastleQ,
		bool blackCastleK,
		bool blackCastleQ,
		ChessSquare? enPassantTarget
	)
	{
		Board = board;
		CurrentTurn = currentTurn;
		WhiteCanCastleKingSide = whiteCastleK;
		WhiteCanCastleQueenSide = whiteCastleQ;
		BlackCanCastleKingSide = blackCastleK;
		BlackCanCastleQueenSide = blackCastleQ;
		EnPassantTarget = enPassantTarget;
	}
}
