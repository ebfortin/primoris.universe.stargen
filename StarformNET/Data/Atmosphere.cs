using System.Collections.Generic;
using System;
using System.Text;


namespace Primoris.Universe.Stargen.Data
{
    [Serializable]
    public class Atmosphere
    {
		public Planet Planet { get; internal set; }

        public double SurfacePressure { get; private set; }

        public Breathability Breathability { get; private set; }

		public List<Gas> Composition { get; private set; } = new List<Gas>();

		public List<Gas> PoisonousGases { get; private set; } = new List<Gas>();

		public Atmosphere(Planet planet)
        {
			Planet = planet;

            Composition = new List<Gas>();
            PoisonousGases = new List<Gas>();
			SurfacePressure = 0.0;

			Breathability = CalculateBreathability();
		}

		public Atmosphere(Planet planet, List<Gas> gases)
		{
			Planet = planet;

			Composition = gases;
			foreach (var gas in Composition)
			{
				SurfacePressure += gas.SurfacePressure;
			}

			Breathability = CalculateBreathability();
		}

		public Atmosphere(Planet planet, double surfPressure)
		{
			Planet = planet;
			SurfacePressure = surfPressure;
			CalculateGases(planet);

			Breathability = CalculateBreathability();
		}

		public Atmosphere(Planet planet, ChemType[] gasTable)
		{
            Planet = planet;
			SurfacePressure = Environment.Pressure(planet.VolatileGasInventory, planet.Radius, planet.SurfaceGravityG);
			CalculateGases(planet, gasTable);

			Breathability = CalculateBreathability();
		}

		/// <summary>
		/// Returns the breathability state of the planet's atmosphere.
		/// </summary>
		/// <returns></returns>
		private Breathability CalculateBreathability()
		{
			// This function uses figures on the maximum inspired partial pressures
			// of Oxygen, other atmospheric and traces gases as laid out on pages 15,
			// 16 and 18 of Dole's Habitable Planets for Man to derive breathability
			// of the planet's atmosphere.                                       JLB

			var planet = Planet;

			var oxygenOk = false;

			if (Composition.Count == 0)
			{
				return Data.Breathability.None;
			}

			var poisonous = false;
			PoisonousGases.Clear();
			for (var index = 0; index < Composition.Count; index++)
			{
				var gas = Composition[index];

				var ipp = Environment.InspiredPartialPressure(SurfacePressure, Composition[index].SurfacePressure);
				if (ipp > gas.GasType.MaxIpp)
				{
					poisonous = true;
					PoisonousGases.Add(gas);
				}

				// TODO why not just have a min_ipp for every gas, even if it's going to be zero for everything that's not oxygen?
				if (gas.GasType.Num == GlobalConstants.AN_O)
				{
					oxygenOk = ((ipp >= GlobalConstants.MIN_O2_IPP) && (ipp <= GlobalConstants.MAX_O2_IPP));
				}
			}

			if (poisonous)
			{
				return Data.Breathability.Poisonous;
			}
			return oxygenOk ? Data.Breathability.Breathable : Data.Breathability.Unbreathable;
		}

		public void RecalculateGases(ChemType[] gasTable)
		{
			CalculateGases(Planet, gasTable);
		}

		private void CalculateGases(Planet planet, ChemType[] gasTable = null)
		{
			gasTable ??= ChemType.Load();

			var sun = planet.Star;
			Composition = new List<Gas>();

			if (!(SurfacePressure > 0))
			{
				return;
			}

			double[] amount = new double[gasTable.Length];
			double totamount = 0;
			double pressure = SurfacePressure / GlobalConstants.MILLIBARS_PER_BAR;
			int n = 0;

			// Determine the relative abundance of each gas in the planet's atmosphere
			for (var i = 0; i < gasTable.Length; i++)
			{
				double yp = gasTable[i].Boil / (373.0 * ((Math.Log((pressure) + 0.001) / -5050.5) + (1.0 / 373.0)));

				// TODO move both of these conditions to separate methods
				if ((yp >= 0 && yp < planet.NighttimeTemperature) && (gasTable[i].Weight >= planet.MolecularWeightRetained))
				{
					double abund, react;
					CheckForSpecialRules(out abund, out react, pressure, planet, gasTable[i]);

					double vrms = Environment.RMSVelocity(gasTable[i].Weight, planet.ExosphereTemperature);
					double pvrms = Math.Pow(1 / (1 + vrms / planet.EscapeVelocityCMSec), sun.Age / 1e9);

					double fract = (1 - (planet.MolecularWeightRetained / gasTable[i].Weight));

					// Note that the amount calculated here is unitless and doesn't really mean
					// anything except as a relative value
					amount[i] = abund * pvrms * react * fract;
					totamount += amount[i];
					if (amount[i] > 0.0)
					{
						n++;
					}
				}
				else
				{
					amount[i] = 0.0;
				}
			}

			// For each gas present, calculate its partial pressure
			if (n > 0)
			{
				Composition = new List<Gas>();

				n = 0;
				for (var i = 0; i < gasTable.Length; i++)
				{
					if (amount[i] > 0.0)
					{
						Composition.Add(
							new Gas(gasTable[i], SurfacePressure * amount[i] / totamount));
					}
				}
			}

		}

		private void CheckForSpecialRules(out double abund, out double react, double pressure, Planet planet, ChemType gas)
		{
			var sun = planet.Star;
			var pres2 = 1.0;
			abund = gas.Abunds;

			if (gas.Symbol == "Ar")
			{
				react = .15 * sun.Age / 4e9;
			}
			else if (gas.Symbol == "He")
			{
				abund = abund * (0.001 + (planet.GasMassSM / planet.MassSM));
				pres2 = (0.75 + pressure);
				react = Math.Pow(1 / (1 + gas.Reactivity), sun.Age / 2e9 * pres2);
			}
			else if ((gas.Symbol == "O" || gas.Symbol == "O2") && sun.Age > 2e9 && planet.SurfaceTemperature > 270 && planet.SurfaceTemperature < 400)
			{
				// pres2 = (0.65 + pressure/2); // Breathable - M: .55-1.4
				pres2 = (0.89 + pressure / 4);  // Breathable - M: .6 -1.8
				react = Math.Pow(1 / (1 + gas.Reactivity), Math.Pow(sun.Age / 2e9, 0.25) * pres2);
			}
			else if (gas.Symbol == "CO2" && sun.Age > 2e9 && planet.SurfaceTemperature > 270 && planet.SurfaceTemperature < 400)
			{
				pres2 = (0.75 + pressure);
				react = Math.Pow(1 / (1 + gas.Reactivity), Math.Pow(sun.Age / 2e9, 0.5) * pres2);
				react *= 1.5;
			}
			else
			{
				pres2 = (0.75 + pressure);
				react = Math.Pow(1 / (1 + gas.Reactivity), sun.Age / 2e9 * pres2);
			}
		}

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (SurfacePressure > 0.0)
            {
                sb.Append(SurfacePressure);
                sb.Append(" : ");
                sb.AppendJoin<Gas>(';', Composition);
            } 
            else 
            {
                sb.Append("None");
            }

			return sb.ToString();
		}
	}
}
