
using Primoris.Universe.Stargen.Astrophysics;

using UnitsNet;

namespace Primoris.Universe.Stargen.Bodies.Burrows;


// TODO break this class up
// TODO orbit zone is supposedly no longer used anywhere. Check references and possibly remove.

[Serializable]
public class Planet : SatelliteBody
{
	/// <summary>
	/// Create a Planet given a Seed and a parent Body. Can generate planet layers or it can be left to
	/// the caller to create layers.
	/// </summary>
	/// <remarks>
	/// At the exit of this constructor Radius has a valid value, although approximative. It can be used
	/// afterward for calculation to create layers. 
	/// </remarks>
	/// <param name="science">Astrophysics model to use.</param>
	/// <param name="seed">Dust and gas seed used to generate this Planet.</param>
	/// <param name="parentBody">PArent Body of this Planet.</param>
	/// <param name="generateLayers">True if the constructor should create layers, false otherwise.</param>
	public Planet(IScienceAstrophysics science, Seed seed, Body parentBody, bool generateLayers = false) : base(science, seed, parentBody)
	{
        var approxDensity = Science.Physics.GetDensityFromStar(seed.Mass, SemiMajorAxis, StellarBody.EcosphereRadius, true);
        Radius = Mathematics.GetRadiusFromDensity(seed.Mass, approxDensity);
        
        if (generateLayers)
			Generate();
	}

    /// <summary>
    /// Generate a Planet using the Astrophysics model of the Parent.
    /// </summary>
    /// <param name="seed">Dust and gas seed used to generate this Planet.</param>
    /// <param name="parentBody">Parent Body if this Planet.</param>
    /// <param name="generateLayers">True if the constructor should create layers, false otherwise.</param>
    public Planet(Seed seed, Body parentBody, bool generateLayers = false) 
		: this(parentBody.Science, seed, parentBody, generateLayers) 
	{ 
	}


    public Planet(IScienceAstrophysics science,
				  Body parentBody,
				  Length semiMajorAxisAU,
				  Ratio eccentricity,
				  Angle axialTilt,
				  Duration dayLengthHours,
				  Duration orbitalPeriodDays,
				  Mass massSM,
				  Mass gasMassSM,
				  Length radius,
				  Temperature dayTimeTempK,
				  Temperature nightTimeTempK,
				  Temperature surfTempK,
				  Acceleration surfGrav,
				  IEnumerable<Chemical> availableChems) : base(science, new Seed(semiMajorAxisAU, eccentricity, massSM - gasMassSM, gasMassSM), parentBody)
	{
		Parent = parentBody;
		var sun = StellarBody;

		AxialTilt = axialTilt;
		var planetRadius = radius;

		var approxDensity = Science.Physics.GetDensityFromStar(Mass, SemiMajorAxis, sun.EcosphereRadius, true);

		DaytimeTemperature = dayTimeTempK;
		NighttimeTemperature = nightTimeTempK;
		Temperature = surfTempK;

		MolecularWeightRetained = Science.Physics.GetMolecularWeightRetained(surfGrav, massSM, planetRadius, ExosphereTemperature, sun.Age);
		var volatileGasInventory = Science.Physics.GetVolatileGasInventory(GasMass + DustMass,
																		   Science.Dynamics.GetEscapeVelocity(GasMass + DustMass, planetRadius),
																		   RMSVelocity,
																		   sun.Mass,
																		   GasMass,
																		   OrbitZone,
																		   Science.Planetology.TestHasGreenhouseEffect(sun.EcosphereRadius, SemiMajorAxis));

		SurfacePressure = Science.Physics.GetSurfacePressure(volatileGasInventory, planetRadius, surfGrav);

		if (Science.Planetology.TestIsGasGiant(massSM, gasMassSM, MolecularWeightRetained))
		{
			Stack.CreateLayer(ls => new BasicGiantGaseousLayer(ls, massSM, planetRadius, Chemical.All.Values));
		}
		else
		{
			var coreRadius = Science.Planetology.GetCoreRadius(massSM, OrbitZone, false);

			// Add basic Burrows layer.
			Stack.CreateLayer(ls => new BasicSolidLayer(ls, massSM - gasMassSM, coreRadius, Array.Empty<(Chemical, Ratio)>()));

			// Generate complete atmosphere.
			Stack.CreateLayer(ls => new BasicGaseousLayer(ls, gasMassSM, planetRadius - coreRadius, availableChems));
		}
	}

