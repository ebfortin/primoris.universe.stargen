using System;
using System.Linq;
using System.Management.Automation;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Primoris.Universe.Stargen.Systems;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Bodies.Burrows;
using Primoris.Universe.Stargen.Systems.Burrows;
using UnitsNet;


namespace Primoris.Universe.Stargen.Cmdlets
{
    internal class PlanetMap : ClassMap<SatelliteBody>
	{
		public PlanetMap()
		{
			Map(m => m.Atmosphere).Ignore();
			Map(m => m.SemiMajorAxis);
			Map(m => m.Eccentricity);
			Map(m => m.AxialTilt);
			Map(m => m.OrbitZone);
			Map(m => m.OrbitalPeriod);
			Map(m => m.AngularVelocity);
			Map(m => m.DayLength);
			Map(m => m.HillSphere);
			Map(m => m.Mass);
			Map(m => m.DustMass);
			Map(m => m.GasMass);
			Map(m => m.EscapeVelocity);
			Map(m => m.SurfaceAcceleration);
			Map(m => m.CoreRadius);
			Map(m => m.Radius);
			Map(m => m.Density);
			Map(m => m.Type);
			Map(m => m.IsTidallyLocked);
			Map(m => m.IsEarthlike);
			Map(m => m.IsHabitable);
			Map(m => m.HasResonantPeriod);
			Map(m => m.HasGreenhouseEffect);
			Map(m => m.RMSVelocity);
			Map(m => m.MolecularWeightRetained);
			Map(m => m.VolatileGasInventory);
			Map(m => m.BoilingPointWater);
			Map(m => m.Albedo);
			Map(m => m.Illumination);
			Map(m => m.ExosphereTemperature);
			Map(m => m.Temperature);
			Map(m => m.GreenhouseRiseTemperature);
			Map(m => m.DaytimeTemperature);
			Map(m => m.NighttimeTemperature);
			Map(m => m.MaxTemperature);
			Map(m => m.WaterCoverFraction);
			Map(m => m.CloudCoverFraction);
			Map(m => m.IceCoverFraction);
		}
	}


	[Cmdlet(VerbsCommon.Get, "Planets")]
	public class PlanetsCmdlet : StarCmdlet
	{
		[Parameter]
		public double DustDensityCoeff { get; set; } = GlobalConstants.DUST_DENSITY_COEFF;

		[Parameter]
		public double GasDensityRatio { get; set; } = GlobalConstants.K;

		[Parameter]
		public double CloudEccentricity { get; set; } = GlobalConstants.CLOUD_ECCENTRICITY;

		[Parameter]
		public bool OnlyHabitableSystem { get; set; } = false;

        private SatelliteBody CreatePlanet(Seed seed,
                                        StellarBody star,
										int pos,
                                        string planetID)
        {
			return new Planet(seed, star, star) { Position = pos };
        }

		protected override void ProcessRecord()
		{
            var sun = GenerateStar();
			bool findsys;

			do
			{
                sun.BodyFormationScience = new Accrete(Ratio.FromDecimalFractions(CloudEccentricity),
													   Ratio.FromDecimalFractions(GasDensityRatio),
													   Ratio.FromDecimalFractions(DustDensityCoeff));
                sun.GenerateSystem(CreatePlanet);
                findsys = (from p in sun.Satellites where p.IsHabitable select p).Count() > 0;
			} while (!findsys && OnlyHabitableSystem);

			if (String.IsNullOrEmpty(CsvOutputPath))
            {
                WriteObject(sun);
                WriteObject(sun.Satellites);
            }
            else
            {
				var conf = new CsvHelper.Configuration.Configuration();
				conf.RegisterClassMap<PlanetMap>();

				var f = new FileStream(CsvOutputPath, FileMode.Create);
				var w = new StreamWriter(f);
				var cw = new CsvWriter(w, conf);

				cw.WriteHeader<SatelliteBody>();
				cw.NextRecord();
				cw.WriteRecords<SatelliteBody>(sun.Satellites);
				cw.Flush();

				w.Close();
			}
		}
	}
}
