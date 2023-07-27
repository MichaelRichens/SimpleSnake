using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace SimpleSnake
{
	/// <summary>
	/// Static class which provides text to be displayed to the user.
	/// </summary>
	static class TextStrings
	{
		/// <summary>
		/// Provides some heading text to be displayed above the game when it is playing.
		/// </summary>
		/// <param name="pauseKey">The currently set pause key.</param>
		/// <param name="quitKey">The currently set quit game key.</param>
		/// <returns>The string to display.</returns>
		internal static string GameBoardHeading(ConsoleKey pauseKey, ConsoleKey quitKey) => $"Press {pauseKey} to pause, press {quitKey} to quit.";
		internal static string GameBoardHeading(Keyboard.Key pauseKey, Keyboard.Key quitKey) => $"Press {pauseKey} to pause, press {quitKey} to quit.";

		/// <summary>
		/// The options to display in the main game menu.  
		/// </summary>
		internal static Dictionary<MainMenuOption, string> MainMenu = new() {
			{ MainMenuOption.Play, "Start Game" },
			{ MainMenuOption.Options, "Options" },
			{ MainMenuOption.Quit, "Exit Game" },
		};

		/// <summary>
		/// The options to display in the options menu.
		/// </summary>
		internal static Dictionary<OptionsMenuOption, string> OptionsMenu = new()
		{
			{ OptionsMenuOption.ChangeBoardSize, "Adjust Map Size" },
			{ OptionsMenuOption.Back, "Back" },
		};

		internal static string scoreTitle = "Score: ";
		internal static string highSoreSessionTitle = "High Score (Session): ";
		internal static string highSoreAllTimeTitle = "High Score (All Time): ";
	}
}
