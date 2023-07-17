using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// The Cell struct is an immutable struct representing the contents of an individual location of a <see cref="Board" /> Board instance.  It holds an enum for the type of cell (Empty, Wall, SnakeSegment, etc.) it is, and if relevant a timer containing how many game update ticks it will remain there for before reverting to being Empty.
	/// An Empty or Wall type of cell has no timer, all other types have a timer and will revert to Empty
	/// </summary>
	public readonly struct Cell
	{
		/// <summary>
		/// The contents of the cell.
		/// </summary>
		internal readonly CellType cellType;

		/// <summary>
		/// If zero, it indicates the cell is permanent.  If positive, it is the number of game ticks before the cell reverts to being empty.
		/// </summary>
		internal readonly int timer;

		/// <summary>
		/// Creates a new empty cell.
		/// </summary>
		public Cell()
		{
			cellType = CellType.Empty;
			timer = 0;
		}

		/// <summary>
		/// Creates a new cell with a 0 timer of the specified type.
		/// </summary>
		/// <param name="cellType">The CellType for the Cell to create.</param>
		internal Cell(CellType cellType)
		{
			this.cellType = cellType;
			timer = 0;
		}

		/// <summary>
		/// Cretaes a new Cell with the passed type and timer
		/// </summary>
		/// <param name="cellType">The CellType of the new cell.</param>
		/// <param name="timer">The starting value of the timer.</param>
		internal Cell(CellType cellType, int timer)
		{
			this.cellType = cellType;
			this.timer = timer;
		}

		/// <summary>
		/// Method that creates a new cell from the existing one by decreasing its timer by 1, and changing it to an Empty type if the timer was previously equal to 1.  cells with timer == 0 (Empty, Walls) return themselves.
		/// </summary>
		/// <returns>The new Cell struct.</returns>
		internal Cell DecTimer()
		{
			if (timer <= 0)
			{
				// Permanent cell type, return unchanged.
				return this;
			}
			if (timer == 1)
			{
				// Expired cell type, return an empty cell.
				return new Cell();
			}

			// Otherwise return a cell with the same type and timer decreased.
			return new Cell(cellType, timer - 1);
		}

	}
}
