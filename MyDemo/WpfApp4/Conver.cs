using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp4
{
    [ValueConversion(typeof(int), typeof(Brush))]
    public class ColorConvert : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush;
            if(values == null)
            {
                return null;
            }
            brush = new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(values[0]), System.Convert.ToByte(values[1]), System.Convert.ToByte(values[2]), System.Convert.ToByte(values[3])));
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
