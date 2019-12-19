using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiSourceTorrentDownloader.Converters
{
    public class TimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime dateTime)
            {
                if(dateTime.Date == DateTime.Now.Date)
                    return dateTime.ToString("hh:mm tt");
                else if(dateTime.Year == DateTime.Now.Year)
                    return dateTime.ToString("MMM. dd");
                else if(dateTime.Year != DateTime.Now.Year)
                    return dateTime.ToString("yyyy MMM.");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
