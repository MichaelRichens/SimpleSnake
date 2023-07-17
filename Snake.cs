using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// The Snake class represents the snake itself and holds information about it. Specifically about its head - the rest of its body is considered part of the board and is handled by the game's <see cref="Board" /> instance.
	/// </summary>
	class Snake
	{
		/// <summary>
		/// The length of the snake - this is equal to both the length in cells it occuipes, and the number of game update ticks it takes for each segment to expire.
		/// </summary>
		internal int length;

		/// <summary>
		/// The direction the snake is heading in.
		/// </summary>
		internal Direction direction;

		/// <summary>
		/// The horizontal position of the snake, 0 is left.
		/// </summary>
		internal int HeadX { get; private set; }

		/// <summary>
		/// The vertical position of the snake's head, 0 is the top.
		/// </summary>
		internal int HeadY { get; private set; }

		internal Snake(int length, Direction direction, int x, int y)
		{
			this.length = length;
			this.direction = direction;
			HeadX = x;
			HeadY = y;
		}

		/// <summary>
		/// Updates the <see cref="HeadX" /> and <see cref="HeadY" /> properties to represent a move of 1 cell in the direction specified by <see cref="direction" />.
		/// </summary>		
		internal void AdvanceHead()
		{
			// Should never be out of bounds, since the walls at the edge of the map will kill the snake the move previous.  So not passing in bounds data to error check it here.
			switch (direction)
			{
				case Direction.Up:
					HeadY--;
					break;
				case Direction.Down:
					HeadY++;
					break;
				case Direction.Left:
					HeadX--;
					break;
				case Direction.Right:
					HeadX++;
					break;
				default:
					throw new NotImplementedException($"Unknown Direction: {direction}");
			}
		}
	}
}

