using Godot;

namespace FFChess.scripts.client;

public static class GameConstants
{
    private static float _squareSize = 80f;
    
    public static float SquareSize
    {
        get => _squareSize;
        set => _squareSize = value;
    }
    
    public static void CalculateSquareSizeFromViewport(Vector2 viewportSize)
    {
        // 8x8 board, fit to viewport height
        _squareSize = viewportSize.Y / 8f;
    }
}