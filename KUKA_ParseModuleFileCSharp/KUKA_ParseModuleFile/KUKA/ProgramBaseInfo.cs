using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class ProgramBaseInfo : ObservableCollection<ProgramBaseInfoItem>
    {

        private ObservableCollection<KeyValuePair<ProgramBaseInfoItemType, bool>> _okList = new ObservableCollection<KeyValuePair<ProgramBaseInfoItemType, bool>>();
        public void SetOK(ProgramBaseInfoItemType key, bool value)
        {
                foreach (KeyValuePair<ProgramBaseInfoItemType, bool> Item in _okList)
                {
                    if (Item.Key == key)
                    {
                        _okList[_okList.IndexOf(Item)] = new KeyValuePair<ProgramBaseInfoItemType, bool>(key, value);
                        break;
                    }
                }
        }

        public ObservableCollection<KeyValuePair<ProgramBaseInfoItemType, bool>> OKList
        {
            get { return _okList; }
            set { _okList = value; }
        }

        public bool hasItem(ProgramBaseInfoItemType ItemType, int Number)
        {
            foreach (ProgramBaseInfoItem Item in this)
            {
                if (Item.Type == ItemType && Item.Number == Number)
                {
                    return true;
                }
            }
            return false;
        }

        public List<ProgramBaseInfoItem> getTypeList(ProgramBaseInfoItemType ItemType)
	{
		return (from item in this where item.Type == ItemType select item).ToList();
	}

        public bool AllOK
        {
            get
            {
                foreach (KeyValuePair<ProgramBaseInfoItemType, bool> aItem in _okList)
                {
                    if (!aItem.Value)
                        return false;
                }
                foreach (ProgramBaseInfoItem aItem in this)
                {
                    if (!aItem.OK)
                        return false;
                }
                return true;
            }
        }

        public ProgramBaseInfo()
        {
            foreach (int x in Enum.GetValues(typeof(ProgramBaseInfoItemType)))
            {
                _okList.Add(new KeyValuePair<ProgramBaseInfoItemType, bool>((ProgramBaseInfoItemType)x, true));
            }
        }


        //Public homeList As List(Of Integer) = New List(Of Integer)
        //Public homeListOK As Boolean = True
        //Public jobList As New List(Of Integer)
        public bool jobListOK = true;
        public Dictionary<int, string> areaList = new Dictionary<int, string>();
        public bool areaListOK = true;
        public List<int> plcList = new List<int>();
        public bool plclistOK = true;
        public Dictionary<int, string> localCollList = new Dictionary<int, string>();
        public Dictionary<int, string> extCollList = new Dictionary<int, string>();
        public bool colllistOK = true;
        public List<int> applicationPointList = new List<int>();
        public bool applicationPointListOK = false;
        public ProgramType ProgramType = ProgramType.UNKNOWN;
    }

}
