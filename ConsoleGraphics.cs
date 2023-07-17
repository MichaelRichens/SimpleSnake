using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	public class ConsoleGraphics : IGraphicsOutput
	{
		// Console settings - might make some of these user configurable.
		private static readonly char emptyCellChar = ' ';
		private static readonly char wallChar = '█';
		private static readonly char snakeSegmentChar = '█';
		private static readonly ConsoleColor backgroundColour = ConsoleColor.Black;
		private static readonly ConsoleColor sceneryColour = ConsoleColor.White;
		private static readonly ConsoleColor snakeColour = ConsoleColor.Green;

		/// <summary>
		/// Holds the console background colour set when the instance was created.
		/// </summary>
		private readonly ConsoleColor initialBackgroundColour;

		/// <summary>
		/// Holds the console foreground colour set when the instance was created.
		/// </summary>
		private readonly ConsoleColor initialForegroundColour;

		/// <summary>
		/// prevCells is used to store a copy of the cells array from the previous loop iteration.  The Draw method uses this to calculate which cells have changed and need to be redrawn.  The Draw method makes the copy once it has finished updating the console.
		/// </summary>
		private Cell[,] prevCells;

		public ConsoleGraphics(int initialWidth, int initialHeight)
		{
			Console.CursorVisible = false;

			// Save user's settings so we can revert back to them after drawing graphics for menu/exit game text.
			initialBackgroundColour = Console.BackgroundColor;
			initialForegroundColour = Console.ForegroundColor;

			prevCells = new Cell[initialWidth, initialHeight];

			Console.BackgroundColor = backgroundColour;
		}

		/// <summary>
		/// Gets the character to be used for drawing a cell based on its CellType.
		/// </summary>
		/// <param name="cellType">The CellType value to query.</param>
		/// <returns>The character to use.</returns>
		/// <exception cref="NotImplementedException">On unknown CellType.</exception>
		private static char GetCellChar(CellType cellType) => cellType switch
		{
			CellType.Empty => emptyCellChar,
			CellType.Wall => wallChar,
			CellType.SnakeSegment => snakeSegmentChar,

			_ => throw new NotImplementedException($"No case found for {cellType}")
		};

		/// <summary>
		/// Gets the foreground colour to be used when drawing a cell based on its CellType.
		/// </summary>
		/// <param name="cellType">The CellType value to query.</param>
		/// <returns>The foregound colour to use.</returns>
		/// <exception cref="NotImplementedException">On unknown CellType.</exception>
		private static ConsoleColor GetCellColour(CellType cellType) => cellType switch
		{
			CellType.Wall or CellType.Empty => sceneryColour,
			CellType.SnakeSegment => snakeColour,

			_ => throw new NotImplementedException($"No case found for {cellType}")
		};


		/// <summary>
		/// Draws the board to the Console.
		/// </summary>
		public void DrawBoard(Cell[,] cells)
		{
			int height = cells.GetLength(0);
			int width = cells.GetLength(1);

			// Check that our prevCells is of the valid dimensions, and generate a new empty version (force a full redraw) if not.
			// Stops resizing the game board mid game breaking this method.
			if (prevCells.GetLength(0) != height || prevCells.GetLength(1) != width)
			{
				prevCells = new Cell[height, width];
			}

			// Check cells and against prevCells for changes, and update the Console with the data from cells for any indexes that have changed.
			// - The Console.Clear() method is far too slow to run every loop, so can't just redraw the whole board each iteration.
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (cells[i, j].cellType != prevCells[i, j].cellType)
					{
						Console.SetCursorPosition(j, i);
						Console.ForegroundColor = GetCellColour(cells[i, j].cellType);
						Console.WriteLine(GetCellChar(cells[i, j].cellType));
					}
				}

			}

			// Store the current cells in prevCells, for use next iteration.
			Array.Copy(cells, prevCells, cells.Length);

			// Leave the console colours in their default state, with the cursor at the end of the output (so if we exit and display some text it appears correctly).
			Console.BackgroundColor = initialBackgroundColour;
			Console.ForegroundColor = initialForegroundColour;
			Console.SetCursorPosition(0, height - 1);
		}

		/// <summary>
		/// Clears the console reasy for the board to be drawn for the first time.
		/// </summary>
		public void InitBoard()
		{
			Console.Clear();
		}

		/// <summary>
		/// Presents the main game menu to the player, and returns their choice.
		/// </summary>
		/// <returns>The choice made by the player.</returns>
		public MainMenuOption MainMenu()
		{
			Console.Clear();
			return DisplayMenuFromEnum<MainMenuOption>(TextStrings.MainMenu);
		}

		/// <summary>
		/// A utility function for displaying a Console menu to the player, getting a choice from all the options in the enum type parameter that is passed.  Text for the options is passed in as a dictionary.
		/// </summary>
		/// <typeparam name="T">The enum representing the options to be presented to the player.</typeparam>
		/// <param name="textLookup">A dictionary containing each of these options as a key, with the value being a string with the text to display.</param>
		/// <returns>The chosen option - a value of the enum type passed in.</returns>
		/// <exception cref="ArgumentException">If there are no values in the enum, or if the number of values in the enum does not match the size of the textLookup dictionary.</exception>
		/// <exception cref="NotImplementedException">Enums with more than 9 values are not handled.</exception>
		private static T DisplayMenuFromEnum<T>(Dictionary<T, string> textLookup) where T : Enum
		{
			T[] values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

			if (values.Length < 1)
			{
				throw new ArgumentException($"Enum {typeof(T).Name} does not have any values.");
			}

			if (values.Length > 9)
			{
				throw new NotImplementedException($"Enum {typeof(T).Name} has more than 9 values - this method only supports options from `1` to `9`. It could easily be expanded to offer alphabetic options as well.");
			}

			if (values.Length != textLookup.Count)
			{
				throw new ArgumentException($"The textLookup dictionary does not contain the same number of entries as there are values in the {typeof(T).Name} enum");
			}

			Dictionary<char, T> options = new();
			char playerChoice = '\0';

			while (playerChoice < '1' || playerChoice >= '1' + values.Length)
			{
				char optionNum = '1';
				foreach (T option in values)
				{
					options[optionNum] = option;
					Console.WriteLine($"{optionNum}. {textLookup[option]}");
					optionNum++;
				}
				playerChoice = Console.ReadKey().KeyChar;
			}

			return options[playerChoice];
		}


	}
}