	public Planet(Body parentBody,
			  Length semiMajorAxisAU,
			  Ratio eccentricity,
			  Angle axialTilt,
			  Duration dayLengthHours,
			  Duration orbitalPeriodDays,
			  Mass massSM,
			  Mass gasMassSM,
			  Length radius,
			  Temperature dayTimeTempK,
			  Temperature nightTimeTempK,
			  Temperature surfTempK,
			  Acceleration surfGrav,
			  IEnumerable<Chemical> availableChems) : this(parentBody.Science,
											parentBody,
											semiMajorAxisAU,
											eccentricity,
											axialTilt,
											dayLengthHours,
											orbitalPeriodDays,
											massSM,
											gasMassSM,
											radius,
											dayTimeTempK,
											nightTimeTempK,
											surfTempK,
											surfGrav,
											availableChems)
	{
	}

	/// <summary>
	/// TODO: Add unit test.
	/// </summary>
    void AdjustPropertiesForRockyBody(Length radius)
	{
		double age = Parent.Age.Years365;
		double massSM = Mass.SolarMasses;
		double gasMassSM = Seed.GasMass.SolarMasses;

		if (gasMassSM / massSM > 0.000001)
		{
			var h2Mass = gasMassSM * 0.85;
			var heMass = (gasMassSM - h2Mass) * 0.999;

			var h2Life = Science.Physics.GetGasLife(Mass.FromGrams(GlobalConstants.MOL_HYDROGEN), ExosphereTemperature, SurfaceAcceleration, radius).Years365;
			var heLife = Science.Physics.GetGasLife(Mass.FromGrams(GlobalConstants.HELIUM), ExosphereTemperature, SurfaceAcceleration, radius).Years365;

			if (h2Life < age)
			{
				var h2Loss = (1.0 - 1.0 / Math.Exp(age / h2Life)) * h2Mass;

				Seed.GasMass -= Mass.FromSolarMasses(h2Loss);
			}

			if (heLife < age)
			{
				var heLoss = (1.0 - 1.0 / Math.Exp(age / heLife)) * heMass;

				Seed.GasMass -= Mass.FromSolarMasses(heLoss);
			}
		}
	}

	void AdjustPropertiesForGasBody()
	{
		//BoilingPointWater = Temperature.FromKelvins(GlobalConstants.NOT_APPLICABLE);
		Temperature = Temperature.FromKelvins(GlobalConstants.NOT_APPLICABLE);
		GreenhouseRiseTemperature = TemperatureDelta.Zero;
		Albedo = Ratio.FromDecimalFractions(Science.Random.About(GlobalConstants.GAS_GIANT_ALBEDO, 0.1));
		WaterCoverFraction = Ratio.Zero;
		CloudCoverFraction = Ratio.Zero;
		IceCoverFraction = Ratio.Zero;
	}

	private Angle GetRandomInclination(Length semiMajorAxis)
	{
		var inclination = ((int)(Math.Pow(semiMajorAxis.Kilometers / GlobalConstants.ASTRONOMICAL_UNIT_KM, 0.2) * Science.Random.About(GlobalConstants.EARTH_AXIAL_TILT, 0.4)) % 360);
		return Angle.FromDegrees(inclination);
	}

