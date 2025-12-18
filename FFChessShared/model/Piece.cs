namespace FFChessShared;

public readonly struct Piece
{
	public PieceType Type { get; }
	public PieceColor Color { get; }

	public Piece(PieceType type, PieceColor color)
	{
		Type = type;
		Color = color;
	}
}
