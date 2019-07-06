using System;
using System.Linq;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Data;


namespace Primoris.Universe.Stargen.Display
{

    public class SystemInfoGroup : InfoGroup
    {
        public void SetSystem(IEnumerable<Planet> planets)
        {
            if (planets == null || planets.ElementAt(0) == null)
            {
                return;
            }

            var star = planets.ElementAt(0).Star;

            var labels = new List<string>()
            {
                "Star Age:",
                "Star Luminosity:",
                "Star Mass:",
                "Planets:"
            };

            var values = new List<string>()
            {
                StarText.GetAgeStringYearsSciN(star),
                StarText.GetLuminosityPercent(star),
                StarText.GetMassPercent(star),
                planets.Count().ToString()
            };

            SetText(labels, values);
        }
    }
}
