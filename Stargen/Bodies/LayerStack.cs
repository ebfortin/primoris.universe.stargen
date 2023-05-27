using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;

using Col = Primoris.Collections;
using System.Reflection.Emit;

namespace Primoris.Universe.Stargen.Bodies;

/// <summary>
/// Stack of layers.
/// </summary>
/// <seealso cref="System.Collections.Generic.IList{Primoris.Universe.Stargen.Bodies.Layer}" />
/// <seealso cref="System.Collections.Generic.IEnumerable{Primoris.Universe.Stargen.Bodies.Layer}" />
public class LayerStack : IEnumerable<Layer>
{
	List<Layer> _layers = new();
	SatelliteBody _parent;

	public SatelliteBody Parent => _parent;

	/// <summary>
	/// Initializes a new instance of the <see cref="LayerStack"/> class.
	/// </summary>
	/// <param name="parent">The parent.</param>
	public LayerStack(SatelliteBody parent)
	{
		_parent = parent;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LayerStack"/> class with an enumerable of layers.
	/// </summary>
	/// <param name="parent">The parent.</param>
	/// <param name="layers">The layers.</param>
	public LayerStack(SatelliteBody parent, IEnumerable<Layer> layers) : this(parent)
	{
		_layers = new List<Layer>(layers);
	}

	/// <summary>
	/// Gets or sets the <see cref="Layer"/> at the specified index.
	/// </summary>
	/// <value>
	/// The <see cref="Layer"/>.
	/// </value>
	/// <param name="index">The index.</param>
	/// <returns>The Layer at the index.</returns>
	public Layer this[int index] { get => _layers[index]; set => _layers[index] = value; }

	/// <summary>
	/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </summary>
	public int Count => _layers.Count;

	/// <summary>
	/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </summary>
	/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
	/// <exception cref="Primoris.Universe.Stargen.Bodies.InvalidBodyLayerSequenceException">Can't put a SolidLayer on top of a GaseousLayer</exception>
	internal void Add(Layer item)
	{
		if (Count > 0 && _layers[Count - 1] is GaseousLayer && item is SolidLayer)
			throw new InvalidBodyLayerSequenceException("Can't put a SolidLayer on top of a GaseousLayer");

		if(item.Parent != _parent)
			throw new ArgumentException("Added Layer has not the same Parent has this LayerStack.");

		_layers.Add(item);
	}

	/// <summary>
	/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </summary>
	internal void Clear()
	{
		_layers.Clear();
	}

	/// <summary>
	/// Determines whether this instance contains the object.
	/// </summary>
	/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
	/// <returns>
	///   <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.
	/// </returns>
	public bool Contains(Layer item)
	{
		return _layers.Contains(item);
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>
	/// An enumerator that can be used to iterate through the collection.
	/// </returns>
	public IEnumerator<Layer> GetEnumerator()
	{
		return _layers.GetEnumerator();
	}

	/// <summary>
	/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
	/// </summary>
	/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
	/// <returns>
	/// The index of <paramref name="item" /> if found in the list; otherwise, -1.
	/// </returns>
	public int IndexOf(Layer item)
	{
		return _layers.IndexOf(item);
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>
	/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
	/// </returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return _layers.GetEnumerator();
	}

	/// <summary>
	/// TODO: Add unit test.
	/// Returns the thickness of all the layer below the specified one.
	/// </summary>
	/// <param name="layer"></param>
	/// <returns></returns>
	public Length ComputeThicknessBelow(Layer layer)
	{
		if (!Contains(layer))
			throw new ArgumentException("Layer not in LayerStack.");

		var index = _layers.FindIndex(x => x == layer);
		if (index == 0)
			return Length.Zero;

		var thick = Length.Zero;
		foreach(var l in _layers.GetRange(0, index))
		{
			thick += l.Thickness;
		}

		return thick;
	}

	/// <summary>
	/// Computes the mass below the current layer.
	/// </summary>
	/// <param name="layer">The layer.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentException">Layer not in LayerStack.</exception>
	public Mass ComputeMassBelow(Layer layer)
	{
		if (!Contains(layer))
			throw new ArgumentException("Layer not in LayerStack.");

		var index = _layers.FindIndex(x => x == layer);
		if (index == 0)
			return Mass.Zero;

		var m = Mass.Zero;
		foreach (var l in _layers.GetRange(0, index))
		{
			m += l.Mass;
		}

		return m;
	}

	/// <summary>
	/// Computes the acceleration at lower boundary of the specified layer.
	/// </summary>
	/// <param name="layer">The layer.</param>
	/// <returns></returns>
	public Acceleration ComputeAccelerationAt(Layer layer)
	{
		var mbelow = ComputeMassBelow(layer);
		var rbelow = ComputeThicknessBelow(layer);

		Acceleration acc;
		if(rbelow.Equals(Length.Zero, 0.00001, ComparisonType.Relative))
		{
			acc = Acceleration.Zero;
		}
		else 
		{
			acc = layer.Parent!.Science.Physics.GetAcceleration(mbelow, rbelow);
		}

		return acc;
	}

	public IEnumerable<Layer> GetLayersBelow(Layer layer)
	{
        if (!Contains(layer))
            throw new ArgumentException("Layer not in LayerStack.");

        var index = _layers.FindIndex(x => x == layer);
        if (index == 0)
            return Array.Empty<Layer>();

        return _layers.GetRange(0, index);
    }

	public void CreateLayer(Func<LayerStack, Layer> layerCreator)
	{
		var layer = layerCreator(this);
	}
}
