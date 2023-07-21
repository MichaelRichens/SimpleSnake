using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleSnake
{
	/// <summary>
	/// The SnakeGame class represents an individual game of Snake - the Play method starts the game, and should only be called once.
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
		/// A reference to the IGraphicsMode that handles display of graphics and user input.
		/// </summary>
		private readonly IGraphicsMode graphicsMode;

		/// <summary>
		/// Used to store moves that are entered - multiple moves can be entered in the same game loop iteration that need to be applied across multiple iterations, these get stored here.
		/// </summary>
		private readonly List<Direction> pendingMoves = new();

		private readonly long sessionHighScore;

		/// <summary>
		/// The delay between game updates in ms
		/// </summary>
		private int Delay { get; }

		/// <summary>
		/// Holds information that needs to survive the game (the score basically).
		/// </summary>
		public GameResults GameResults { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SnakeGame"/> class.
		/// </summary>
		/// <param name="width">The width of the game board in cells.</param>
		/// <param name="height">The height of the game board in cells.</param>
		/// <param name="delay">The delay between game updates in ms.</param>
		/// <param name="graphicsMode">Provide an IGraphicsMode implementor to display the graphics.</param>
		internal SnakeGame(int width, int height, int startingLength, Direction startingDirection, int delay, long sessionHighScore, IGraphicsMode graphicsMode)
		{
			this.graphicsMode = graphicsMode;

			this.sessionHighScore = sessionHighScore;

			Delay = delay;

			GameResults = new GameResults();

			snake = new Snake(startingLength, startingDirection, width / 2, height / 2);

			board = new Board(width, height, snake.HeadX, snake.HeadY, snake.length, graphicsMode);
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

				// Award score
				GameResults.TickUpScore(Delay, snake.length);

				// Output the current state of the board.
				board.Draw(GameResults);

				// Get any player actions placed in the buffer during the last iteration
				List<PlayerAction> actions = graphicsMode.GetPlayerActions();

				// Process player actions in the order they were made.
				for (int i = 0; i < actions.Count; i++)
				{
					// Calculate any move orders based on the last pending move order entered, and if this is a valid move order, add it to the pendingMoves to make.
					Direction lastDirection = pendingMoves.Count == 0 ? snake.direction : pendingMoves[^1];
					Direction nextDirection = GetNewDirection(lastDirection, actions[i]);
					if (nextDirection != lastDirection)
					{
						pendingMoves.Add(nextDirection);
					}

					// Handle player quitting.
					if (actions[i] == PlayerAction.Quit)
					{
						endGame = true;
					}

					// Handle the player pausing, but ignore it if it isn't the last action in the buffer.
					if (actions[i] == PlayerAction.Pause && i == actions.Count - 1)
					{
						loopTimer.Stop();
						bool quitGameAfterPause = graphicsMode.PauseGameWithExitOption();
						loopTimer.Start();

						if (quitGameAfterPause)
						{
							endGame = true;
						}
					}
				}

				// Apply a single pending move order - any further orders will be applied next iteration (before any orders entered then are applied).
				// Only allowing a single move execution per iteration, but preservbing subsequent moves, allows the player to quickly enter two moves and have them execute in sequence as they would expect.
				// If moves are not saved across iterations, this would either result in one of the moves being discarded, or the snake only applying the resulting move (potentially doubling back on itself within a single iteration and being killed).
				if (pendingMoves.Count > 0)
				{
					snake.direction = pendingMoves[0];
					pendingMoves.RemoveAt(0); // Can't use a Queue for pendingMoves since need to be able to inspect last element.
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