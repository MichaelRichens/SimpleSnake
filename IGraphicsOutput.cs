using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// IGraphicsOutput controls iteraction with the graphics display, and is implemented by classes that are injected into the application to run it in different graphical environments
	/// </summary>
	internal interface IGraphicsOutput
	{
		/// <summary>
		/// The DrawBoard method is passed a 2D cells array, and displays it to the user.
		/// </summary>
		/// <param name="cells"></param>
		internal void DrawBoard(Cell[,] cells);

		/// <summary>
		/// Does any start of game housekeeping needed before the board is first drawn (eg clearing the screen).
		/// </summary>
		internal void InitBoard();

		/// <summary>
		/// Presents the main game menu to the player, and returns their choice.
		/// </summary>
		/// <returns>The choice made by the player.</returns>
		internal MainMenuOption MainMenu();
	}
}
