using System;
using Primoris.Universe.Stargen.Data;

namespace Primoris.Universe.Stargen
{

	[Obsolete]
    public static class StarGenerator
    {
        public static double MinSunAge = 1.0E9;
        public static double MaxSunAge = 6.0E9;

        public static Star GetDefaultStar()
        {
			return new Star();
        }
    }
}
