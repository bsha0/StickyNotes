using System;
using System.Globalization;
using System.Windows.Data;

namespace StickyNotes.UI.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                var now = DateTime.Now;
                if ((now - dt).Days > 1)
                {
                    return string.Format(Resources.Localization.AppResource.DateFormat, dt);
                }
                else
                {
                    return string.Format(Resources.Localization.AppResource.TimeFormat, dt);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
