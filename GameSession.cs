using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// Creates a new instance of the game session, configured with the desired GraphicsMode.
		/// </summary>
		/// <param name="graphicsMode">The graphics mode to use fo the game session.</param>
		internal GameSession(IGraphicsMode graphicsMode)
		{
			this.graphicsMode = graphicsMode;
		}

		/// <summary>
		/// This is the outer method.  It is called after creation to start the game session, and it returns when the user has quit the game.		
		/// </summary>
		internal void MainMenu()
		{
			MainMenuOption choice;

			do
			{
				choice = graphicsMode.MenuFromEnum<MainMenuOption>(TextStrings.MainMenu);
				if (choice == MainMenuOption.Play)
				{
					var game = new SnakeGame(Settings.defaultWidth, Settings.defaultHeight, Settings.startingLength, Settings.startingDirection, Settings.defaultDelay, graphicsMode);
					game.Play();
					graphicsMode.PostPlayCleanup();
				}
			} while (choice != MainMenuOption.Quit);

			// Game session is over and the player has chosen to quit.
			// There is the ability to quit the game via Environment.Exit(), so reaching this point is not gauranteed.
		}
	}
}
