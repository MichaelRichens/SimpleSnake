using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SimpleSnake
{
	/// <summary>
	/// This class provides the SFML specific elements required to run the game in an SFML window, as defined in the <see cref="IGraphicsMode" /> interface.	
	/// </summary>
	public class SFMLGraphics : IGraphicsMode
	{
		/// <summary>
		/// The integer value of Keyboard.Key.Num0
		/// </summary>
		private const int SfmlNum0 = 26;

		/// <summary>
		/// The integer value of Keyboard.Key.Numpad0
		/// </summary>
		private const int SfmlNumPad0 = 75;

		/// <summary>
		/// The size of each cell in pixels.
		/// </summary>
		private static readonly int cellSize = 10;

		/// <summary>
		/// Set to whatever the height and width of the sprites is.
		/// </summary>
		private static readonly int nativeSpriteSize = 20;

		/// <summary>
		/// This field is a buffer used by the <see cref="GetPlayerActions" /> method, declared as a class field and reused to avoid frequent allocations.
		/// </summary>
		private readonly List<PlayerAction> actionsBuffer = new(5);

		/// <summary>
		/// The font used for text output.
		/// </summary>
		private readonly Font font;

		/// <summary>
		/// The title text to appear above the game board.
		/// </summary>
		private Text gameBoardHeading;

		/// <summary>
		/// The sprites available for each CellType.
		/// </summary>
		private readonly Dictionary<CellType, List<Sprite>> sprites = new();

		/// <summary>
		/// The game window renderer.
		/// </summary>
		private readonly RenderWindow window;

		/// <summary>
		/// Returns a new instance of SFMLGraphics, which creates a window that it will use for its output.
		/// </summary>
		public SFMLGraphics()
		{
			window = new RenderWindow(new VideoMode(800, 600), "SimpleSnake");

			font = new Font("../../../fonts/MontserratMedium.ttf");

			LoadSprites();

			gameBoardHeading = new Text(TextStrings.GameBoardHeading(Settings.pauseKey.sfml, Settings.quitKey.sfml), font, 20);

			window.Clear(Settings.backgroundColour.sfml);

			// Since this window runs the whole application, closing it should close the app.
			// There will be no resources that need cleanup, so attaching a handler that does an immediate program termination is a convenient way to provide this functionality.
			// This handler can be called anytime window.DispatchEvents() runs.
			window.Closed += (sender, e) => Environment.Exit(0);
		}

		/// <summary>
		/// Gets the foreground colour to be used when drawing a cell based on its CellType.
		/// </summary>
		/// <param name="cellType">The CellType value to query.</param>
		/// <returns>The foregound colour to use.</returns>
		/// <exception cref="NotImplementedException">On unknown CellType.</exception>
		private static Color GetCellColour(CellType cellType) => cellType switch
		{
			CellType.Wall or CellType.Empty => Settings.sceneryColour.sfml,
			CellType.SnakeSegment => Settings.snakeColour.sfml,
			CellType.GrowPill => Settings.pillColour.sfml,

			_ => throw new NotImplementedException($"No case found for {cellType}")
		};

		/// <summary>
		/// The DrawBoard method is passed a 2D cells array, and displays it to the user.
		/// </summary>
		/// <param name="cells"></param>
		public void DrawBoard(Cell[,] cells)
		{
			window.Clear(Settings.backgroundColour.sfml);

			window.Draw(gameBoardHeading);

			float boardTop = gameBoardHeading.GetGlobalBounds().Top + gameBoardHeading.GetGlobalBounds().Height + 10;

			float boardLeft = 20;

			for (int i = 0; i < cells.GetLength(0); i++)
			{
				for (int j = 0; j < cells.GetLength(1); j++)
				{
					Sprite sprite = sprites[cells[i, j].cellType][0];

					sprite.Scale = new Vector2f(cellSize / (float)nativeSpriteSize, cellSize / (float)nativeSpriteSize);

					sprite.Color = GetCellColour(cells[i, j].cellType);

					sprite.Position = new Vector2f(j * cellSize + boardLeft, i * cellSize + boardTop);

					window.Draw(sprite);
				}
			}

			window.Display();
		}

		/// <summary>
		/// Returns any PlayerActions that have been created during this iteration.
		/// </summary>		
		/// <returns>The PlayerActions in the order they were created.</returns>	
		// This method populates the <see cref="keypressBuffer">actionsBuffer</see> class field and returns it, but does not care about its starting contents or its contents once it has returned.
		public List<PlayerAction> GetPlayerActions()
		{
			// Clear out actions from last iteration
			actionsBuffer.Clear();

			// Run the handlers - populate actionsBuffer from any keypresses placed this iteration
			window.DispatchEvents();

			// And return them.
			return actionsBuffer;
		}

		/// <summary>
		/// Does any start of game housekeeping needed before the board is first drawn (eg clearing the screen).
		/// </summary>
		/// <param name="width">The width of the board in cells</param>
		/// <param name="height">The height of the board in cells.</param>
		public void InitBoard(int width, int height)
		{
			// check window is large enough to fit game board, and expand it if not
			// Let's say we want 200px in each dimension larger than it takes to display the board.
			Vector2u oldWinSize = window.Size;
			uint minX = (uint)width * (uint)cellSize + 200;
			uint minY = (uint)height * (uint)cellSize + 200;
			if (minX > oldWinSize.X || minY > oldWinSize.Y)
			{
				Vector2u newWinSize = new(minX > oldWinSize.X ? minX : oldWinSize.X, minY > oldWinSize.Y ? minY : oldWinSize.Y);
				window.Size = newWinSize;
			}

			// Add handlers
			window.KeyPressed += InGameKeypressHandler;
		}

		/// <summary>
		/// A utility function for displaying a menu to the player inside the instance's window, and getting a choice from all the options in the enum type parameter that is passed.  Either by keypress or mouse click.
		/// Text for the options is passed in as a dictionary.
		/// </summary>
		/// <typeparam name="TEnum">The enum representing the options to be presented to the player.</typeparam>
		/// <param name="enumTextLookup">A dictionary containing each of these options as a key, with the value being a string with the text to display.</param>
		/// <returns>The chosen option - a value of the enum type passed in.</returns>
		/// <exception cref="ArgumentException">If there are no values in the enum, or if the number of values in the enum does not match the size of the enumTextLookup dictionary.</exception>
		/// <exception cref="NotImplementedException">Enums with more than 9 values are not handled.</exception>
		public TEnum MenuFromEnum<TEnum>(Dictionary<TEnum, string> enumTextLookup) where TEnum : struct, Enum
		{
			TEnum[] values = Enum.GetValues<TEnum>().ToArray();

			if (values.Length < 1)
			{
				throw new ArgumentException($"Enum {typeof(TEnum).Name} does not have any values.");
			}

			if (values.Length > 9)
			{
				throw new NotImplementedException($"Enum {typeof(TEnum).Name} has more than 9 values - this method only supports options from `1` to `9`. It could easily be expanded to offer alphabetic options as well.");
			}

			if (values.Length != enumTextLookup.Count)
			{
				throw new ArgumentException($"The enumTextLookup dictionary does not contain the same number of entries as there are values in the {typeof(TEnum).Name} enum");
			}

			// The number of pixels that menu item Text elements are positioned from the left.
			int textPosLeft = 100;
			// The number of pixels that menu items are positioned from each other (and for the first element, the nuimber of pixels from the top)
			int textPosTop = 50;
			// The size of the text characters
			uint textSize = 30;

			// Dictionary to store keypress assigned to choose menu option to the number that will be displayed for that option.
			Dictionary<int, TEnum> options = new();

			// Create an array of Text objects to display the options
			var optionText = new List<Text>();
			int optionNum = 1;
			foreach (TEnum option in values)
			{
				// Create the text object
				var text = new Text($"{(char)(optionNum + '0')}. {enumTextLookup[option]}", font, textSize)
				{
					// Set the position of the text
					Position = new Vector2f(textPosLeft, textPosTop * optionNum),

					// Set the color of the text
					FillColor = Settings.textColour.sfml
				};
				optionText.Add(text);

				// populate the options dictionary at the same time
				options[optionNum] = option;

				optionNum++;
			}

			// This will hold the integer value of the number pressed on the keyboard by the player to select an option.
			// It is also the control variable for the window loop - once it is chaged from -1 the loop exits.
			int playerChoice = -1;

			// This is the keypress handler for the window
			// Sets playerChoice to the relevant menu item value based on the key entered
			void keyPressHandler(object? sender, KeyEventArgs e)
			{
				// Dealing with converting KeyEventArgs.Code enum back to character is annoying since there doesn't appear to be a conversion function.
				// And since we are using numbers, it could be either the main number keys or the num pad keys that are used.
				// We'll check for both (using the offset from the integer value of the two '0' keys), and if either of them works out at a valid option number, let that option be selected.
				int maybeNum = (int)e.Code - SfmlNum0;
				int maybeNumPad = (int)e.Code - SfmlNumPad0;

				if (maybeNum >= 1 && maybeNum <= options.Count)
				{
					playerChoice = maybeNum;
				}

				if (maybeNumPad >= 1 && maybeNumPad <= options.Count)
				{
					playerChoice = maybeNumPad;
				}
			}

			// This is the mouse click handler for the window
			// Sets playerChoice to the relevant menu item value if one is clicked
			void mouseClickHandler(object? sender, MouseButtonEventArgs e)
			{
				int optionNum = 1;
				foreach (Text text in optionText)
				{
					FloatRect bounds = text.GetGlobalBounds();
					if (e.X >= bounds.Left && e.X <= bounds.Left + bounds.Width && e.Y >= bounds.Top && e.Y <= bounds.Top + bounds.Height)
					{
						playerChoice = optionNum;
						return;
					}

					optionNum++;
				}
			}

			// Cursor objects for the mouseMoveEventHandler
			Cursor handCursor = new(Cursor.CursorType.Hand);
			Cursor arrowCursor = new(Cursor.CursorType.Arrow);

			// This is the mouse move handler for the window.
			// It changes the cursor between arrow and hand depending on whether it is over a menu item or not.
			void mouseMoveEventHandler(object? sender, MouseMoveEventArgs e)
			{
				int optionNum = 1;
				foreach (Text text in optionText)
				{
					FloatRect bounds = text.GetGlobalBounds();
					if (e.X >= bounds.Left && e.X <= bounds.Left + bounds.Width && e.Y >= bounds.Top && e.Y <= bounds.Top + bounds.Height)
					{
						window.SetMouseCursor(handCursor);
						return;
					}

					optionNum++;
				}
				window.SetMouseCursor(arrowCursor);
			}


			// Add event handlers to window
			window.KeyPressed += keyPressHandler;
			window.MouseButtonPressed += mouseClickHandler;
			window.MouseMoved += mouseMoveEventHandler;

			// Window loop
			while (playerChoice == -1)
			{
				window.Clear(Settings.backgroundColour.sfml);

				foreach (Text text in optionText)
				{
					window.Draw(text);
				}

				window.Display();

				window.DispatchEvents();
			}

			// Remove event handlers from window
			window.KeyPressed -= keyPressHandler;
			window.MouseButtonPressed -= mouseClickHandler;
			window.MouseMoved -= mouseMoveEventHandler;

			// Return the selected option.
			return options[playerChoice];
		}

		/// <summary>
		/// Removes handlers, and does anything else that needs to be done to the window post-game.
		/// </summary>
		public void PostPlayCleanup()
		{
			// Remove handlers
			window.KeyPressed -= InGameKeypressHandler;
		}

		/// <summary>
		/// This handler is called for every keypress in the order that they were placed, when window.DispatchEvents() is called by <see cref="GetPlayerActions" />.
		/// It adds any valid actions the the keypress translates to into <see cref="actionsBuffer" />.				
		/// </summary>
		/// <param name="sender">Not used.</param>
		/// <param name="e">The KeyEventArgs object.</param>
		private void InGameKeypressHandler(object? sender, KeyEventArgs e)
		{
			PlayerAction action = e.Code switch
			{
				var k when k == Settings.upKey.sfml => PlayerAction.Up,
				var k when k == Settings.downKey.sfml => PlayerAction.Down,
				var k when k == Settings.leftKey.sfml => PlayerAction.Left,
				var k when k == Settings.rightKey.sfml => PlayerAction.Right,
				var k when k == Settings.pauseKey.sfml => PlayerAction.Pause,
				var k when k == Settings.quitKey.sfml => PlayerAction.Quit,
				_ => PlayerAction.None
			};

			if (action != PlayerAction.None)
			{
				actionsBuffer.Add(action);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void LoadSprites()
		{
			sprites[CellType.Empty] = new List<Sprite>();
			Texture tx = new("../../../sprites/floor.png");
			Sprite sp = new(tx);
			sprites[CellType.Empty].Add(sp);

			sprites[CellType.Wall] = new List<Sprite>();
			tx = new("../../../sprites/wall.png");
			sp = new(tx);
			sprites[CellType.Wall].Add(sp);

			sprites[CellType.SnakeSegment] = new List<Sprite>();
			tx = new("../../../sprites/snake.png");
			sp = new(tx);
			sprites[CellType.SnakeSegment].Add(sp);

			sprites[CellType.GrowPill] = new List<Sprite>();
			tx = new("../../../sprites/pill.png");
			sp = new(tx);
			sprites[CellType.GrowPill].Add(sp);
		}
	}
}
