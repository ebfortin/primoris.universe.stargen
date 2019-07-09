using System;
using System.Management.Automation;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Primoris.Universe.Stargen.Systems;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Systems.Burrows;

namespace Primoris.Universe.Stargen.Cmdlets
{
	internal class PlanetMap : ClassMap<Body>
	{
		public PlanetMap()
		{
			Map(m => m.Atmosphere).Ignore();
			Map(m => m.SemiMajorAxisAU);
			Map(m => m.Eccentricity);
			Map(m => m.AxialTilt);
			Map(m => m.OrbitZone);
			Map(m => m.OrbitalPeriod);
			Map(m => m.AngularVelocityRadSec);
			Map(m => m.DayLength);
			Map(m => m.HillSphere);
			Map(m => m.MassSM);
			Map(m => m.DustMassSM);
			Map(m => m.GasMassSM);
			Map(m => m.EscapeVelocityCMSec);
			Map(m => m.SurfaceAccelerationCMSec2);
			Map(m => m.SurfaceGravityG);
			Map(m => m.CoreRadius);
			Map(m => m.Radius);
			Map(m => m.DensityGCC);
			Map(m => m.Type);
			Map(m => m.IsTidallyLocked);
			Map(m => m.IsEarthlike);
			Map(m => m.IsHabitable);
			Map(m => m.HasResonantPeriod);
			Map(m => m.HasGreenhouseEffect);
			Map(m => m.RMSVelocityCMSec);
			Map(m => m.MolecularWeightRetained);
			Map(m => m.VolatileGasInventory);
			Map(m => m.BoilingPointWater);
			Map(m => m.Albedo);
			Map(m => m.Illumination);
			Map(m => m.ExosphereTemperature);
			Map(m => m.SurfaceTemperature);
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

		protected override void ProcessRecord()
		{
            var sun = GenerateStar();

            var sys = SystemGenerator.GenerateStellarSystem(Name, new SystemGenerationOptions(DustDensityCoeff, CloudEccentricity, GasDensityRatio), sun : sun);
            if (String.IsNullOrEmpty(CsvOutputPath))
            {
                WriteObject(sun);
                WriteObject(sys.Planets);
            }
            else
            {
				var conf = new CsvHelper.Configuration.Configuration();
				conf.RegisterClassMap<PlanetMap>();

				var f = new FileStream(CsvOutputPath, FileMode.Create);
				var w = new StreamWriter(f);
				var cw = new CsvWriter(w, conf);

				cw.WriteHeader<Body>();
				cw.NextRecord();
				cw.WriteRecords<Body>(sys.Planets);
				cw.Flush();

				w.Close();
			}
		}
	}
}
