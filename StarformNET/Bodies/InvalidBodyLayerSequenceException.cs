using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Primoris.Universe.Stargen.Bodies
{
	/// <summary>
	/// A layer type is not compatible with the layer that is below it. 
	/// </summary>
	/// <seealso cref="System.Exception" />
	public class InvalidBodyLayerSequenceException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyLayerSequenceException"/> class.
		/// </summary>
		public InvalidBodyLayerSequenceException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyLayerSequenceException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public InvalidBodyLayerSequenceException(string? message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyLayerSequenceException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
		public InvalidBodyLayerSequenceException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyLayerSequenceException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		protected InvalidBodyLayerSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