	protected override void Generate()
	{
		var planet = this;
		var sun = StellarBody;
		var mass = GasMass + DustMass;

		planet.AxialTilt = GetRandomInclination(SemiMajorAxis);

		var surfaceAcceleration = GetAcceleration(mass, Radius);

		MolecularWeightRetained = Science.Physics.GetMolecularWeightRetained(surfaceAcceleration, mass, Radius, ExosphereTemperature, sun.Age);

		// Is the planet a gas giant?
		if (Science.Planetology.TestIsGasGiant(mass, GasMass, MolecularWeightRetained))
		{
			//Type = GetGasGiantType(MassSM, GasMassSM);

			AdjustPropertiesForGasBody();
			SurfacePressure = Pressure.Zero;
			Stack.Clear();
			Stack.CreateLayer(ls => new BasicGiantGaseousLayer(ls, GasMass, Radius));
		}
		else // If not, it's rocky.
		{
			AdjustPropertiesForRockyBody(Radius);
            MolecularWeightRetained = Science.Physics.GetMolecularWeightRetained(surfaceAcceleration, mass, Radius, ExosphereTemperature, sun.Age);
		}

		planet.AngularVelocity = Science.Dynamics.GetAngularVelocity(mass,
                                                              Radius,
															  Density,
															  SemiMajorAxis,
															  Science.Planetology.TestIsGasGiant(Mass, GasMass, MolecularWeightRetained),
															  sun.Mass,
															  sun.Age);

		var volatileGasInventory = Science.Physics.GetVolatileGasInventory(mass,
																   EscapeVelocity,
																   RMSVelocity,
																   sun.Mass,
																   GasMass,
																   OrbitZone,
																   HasGreenhouseEffect);

		if (!Science.Planetology.TestIsGasGiant(mass, GasMass, MolecularWeightRetained))
		{
            SurfacePressure = Science.Physics.GetSurfacePressure(volatileGasInventory, Radius, planet.SurfaceAcceleration);

			// Sets: planet.surf_temp, planet.greenhs_rise, planet.albedo, planet.hydrosphere,
			// planet.cloud_cover, planet.ice_cover
			AdjustSurfaceTemperatures(SurfacePressure, Radius);

			//planet.IsTidallyLocked = Science.Planetology.TestIsTidallyLocked(DayLength, OrbitalPeriod);

			// Add basic Burrows layer.
			var coreRadius = Science.Planetology.GetCoreRadius(mass, OrbitZone, false);
			Stack.CreateLayer(ls => new BasicSolidLayer(ls, DustMass, coreRadius, Array.Empty<(Chemical, Ratio)>()));

			// Generate complete atmosphere.
			if (SurfacePressure.Millibars > 0.0 && GasMass.SolarMasses > 0.0)
				Stack.CreateLayer(ls => new BasicGaseousLayer(ls, GasMass, Radius - coreRadius, Chemical.All.Values));
		}

	}

	/// <summary>
	/// Calculates the surface acceleration of the planet.
	/// </summary>
	/// <param name="mass">Mass of the planet in solar masses</param>
	/// <param name="radius">Radius of the planet in km</param>
	/// <returns>Acceleration returned in units of cm/sec2</returns>
	private Acceleration GetAcceleration(Mass mass, Length radius)
	{
		return Acceleration.FromCentimetersPerSecondSquared(GlobalConstants.GRAV_CONSTANT * (mass.Grams) / Extensions.Pow2(radius.Centimeters));
	}

	protected override IEnumerable<SatelliteBody> GenerateSatellites(Seed seed)
	{
		var planet = this;
		var star = StellarBody;

		// Generate moons
		var sat = new List<SatelliteBody>();
		//var curMoon = seed;
		var n = 0;
		/*if (curMoon != null)
		{
			while (curMoon != null)
			{
				if (curMoon.Mass * GlobalConstants.SUN_MASS_IN_EARTH_MASSES > .000001)
				{
					curMoon.SemiMajorAxisAU = planet.SemiMajorAxisAU;
					curMoon.Eccentricity = planet.Eccentricity;

					n++;

					string moon_id = string.Format("{0}.{1}", parentBody.Position, n);

					var generatedMoon = new Moon(curMoon, star, planet, n, useRandomTilt, moon_id, genOptions);

					sat.Add(generatedMoon);
				}
				curMoon = curMoon.NextBody;
			}
		}*/

		foreach (var curMoon in seed.Satellites)
		{
			if (curMoon.Mass.EarthMasses > .000001)
			{
				curMoon.SemiMajorAxis = planet.SemiMajorAxis;
				curMoon.Eccentricity = planet.Eccentricity;

				n++;

				string moon_id = string.Format("{0}.{1}", this.Position, n);

				var generatedMoon = new Moon(Science, curMoon, star, planet, moon_id);

				sat.Add(generatedMoon);
			}
		}

		return sat;
	}

