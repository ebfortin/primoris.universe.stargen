using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Primoris.Universe.Stargen.Bodies
{
	public class InvalidBodyLayerSequenceException : Exception
	{
		public InvalidBodyLayerSequenceException()
		{
		}

		public InvalidBodyLayerSequenceException(string? message) : base(message)
		{
		}

		public InvalidBodyLayerSequenceException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected InvalidBodyLayerSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
