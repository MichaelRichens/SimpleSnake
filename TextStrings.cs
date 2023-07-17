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
		/// <summary>
		/// Provides some heading text to be displayed above the game when playing in the Console.
		/// </summary>
		/// <param name="pauseKey">The currently set pause key.</param>
		/// <param name="quitKey">The currently set quit game key.</param>
		/// <returns>The string to display.</returns>
		internal static string ConsoleBoardHeading(ConsoleKey pauseKey, ConsoleKey quitKey) => $"Press {pauseKey} to pause, press {quitKey} to quit.";

		/// <summary>
		/// A lookup to provide the text to display for each memeber of the MainMenuOption enum.
		/// </summary>
		internal static Dictionary<MainMenuOption, string> MainMenu = new() {
			{ MainMenuOption.Play, "Start Game" },
			{ MainMenuOption.Quit, "Exit Game" },
		};
	}
}
