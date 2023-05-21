using Primoris.Universe.Stargen.Astrophysics;

namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Layer that is Homogeneous.
/// </summary>
/// <remarks>
/// In this version of Stargen, all layers are Homogeneous.
/// </remarks>
/// <seealso cref="Primoris.Universe.Stargen.Bodies.Layer" />
public abstract class HomogeneousLayer : Layer
{
    protected HomogeneousLayer(LayerStack stack, Mass mass, Length thickness) : base(stack, mass, thickness)
    {
    }

    protected HomogeneousLayer(LayerStack stack, Mass mass, Length thickness, Temperature temperature) : base(stack, mass, thickness, temperature)
    {
    }

    protected HomogeneousLayer(LayerStack stack, Mass mass, Length thickness, Temperature temperature, IEnumerable<(Chemical, Ratio)> composition) : base(stack, mass, thickness, temperature, composition)
    {
    }
}
