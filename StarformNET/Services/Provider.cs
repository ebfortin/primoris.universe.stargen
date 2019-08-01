using System;
using System.Collections.Generic;
using System.Text;
using Primoris.Universe.Stargen.Astrophysics;
using Primoris.Universe.Stargen.Bodies;


namespace Primoris.Universe.Stargen.Services
{
    public class Provider
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

        private static Provider _instance = null;

        public static Provider Use()
        {
            if (_instance is null)
                return new Provider();
            else
                return _instance;
        }


        private Provider() { }


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
                throw new NotSupportedException();

            return _services[typeof(T)] as T;
        }

    }
}
