using System;
using UnitsNet;


namespace Primoris.Universe.Stargen.Astrophysics
{


	// TODO destroy this motherfucker
	public static class GlobalConstants
	{
		public static readonly double EARTH_SUN_TEMPERATURE = 5778.0d;

		public static readonly double RADIANS_PER_ROTATION = 2.0 * Math.PI;

		// Cosmic microwave background radiation temperature. Absolute minimum un space.
		public static readonly double VACCUM_TEMPERATURE = 2.7;

		public static readonly double ASTRONOMICAL_UNIT_KM = 1.4959787070E+08;
		public static readonly double CMSEC_TO_MSEC = 0.01;
		public static readonly double CMSEC_TO_KMSEC = 1.0 / 100000.0;
		public static readonly double CMSEC2_TO_MSEC2 = 0.01;
		public static readonly double GCM3_TO_KGM3 = 1000.0;

		public static readonly double SUN_AGE_IN_YEARS = 4600000000;
		public static readonly double ECCENTRICITY_COEFF = 0.077;                       // Dole's was 0.077			
		public static readonly Mass PROTOPLANET_MASS = Mass.FromSolarMasses(1.0E-15);                     // Units of solar masses	
		public static readonly double CHANGE_IN_EARTH_ANG_VEL = -1.3E-15;                    // Units of radians/sec/year
		public static readonly double SOLAR_MASS_IN_GRAMS = 1.989E33;                    // Units of grams			
		public static readonly double SOLAR_MASS_IN_KILOGRAMS = 1.989E30;                    // Units of kg				
		public static readonly double EARTH_MASS_IN_GRAMS = 5.977E27;                    // Units of grams
		public static readonly double EARTH_MASS_ATMOSPHERE_IN_GRAMS = 5.1480E21;       // Units of grams.
		public static readonly double EARTH_THICKNESS_ATMOSPHERE_KILOMETERS = 480.0;	// Units of km.
		public static readonly double EARTH_RADIUS = 6.378E8;                     // Units of cm				
		public static readonly double EARTH_DENSITY = 5.52;                        // Units of g/cc			
		public static readonly double KM_EARTH_RADIUS = 6378.0;                      // Units of km	
		public static readonly double KM_SUN_RADIUS = 695500.0;                     // Sun radius in KM.
		public static readonly double EARTH_ACCELERATION = 980.7;                       // Units of cm/sec2 (was 981.0)
		public static readonly double EARTH_AXIAL_TILT = 23.4;                        // Units of degrees			
		public static readonly double EARTH_EXOSPHERE_TEMP = 1273.0;                      // Units of degrees Kelvin (should be 1773).
		public static readonly double SUN_MASS_IN_EARTH_MASSES = 332775.64;
		public static readonly double ASTEROID_MASS_LIMIT = 0.001;                       // Units of Earth Masses	
		public static readonly double EARTH_EFFECTIVE_TEMP = 250.0;                       // Units of degrees Kelvin (was 255);	
		public static readonly double CLOUD_COVERAGE_FACTOR = 1.839E-8;                    // Km2/kg					
		public static readonly double EARTH_WATER_MASS_PER_AREA = 3.83E15;                     // grams per square km		
		public static readonly double EARTH_SURF_PRES_IN_MILLIBARS = 1013.25;
		public static readonly double EARTH_SURF_PRES_IN_MMHG = 760.0;                       // Dole p. 15				
		public static readonly double EARTH_SURF_PRES_IN_PSI = 14.696;                      // Pounds per square inch	
		public static readonly double MMHG_TO_MILLIBARS = EARTH_SURF_PRES_IN_MILLIBARS / EARTH_SURF_PRES_IN_MMHG;
		public static readonly double PSI_TO_MILLIBARS = EARTH_SURF_PRES_IN_MILLIBARS / EARTH_SURF_PRES_IN_PSI;
		public static readonly double H20_ASSUMED_PRESSURE = 47.0 * MMHG_TO_MILLIBARS;    // Dole p. 15      
		public static readonly double PPM_PRSSURE = EARTH_SURF_PRES_IN_MILLIBARS / 1000000.0;

