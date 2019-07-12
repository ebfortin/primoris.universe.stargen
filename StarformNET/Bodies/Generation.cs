namespace Primoris.Universe.Stargen.Bodies
{
	public class Generation
	{
		public DustRecord Dusts { get; set; }
		public Seed Bodies { get; set; }
		public Generation Next { get; set; }
	}
}
