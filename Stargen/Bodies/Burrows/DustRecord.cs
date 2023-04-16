namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	internal class DustRecord
	{
		public double InnerEdge;
		public double OuterEdge;
		public bool DustPresent;
		public bool GasPresent;
		public DustRecord? NextBand = null;
	}
}
