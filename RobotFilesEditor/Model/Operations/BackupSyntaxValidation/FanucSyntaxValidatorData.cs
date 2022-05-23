using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.BackupSyntaxValidation
{
    public class FanucSyntaxValidatorData
    {
        public string FilePath { get; set; }
        public List<string> WorkbookContent { get; set; }
        public Dictionary<string, List<string>> LSFiles { get; set; }

        public FanucSyntaxValidatorData(string filePath, List<string> workbookContent, Dictionary<string, List<string>> lsFiles)
        {
            FilePath = filePath;
            WorkbookContent = workbookContent;
            LSFiles = lsFiles;
        }
    }

    public class HomeFanuc : ICloneable
    {
        public int Number { get; set; }
        List<double> DegValues { get; set; }
        List<double> RadValues { get; set; }
        public string ErrorState {get; set;}

        public HomeFanuc(int number, List<double> radValues)
        {
            Number = number;
            RadValues = radValues;
            DegValues = new List<double>();
            ErrorState = string.Empty;
        }

        internal HomeFanuc UpdateHome(string inputString)
        {
            HomeFanuc result = (HomeFanuc)this.Clone();
            Regex valuesRegex = new Regex(@"(?<=(J1|J2|J3|J4|J5|J6|EXT1|EXT2)\s*(\=|:)\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            MatchCollection matches = valuesRegex.Matches(inputString);
            int couter = 0;
            foreach (var jointValue in matches)
            {
                double currentValue = double.Parse(jointValue.ToString(),CultureInfo.InvariantCulture);
                result.DegValues.Add(currentValue);
                result.ErrorState += CompareDegAndRad(result.DegValues[couter], result.RadValues[couter], couter + 1);
                couter++;
            }
            return result;
        }

        private string CompareDegAndRad(double degValue, double radValue, int axisNum)
        {
            double delta = 0.1;
            if (axisNum < 7)
            {
                double radiansConvertedToDegrees = CommonLibrary.CommonMethods.ConvertToDegrees(radValue);                
                if (Math.Abs(degValue - radiansConvertedToDegrees) > delta)
                    return "A" + axisNum.ToString() + " ";
            }
            else
            {
                if (Math.Abs(degValue - radValue) > delta)
                    return "E" + (axisNum - 6).ToString() + " ";
            }
            return string.Empty;
        }

        public object Clone()
        {
            return new HomeFanuc(this.Number, this.RadValues);
        }

    }
}
