using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	class WinFormsGraphics : IGraphicsOutput
	{
		internal WinFormsGraphics()
		{

		}

		/// <summary>
		/// The DrawBoard method is passed a 2D cells array, and displays it to the user.
		/// </summary>
		/// <param name="cells"></param>
		public void DrawBoard(Cell[,] cells)
		{
			throw new NotImplementedException("DrawBoard");
		}

		/// <summary>
		/// Does any start of game housekeeping needed before the board is first drawn (eg clearing the screen).
		/// </summary>
		/// <param name="width">The width of the board in cells</param>
		/// <param name="height">The height of the board in cells.</param>
		public void InitBoard(int width, int height)
		{
			throw new NotImplementedException("InitBoard");
		}

		/// <summary>
		/// Presents the main game menu to the player, and returns their choice.
		/// </summary>
		/// <returns>The choice made by the player.</returns>
		public MainMenuOption MainMenu()
		{
			throw new NotImplementedException("MainMenu");
		}
	}
}
