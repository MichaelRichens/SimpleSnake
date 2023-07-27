using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// IGraphicsMode controls iteraction with the graphics display, and is implemented by classes that are provided to the application to run it in the selected graphical environments
	/// </summary>
	public interface IGraphicsMode
	{
		/// <summary>
		/// The DrawBoard method is passed a 2D cells array, and displays it to the user.
		/// </summary>
		/// <param name="cells">The cells array representation of the game board.</param>
		/// <param name="gameResults">The GameResult object for the game in progress.</param>
		public void DrawBoard(Cell[,] cells, GameResults gameResults);

		/// <summary>
		/// Does any start of game housekeeping needed before the board is first drawn (eg clearing the screen).
		/// </summary>
		/// <param name="width">The width of the board in cells</param>
		/// <param name="height">The height of the board in cells.</param>
		public void InitBoard(int width, int height, GameResults gameResults);

		/// <summary>
		/// Method for displaying a menu to the player, and getting a choice from all the options in the passed dictionary.  The dictionary is keyed with a generic type, and the value is the text to display to the player.
		/// </summary>
		/// <typeparam name="T">The type of the dictionary key and return value.</typeparam>
		/// <param name="menuOptions">A dictionary containing each of the options to be presented with the enum value of the option as the key, and the value being a string with the text to display.</param>
		/// <returns>The key of the chosen option in the menuOptions parameter.</returns>
		/// <exception cref="ArgumentException">If there are no values in the dictionary.</exception>
		public T Menu<T>(Dictionary<T, string> menuOptions) where T : notnull;

		/// <summary>
		/// Pauses the game until suitable unpause action is made.  Returns a boolean to say whether the unpasue should also be treated as a game exit.
		/// </summary>
		/// <returns>True if the game should exit, false otherwise.</returns>
		public bool PauseGameWithExitOption();

		/// <summary>
		/// Called immediately after a game is over to revert any changes made to the graphics system for it.
		/// </summary>
		public void PostPlayCleanup();

		/// <summary>
		/// Returns any PlayerActions that have been created during this iteration.
		/// </summary>		
		/// <returns>The PlayerActions in the order they were created.</returns>		
		public List<PlayerAction> GetPlayerActions();
	}
}
