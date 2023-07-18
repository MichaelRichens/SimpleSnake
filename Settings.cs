using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace SimpleSnake
{
	internal static class Settings
	{
		// These keybindings store the default keys in the enum type needed in the different environments the game runs in.
		// These don't need to be the same accross environments, and if redefining is added, we only need to change the environment we are actually running in.
		internal static KeyBinding upKey = new(ConsoleKey.UpArrow, Keyboard.Key.Up);
		internal static KeyBinding downKey = new(ConsoleKey.DownArrow, Keyboard.Key.Down);
		internal static KeyBinding leftKey = new(ConsoleKey.LeftArrow, Keyboard.Key.Left);
		internal static KeyBinding rightKey = new(ConsoleKey.RightArrow, Keyboard.Key.Right);
		internal static KeyBinding pauseKey = new(ConsoleKey.P, Keyboard.Key.P);
		internal static KeyBinding quitKey = new(ConsoleKey.Escape, Keyboard.Key.Escape);

		// Similar for colours
		internal static readonly Colour backgroundColour = new(ConsoleColor.Black, Color.Black);
		internal static readonly Colour pillColour = new(ConsoleColor.Yellow, Color.Yellow);
		internal static readonly Colour sceneryColour = new(ConsoleColor.White, Color.White);
		internal static readonly Colour snakeColour = new(ConsoleColor.Blue, Color.Blue);
		internal static readonly Colour textColour = new(ConsoleColor.White, Color.White);

		internal static int defaultWidth = 70;
		internal static int defaultHeight = 50;

		internal static int defaultDelay = 70;

		internal static readonly Direction startingDirection = Direction.Right;
		internal static readonly int startingLength = 20;

		internal static readonly int pillGrowAmount = 10;
	}
}