		// Maximum inspired partial pressures in mmHg for common atmospheric gases - Dole pg. 15-16
		public static readonly double MIN_O2_IPP = Pressure.FromMillimetersOfMercury(72.0).Millibars;
		public static readonly double MAX_O2_IPP = Pressure.FromMillimetersOfMercury(400.0).Millibars; 
		public static readonly double MAX_HE_IPP = Pressure.FromMillimetersOfMercury(61000.0).Millibars; 
		public static readonly double MAX_NE_IPP = Pressure.FromMillimetersOfMercury(3900.0).Millibars; 
		public static readonly double MAX_N2_IPP = Pressure.FromMillimetersOfMercury(2330.0).Millibars; 
		public static readonly double MAX_AR_IPP = Pressure.FromMillimetersOfMercury(1220.0).Millibars; 
		public static readonly double MAX_KR_IPP = Pressure.FromMillimetersOfMercury(350.0).Millibars; 
		public static readonly double MAX_XE_IPP = Pressure.FromMillimetersOfMercury(160.0).Millibars; 
		public static readonly double MAX_CO2_IPP = Pressure.FromMillimetersOfMercury(7.0).Millibars; 
		public static readonly double MAX_HABITABLE_PRESSURE = Pressure.FromMillimetersOfMercury(118).Millibars; 

		// The next gases are listed as poisonous in parts per million by volume at 1 atm:
		public static readonly double MAX_F_IPP = UnitConversions.PPMToMillibars(0.1);
		public static readonly double MAX_CL_IPP = UnitConversions.PPMToMillibars(1.0);
		public static readonly double MAX_NH3_IPP = UnitConversions.PPMToMillibars(100.0);
		public static readonly double MAX_O3_IPP = UnitConversions.PPMToMillibars(0.1);
		public static readonly double MAX_CH4_IPP = UnitConversions.PPMToMillibars(50000.0);

		public static readonly double EARTH_CONVECTION_FACTOR = 0.43;                        // from Hart, eq.20			
		public static readonly double FREEZING_POINT_OF_WATER = 273.15;                      // Units of degrees Kelvin (was 273.0)
		public static readonly double EARTH_AVERAGE_CELSIUS = 14.0;                        // Average Earth Temperature (was 15.5)
		public static readonly double EARTH_AVERAGE_KELVIN = EARTH_AVERAGE_CELSIUS + FREEZING_POINT_OF_WATER;
		public static readonly double DAYS_IN_A_YEAR = 365.256;                     // Earth days per Earth year
		public static readonly double GAS_RETENTION_THRESHOLD = 6.0;                         // Ratio of esc vel to RMS vel (was 5.0)

		public static readonly double ICE_ALBEDO = 0.7;
		public static readonly double CLOUD_ALBEDO = 0.52;
		public static readonly double GAS_GIANT_ALBEDO = 0.5;                         // albedo of a gas giant	
		public static readonly double AIRLESS_ICE_ALBEDO = 0.5;
		public static readonly double EARTH_ALBEDO = 0.3;                         // was .33 for a while 
		public static readonly double GREENHOUSE_TRIGGER_ALBEDO = 0.20;
		public static readonly double ROCKY_ALBEDO = 0.15;
		public static readonly double ROCKY_AIRLESS_ALBEDO = 0.07;
		public static readonly double WATER_ALBEDO = 0.04;

		public static readonly double SECONDS_PER_HOUR = 3600.0;
		public static readonly double CM_PER_AU = 1.495978707E13;              // number of cm in an AU	
		public static readonly double CM_PER_KM = 1.0E5;                       // number of cm in a km		
		public static readonly double KM_PER_AU = CM_PER_AU / CM_PER_KM;
		public static readonly double CM_PER_METER = 100.0;
		public static readonly double MILLIBARS_PER_BAR = 1000.00;                     // was 1013.25

