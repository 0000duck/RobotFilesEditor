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

namespace RobotSoftwareManager.Model
{
    public partial class RobotBase : RobotSoftwareBase
    {
        List<string> basesFromStandard;

        [ObservableProperty]
        RobotBase selectedBase;

        public RobotBase(int number, List<string> foundNames, List<string> baseNamesFromStandard)
        {
            Number = number;
            FoundNames = foundNames;
            SelectedName = FoundNames.FirstOrDefault(x => x.Length > 0);
            CurrentName = SelectedName;
            basesFromStandard = baseNamesFromStandard;
            Validate();
        }

        public override void Validate()
        {
            if (FoundNames.Count == 0)
            {
                CheckState = RobotSoftCheckState.Error;
                Message = "No names found for base";
                return;
            }
            if (basesFromStandard != null)
            {
                Regex regexBaseName = new Regex(basesFromStandard[Number], RegexOptions.IgnoreCase);
                string matchBaseName = regexBaseName.Match(CurrentName).ToString();
                if (matchBaseName != CurrentName)
                {
                    CheckState = RobotSoftCheckState.Warning;
                    Message = "Name of base not according to standard";
                    return;
                }
            }
            if (FoundNames.Count > 1)
            {
                CheckState = RobotSoftCheckState.Error;
                Message = "Multiple names found for base";
                return;
            }
            CheckState = RobotSoftCheckState.OK;
            Message = "OK";
            return;
        }
    }
}
