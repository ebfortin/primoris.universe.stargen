using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Bodies.Burrows;
using UnitsNet;


namespace Primoris.Universe.Stargen.Services
{
	/// <summary>
	/// Services provider of the Primoris.Universe.Stargen framework.
	/// </summary>
	/// <remarks>
	/// This Provider is called from classes when a Service is needed and no interface to that service is specified. Only one instance can exist for each 
	/// Service type.
	/// </remarks>
	public class Provider
    {
		/// <summary>
		/// Initializes the <see cref="Provider"/> class.
		/// </summary>
		/// <remarks>
		/// Adds all the default services.
		/// </remarks>
		static Provider()
		{
			_services.Add(typeof(IScienceAstrophysics), new BodyPhysics());
			_services.Add(typeof(IBodyFormationAlgorithm), new Accrete(Ratio.FromDecimalFractions(GlobalConstants.CLOUD_ECCENTRICITY),
																	Ratio.FromDecimalFractions(GlobalConstants.K),
																	Ratio.FromDecimalFractions(GlobalConstants.DUST_DENSITY_COEFF)));
			_services.Add(typeof(Random), new Random());
		}

        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

        private static Provider _instance = null;

		/// <summary>
		/// Uses this instance.
		/// </summary>
		/// <remarks>
		/// A Provider is a Singleton for a given Process. Each call to this method will always return the same instance.
		/// </remarks>
		/// <returns>Provider instance.</returns>
		public static Provider Use()
        {
			if (_instance is null)
				_instance = new Provider();

			return _instance;
        }


        private Provider() { }

		/// <summary>
		/// Withes the random.
		/// </summary>
		/// <param name="rand">The Random instance to use.</param>
		/// <returns></returns>
		public Provider WithRandom(Random rand)
		{
			_services[typeof(Random)] = rand;
			return this;
		}

		/// <summary>
		/// Withes the astrophysics.
		/// </summary>
		/// <param name="phy">The astrophysics services to use.</param>
		/// <returns></returns>
		public Provider WithAstrophysics(IScienceAstrophysics phy)
        {
            _services[typeof(IScienceAstrophysics)] = phy;
            return this;
        }

		/// <summary>
		/// Withes the formation algorithm.
		/// </summary>
		/// <param name="frm">The formation algorithm service to use.</param>
		/// <returns></returns>
		public Provider WithFormationAlgorithm(IBodyFormationAlgorithm frm)
        {
            _services[typeof(IBodyFormationAlgorithm)] = frm;
            return this;
        }

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <typeparam name="T">Type of service to get.</typeparam>
		/// <returns>Service.</returns>
		/// <exception cref="Primoris.Universe.Stargen.Services.MissingServiceConfigurationException">Service type specified is not defined.</exception>
		public T GetService<T>()
            where T : class
        {
            if (!_services.ContainsKey(typeof(T)))
                throw new MissingServiceConfigurationException();

            return _services[typeof(T)] as T;
        }

    }
}
