using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;

namespace RobKalDat.Model.Converters
{
    public class ItemInSafetyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            if (value != null)
            {
                foreach (var item in (ObservableCollection<ItemInSafety>)value)
                {
                    if (item.Type == "Tool")
                    {
                        result.Add("Tool " + item.Tool.Number);
                    }
                    else if (item.Type == "Cellspace")
                    {
                        result.Add("Cellspace");
                    }
                    else if (item.Type == "Safespaces")
                    {
                        result.Add("Sp" + item.SafeSpaces.Number + ": " + item.SafeSpaces.Name);
                    }
                    else
                    {
                        MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return new ObservableCollection<string>();
                    }
                }
            }
            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
