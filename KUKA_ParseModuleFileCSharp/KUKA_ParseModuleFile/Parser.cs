using ParseModuleFile.KUKA;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarningHelper;

namespace ParseModuleFile
{
    public class Parser : MyNotifyPropertyChanged
    {
        #region fields
        private File.Dat dat;
        private File.Src src;
        private File.Olp olp;
        private ObservableCollection<Variables> variables = new ObservableCollection<Variables>();
        private Warnings warnings;
        #endregion // fields

        #region properties
        public File.Dat Dat { get { return dat; } set { Set(ref dat, value); } }
        public File.Src Src { get { return src; } set { Set(ref src, value); } }
        public File.Olp Olp { get { return olp; } set { Set(ref olp, value); } }
        public ObservableCollection<Variables> Variables { get { return variables; } set { Set(ref variables, value); } }
        #endregion // properties

        #region constructors
        public Parser(string FileName, Stream stream, Warnings Warnings)
        {
            this.warnings = Warnings;
            string ext = Path.GetExtension(FileName);
            if (ext == ".DAT") dat = new File.Dat(FileName, stream , warnings);
            else if (ext == ".SRC") src = new File.Src(FileName, stream, warnings);
            else if (ext == ".OLP") olp = new File.Olp(FileName, stream, warnings);
        }
        #endregion // constructors

        #region methods

        #endregion // methods

    }
}
