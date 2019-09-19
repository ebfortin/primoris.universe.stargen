using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Primoris.Universe.Stargen.Services
{
	/// <summary>
	/// A service in the Provider is missing.
	/// </summary>
	/// <seealso cref="System.Exception" />
	public class MissingServiceConfigurationException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="MissingServiceConfigurationException"/> class.
		/// </summary>
		public MissingServiceConfigurationException()
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MissingServiceConfigurationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public MissingServiceConfigurationException(string? message) : base(message)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MissingServiceConfigurationException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
		public MissingServiceConfigurationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="MissingServiceConfigurationException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		protected MissingServiceConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
