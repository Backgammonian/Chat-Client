using System;
using System.Globalization;
using System.Windows.Data;

namespace ChatClient
{
    public class MessageColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var sourceID = (string)values[0];
            var ownID = (string)values[1];

            return sourceID == ownID ? MessageColors.OwnMessage : MessageColors.ForeignMessage;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }
    }
}