	// TODO write summary
	// TODO parameter for number of iterations? does it matter?
	/// <summary>
	/// 
	/// </summary>
	/// <param name="planet"></param>
	void AdjustSurfaceTemperatures(Pressure surfpres, Length radius)
	{
		var initTemp = Science.Thermodynamics.GetEstimatedAverageTemperature(StellarBody.EcosphereRadius, SemiMajorAxis, Albedo);

		CalculateSurfaceTemperature(true,
						   Ratio.Zero,
						   Ratio.Zero,
						   Ratio.Zero,
						   Temperature.Zero,
						   Ratio.Zero,
						   surfpres,
						   radius);

        for (var count = 0; count <= 25; count++)
		{
            var lastWater = WaterCoverFraction;
            var lastClouds = CloudCoverFraction;
            var lastIce = IceCoverFraction;
            var lastTemp = Temperature;
            var lastAlbedo = Albedo;

            CalculateSurfaceTemperature(false, lastWater, lastClouds, lastIce, lastTemp, lastAlbedo, surfpres, radius);

			if (Math.Abs((Temperature - lastTemp).Kelvins) < 0.25)
				break;
		}

		if(initTemp < Temperature)
			GreenhouseRiseTemperature = Temperature - initTemp;
	}

	/// <summary>
	/// The temperature of the planet calculated in degrees Kelvin
	/// TODO: Eliminate calls to Environment and replace with calls to Physics.
	/// TODO: Create unit test.
	/// </summary>
	/// <param name="planet"></param>
	/// <param name="first"></param>
	/// <param name="last_water"></param>
	/// <param name="last_clouds"></param>
	/// <param name="last_ice"></param>
	/// <param name="last_temp"></param>
	/// <param name="last_albedo"></param>
	void CalculateSurfaceTemperature(bool first, 
									 Ratio last_water, 
									 Ratio last_clouds, 
									 Ratio last_ice, 
									 Temperature last_temp, 
									 Ratio last_albedo, 
									 Pressure surfpres,
									 Length radius)
	{
		Temperature effectiveTemp;
		Ratio waterRaw;
		Ratio cloudsRaw;
		TemperatureDelta greenhouseTemp;
		bool boilOff = false;

		var planet = this;

		if (first)
		{
			planet.Albedo = Ratio.FromDecimalFractions(GlobalConstants.EARTH_ALBEDO);

			effectiveTemp = Science.Thermodynamics.GetEstimatedEffectiveTemperature(planet.StellarBody.EcosphereRadius, planet.SemiMajorAxis, planet.Albedo);
			greenhouseTemp = Science.Thermodynamics.GetGreenhouseTemperatureRise(Science.Planetology.GetOpacity(planet.MolecularWeightRetained, surfpres), effectiveTemp, surfpres);
			planet.Temperature = effectiveTemp + greenhouseTemp;

			SetTempRange(surfpres, Temperature);
		}

		waterRaw = planet.WaterCoverFraction = Science.Planetology.GetWaterFraction(planet.VolatileGasInventory, radius);
		cloudsRaw = planet.CloudCoverFraction = Science.Planetology.GetCloudFraction(planet.Temperature,
												 planet.MolecularWeightRetained,
												 radius,
												 planet.WaterCoverFraction);
		planet.IceCoverFraction = Science.Planetology.GetIceFraction(planet.WaterCoverFraction, planet.Temperature);

		if (planet.HasGreenhouseEffect && surfpres.Millibars > 0.0)
		{
			planet.CloudCoverFraction = Ratio.FromDecimalFractions(1.0);
		}

		if (planet.DaytimeTemperature >= planet.BoilingPointWater && !first && !(Science.Planetology.TestIsTidallyLocked(planet.DayLength, planet.OrbitalPeriod) || planet.HasResonantPeriod))
		{
			planet.WaterCoverFraction = Ratio.FromDecimalFractions(0.0);
			boilOff = true;

			if (planet.MolecularWeightRetained.Grams > GlobalConstants.WATER_VAPOR)
			{
				planet.CloudCoverFraction = Ratio.FromDecimalFractions(0.0);
			}
			else
			{
				planet.CloudCoverFraction = Ratio.FromDecimalFractions(1.0);
			}
		}

		if (planet.Temperature.Kelvins < GlobalConstants.FREEZING_POINT_OF_WATER - 3.0)
		{
			planet.WaterCoverFraction = Ratio.FromDecimalFractions(0.0);
		}

		planet.Albedo = Science.Planetology.GetAlbedo(planet.WaterCoverFraction, planet.CloudCoverFraction, planet.IceCoverFraction, surfpres);

		effectiveTemp = Science.Thermodynamics.GetEstimatedEffectiveTemperature(planet.StellarBody.EcosphereRadius, planet.SemiMajorAxis, planet.Albedo);
		greenhouseTemp = Science.Thermodynamics.GetGreenhouseTemperatureRise(
			Science.Planetology.GetOpacity(planet.MolecularWeightRetained, surfpres),
			effectiveTemp, surfpres);
		planet.Temperature = effectiveTemp + greenhouseTemp;

		if (!first)
		{
			if (!boilOff)
			{
				planet.WaterCoverFraction = (planet.WaterCoverFraction + last_water * 2) / 3;
			}
			planet.CloudCoverFraction = (planet.CloudCoverFraction + last_clouds * 2) / 3;
			planet.IceCoverFraction = (planet.IceCoverFraction + last_ice * 2) / 3;
			planet.Albedo = (planet.Albedo + last_albedo * 2) / 3;
			planet.Temperature = Temperature.FromKelvins((planet.Temperature.Kelvins + last_temp.Kelvins * 2.0) / 3.0);
		}

		SetTempRange(surfpres, Temperature);
	}

