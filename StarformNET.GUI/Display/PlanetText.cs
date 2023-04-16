using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Bodies;

using UnitsNet;

namespace Primoris.Universe.Stargen.Display
{


	public static class PlanetText
    {
        public static string GetSystemText(IEnumerable<SatelliteBody> planets)
        {
            var sb = new StringBuilder();
            var sun = planets.ElementAt(0).StellarBody;
            sb.AppendLine(StarText.GetFullStarTextRelative(sun, true));
            sb.AppendLine();

            foreach (var p in planets)
            {
                sb.AppendLine(GetPlanetText(p));
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }
        public static string GetPlanetText(SatelliteBody planet)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1}", GetPlanetNumber(planet), GetPlanetTypeText(planet));
            sb.AppendLine();
            sb.AppendLine("-------------------------");
            sb.AppendLine();
            sb.AppendFormat("Orbital Distance: {0}\n", GetOrbitalDistanceAU(planet));
            sb.AppendLine();
            sb.AppendFormat("Equatorial Radius: {0}\n", GetRadiusER(planet));
            sb.AppendLine();
            sb.AppendFormat("Surface Gravity: {0}\n", GetSurfaceGravityG(planet));
            sb.AppendLine();
            sb.AppendFormat("Escape Velocity: {0}\n", GetEscapeVelocity(planet));
            sb.AppendLine();
            sb.AppendFormat("Mass: {0}\n", GetMassStringEM(planet));
            sb.AppendLine();
            sb.AppendFormat("Density: {0}\n", GetDensity(planet));
            sb.AppendLine();
            sb.AppendFormat("Length of Year: {0}\n", GetOrbitalPeriodDay(planet));
            sb.AppendLine();
            sb.AppendFormat("Length of Day: {0}\n", GetLengthofDayHours(planet));
            sb.AppendLine();
            sb.AppendFormat("Average Day Temperature: {0}\n", GetDayTemp(planet));
            sb.AppendLine();
            sb.AppendFormat("Average Night Temperature: {0}\n", GetNightTemp(planet));
            sb.AppendLine();
            sb.AppendFormat("Boiling Point: {0}\n", GetBoilingPoint(planet));
            sb.AppendLine();
            sb.AppendFormat("Greenhouse Rise: {0}\n", GetGreenhouseRise(planet));
            sb.AppendLine();
            sb.AppendFormat("Water Cover: {0}\n", GetHydrosphere(planet));
            sb.AppendLine();
            sb.AppendFormat("Ice Cover: {0}\n", GetIceCover(planet));
            sb.AppendLine();
            sb.AppendFormat("Cloud Cover: {0}\n", GetCloudCover(planet));
            sb.AppendLine();
            sb.AppendFormat("Surface Pressure: {0}\n", GetSurfacePressureStringAtm(planet));
            sb.AppendLine();
            sb.AppendFormat("Atmospheric Composition (Percentage): {0}\n", GetAtmoString(planet));
            sb.AppendLine();
            sb.AppendFormat("Atmospheric Composition (Partial Pressure): {0}\n", GetAtmoStringPP(planet));

            return sb.ToString();
        }

        public static string GetDensity(SatelliteBody planet)
        {
            return String.Format("{0:0.00} g/cm3", planet.Density.GramsPerCubicCentimeter);
        }

        public static string GetBoilingPoint(SatelliteBody planet)
        {
            if (planet.Type == BodyType.GasGiant || planet.Type == BodyType.SubGasGiant || planet.Type == BodyType.SubSubGasGiant)
            {
                return "-";
            }
            return String.Format("{0:0.00} F", planet.BoilingPointWater.DegreesFahrenheit);
        }

        public static string GetGreenhouseRise(SatelliteBody planet)
        {
            return String.Format("{0:0.00} F", planet.GreenhouseRiseTemperature.DegreesFahrenheit);
        }

        public static string GetEscapeVelocity(SatelliteBody planet)
        {
            return String.Format("{0:0.00} km/sec", planet.EscapeVelocity.KilometersPerSecond);
        }

