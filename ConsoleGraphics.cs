using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// This class provides the C# Console specific elements required to run the game in a Console window, as defined in the <see cref="IGraphicsMode" /> interface.	
	/// </summary>
	public class ConsoleGraphics : IGraphicsMode
	{
		// Console settings - might make some of these user configurable.		
		private static readonly char wallChar = '█';
		// emptyCellChar being a space is assumed by the Draw method - initial draw is performed only on cells which differ from empty space, so just chaniging the char here will only effect cells which get changed to Empty during a game.
		// Easiest way to fix this would be to have InitBoard actually draw out the board in empty cells as well as populate the prevCells array.  But this is skipped since the space char is currently used.
		private static readonly char emptyCellChar = ' ';
		private static readonly char snakeSegmentChar = '█';
		private static readonly char pillChar = '@';

		/// <summary>
		/// Holds the cursor position that the board is drawn relative to.  Value set by <see cref="InitBoard" />.
		/// </summary>
		private (int x, int y) gameOriginCursor;

		/// <summary>
		/// This field is a buffer used by the <see cref="GetPlayerActions" /> method, declared as a class field and reused to avoid frequent allocations.
		/// </summary>
		private readonly List<PlayerAction> actionsBuffer = new(5);

		/// <summary>
		/// prevCells is used to store a copy of the cells array from the previous loop iteration.  The Draw method uses this to calculate which cells have changed and need to be redrawn.  The Draw method makes the copy once it has finished updating the console.
		/// </summary>
		private Cell[,] prevCells;

		public ConsoleGraphics()
		{
			Console.CursorVisible = false;

			// Put an empty array in prevCells to avoid it being null.
			prevCells = new Cell[0, 0];

			// Set up colours for text.
			Console.BackgroundColor = Settings.backgroundColour.console;
			Console.ForegroundColor = Settings.textColour.console;
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
			CellType.GrowPill => pillChar,

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
			CellType.Wall or CellType.Empty => Settings.sceneryColour.console,
			CellType.SnakeSegment => Settings.snakeColour.console,
			CellType.GrowPill => Settings.pillColour.console,

			_ => throw new NotImplementedException($"No case found for {cellType}")
		};

		/// <summary>
		/// Draws the board to the Console.
		/// </summary>
		/// <param name="cells">The cells array representation of the game board.</param>
		/// <param name="gameResults">The GameResult object for the game in progress.</param>
		public void DrawBoard(Cell[,] cells, GameResults gameResults)
		{
			// Output the score (changes every tick)
			// Since score never goes down it will never occupy less space, so we don't worry about clearing the previous score and just overwrite.
			Console.ForegroundColor = Settings.textColour.console;
			Console.SetCursorPosition(gameOriginCursor.x, gameOriginCursor.y);
			Console.Write(TextStrings.scoreTitle);
			Console.ForegroundColor = Settings.scoreColour.console;
			Console.Write(gameResults.Score.ToString("N0"));
			int spaceForScore = 2; // leave a line for the score and a blank line below it.

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
						Console.SetCursorPosition(gameOriginCursor.x + j, gameOriginCursor.y + spaceForScore + i);
						Console.ForegroundColor = GetCellColour(cells[i, j].cellType);
						Console.WriteLine(GetCellChar(cells[i, j].cellType));
					}
				}

			}

			// Store the current cells in prevCells, for use next iteration.
			Array.Copy(cells, prevCells, cells.Length);
		}

		/// <summary>
		/// Returns any PlayerActions that have been created during this iteration.
		/// </summary>		
		/// <returns>The PlayerActions in the order they were created.</returns>		
		// This method populates the <see cref="keypressBuffer">actionsBuffer</see> class field and returns it, but does not care about its starting contents or its contents once it has returned.
		public List<PlayerAction> GetPlayerActions()
		{
			// To avoid allocating it every game loop, we have a buffer declared as a field and reuse it.
			actionsBuffer.Clear();

			// Read all pending keys and if they are valid, place them into the actionsBuffer
			while (Console.KeyAvailable)
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);
				PlayerAction action = keyInfo.Key switch
				{
					var k when k == Settings.upKey.console => PlayerAction.Up,
					var k when k == Settings.downKey.console => PlayerAction.Down,
					var k when k == Settings.leftKey.console => PlayerAction.Left,
					var k when k == Settings.rightKey.console => PlayerAction.Right,
					var k when k == Settings.pauseKey.console => PlayerAction.Pause,
					var k when k == Settings.quitKey.console => PlayerAction.Quit,
					_ => PlayerAction.None
				};

				if (action != PlayerAction.None)
				{
					actionsBuffer.Add(action);
				}
			}

			return actionsBuffer;
		}

		/// <summary>
		/// Prepares the console ready for the board to be drawn for the first time.
		/// </summary>
		/// <param name="width">The width of the board in cells</param>
		/// <param name="height">The height of the board in cells.</param>
		public void InitBoard(int width, int height, GameResults gameResults)
		{
			// Create a prevCells array populated by Empty cells ready for the first call of the Draw method.
			prevCells = new Cell[width, height];

			// If running on Windows, resize the Console window to fit the game if needed.  Skip on other platforms since it is not supported - user can do it themselves if they want.
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				// Check that the window is large enough, and enlarge it if not.  units are characters.
				// Lets say we want 10 chars extra in each dimension.
				int minWidth = width + 10;
				int minHeight = height + 10;

				if (minWidth < Console.WindowWidth || minHeight < Console.WindowHeight)
				{
					// Calculate new dimensions
					int newWidth = minWidth > Console.WindowWidth ? minWidth : Console.WindowWidth;
					int newHeight = minHeight > Console.WindowHeight ? minHeight : Console.WindowHeight;

					// See if we need to expand the buffer
					if (Console.BufferWidth < newWidth)
					{
						Console.BufferWidth = newWidth;
					}
					if (Console.BufferHeight < newHeight)
					{
						Console.BufferHeight = newHeight;
					}

					// Resize the Console window
					Console.SetWindowSize(newWidth, newHeight);
				}
			}

			// Clear screan and write header text.
			Console.Clear();

			Console.ForegroundColor = Settings.textColour.console;
			Console.WriteLine(TextStrings.GameBoardHeading(Settings.pauseKey.console, Settings.quitKey.console));

			Console.Write(TextStrings.highSoreSessionTitle);
			Console.ForegroundColor = Settings.scoreColour.console;
			Console.Write(gameResults.SessionHighScore.ToString("N0"));
			Console.Write("   ");
			Console.ForegroundColor = Settings.textColour.console;
			Console.Write(TextStrings.highSoreAllTimeTitle);
			Console.ForegroundColor = Settings.scoreColour.console;
			Console.WriteLine(gameResults.AllTimeHighScore.ToString("N0"));

			Console.WriteLine();

			// Store where the cursor is so that the game can be drawn below it (and to the right of it if we want a side menu).
			gameOriginCursor = (x: Console.CursorLeft, y: Console.CursorTop);
		}

		/// 
		/// <summary>
		/// A utility function for displaying a keyboard driven Console menu to the player, and getting a choice from all the options in the passed dictionary.  The dictionary is keyed with an enum, the type of which is passed as a stype parameter.
		/// </summary>
		/// <typeparam name="T">The type of the dictionary key and return value.</typeparam>
		/// <param name="menuOptions">A dictionary containing each of the options to be presented with the enum value of the option as the key, and the value being a string with the text to display.</param>
		/// <returns>The key of the chosen option in the menuOptions parameter.</returns>
		/// <exception cref="ArgumentException">If there are no values in the dictionary.</exception>
		/// <exception cref="NotImplementedException">Dictionaries with more than 9 values are not handled.</exception>
		public T Menu<T>(Dictionary<T, string> menuOptions) where T : notnull
		{
			Console.Clear();

			if (menuOptions.Count < 1)
			{
				throw new ArgumentException($"menuOptions does not have any values.");
			}

			if (menuOptions.Count > 9)
			{
				throw new NotImplementedException($"menuOptions has more than 9 values - this method only supports options from `1` to `9`. It could easily be expanded to offer alphabetic options as well.");
			}

			// Dictionary to store keypress assigned to choose menu option to that option.
			Dictionary<char, T> options = new();

			// Output menu, and populate the options dictionary.
			char optionNum = '1';
			foreach (KeyValuePair<T, string> option in menuOptions)
			{
				options[optionNum] = option.Key;
				Console.WriteLine($"{optionNum}. {option.Value}");
				optionNum++;
			}

			// Loop on ReadKey until a valid option key is entered.
			char playerChoice = '\0';
			while (playerChoice < '1' || playerChoice >= '1' + menuOptions.Count)
			{
				playerChoice = Console.ReadKey(true).KeyChar;
			}

			return options[playerChoice];
		}

		/// <summary>
		/// Pauses the game until a key is pressed.  If that key is the quit game key, return true, otherwise return false.
		/// </summary>
		/// <returns>True if the game should exit, false otherwise.</returns>
		public bool PauseGameWithExitOption()
		{
			ConsoleKeyInfo unpauseKey = Console.ReadKey(true); // Blocks until keypress.

			if (unpauseKey.Key == Settings.quitKey.console)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Clear the console and reset colours ready for text output.
		/// </summary>
		public void PostPlayCleanup()
		{
			Console.BackgroundColor = Settings.backgroundColour.console;
			Console.ForegroundColor = Settings.textColour.console;
			Console.Clear();
		}
	}
}
