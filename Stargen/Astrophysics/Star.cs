using Primoris.Universe.Stargen.Bodies;


namespace Primoris.Universe.Stargen.Astrophysics;

public class Star : StellarBody
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	public Star() : base() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="phy">The astrophysics interface.</param>
	public Star(IScienceAstrophysics phy) : base(phy) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="mass">The mass of the Star.</param>
	public Star(Mass mass) : base(mass) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="phy">The astrophysics interface.</param>
	/// <param name="mass">The mass of the Star.</param>
	public Star(IScienceAstrophysics phy, Mass mass) : base(phy, mass) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="st">The StellarType of the Star to create.</param>
	public Star(StellarType st) : base(st) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="phy">The astrophysics interface.</param>
	/// <param name="st">The StellarType of the Star to create.</param>
	public Star(IScienceAstrophysics phy, StellarType st) : base(phy, st) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="st">The StellarType of the Star to create.</param>
	/// <param name="name">The name to give to the Star.</param>
	public Star(StellarType st, string name) : base(st, name) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="phy">The astrophysics interface.</param>
	/// <param name="st">The StellarType of the Star to create.</param>
	/// <param name="name">The name to give to the Star.</param>
	public Star(IScienceAstrophysics phy, StellarType st, string name) : base(phy, st, name) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="mass">The mass of the Star to create.</param>
	/// <param name="lum">The luminosity of the Star to create.</param>
	/// <param name="age">The age of the Star to create.</param>
	public Star(Mass mass, Luminosity lum, Duration age) : base(mass, lum, age) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="Star"/> class.
	/// </summary>
	/// <param name="phy">The astrophysics interface.</param>
	/// <param name="mass">The mass of the Star to create.</param>
	/// <param name="lum">The luminosity of the Star to create.</param>
	/// <param name="age">The age of the Star to create.</param>
	public Star(IScienceAstrophysics phy, Mass mass, Luminosity lum, Duration age) : base(phy, mass, lum, age) { }

	/// <summary>
	/// Generates the satellites.
	/// </summary>
	/// <param name="seeds">The seeds used to create the satellites.</param>
	/// <param name="createFunc">The create function called for each satellite.</param>
	/// <returns>An IEnumerable of the Satellites created.</returns>
	protected override IEnumerable<SatelliteBody> GenerateSatellites(IEnumerable<Seed> seeds, CreateSatelliteBodyDelegate createFunc)
	{
		var planets = new List<SatelliteBody>();
		var i = 0;
		foreach (var seed in seeds)
		{
			var planetNo = i + 1; // start counting planets at 1
			i += 1;

			string planet_id = planetNo.ToString();

			var planet = createFunc(seed, this, i, planet_id);
			planets.Add(planet);
		}

		return planets;
	}

}
