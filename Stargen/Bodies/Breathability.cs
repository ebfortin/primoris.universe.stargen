namespace Primoris.Universe.Stargen.Bodies
{
	/// <summary>
	/// Breathability of a body. 
	/// </summary>
	public enum Breathability
    {
		/// <summary>
		/// No atmosphere.
		/// </summary>
		None,
		/// <summary>
		/// Atmosphere is breathable.
		/// </summary>
		Breathable,
		/// <summary>
		/// Atmosphere is not breathable.
		/// </summary>
		Unbreathable,
		/// <summary>
		/// Atmosphere is breathable but poisonous.
		/// </summary>
		Poisonous
	}
}