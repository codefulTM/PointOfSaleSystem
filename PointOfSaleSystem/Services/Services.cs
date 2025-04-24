using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;

namespace PointOfSaleSystem.Services
{

    /// <summary>
    /// The <c>Services</c> class provides a mechanism for managing singleton instances
    /// of objects keyed by their interface or type name. It allows adding and retrieving
    /// singleton instances dynamically at runtime.
    /// </summary>
    public class Services
    {
        static Dictionary<string, object> _singletons = new Dictionary<string, object>();

        public static void AddKeyedSingleton<IParent, Child>()
        {
            Type parent = typeof(IParent);
            Type child = typeof(Child);
            _singletons[parent.Name] = Activator.CreateInstance(child);   
        }

        public static IParent GetKeyedSingleton<IParent>()
        {
            Type parent = typeof(IParent);
            return (IParent)_singletons[parent.Name];
        }
    }
}
