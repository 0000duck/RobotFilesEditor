using CommonLibrary;
using Microsoft.CSharp;
using RobotFilesEditor.Dialogs.SelectJob;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Common = CommonLibrary.CommonMethods;

namespace RobotFilesEditor.Model.Operations
{
    public static class SasFillerFromBackupMethods
    {
        private static Excel.Application oXL;
        private static Excel.Workbooks oWBs;
        private static Excel.Workbook oWB;
        private static Excel.Sheets sheets;
        private static Excel.Worksheet oSheet;
        private static IDictionary<string,SortedDictionary<int, List<int>>> jobAreaAssignment;

        internal static void Execute()
        {
            jobAreaAssignment = new Dictionary<string, SortedDictionary<int, List<int>>>();
            MessageBox.Show("Select SAS file", "Select File", MessageBoxButton.OK, MessageBoxImage.Information);
            string sasfile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "SAS file", "*.sasz");
            if (string.IsNullOrEmpty(sasfile))
                return;
            MessageBox.Show("Select directory containing backups", "Select File", MessageBoxButton.OK, MessageBoxImage.Information);
            string backupDirectory = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            if (string.IsNullOrEmpty(backupDirectory))
                return;
            List<string> foundBackups = CommonLibrary.CommonMethods.FindBackupsInDirectory(backupDirectory, false, includeSafeRobot:false);
            //Backups = CovertListToObsCollection(foundBackups);
            XElement sasXml = GetSasMainFile(sasfile);
            if (sasXml == null)
            {
                MessageBox.Show("Error while reading sas file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SortedDictionary<string, List<AreaClass>> areas = new SortedDictionary<string, List<AreaClass>>();
            IDictionary<string, bool> isKRC4Dict = new Dictionary<string, bool>();
            IDictionary<string, SortedDictionary<int, OrgFromBackup>> orgsFromBackpus = GetOrgsFromBackups(foundBackups, out areas, out isKRC4Dict);
            Task.Run(() => WriteAreasXLS(areas));
            if (orgsFromBackpus == null)
            {
                return;
            }
            List<RobotFromSas> robotsInSas = FindRobotsInSas(sasXml);

            Dialogs.SasFiller.SasFillerAssignRobotsViewModel vm = new Dialogs.SasFiller.SasFillerAssignRobotsViewModel(CreateRobotDataObsCollection(robotsInSas) , CovertListToObsCollection(foundBackups));
            Dialogs.SasFiller.SasFillerAssignRobots sW = new Dialogs.SasFiller.SasFillerAssignRobots(vm);
            var dialogResult = sW.ShowDialog();
            if (dialogResult == false)
                return;
            SortedDictionary<string, RobotSafetyAndHomes> isSafeRobot = CheckSafeRobot(foundBackups);
            List<RobotFromSas> assignedRobots = CorrectBackupPath(vm.Robots,foundBackups, orgsFromBackpus, isSafeRobot, isKRC4Dict);
            string workFile = CreateSasCopy(sasfile);
            UpdateSas(workFile, assignedRobots);
        }

        private static void WriteAreasXLS(SortedDictionary<string, List<AreaClass>> areas)
        {
            try
            {
                var revAreas = areas.Reverse();
                int counter = 1;
                oXL = new Excel.Application();
                oWBs = oXL.Workbooks;
                oWB = oWBs.Add("");
                sheets = oWB.Sheets;
                Excel.Range aRange = null;
                oSheet = sheets[1] as Excel.Worksheet;
                foreach (var backup in revAreas)
                {
                    int rangeStart = 0;
                    int currentLine = 1;
                    if (counter > 1)
                        oWB.Worksheets.Add(oSheet);
                    oSheet = oWB.ActiveSheet;
                    oSheet.Name = backup.Key.Length <= 32 ? backup.Key : backup.Key.Substring(0, 31);
                    oSheet.Cells[1, 1] = "Area number";
                    oSheet.Cells[1, 2] = "Descriptions";
                    oSheet.Cells[1, 3] = "Used in paths";
                    aRange = oSheet.Range["A1", "C1"];
                    aRange.Font.Bold = true;

                    foreach (var area in backup.Value)
                    {
                        rangeStart = currentLine + 1;
                        oSheet.Cells[currentLine + 1, 1] = area.Number;
                        currentLine++;
                        int maxLine = area.Descriptions.Count > area.UsedInPaths.Count ? area.Descriptions.Count : area.UsedInPaths.Count;
                        for (int i = 0; i <= maxLine - 1; i++)
                        {
                            if (area.Descriptions.Count > i)
                            {
                                string stringToAdd = area.Descriptions[i];
                                oSheet.Cells[currentLine + i, 2] = stringToAdd;
                            }
                            if (area.UsedInPaths.Count > i)
                            {
                                string stringToAdd = area.UsedInPaths[i];
                                oSheet.Cells[currentLine + i, 3] = stringToAdd;
                            }
                        }
                        currentLine += maxLine - 1;
                        aRange = oSheet.Range["A" + rangeStart, "A" + currentLine];
                        aRange.Merge();
                        aRange.Font.Bold = true;
                        aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                        aRange.Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        aRange.Style.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;                      
                        aRange = oSheet.Range["B" + rangeStart, "B" + currentLine];
                        aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                        //borders = aRange.Borders;
                        //borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        //borders.Weight = 2d;
                        aRange = oSheet.Range["C" + rangeStart, "C" + currentLine];
                        aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                        //borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        //borders.Weight = 2d;
                    }
   
                    counter++;
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                }

                oXL.Visible = true;
                Marshal.FinalReleaseComObject(aRange);
                Marshal.FinalReleaseComObject(oSheet);
                Marshal.FinalReleaseComObject(oWB);
                Marshal.FinalReleaseComObject(oWBs);
                Marshal.FinalReleaseComObject(oXL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong with areas Excel file generation", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private static SortedDictionary<string, RobotSafetyAndHomes> CheckSafeRobot(List<string> foundBackups)
        {
            Regex homeNumRegex = new Regex(@"(?<=PTP\s+XHOME)\d+", RegexOptions.IgnoreCase);
            SortedDictionary<string, RobotSafetyAndHomes> result = new SortedDictionary<string, RobotSafetyAndHomes>();
            foreach (var backup in foundBackups)
            {
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    string backupName = Path.GetFileNameWithoutExtension(backup).ToLower();
                    List<int> usedHomes = new List<int>();
                    result.Add(backupName, new RobotSafetyAndHomes());
                    var entries = archive.Entries.Where(x => Path.GetExtension(x.Name) == ".src").Where(y=>y.FullName.ToLower().Contains("bmw_program") || y.FullName.ToLower().Contains("bmw_utilities"));
                    if (entries.Any(x => Path.GetFileNameWithoutExtension(x.FullName).ToLower().Contains("a01_masterreference")))
                        result[backupName].IsSafe = true;
                    else
                        result[backupName].IsSafe = false;
                    foreach (var entry in entries)
                    {
                        StreamReader reader = new StreamReader(entry.Open());
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (homeNumRegex.IsMatch(line))
                            {
                                int homenum = int.Parse(homeNumRegex.Match(line).ToString());
                                if (homenum > 2)
                                { }
                                if (!usedHomes.Contains(homenum))
                                    usedHomes.Add(homenum);
                            }
                        }
                        reader.Close();
                        result[backupName].NrOfHomes = usedHomes.Count;
                     }
                    archive.Dispose();
                }              
            }
            return result;
        }

        private static void UpdateSas(string workFile, List<RobotFromSas> assignedRobots)
        {
            List<string> messages = new List<string>();
            XElement sasXml = GetSasMainFile(workFile);
            IDictionary<int, string> types = GetTypeGUID(sasXml);

            try
            {
                foreach (var robot in assignedRobots.Where(x=>x.IsKRC4!=null))
                {
                    SortedDictionary<int, string> specialOrgNames = GetSpecialOrgsName(robot.IsKRC4);
                    SortedDictionary<int, IJobOrAnyjob> allJobs = GetJobsFromOrgs(robot);
                    XElement jobsXelement = GetJobsXElement(allJobs);
                    XElement anyJobsElement = GetAnyJobsXElement(allJobs);
                    sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Robot").Attribute("Safe").Value = robot.IsSafeRobot ? "True" : "False";
                    for (int i = 1; i <= robot.NrOfHomes; i++)
                    {
                        if (i==1)
                            sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Robot").Attribute("Home1").Value = "ohne BT";
                        else if (i==2)
                            sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Robot").Attribute("Home2").Value = "mit BT";
                        else
                            sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Robot").Attribute("Home" + i).Value = "Home "+i;
                    }
                    if (sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("AnyJobs") != null)
                        sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("AnyJobs").Elements("AnyJob").Remove();
                    if (anyJobsElement != null)
                        sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("AnyJobs").Add(anyJobsElement.Elements("AnyJob"));
                    sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Jobs").Elements("Job").Remove();
                    sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Orgs").Elements("Org").Remove();
                    sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Jobs").Add(jobsXelement.Elements("Job"));
                    foreach (var org in robot.OrgsFromBackup)
                    {
                        string orgName = string.Empty;
                        if (org.Value.Jobs.Count > 0)
                            orgName = org.Value.Jobs[0].Name;                        
                        else if (org.Value.Jobs.Count == 0 && (robot.IsKRC4.Value==true && org.Key == 250 || robot.IsKRC4.Value == false && org.Key == 61))
                            orgName = "Justagereferenzierung";
                        else if (org.Value.Jobs.Count == 0 && (robot.IsKRC4.Value == true && org.Key == 251 || robot.IsKRC4.Value == false && org.Key == 62))
                            orgName = "Bremsentest";
                        else
                            orgName = "Unknown org name";

                        if ((robot.IsKRC4.Value == true && org.Key >= 50) || (robot.IsKRC4.Value == false && org.Key >= 25))
                            if (specialOrgNames.Keys.Contains(org.Key))
                                orgName = specialOrgNames[org.Key];
                            else
                                orgName = "Unknown org name";

                        IDictionary<int, List<int>> typesAndJobs = GetTypesAndJobs(org);
                        XElement currXElement = robot.RobotXML;
                        XElement orgElement = new XElement("Org",
                            new XAttribute("Number", org.Key),
                            new XAttribute("DescriptionLong", orgName),
                            new XAttribute("DescriptionShort", orgName),
                            new XAttribute("HomeProcess", "keine"),
                            new XAttribute("HomeAdditional", "keine"),
                            new XAttribute("HomeStart", "Job Freigabe"),
                            new XAttribute("StartHome", org.Value.StartHome)
                            );
                        foreach (var type in typesAndJobs)
                        {
                            if (!types.Keys.Contains(type.Key))
                            {
                                string message = "Sas does not contain type " + type.Key + ".\r\nOrg for this type for robot " + robot.BackupName + " will not be added!";
                                if (!messages.Contains(message))
                                    messages.Add(message);
                            }
                            else
                            {
                                XElement element = new XElement("Type",
                                new XAttribute("Number", types[type.Key]));
                                foreach (var job in type.Value)
                                {
                                    element.Add(new XElement("Job",
                                        new XAttribute("Number", job < 255 ? job.ToString() : ""),
                                        new XAttribute("Name", job < 255 ? "" : org.Value.Jobs.First(x=>x.JobNum == job).Name)));
                                }
                                orgElement.Add(element);
                            }
                        }
                        //var currentorg = sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == robot.Station.ToString()).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value  == Get3Digits(robot.Number)).First().Element("SW").Element("Orgs");                    
                        sasXml.Element("Anlage").Elements("SG").Where(x => x.Attribute("Name").Value == robot.SG).First().Elements("xx").Where(x => x.Attribute("Name").Value == Get3Digits(robot.Station)).First().Elements("x").Where(x => x.Attribute("Gruppe").Value == "IR" && x.Attribute("Name").Value == Get3Digits(robot.Number)).First().Element("SW").Element("Orgs").Add(orgElement);
                    }
                }
                if (messages.Count > 0)
                {
                    string message = string.Empty;
                    messages.ForEach(x => message += x + "\r\n");
                    MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                using (FileStream zipToOpen = new FileStream(workFile, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        archive.Entries.Where(x => x.FullName.ToLower().Contains("main.xml")).First().Delete();
                        ZipArchiveEntry readmeEntry = archive.CreateEntry("main.xml");
                        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                        {
                            string[] alllines = sasXml.ToString().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in alllines)
                                writer.WriteLine(line);
                            writer.Close();
                        }
                        archive.Dispose();
                    }
                    zipToOpen.Close();
                }
                MessageBox.Show("SAS saves succesfully at: " + workFile, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static XElement GetAnyJobsXElement(SortedDictionary<int, IJobOrAnyjob> allJobs)
        {
            if (!allJobs.Any(x => x.Key > 254))
                return null;

            XElement result = new XElement("AnyJobs");
            foreach (var anyjob in allJobs.Where(x => x.Key > 254))
            { 
                if (anyjob.Value != null)
                {
                    XElement element = new XElement("AnyJob",
                        new XAttribute("Name", anyjob.Value.Name));
                    foreach (var job in (anyjob.Value as AnyJobInOrg).JobsInAnyjob)
                    {
                        element.Add(new XElement("Job", new XAttribute("Number", job), new XAttribute("Name", "")));
                    }
                    result.Add(element);
                }
            }
            return result;

        }

        private static SortedDictionary<int, string> GetSpecialOrgsName(bool? isKRC4)
        {
            SortedDictionary<int, string> result = new SortedDictionary<int, string>();
            if (!isKRC4.HasValue)
                return result;

            if (isKRC4.Value == true)
            {
                result.Add(50, "Matrix Prufen");
                result.Add(51, "Spulen");
                result.Add(62, "Kappenfraesen");
                result.Add(63, "Kappenwechseln");
                result.Add(64, "Wartung");
                result.Add(250, "Justagereferenzierung");
                result.Add(251, "Bremsentest");
            }
            else
            {
                result.Add(32, "Wartung");
                result.Add(61, "Justagereferenzierung");
                result.Add(62, "Bremsentest");
                result.Add(30, "Kappenwechseln");
                result.Add(31, "Kappenfraesen");
                result.Add(25, "Matrix Prufen");
                result.Add(28, "Spulen");

            }
            return result;
        }

        private static XElement GetJobsXElement(SortedDictionary<int, IJobOrAnyjob> allJobs)
        {
            XElement result = new XElement("Jobs");
            foreach (var job in allJobs.Where(x=>x.Key < 255))
            {
                if (job.Value == null)
                    result.Add(new XElement("Job", new XAttribute("Number", job.Key.ToString()), new XAttribute("Description", "")));
                else
                {
                    XElement element = new XElement("Job",
                        new XAttribute("Number",job.Key.ToString()),
                        new XAttribute("Description", job.Value.Name));
                    if (job.Value.AssignedToAreas != null)
                    {
                        foreach (var area in job.Value.AssignedToAreas)
                        {
                            element.Add(new XElement("Area", new XAttribute("Number", area.ToString())));
                        }
                    }
                    result.Add(element);
                }
            }
            return result;
        }

        private static SortedDictionary<int, IJobOrAnyjob> GetJobsFromOrgs(RobotFromSas robot)
        {
            SortedDictionary<int, IJobOrAnyjob> result = new SortedDictionary<int, IJobOrAnyjob>();
            for (int i = 1; i < 65; i++)
                result.Add(i, null);
            foreach (var org in robot.OrgsFromBackup)
            {
                foreach (var job in org.Value.Jobs)
                {
                    if (job.JobNum > 254)
                    {
                        if (!result.Keys.Contains(job.JobNum))
                            result.Add(job.JobNum, job);
                        foreach (var jobAndDescr in (job as AnyJobInOrg).JobsAndDescriptions)
                        {
                            if (result[jobAndDescr.Key] == null)
                            {
                                result[jobAndDescr.Key] = new JobsInOrg(jobAndDescr.Key, jobAndDescr.Value, false, job.TypesUsed) { AssignedToAreas = jobAreaAssignment[robot.BackupName][jobAndDescr.Key]};                               
                            }
                        }
                    }
                    else
                    {
                        if (result[job.JobNum] == null)
                            result[job.JobNum] = job;
                    }
                }
            }
            return result;
        }

        private static string Get3Digits(int number)
        {
            string name = number.ToString();
            if (name.Length == 1)
                return "00" + name;
            else if (name.Length == 2)
                return "0" + name;
            else
                return name;
        }

        private static IDictionary<int, List<int>> GetTypesAndJobs(KeyValuePair<int, OrgFromBackup> org)
        {
            IDictionary<int, List<int>> result = new Dictionary<int, List<int>>();
            List<int> foundTypes = new List<int>();
            foreach(var job in org.Value.Jobs)
            {
                foreach (var type in job.TypesUsed)
                {
                    if (!foundTypes.Contains(type))
                        foundTypes.Add(type);
                }
            }
            foreach (var type in foundTypes)
            {
                result.Add(type, new List<int>());
                foreach (var job in org.Value.Jobs.Where(x=>x.TypesUsed.Contains(type)))
                {
                    result[type].Add(job.JobNum);
                }
            }
            return result;

        }

        private static IDictionary<int, string> GetTypeGUID(XElement sasXml)
        {
            var typesInSas = sasXml.Element("Anlage").Element("SW").Element("Types").Elements("Type");
            IDictionary<int, string> result = new Dictionary<int, string>();
            foreach (var typ in typesInSas)
            {
                int typNum = int.Parse(typ.Attribute("Number").Value.ToString());
                if (typNum > 0)
                {
                    string guid = typ.Attribute("GUID").Value.ToString();
                    result.Add(typNum, guid);
                }
            }
            return result;
        }

        private static string CreateSasCopy(string sasfile)
        {
            string date = DateTime.Now.ToString("yyyy'_'MM'_'dd_HH_mm_ss");
            File.Copy(sasfile, sasfile.Replace(".sasz", "_"+date+".sasz"));
            return sasfile.Replace(".sasz", "_" + date + ".sasz");
        }

        private static List<RobotFromSas> CorrectBackupPath(ObservableCollection<RobotAssingmentData> robots, List<string> foundBackups, IDictionary<string, SortedDictionary<int, OrgFromBackup>> orgs, SortedDictionary<string, RobotSafetyAndHomes> isSafeRobots, IDictionary<string, bool> isKRC4Dict)
        {
            List<RobotFromSas> result = new List<RobotFromSas>();
            foreach (var robot in robots)
            {
                SortedDictionary<int, OrgFromBackup> assignedOrg = new SortedDictionary<int, OrgFromBackup>();
                string fullpath = string.Empty;
                bool isSafe = false;
                int nrOfHomes = 0;
                if (robot.RobotsFromBackups.Length > 0)
                {
                    fullpath = foundBackups.Where(x => x.Contains(robot.RobotsFromBackups)).First();
                    assignedOrg = orgs[robot.RobotsFromBackups];
                    isSafe = isSafeRobots[robot.RobotsFromBackups.ToLower()].IsSafe;
                    nrOfHomes = isSafeRobots[robot.RobotsFromBackups.ToLower()].NrOfHomes;
                }
                bool? isKRC4 = null;
                if (isKRC4Dict.Keys.Contains(robot.RobotsFromBackups))
                    isKRC4 = isKRC4Dict[robot.RobotsFromBackups];

                result.Add(new RobotFromSas(fullpath, robot.RobotsFromSas.SG, robot.RobotsFromSas.RobotXML, robot.RobotsFromSas.Station, robot.RobotsFromSas.Number, isKRC4) { BackupName = robot.RobotsFromBackups , OrgsFromBackup = assignedOrg, IsSafeRobot = isSafe, NrOfHomes = nrOfHomes  } );
            }
            return result;
        }

        private static ObservableCollection<RobotAssingmentData> CreateRobotDataObsCollection(List<RobotFromSas> robotsInSas)
        {
            ObservableCollection<RobotAssingmentData> result = new ObservableCollection<RobotAssingmentData>();
            robotsInSas.ForEach(x => result.Add(new RobotAssingmentData() { RobotsFromSas = x, RobotsFromBackups = string.Empty }));
            return result;
        }

        private static ObservableCollection<string> CovertListToObsCollection(List<string> foundBackups)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            foundBackups.ForEach(x => result.Add(x));
            return result;
        }

        private static List<RobotFromSas> FindRobotsInSas(XElement sasXml)
        {
            List<RobotFromSas> result = new List<RobotFromSas>();
            List<XElement> robots = new List<XElement>();
            var sgs = sasXml.Element("Anlage").Elements("SG");
            foreach(var sg in sgs)
            {
                string sgNum = sg.Attribute("Name").Value;
                var xxElements = sg.Elements("xx");
                foreach (var xxElement in xxElements)
                {
                    var stationNr = xxElement.Attribute("Name").Value;
                    var xElements = xxElement.Elements("x");
                    var robotsinSg = xElements.Where(x => x.Attribute("Gruppe").Value == "IR");
                    foreach(var robot in robotsinSg)
                    {
                        var robotnum = robot.Attribute("Name").Value;
                        result.Add(new RobotFromSas((stationNr + "IR" + robotnum),sgNum,robot,int.Parse(stationNr),int.Parse(robotnum),null));
                    }
                }
            }
            return result;
        }

        private static IDictionary<string, SortedDictionary<int, OrgFromBackup>> GetOrgsFromBackups(List<string> foundBackups, out SortedDictionary<string, List<AreaClass>> returnedAreas, out IDictionary<string,bool> isKRC4Dict)
        {
            bool isKRC4 = false;
            isKRC4Dict = new Dictionary<string, bool>();
            IDictionary<string, SortedDictionary<int, OrgFromBackup>> result = new Dictionary<string, SortedDictionary<int, OrgFromBackup>>();
            returnedAreas = new SortedDictionary<string, List<AreaClass>>();
            if (ContainsDoubleBackup(foundBackups))
            {
                returnedAreas = null;
                return null;
            }
            foreach (var backup in foundBackups)
            {
                string backupName = Path.GetFileNameWithoutExtension(backup);
                List<AreaClass> areas= new List<AreaClass>();
                IDictionary<int, string> orgsInCell = GetOrgsFromCell(backup, out isKRC4);
                SortedDictionary<int, string> jobdescr;
                SortedDictionary<int, OrgFromBackup> orgsfrombackup = GetOrgsFromBackup(backup, orgsInCell, out jobdescr, isKRC4);
                SortedDictionary<int, List<int>> jobsAssignedToAreas = AssignJobsToAreas(orgsfrombackup,backup, out areas, isKRC4);
                jobAreaAssignment.Add(backupName,jobsAssignedToAreas);
                areas = areas.OrderBy(o => o.Number).ToList();
                orgsfrombackup = UpdateOrgsFromBackup(orgsfrombackup, jobsAssignedToAreas,jobdescr);
                returnedAreas.Add(backupName, areas);
                result.Add(backupName, orgsfrombackup);
                isKRC4Dict.Add(backupName, isKRC4);

            }
            return result;
            
        }

        private static bool ContainsDoubleBackup(List<string> foundBackups)
        {
            List<string> alreadyScanned = new List<string>();
            List<string> doubleBackups = new List<string>();
            foreach (var backup in foundBackups)
            {
                string backupName = Path.GetFileNameWithoutExtension(backup).ToLower(); ;
                if (!alreadyScanned.Contains(backupName))
                    alreadyScanned.Add(backupName);
                else
                    doubleBackups.Add(backupName);
            }
            if (doubleBackups.Count == 0)
                return false;
            else
            {
                string message = string.Empty;
                doubleBackups.ForEach(x => message += (x + "\r\n"));
                MessageBox.Show("Multiple backup name: \r\n" + message + "Program will abort", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
        }

        private static SortedDictionary<int, OrgFromBackup> UpdateOrgsFromBackup(SortedDictionary<int, OrgFromBackup> orgsfrombackup, SortedDictionary<int, List<int>> jobsAssignedToAreas, SortedDictionary<int, string> jobDescr)
        {
            SortedDictionary<int, OrgFromBackup> result = new SortedDictionary<int, OrgFromBackup>();
            foreach (var org in orgsfrombackup)
            { 
                List<IJobOrAnyjob> jobsInOrgs = new List<IJobOrAnyjob>();
                foreach (var job in org.Value.Jobs)
                {
                    if (job is AnyJobInOrg)
                    {
                        AnyJobInOrg currentJob = new AnyJobInOrg(job.JobNum, GetAnyJobName(job), job.TypNumReq, job.TypesUsed, (job as AnyJobInOrg).JobsInAnyjob, (job as AnyJobInOrg).JobsAndDescriptions);
                        foreach (var jobInAnyjob in (job as AnyJobInOrg).JobsInAnyjob)
                        {
                            if (currentJob.AssignedToAreas == null)
                                currentJob.AssignedToAreas = new List<int>();
                            if (!currentJob.AssignedToAreas.Contains(jobInAnyjob))
                                currentJob.AssignedToAreas.Add(jobInAnyjob);
                            
                        }
                       
                        jobsInOrgs.Add(currentJob);
                    }
                    else
                    {
                        JobsInOrg currentJob = new JobsInOrg(job.JobNum, job.Name, job.TypNumReq, job.TypesUsed);
                        if (jobsAssignedToAreas.Keys.Contains(job.JobNum))
                        {
                            List<int> arealist = jobsAssignedToAreas[job.JobNum];
                            arealist.Sort();
                            currentJob.AssignedToAreas = arealist;
                            jobsInOrgs.Add(currentJob);
                        }
                        else
                            jobsInOrgs.Add(job);
                    }
                }
                result.Add(org.Key, new OrgFromBackup() { Number = org.Key , Jobs = jobsInOrgs, OrgName = "Org" + org.Key, StartHome = org.Value.StartHome});
            }

            return result;
        }

        private static string GetAnyJobName(IJobOrAnyjob job)
        {
            string result = string.Empty;
            foreach (var item in (job as AnyJobInOrg).JobsAndDescriptions)
            {
                result += item.Value + "\\";
            }
            result = result.Remove(result.Length - 1, 1);
            return result;
        }

        private static SortedDictionary<int, List<int>> AssignJobsToAreas(SortedDictionary<int, OrgFromBackup> orgsfrombackup,string backup, out List<AreaClass> returnedareas, bool isKRC4)
        {
            List<int> joblist = GetJobListInCurrentBackups(orgsfrombackup);
            List<AreaClass> areas = new List<AreaClass>();
            SortedDictionary<int, List<int>> result = AssingAreasFromBackup(joblist, backup, out areas, isKRC4);
            returnedareas = areas;
            return result;
        }

        private static SortedDictionary<int, List<int>> AssingAreasFromBackup(List<int> joblist, string backup, out List<AreaClass> returnedAreas, bool isKRC4)
        {
            List<AreaClass> areas = new List<AreaClass>();
            Regex isJobRegex, areanumRegex;
            if (isKRC4)
            {
                isJobRegex = new Regex(@"(?<=Plc_Job\s*\(\s*\d+\s*,\s*)\d+", RegexOptions.IgnoreCase);
                areanumRegex = new Regex(@"(?<=(Plc_AreaReq|Plc_AreaRelease)\s*\(\s*)\d+", RegexOptions.IgnoreCase);
            }
            else
            {
                isJobRegex = new Regex(@"(?<=JOB.(FINISHED_(PTP|LIN)|REQUEST).+JobNr\s*\=\s*)\d+", RegexOptions.IgnoreCase);
                areanumRegex = new Regex(@"(?<=Area\.(Request|Release)\s+AreaNr\s*\=\s*)\d+", RegexOptions.IgnoreCase);
            }
            SortedDictionary<int, List<int>> result = new SortedDictionary<int, List<int>>();
            using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
            {
                var entries = archive.Entries.Where(x => Path.GetExtension(x.Name) == ".src");
                string backupName = Path.GetFileNameWithoutExtension(backup);
                //areas.Add(backupName, new List<AreaClass>());
                foreach(var entry in entries)
                {
                    StreamReader reader = new StreamReader(entry.Open());
                    string filecontent = reader.ReadToEnd();
                    if (isJobRegex.IsMatch(filecontent) && isJobRegex.Match(filecontent).ToString() != "0" && isJobRegex.Match(filecontent).ToString() != "255")
                    {
                        int foundJobNum = int.Parse(isJobRegex.Match(filecontent).ToString());
                        if (areanumRegex.IsMatch(filecontent))
                        {
                            if (!result.Keys.Contains(foundJobNum))
                                result.Add(foundJobNum, new List<int>());
                            var foundAreas = areanumRegex.Matches(filecontent);
                            foreach(var match in foundAreas)
                            {
                                int areanum = int.Parse(match.ToString());
                                Regex getAreaDescriptions;
                                if (isKRC4)
                                    getAreaDescriptions = new Regex(@"(?<=Area\s+(Request|Release)\s+AreaNum\s*:\s*" + match.ToString() + @"[\s\w:]+Desc\s*:\s*).*", RegexOptions.IgnoreCase);
                                else
                                    getAreaDescriptions = new Regex(@"(?<=Area\.(Request|Release).+DESC\s*\=\s*)[\w\s\+_]+", RegexOptions.IgnoreCase);
                                if (!areas.Any(x => x.Number == areanum))
                                    areas.Add(new AreaClass(areanum));                                
                                var area = areas.First(x => x.Number == areanum);
                                string path = Path.GetFileNameWithoutExtension(entry.FullName);
                                if (!area.UsedInPaths.Any(x=>x.ToLower() == path.ToLower()))
                                    areas.First(x => x.Number == areanum).UsedInPaths.Add(path);
                                List<string> descriptions = new List<string>();
                                var descrMatches = getAreaDescriptions.Matches(filecontent);
                                foreach (var matchDescr in descrMatches)
                                {
                                    if (!descriptions.Contains(matchDescr.ToString()))
                                        descriptions.Add(matchDescr.ToString().Trim());
                                }
                                foreach (var desc in descriptions.Where(x => !areas.First(y => y.Number == areanum).Descriptions.Contains(x)))
                                    areas.First(x => x.Number == areanum).Descriptions.Add(desc);
                                if (!result[foundJobNum].Contains(areanum))
                                {                                    
                                    result[foundJobNum].Add(areanum);
                                }
                            }
                        }
                    }
                    reader.Close();
                }
                archive.Dispose();
            }
            returnedAreas = areas;
            return result;
        }

        private static List<int> GetJobListInCurrentBackups(SortedDictionary<int, OrgFromBackup> orgsfrombackup)
        {
            List<int> result = new List<int>();
            foreach(var org in orgsfrombackup)
            {
                var currentJobs = org.Value.Jobs;
                foreach (var job in currentJobs)
                {
                    if (!result.Contains(job.JobNum))
                        result.Add(job.JobNum);
                    if (job is AnyJobInOrg)
                    {
                        foreach (var jobInAnyjob in (job as AnyJobInOrg).JobsAndDescriptions)
                        {
                            if (!result.Contains(jobInAnyjob.Key))
                                result.Add(jobInAnyjob.Key);
                        }
                    }
                }
            }
            result.Sort();
            return result;
        }

        private static SortedDictionary<int, OrgFromBackup> GetOrgsFromBackup(string backup, IDictionary<int, string> orgsInCell, out SortedDictionary<int, string> jobDescr, bool isKrc4)
        {
            Regex jobNumRegex = new Regex(@"(?<=(Plc_JobReq|PLC_ReqNum)\s*\([\sa-zA-Z,]*)\d+", RegexOptions.IgnoreCase);
            Regex isTypNumReqRegex = new Regex(@"(?<=(Plc_JobReq|PLC_ReqNum)\s*\((\s*|\s*\d+\s*,\s*\d+\s*,\s*))[a-zA-Z]+", RegexOptions.IgnoreCase);
            Regex typNumRegex = new Regex(@"(?<=CASE\s+)\d+", RegexOptions.IgnoreCase);
            Regex jobFoldLineRegex;
            if (isKrc4)
                jobFoldLineRegex = new Regex(@"(?<=Job\s+(Request|Started|Done)\s+JobNum\s*\:\s*)\d+", RegexOptions.IgnoreCase);
            else
                jobFoldLineRegex = new Regex(@"(?<=JOB.(FINISHED_(PTP|LIN)|REQUEST).+JobNr\s*\=\s*)\d+", RegexOptions.IgnoreCase);
            Regex hasDescription = new Regex(@"(?<=Desc\s*(:|\=)\s*)[\w\s/]+", RegexOptions.IgnoreCase);
            Regex startHomeRegex = new Regex(@"(?<=(WAIT\s+FOR\s+\$IN_HOME|Plc_CheckHome\s*\(\s*))\d+", RegexOptions.IgnoreCase);

            IDictionary<int, List<int>> anyjobs = new Dictionary<int, List<int>>();
            SortedDictionary<int, OrgFromBackup> result = new SortedDictionary<int, OrgFromBackup>();
            string line = string.Empty;
            string currentOrg = string.Empty;
            SortedDictionary<int, List<string>> jobDescriptions = new SortedDictionary<int, List<string>>();
            jobDescr = new SortedDictionary<int, string>();
            try
            {
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {                    
                    var entries = archive.Entries.Where(x => x.Name.ToLower().Contains(".src"));
                    foreach (var entry in entries)
                    {
                        StreamReader reader = new StreamReader(entry.Open());
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            if (jobFoldLineRegex.IsMatch(line))
                            {
                                int jobnum = int.Parse(jobFoldLineRegex.Match(line).ToString());
                                if (!jobDescriptions.Keys.Contains(jobnum))
                                    jobDescriptions.Add(jobnum, new List<string>());
                                if (hasDescription.IsMatch(line))
                                {
                                    string currentMatch = hasDescription.Match(line).ToString();
                                    if (!jobDescriptions[jobnum].Contains(currentMatch))
                                        jobDescriptions[jobnum].Add(currentMatch);
                                }
                            }
                        }
                        reader.Close();
                    }

                    int counter;
                    if (isKrc4)
                        counter = 50;
                    else
                        counter = 25;
                    while (counter <= 64)
                    {
                        if (jobDescriptions.Keys.Contains(counter))
                            jobDescriptions[counter] = new List<string>() { "" };
                        counter++;
                    }
                    
                    foreach(var job in jobDescriptions)
                    {
                        if (job.Value.Count == 1)
                            jobDescr.Add(job.Key, job.Value[0]);
                        else
                        {
                            SelectJobViewModel vm = new SelectJobViewModel(job, Path.GetFileNameWithoutExtension(backup));
                            SelectJob sW = new SelectJob(vm);
                            var dialogResult = sW.ShowDialog();
                            string selectedJob = vm.ResultText.Trim();
                            jobDescr.Add(job.Key, selectedJob);
                        }
                    }
                    
                    foreach (var org in orgsInCell)
                    {
                        var entry = archive.Entries.Where(x => x.Name.ToLower().Contains(org.Value.ToLower() + ".src")).First();
                        List<SwitchInstruction> switches = GetSwitches(entry);
                        anyjobs = UpdateAnyjobs(anyjobs, switches);
                    }
                    foreach (var org in orgsInCell)
                    {
                        int startHome = 0, anyjobnumber = 255, currentAnyJobLevel = 0;
                        bool isLastSwitchAnyJob = false;
                        List<int> lastTypes = new List<int>();
                        currentOrg = org.Value;
                        result.Add(org.Key, new OrgFromBackup());
                        int currentJobNum = 0, switchLevel = 0, lastJobNum=0;
                        JobsInOrg currentJob = new JobsInOrg();
                        bool isTypNumReq = false, typSwitchActive = false, anyjobActive = false;
                        List<int> usedTypes = new List<int>();
                        var entry = archive.Entries.Where(x => x.Name.ToLower().Contains(org.Value.ToLower() + ".src")).First();
                        List<SwitchInstruction> switches = GetSwitches(entry);
                        anyjobs = UpdateAnyjobs(anyjobs, switches);
                        StreamReader reader = new StreamReader(entry.Open());
                        List<int> notUsedTypes = FindNotUsedTypes(reader);
                        reader = new StreamReader(entry.Open());
                        List<int> lastTypesFound = new List<int>();
                        int lastJobWithJobReq = 0;

                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            if (IsEndLine(line))
                                break;
                            if (startHomeRegex.IsMatch(line))
                                startHome = int.Parse(startHomeRegex.Match(line).ToString());
                            if (jobNumRegex.IsMatch(line))
                            {
                                //if (currentJobNum < 255)
                                currentJobNum = int.Parse(jobNumRegex.Match(line).ToString());
                                isTypNumReq = isTypNumReqRegex.Match(line).ToString().ToLower().Contains("true") ? true : false;
                            }
                            string removeCommentLine = Common.RemoveComment(line).ToLower();
                            if (removeCommentLine.Contains("switch") && !removeCommentLine.Contains("endswitch"))
                                switchLevel++;
                            if (removeCommentLine.Contains("endswitch"))
                            {
                                isLastSwitchAnyJob = false;
                                switchLevel--;
                                if (switchLevel == 0)
                                    typSwitchActive = false;
                            }
                            if (removeCommentLine.Contains("switch") && (removeCommentLine.Contains("plc_gettypnum") || removeCommentLine.Contains("mytypnum")))
                                typSwitchActive = true;
                            if (anyjobActive && switchLevel ==2 && removeCommentLine.Contains("switch") && (removeCommentLine.Contains("plc_getjobnum") || removeCommentLine.Contains("myjobnum")))
                            {
                                isLastSwitchAnyJob = true;
                            }
                            bool isJobreq = jobNumRegex.IsMatch(line);
                            bool isCase = typNumRegex.IsMatch(line);

                            if (switchLevel == 1 && typSwitchActive && isCase || isJobreq || isCase && anyjobActive && switchLevel == 2)
                            {
                                if (isJobreq && isTypNumReq)
                                {
                                    if (!result[org.Key].Jobs.Any(x => x.JobNum == currentJobNum) || currentJobNum == 255)
                                    {
                                        string jobdescr = currentJobNum == 255 ? "Anyjob" : jobDescr[currentJobNum];
                                        if (currentJobNum == 255)
                                        {
                                            //result[org.Key].Jobs.Add(new AnyJobInOrg(anyjobnumber, jobdescr, isTypNumReq, new List<int>(), new List<int>()));                                            
                                            anyjobActive = true;
                                        }
                                        else
                                        {
                                            result[org.Key].Jobs.Add(new JobsInOrg(currentJobNum, jobdescr, isTypNumReq, new List<int>()));
                                            anyjobActive = false;
                                        }
                                    }
                                }
                                else if (isCase && isTypNumReq && !isLastSwitchAnyJob)
                                {
                                    lastTypesFound = GetTypesFromCaseLine(line);
                                    if (!CheckIfNotUsedTypesContains(notUsedTypes, lastTypesFound))
                                    {
                                        //lastJobWithJobReq = currentJobNum <= 255 ? currentJobNum : currentJobNum;
                                        lastJobWithJobReq = currentJobNum ;
                                        if (!anyjobActive && (switchLevel > 1 && lastJobWithJobReq >=255 || switchLevel==1 && lastJobWithJobReq<255))
                                            result[org.Key].Jobs.First(x => x.JobNum == currentJobNum).TypesUsed.AddRange(lastTypesFound);
                                    }
                                }
                                else if (isJobreq && !isTypNumReq && !anyjobActive)
                                {
                                    if (!result[org.Key].Jobs.Any(x => x.JobNum == currentJobNum) || currentJobNum == 255)
                                    {
                                        string jobdescr = currentJobNum == 255 ? "Anyjob" : jobDescr[currentJobNum];
                                        if (currentJobNum == 255)
                                        {
                                            currentAnyJobLevel++;
                                            //result[org.Key].Jobs.Add(new AnyJobInOrg(anyjobnumber, jobdescr, isTypNumReq, CloneList(lastTypesFound), new List<int>()));                                            
                                            anyjobActive = true;
                                        }
                                        else
                                        {
                                            result[org.Key].Jobs.Add(new JobsInOrg(currentJobNum, jobdescr, isTypNumReq, CloneList(lastTypesFound)));
                                            anyjobActive = false;
                                        }
                                    }
                                    else
                                        result[org.Key].Jobs.First(x => x.JobNum == currentJobNum).TypesUsed.AddRange(CloneList(lastTypesFound));
                                }
                                else if (isCase && !isTypNumReq && !anyjobActive)
                                {
                                    lastTypesFound = GetTypesFromCaseLine(line);
                                    if (!CheckIfNotUsedTypesContains(notUsedTypes, lastTypesFound))
                                    {
                                        foreach (var type in lastTypesFound)
                                        {
                                            if (result[org.Key].Jobs.Any(x => x.JobNum == lastJobWithJobReq) && !result[org.Key].Jobs.First(x => x.JobNum == lastJobWithJobReq).TypesUsed.Contains(type))
                                                result[org.Key].Jobs.First(x => x.JobNum == lastJobWithJobReq).TypesUsed.AddRange(lastTypesFound);
                                        }
                                    }
                                }
                                else if (isCase && anyjobActive && isLastSwitchAnyJob)
                                {
                                    anyjobActive = false;
                                    int jobNum = int.Parse(typNumRegex.Match(line).ToString());
                                    lastJobNum = jobNum;
                                    int currentAnyJobIndex = anyjobs.First(x => x.Value.Contains(jobNum)).Key;
                                    currentJobNum = currentAnyJobIndex;
                                    if (!result[org.Key].Jobs.Any(x => x.JobNum == currentAnyJobIndex))
                                    {
                                        var currentAnyJob = anyjobs[currentAnyJobIndex];
                                        List<int> usedInTypes = FindUsedTypes(currentAnyJob, switches);
                                        var currentJobsInAnyJob = new AnyJobInOrg(currentAnyJobIndex, "Anyjob", isTypNumReq, usedInTypes, currentAnyJob, FillJobsAndDescriptions(jobDescr, currentAnyJob));
                                        result[org.Key].Jobs.Add(currentJobsInAnyJob);
                                        //    int currentAnyJob = anyjobnumber;
                                        //    int jobNum = int.Parse(typNumRegex.Match(line).ToString());
                                        //    var currentJobsInAnyJob = (result[org.Key].Jobs.First(x => x.JobNum == currentAnyJob) as AnyJobInOrg).JobsInAnyjob;
                                        //    if (!currentJobsInAnyJob.Contains(jobNum))
                                        //        currentJobsInAnyJob.Add(jobNum);
                                    }
                                }
                            }
                        }
                        if (startHome == 0)
                        {
                            if (org.Key < 50)
                                MessageBox.Show("No start home found for org: " + org.Value + "\r\nRobot: " + backup + "\r\nDefault number of 1 will be used","Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
                            startHome = 1;
                        }
                        result[org.Key].StartHome = startHome;
                        reader.Close();
                    }
                    archive.Dispose();
                }
                return result;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while reading " + backup + "\r\nLine: " + line + "\r\nFile: " + currentOrg + "\r\nException: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private static IDictionary<int, string> FillJobsAndDescriptions(SortedDictionary<int, string> jobDescr, List<int> currentAnyJob)
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            currentAnyJob.ForEach(x => result.Add(x, jobDescr[x]));
            return result;
        }

        private static int GetCurrentJobNum(IDictionary<int, List<int>> anyjobs, int lastJobNum)
        {
            throw new NotImplementedException();
        }

        private static List<int> FindUsedTypes(List<int> currentAnyJob, List<SwitchInstruction> switches)
        {
            List<int> result = new List<int>();
            foreach (var jobnum in currentAnyJob)
            {
                foreach (var switchInstr in switches.Where(x=>x.InnerSwitches.Count>0 && (x.SwitchVariable.ToLower().Contains("mytypnum")) || x.SwitchVariable.ToLower().Contains("plc_gettypnum")))
                {
                    var anyjobswithjobnum = switchInstr.InnerSwitches.Where(x => x.Cases.Contains(jobnum));
                    foreach(var job in anyjobswithjobnum.Where(x=>!result.Contains(x.CaseOfParent)))
                    {
                        result.Add(job.CaseOfParent);
                    }
                }
                //var test = switches. Where(x => x.InnerSwitches.Any(y => y.Cases.Any(z=>z == jobnum)));
            }
            return result;
        }

        private static IDictionary<int, List<int>> UpdateAnyjobs(IDictionary<int, List<int>> anyjobs, List<SwitchInstruction> switches)
        {
            int startingAnyJobIndex = 255 + anyjobs.Count;
            int anyjobnum = startingAnyJobIndex;
            IDictionary<int, List<int>> result = new Dictionary<int, List<int>>(anyjobs);
            if (switches.Any(x => x.SwitchVariable.ToLower().Contains("mytypnum")) || switches.Any(x => x.SwitchVariable.ToLower().Contains("plc_gettypnum")))
            {
                foreach (var switchInstruction in switches)
                {
                    foreach (var typeCase in switchInstruction.Cases)
                    {
                        anyjobnum = startingAnyJobIndex;
                        foreach (var innerSwitch in switchInstruction.InnerSwitches.Where(x => x.CaseOfParent == typeCase))
                        {
                            List<int> listOfNewCases = CheckIfCasesAlreadyExits(innerSwitch.Cases, result);
                            if (listOfNewCases.Count > 0)
                            {
                                if (!result.Keys.Contains(anyjobnum))
                                {
                                    result.Add(anyjobnum, new List<int>(listOfNewCases));
                                }
                                else
                                {
                                    //foreach (var caseNum in innerSwitch.Cases.Where(x => !result[anyjobnum].Contains(x)))
                                    result[anyjobnum].AddRange(listOfNewCases);
                                }
                                anyjobnum++;
                            }
                        }
                    }
                }
            }
            else
                return anyjobs;
            return result;
        }

        private static List<int> CheckIfCasesAlreadyExits(List<int> cases, IDictionary<int, List<int>> inputCases)
        {
            List<int> result = new List<int>();
            foreach (var caseNum in cases)
            {
                if (!inputCases.Any(x => x.Value.Any(y => y == caseNum)))
                    result.Add(caseNum);
            }
            return result;
        }

        private static List<SwitchInstruction> GetSwitches(ZipArchiveEntry entry)
        {
            IDictionary<int, SwitchLevels> switchLevels = new Dictionary<int, SwitchLevels>();
            for (int i = 1; i <= 5; i++)
                switchLevels.Add(i, new SwitchLevels() { Ids = new List<int>() });
            Regex switchVarRegex = new Regex(@"(?<=SWITCH\s+)[\w_\-\(\)\s]+", RegexOptions.IgnoreCase);
            Regex caseNumRegex = new Regex(@"(?<=CASE\s+)\d+", RegexOptions.IgnoreCase);
            List<SwitchInstruction> result = new List<SwitchInstruction>();
            int currentSwitchLevel = 0, lastSwitchId =0 ;
            StreamReader reader = new StreamReader(entry.Open());
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (Common.RemoveComment(line).ToLower().Contains("switch") || Common.RemoveComment(line).ToLower().Contains("case"))
                { }
                if (Common.RemoveComment(line).ToLower().Contains("switch") && !Common.RemoveComment(line).ToLower().Contains("endswitch"))
                {
                    lastSwitchId++;
                    if (currentSwitchLevel == 0)
                    {
                        currentSwitchLevel++;
                        switchLevels[currentSwitchLevel].Ids.Add(lastSwitchId);
                        switchLevels[currentSwitchLevel].LastId = lastSwitchId;
                        SwitchInstruction currentSwitch = new SwitchInstruction(lastSwitchId,currentSwitchLevel) { SwitchVariable = switchVarRegex.Match(line).ToString(), InnerSwitches = new List<SwitchInstruction>() , CaseOfParent = 0};
                        result.Add(currentSwitch);
                    }
                    else
                    {
                        currentSwitchLevel++;
                        switchLevels[currentSwitchLevel].Ids.Add(lastSwitchId);
                        switchLevels[currentSwitchLevel].LastId = lastSwitchId;
                        SwitchInstruction currentSwitch = new SwitchInstruction(lastSwitchId, currentSwitchLevel) { SwitchVariable = switchVarRegex.Match(line).ToString(), InnerSwitches = new List<SwitchInstruction>() };
                        result = RefreshResult(currentSwitch, switchLevels, currentSwitchLevel, result);
                    }                        
                }
                if (Common.RemoveComment(line).ToLower().Contains("endswitch"))
                    currentSwitchLevel--;
                if (Common.RemoveComment(line).ToLower().Contains("case"))
                {
                    int caseNum =  int.Parse(caseNumRegex.Match(line).ToString());
                    result = RefreshResult(null, switchLevels, currentSwitchLevel, result,caseNum);
                }
            }
            reader.Close();
    
            return result;
        }

        private static List<SwitchInstruction> RefreshResult(SwitchInstruction currentSwitch, IDictionary<int, SwitchLevels> switchLevels, int currentSwitchLevel, List<SwitchInstruction> input, int caseNum = 0)
        {
            List<SwitchInstruction> result = new List<SwitchInstruction>(input);
            if (currentSwitchLevel == 1 && caseNum > 0)
            {
                result.First(x => x.Id == switchLevels[1].LastId).Cases.Add(caseNum);
                result.First(x => x.Id == switchLevels[1].LastId).MyLastCase = caseNum;
            }
            else if (currentSwitchLevel == 2 && input.Any(x => x.Id == switchLevels[1].LastId))
            {
                if (caseNum == 0)
                    result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.Add(currentSwitch);
                else
                {
                    var currentItem = result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId);
                    currentItem.Cases.Add(caseNum);
                    currentItem.CaseOfParent = result.First(x => x.Id == switchLevels[1].LastId).MyLastCase;
                    currentItem.MyLastCase = caseNum;
                }
            }
            else if (currentSwitchLevel == 3 && input.Any(x => x.InnerSwitches.Any(y => y.Id == switchLevels[2].LastId)))
            {
                if (caseNum == 0)
                    result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId).InnerSwitches.Add(currentSwitch);
                else
                {
                    var currentItem = result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId).InnerSwitches.First(z => z.Id == switchLevels[3].LastId);
                    currentItem.Cases.Add(caseNum);
                    currentItem.CaseOfParent = result.First(x => x.Id == switchLevels[1].LastId).MyLastCase;
                    currentItem.MyLastCase = caseNum;
                }
            }
            else if (currentSwitchLevel == 4 && input.Any(x => x.InnerSwitches.Any(y => y.InnerSwitches.Any(z => z.Id == switchLevels[3].LastId))))
            {
                if (caseNum == 0)
                    result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId).InnerSwitches.First(z => z.Id == switchLevels[3].LastId).InnerSwitches.Add(currentSwitch);
                else
                {
                    var currentItem = result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId).InnerSwitches.First(z => z.Id == switchLevels[3].LastId).InnerSwitches.First(a => a.Id == switchLevels[4].LastId);
                    currentItem.Cases.Add(caseNum);
                    currentItem.CaseOfParent = result.First(x => x.Id == switchLevels[1].LastId).MyLastCase;
                    currentItem.MyLastCase = caseNum;
                }
            }
            else if (currentSwitchLevel == 5 && input.Any(x => x.InnerSwitches.Any(y => y.InnerSwitches.Any(z => z.InnerSwitches.Any(a => a.Id == switchLevels[4].LastId)))))
            {
                if (caseNum == 0)
                    result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId).InnerSwitches.First(z => z.Id == switchLevels[3].LastId).InnerSwitches.First(a => a.Id == switchLevels[4].LastId).InnerSwitches.Add(currentSwitch);
                else
                {
                    var currentItem = result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId).InnerSwitches.First(z => z.Id == switchLevels[3].LastId).InnerSwitches.First(a => a.Id == switchLevels[4].LastId).InnerSwitches.First(a => a.Id == switchLevels[5].LastId);
                    currentItem.Cases.Add(caseNum);
                    currentItem.CaseOfParent = result.First(x => x.Id == switchLevels[1].LastId).MyLastCase;
                    currentItem.MyLastCase = caseNum;
                }
            }
            else if (currentSwitchLevel == 6 && input.Any(x => x.InnerSwitches.Any(y => y.InnerSwitches.Any(z => z.InnerSwitches.Any(a => a.InnerSwitches.Any(b => b.Id == switchLevels[5].LastId))))))
            {
                if (caseNum == 0)
                    result.First(x => x.Id == switchLevels[1].LastId).InnerSwitches.First(y => y.Id == switchLevels[2].LastId).InnerSwitches.First(z => z.Id == switchLevels[3].LastId).InnerSwitches.First(a => a.Id == switchLevels[4].LastId).InnerSwitches.First(b => b.Id == switchLevels[5].LastId).InnerSwitches.Add(currentSwitch);
            }
            else
                return null;
            return result;
        }

        private static bool CheckIfNotUsedTypesContains(List<int> notUsedTypes, List<int> lastTypesFound)
        {
            foreach (var item in lastTypesFound)
            {
                if (notUsedTypes.Contains(item))
                    return true;
            }
            return false;
        }

        private static List<int> FindNotUsedTypes(StreamReader reader)
        {
            Regex isProcCall = new Regex(@"\w+\s*\(\s*\)",RegexOptions.IgnoreCase);
            Regex typNumRegex = new Regex(@"(?<=CASE\s+)\d+", RegexOptions.IgnoreCase);
            int switchLevel = 0;
            bool typSwitchActive = false;
            List<int> result = new List<int>();
            List<int> currentTypes = new List<int>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string removeCommentLine = Common.RemoveComment(line).ToLower();
                if (removeCommentLine.Contains("switch") && !removeCommentLine.Contains("endswitch"))
                    switchLevel++;
                if (removeCommentLine.Contains("endswitch"))
                {
                    switchLevel--;
                    if (switchLevel == 0)
                        typSwitchActive = false;
                }
                if (removeCommentLine.Contains("switch") && (removeCommentLine.Contains("plc_gettypnum") || removeCommentLine.Contains("mytypnum")))
                    typSwitchActive = true;
                bool isCase = typNumRegex.IsMatch(line);
                if (switchLevel == 1 && typSwitchActive && isCase)
                {
                    result.AddRange(GetTypesFromCaseLine(line));
                    currentTypes = GetTypesFromCaseLine(line);
                }
                if (isProcCall.IsMatch(removeCommentLine) && !removeCommentLine.Contains("defaulterror"))
                {
                    currentTypes.ForEach(x => result.Remove(x));
                }
            }
            reader.Close();
            return result;
        }

        private static List<int> CloneList(List<int> lastTypesFound)
        {
            List<int> result = new List<int>();
            lastTypesFound.ForEach(x => result.Add(x));
            return result;
        }

        private static List<int> GetTypesFromCaseLine(string line)
        {
            string lineWithoutComment = Common.RemoveComment(line);
            List<int> result = new List<int>();
            Regex getTypesRegex = new Regex(@"\d+", RegexOptions.IgnoreCase);
            var matches = getTypesRegex.Matches(lineWithoutComment);
            foreach (var match in matches)
            {
                result.Add(int.Parse(match.ToString()));
            }
            return result;
        }

        private static bool IsEndLine(string line)
        {
            string tempstring = line.ToLower().Trim().Replace(" ", "");
            if (tempstring.Length == 3 && tempstring == "end")
                return true;
            return false;
        }

        private static IDictionary<int, string> GetOrgsFromCell(string backup, out bool isKRC4)
        {
            isKRC4 = true;
            IDictionary<int, string> result = new Dictionary<int, string>();
            string line = string.Empty;
            Regex orgNumRegex = new Regex(@"(?<=CASE\s+)\d+", RegexOptions.IgnoreCase);
            Regex isProcCall = new Regex(@".*\(\s*\)", RegexOptions.IgnoreCase);
            Regex procnameRegex = new Regex(@"[a-zA-Z0-9_\-]+(?=\(\s*\))", RegexOptions.IgnoreCase);
            string entrycontent = string.Empty;
            try
            {
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    int orgnum = 0;
                    bool switchfound = false;
                    var entry = archive.Entries.Where(x => x.Name.ToLower().Contains("cell.src")).First();
                    StreamReader reader = new StreamReader(entry.Open());
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        if (line.Trim().Length > 6 && line.Trim().ToLower().Substring(0, 7) == "default")
                            break;
                        if (line.ToLower().Replace(" ", "").Contains("m_pgno") && !line.ToLower().Replace(" ", "").Contains("plc_i_m_pgno"))
                            isKRC4 = false;
                        if (line.ToLower().Replace(" ", "").Contains("m_pgno") && line.ToLower().Replace(" ", "").Contains("switch"))
                            switchfound = true;
                        if (switchfound && orgNumRegex.IsMatch(line))
                            orgnum = int.Parse(orgNumRegex.Match(line).ToString());
                        if (orgnum > 0 && line.Trim().Length > 0 && line.Trim().Substring(0, 1) != ";" && isProcCall.IsMatch(line))
                        {
                            result.Add(orgnum, procnameRegex.Match(line.Trim().Replace(" ","")).ToString());
                        }
                    }
                    reader.Close();
                    archive.Dispose();
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while reading " + backup + "\r\nLine: " + line + "\r\nException: " + ex.Message,"Error", MessageBoxButton.OK,MessageBoxImage.Error);
                return null;
            }
        }

        private static XElement GetSasMainFile(string sasfile)
        {
            ZipArchive archive = null;
            try
            {
                string entrycontent = string.Empty;
                using (archive = ZipFile.Open(sasfile, ZipArchiveMode.Read))
                {
                    var entry = archive.Entries.Where(x => x.Name.Contains("main.xml")).First();
                    entrycontent = new StreamReader(entry.Open()).ReadToEnd();
                    archive.Dispose();
                }
                XElement result = XElement.Parse(entrycontent);                 
                return result;
            }
            catch (Exception ex)
            {
                archive.Dispose();
                return null;
            }
        }
    }
}
