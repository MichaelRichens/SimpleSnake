using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// IGraphicsOutput controls iteraction with the graphics display, and is implemented by classes that are provided to the application to run it in the selected graphical environments
	/// </summary>
	public interface IGraphicsOutput
	{
		/// <summary>
		/// The DrawBoard method is passed a 2D cells array, and displays it to the user.
		/// </summary>
		/// <param name="cells"></param>
		public void DrawBoard(Cell[,] cells);

		/// <summary>
		/// Does any start of game housekeeping needed before the board is first drawn (eg clearing the screen).
		/// </summary>
		/// <param name="width">The width of the board in cells</param>
		/// <param name="height">The height of the board in cells.</param>
		public void InitBoard(int width, int height);

		/// <summary>
		/// Method for displaying a menu to the player, and getting a choice from all the options in the enum type parameter that is passed.  Display text for the options is passed in as a dictionary.
		/// </summary>
		/// <typeparam name="TEnum">The enum representing the options to be presented to the player.</typeparam>
		/// <param name="enumTextLookup">A dictionary containing each of these options as a key, with the value being a string with the text to display.</param>
		/// <returns>The chosen option - a value of the enum type passed in.</returns>
		/// <exception cref="ArgumentException">If there are no values in the enum, or if the number of values in the enum does not match the size of the enumTextLookup dictionary.</exception>
		public TEnum MenuFromEnum<TEnum>(Dictionary<TEnum, string> enumTextLookup) where TEnum : struct, Enum;

		/// <summary>
		/// Called immediately after a game is over to revert any changes made to the graphics system for it.
		/// </summary>
		public void PostPlayCleanup();
	}
}
