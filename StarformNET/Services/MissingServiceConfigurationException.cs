using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Primoris.Universe.Stargen.Services
{
    public class MissingServiceConfigurationException : Exception
    {
        public MissingServiceConfigurationException()
        {
        }

        public MissingServiceConfigurationException(string? message) : base(message)
        {
        }

        public MissingServiceConfigurationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MissingServiceConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
