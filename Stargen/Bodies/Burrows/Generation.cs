namespace Primoris.Universe.Stargen.Bodies.Burrows
{
	internal class Generation
	{
		public DustRecord Dusts { get; set; }
		public Seed Bodies { get; set; }
		public Generation Next { get; set; }
	}
}