        public static string GetPlanetTypeText(SatelliteBody planet)
        {
            var sb = new StringBuilder();
            switch (planet.Type)
            {
                case BodyType.GasGiant:
                    sb.Append("Gas Giant");
                    break;
                case BodyType.SubGasGiant:
                    sb.Append("Small Gas Giant");
                    break;
                case BodyType.SubSubGasGiant:
                    sb.Append("Gas Dwarf");
                    break;
                default:
                    sb.Append(planet.Type);
                    break;
            }
            if (planet.HasResonantPeriod)
            {
                sb.Append(", Resonant Orbital Period");
            }
            if (planet.IsTidallyLocked)
            {
                sb.Append(", Tidally Locked");
            }
            if (planet.SurfacePressure.Millibars > 0 && planet.HasGreenhouseEffect)
            {
                sb.Append(", Runaway Greenhouse Effect");
            }
            switch (planet.Breathability)
            {
                case Breathability.Breathable:
                case Breathability.Unbreathable:
                case Breathability.Poisonous:
                    sb.AppendFormat(", {0} Atmosphere", planet.Breathability);
                    break;
                default:
                    sb.Append(", No Atmosphere");
                    break;
            }
            if (planet.IsEarthlike)
            {
                sb.Append(", Earthlike");
            }
            return sb.ToString();
        }

        public static string GetSurfaceGravityG(SatelliteBody planet)
        {
            if (planet.Type == BodyType.GasGiant || planet.Type == BodyType.SubGasGiant || planet.Type == BodyType.SubSubGasGiant)
            {
                return "Oh yeah";
            }
            return String.Format("{0:0.00} G", planet.SurfaceAcceleration.StandardGravity);
        }

        public static string GetHydrosphere(SatelliteBody planet)
        {
            return String.Format("{0:0.0}%", planet.WaterCoverFraction * 100);
        }

        public static string GetIceCover(SatelliteBody planet)
        {
            return String.Format("{0:0.0}%", planet.IceCoverFraction * 100);
        }

        public static string GetCloudCover(SatelliteBody planet)
        {
            return String.Format("{0:0.0}%", planet.CloudCoverFraction * 100);
        }

        public static string GetDayTemp(SatelliteBody planet)
        {
            return String.Format("{0:0.0} F", planet.DaytimeTemperature.DegreesFahrenheit);
        }

        public static string GetNightTemp(SatelliteBody planet)
        {
            return String.Format("{0:0.0} F", planet.NighttimeTemperature.DegreesFahrenheit);
        }

        public static string GetExoTemp(SatelliteBody planet)
        {
            return String.Format("{0:0.0} K", planet.ExosphereTemperature);
        }

        public static string GetEstimatedHillSphereKM(SatelliteBody planet)
        {
            return String.Format("{0:n0} km", planet.HillSphere);
        }

        public static string GetLengthofDayHours(SatelliteBody planet)
        {
            if (planet.DayLength.Hours > 24 * 7)
            {
                return string.Format("{0:0.0} days ({1:0.0} hours)", planet.DayLength / 24, planet.DayLength);
            }
            return String.Format("{0:0.0} hours", planet.DayLength);
        }

        public static string GetOrbitalPeriodDay(SatelliteBody planet)
        {
            if (planet.OrbitalPeriod.Days > 365 * 1.5)
            {
                return String.Format("{0:0.00} ({0:0.0} days)", planet.OrbitalPeriod / 365, planet.OrbitalPeriod);
            }
            return String.Format("{0:0.0} days", planet.OrbitalPeriod);
        }

        public static string GetOrbitalEccentricity(SatelliteBody planet)
        {
            return String.Format("{0:0.00}", planet.Eccentricity);
        }

        public static string GetOrbitalDistanceAU(SatelliteBody planet)
        {
            return String.Format("{0:0.00} AU", planet.SemiMajorAxis.AstronomicalUnits);
        }

        public static string GetPlanetNumber(SatelliteBody planet)
        {
            return String.Format("{0}.", planet.Position);
        }

        public static string GetRadiusKM(SatelliteBody planet)
        {
            return String.Format("{0:0} km", planet.Radius);
        }

        public static string GetRadiusER(SatelliteBody planet)
        {
            return String.Format("{0:0.00} ER", planet.Radius / GlobalConstants.KM_EARTH_RADIUS);
        }

