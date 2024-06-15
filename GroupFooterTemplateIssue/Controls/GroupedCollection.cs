using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace GroupFooterTemplateIssue.Controls
{
    public class GroupedCollection : CollectionView
    {
        public GroupedCollection()
        {
        }

        ~GroupedCollection()
        {
            Debug.WriteLine("Confirm destructor is called");
        }
    }

    public interface ICleanObservableCollection
    {
        void CleanUp(string handler);
    }

    public class CleanObservableCollection<T> : ObservableCollection<T>, ICleanObservableCollection
    {
        /// <summary>
        /// Initializes a new instance of CleanObservableCollection that is empty and has default initial capacity.
        /// </summary>
        public CleanObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class that contains
        /// elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
        public CleanObservableCollection(IEnumerable<T> collection) : base(new List<T>(collection ?? throw new ArgumentNullException(nameof(collection))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class
        /// that contains elements copied from the specified list
        /// </summary>
        /// <param name="list">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference </exception>
        public CleanObservableCollection(List<T> list) : base(new List<T>(list ?? throw new ArgumentNullException(nameof(list))))
        {
        }

        public void CleanUp(string handlerName)
        {
            // workaround for issue: https://github.com/dotnet/maui/issues/22954
            foreach (var handler in delegates.Where(h => h.Method.Name.Equals(handlerName)).ToList())
            {
                CollectionChanged -= handler;
            }
        }

        private List<NotifyCollectionChangedEventHandler> delegates = new();

        public override event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add
            {
                base.CollectionChanged += value;
                if (value != null) delegates.Add(value);
            }
            remove
            {
                base.CollectionChanged -= value;
                if (value != null) delegates.Remove(value);
            }
        }
    }
}