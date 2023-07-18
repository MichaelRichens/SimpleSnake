using System;

namespace SimpleSnake
{
	class Program
	{
		static void Main(string[] args)
		{
			bool useWindowed = false;
			foreach (string arg in args)
			{
				if (arg == "wd")
				{
					useWindowed = true;
				}
			}

			IGraphicsMode graphicsMode = useWindowed ? new SFMLGraphics() : new ConsoleGraphics();

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
		}
	}
}
