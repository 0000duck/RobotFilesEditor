using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonLibrary.DataClasses;

namespace BaseManager.Model
{
    public partial class RobotBase : ObservableObject
    {
        List<string> basesFromStandard;

        [ObservableProperty]
        int number;

        [ObservableProperty]
        List<string> foundNames;

        [ObservableProperty]
        string baseName;

        [ObservableProperty]
        string selectedName;

        [ObservableProperty]
        BaseCheckState checkState;

        [ObservableProperty]
        string message;

        public RobotBase(int number, List<string> foundNames, List<string> baseNamesFromStandard)
        {
            Number = number;
            FoundNames = foundNames;
            SelectedName = FoundNames.FirstOrDefault();
            BaseName = FoundNames.Count > 0 ? FoundNames[0] : string.Empty;
            basesFromStandard = baseNamesFromStandard;
            Validate();
        }

        internal void UpdateSelectedValue(ObservableCollection<int> availableBases)
        {
            int oldNum = Number;
            Number = availableBases.FirstOrDefault(x => x == oldNum);
        }

        private void Validate()
        {
            if (foundNames.Count > 1)
            {
                CheckState = BaseCheckState.Error;
                Message = "Multiple names found for base";
                return;
            }
            Regex regexBaseName = new Regex(basesFromStandard[Number], RegexOptions.IgnoreCase);
            if (FoundNames.Count == 0)
            {
                CheckState = BaseCheckState.Error;
                Message = "No names found for base";
                return;
            }
            string matchBaseName = regexBaseName.Match(FoundNames[0]).ToString();
            if (matchBaseName != FoundNames[0])
            {
                CheckState = BaseCheckState.Warning;
                Message = "Name of base on according to standard";
                return;
            }

        }
    }
}
