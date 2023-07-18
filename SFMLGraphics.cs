using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace SimpleSnake
{
	class SFMLGraphics : IGraphicsOutput
	{
		private readonly Font font;

		private readonly RenderWindow window;

		/// <summary>
		/// Returns a new instance of SFMLGraphics, which creates a window that it will use for its output.
		/// </summary>
		internal SFMLGraphics()
		{
			window = new RenderWindow(new VideoMode(800, 600), "SimpleSnake");

			font = new Font("../../../fonts/MontserratMedium.ttf");

			window.Clear(Color.Black);
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
			var text = new Text("Hello, world!", font, 50)
			{
				// Set the position of the text
				Position = new SFML.System.Vector2f(100, 100),

				// Set the color of the text
				FillColor = Color.White
			};
			while (window.IsOpen)
			{
				window.DispatchEvents();

				// Draw the text
				window.Draw(text);

				window.Display();
			}
			return MainMenuOption.Quit;

		}
	}
}
