using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SimpleSnake
{
	class Program
	{
		static void Main(string[] args)
		{
			// Check arguments to select Console or windowed running mode.
			bool useConsole = false;
			foreach (string arg in args)
			{
				if (arg == "csl")
				{
					useConsole = true;
				}
			}

			// Create the IGraphicsMode for either Console or windowed mode. 
			IGraphicsMode graphicsMode = useConsole ? new ConsoleGraphics() : new SFMLGraphics();

			long savedHighScore;
			string dataPath = Path.Combine(Settings.userDataPath, Settings.userDataFilename);
			if (File.Exists(dataPath))
			{
				try
				{
					string json = File.ReadAllText(dataPath);
					JObject jsonObject = JObject.Parse(json);
					JToken? hsJson = jsonObject[Settings.jsonHighScore];
					if (hsJson != null)
					{
						savedHighScore = (long)hsJson;
					}
					else
					{
						savedHighScore = 0;
					}
				}
				catch (Exception ex)
				{
					savedHighScore = 0;
					Console.WriteLine($"Error loading high score: {ex.Message}");
				}
			}
			else
			{
				savedHighScore = 0;
			}


			// Create and run the game
			GameSession gameSession = new(graphicsMode, savedHighScore);
			gameSession.MainMenu();
		}
	}
}
