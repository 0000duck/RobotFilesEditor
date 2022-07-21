using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.DataTypes
{
    public class Grippers : ObservableCollection<GripStruc>
    {
        public int ConfiguredGripperCount
        {
            get {
			int x = 0;
			foreach (var xItem in this.Items) {
				if (xItem.IsConfig)
					x += 1;
			}
			return x;
		}
        }
    }
}
