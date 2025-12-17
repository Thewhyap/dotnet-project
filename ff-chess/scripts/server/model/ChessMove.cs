using Godot;
using System;

namespace Shared;

public readonly struct ChessMove
{
	public readonly Case From;
	public readonly Case To;
	
	public ChessMove(Case from, Case to)
	{
		From = from;
		To = to;
	}
}
