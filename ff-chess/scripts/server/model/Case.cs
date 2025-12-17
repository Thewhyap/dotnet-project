using Godot;
using System;

namespace Shared;

public readonly struct Case
{
	private readonly int BOARD_SIZE = 8;
	
	public readonly int X;
	public readonly int Y;
	
	public Case(int x, int y)
	{
		X = x;
		Y = y;
	}

	public bool IsValid() => X >= 0 && X < BOARD_SIZE && Y >= 0 && Y < BOARD_SIZE;
}
