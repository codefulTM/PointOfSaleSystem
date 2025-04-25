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

        /// <summary>
        /// Adds a singleton instance of a child class to the dictionary.
        /// </summary>
        /// <typeparam name="IParent">The interface or parent class type.</typeparam>
        /// <typeparam name="Child">The child class type to create and add.</typeparam>
        /// <returns>Nothing.</returns>
        public static void AddKeyedSingleton<IParent, Child>()
        {
            Type parent = typeof(IParent);
            Type child = typeof(Child);
            _singletons[parent.Name] = Activator.CreateInstance(child);   
        }

        /// <summary>
        /// Retrieves a singleton instance of a class keyed by its interface or type name.
        /// </summary>
        /// <typeparam name="IParent">The interface or parent class type.</typeparam>
        /// <returns>The singleton instance of the specified type.</returns>
        public static IParent GetKeyedSingleton<IParent>()
        {
            Type parent = typeof(IParent);
            return (IParent)_singletons[parent.Name];
        }
    }
}
