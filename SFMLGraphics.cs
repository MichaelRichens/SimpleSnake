using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace SimpleSnake
{
	class SFMLGraphics : IGraphicsOutput
	{
		// The integer value of Keyboard.Key.Num0
		private const int SfmlNum0 = 26;
		// The integer value of Keyboard.Key.Numpad0
		private const int SfmlNumPad0 = 75;

		private readonly Font font;

		private readonly RenderWindow window;

		/// <summary>
		/// Returns a new instance of SFMLGraphics, which creates a window that it will use for its output.
		/// </summary>
		internal SFMLGraphics()
		{
			window = new RenderWindow(new VideoMode(800, 600), "SimpleSnake");

			font = new Font("../../../fonts/MontserratMedium.ttf");

			window.Clear(Color.Black);
		}

		/// <summary>
		/// The DrawBoard method is passed a 2D cells array, and displays it to the user.
		/// </summary>
		/// <param name="cells"></param>
		public void DrawBoard(Cell[,] cells)
		{
			throw new NotImplementedException("DrawBoard");
		}

		/// <summary>
		/// Does any start of game housekeeping needed before the board is first drawn (eg clearing the screen).
		/// </summary>
		/// <param name="width">The width of the board in cells</param>
		/// <param name="height">The height of the board in cells.</param>
		public void InitBoard(int width, int height)
		{
			throw new NotImplementedException("InitBoard");
		}

		/// <summary>
		/// Presents the main game menu to the player, and returns their choice.
		/// </summary>
		/// <returns>The choice made by the player.</returns>
		public MainMenuOption MainMenu()
		{
			return MenuFromEnum<MainMenuOption>(TextStrings.MainMenu);

		}

		/// <summary>
		/// A utility function for displaying a menu to the player inside the instance's window, and getting a choice from all the options in the enum type parameter that is passed.  Text for the options is passed in as a dictionary.
		/// </summary>
		/// <typeparam name="TEnum">The enum representing the options to be presented to the player.</typeparam>
		/// <param name="enumTextLookup">A dictionary containing each of these options as a key, with the value being a string with the text to display.</param>
		/// <returns>The chosen option - a value of the enum type passed in.</returns>
		/// <exception cref="ArgumentException">If there are no values in the enum, or if the number of values in the enum does not match the size of the enumTextLookup dictionary.</exception>
		/// <exception cref="NotImplementedException">Enums with more than 9 values are not handled.</exception>
		private TEnum MenuFromEnum<TEnum>(Dictionary<TEnum, string> enumTextLookup) where TEnum : struct, Enum
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

			// Dictionary to store keypress assigned to choose menu option to the number that will be displayed for that option.
			Dictionary<int, TEnum> options = new();

			// Create an array of Text objects to display the options
			var optionText = new List<Text>();
			int optionNum = 1;
			foreach (TEnum option in values)
			{
				// Create the text object
				var text = new Text($"{(char)(optionNum + '0')}. {enumTextLookup[option]}", font, 50)
				{
					// Set the position of the text
					Position = new SFML.System.Vector2f(100, 100 * optionNum),

					// Set the color of the text
					FillColor = Color.White
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
			void keyPressHandler(object? sender, KeyEventArgs e)
			{
				// Dealing with converting KeyEventArgs.Code enum back to character is annoying since there doesn't appear to be a conversion function.
				// And since we are using numbers, it could be either the main number keys of the num pad keys that are used.
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

			// Add event handlers to window
			window.KeyPressed += keyPressHandler;

			// Window loop
			while (playerChoice == -1)
			{

				window.Clear(Color.Black);

				foreach (Text text in optionText)
				{
					window.Draw(text);
				}

				window.Display();

				window.DispatchEvents();
			}

			// Remove event handlers from window
			window.KeyPressed -= keyPressHandler;

			// Return the selected option.
			return options[playerChoice];
		}
	}
}
