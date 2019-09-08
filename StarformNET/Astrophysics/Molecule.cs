using System;
using System.Collections.Generic;
using System.Text;

namespace Primoris.Universe.Stargen.Astrophysics
{
	/// <summary>
	/// Collection of Chemicals in an organized manner.
	/// </summary>
	public class Molecule : Chemical
	{
		public Molecule(int an,
				  string sym,
				  string htmlsym,
				  string name,
				  double w,
				  double m,
				  double b,
				  double dens,
				  double ae,
				  double abs,
				  double rea,
				  double mipp) : base(an,
						  sym,
						  htmlsym,
						  name,
						  w,
						  m,
						  b,
						  dens,
						  ae,
						  abs,
						  rea,
						  mipp)
		{
		}
	}
}
