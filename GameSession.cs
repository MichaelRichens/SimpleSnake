using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SimpleSnake
{
	/// <summary>
	/// This class represents a game session, and controls the launching of individual games.
	/// </summary>
	class GameSession
	{
		/// <summary>
		/// The graphics mode being used for interaction with the player.
		/// </summary>
		private readonly IGraphicsMode graphicsMode;

		/// <summary>
		/// Stores the highest score of all games played under this instance of GameSession.
		/// </summary>
		private long sessionHighScore = 0;

		private long allTimeHighScoreBackingField = 0;

		private long AllTimeHighScore
		{
			get => allTimeHighScoreBackingField;
			set
			{
				allTimeHighScoreBackingField = value;

				// Save the new high score to disk.
				// This is a synchronous file write - don't want to do it async since we allow Environmnet.Exit in response to an event when running in windowed mode, so want to complete the write before checking for new events.
				SaveToDataFile(Settings.jsonHighScore, value);
			}
		}

		/// <summary>
		/// Creates a new instance of the game session, configured with the desired GraphicsMode.
		/// </summary>
		/// <param name="graphicsMode">The graphics mode to use fo the game session.</param>
		/// <param name="allTimeHighScore">The all time high score - should be read from disk and passed in.</param>
		internal GameSession(IGraphicsMode graphicsMode, long allTimeHighScore)
		{
			this.graphicsMode = graphicsMode;

			// Write directly to the backing field to avoid saving it to file 
			allTimeHighScoreBackingField = allTimeHighScore;
		}

		/// <summary>
		/// This is the outer method.  It is called after creation to start the game session, and it returns when the user has quit the game.		
		/// </summary>
		internal void MainMenu()
		{
			MainMenuOption choice;

			do
			{
				choice = graphicsMode.Menu<MainMenuOption>(TextStrings.MainMenu);
				if (choice == MainMenuOption.Play)
				{
					// Play a game.
					var game = new SnakeGame(Settings.defaultWidth, Settings.defaultHeight, Settings.startingLength, Settings.startingDirection, Settings.defaultDelay, sessionHighScore, AllTimeHighScore, graphicsMode);
					game.Play();

					graphicsMode.PostPlayCleanup();

					// Check if a new high score was set.

					if (game.GameResults.Score > sessionHighScore)
					{
						sessionHighScore = game.GameResults.Score;
					}

					if (game.GameResults.Score > AllTimeHighScore)
					{
						AllTimeHighScore = game.GameResults.Score;
					}
				}
				else if (choice == MainMenuOption.Options)
				{
					// Displays the options menu and applies any changes made.
					ChangeOptions();
				}
			} while (choice != MainMenuOption.Quit);

			// Game session is over and the player has chosen to quit.
			// There is the ability to quit the game via Environment.Exit(), so reaching this point is not guaranteed.
		}

		/// <summary>
		/// Helper method which saves passed property and value to the game's data file.  The data must be a valid JSON value.
		/// This method blocks until file access is complete.
		/// </summary>
		/// <param name="key">The key to save the data under.</param></param>
		/// <param name="value">The data value to be saved.</param>
		private static void SaveToDataFile(string key, JToken value)
		{
			try
			{
				string directoryPath = Settings.userDataPath;
				string dataPath = Path.Combine(directoryPath, Settings.userDataFilename);

				// Check if the directory exists, and if not, create it.
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}

				JObject jsonObject;

				// Check if the file exists, and if not, create it and initialise it with an empty JSON object.
				if (!File.Exists(dataPath))
				{
					jsonObject = new JObject();
				}
				else
				{
					jsonObject = JObject.Parse(File.ReadAllText(dataPath));
				}

				jsonObject[key] = value;
				File.WriteAllText(dataPath, jsonObject.ToString());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving high score: {ex.Message}");
			}
		}

		/// <summary>
		/// Displays the options menu and applies any changes made.
		/// </summary>
		private void ChangeOptions()
		{
			OptionsMenuOption choice;
			do
			{
				choice = graphicsMode.Menu<OptionsMenuOption>(TextStrings.OptionsMenu);

				switch (choice)
				{
					case OptionsMenuOption.ChangeBoardSize:
						{

							break;
						}
				}
			} while (choice != OptionsMenuOption.Back);
		}
	}
}
