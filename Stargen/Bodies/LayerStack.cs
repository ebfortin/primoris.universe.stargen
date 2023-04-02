using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using UnitsNet;


namespace Primoris.Universe.Stargen.Bodies
{
	/// <summary>
	/// Stack of layers.
	/// </summary>
	/// <seealso cref="System.Collections.Generic.IList{Primoris.Universe.Stargen.Bodies.Layer}" />
	/// <seealso cref="System.Collections.Generic.IEnumerable{Primoris.Universe.Stargen.Bodies.Layer}" />
	public class LayerStack : IList<Layer>, IEnumerable<Layer>
	{
		private List<Layer> _layers = new List<Layer>();
		private SatelliteBody _parent;

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
		public Layer this[int index] { get => ((IList<Layer>)_layers)[index]; set => ((IList<Layer>)_layers)[index] = value; }

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		public int Count => ((IList<Layer>)_layers).Count;

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		/// </summary>
		public bool IsReadOnly => ((IList<Layer>)_layers).IsReadOnly;

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		/// <exception cref="Primoris.Universe.Stargen.Bodies.InvalidBodyLayerSequenceException">Can't put a SolidLayer on top of a GaseousLayer</exception>
		public void Add(Layer item)
		{
			if (Count > 0 && _layers[Count - 1] is GaseousLayer && item is SolidLayer)
				throw new InvalidBodyLayerSequenceException("Can't put a SolidLayer on top of a GaseousLayer");

			item.Parent = _parent;

			((IList<Layer>)_layers).Add(item);
			item.OnAddedToStack();
		}

		/// <summary>
		/// Adds multiple Layers at once.
		/// </summary>
		/// <param name="items">The items.</param>
		public void AddMany(IEnumerable<Layer> items)
		{
			foreach(var item in items)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		public void Clear()
		{
			((IList<Layer>)_layers).Clear();
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
			return ((IList<Layer>)_layers).Contains(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		public void CopyTo(Layer[] array, int arrayIndex)
		{
			((IList<Layer>)_layers).CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Layer> GetEnumerator()
		{
			return ((IList<Layer>)_layers).GetEnumerator();
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
			return ((IList<Layer>)_layers).IndexOf(item);
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="Primoris.Universe.Stargen.Bodies.InvalidBodyLayerSequenceException">Can't put a SolidLayer on top of a GaseousLayer</exception>
		public void Insert(int index, Layer item)
		{
			if (index >= Count)
				throw new IndexOutOfRangeException();

			if (_layers[index - 1] is GaseousLayer && item is SolidLayer)
				throw new InvalidBodyLayerSequenceException("Can't put a SolidLayer on top of a GaseousLayer");

			((IList<Layer>)_layers).Insert(index, item);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </returns>
		public bool Remove(Layer item)
		{
			return ((IList<Layer>)_layers).Remove(item);
		}

		/// <summary>
		/// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public void RemoveAt(int index)
		{
			((IList<Layer>)_layers).RemoveAt(index);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<Layer>)_layers).GetEnumerator();
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

			Acceleration acc = default;
			if(rbelow.Equals(Length.Zero, 0.00001, ComparisonType.Relative))
			{
				acc = Acceleration.Zero;
			}
			else 
			{
				acc = layer.Parent.Science.Physics.GetAcceleration(mbelow, rbelow);
			}

			return acc;
		}
	}
}
