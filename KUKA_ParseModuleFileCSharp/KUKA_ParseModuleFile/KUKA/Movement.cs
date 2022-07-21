using ParseModuleFile.KUKA.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{
    public class Movement : NotifyPropertyChanged
    {
        private Variable point;
        protected int homeNum;
        private string pointName;
        private string fdatName;
        private FDAT fdat;
        private Enums.Approximate_Positioning approx;

        public int HomeNum { get { return homeNum; } set { Set(ref homeNum, value); } }
        public string PointName { get { return pointName; } set { Set( ref pointName, value); } }
        public string FDATName { get { return fdatName; } set { Set(ref fdatName, value); } }
        public FDAT FDAT { get { return fdat; } set { Set(ref fdat, value); } }
        public Enums.Approximate_Positioning Approx { get { return approx; } set { Set(ref approx, value); } }

        public Variable Point
        {
            get { return point; }
            set { point = value; }
        }

        public static void Parse(ref oldFold fold)
        {
            throw new NotImplementedException();
        }

        static internal string getReValue(string patern, string search)
        {
            return Application.getReValue(patern, search);
        }

        static internal int getReInteger(string patern, string search, oldFold fold)
        {
            return Application.getReInteger(patern, search, fold);
        }

        static internal double getReDouble(string patern, string search, oldFold fold)
        {
            return Application.getReDouble(patern, search, fold);
        }



        #region "Movement"

        #endregion
    }
}
