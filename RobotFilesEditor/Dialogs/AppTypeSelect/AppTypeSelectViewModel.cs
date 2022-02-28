using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Dialogs.AppTypeSelect
{
    public class AppTypeSelectViewModel : ViewModelBase
    {
        private string textA;
        public string TextA
        {
            get { return textA; }
            set { Set(ref textA, value); }
        }

        private string textB;
        public string TextB
        {
            get { return textB; }
            set { Set(ref textB, value); }
        }

        private bool varianta;
        public bool VariantA
        {
            get { return varianta; }
            set
            {
                Set (ref varianta, value);
                if (value)
                    ResultType = TextA;
            }
        }

        private bool variantb;
        public bool VariantB
        {
            get { return variantb; }
            set
            {
                Set(ref variantb, value);
                if (value)
                    ResultType = TextB;
            }
        }

        private string resultType;
        public string ResultType
        {
            get { return resultType; }
            set
            {
                Set(ref resultType, value);
            }
        }

        private string headerText;
        public string HeaderText
        {
            get { return headerText; }
            set
            {
                Set(ref headerText, value);
            }
        }

        public AppTypeSelectViewModel(string textAinput, string textBinput, string header)
        {
            HeaderText = header;
            TextA = textAinput.ToUpper();
            TextB = textBinput.ToUpper();
            VariantA = true;
            VariantB = false;

        }
    }
}
