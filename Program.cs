using System;

namespace SimpleSnake
{
	class Program
	{
		static void Main(string[] args)
		{
			bool useWinForms = false;
			foreach (string arg in args)
			{
				if (arg == "wf")
				{
					useWinForms = true;
				}
			}

			IGraphicsOutput graphicsOutput = useWinForms ? new WinFormsGraphics() : new ConsoleGraphics();

			MainMenuOption choice;

			do
			{
				choice = graphicsOutput.MainMenu();
				if (choice == MainMenuOption.Play)
				{
					var game = new SnakeGame(Settings.defaultWidth, Settings.defaultHeight, Settings.startingLength, Settings.startingDirection, Settings.defaultDelay, graphicsOutput);
					game.Play();
				}
			} while (choice != MainMenuOption.Quit);
		}
	}
}
