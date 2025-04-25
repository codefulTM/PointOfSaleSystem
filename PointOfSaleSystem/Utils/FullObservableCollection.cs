using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace PointOfSaleSystem.Utils
{
    /// <summary>
    /// A specialized implementation of <see cref="ObservableCollection{T}"/> that provides 
    /// notifications when properties of its items change. This class ensures that changes 
    /// to the properties of the items in the collection trigger the <see cref="CollectionChanged"/> 
    /// event, allowing for more granular updates in data-bound scenarios.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the collection. Must implement <see cref="INotifyPropertyChanged"/> 
    /// to support property change notifications.
    /// </typeparam>
    public sealed class FullObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FullObservableCollection{T}"/> class.
        /// </summary>
        public FullObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FullObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="pItems">The items to add to the collection.</param>
        /// <returns>A new instance of the <see cref="FullObservableCollection{T}"/> class.</returns>
        public FullObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Handles the <see cref="NotifyCollectionChanged"/> event, hooking up the 
        /// <see cref="ItemPropertyChanged"/> event handler to the items being added, 
        /// and unhooking it from the items being removed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void FullObservableCollectionCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="PropertyChanged"/> event for the items in the collection.
        /// </summary>
        /// <param name="sender">The source of the event, which is the item that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <returns>This method does not return a value.</returns>
        private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender!)
            );
            OnCollectionChanged(args);
        }
    }
}
