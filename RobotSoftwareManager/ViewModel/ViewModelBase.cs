using CommunityToolkit.Mvvm.ComponentModel;
using RobotSoftwareManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotSoftwareManager.ViewModel
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        public List<RobotSoftwareBase> Sort(List<RobotSoftwareBase> m_BaseCollection)
        {
            var result = new List<RobotSoftwareBase>();
            var softElements = m_BaseCollection.OrderBy(x => x.Number).ToList();
            softElements.ForEach(x => result.Add(x));
            return result;
        }
    }
}
