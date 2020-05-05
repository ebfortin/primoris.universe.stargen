using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Primoris.Universe.Stargen.Bodies
{
	/// <summary>
	/// An operation done on a Body is invalid.
	/// </summary>
	/// <seealso cref="System.InvalidOperationException" />
	class InvalidBodyOperationException : InvalidOperationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyOperationException"/> class.
		/// </summary>
		public InvalidBodyOperationException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyOperationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public InvalidBodyOperationException(string? message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyOperationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference (<see langword="Nothing" /> in Visual Basic), the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
		public InvalidBodyOperationException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidBodyOperationException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected InvalidBodyOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
