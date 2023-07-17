using System;

namespace SimpleSnake
{
	class Program
	{
		static void Main()
		{
			int delay = Settings.defaultDelay;
			int width = Settings.defaultWidth;
			int height = Settings.defaultHeight;
			int startingLength = Settings.startingLength;
			Direction startingDirection = Settings.startingDirection;

			IGraphicsOutput graphicsOutput = new ConsoleGraphics(width, height);

			MainMenuOption choice;

			do
			{
				choice = graphicsOutput.MainMenu();
				if (choice == MainMenuOption.Play)
				{
					var game = new SnakeGame(width, height, startingLength, startingDirection, delay, graphicsOutput);
					game.Play();
				}
			} while (choice != MainMenuOption.Quit);
		}
	}
}
