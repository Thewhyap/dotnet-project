using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using FFChessShared;

public partial class GameUpdaterServer : Node
{
	private HttpListener _httpListener;
	private const int PORT = 8080;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpListener = new HttpListener();
		_httpListener.Prefixes.Add($"http://localhost:{PORT}/");
		_httpListener.Start();
		GD.Print("Game update server listening on port ", PORT);

		// Écouter les requêtes en arrière-plan
		Task.Run(ListenForRequests);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private async Task ListenForRequests()
	{
		while (_httpListener.IsListening)
		{
			try
			{
				HttpListenerContext context = await _httpListener.GetContextAsync();
				if (context.Request.Url.AbsolutePath == "/game/update")
				{
					HandleGameUpdate(context);
				}
			}
			catch (Exception ex)
			{
				GD.PrintErr("Error: ", ex.Message);
			}
		}
	}

	
	private void HandleGameUpdate(HttpListenerContext context)
	{
		using (var reader = new System.IO.StreamReader(context.Request.InputStream))
		{
			string jsonData = reader.ReadToEnd();
			Game game = JsonSerializer.Deserialize<Game>(jsonData);

			// Exécuter sur le thread principal
			CallDeferred(nameof(UpdateGameDeferred), jsonData);
		}

		context.Response.StatusCode = 200;
		context.Response.Close();
	}

	private void UpdateGameDeferred(string jsonData)
	{
		Game game = JsonSerializer.Deserialize<Game>(jsonData);

		var sceneRouter = GetTree().Root.GetNode<SceneRouter>("/root/ClientRoot/GameUpdaterServer/SceneRouter");
		if (sceneRouter != null)
		{
			sceneRouter.UpdateGame(game);
		}
	}

	public override void _ExitTree()
	{
		_httpListener?.Stop();
		_httpListener?.Close();
	}
}
