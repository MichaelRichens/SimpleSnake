using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// Immutable struct which holds a CellType and an index, used to record which Sprite is being used for a particular cell.
	/// </summary>
	internal readonly struct CellSpriteIndex
	{
		/// <summary>
		/// The CellType of the cell
		/// </summary>
		public readonly CellType cellType;

		/// <summary>
		/// The index of the sprite chosen.  -1 indicates unset and is the default value.
		/// </summary>
		public readonly int index;

		public CellSpriteIndex()
		{
			cellType = CellType.Empty;
			index = -1;
		}

		public CellSpriteIndex(CellType cellType, int index)
		{
			this.cellType = cellType;
			this.index = index;
		}
	}
}
