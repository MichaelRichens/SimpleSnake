﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// The Board class represents the playing field on which the game of snake is played.  It holds the position of all items and how long they will remain in place - this includes segments of the snake.
	/// </summary>
	/// 
	/// Internally the board class is a 2D array of immutable Cell structs, which contain a timer (counting game update ticks) and an enum holding information about the cell.
	/// Therefore the logic for handling interactions with the Cell struct are mainly contained here, doing lookups on the CellType enum.
	/// An alternative implementation would be to have each Cell as a class that implemeneted an interface, with the behaviour of each type of cell contained within each concrete class.
	/// However having the board be a simple array of structs simplifies copying the structure (implementation details of the ConsoleGraphics class, so as to compare changes to calculate which characters on the Console need to be redrawen). 	
	/// (And while performance considerations are meaningless here, it does make the memory footprint neater, since everything is held in an array of structs which pack within 8 bytes).
	/// 
	/// This snake's body is part of the board, with the CellType.SnakeSegment value being used.  Its length is determined by how long the timer is set in each cell - it ticks down and changes back to an empty cell when it expires.
	/// Each subsequent piece of snake placed expires one tick further on, creating the effect of it moving.  To grow the snake, simply add an amount to all SnakeSegment timers.
	class Board
	{
		/// <summary>
		/// The array of Cell structs, dimensioned [Height, Width] (ie [Y, X]) with [0, 0] being the top left.  Note: Cell structs are immutable.
		/// </summary>
		private readonly Cell[,] cells;

		/// <summary>
		/// Holds a reference to the injected IGraphicsOutput that handles drawing the board.
		/// </summary>
		private readonly IGraphicsOutput graphicsOutput;

		/// <summary>
		/// The height of the board in characters
		/// </summary>
		internal int Height { get; }

		/// <summary>
		/// The width of the board in characters
		/// </summary>
		internal int Width { get; }

		/// <summary>
		/// Creates a new instance of the Board class, representing the Snake game board.
		/// </summary>
		/// <param name="width">The width of the board in tiles (locations that can be occupied).</param>
		/// <param name="height">The height of the board in tiles (locations that can be occupied).</param>
		/// <param name="snakeStartX">The horizontal tile to start the snake (measured from the left).</param>
		/// <param name="snakeStartY">The vertical tile to start the snake (measured from the top).</param>
		/// <param name="snakeLength">The starting length of the snake.</param>
		/// <param name="graphicsOutput">Provide an IGraphicsOutput implementor to display the graphics.</param>
		internal Board(int width, int height, int snakeStartX, int snakeStartY, int snakeLength, IGraphicsOutput graphicsOutput)
		{
			this.graphicsOutput = graphicsOutput;

			Width = width;
			Height = height;

			// Populate the cells array with default CellType.Empty/timer = 0 cells.
			cells = new Cell[Height, Width];

			// Create the initial map
			// Create the walls
			for (int i = 0; i < Width; i++)
			{
				cells[0, i] = new Cell(CellType.Wall);
				cells[Height - 1, i] = new Cell(CellType.Wall);
			}
			for (int i = 0; i < Height; i++)
			{
				cells[i, 0] = new Cell(CellType.Wall);
				cells[i, Width - 1] = new Cell(CellType.Wall);
			}
			// Place the snake's head
			cells[snakeStartY, snakeStartX] = new Cell(CellType.SnakeSegment, snakeLength);
		}

		/// <summary>
		/// Place's the snakes head in the board at the position held in the passed Snake instance, and returns the previous CellType of the cell where it was placed.
		/// </summary>
		/// <param name="snake">The snake instance holding the snakes current stats, including the new poistion of its head.  This object is not modified.</param>
		/// <returns>The CellType found at the cell that now holds the snake's head.</returns>
		internal CellType PlaceSnakeHead(Snake snake)
		{

			CellType tileType = cells[snake.HeadY, snake.HeadX].cellType;
			cells[snake.HeadY, snake.HeadX] = new Cell(CellType.SnakeSegment, snake.length);

			return tileType;
		}

		/// <summary>
		/// Draws the current state of the board using the IGraphicsOutput instance that was injected at construction.
		/// </summary>
		internal void Draw()
		{
			graphicsOutput.DrawBoard(cells);
		}

		/// <summary>
		/// Prepares the graphics output ready for the board to first be drawn.
		/// </summary>
		internal void InitBoard()
		{
			graphicsOutput.InitBoard();
		}

		/// <summary>
		/// Runs a game time tick on every cell in the board - replaces any cells which have expired with empty cells.
		/// </summary>
		internal void TickDownTimers()
		{
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					cells[i, j] = cells[i, j].DecTimer();
				}
			}
		}
	}
}