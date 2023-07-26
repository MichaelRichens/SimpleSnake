using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// Class to hold and modify any results from the game that need to be passed back - score basically.
	/// </summary>
	public class GameResults
	{
		/// <summary>
		/// Backing value for Score property.
		/// </summary>
		private long score;

		/// <summary>
		/// Gets the game score.
		/// </summary>
		public long Score { get => score; }

		/// <summary>
		/// Gets the highest score achieved this session.
		/// </summary>
		public long SessionHighScore { get; }

		/// <summary>
		/// Gets the highest score achieved and saved on this system.
		/// </summary>
		public long AllTimeHighScore { get; }

		/// <summary>
		/// Constructs a new instance of game result.
		/// </summary>
		/// <param name="sessionHighScore">The highest score achieved this session</param>
		/// <param name="allTimeHighScore">The highest score achieved in any session.</param>
		public GameResults(long sessionHighScore, long allTimeHighScore)
		{
			SessionHighScore = sessionHighScore;
			AllTimeHighScore = allTimeHighScore;
		}

		/// <summary>
		/// Applies an increase in the game score - intended to be run every game tick.  Calculated based on the game delay (lower delay means higher score).
		/// </summary>
		/// <param name="delay">The delay being applied each tick (in ms)</param>		
		public void TickUpScore(int delay, int length)
		{
			// Score is increased by the reciprocal of the game delay, multiplied by the snake length and a constant.

			double delayReciprocal = 1d / delay;
			int scoreForTick = (int)Math.Round(delayReciprocal * length * Settings.scoreMultipler);

			score += scoreForTick;
		}
	}
}
