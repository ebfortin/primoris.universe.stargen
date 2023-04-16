using System;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies.Burrows
{

	public class Accrete : IBodyFormationAlgorithm
	{
		private record InnerSeed : Seed
		{
			internal InnerSeed? NextBody { get; set; } = null;
			internal InnerSeed? FirstSatellite { get; set; } = null;

			public InnerSeed(Length a, Ratio e, Mass mass, Mass dMass, Mass gMass) : base(a, e, mass, dMass, gMass)
			{
			}
		}

		public Ratio DustDensityCoefficient { get; }
		public Ratio CloudEccentricity { get; }
		public Ratio GasDustRatio { get; }
		

		private bool _dustLeft;
		private double _rInner;
		private double _rOuter;
		private double _reducedMass;
		private double _dustDensity;
		private Ratio _cloudEccentricity;
		private DustRecord? _dustHead = null;
		private InnerSeed? _planetHead;
		private Generation? _histHead;


		public Accrete(Ratio e, Ratio gdr, Ratio dust)
		{
			CloudEccentricity = e;
			GasDustRatio = gdr;
			DustDensityCoefficient = dust;
		}

		// TODO documentation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stellarMassRatio"></param>
		/// <param name="stellarLumRatio"></param>
		/// <param name="innerDust"></param>
		/// <param name="outerDust"></param>
		/// <param name="outerPlanetLimit"></param>
		/// <param name="dustDensityCoeff"></param>
		/// <returns></returns>
		public IEnumerable<Seed> CreateSeeds(Mass stellarMassRatio,
										Luminosity stellarLumRatio,
										Length innerDust,
										Length outerDust,
										Length outerPlanetLimit,
										Length semiMajorAxisAU)
		{
			SetInitialConditions(innerDust, outerDust);

			Length planet_inner_bound = NearestPlanet(stellarMassRatio);
			Length planet_outer_bound = outerPlanetLimit.AstronomicalUnits == 0.0
				? FarthestPlanet(stellarMassRatio)
				: outerPlanetLimit;

			while (_dustLeft)
			{
				Length a;
				Ratio e;
				a = semiMajorAxisAU.Equals(Length.Zero, Extensions.Epsilon, ComparisonType.Relative) ? Length.FromAstronomicalUnits(Extensions.RandomNumber(planet_inner_bound.AstronomicalUnits, planet_outer_bound.AstronomicalUnits)) : semiMajorAxisAU;
				e = CloudEccentricity.Equals(Ratio.Zero, Extensions.Epsilon, ComparisonType.Relative) ? Ratio.FromDecimalFractions(Extensions.RandomEccentricity()) : CloudEccentricity;

				Mass mass = GlobalConstants.PROTOPLANET_MASS;
				Mass dust_mass = Mass.FromSolarMasses(0.0);
				Mass gas_mass = Mass.FromSolarMasses(0.0);


				if (DustAvailable(InnerEffectLimit(a.AstronomicalUnits, e.DecimalFractions, mass.SolarMasses), OuterEffectLimit(a.AstronomicalUnits, e.DecimalFractions, mass.SolarMasses)))
				{
					_dustDensity = (DustDensityCoefficient * Math.Sqrt(stellarMassRatio.SolarMasses) * Math.Exp(-GlobalConstants.ALPHA * Math.Pow(a.AstronomicalUnits, 1.0 / GlobalConstants.N))).DecimalFractions;
					Mass crit_mass = Mass.FromSolarMasses(CriticalLimit(a.AstronomicalUnits, e.DecimalFractions, stellarLumRatio.SolarLuminosities));
					AccreteDust(ref mass, ref dust_mass, ref gas_mass, a, e, crit_mass, planet_inner_bound, planet_outer_bound);

					dust_mass += GlobalConstants.PROTOPLANET_MASS;

					if (mass > GlobalConstants.PROTOPLANET_MASS)
					{
						CoalescePlanetesimals(a, e, mass, crit_mass,
											   dust_mass, gas_mass,
											   stellarLumRatio,
											   planet_inner_bound, planet_outer_bound,
											   true);
					}

				}

			}

			return ProcessSeeds(_planetHead);
		}

		private IEnumerable<Seed> ProcessSeeds(InnerSeed? nextSeed)
		{
			var seedList = new List<Seed>();
			var next = nextSeed;

			while (next != null)
			{
				seedList.Add(next);
				if (next.FirstSatellite != null)
				{
					next.Satellites = ProcessSeeds(next.FirstSatellite);
				}
				next = next.NextBody;
			}

			return seedList;
		}

		private void SetInitialConditions(Length inner_limit_of_dust, Length outer_limit_of_dust)
		{
			_histHead = new Generation();

			_planetHead = null;
			_dustHead = new DustRecord();
			_dustHead.NextBand = null;
			_dustHead.OuterEdge = outer_limit_of_dust.AstronomicalUnits;
			_dustHead.InnerEdge = inner_limit_of_dust.AstronomicalUnits;
			_dustHead.DustPresent = true;
			_dustHead.GasPresent = true;
			_dustLeft = true;
			_cloudEccentricity = CloudEccentricity;

			_histHead.Dusts = _dustHead;
			_histHead.Bodies = _planetHead;
			_histHead.Next = _histHead;
		}

		private Length NearestPlanet(Mass stell_mass_ratio)
		{
			return Length.FromAstronomicalUnits(0.3 * Math.Pow(stell_mass_ratio.SolarMasses, 1.0 / 3.0));
		}

		private Length FarthestPlanet(Mass stell_mass_ratio)
		{
			return Length.FromAstronomicalUnits(50.0 * Math.Pow(stell_mass_ratio.SolarMasses, 1.0 / 3.0));
		}

		private double InnerEffectLimit(double a, double e, double mass)
		{
			return a * (1.0 - e) * (1.0 - mass) / (1.0 + _cloudEccentricity.DecimalFractions);
		}

		private double OuterEffectLimit(double a, double e, double mass)
		{
			return a * (1.0 + e) * (1.0 + mass) / (1.0 - _cloudEccentricity.DecimalFractions);
		}

		private bool DustAvailable(double inside_range, double outside_range)
		{
			DustRecord? current_dust_band;
			bool dust_here;

			current_dust_band = _dustHead;
			while (current_dust_band != null && current_dust_band.OuterEdge < inside_range)
			{
				current_dust_band = current_dust_band.NextBand;
			}

			if (current_dust_band == null)
			{
				dust_here = false;
			}
			else
			{
				dust_here = current_dust_band.DustPresent;
				while (current_dust_band != null && current_dust_band.InnerEdge < outside_range)
				{
					dust_here = dust_here || current_dust_band.DustPresent;
					current_dust_band = current_dust_band.NextBand;
				}
			}

			return dust_here;
		}

		private void UpdateDustLanes(double min, double max, Mass mass, Mass crit_mass, Length body_inner_bound, Length body_outer_bound)
		{
			bool gas;
			DustRecord? node1 = null;
			DustRecord? node2 = null;
			DustRecord? node3 = null;

			_dustLeft = false;
			if (mass > crit_mass)
			{
				gas = false;
			}
			else
			{
				gas = true;
			}

			node1 = _dustHead;
			while (node1 != null)
			{
				if (node1.InnerEdge < min && node1.OuterEdge > max)
				{
					node2 = new DustRecord();
					node2.InnerEdge = min;
					node2.OuterEdge = max;
					if (node1.GasPresent == true)
					{
						node2.GasPresent = gas;
					}
					else
					{
						node2.GasPresent = false;
					}
					node2.DustPresent = false;
					node3 = new DustRecord();
					node3.InnerEdge = max;
					node3.OuterEdge = node1.OuterEdge;
					node3.GasPresent = node1.GasPresent;
					node3.DustPresent = node1.DustPresent;
					node3.NextBand = node1.NextBand;
					node1.NextBand = node2;
					node2.NextBand = node3;
					node1.OuterEdge = min;
					node1 = node3.NextBand;
				}
				else if (node1.InnerEdge < max && node1.OuterEdge > max)
				{
					node2 = new DustRecord();
					node2.NextBand = node1.NextBand;
					node2.DustPresent = node1.DustPresent;
					node2.GasPresent = node1.GasPresent;
					node2.OuterEdge = node1.OuterEdge;
					node2.InnerEdge = max;
					node1.NextBand = node2;
					node1.OuterEdge = max;
					if (node1.GasPresent == true)
					{
						node1.GasPresent = gas;
					}
					else
					{
						node1.GasPresent = false;
					}
					node1.DustPresent = false;
					node1 = node2.NextBand;
				}
				else if (node1.InnerEdge < min && node1.OuterEdge > min)
				{
					node2 = new DustRecord();
					node2.NextBand = node1.NextBand;
					node2.DustPresent = false;
					if (node1.GasPresent == true)
					{
						node2.GasPresent = gas;
					}
					else
					{
						node2.GasPresent = false;
					}
					node2.OuterEdge = node1.OuterEdge;
					node2.InnerEdge = min;
					node1.NextBand = node2;
					node1.OuterEdge = min;
					node1 = node2.NextBand;
				}
				else if (node1.InnerEdge >= min && node1.OuterEdge <= max)
				{
					if (node1.GasPresent == true)
					{
						node1.GasPresent = gas;
					}
					node1.DustPresent = false;
					node1 = node1.NextBand;
				}
				else if (node1.OuterEdge < min || node1.InnerEdge > max)
				{
					node1 = node1.NextBand;
				}
			}

			// GL: This seems to be combining adjacent dust bands?
			node1 = _dustHead;
			while (node1 != null)
			{
				if (node1.DustPresent && node1.OuterEdge >= body_inner_bound.AstronomicalUnits && node1.InnerEdge <= body_outer_bound.AstronomicalUnits)
				{
					_dustLeft = true;
				}

				node2 = node1.NextBand;
				if (node2 != null)
				{
					if (node1.DustPresent == node2.DustPresent && node1.GasPresent == node2.GasPresent)
					{
						node1.OuterEdge = node2.OuterEdge;
						node1.NextBand = node2.NextBand;
						node2 = null;
					}
				}
				node1 = node1.NextBand;
			}
		}

		private Mass CollectDust(Mass last_mass, ref Mass new_dust, ref Mass new_gas, Length a, Ratio e, Mass crit_mass, ref DustRecord? dust_band)
		{
			double temp = last_mass.SolarMasses / (1.0 + last_mass.SolarMasses);
			_reducedMass = Math.Pow(temp, 1.0 / 4.0);
			_rInner = InnerEffectLimit(a.AstronomicalUnits, e.DecimalFractions, _reducedMass);
			_rOuter = OuterEffectLimit(a.AstronomicalUnits, e.DecimalFractions, _reducedMass);

			if (_rInner < 0.0)
			{
				_rInner = 0.0;
			}

			if (dust_band is null)
			{
				return Mass.FromSolarMasses(0.0);
			}
			else
			{
				double gas_density = 0.0;
				double temp_density;
				double mass_density;
				if (dust_band.DustPresent == false)
				{
					temp_density = 0.0;
				}
				else
				{
					temp_density = _dustDensity;
				}

				if (last_mass < crit_mass || dust_band.GasPresent == false)
				{
					mass_density = temp_density;
				}
				else
				{
					mass_density = GasDustRatio.DecimalFractions * temp_density / (1.0 + Math.Sqrt(crit_mass / last_mass) * (GasDustRatio.DecimalFractions - 1.0));
					gas_density = mass_density - temp_density;
				}

				if (dust_band.OuterEdge <= _rInner || dust_band.InnerEdge >= _rOuter)
				{
					return CollectDust(last_mass, ref new_dust, ref new_gas, a, e, crit_mass, ref dust_band.NextBand);
				}
				else
				{
					double bandwidth = _rOuter - _rInner;

					double temp1 = _rOuter - dust_band.OuterEdge;
					if (temp1 < 0.0)
					{
						temp1 = 0.0;
					}
					double width = bandwidth - temp1;

					double temp2 = dust_band.InnerEdge - _rInner;
					if (temp2 < 0.0)
					{
						temp2 = 0.0;
					}
					width = width - temp2;

					temp = 4.0 * Math.PI * Math.Pow(a.AstronomicalUnits, 2.0) * _reducedMass * (1.0 - e.DecimalFractions * (temp1 - temp2) / bandwidth);
					double volume = temp * width;

					Mass new_mass = Mass.FromSolarMasses(volume * mass_density);
					new_gas = Mass.FromSolarMasses(volume * gas_density);
					new_dust = new_mass - new_gas;

					Mass next_dust = Mass.FromSolarMasses(0.0);
					Mass next_gas = Mass.FromSolarMasses(0.0);
					Mass next_mass = CollectDust(last_mass, ref next_dust, ref next_gas, a, e, crit_mass, ref dust_band.NextBand);

					new_gas = new_gas + next_gas;
					new_dust = new_dust + next_dust;

					return new_mass + next_mass;
				}
			}
		}

		private double CriticalLimit(double orb_radius, double eccentricity, double stell_luminosity_ratio)
		{
			double perihelion_dist = orb_radius - orb_radius * eccentricity;
			double temp = perihelion_dist * Math.Sqrt(stell_luminosity_ratio);
			return GlobalConstants.B * Math.Pow(temp, -0.75);
		}

		private void AccreteDust(ref Mass seed_mass, ref Mass new_dust, ref Mass new_gas, Length a, Ratio e, Mass crit_mass, Length body_inner_bound, Length body_outer_bound)
		{
			Mass new_mass = seed_mass;
			Mass temp_mass;

			do
			{
				temp_mass = new_mass;
				new_mass = CollectDust(new_mass, ref new_dust, ref new_gas, a, e, crit_mass, ref _dustHead);
			}
			while (!(new_mass - temp_mass < 0.0001 * temp_mass));

			seed_mass = seed_mass + new_mass;
			UpdateDustLanes(_rInner, _rOuter, seed_mass, crit_mass, body_inner_bound, body_outer_bound);
		}

		private bool DoMoons(InnerSeed planet, Mass mass, Mass critMass, Mass dustMass, Mass gasMass)
		{
			bool finished = false;
			Mass existingMass = Mass.FromSolarMasses(0.0);

			if (planet.FirstSatellite != null)
			{
				for (InnerSeed? m = planet.FirstSatellite; m != null; m = m.NextBody)
				{
					existingMass += m.Mass;
				}
			}

			if (mass < critMass)
			{
				if (mass.EarthMasses < 2.5 && mass.EarthMasses > .0001 && existingMass < planet.Mass * .05)
				{
					InnerSeed moon = new InnerSeed(Length.FromMeters(0.0), Ratio.FromDecimalFractions(0.0), mass, dustMass, gasMass);

					if (moon.DustMass + moon.GasMass > planet.DustMass + planet.GasMass)
					{
						Mass tempDust = planet.DustMass;
						Mass tempGas = planet.GasMass;
						Mass tempMass = planet.Mass;

						planet.DustMass = moon.DustMass;
						planet.GasMass = moon.GasMass;
						planet.Mass = moon.Mass;

						moon.DustMass = tempDust;
						moon.GasMass = tempGas;
						moon.Mass = tempMass;
					}

					if (planet.FirstSatellite == null)
					{
						planet.FirstSatellite = moon;
					}
					else
					{
						moon.NextBody = planet.FirstSatellite;
						planet.FirstSatellite = moon;
					}

					finished = true;

					//Trace.TraceInformation("Moon captured... {0:0.00} AU ({1:0.00}EM) <- {2:0.00} EM", 
					//    the_planet.a, 
					//    the_planet.mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
					//    mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES);
				}
				//else
				//{
				//    Trace.TraceInformation("Moon escapes... {0:0.00} AU ({1:0.00} EM){2} {3:0.00}L {4}", 
				//        the_planet.a,
				//        the_planet.mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
				//        existing_mass < (the_planet.mass * .05) ? "" : " (big moons)",
				//        mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
				//        (mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) >= 2.5 ? ", too big" : (mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES) <= .0001 ? ", too small" : "");
				//}
			}

			return finished;
		}

		private Ratio GetNewEccentricity(Seed the_planet, Ratio e, Length a, Mass mass, Length newA)
		{
			var newE = the_planet.Mass.SolarMasses * Math.Sqrt(the_planet.SemiMajorAxis.AstronomicalUnits) * Math.Sqrt(1.0 - Math.Pow(the_planet.Eccentricity.Value, 2.0));
			newE = newE + mass.SolarMasses * Math.Sqrt(a.AstronomicalUnits) * Math.Sqrt(Math.Sqrt(1.0 - Math.Pow(e.Value, 2.0)));
			newE = newE / ((the_planet.Mass.SolarMasses + mass.SolarMasses) * Math.Sqrt(newA.AstronomicalUnits));
			newE = 1.0 - Math.Pow(newE, 2.0);
			if (newE < 0.0 || newE >= 1.0)
			{
				newE = 0.0;
			}
			return Ratio.FromDecimalFractions(Math.Sqrt(newE));
		}

		private void CoalescePlanetesimals(Length a,
									 Ratio e,
									 Mass mass,
									 Mass critMass,
									 Mass dustMass,
									 Mass gasMass,
									 Luminosity stellLuminosityRatio,
									 Length bodyInnerBound,
									 Length bodyOuterBound,
									 bool doMoons)
		{
			// First we try to find an existing planet with an over-lapping orbit.
			InnerSeed? thePlanet = null;
			InnerSeed? nextPlanet = null;
			InnerSeed? prevPlanet = null;
			var finished = false;
			for (thePlanet = _planetHead; thePlanet != null; thePlanet = thePlanet.NextBody)
			{
				Length diff = thePlanet.SemiMajorAxis - a;
				Length dist1;
				Length dist2;

				if (diff.AstronomicalUnits > 0.0)
				{
					dist1 = a * (1.0 + e.Value) * (1.0 + _reducedMass) - a;
					/* x aphelion	 */
					_reducedMass = Math.Pow(thePlanet.Mass.SolarMasses / (1.0 + thePlanet.Mass.SolarMasses), 1.0 / 4.0);
					dist2 = thePlanet.SemiMajorAxis - thePlanet.SemiMajorAxis * (1.0 - thePlanet.Eccentricity.Value) * (1.0 - _reducedMass);
				}
				else
				{
					dist1 = a - a * (1.0 - e.Value) * (1.0 - _reducedMass);
					/* x perihelion */
					_reducedMass = Math.Pow(thePlanet.Mass.SolarMasses / (1.0 + thePlanet.Mass.SolarMasses), 1.0 / 4.0);
					dist2 = thePlanet.SemiMajorAxis * (1.0 + thePlanet.Eccentricity.Value) * (1.0 + _reducedMass)
						- thePlanet.SemiMajorAxis;
				}

				// Did the planetesimal collide with this planet?
				if (Math.Abs(diff.Value) <= Math.Abs(dist1.Value) || Math.Abs(diff.Value) <= Math.Abs(dist2.Value))
				{
					Mass new_dust = Mass.FromSolarMasses(0.0);
					Mass new_gas = Mass.FromSolarMasses(0.0);
					Length new_a = Length.FromAstronomicalUnits((thePlanet.Mass.SolarMasses + mass.SolarMasses) 
														/ (thePlanet.Mass.SolarMasses / thePlanet.SemiMajorAxis.AstronomicalUnits + mass.SolarMasses / a.AstronomicalUnits));

					e = GetNewEccentricity(thePlanet, e, a, mass, new_a);

					if (doMoons)
					{
						finished = DoMoons(thePlanet, mass, critMass, dustMass, gasMass);
					}

					if (!finished)
					{
						//Trace.TraceInformation("Collision between two planetesimals!\n {0:0.00} AU ({1:0.00}EM) + {2:0.00} AU ({3:0.00}EM = {4:0.00}EMd + {5:0.00}EMg [{6:0.00}EM]). {7:0.00} AU ({8:0.00})",
						//    the_planet.a, the_planet.mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
						//    a, mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
						//    dust_mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
						//    gas_mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
						//    crit_mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES,
						//    new_a, e);

						var newMass = thePlanet.Mass + mass;
						// TODO: Why is a solar luminosity ratio passed as a Mass for critical mass?
						AccreteDust(ref newMass, ref new_dust, ref new_gas,
									 new_a, e, Mass.FromSolarMasses(stellLuminosityRatio.SolarLuminosities),
									 bodyInnerBound, bodyOuterBound);

						thePlanet.SemiMajorAxis = new_a;
						thePlanet.Eccentricity = e;
						thePlanet.Mass = newMass;
						thePlanet.DustMass += dustMass + new_dust;
						thePlanet.GasMass += gasMass + new_gas;
						if (thePlanet.Mass >= critMass)
						{
							thePlanet.IsGasGiant = true;
						}

						while (thePlanet.NextBody != null && thePlanet.NextBody.SemiMajorAxis < new_a)
						{
							nextPlanet = thePlanet.NextBody;

							if (thePlanet == _planetHead)
							{
								_planetHead = nextPlanet;
							}
							else
							{
								prevPlanet!.NextBody = nextPlanet;
							}

							thePlanet.NextBody = nextPlanet.NextBody;
							nextPlanet.NextBody = thePlanet;
							prevPlanet = nextPlanet;
						}
					}

					finished = true;
					break;
				}
				else
				{
					prevPlanet = thePlanet;
				}
			}

			// Planetesimals didn't collide. Make it a planet.
			if (!finished)
			{
				thePlanet = new InnerSeed(a, e, mass, dustMass, gasMass);

				if (mass >= critMass)
				{
					thePlanet.IsGasGiant = true;
				}
				else
				{
					thePlanet.IsGasGiant = false;
				}

				if (_planetHead == null)
				{
					_planetHead = thePlanet;
					thePlanet.NextBody = null;
				}
				else if (a < _planetHead.SemiMajorAxis)
				{
					thePlanet.NextBody = _planetHead;
					_planetHead = thePlanet;
				}
				else if (_planetHead.NextBody == null)
				{
					_planetHead.NextBody = thePlanet;
					thePlanet.NextBody = null;
				}
				else
				{
					nextPlanet = _planetHead;
					while (nextPlanet != null && nextPlanet.SemiMajorAxis < a)
					{
						prevPlanet = nextPlanet;
						nextPlanet = nextPlanet.NextBody;
					}
					thePlanet.NextBody = nextPlanet;
					prevPlanet!.NextBody = thePlanet;
				}
			}
		}

	}
}