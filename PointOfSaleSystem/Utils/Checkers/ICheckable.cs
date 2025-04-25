using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Utils.Checkers
{
    
    /// <summary>
    /// Represents an interface for objects that can be checked by an implementation of the <see cref="IChecker"/> interface.
    /// Provides a method to accept a checker and perform a checking operation.
    /// </summary>
    public interface ICheckable
    {
        public string? AcceptForChecking(IChecker checker);
    }
}
