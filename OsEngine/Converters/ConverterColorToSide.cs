using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using OsEngine.Entity;

namespace OsEngine.Converters
{
    public class ConverterColorToSide : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush color = Brushes.White;

            if (value is Side)
            {
                if ((Side)value == Side.Buy)
                {
                    color = Brushes.LightGreen;
                }
                else
                {
                    color = Brushes.LightPink;
                }

            }

            if (value is string)
            {
                if ((string)value == "Connect")
                {
                    color = Brushes.DarkGreen;
                }
                else
                {
                    color = Brushes.DarkRed;
                }
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConverterIsRunToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = "START";
            

            if (value is bool)
            {
                if ((bool)value == true)
                {
                    str = "STOP";
                }
                
            }

            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
