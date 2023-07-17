using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	internal static class Settings
	{
		internal static ConsoleKey upKey = ConsoleKey.UpArrow;
		internal static ConsoleKey downKey = ConsoleKey.DownArrow;
		internal static ConsoleKey leftKey = ConsoleKey.LeftArrow;
		internal static ConsoleKey rightKey = ConsoleKey.RightArrow;

		internal static readonly ConsoleKey pauseKey = ConsoleKey.P;
		internal static readonly ConsoleKey quitKey = ConsoleKey.Escape;

		internal static int defaultWidth = 100;
		internal static int defaultHeight = 30;

		internal static int defaultDelay = 100;

		internal static readonly Direction startingDirection = Direction.Right;
		internal static readonly int startingLength = 20;

	}
}
