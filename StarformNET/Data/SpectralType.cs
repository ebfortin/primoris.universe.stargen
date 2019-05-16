using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace DLS.StarformNET.Data
{
	public class SpectralType
	{
		#region Static Constructor
		static SpectralType()
		{
			// Star temperature data from Lang's _Astrophysical Data: Planets and Stars_
			// Temperatures from missing (and typically not used) types in those
			// tables were just interpolated.

			StarTemperatures.Add(SpectralClass.O, new List<double[]>());
			StarTemperatures[SpectralClass.O].Add(new double[] { 52500, 52500, 52500, 52500, 48000, 44500, 41000, 38000, 35800, 33000 });
			StarTemperatures[SpectralClass.O].Add(new double[] { 50000, 50000, 50000, 50000, 45500, 42500, 39500, 37000, 34700, 32000 });
			StarTemperatures[SpectralClass.O].Add(new double[] { 47300, 47300, 47300, 47300, 44100, 42500, 39500, 37000, 34700, 32000 });

			StarTemperatures.Add(SpectralClass.B, new List<double[]>());
			StarTemperatures[SpectralClass.B].Add(new double[] { 30000, 25400, 22000, 18700, 17000, 15400, 14000, 13000, 11900, 10500 });
			StarTemperatures[SpectralClass.B].Add(new double[] { 29000, 24000, 20300, 17100, 16000, 15000, 14100, 13200, 12400, 11000 });
			StarTemperatures[SpectralClass.B].Add(new double[] { 26000, 20800, 18500, 16200, 15100, 13600, 13000, 12200, 11200, 10300 });

			StarTemperatures.Add(SpectralClass.A, new List<double[]>());
			StarTemperatures[SpectralClass.A].Add(new double[] { 9520, 9230, 8970, 8720, 8460, 8200, 8020, 7850, 7580, 7390 });
			StarTemperatures[SpectralClass.A].Add(new double[] { 10100, 9480, 9000, 8600, 8300, 8100, 7850, 7650, 7450, 7250 });
			StarTemperatures[SpectralClass.A].Add(new double[] { 9730, 9230, 9080, 8770, 8610, 8510, 8310, 8150, 7950, 7800 });

			StarTemperatures.Add(SpectralClass.F, new List<double[]>());
			StarTemperatures[SpectralClass.F].Add(new double[] { 7200, 7050, 6890, 6740, 6590, 6440, 6360, 6280, 6200, 6110 });
			StarTemperatures[SpectralClass.F].Add(new double[] { 7150, 7000, 6870, 6720, 6570, 6470, 6350, 6250, 6150, 6080 });
			StarTemperatures[SpectralClass.F].Add(new double[] { 7700, 7500, 7350, 7150, 7000, 6900, 6500, 6300, 6100, 5800 });

			StarTemperatures.Add(SpectralClass.G, new List<double[]>());
			StarTemperatures[SpectralClass.G].Add(new double[] { 6030, 5940, 5860, 5830, 5800, 5770, 5700, 5630, 5570, 5410 });
			StarTemperatures[SpectralClass.G].Add(new double[] { 5850, 5650, 5450, 5350, 5250, 5150, 5050, 5070, 4900, 4820 });
			StarTemperatures[SpectralClass.G].Add(new double[] { 5550, 5350, 5200, 5050, 4950, 4850, 4750, 4660, 4600, 4500 });

			StarTemperatures.Add(SpectralClass.K, new List<double[]>());
			StarTemperatures[SpectralClass.K].Add(new double[] { 5250, 5080, 4900, 4730, 4590, 4350, 4200, 4060, 3990, 3920 });
			StarTemperatures[SpectralClass.K].Add(new double[] { 4750, 4600, 4420, 4200, 4000, 3950, 3900, 3850, 3830, 3810 });
			StarTemperatures[SpectralClass.K].Add(new double[] { 4420, 4330, 4250, 4080, 3950, 3850, 3760, 3700, 3680, 3660 });

			StarTemperatures.Add(SpectralClass.M, new List<double[]>());
			StarTemperatures[SpectralClass.M].Add(new double[] { 3850, 3720, 3580, 3470, 3370, 3240, 3050, 2940, 2640, 2000 });
			StarTemperatures[SpectralClass.M].Add(new double[] { 3800, 3720, 3620, 3530, 3430, 3330, 3240, 3240, 3240, 3240 });
			StarTemperatures[SpectralClass.M].Add(new double[] { 3650, 3550, 3450, 3200, 2980, 2800, 2600, 2600, 2600, 2600 });

			StarTemperatures.Add(SpectralClass.WN, new List<double[]>());
			StarTemperatures[SpectralClass.WN].Add(new double[] { 50000, 50000, 50000, 50000, 47000, 43000, 39000, 32000, 29000, 29000 });

			StarTemperatures.Add(SpectralClass.WC, new List<double[]>());
			StarTemperatures[SpectralClass.WC].Add(new double[] { 60000, 60000, 60000, 60000, 60000, 60000, 60000, 54000, 46000, 38000 });

			StarTemperatures.Add(SpectralClass.L, new List<double[]>());
			StarTemperatures[SpectralClass.L].Add(new double[] { 1960, 1930, 1900, 1850, 1800, 1740, 1680, 1620, 1560, 1500 });

			StarTemperatures.Add(SpectralClass.T, new List<double[]>());
			StarTemperatures[SpectralClass.T].Add(new double[] { 1425, 1350, 1275, 1200, 1140, 1080, 1020, 900, 800, 750 });

			StarTemperatures.Add(SpectralClass.H, new List<double[]>());
			StarTemperatures[SpectralClass.H].Add(new double[] { 500, 450, 400, 350, 300, 250, 200, 150, 100, 0 });

			StarTemperatures.Add(SpectralClass.Y, new List<double[]>());
			StarTemperatures[SpectralClass.Y].Add(new double[] { 750, 700, 650, 600, 550, 500, 450, 400, 300, 273 });

			StarTemperatures.Add(SpectralClass.I, new List<double[]>());
			StarTemperatures[SpectralClass.I].Add(new double[] { 1000000, 900000, 800000, 700000, 600000, 500000, 400000, 300000, 200000, 100000 });

			StarTemperatures.Add(SpectralClass.E, new List<double[]>());
			StarTemperatures[SpectralClass.E].Add(new double[] { 10000000, 9000000, 8000000, 7000000, 6000000, 5000000, 4000000, 3000000, 2000000, 1000000 });

			StarTemperatures.Add(SpectralClass.WD, new List<double[]>());
			StarTemperatures[SpectralClass.WD].Add(new double[] { 100000.0f, 50400.0f, 25200.0f, 16800.0f, 12600.0f, 10080.0f, 8400.0f, 7200.0f, 6300.0f, 5600.0f });
		}
		#endregion

		private static Dictionary<SpectralClass, List<double[]>> StarTemperatures { get; }
		public SpectralClass SpectralClass { get; private set; }
		public int SubType { get; private set; }
		public LuminosityClass LuminosityClass { get; private set; }
		public double Temperature { get; private set; }

		public SpectralType(SpectralClass sc, LuminosityClass lc, int subType = 0, double temp = 0.0)
		{
			SpectralClass = sc;
			LuminosityClass = lc;
			SubType = subType;
			Temperature = temp;
		}

		private SpectralType() { }

		public static SpectralType FromTemperature(double eff_temp, double luminosity = 0.0)
		{
			string[] classes = { "x", "WN", "O", "B", "A", "F", "G", "K", "M", "L", "T", "Y" };
			double[] tclass = { 200000.0, 52000.0, 30000.0, 10000.0, 7500.0, 6000.0, 5000.0, 3500.0, 2000.0, 1300.0, 700.0, 0.0 };

			SpectralClass aclass = SpectralClass.Undefined;
			int subType = 0;
			LuminosityClass clum = LuminosityClass.Undefined;

			if (luminosity == 0.0)
				luminosity = 0.0000001;

			double xmag = 4.83 - (2.5 * (Math.Log(luminosity) / Math.Log(10.0)));

			// Determine spectral class.
			for (int i = 1; i < tclass.Length; i++)
			{
				if (eff_temp > tclass[i])
				{
					string ac = classes[i];
					double csiz = tclass[i - 1] - tclass[i];
					double cdel = eff_temp - tclass[i];
					double cfrac = cdel / csiz;
					double dt = 10.0 - (10.0 * cfrac);
					if (dt < 0.0)
					{
						dt = 0.0;
					}

					subType = (int)Math.Floor(dt);
					aclass = (SpectralClass)Enum.Parse(typeof(SpectralClass), ac);
					
					break;
				}
			}

			// Determine luminosity class.
			switch(aclass)
			{
				case SpectralClass.O:
					if (xmag < -9)
						clum = LuminosityClass.O;
					else if (xmag < -7)
						clum = LuminosityClass.Ia;
					else if (xmag < -6)
						clum = LuminosityClass.Ib;
					else if (xmag < -4.9)
						clum = LuminosityClass.II;
					else if (xmag < -4)
						clum = LuminosityClass.III;
					else
						clum = LuminosityClass.V;
					break;
				case SpectralClass.B:
					if (xmag < -9)
						clum = LuminosityClass.O;
					else if (xmag < -7)
						clum = LuminosityClass.Ia;
					else if (xmag < -5)
						clum = LuminosityClass.Ib;
					else if (xmag < -4.5)
						clum = LuminosityClass.II;
					else if (xmag < -0.5)
						clum = LuminosityClass.III;
					else
						clum = LuminosityClass.V;
					break;
				case SpectralClass.A:
					if (xmag < -9)
						clum = LuminosityClass.O;
					else if (xmag < -7)
						clum = LuminosityClass.Ia;
					else if (xmag < -4.5)
						clum = LuminosityClass.Ib;
					else if (xmag < -2.25)
						clum = LuminosityClass.II;
					else if (xmag < 0)
						clum = LuminosityClass.III;
					else if (xmag < 0.125)
						clum = LuminosityClass.IV;
					else
						clum = LuminosityClass.V;
					break;
				case SpectralClass.F:
				case SpectralClass.G:
					if (xmag < -9)
						clum = LuminosityClass.O;
					else if (xmag < -7)
						clum = LuminosityClass.Ia;
					else if (xmag < -4.5)
						clum = LuminosityClass.Ib;
					else if (xmag < -2)
						clum = LuminosityClass.II;
					else if (xmag < 1.75)
						clum = LuminosityClass.III;
					else if (xmag < 3)
						clum = LuminosityClass.IV;
					else
						clum = LuminosityClass.V;
					break;
				case SpectralClass.K:
					if (xmag < -9)
						clum = LuminosityClass.O;
					else if (xmag < -7)
						clum = LuminosityClass.Ia;
					else if (xmag < -4.5)
						clum = LuminosityClass.Ib;
					else if (xmag < -2)
						clum = LuminosityClass.II;
					else if (xmag < 2)
						clum = LuminosityClass.III;
					else if (xmag < 4)
						clum = LuminosityClass.IV;
					else
						clum = LuminosityClass.V;
					break;
				case SpectralClass.M:
					if (xmag < -9)
						clum = LuminosityClass.O;
					else if (xmag < -7)
						clum = LuminosityClass.Ia;
					else if (xmag < -4.5)
						clum = LuminosityClass.Ib;
					else if (xmag < -2)
						clum = LuminosityClass.II;
					else if (xmag < 2.5)
						clum = LuminosityClass.III;
					else
						clum = LuminosityClass.V;
					break;
				default:
					clum = LuminosityClass.V;
					break;
			}

			return new SpectralType(aclass, clum, subType, eff_temp);
		}

		public static SpectralType FromString(string st)
		{
			if (String.IsNullOrEmpty(st))
				return new SpectralType(SpectralClass.Undefined, LuminosityClass.Undefined);

			try
			{
				var stc = new SpectralType();

				var mt = Regex.Match(st, @"(\D*)(\d*)(\D*)");
				stc.SpectralClass = (SpectralClass)Enum.Parse(typeof(SpectralClass), mt.Groups[0].Value);
				stc.LuminosityClass = (LuminosityClass)Enum.Parse(typeof(LuminosityClass), mt.Groups[2].Value);

				int lumIndex = GetLuminosityIndex(stc.LuminosityClass);
				SpectralClass starType = GetStarType(stc.SpectralClass);
				stc.SubType = Int32.Parse(mt.Groups[1].Value);

				stc.Temperature = StarTemperatures[starType][lumIndex][stc.SubType];

				return stc;
			}
			catch (Exception)
			{
				return new SpectralType(SpectralClass.Undefined, LuminosityClass.Undefined);
			}
		}

		private static SpectralClass GetStarType(SpectralClass c)
		{
			switch(c)
			{
				case SpectralClass.DA:
				case SpectralClass.DB:
				case SpectralClass.DC:
				case SpectralClass.DO:
				case SpectralClass.DQ:
				case SpectralClass.DZ:
					return SpectralClass.WD;
				case SpectralClass.WN:
					return SpectralClass.WN;
				case SpectralClass.WC:
					return SpectralClass.WC;
				case SpectralClass.O:
					return SpectralClass.O;
				case SpectralClass.B:
					return SpectralClass.B;
				case SpectralClass.A:
					return SpectralClass.A;
				case SpectralClass.F:
					return SpectralClass.F;
				case SpectralClass.G:
					return SpectralClass.G;
				case SpectralClass.K:
				case SpectralClass.R:
					return SpectralClass.K;
				case SpectralClass.M:
				case SpectralClass.S:
				case SpectralClass.N:
				case SpectralClass.C:
					return SpectralClass.M;
				case SpectralClass.L:
					return SpectralClass.L;
				case SpectralClass.T:
					return SpectralClass.T;
				case SpectralClass.Y:
					return SpectralClass.Y;
				case SpectralClass.H:
					return SpectralClass.H;
				case SpectralClass.E:
					return SpectralClass.E;
				case SpectralClass.I:
					return SpectralClass.I;
				default:
					throw new ArgumentException();
			}
		}

		private static int GetLuminosityIndex(LuminosityClass c)
		{
			switch(c)
			{
				case LuminosityClass.Ia:
				case LuminosityClass.Ib:
				case LuminosityClass.II:
					return 2;
				case LuminosityClass.III:
				case LuminosityClass.IV:
					return 1;
				default:
					return 0;
			}
		}

		public override string ToString()
		{
			return Enum.GetName(typeof(SpectralClass), SpectralClass) + SubType.ToString() + Enum.GetName(typeof(LuminosityClass), LuminosityClass);
		}
	}
}
