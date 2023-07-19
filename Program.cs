using System;

namespace SimpleSnake
{
	class Program
	{
		static void Main(string[] args)
		{
			bool useConsole = false;
			foreach (string arg in args)
			{
				if (arg == "csl")
				{
					useConsole = true;
				}
			}

			IGraphicsMode graphicsMode = useConsole ? new ConsoleGraphics() : new SFMLGraphics();

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
