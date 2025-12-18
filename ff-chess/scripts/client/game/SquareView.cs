using Godot;
using System;
using FFChess.scripts.client;

public partial class SquareView : Node2D
{
	private bool _isBlack;
	private const string Variant = "1";
	private Sprite2D _sprite;
	
	public SquareView(bool isBlack)
	{
		_isBlack = isBlack;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = new Sprite2D();
		_sprite.Texture = LoadTexture();
		_sprite.Centered = false;
		
		// Scale the sprite to fit within in the given square size
		float textureWidth = _sprite.Texture.GetWidth();
		float textureHeight = _sprite.Texture.GetHeight();
		float scaleX = GameConstants.SquareSize / textureWidth;
		float scaleY = GameConstants.SquareSize / textureHeight;
 
		_sprite.Scale = new Vector2(scaleX, scaleY);
		AddChild(_sprite);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	public void setCoordinates(float x, float y)
	{
		Position = new Vector2(x, y);
	}
	
	private Texture2D LoadTexture()
	{
		var color = _isBlack ? "black" : "white";
		var imagePath = $"res://assets/textures/squares/{color}_square_{Variant}.png";
		Texture2D texture = GD.Load<Texture2D>(imagePath);
		if (texture == null)
		{
			throw new Exception("Failed to load texture at path: " + imagePath);
		}
		return texture;
	}
}
