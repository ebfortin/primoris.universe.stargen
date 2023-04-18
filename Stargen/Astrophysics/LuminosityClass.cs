using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Primoris.Universe.Stargen.Astrophysics;

/// <summary>
/// Luminosity classes enumeration.
/// </summary>
public enum LuminosityClass
{
	Undefined,
	O,
	/// <summary>
	/// Very luminous supergiants.
	/// </summary>
	Ia0,
	/// <summary>
	/// Very luminous supergiants.
	/// </summary>
	Ia,
	/// <summary>
	/// Less luminous supergiants.
	/// </summary>
	Ib,
	/// <summary>
	/// Luminous giants.
	/// </summary>
	II,
	/// <summary>
	/// Giants.
	/// </summary>
	III,
	/// <summary>
	/// Subgiants.
	/// </summary>
	IV,
	/// <summary>
	/// Main sequence stars (dwarf stars)
	/// </summary>
	V,
	/// <summary>
	/// Subdwarf.
	/// </summary>
	VI,
	/// <summary>
	/// White Dwarf.
	/// </summary>
	VII
}
