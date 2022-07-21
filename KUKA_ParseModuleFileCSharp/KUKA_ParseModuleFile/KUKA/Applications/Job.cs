using WarningHelper;
using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA.Applications
{
    public class Job : Application
    {

        private JobType jobType;
        private int number;
        private string abort;

        public int Number { get { return number; } set { Set(ref number, value); } }
        public string Abort { get { return abort; } set { Set(ref abort, value); } }

        public JobType JobType { get { return jobType; } set { Set(ref jobType, value); } }

        public override void Enumerate(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            //Dim x As List(Of Integer) = New List(Of Integer)
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is Job))
                    continue;
                Job appl = (Job) item.Application;
                if (!_baseinfo.hasItem(ProgramBaseInfoItemType.Job, appl.Number))
                {
                    _baseinfo.Add(new ProgramBaseInfoItem(appl));
                }
                //If Not x.Contains(appl.Number) Then x.Add(appl.Number)
            }
            //x.Sort()
            //_baseinfo.jobList = x
            //If _baseinfo.getTypeList(ProgramBaseInfoItemType.Job).Count = 0 Then Return
            //If x.Count = 0 Then Return
            //_warnings.Add(-2, WarningType.Program_Applications, Level.Information, "Using Job numbers", String.Join(",", x))
        }

        public override void Test(oldFolds list, ProgramBaseInfo _baseinfo)
        {
            List<int> openList = new List<int>();
            List<int> closeList = new List<int>();
            int open = 0;
            int close = 0;
            foreach (oldFold item in list)
            {
                if (item.Application == null)
                    continue;
                if (!(item.Application is Job))
                    continue;
                Job appl = (Job)item.Application;
                // item.f.Request = JobType.REQUEST Or  taken out
                if (appl.JobType == JobType.STARTED)
                {
                    open += 1;
                    openList.Add(appl.Number);
                }
                else if (appl.JobType == JobType.DONE)
                {
                    close += 1;
                    closeList.Add(appl.Number);
                }
            }
            if (open != close)
            {
                _baseinfo.jobListOK = false;
                _warnings.Add(-2, WarningType.Program_Applications, Level.Failure, "Job start count doesn't match Job done count");
            }
            foreach (int i in openList)
            {
                if (!closeList.Contains(i))
                {
                    _baseinfo.jobListOK = false;
                    _warnings.Add(-2, WarningType.Program_Applications, Level.Failure, "Job " + i.ToString() + " is not done");
                }
            }
        }

        public Job(Warnings warnings)
        {
            _warnings = warnings;
        }

        public Job(oldFold fold)
        {
            _warnings = fold.Warnings;
            jobType = JobType.UNKNOWN;
            number = -1;
            OK = true;
            Number = GetSingleInteger(fold.Name, "JobNum", fold);
            if (fold.Name.Contains(" Started "))
            {
                jobType = JobType.STARTED;
            }
            else if (fold.Name.Contains(" Request "))
            {
                jobType = JobType.REQUEST;
            }
            else if (fold.Name.Contains(" Done "))
            {
                jobType = JobType.DONE;
            }
            else
            {
                jobType = JobType.UNKNOWN;
                _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Integrity check failed");
                OK = false;
            }
            Dictionary<string, string> @params = GetParams(fold);
            if (@params != null)
            {
                if (@params.ContainsKey("Plc_JobNum"))
                {
                    if ((@params["Plc_JobNum"] == number.ToString()))
                    {
                    }
                    else
                    {
                        _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Integrity check failed");
                        OK = false;
                    }
                }
                else
                {
                    _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not find Plc_JobNum in property \";Params\"!");
                    OK = false;
                }
                if (@params.ContainsKey("Plc_JobAbort"))
                {
                    abort = @params["Plc_JobAbort"];
                }
                else
                {
                    if (jobType == JobType.REQUEST)
                    {
                        _warnings.Add(fold.LineStart, WarningType.Program_Applications, Level.Failure, "No abort action");
                        OK = false;
                    }
                }
            }
            else
            {
                _warnings.Add(fold.LineStart, WarningType.Intern, Level.Failure, "Could not read property \";Params\" in fold Job");
                OK = false;
            }
        }

        public override string ToString()
        {
            return "Job " + jobType.ToString() + " " + Number.ToString();
        }
    }
}
