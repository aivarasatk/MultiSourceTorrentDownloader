using MultiSourceTorrentDownloader.Enums;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiSourceTorrentDownloader.Converters
{
    public class MessageTypeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(Enum.TryParse(value.ToString(), out MessageType messageType))
            {
                switch (messageType)
                {
                    case MessageType.Information: return Brushes.Indigo;
                    case MessageType.Error: return Brushes.DarkRed;
                    case MessageType.Empty: return Brushes.White;
                }
            } 
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