		public static readonly double GRAV_CONSTANT = 6.672E-8;                    // units of dyne cm2/gram2	
		public static readonly double MOLAR_GAS_CONST = 8314.41;                     // units: g*m2/ = (sec2*K*mol); 
		public const double K = 50.0;                        // K = gas/dust ratio. Was 50.0, is now 25.0 to allows for habitable planets.	
		public static readonly double B = 1.2E-5;                      // Used in Crit_mass calc	
		public const double DUST_DENSITY_COEFF = 0.002;                       // A in Dole's paper		
		public static readonly double ALPHA = 5.0;                         // Used in density calcs	
		public static readonly double N = 3.0;                         // Used in density calcs	
		public static readonly double J = 1.46E-19;                    // Used in day-length calcs (cm2/sec2 g) 
		public const double CLOUD_ECCENTRICITY = 0.25;

		// Now for a few molecular weights (used for RMS velocity calcs):
		// This table is from Dole's book "Habitable Planets for Man", p. 38
		public static readonly double ATOMIC_HYDROGEN = 1.0;                         // H
		public static readonly double MOL_HYDROGEN = 2.0;                         // H2
		public static readonly double HELIUM = 4.0;                         // He
		public static readonly double ATOMIC_NITROGEN = 14.0;                        // N
		public static readonly double ATOMIC_OXYGEN = 16.0;                        // O
		public static readonly double METHANE = 16.0;                        // CH4
		public static readonly double AMMONIA = 17.0;                        // NH3
		public static readonly double WATER_VAPOR = 18.0;                        // H2O
		public static readonly double NEON = 20.2;                        // Ne
		public static readonly double MOL_NITROGEN = 28.0;                        // N2
		public static readonly double CARBON_MONOXIDE = 28.0;                        // CO
		public static readonly double NITRIC_OXIDE = 30.0;                        // NO
		public static readonly double MOL_OXYGEN = 32.0;                        // O2
		public static readonly double HYDROGEN_SULPHIDE = 34.1;                        // H2S
		public static readonly double ARGON = 39.9;                        // Ar
		public static readonly double CARBON_DIOXIDE = 44.0;                        // CO2
		public static readonly double NITROUS_OXIDE = 44.0;                        // N2O
		public static readonly double NITROGEN_DIOXIDE = 46.0;                        // NO2
		public static readonly double OZONE = 48.0;                        // O3
		public static readonly double SULPH_DIOXIDE = 64.1;                        // SO2
		public static readonly double SULPH_TRIOXIDE = 80.1;                        // SO3
		public static readonly double KRYPTON = 83.8;                        // Kr
		public static readonly double XENON = 131.3;                       // Xe

		//	And atomic numbers, for use in ChemTable indexes
		public static readonly int AN_H = 1;
		public static readonly int AN_HE = 2;
		public static readonly int AN_N = 7;
		public static readonly int AN_O = 8;
		public static readonly int AN_F = 9;
		public static readonly int AN_NE = 10;
		public static readonly int AN_P = 15;
		public static readonly int AN_CL = 17;
		public static readonly int AN_AR = 18;
		public static readonly int AN_BR = 35;
		public static readonly int AN_KR = 36;
		public static readonly int AN_I = 53;
		public static readonly int AN_XE = 54;
		public static readonly int AN_HG = 80;
		public static readonly int AN_AT = 85;
		public static readonly int AN_RN = 86;
		public static readonly int AN_FR = 87;
		public static readonly int AN_NH3 = 900;
		public static readonly int AN_H2O = 901;
		public static readonly int AN_CO2 = 902;
		public static readonly int AN_O3 = 903;
		public static readonly int AN_CH4 = 904;
		public static readonly int AN_CH3CH2OH = 905;

		// The following defines are used in the kothari_radius function in
		// file enviro.c.
		public static readonly double A1_20 = 6.485E12;                    // All units are in cgs system.
		public static readonly double A2_20 = 4.0032E-8;                   // ie: cm, g, dynes, etc.
		public static readonly double BETA_20 = 5.71E12;
		public static readonly double JIMS_FUDGE = 1.004;

		// The following defines are used in determining the fraction of a planet
		// covered with clouds in function cloud_fraction in file enviro.c.
		public static readonly double Q1_36 = 1.258E19;                    // grams
		public static readonly double Q2_36 = 0.0698;                      // 1/Kelvin

		public static readonly double NOT_APPLICABLE = double.MaxValue; //(9.9999E37);
		public static readonly double OUT_OF_RANGE = 0.0;
	}
}
