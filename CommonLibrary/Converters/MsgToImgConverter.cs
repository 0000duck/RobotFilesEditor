using CommonLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CommonLibrary.Converters
{
    public class MsgToImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case BaseCheckState.OK:
                case LogResultTypes.OK:
                    return "pack://application:,,,/Resources/tick.png";
                case LogResultTypes.Warning:
                case BaseCheckState.Warning:
                    return "pack://application:,,,/Resources/alert.png";
                case LogResultTypes.Error:
                case BaseCheckState.Error:
                    return "pack://application:,,,/Resources/error.png";
                case LogResultTypes.Information:
                    return "pack://application:,,,/Resources/info.png";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
