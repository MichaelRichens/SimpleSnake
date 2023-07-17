using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// Static class which provides text to be displayed to the user.
	/// </summary>
	static class TextStrings
	{
		internal static string ConsoleBoardHeading(ConsoleKey pauseKey, ConsoleKey quitKey) => $"Press {pauseKey} to pause, press {quitKey} to quit.";

		internal static Dictionary<MainMenuOption, string> MainMenu = new() {
			{ MainMenuOption.Play, "Start Game" },
			{ MainMenuOption.Quit, "Exit Game" },
		};
	}
}
