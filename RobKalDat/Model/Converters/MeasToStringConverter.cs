using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RobKalDat.Model.Converters
{
    public class MeasToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            ObservableCollection<string> result = new ObservableCollection<string>();
            foreach (var item in (ObservableCollection<Measurement>)value)
            {
                result.Add(item.Name);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
