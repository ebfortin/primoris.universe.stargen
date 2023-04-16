using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Primoris.Universe.Stargen;

public class InvalidBodyException : Exception
{
	public InvalidBodyException()
	{
	}

	public InvalidBodyException(string? message) : base(message)
	{
	}

	public InvalidBodyException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	protected InvalidBodyException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}
