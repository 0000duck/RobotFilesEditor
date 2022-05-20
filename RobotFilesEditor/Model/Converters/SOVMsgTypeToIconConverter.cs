using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RobotFilesEditor.Model.Converters
{
    class SOVMsgTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case GlobalData.SovLogContentInfoTypes.OK:
                    return "pack://application:,,,/Resources/tick.png";
                case GlobalData.SovLogContentInfoTypes.Warning:
                    return "pack://application:,,,/Resources/alert.png";
                case GlobalData.SovLogContentInfoTypes.Error:
                    return "pack://application:,,,/Resources/error.png";
                case GlobalData.SovLogContentInfoTypes.Information:
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
