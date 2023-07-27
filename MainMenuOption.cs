using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// Enum that contains the options the player may make in the main game menu.  Used to populate TextStrings.MainMenu - new values must be added there to take effect, any values not included there are ignored.
	/// </summary>
	public enum MainMenuOption
	{
		Play,
		Options,
		Quit,
	}
}