        public static string GetMassStringEM(SatelliteBody planet)
        {
            return String.Format("{0:0.00} EM", planet.Mass.EarthMasses);
        }

        public static string GetSurfacePressureStringAtm(SatelliteBody planet)
        {
            if (planet.Type == BodyType.GasGiant || planet.Type == BodyType.SubGasGiant || planet.Type == BodyType.SubSubGasGiant)
            {
                return "Uh, a lot";
            }
            return String.Format("{0:0.000} atm", planet.SurfacePressure.Atmospheres);
        }

        public static string GetAtmoStringPP(SatelliteBody planet)
        {
            if (planet.Type == BodyType.GasGiant || planet.Type == BodyType.SubGasGiant || planet.Type == BodyType.SubSubGasGiant)
            {
                return "Yes";
            }
            if (planet.AtmosphereComposition.Count() == 0)
            {
                return "None";
            }
            var str = "";
            var orderedGases = planet.AtmosphereComposition.OrderByDescending(g => g.Item2).ToArray();
            if (orderedGases.Length == 0)
            {
                return "Trace gases only";
            }
            for (var i = 0; i < orderedGases.Length; i++)
            {
                var gas = orderedGases[i];
                var curGas = gas.Item1;
                str += String.Format("{0} [{1:0.0000} mb]", curGas.Symbol, gas.Item2);
                if (i < orderedGases.Length - 1)
                {
                    str += ", ";
                }
            }
            return str;
        }

		// TODO: FIX. Doesn't work since changing to Ratio instead of Pressure for gas amount in atmosphere.
        public static string GetPoisonString(SatelliteBody planet)
        {
            var str = "";
            var orderedGases = planet.AtmospherePoisonousComposition.OrderByDescending(g => g.Item2.DecimalFractions).ToList();
            for (var i = 0; i < orderedGases.Count(); i++)
            {
                if (orderedGases[i].Item2.DecimalFractions > 1)
                {
                    str += String.Format("{0:0.0000}mb {1}", orderedGases[i].Item2.DecimalFractions, orderedGases[i].Item1.Symbol);
                }
                else
                {
					var mb = Pressure.FromMillibars(orderedGases[i].Item2.DecimalFractions);
                    var ppm = (mb / (1013.25 * 1.0)) * 1000000;
					str += String.Format("{0:0.0000}ppm {1}", ppm, orderedGases[i].Item1.Symbol);
                }
                if (i < orderedGases.Count() - 1)
                {
                    str += ", ";
                }
            }
            return str;
        }

        public static string GetAtmoString(SatelliteBody planet, double minFraction = 1.0)
        {
            if (planet.Type == BodyType.GasGiant || planet.Type == BodyType.SubGasGiant || planet.Type == BodyType.SubSubGasGiant)
            {
                return "Yes";
            }
            if (planet.AtmosphereComposition.Count() == 0)
            {
                return "None";
            }
            if (planet.SurfacePressure.Millibars < 0.0005)
            {
                return "Almost None";
            }

            var str = "";
            var orderedGases = planet.AtmosphereComposition.Where(g => ((g.Item2.DecimalFractions) * 100) > minFraction).OrderByDescending(g => g.Item2.DecimalFractions).ToArray();
            for (var i = 0; i < orderedGases.Length; i++)
            {
                var gas = orderedGases[i];
                var curGas = gas.Item1;
                var pct = (gas.Item2.DecimalFractions) * 100;
                str += String.Format("{0:0.0}% {1}", pct, curGas.Symbol);
                if (i < orderedGases.Length - 1)
                {
                    str += ", ";
                }
            }
            if (orderedGases.Length < planet.AtmosphereComposition.Count())
            {
                var traceGasSum = 0.0;
                foreach (var gas in planet.AtmosphereComposition)
                {
                    var frac = (gas.Item2.DecimalFractions) * 100;
                    if (frac <= minFraction)
                    {
                        traceGasSum += frac;
                    }
                }
                if (traceGasSum > 0.05)
                {
                    str += String.Format(", {0:0.0}% trace gases", traceGasSum);
                }
            }
            return str;
        }
    }
}
