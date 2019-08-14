using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	public class BasicGaseousLayer : GaseousLayer
	{
		public BasicGaseousLayer(Pressure surfPres) : base(surfPres)
		{
		}

		public BasicGaseousLayer(IEnumerable<(Chemical, Ratio)> composition, Pressure surfPres) : base(composition, surfPres)
		{
			Breathability = CalculateBreathability();
		}

		public override Layer Generate(SatelliteBody parentBody, Mass availableMass, IEnumerable<Chemical> availableChems, IEnumerable<Layer> curLayers)
		{
			if(curLayers.Count() != 1)
			{
				throw new InvalidBodyLayerSequenceException();
			}

			if(availableMass == Mass.Zero)
			{
				throw new InvalidBodyOperationException("Layer can't have an available mass of Zero.");
			}

			var sun = parentBody.StellarBody;
			var planet = parentBody;
			var gasTable = availableChems.ToArray();

			if (!(LowerBoundaryPressure.Millibars > 0))
			{
				return this;
			}

			double[] amount = new double[gasTable.Length];
			double totamount = 0;
			double pressure = LowerBoundaryPressure.Bars;
			int n = 0;

			// Determine the relative abundance of each gas in the planet's atmosphere
			for (var i = 0; i < gasTable.Count(); i++)
			{
				Temperature yp = Temperature.FromKelvins(gasTable[i].Boil.Kelvins / (373.0 * (Math.Log(pressure + 0.001) / -5050.5 + 1.0 / 373.0)));

				// TODO move both of these conditions to separate methods
				if (yp.Kelvins >= 0 && yp < planet.NighttimeTemperature && gasTable[i].Weight >= planet.MolecularWeightRetained)
				{
					double abund, react;
					CheckForSpecialRules(out abund, out react, pressure, planet, gasTable[i]);

					Speed vrms = Science.Physics.GetRMSVelocity(gasTable[i].Weight, planet.ExosphereTemperature);
					double pvrms = Math.Pow(1 / (1 + vrms / planet.EscapeVelocity), sun.Age.Years365 / 1e9);

					double fract = 1 - planet.MolecularWeightRetained / gasTable[i].Weight;

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

			// For each gas present, calculate its partial pressure. Was partial pressure, is now only a ratio that is "equivalentish" to mol/mol.
			if (n > 0)
			{
				CompositionInternal.Clear();

				n = 0;
				for (var i = 0; i < gasTable.Length; i++)
				{
					if (amount[i] > 0.0)
					{
						CompositionInternal.Add(
								new ValueTuple<Chemical, Ratio>() { Item1 = gasTable[i], Item2 = Ratio.FromDecimalFractions(amount[i] / totamount) }
							);
					}
				}
			}

			Breathability = CalculateBreathability();
			Mass = availableMass;

			return this;
		}

		/// <summary>
		/// This function uses figures on the maximum inspired partial pressures
		/// of Oxygen, other atmospheric and traces gases as laid out on pages 15,
		/// 16 and 18 of Dole's Habitable Planets for Man to derive breathability
		/// of the planet's atmosphere. 
		/// </summary>
		/// <returns></returns>
		private Breathability CalculateBreathability()
		{
			var planet = Parent;

			var oxygenOk = false;

			if (Composition.Count() == 0)
			{
				return Breathability.None;
			}

			var poisonous = false;
			PoisonousCompositionInternal.Clear();
			for (var index = 0; index < CompositionInternal.Count; index++)
			{
				var gas = CompositionInternal[index].Item1;

				var ipp = Science.Physics.GetInspiredPartialPressure(LowerBoundaryPressure, CompositionInternal[index].Item2.DecimalFractions * LowerBoundaryPressure);
				if (ipp > gas.MaxIpp)
				{
					poisonous = true;
					PoisonousCompositionInternal.Add(new ValueTuple<Chemical, Ratio>(gas, CompositionInternal[index].Item2));
				}

				// TODO why not just have a min_ipp for every gas, even if it's going to be zero for everything that's not oxygen?
				if (gas.Num == GlobalConstants.AN_O)
				{
					oxygenOk = ipp.Millibars >= GlobalConstants.MIN_O2_IPP && ipp.Millibars <= GlobalConstants.MAX_O2_IPP;
				}
			}

			if (poisonous)
			{
				return Breathability.Poisonous;
			}
			return oxygenOk ? Breathability.Breathable : Breathability.Unbreathable;
		}

		private void CheckForSpecialRules(out double abund, out double react, double pressure, SatelliteBody planet, Chemical gas)
		{
			var sun = planet.Parent;
			var pres2 = 1.0;
			abund = gas.Abunds.Value;

			if (gas.Symbol == "Ar")
			{
				react = .15 * sun.Age.Years365 / 4e9;
			}
			else if (gas.Symbol == "He")
			{
				abund = abund * (0.001 + planet.GasMass / planet.Mass);
				pres2 = 0.75 + pressure;
				react = Math.Pow(1 / (1 + gas.Reactivity.Value), sun.Age.Years365 / 2e9 * pres2);
			}
			else if ((gas.Symbol == "O" || gas.Symbol == "O2") && sun.Age.Years365 > 2e9 && planet.Temperature.Kelvins > 270 && planet.Temperature.Kelvins < 400)
			{
				// pres2 = (0.65 + pressure/2); // Breathable - M: .55-1.4
				pres2 = 0.89 + pressure / 4;  // Breathable - M: .6 -1.8
				react = Math.Pow(1 / (1 + gas.Reactivity.Value), Math.Pow(sun.Age.Years365 / 2e9, 0.25) * pres2);
			}
			else if (gas.Symbol == "CO2" && sun.Age.Years365 > 2e9 && planet.Temperature.Kelvins > 270 && planet.Temperature.Kelvins < 400)
			{
				pres2 = 0.75 + pressure;
				react = Math.Pow(1 / (1 + gas.Reactivity.Value), Math.Pow(sun.Age.Years365 / 2e9, 0.5) * pres2);
				react *= 1.5;
			}
			else
			{
				pres2 = 0.75 + pressure;
				react = Math.Pow(1 / (1 + gas.Reactivity.Value), sun.Age.Years365 / 2e9 * pres2);
			}
		}

	}
}
