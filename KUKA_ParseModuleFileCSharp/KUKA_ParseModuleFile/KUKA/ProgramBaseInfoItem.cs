using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ParseModuleFile.KUKA
{
    public class ProgramBaseInfoItem : NotifyPropertyChanged
    {

        private ProgramBaseInfoItemType _type;
        private int _number;
        private string _description;

        private bool _ok;
        public SolidColorBrush Brush
        {
            get
            {
                switch (_type)
                {
                    case ProgramBaseInfoItemType.Job:
                        return new SolidColorBrush(Colors.LightGreen);
                    case ProgramBaseInfoItemType.Area:
                        return new SolidColorBrush(Colors.Yellow);
                    case ProgramBaseInfoItemType.PlcCom:
                        return new SolidColorBrush(Colors.LightGray);
                    case ProgramBaseInfoItemType.CollZone:
                        return new SolidColorBrush(Colors.PaleVioletRed);
                    case ProgramBaseInfoItemType.Home:
                        return new SolidColorBrush(Colors.Wheat);
                    default:
                        return new SolidColorBrush(Colors.OrangeRed);
                }
            }
        }

        public string ShortType
        {
            get
            {
                switch (_type)
                {
                    case ProgramBaseInfoItemType.Job:
                        return "J";
                    case ProgramBaseInfoItemType.Area:
                        return "A";
                    case ProgramBaseInfoItemType.PlcCom:
                        return "P";
                    case ProgramBaseInfoItemType.CollZone:
                        return "C";
                    case ProgramBaseInfoItemType.Home:
                        return "H";
                    default:
                        return _type.ToString();
                }
            }
        }

        public ProgramBaseInfoItemType Type { get { return _type; } set { Set(ref _type, value); } }

        public int Number { get { return _number; } set { Set(ref _number, value); } }

        public bool OK { get { return _ok; } set { Set(ref _ok, value); } }

        public string Description { get { return _description; } set { Set(ref _description, value); } }

        public ProgramBaseInfoItem(ProgramBaseInfoItemType Type, int Number, string Description, bool OK = true)
        {
            _type = Type;
            _number = Number;
            _description = Description;
            _ok = OK;
        }

        public ProgramBaseInfoItem(Applications.Job job)
        {
            _type = ProgramBaseInfoItemType.Job;
            _number = job.Number;
            _ok = job.OK;
            _description = "";
        }
    }

}
