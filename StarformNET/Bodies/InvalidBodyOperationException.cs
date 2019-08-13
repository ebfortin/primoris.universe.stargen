using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Primoris.Universe.Stargen.Bodies
{
	class InvalidBodyOperationException : InvalidOperationException
	{
		public InvalidBodyOperationException()
		{
		}

		public InvalidBodyOperationException(string? message) : base(message)
		{
		}

		public InvalidBodyOperationException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected InvalidBodyOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
