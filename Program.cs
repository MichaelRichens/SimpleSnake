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

			GameSession gameSession = new(graphicsMode);

			gameSession.MainMenu();
		}
	}
}
