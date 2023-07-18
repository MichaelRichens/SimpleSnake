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

			IGraphicsOutput graphicsOutput = useWindowed ? new SFMLGraphics() : new ConsoleGraphics();

			MainMenuOption choice;

			do
			{
				choice = graphicsOutput.MenuFromEnum<MainMenuOption>(TextStrings.MainMenu);
				if (choice == MainMenuOption.Play)
				{
					var game = new SnakeGame(Settings.defaultWidth, Settings.defaultHeight, Settings.startingLength, Settings.startingDirection, Settings.defaultDelay, graphicsOutput);
					game.Play();
					graphicsOutput.PostPlayCleanup();
				}
			} while (choice != MainMenuOption.Quit);
		}
	}
}
