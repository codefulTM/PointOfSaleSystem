using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Utils.Checkers;

namespace PointOfSaleSystem.Models
{
    /// <summary>
    /// Represents a customer in the Point of Sale system.
    /// Implements the <see cref="INotifyPropertyChanged"/> interface to support property change notifications
    /// and the <see cref="ICheckable"/> interface for validation.
    /// </summary>
    public class Customer : INotifyPropertyChanged, ICheckable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Gender { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Accepts a checker to perform a validation check on this <see cref="Customer"/> instance.
        /// </summary>
        /// <param name="checker">The checker object to perform the validation.</param>
        /// <returns>
        /// A string containing the error message if the validation check fails, or
        /// <c>null</c> if the validation check succeeds.
        /// </returns>
        public string? AcceptForChecking(IChecker checker)
        {
            return checker.Check(this);
        }

        public bool Check()
        {
            return true;
        }
    }
}
