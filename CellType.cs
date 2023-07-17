using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// The CellType enum represents the different contents a <see cref="Cell" /> can have.
	/// </summary>
	internal enum CellType
	{
		Empty,
		Wall,
		SnakeSegment,
		/// <summary>
		/// A pill which grows the snake by a fixed amount and remains in place until collected.
		/// </summary>
		FixedGrowPillPermanent,
		/// <summary>
		/// A pill which grows the sname by a fixed amount and remains in place for a duration before dissapearing.
		/// </summary>
		FixedGrowPillTimed,
		/// <summary>
		/// A pill which doubles the snake's and remains in place for a duration before dissapearing.
		/// </summary>
		DoubleGrowPillPermanent,
		/// <summary>
		/// A pill which doubles the snake's and remains in place until collected.
		/// </summary>
		DoubleGrowPillTimed,
	}
}
