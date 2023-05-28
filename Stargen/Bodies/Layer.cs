using Primoris.Types;
using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Body layer.
/// </summary>
/// <remarks>
/// Bodies are formed from different layers one on top of the other.
/// </remarks>
/// <seealso cref="System.IEquatable{Primoris.Universe.Stargen.Bodies.Layer}" />
public abstract class Layer : IEquatable<Layer>
{
    /// <summary>
    /// Gets or sets the science.
    /// </summary>
    /// <value>
    /// The science.
    /// </value>
    public IScienceAstrophysics Science => Stack.Parent.Science;

    /// <summary>
    /// Gets the stellar body.
    /// </summary>
    /// <remarks>
    /// Get the StellarBody of the system. 
    /// </remarks>
    /// <value>
    /// The stellar body, ie Parent.StellarBody.
    /// </value>
    public StellarBody StellarBody => Parent.StellarBody;

    /// <summary>
    /// Gets the parent this layer belongs to. 
    /// </summary>
    /// <value>
    /// The parent.
    /// </value>
    public SatelliteBody Parent => Stack.Parent;

    /// <summary>
    /// 
    /// </summary>
    public LayerStack Stack { get; } 

    /// <summary>
    /// Gets or sets the thickness of the layer. 
    /// </summary>
    /// <value>
    /// The thickness.
    /// </value>
    public Length Thickness { get; }

    /// <summary>
    /// Total mass of the Layer. 
    /// </summary>
    /// <remarks>
    /// Combined mass of all layers should equal SatelliteBody mass. There is currently no automated way to do this.
    /// </remarks>
    /// <returns>
    /// Mass of the Layer.
    /// </returns>
    public Mass Mass { get; protected set; } = Mass.Zero;

    /// <summary>
    /// Gets the mean density.
    /// </summary>
    /// <remarks>
    /// MeanDensity equals to Mass / Volume. This property is virtual so derived classes can customize the default value.
    /// </remarks>
    /// <value>
    /// The mean density.
    /// </value>
    public Density MeanDensity => Mass / Volume;

    /// <summary>
    /// Gets or sets the mean temperature.
    /// </summary>
    /// <value>
    /// The mean temperature.
    /// </value>
    public Temperature MeanTemperature { get; protected set; } = Temperature.Zero;

    public bool IsMeanTemperatureKnown => MeanTemperature > Temperature.Zero;

    /// <summary>
    /// TODO: Add Unit Test.
    /// </summary>
    public Volume Volume
    {
        get
        {
            var belowrad = LowerBoundaryRadius.Kilometers;
            var aboverad = UpperBoundaryRadius.Kilometers;
            var outer = 4.0 / 3.0 * double.Pi * double.Pow(aboverad, 3.0);
            var inner = 4.0 / 3.0 * double.Pi * double.Pow(belowrad, 3.0);

            return Volume.FromCubicKilometers(outer - inner);
        }
    }

    /// <summary>
    /// Gets the upper boundary area.
    /// </summary>
    /// <remarks>
    /// Area encompassing the outer surface of the Layer. Equals the LowerBoundaryArea of the next layer in the stack.
    /// </remarks>
    /// <value>
    /// The upper boundary area.
    /// </value>
    public Area UpperBoundaryArea
    {
        get
        {
            var aboverad = Stack.ComputeThicknessBelow(this).Kilometers + Thickness.Kilometers;
            var outer = 4.0 * Math.PI * Math.Pow(aboverad, 2.0);

            return Area.FromSquareKilometers(outer);
        }
    }

    /// <summary>
    /// Gets the lower boundary area.
    /// </summary>
    /// <remarks>
    /// Area encompassing the inner surface of the Layer. Equals the UpperBoundaryArea of the previous layer in the stack.
    /// </remarks>
    /// <value>
    /// The lower boundary area.
    /// </value>
    public Area LowerBoundaryArea
    {
        get
        {
            var belowrad = LowerBoundaryRadius.Kilometers;
            var inner = 4.0 * double.Pi * double.Pow(belowrad, 2.0);

            return Area.FromSquareKilometers(inner);
        }
    }

    public Length LowerBoundaryRadius => Stack.ComputeThicknessBelow(this);

    public Length UpperBoundaryRadius => LowerBoundaryRadius + Thickness;

    /// <summary>
    /// Gets or sets the chemical composition collection of the Layer.
    /// </summary>
    /// <value>
    /// The composition internal.
    /// </value>
    protected IList<(Chemical, Ratio)> CompositionInternal { get; set; }

    /// <summary>
    /// Gets the composition.
    /// </summary>
    /// <value>
    /// The composition.
    /// </value>
    public IEnumerable<(Chemical, Ratio)> Composition => CompositionInternal;

    public bool IsCompositionKnown => CompositionInternal.Count > 0;

   


    /// <summary>
    /// Initializes a new instance of the <see cref="Layer"/> class with Mass and Thickness set but without any composition.
    /// </summary>
    /// <remarks>
    /// A Layer without composition is valid and will only returns an empty IEnumerable when checked for its composition. It
    /// is considered to be composition unknown. This constructor also set the MeanTemperature to Zero. Since even
    /// cosmic void is not at absolute zero this value means that the temperature of the Layer is unknown.
    /// </remarks>
    /// <param name="mass">Mass of the Layer.</param>
    /// <param name="thickness">The thickness.</param>
    protected Layer(LayerStack stack, Mass mass, Length thickness)
    {
        CompositionInternal = new List<(Chemical, Ratio)>();

        Stack = stack;
        Mass = mass;
        Thickness = thickness;

        Stack.Add(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Layer"/> class with all its attributes set.
    /// </summary>
    /// <remarks>
    /// A Layer without composition is valid and will only returns an empty IEnumerable when checked for its composition. It
    /// is considered to be composition unknown.
    /// </remarks>
    /// <param name="stack">LayerStack this Layer belongs to.</param>
    /// <param name="mass">Mass of the Layer.</param>
    /// <param name="thickness">The thickness.</param>
    /// <param name="temperature">Mean temperature of the Layer.</param>
    protected Layer(LayerStack stack, Mass mass, Length thickness, Temperature temperature)
    {
        CompositionInternal = new List<(Chemical, Ratio)>();

        Stack = stack;
        Mass = mass;
        Thickness = thickness;
        MeanTemperature = temperature;

        stack.Add(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Layer"/> class with all its attributes set.
    /// </summary>
    /// <param name="stack">LayerStack this Layer belongs to.</param>
    /// <param name="mass">Mass of the Layer.</param>
    /// <param name="thickness">The thickness.</param>
    /// <param name="temperature">Mean temperature of the Layer.</param>
    /// <param name="composition">The composition.</param>
    protected Layer(LayerStack stack, Mass mass, Length thickness, Temperature temperature, IEnumerable<(Chemical, Ratio)> composition)
    {
        Stack = stack;
        Mass = mass;
        Thickness = thickness;
        MeanTemperature = temperature;
        
        CompositionInternal = new List<(Chemical, Ratio)>(composition);

        stack.Add(this);
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Layer);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
    /// </returns>
    public bool Equals(Layer? other)
    {
        if (other is null)
            return false;

        return ReferenceEquals(this, other);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Stack, StellarBody, Parent, Thickness, Mass, MeanTemperature, CompositionInternal, Composition);
    }


    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(Layer left, Layer right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(Layer left, Layer right)
    {
        return !(left == right);
    }
}
