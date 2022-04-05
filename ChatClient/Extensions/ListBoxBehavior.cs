using System.Windows;
using System.Windows.Controls;

namespace Extensions
{
    /// <summary>
    /// ListBox AutoScroll attached properties
    /// Source: https://ivanderevianko.com/2013/10/wpf-auto-scroll-behavior-for-listbox
    /// </summary>
    public static class ListBoxBehavior
    {
        public static readonly DependencyProperty AutoScrollProperty = DependencyProperty.RegisterAttached(
            "AutoScroll",
            typeof(bool),
            typeof(ListBox),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AutoScrollHandlerProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollHandler",
                typeof(AutoScrollHandler),
                typeof(ListBox));

        public static bool GetAutoScroll(ListBox instance)
        {
            return (bool)instance.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(ListBox instance, bool value)
        {
            AutoScrollHandler OldHandler = (AutoScrollHandler)instance.GetValue(AutoScrollHandlerProperty);
            if (OldHandler != null)
            {
                OldHandler.Dispose();
                instance.SetValue(AutoScrollHandlerProperty, null);
            }

            instance.SetValue(AutoScrollProperty, value);
            if (value)
            {
                instance.SetValue(AutoScrollHandlerProperty, new AutoScrollHandler(instance));
            }
        }
    }
}