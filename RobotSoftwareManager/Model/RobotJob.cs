using CommonLibrary.DataClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotSoftwareManager.Model
{
    public partial class RobotJob : RobotSoftwareBase
    {
        [ObservableProperty]
        RobotJob selectedJob;
        public RobotJob(int number, List<string> foundNames)
        {
            Number = number;
            FoundNames = foundNames;
            SelectedName = FoundNames.FirstOrDefault(x => x.Length > 0);
            CurrentName = SelectedName;
            Validate();
        }

        public override void Validate()
        {
            if (FoundNames.Count == 0 || FoundNames.Count == 1 && string.IsNullOrEmpty(FoundNames[0]))
            {
                if (string.IsNullOrEmpty(CurrentName))
                {
                    CheckState = RobotSoftCheckState.Error;
                    Message = "No names found for job";
                    return;
                }
            }
            if (string.IsNullOrEmpty(CurrentName))
            {
                CheckState = RobotSoftCheckState.Error;
                Message = "No name selected for job";
                return;
            }
            if (FoundNames.Count > 1)
            {
                CheckState = RobotSoftCheckState.Warning;
                Message = "Multiple names found for job";
                return;
            }
            CheckState = RobotSoftCheckState.OK;
            Message = "OK";
            return;
        }
    }
}
