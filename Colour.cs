using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// Immutable struct for holding a colour in the different types of enum used in different environments.
	/// For use by Settings to store game colours.
	/// No need for the same actual colour to be used in each environemnt - they can be defined and redefined seperately.
	/// </summary>
	internal readonly struct Colour
	{
		/// <summary>
		/// The binding used by the Console.
		/// </summary>
		internal readonly ConsoleColor console;

		/// <summary>
		/// The binding used by SFNL.
		/// </summary>
		internal readonly SFML.Graphics.Color sfml;

		internal Colour(ConsoleColor console, SFML.Graphics.Color sfml)
		{
			this.console = console;
			this.sfml = sfml;
		}
	}
}
