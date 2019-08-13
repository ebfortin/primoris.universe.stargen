using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;


namespace Primoris.Universe.Stargen.Bodies
{
	public class LayerStack : IList<Layer>, IEnumerable<Layer>
	{
		private List<Layer> _layers = new List<Layer>();
		private SatelliteBody _parent;

		public LayerStack(SatelliteBody parent)
		{
			_parent = parent;
		}

		public LayerStack(SatelliteBody parent, IEnumerable<Layer> layers) : this(parent)
		{
			_layers = new List<Layer>(layers);
		}

		public Layer this[int index] { get => ((IList<Layer>)_layers)[index]; set => ((IList<Layer>)_layers)[index] = value; }

		public int Count => ((IList<Layer>)_layers).Count;

		public bool IsReadOnly => ((IList<Layer>)_layers).IsReadOnly;

		public void Add(Layer item)
		{
			if (Count > 0 && _layers[Count - 1] is GaseousLayer && item is SolidLayer)
				throw new InvalidBodyLayerSequenceException("Can't put a SolidLayer on top of a GaseousLayer");

			item.Parent = _parent;

			((IList<Layer>)_layers).Add(item);
		}

		public void AddMany(IEnumerable<Layer> items)
		{
			foreach(var item in items)
			{
				Add(item);
			}
		}

		public void Clear()
		{
			((IList<Layer>)_layers).Clear();
		}

		public bool Contains(Layer item)
		{
			return ((IList<Layer>)_layers).Contains(item);
		}

		public void CopyTo(Layer[] array, int arrayIndex)
		{
			((IList<Layer>)_layers).CopyTo(array, arrayIndex);
		}

		public IEnumerator<Layer> GetEnumerator()
		{
			return ((IList<Layer>)_layers).GetEnumerator();
		}

		public int IndexOf(Layer item)
		{
			return ((IList<Layer>)_layers).IndexOf(item);
		}

		public void Insert(int index, Layer item)
		{
			if (index >= Count)
				throw new IndexOutOfRangeException();

			if (_layers[index - 1] is GaseousLayer && item is SolidLayer)
				throw new InvalidBodyLayerSequenceException("Can't put a SolidLayer on top of a GaseousLayer");

			((IList<Layer>)_layers).Insert(index, item);
		}

		public bool Remove(Layer item)
		{
			return ((IList<Layer>)_layers).Remove(item);
		}

		public void RemoveAt(int index)
		{
			((IList<Layer>)_layers).RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<Layer>)_layers).GetEnumerator();
		}
	}
}
