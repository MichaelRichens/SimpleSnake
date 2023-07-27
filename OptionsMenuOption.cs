using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSnake
{
	/// <summary>
	/// Enum that contains the options the player may make in the options menu.  Used to populate TextStrings.OptionsMenu - new values must be added there to take effect, any values not included there are ignored.
	/// </summary>
	enum OptionsMenuOption
	{
		ChangeBoardSize,
		Back,
	}
}