	static double Lim(double x)
	{
		return x / Math.Sqrt(Math.Sqrt(1 + x * x * x * x));
	}

	static double Soft(double v, double max, double min)
	{
		double dv = v - min;
		double dm = max - min;
		return (Lim(2 * dv / dm - 1) + 1) / 2 * dm + min;
	}

	void SetTempRange(Pressure surfacePressure, Temperature temperature)
	{
		double surfpres = surfacePressure.Millibars;
		double temp = temperature.Kelvins; 

		var planet = this;

		var pressmod = 1 / Math.Sqrt(1 + 20 * surfpres / 1000.0);
		var ppmod = 1 / Math.Sqrt(10 + 5 * surfpres / 1000.0);
		var tiltmod = Math.Abs(Math.Cos(planet.AxialTilt.Degrees * Math.PI / 180) * Math.Pow(1 + planet.Eccentricity.DecimalFractions, 2));
		var daymod = 1 / (200 / planet.DayLength.Hours + 1);
		var mh = Math.Pow(1 + daymod, pressmod);
		var ml = Math.Pow(1 - daymod, pressmod);
		var hi = mh * temp;
		var lo = ml * temp;
		var sh = hi + Math.Pow((100 + hi) * tiltmod, Math.Sqrt(ppmod));
		var wl = lo - Math.Pow((150 + lo) * tiltmod, Math.Sqrt(ppmod));
		var max = temp + Math.Sqrt(temp) * 10;
		var min = temp / Math.Sqrt(planet.DayLength.Hours + 24);

		if (lo < min) lo = min;
		if (wl < 0) wl = 0;

		planet.DaytimeTemperature = Temperature.FromKelvins(Soft(hi, max, min));
		planet.NighttimeTemperature = Temperature.FromKelvins(Soft(lo, max, min));
		planet.MaxTemperature = Temperature.FromKelvins(Soft(sh, max, min));
		planet.MinTemperature = Temperature.FromKelvins(Soft(wl, max, min));
	}

}
