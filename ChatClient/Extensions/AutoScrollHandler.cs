using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace Extensions
{
    /// <summary>
    /// Handle auto scroll functionality
    /// Source: https://ivanderevianko.com/2013/10/wpf-auto-scroll-behavior-for-listbox
    /// </summary>
    public class AutoScrollHandler : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(AutoScrollHandler),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.None,
                ItemsSourcePropertyChanged));

        private readonly ListBox target;

        public AutoScrollHandler(ListBox target)
        {
            this.target = target;
            var binding = new Binding("ItemsSource") { Source = this.target };
            BindingOperations.SetBinding(this, ItemsSourceProperty, binding);
        }

        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ItemsSourceProperty);
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((AutoScrollHandler)o).ItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void ItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            var collection = oldValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged -= this.CollectionChangedEventHandler;
            }

            collection = newValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += this.CollectionChangedEventHandler;
            }
        }

        private void CollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems == null || e.NewItems.Count < 1)
            {
                return;
            }

            target.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
        }
    }
}