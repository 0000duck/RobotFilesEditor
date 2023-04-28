using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectInformations.Model
{
    public class ApplicationBase : ObservableObject
    {
        private bool GetBoolValue(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Equals("false", StringComparison.OrdinalIgnoreCase))
                    return false;
                else
                    return true;
            }
            return false;
        }

        public string ReverseStringValue(string str) 
        {
            bool boolval = GetBoolValue(str);
            boolval = !boolval;
            return boolval.ToString().ToLower();
        }
    }
}
