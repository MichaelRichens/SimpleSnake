using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace SimpleSnake
{
	/// <summary>
	/// Immutable struct for holding a keybinding in the different types of enum used in different environments.
	/// For use by Settings to store keybindings.
	/// No need for the same actual key to be used in each environemnt - they can be defined and redefined seperately.
	/// </summary>
	internal readonly struct KeyBinding
	{
		/// <summary>
		/// The binding used by the Console.
		/// </summary>
		internal readonly ConsoleKey console;

		/// <summary>
		/// The binding used by SFNL.
		/// </summary>
		internal readonly Keyboard.Key sfml;

		internal KeyBinding(ConsoleKey console, Keyboard.Key sfml)
		{
			this.console = console;
			this.sfml = sfml;
		}
	}
}
