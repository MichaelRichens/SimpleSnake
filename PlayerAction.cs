using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// The PlayerAction enum is used to represent an instruction from the player during the game, such as quitting the game or changing the direction of movement.
	/// The default value is `None`.
	/// </summary>
	enum PlayerAction
	{
		None,
		Quit,
		Pause,
		Left,
		Right,
		Up,
		Down,
	}
}
