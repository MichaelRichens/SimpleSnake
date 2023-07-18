using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleSnake
{
	/// <summary>
	/// The SnakeGame class represents an individual game of Snake.
	/// </summary>
	class SnakeGame
	{
		/// <summary>
		/// This field references the <see cref="Snake" /> instance which holds information about the snake.
		/// </summary>
		internal readonly Snake snake;

		/// <summary>
		/// This field references the <see cref="Board" /> instance which represents the game board.
		/// </summary>
		internal readonly Board board;

		/// <summary>
		/// A reference to the IGraphicsOutput that handles display of graphics and user input.
		/// </summary>
		private readonly IGraphicsOutput graphicsOutput;

		/// <summary>
		/// The delay between game updates in ms
		/// </summary>
		private int Delay { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SnakeGame"/> class.
		/// </summary>
		/// <param name="width">The width of the game board in cells.</param>
		/// <param name="height">The height of the game board in cells.</param>
		/// <param name="delay">The delay between game updates in ms.</param>
		/// <param name="graphicsOutput">Provide an IGraphicsOutput implementor to display the graphics.</param>
		internal SnakeGame(int width, int height, int startingLength, Direction startingDirection, int delay, IGraphicsOutput graphicsOutput)
		{
			this.graphicsOutput = graphicsOutput;

			Delay = delay;

			snake = new Snake(startingLength, startingDirection, width / 2, height / 2);

			board = new Board(width, height, snake.HeadX, snake.HeadY, snake.length, graphicsOutput);
		}

		/// <summary>
		/// This method plays the game of snake.
		/// </summary>
		internal void Play()
		{
			// Prep the board
			board.InitBoard();

			// Main loop control flag.
			bool endGame = false;

			// Timer for keeping track of how long each loop iteration has taken (for a consistent game speed).
			var loopTimer = new Stopwatch();

			board.PlacePillAtRandomEmpty();

			//****************
			// Main Game Loop.
			while (!endGame)
			{
				// Keep track of the time it takes to execute the loop so that we can have a consistent delay between iterations.
				loopTimer.Reset();
				loopTimer.Start();

				// Output the current state of the board.
				board.Draw();

				// Handle player keypress.
				if (graphicsOutput.TryGetPlayerAction(out PlayerAction action))
				{
					// Handle player changing the snake's direction.
					snake.direction = GetNewDirection(snake.direction, action);

					// Handle player quitting.
					if (action == PlayerAction.Quit)
					{
						endGame = true;
					}

					// Handle the player pausing.
					if (action == PlayerAction.Pause)
					{
						loopTimer.Stop();
						ConsoleKeyInfo unpauseKey = Console.ReadKey(true); // Blocks until keypress.
						loopTimer.Start();

						// If the user pressed the quit key, assume they wanted to quit rather than unpause.
						if (unpauseKey.Key == Settings.quitKey.console)
						{
							endGame = true;
						}
					}
				}

				// Place the snake head in the next position
				snake.AdvanceHead();
				CellType cellUnderHead = board.PlaceSnakeHead(snake);

				// Check if entering this cell has killed the snake.
				if (DoesCollisionWithKill(cellUnderHead))
				{
					endGame = true;
				}

				// Check if entering this cell has changed the snake length
				int lengthChange = LengthChangeFromCell(cellUnderHead);
				if (lengthChange != 0)
				{
					snake.length += lengthChange;
					board.ChangeSnakeLength(lengthChange);
				}

				// If a pill has just been eaten, make a new one.
				if (cellUnderHead == CellType.GrowPill)
				{
					board.PlacePillAtRandomEmpty();
				}

				// Decrease all cells with timers, and remove any cells with expired timers from the board.
				board.TickDownTimers();

				// If the loop has taken less time to execute than the value of `Delay`, wait for the additional time.
				loopTimer.Stop();
				int waitTime = Delay - (int)loopTimer.ElapsedMilliseconds;
				if (waitTime > 0)
				{
					Thread.Sleep(waitTime);
				}
			}
			// End Main Game Loop.
			//********************			
		}

		/// <summary>
		/// The GetNewDirection static method is passed the current direction the snake is travelling in, and a PlayerAction value, and returns the new direction after that value is applied.
		/// </summary>
		/// <param name="currentDirection">The current direction the snake is travveling in.</param>
		/// <param name="action">The PlayerAction to use to calcualte the new direction.</param>
		/// <returns>The new direction.</returns>
		private static Direction GetNewDirection(Direction currentDirection, PlayerAction action)
		{
			// The player can change to a direction 90 degrees from their current one.  Any other action results in the direction being unchanged.
			switch (action)
			{
				case PlayerAction.Left:
					if (currentDirection != Direction.Right)
					{
						return Direction.Left;
					}
					break;
				case PlayerAction.Right:
					if (currentDirection != Direction.Left)
					{
						return Direction.Right;
					}
					break;
				case PlayerAction.Up:
					if (currentDirection != Direction.Down)
					{
						return Direction.Up;
					}
					break;
				case PlayerAction.Down:
					if (currentDirection != Direction.Up)
					{
						return Direction.Down;
					}
					break;
			}
			return currentDirection;
		}

		/// <summary>
		/// Static method which returns whether entering this type of cell kills the snake.
		/// </summary>
		/// <param name="cellType">The CellType</param>
		/// <returns>True if the snake is killed, false otherwise.</returns>
		private static bool DoesCollisionWithKill(CellType cellType) => cellType switch
		{
			CellType.Wall or CellType.SnakeSegment => true,

			_ => false,
		};

		private static int LengthChangeFromCell(CellType cellType)
		{
			if (cellType == CellType.GrowPill)
			{
				return Settings.pillGrowAmount;
			}

			return 0;
		}
	}
}