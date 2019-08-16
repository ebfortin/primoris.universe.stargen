using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Bodies;
using Primoris.Universe.Stargen.Astrophysics.Burrows;
using Primoris.Universe.Stargen.Bodies.Burrows;


namespace Primoris.Universe.Stargen.Services
{
    public class Provider
    {
		static Provider()
		{
			_services.Add(typeof(IScienceAstrophysics), new BodyPhysics());
			_services.Add(typeof(IBodyFormationAlgorithm), new Accrete(GlobalConstants.CLOUD_ECCENTRICITY, GlobalConstants.K));
			_services.Add(typeof(Random), new Random());
		}

        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

        private static Provider _instance = null;

        public static Provider Use()
        {
			if (_instance is null)
				_instance = new Provider();

			return _instance;
        }


        private Provider() { }

		public Provider WithRandom(Random rand)
		{
			_services[typeof(Random)] = rand;
			return this;
		}

        public Provider WithAstrophysics(IScienceAstrophysics phy)
        {
            _services[typeof(IScienceAstrophysics)] = phy;
            return this;
        }

        public Provider WithFormationAlgorithm(IBodyFormationAlgorithm frm)
        {
            _services[typeof(IBodyFormationAlgorithm)] = frm;
            return this;
        }

        public T GetService<T>()
            where T : class
        {
            if (!_services.ContainsKey(typeof(T)))
                throw new MissingServiceConfigurationException();

            return _services[typeof(T)] as T;
        }

    }
}
