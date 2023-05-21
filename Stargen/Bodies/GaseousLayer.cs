using Primoris.Universe.Stargen.Astrophysics;


namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Body Layer that contains gas.
/// </summary>
/// <seealso cref="Primoris.Universe.Stargen.Bodies.HomogeneousLayer" />
public abstract class GaseousLayer : HomogeneousLayer
{

    /// <summary>
    /// Gets or sets the lower boundary pressure.
    /// </summary>
    /// <remarks>
    /// The pressure at the inner surface of the layer. 
    /// </remarks>
    /// <value>
    /// The lower boundary pressure.
    /// </value>
    public Pressure LowerBoundaryPressure { get; protected set; }

    /// <summary>
    /// Gets or sets the breathability.
    /// </summary>
    /// <value>
    /// The breathability.
    /// </value>
    public Breathability Breathability { get; protected set; } = Breathability.None;

    // TODO: Have poisonous gas being put in the list. Right now doesn't work.		
    /// <summary>
    /// Gets the poisonous composition internal.
    /// </summary>
    /// <value>
    /// The poisonous composition internal IList.
    /// </value>
    protected IList<(Chemical, Ratio)> PoisonousCompositionInternal { get; } = new List<(Chemical, Ratio)>();

    /// <summary>
    /// Gets or sets the poisonous composition.
    /// </summary>
    /// <value>
    /// The poisonous composition.
    /// </value>
    public IEnumerable<(Chemical, Ratio)> PoisonousComposition { get; protected set; } = Array.Empty<(Chemical, Ratio)>();

    protected GaseousLayer(LayerStack stack, Length thickness)
        : base(stack, Mass.Zero, thickness) 
    { 
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaseousLayer"/> class.
    /// </summary>
    /// <param name="thickness">The thickness.</param>
    /// <param name="surfPres">The surface pressure at the lower boundary with the layer below.</param>
    protected GaseousLayer(LayerStack stack, Length thickness, Pressure surfPres)
        : base(stack, Mass.Zero, thickness)
    {
        LowerBoundaryPressure = surfPres;
        Mass = GetMassFromPressure(surfPres);
    }

    protected GaseousLayer(LayerStack stack, Mass mass, Length thickness)
        : base(stack, mass, thickness)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaseousLayer"/> class.
    /// </summary>
    /// <param name="thickness">The thickness.</param>
    /// <param name="composition">The composition.</param>
    /// <param name="surfPres">The surface pressure at the lower boundary with the layer below.</param>
    protected GaseousLayer(LayerStack stack, Length thickness, IEnumerable<(Chemical, Ratio)> composition, Pressure surfPres)
        : base(stack, Mass.Zero, thickness, Temperature.Zero, composition)
    {
        LowerBoundaryPressure = surfPres;
        Mass = GetMassFromPressure(surfPres);
    }

    /// <summary>
    /// Gets the mass from pressure.
    /// </summary>
    /// <remarks>
    /// This methods is virtual to allow for more complex algorithm to get Mass from Pressure. Right now it uses
    /// pres * LowerBoundaryArea / "Acceleration at lower boundary".
    /// </remarks>
    /// <param name="pres">The pressure.</param>
    /// <returns>Mass</returns>
    protected virtual Mass GetMassFromPressure(Pressure pres)
    {
        return pres * LowerBoundaryArea / Stack.ComputeAccelerationAt(this);
    }
}

