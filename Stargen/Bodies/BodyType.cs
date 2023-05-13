namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Types of body, planet or satellite.
/// </summary>
public enum BodyType
{
    /// <summary>
    /// Undefined Body type.
    /// </summary>
    Undefined,
    /// <summary>
    /// Body deprived of naturally or conventionally appropriate covering.
    /// </summary>
    Barren,
    /// <summary>
    /// Body with a thick atmosphere.
    /// </summary>
    Venusian,
    /// <summary>
    /// Body with an atmosphere comparable to the one of planet Earth in the Solar System.
    /// </summary>
    Terrestrial,
    /// <summary>
    /// Body composed of only gas or with a small solid core.
    /// </summary>
    GasGiant,
    /// <summary>
    /// Body with a thin atmosphere.
    /// </summary>
    Martian,
    /// <summary>
    /// Body entirely covered with water.
    /// </summary>
    Water,
    /// <summary>
    /// Body entirely covered with ice.
    /// </summary>
    Ice,
    /// <summary>
    /// Body composed of only gas or with a small solid core, smaller than a GasGiant.
    /// </summary>
    SubGasGiant,
    /// <summary>
    /// Body composed of only gas or with a small solid core, smaller than a SubGasGiant.
    /// </summary>
    SubSubGasGiant,
    /// <summary>
    /// Body too small to be considered a planet.
    /// </summary>
    Asteroid
}
