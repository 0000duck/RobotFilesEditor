using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using CommonLibrary;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RobotFilesEditor.Model.DataInformations;
using System.IO.Compression;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using RobotFilesEditor.Dialogs;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace RobotFilesEditor.Model.Operations
{
    public static class ReadSpotsMethods
    {
        private static string[] omitedLocalPoints = { "reload" };
        //static int spotNumberColumnInMPL = 0;
        //static int xColumnInMPL = 1;
        //static int yColumnInMPL = 1;
        //static int zColumnInMPL = 1;
        //static int firstSpotRow = 1;
        //static string sheetName = "test";
        static DatasInExcel excelParams;

        public static string errorMessage;
        private static Excel.Application destinationXlApp;
        private static Excel.Workbook destinationXlWorkbook;
        private static Excel.Worksheet destinationXlWorksheet;
        private static Excel.Application mplXlApp;
        private static Excel.Workbooks mplXlWorkbooks;
        private static Excel.Workbook mplXlWorkbook;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        static PleaseWaitDialog waitdialog;
        private enum FormatType { GreaterThan5, Compare2Values };

        internal static async Task Execute(string robotType)
        {
            try {
                bool isVW = false;
                errorMessage = "";
                string selectedDir = CommonMethods.SelectDirOrFile(true);
                if (string.IsNullOrEmpty(selectedDir))
                    return;
                waitdialog = new PleaseWaitDialog();
                waitdialog.Show();
                IDictionary<string, IWeldpoint> resultWeldPoints = new Dictionary<string, IWeldpoint>();
                if (robotType == "KUKA")
                {
                    List<string> foundBackups = CommonLibrary.CommonMethods.FindBackupsInDirectory(selectedDir);
                    IDictionary<string, WeldpointBMW> weldPoints = await FindWeldPoints(foundBackups);
                    IDictionary<string, List<SpotsInFile>> usedSpots = FindUsedPointsInSrcFiles(foundBackups);
                    IDictionary<string, WeldpointBMW> usedSpotsDict = CreateDictWithUsedPoints(usedSpots);
                    var templist = CompareGlobalAndSrc(weldPoints as dynamic, usedSpotsDict);
                    resultWeldPoints = ConvertToIWeld(templist);
                    resultWeldPoints = FindLocalCoords(resultWeldPoints, selectedDir);
                }
                else if (robotType == "ABB")
                {
                    List<string> modFiles = Directory.GetFiles(selectedDir, "*.mod", SearchOption.AllDirectories).ToList();
                    resultWeldPoints = FindWeldPointsABB(modFiles, out isVW);
                }
                else
                    return;
                waitdialog.Close();
                IDictionary<int, WeldpointBMW> pointsInBosch = new Dictionary<int, WeldpointBMW>();
                if (!isVW)
                {                   
                    System.Windows.Forms.DialogResult dialogBackupFromBosch = System.Windows.Forms.MessageBox.Show("Do you want to compare points backup from Bosch?", "Compare with Bosch?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                    if (dialogBackupFromBosch == System.Windows.Forms.DialogResult.Yes)
                    {
                        pointsInBosch = GetPoitsFromBosch();
                    }
                }
                System.Windows.Forms.DialogResult dialogMPL = System.Windows.Forms.MessageBox.Show("Do you want to compare points with MPL list?", "Compare with MPL?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

                if (dialogMPL == System.Windows.Forms.DialogResult.Yes)
                {
                    if (excelParams == null)
                    {
                        excelParams = new DatasInExcel(ConfigurationManager.AppSettings["MPLSpotNumberColumn"]
                        , ConfigurationManager.AppSettings["MPLXCoord"]
                        , ConfigurationManager.AppSettings["MPLYCoord"]
                        , ConfigurationManager.AppSettings["MPLZCoord"]
                        , ConfigurationManager.AppSettings["MPLFirstPointRow"]
                        , ConfigurationManager.AppSettings["MPLSheetName"]
                        , ConfigurationManager.AppSettings["MPLPuntkType"]
                        , ConfigurationManager.AppSettings["MPLSpotIndex"]);
                    }
                    MPLListParamsVM vm = new MPLListParamsVM(excelParams);
                    MPLListParams dialog = new MPLListParams(vm);
                    var dialogResult = dialog.ShowDialog();
                    FillAppConfig(vm);
                    CreateExcel(resultWeldPoints, selectedDir + "\\spotList_vs_mpl.xlsx", pointsInBosch, true, vm.SelectedType, isVW);
                }
                else if (dialogMPL == System.Windows.Forms.DialogResult.No)
                {
                    var dialogStatusAndTurn = System.Windows.Forms.MessageBox.Show("Do you want to add A,B,C, Status and Turn columns to excel?", "?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                    if (dialogStatusAndTurn == System.Windows.Forms.DialogResult.No)
                        CreateExcel(resultWeldPoints, selectedDir + "\\spotList.xlsx", pointsInBosch, false, "", isVW);
                    else
                        CreateExcel(resultWeldPoints, selectedDir + "\\spotList.xlsx", pointsInBosch, false, "", isVW,true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CleanUpExcel("destination");
                CleanUpExcel("mpl");
                waitdialog.Close();
            }
        }

        public static IDictionary<string, IWeldpoint> FindLocalCoords(IDictionary<string, IWeldpoint> resultWeldPoints, string selectedDir)
        {
            string doublePoits = string.Empty;
            Regex getCoordsRegex = new Regex(@"(?<=(X|Y|Z|A|B|C|S|T)\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            IDictionary<string, IWeldpoint> result = new Dictionary<string, IWeldpoint>();
            List<string> localPointList = new List<string>();
            foreach (var point in resultWeldPoints)
            {
                if (!point.Key.Contains("_LOCAL"))
                    result.Add(point);
                else
                    localPointList.Add(point.Value.PointFullName);
            }
            foreach (var localPoint in localPointList)
            {
                var currentPoint = resultWeldPoints.First(x => x.Value.PointFullName == localPoint);
                string backupPath = currentPoint.Value.PathToBackup;
                using (ZipArchive archive = ZipFile.Open(backupPath, ZipArchiveMode.Read))
                {
                    foreach (var dat in archive.Entries.Where(x => Path.GetExtension(x.FullName) == ".dat"))
                    {
                        StreamReader reader = new StreamReader(dat.Open());
                        string filecontent = reader.ReadToEnd().ToLower();
                        reader.Close();
                        if (filecontent.Contains(currentPoint.Value.PointFullName))
                        {
                            reader = new StreamReader(dat.Open());
                            while(!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (line.ToLower().Contains(localPoint.ToLower()))
                                {
                                    var pointToAdd = currentPoint;
                                    MatchCollection matches = getCoordsRegex.Matches(line);
                                    pointToAdd.Value.XPos = double.Parse(matches[0].ToString(), CultureInfo.InvariantCulture);
                                    pointToAdd.Value.YPos = double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture);
                                    pointToAdd.Value.ZPos = double.Parse(matches[2].ToString(), CultureInfo.InvariantCulture);
                                    pointToAdd.Value.A = double.Parse(matches[3].ToString(), CultureInfo.InvariantCulture);
                                    pointToAdd.Value.B = double.Parse(matches[4].ToString(), CultureInfo.InvariantCulture);
                                    pointToAdd.Value.C = double.Parse(matches[5].ToString(), CultureInfo.InvariantCulture);
                                    pointToAdd.Value.Status = int.Parse(matches[6].ToString(), CultureInfo.InvariantCulture);
                                    pointToAdd.Value.Turn = int.Parse(matches[7].ToString(), CultureInfo.InvariantCulture);
                                    if (!result.Keys.Contains(pointToAdd.Key))
                                        result.Add(pointToAdd);
                                    else
                                    {
                                        doublePoits += pointToAdd.Value.PointFullName + ",\r\n";
                                    }
                                }
                            }
                            reader.Close();
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(doublePoits))
            {
                Task.Run(()=>DisplayMessage("Double declarations\n\r:" + doublePoits));
            }
            return result;
        }

        private static void DisplayMessage(string v)
        {
            MessageBox.Show(v, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private static IDictionary<string, IWeldpoint> FindWeldPointsABB(List<string> modFiles, out bool isVWBool)
        {
            bool? isVW = null;
            Regex isLspBMWRegex = new Regex(@"\s+robtarget\s+lsp\d+", RegexOptions.IgnoreCase);
            Regex isLspVWRegex = new Regex(@"(?<=robtarget\s+(p|))P\d+_\w+_\d+_\d+_[a-zA-Z0-9]+", RegexOptions.IgnoreCase);
            Regex spotNrRegex = new Regex(@"(?<=\s+robtarget\s+lsp)\d+", RegexOptions.IgnoreCase);
            Regex getCoordsRegex = new Regex(@"(?<=\=.*)(-\d+\.\d+|\d+\.\d+|-\d+|\d+)", RegexOptions.IgnoreCase);
            IDictionary<string, IWeldpoint> result = new Dictionary<string,IWeldpoint>();
            string message = string.Empty;
            foreach (var file in modFiles)
            {
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!isVW.HasValue && (isLspBMWRegex.IsMatch(line) || isLspVWRegex.IsMatch(line)))
                    {
                        if (isLspBMWRegex.IsMatch(line))
                            isVW = false;
                        else
                            isVW = true;
                    }
                    if (isVW.HasValue && (isLspBMWRegex.IsMatch(line) || isLspVWRegex.IsMatch(line)))
                    {
                        if (!isVW.Value)
                        {
                            int number = int.Parse(spotNrRegex.Match(line).ToString());
                            var matches = getCoordsRegex.Matches(line);
                            WeldpointBMW currentPoint = new WeldpointBMW(Path.GetFileName(Path.GetDirectoryName(file)), Path.GetFileNameWithoutExtension(file), string.Empty, number, 0, double.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), 0, 0, 0, "LSP", "");
                            result.Add(number + "_0_LSP", currentPoint);
                        }
                        else
                        {
                            string pointName = isLspVWRegex.Match(line).ToString();
                            var matches = getCoordsRegex.Matches(line);
                            string dirs = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(file)));
                            string lastFolderName = Path.GetFileName(Path.GetDirectoryName(dirs));
                            WeldpointVW currentPoint = new WeldpointVW(lastFolderName, pointName, double.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), 0, 0, 0, "", "LSP", file,"");
                            if (!result.Keys.Contains(pointName))
                                result.Add(pointName, currentPoint);
                            else
                            {
                                message += pointName + ", ";
                                while (result.Keys.Contains(pointName))
                                {
                                    pointName += ";";
                                }
                                result.Add(pointName, currentPoint);
                            }
                        }
                    }
                }
                reader.Close();
            }
            if (!string.IsNullOrEmpty(message))
                CommonMethods.CreateLogFile("Multiple declaration of points: \r\n" + message, "\\logReadSpots.txt");
            isVWBool = isVW.Value;
            if (isVWBool)
            {
                string lastPath = string.Empty;
                foreach (var file in modFiles)
                {
                    Regex procRegex = new Regex(@"(?<=PROC\s+)[a-zA-Z0-9_\-]+(?=\s*\(\s*\))", RegexOptions.IgnoreCase);
                    StreamReader reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (procRegex.IsMatch(line))
                            lastPath = procRegex.Match(line).ToString();
                        if (line.ToLower().ContainsAny(new string[] {"spz_ptp","spz_lin"}) && line.ToLower().ContainsAny(result.Keys.ToArray()))
                        {
                            Regex getSpotIndexRegex = new Regex(@"(?<=SPZ_(PTP|LIN)\s+\w+\s*,\s*[a-zA-Z0-9_]+\s*,\s*)\d+", RegexOptions.IgnoreCase);
                            Regex pointNameRegex = new Regex(@"(?<=SPZ_(PTP|LIN)\s+(p|))P\d+_\w+_\d+_\d+_[a-zA-Z0-9]+", RegexOptions.IgnoreCase);
                            string pointName = pointNameRegex.Match(line).ToString();
                            (result[result.Keys.First(x => x.ToLower() == pointName.ToLower())] as WeldpointVW).SpotIndex = getSpotIndexRegex.Match(line).ToString();
                            (result[result.Keys.First(x => x.ToLower() == pointName.ToLower())] as WeldpointVW).Path = lastPath;
                        }
                    }
                    reader.Close();
                }
            }
            return result;
        }

        private static void FillAppConfig(MPLListParamsVM vm)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            config.AppSettings.Settings.Remove("MPLSpotNumberColumn");
            config.AppSettings.Settings.Add("MPLSpotNumberColumn", vm.SpotNumberColumnInMPL);
            config.AppSettings.Settings.Remove("MPLXCoord");
            config.AppSettings.Settings.Add("MPLXCoord", vm.XColumnInMPL);
            config.AppSettings.Settings.Remove("MPLYCoord");
            config.AppSettings.Settings.Add("MPLYCoord", vm.YColumnInMPL);
            config.AppSettings.Settings.Remove("MPLZCoord");
            config.AppSettings.Settings.Add("MPLZCoord", vm.ZColumnInMPL);
            config.AppSettings.Settings.Remove("MPLFirstPointRow");
            config.AppSettings.Settings.Add("MPLFirstPointRow", vm.FirstSpotRow);
            config.AppSettings.Settings.Remove("MPLSheetName");
            config.AppSettings.Settings.Add("MPLSheetName", vm.SheetName);
            config.AppSettings.Settings.Remove("MPLPuntkType");
            config.AppSettings.Settings.Add("MPLPuntkType", vm.PunktType);
            config.AppSettings.Settings.Remove("MPLSpotIndex");
            config.AppSettings.Settings.Add("MPLSpotIndex", vm.SpotIndexVW);
            config.Save(ConfigurationSaveMode.Minimal);

            excelParams = new DatasInExcel(vm.SpotNumberColumnInMPL, vm.XColumnInMPL, vm.YColumnInMPL, vm.ZColumnInMPL, vm.FirstSpotRow, vm.SheetName, vm.PunktType, vm.SpotIndexVW);
        }

        public static int GetColumnNumber(string inputString)
        {
            int result = 0;
            if (int.TryParse(inputString, out result))
                return result;
            inputString = inputString.ToUpper();
            string currentString = CommonMethods.ToNumericCoordinates(inputString);
            currentString = currentString.Replace(",", "");
            if (int.TryParse(currentString, out result))
                return result;
            else
            {
                MessageBox.Show("Wrong parameter!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public static IDictionary<int, WeldpointBMW> GetPoitsFromBosch()
        {
            string doublePoints = "";
            Regex getTypId = new Regex(@"\d+(?=_)", RegexOptions.IgnoreCase);
            Regex getPointNum = new Regex(@"(?<=(\d+|[a-zA-Z]+)_)\d+", RegexOptions.IgnoreCase);
            IDictionary<int, WeldpointBMW> result = new Dictionary<int, WeldpointBMW>();
            string selectedfile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "Text File .txt", ".txt");
            if (!string.IsNullOrEmpty(selectedfile))
            {
                StreamReader reader = new StreamReader(selectedfile);
                while (!reader.EndOfStream)
                {
                    int number = 0;
                    string line = reader.ReadLine();
                    string firstChar = line.Substring(0, 1);
                    if (int.TryParse(firstChar, out number))
                    {
                        if (number != 0)
                        {
                            int typId = int.Parse(getTypId.Match(line).ToString());
                            int pointNum = int.Parse(getPointNum.Match(line).ToString());
                            WeldpointBMW point = new WeldpointBMW("", "", "", pointNum, 0, 0, 0, 0, 0, 0, 0, "", "", typIDsInBosch: new List<int>());
                            if (!result.Keys.Contains(pointNum))
                            {
                                result.Add(pointNum, point);
                            }
                            else
                                doublePoints += "Double declaration of point " + pointNum + " in Bosch\r\n";
                            result[pointNum].TypIDsInBosch.Add(typId);
                        }
                    }
                }
                reader.Close();
                if (!string.IsNullOrEmpty(doublePoints))
                    MessageBox.Show(doublePoints, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                return null;
            return result;
        }

        public static IDictionary<string, WeldpointBMW> CompareGlobalAndSrc(dynamic weldPoints, IDictionary<string, WeldpointBMW> usedSpotsDict)
        {
            IDictionary<string, WeldpointBMW> copyOfusedSpots = new Dictionary<string, WeldpointBMW>();
            usedSpotsDict.ToList().ForEach(x => copyOfusedSpots.Add(x));
            IDictionary<string, WeldpointBMW> result = new Dictionary<string, WeldpointBMW>();
            try
            {
                foreach (var item in weldPoints)
                {
                    int currentPointnum = 0;
                    var type = item.Key.GetType();
                    if (type.Name == "String")
                        currentPointnum = item.Value.Number;
                    else
                        currentPointnum = item.Key;
                    string pointName = currentPointnum.ToString() + "_" + item.Value.TypId + "_" + item.Value.ProcessType;
                    if (item.Value is WeldpointBMW && ((item.Value as WeldpointBMW).ProcessType == "GL" || (item.Value as WeldpointBMW).ProcessType == "ARC"))
                    {
                        pointName = currentPointnum.ToString() + "_" + (item.Value as WeldpointBMW).PointNum + "_" + item.Value.TypId + "_" + item.Value.ProcessType;
                    }
                    if (usedSpotsDict.Keys.ToList().ContainsAny(new string[] { pointName, pointName.Replace("SWx","SWH"), pointName.Replace("SWx", "SWR"), pointName.Replace("SWx", "SWN") }))
                    {
                        if (pointName.Substring(pointName.Length-3,3).ToLower() == "swx")
                        {
                            if (usedSpotsDict.Keys.Any(x => x.ToLower() == pointName.ToLower().Substring(0, pointName.Length - 1) + "n"))
                                pointName = pointName.Substring(0, pointName.Length - 1) + "N";
                            else if (usedSpotsDict.Keys.Any(x => x.ToLower() == pointName.ToLower().Substring(0, pointName.Length - 1) + "r"))
                                pointName = pointName.Substring(0, pointName.Length - 1) + "R";
                            else if (usedSpotsDict.Keys.Any(x => x.ToLower() == pointName.ToLower().Substring(0, pointName.Length - 1) + "h"))
                                pointName = pointName.Substring(0, pointName.Length - 1) + "H";
                        }
                        copyOfusedSpots.Remove(copyOfusedSpots.First(x => x.Key == pointName));
                        result.Add(pointName, new WeldpointBMW(usedSpotsDict[pointName].Robot, usedSpotsDict[pointName].Path, usedSpotsDict[pointName].PLC, currentPointnum, item.Value.TypId, item.Value.XPos, item.Value.YPos, item.Value.ZPos, item.Value.A, item.Value.B, item.Value.C, item.Value.ProcessType, usedSpotsDict[pointName].PathToBackup, status: item.Value.Status, turn: item.Value.Turn) { PointNum = item.Value.ProcessType == "GL" ? item.Value.PointNum : 0 , PointFullName = usedSpotsDict[pointName].PointFullName });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.Value.Path))
                            result.Add(pointName, item.Value);
                    }
                }
                foreach (var point in usedSpotsDict.Where(x => !result.Keys.Contains(x.Key)))
                    result.Add(point.Key + "_LOCAL", point.Value);
            return result;
        }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                // MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static IDictionary<string, WeldpointBMW> CreateDictWithUsedPoints(IDictionary<string, List<SpotsInFile>> usedSpots)
        {
            IDictionary<string, WeldpointBMW> result = new Dictionary<string, WeldpointBMW>();
            foreach (var backup in usedSpots)
            {
                string backupName = Path.GetFileNameWithoutExtension(backup.Key);
                foreach (var point in backup.Value)
                {
                    string keyToAdd = string.Empty;
                    if (point.ProcessType != "GL" && point.ProcessType != "ARC")
                        keyToAdd = point.SpotNr.ToString() + "_" + point.TypID + "_" + point.ProcessType;
                    else
                        keyToAdd = point.SpotNr.ToString() + "_" + point.PointNum + "_" + point.TypID + "_" + point.ProcessType;

                    if (!result.Keys.Contains(keyToAdd))
                        result.Add(keyToAdd, new WeldpointBMW(GetRobot(backupName), point.PathName, GetPLC(backupName).ToUpper(), point.SpotNr, point.TypID, 0, 0, 0,0,0,0, point.ProcessType, "") { PointNum = point.PointNum , PointFullName = point.PointFullName, PathToBackup = backup.Key });
                    else
                    {
                        if (!result[keyToAdd].Path.Contains(point.PathName))
                            result[keyToAdd].Path += ", " + point.PathName;
                    }                   
                }
            }

            return result;
        }

        public static IDictionary<string, List<SpotsInFile>> FindUsedPointsInSrcFiles(List<string> foundBackups)
        {
            Regex isOtherProcess = new Regex(@"^(Swh_Exe|Swr_Exe|SNH_EXE)\s*\(\s*[a-zA-Z]+\d+_\d+", RegexOptions.IgnoreCase);
            Regex isFls = new Regex(@"^\s*(Fls_Exe|Rvt_Exe|Brt_Exe|Swp_Exe)\s*\(\d*\d+\s*,\s*\d+", RegexOptions.IgnoreCase);
            Regex isCont = new Regex(@"(?<=(Gl_Move|GL_On|Gl_Off|Arc_On|ATB_Definition|Arc_Off)\s*\(\s*)[\d\s,\w]+", RegexOptions.IgnoreCase);
            Regex arcPointRegex = new Regex(@"(?<=LIN\s+)Xarc\d+P\d+_\d+", RegexOptions.IgnoreCase);
            IDictionary<string, List<SpotsInFile>> result = new Dictionary<string, List<SpotsInFile>>();
            foreach (string backup in foundBackups)
            {
                List<SpotsInFile> spotsInCurrentFile = new List<SpotsInFile>();
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    List<ZipArchiveEntry> entries = GetSrcFilesFromBackup(archive.Entries);
                    if (entries.Count > 0)
                    {
                        foreach (var entry in entries)
                        {
                            string lastArcPoint = string.Empty;
                            var srcFile = archive.GetEntry(CommonLibrary.CommonMethods.GetEntryName(archive.Entries, entry.ToString()));
                            StreamReader reader = new StreamReader(srcFile.Open());
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (arcPointRegex.IsMatch(line))
                                    lastArcPoint = arcPointRegex.Match(line).ToString();
                                if (line.ToLower().Contains("advspot") || line.ToLower().Contains("pickexe") || isOtherProcess.IsMatch(line) || isFls.IsMatch(line) || isCont.IsMatch(line))
                                {
                                    SpotsInFile currentPoint = GetPointFropSrc(line, Path.GetFileNameWithoutExtension(entry.ToString()),lastArcPoint);
                                    if (currentPoint!=null && !(currentPoint.ProcessType == "SWP" && spotsInCurrentFile.Where(x=>x.SpotNr == currentPoint.SpotNr).Where(y => y.TypID == currentPoint.TypID).Any(z=>z.ProcessType == "LSP")))
                                        spotsInCurrentFile.Add(currentPoint);
                                }

                            }
                            reader.Close();
                        }
                    }

                    CheckLocalSpots(backup, entries, spotsInCurrentFile);
                }
                result.Add(backup, spotsInCurrentFile);
            }
            return result;
        }

        public static void CheckLocalSpots(string backup, List<ZipArchiveEntry> entries, List<SpotsInFile> spotsInCurrentFile)
        {
            foreach (var entry in entries)
            {
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    //var datFile = archive.GetEntry(entry.ToString().Replace(".src", ".dat"));
                    var datFile = archive.GetEntry(CommonMethods.GetEntryName(archive.Entries, entry.ToString().Replace(".src", ".dat")));
                    foreach (SpotsInFile spot in spotsInCurrentFile)
                    {
                        if (datFile != null)
                        {
                            StreamReader reader = new StreamReader(datFile.Open());
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (line.ToLower().Contains("e6pos") && line.Contains(spot.SpotNr + "_" + spot.TypID) && !ContainOmitedString(line))
                                    errorMessage += "Local definition of point " + spot.SpotNr + "_" + spot.TypID + " in robot " + Path.GetFileNameWithoutExtension(backup) + " path " + spot.PathName + "\r\n";
                            }
                            reader.Close();
                        }
                    }
                }
            }

        }

        private static bool ContainOmitedString(string line)
        {
            foreach (string omitedString in omitedLocalPoints)
            {
                if (line.ToLower().Contains(omitedString))
                    return true;
            }
            return false;
        }

        public static SpotsInFile GetPointFropSrc(string line, string file, string lastArcPoint)
        {
            Regex getType = new Regex(@"^.{3}", RegexOptions.IgnoreCase);
            Regex getNumbers = new Regex(@"\d+", RegexOptions.IgnoreCase);
            string pointFullName = "";
            MatchCollection foundValues = getNumbers.Matches(line);
            Regex isCont = new Regex(@"(?<=(Gl_Move|GL_On|Gl_Off|Arc_on|ATB_Definition|Arc_Off)\s*\(\s*)[\d\s,\w]+", RegexOptions.IgnoreCase);
            if (line.ToLower().Contains("fls_exe"))
                return new SpotsInFile(file, int.Parse(foundValues[1].ToString()), int.Parse(foundValues[0].ToString()), "FLS", "xfls" + foundValues[1].ToString()+ "_" + foundValues[0].ToString());
            else if (line.ToLower().Contains("advspot"))
                return new SpotsInFile(file, int.Parse(foundValues[2].ToString()), int.Parse(foundValues[1].ToString()), "LSP", "xswp" + foundValues[2].ToString() + "_" + foundValues[1].ToString());
            else if (line.ToLower().Contains("pickexe"))
                return new SpotsInFile(file, int.Parse(foundValues[2].ToString()), int.Parse(foundValues[1].ToString()), "PICK", pointFullName);
            else if (line.ToLower().Contains("swh_exe") || line.ToLower().Contains("swr_exe"))
                return new SpotsInFile(file, int.Parse(foundValues[2].ToString()), int.Parse(foundValues[1].ToString()), line.Trim().Substring(0,3).ToUpper(), "x" + getType.Match(line.Trim().Replace(" ", "")).ToString().ToLower() + foundValues[2].ToString() + "_" + foundValues[1].ToString());
            else if (isCont.IsMatch(line))
            {
                if (line.ToLower().Contains("gl_on") || line.ToLower().Contains("gl_off"))
                    return new SpotsInFile(file, int.Parse(foundValues[2].ToString()), int.Parse(foundValues[1].ToString()), "GL", "xgl"+ foundValues[2].ToString() + "_" + foundValues[1].ToString()) { PointNum = int.Parse(foundValues[8].ToString()) };
                else if (line.ToLower().Contains("gl_move"))
                    return new SpotsInFile(file, int.Parse(foundValues[2].ToString()), int.Parse(foundValues[1].ToString()), "GL", "xgl" + foundValues[2].ToString() + "_" + foundValues[1].ToString()) { PointNum = int.Parse(foundValues[5].ToString()) };
                else if (line.ToLower().Contains("arc_on") || line.ToLower().Contains("arc_off") || line.ToLower().Contains("atb_definition"))
                {
                    if (!string.IsNullOrEmpty(lastArcPoint))
                    {
                        MatchCollection arcMatches = getNumbers.Matches(lastArcPoint);
                        return new SpotsInFile(file, int.Parse(arcMatches[0].ToString()), int.Parse(arcMatches[2].ToString()), "ARC", pointFullName) { PointNum = int.Parse(arcMatches[1].ToString()) };
                    }
                    else
                        return null;
                }
            }
            else
                return new SpotsInFile(file, int.Parse(foundValues[2].ToString()), int.Parse(foundValues[1].ToString()), getType.Match(line.Trim().Replace(" ", "")).ToString().ToUpper(), "x" + getType.Match(line.Trim().Replace(" ", "")).ToString().ToLower() + foundValues[2].ToString() + "_" + foundValues[1].ToString());
            return null;
        }

        private static List<ZipArchiveEntry> GetSrcFilesFromBackup(ReadOnlyCollection<ZipArchiveEntry> entries)
        {
            List<ZipArchiveEntry> result = new List<ZipArchiveEntry>();
            foreach (var entry in entries.Where(x => ((x.ToString().ToLower().Contains(@"krc/r1/program") || x.ToString().ToLower().Contains(@"krc/r1/bmw_program")) && x.ToString().ToLower().Contains(@".src") && !x.ToString().ToLower().Contains(@"tm_useraction"))))
            {
                result.Add(entry);
            }
            return result;
        }

        private static void CreateExcel(IDictionary<string, IWeldpoint> weldPointsDict, string savePath, IDictionary<int, WeldpointBMW> pointsInBosch, bool compareWithMPL, string procType, bool isVW, bool addStatusAndTurn = false)
        {
            try
            {
                string mplPath = "";
                int columncounter = 0;
                waitdialog = new PleaseWaitDialog();
                IDictionary<string, IWeldpoint> spotsFromMPL = new Dictionary<string, IWeldpoint>();
                bool isPointInBoschPresent = false;
                if (pointsInBosch.Count > 0)
                    isPointInBoschPresent = true;
                List<IWeldpoint> weldPoints = new List<IWeldpoint>();
                foreach (var point in weldPointsDict.Values)
                    weldPoints.Add(point);

                if (compareWithMPL)
                {
                    MessageBox.Show("Select MPL list file", "Select file", MessageBoxButton.OK, MessageBoxImage.Information);
                    mplPath = CommonLibrary.CommonMethods.SelectDirOrFile(false, "Excel file (*.xlsm) (*.xlsx)", "*.xlsm; *.xlsx");
                    if (!string.IsNullOrEmpty(mplPath))
                    {
                        waitdialog = new PleaseWaitDialog();
                        waitdialog.Show();
                        spotsFromMPL = FindWeldPointsInMPL(mplPath, procType, isVW);
                        waitdialog.Close();
                    }
                }
                waitdialog = new PleaseWaitDialog();
                waitdialog.Show();
                destinationXlApp = new Excel.Application();
                object misValue = System.Reflection.Missing.Value;

                destinationXlWorkbook = destinationXlApp.Workbooks.Add(misValue);
                destinationXlWorksheet = (Excel.Worksheet)destinationXlWorkbook.Worksheets.get_Item(1);

                destinationXlWorksheet.Cells[1, 1] = "PLC";
                destinationXlWorksheet.Cells[1, 1].Font.Bold = true;
                destinationXlWorksheet.Cells[1, 2] = "Robot";
                destinationXlWorksheet.Cells[1, 2].Font.Bold = true;
                destinationXlWorksheet.Cells[1, 3] = "Point Nr";
                destinationXlWorksheet.Cells[1, 3].Font.Bold = true;
                destinationXlWorksheet.Cells[1, 4] = "Point TypID";
                destinationXlWorksheet.Cells[1, 4].Font.Bold = true;
                destinationXlWorksheet.Cells[1, 5] = "Point Type";
                destinationXlWorksheet.Cells[1, 5].Font.Bold = true;
                if (isPointInBoschPresent)
                {
                    destinationXlWorksheet.Cells[1, 6] = "TypID in Bosch";
                    destinationXlWorksheet.Cells[1, 6].Font.Bold = true;
                    columncounter = 1;
                }
                destinationXlWorksheet.Cells[1, columncounter + 6] = "Path";
                destinationXlWorksheet.Cells[1, columncounter + 6].Font.Bold = true;
                destinationXlWorksheet.Cells[1, columncounter + 7] = "X";
                destinationXlWorksheet.Cells[1, columncounter + 7].Font.Bold = true;
                destinationXlWorksheet.Cells[1, columncounter + 8] = "Y";
                destinationXlWorksheet.Cells[1, columncounter + 8].Font.Bold = true;
                destinationXlWorksheet.Cells[1, columncounter + 9] = "Z";
                destinationXlWorksheet.Cells[1, columncounter + 9].Font.Bold = true;

                if (addStatusAndTurn)
                {
                    destinationXlWorksheet.Cells[1, columncounter + 10] = "A";
                    destinationXlWorksheet.Cells[1, columncounter + 10].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 11] = "B";
                    destinationXlWorksheet.Cells[1, columncounter + 11].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 12] = "C";
                    destinationXlWorksheet.Cells[1, columncounter + 12].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 13] = "Status";
                    destinationXlWorksheet.Cells[1, columncounter + 13].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 14] = "Turn";
                    destinationXlWorksheet.Cells[1, columncounter + 14].Font.Bold = true;
                }
                if (compareWithMPL && !string.IsNullOrEmpty(mplPath))
                {
                    destinationXlWorksheet.Cells[1, columncounter + 10] = "X MPL";
                    destinationXlWorksheet.Cells[1, columncounter + 10].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 11] = "Y MPL";
                    destinationXlWorksheet.Cells[1, columncounter + 11].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 12] = "Z MPL";
                    destinationXlWorksheet.Cells[1, columncounter + 12].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 13] = "Diff X";
                    destinationXlWorksheet.Cells[1, columncounter + 13].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 14] = "Diff Y";
                    destinationXlWorksheet.Cells[1, columncounter + 14].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 15] = "Diff Z";
                    destinationXlWorksheet.Cells[1, columncounter + 15].Font.Bold = true;
                    destinationXlWorksheet.Cells[1, columncounter + 16] = "Diff Total";
                    destinationXlWorksheet.Cells[1, columncounter + 16].Font.Bold = true;
                    if (isVW)
                    {
                        destinationXlWorksheet.Cells[1, columncounter + 17] = "SpotIndex MPL";
                        destinationXlWorksheet.Cells[1, columncounter + 17].Font.Bold = true;
                        destinationXlWorksheet.Cells[1, columncounter + 18] = "SpotIndex Backup";
                        destinationXlWorksheet.Cells[1, columncounter + 18].Font.Bold = true;
                    }
                    columncounter += 6;
                }
                if (isPointInBoschPresent)
                {
                    destinationXlWorksheet.Cells[1, columncounter + 10] = "Comment";
                    destinationXlWorksheet.Cells[1, columncounter + 10].Font.Bold = true;
                    columncounter = 1;
                }
                else
                    columncounter = 0;

                for (int i = 2; i < weldPoints.Count + 2; i++)
                {
                    int j = i - 2;
                    if (weldPoints[j] is WeldpointBMW)
                    {
                        destinationXlWorksheet.Cells[i, 1] = (weldPoints[j] as WeldpointBMW).PLC;
                        destinationXlWorksheet.Cells[i, 3] = (weldPoints[j] as WeldpointBMW).Number;
                        destinationXlWorksheet.Cells[i, 4] = (weldPoints[j] as WeldpointBMW).TypId;
                    }
                    else
                    {
                        destinationXlWorksheet.Cells[i, 1] = "";
                        destinationXlWorksheet.Cells[i, 3] = (weldPoints[j] as WeldpointVW).Name;
                        destinationXlWorksheet.Cells[i, 4] = "";
                    }
                    destinationXlWorksheet.Cells[i, 2] = weldPoints[j].Robot;
                    destinationXlWorksheet.Cells[i, 5] = weldPoints[j].ProcessType;
                    if (isPointInBoschPresent)
                    {
                        if (pointsInBosch.Keys.Contains((weldPoints[j] as WeldpointBMW).Number))
                        {
                            int temp = 0;
                            string currentCell = GetTypIDsFromBosch(pointsInBosch[(weldPoints[j] as WeldpointBMW).Number].TypIDsInBosch);
                            destinationXlWorksheet.Cells[i, 6] = currentCell;
                            if (!int.TryParse(currentCell, out temp))
                            {
                                Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates("6," + i)];
                                FormatRangeRed(range, FormatType.GreaterThan5);
                            }
                        }
                    }
                    destinationXlWorksheet.Cells[i, columncounter + 6] = weldPoints[j].Path;
                    destinationXlWorksheet.Cells[i, columncounter + 7] = weldPoints[j].XPos;
                    destinationXlWorksheet.Cells[i, columncounter + 8] = weldPoints[j].YPos;
                    destinationXlWorksheet.Cells[i, columncounter + 9] = weldPoints[j].ZPos;
                    if (addStatusAndTurn && !isVW)
                    {
                        destinationXlWorksheet.Cells[i, columncounter + 10] = weldPoints[j].A;
                        destinationXlWorksheet.Cells[i, columncounter + 11] = weldPoints[j].B;
                        destinationXlWorksheet.Cells[i, columncounter + 12] = weldPoints[j].C;
                        destinationXlWorksheet.Cells[i, columncounter + 13] = weldPoints[j].Status;
                        destinationXlWorksheet.Cells[i, columncounter + 14] = weldPoints[j].Turn;
                    }
                    string indexator = string.Empty;
                    if (compareWithMPL && !string.IsNullOrEmpty(mplPath))
                    {
                        if (isVW)
                        {
                            if (spotsFromMPL.Keys.Contains((weldPoints[j] as WeldpointVW).Name))
                            {
                                destinationXlWorksheet.Cells[i, columncounter + 10] = spotsFromMPL[(weldPoints[j] as WeldpointVW).Name].XPos;
                                destinationXlWorksheet.Cells[i, columncounter + 11] = spotsFromMPL[(weldPoints[j] as WeldpointVW).Name].YPos;
                                destinationXlWorksheet.Cells[i, columncounter + 12] = spotsFromMPL[(weldPoints[j] as WeldpointVW).Name].ZPos;
                                destinationXlWorksheet.Cells[i, columncounter + 13] = Math.Abs(weldPoints[j].XPos - spotsFromMPL[(weldPoints[j] as WeldpointVW).Name].XPos);
                                destinationXlWorksheet.Cells[i, columncounter + 14] = Math.Abs(weldPoints[j].YPos - spotsFromMPL[(weldPoints[j] as WeldpointVW).Name].YPos);
                                destinationXlWorksheet.Cells[i, columncounter + 15] = Math.Abs(weldPoints[j].ZPos - spotsFromMPL[(weldPoints[j] as WeldpointVW).Name].ZPos);
                                indexator = (weldPoints[j] as WeldpointVW).Name;
                                destinationXlWorksheet.Cells[i, columncounter + 16] = Math.Sqrt(Math.Pow(weldPoints[j].XPos - spotsFromMPL[indexator].XPos, 2) + Math.Pow(weldPoints[j].YPos - spotsFromMPL[indexator].YPos, 2) + Math.Pow(weldPoints[j].ZPos - spotsFromMPL[indexator].ZPos, 2));
                                destinationXlWorksheet.Cells[i, columncounter + 17] = (spotsFromMPL[(weldPoints[j] as WeldpointVW).Name] as WeldpointVW).SpotIndex;
                                destinationXlWorksheet.Cells[i, columncounter + 18] = (weldPoints[j] as WeldpointVW).SpotIndex;
                                {
                                    Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(columncounter + 17 +","+i), CommonMethods.ToExcelCoordinates(columncounter + 18 + "," + i)];
                                    FormatRangeRed(range, FormatType.Compare2Values, destinationXlWorksheet.Cells[i, columncounter + 17].FormulaLocal, destinationXlWorksheet.Cells[i, columncounter + 18].FormulaLocal);
                                }
                            }
                            else
                            {
                                errorMessage += "Point " + (weldPoints[j] as WeldpointVW).Name + " not found in MPL list\r\n";
                            }
                        }
                        else
                        {
                            if (spotsFromMPL.Keys.Contains((weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType))
                            {
                                destinationXlWorksheet.Cells[i, columncounter + 10] = spotsFromMPL[(weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType].XPos;
                                destinationXlWorksheet.Cells[i, columncounter + 11] = spotsFromMPL[(weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType].YPos;
                                destinationXlWorksheet.Cells[i, columncounter + 12] = spotsFromMPL[(weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType].ZPos;
                                destinationXlWorksheet.Cells[i, columncounter + 13] = Math.Abs(weldPoints[j].XPos - spotsFromMPL[(weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType].XPos);
                                destinationXlWorksheet.Cells[i, columncounter + 14] = Math.Abs(weldPoints[j].YPos - spotsFromMPL[(weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType].YPos);
                                destinationXlWorksheet.Cells[i, columncounter + 15] = Math.Abs(weldPoints[j].ZPos - spotsFromMPL[(weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType].ZPos);
                                indexator = (weldPoints[j] as WeldpointBMW).Number + "_" + weldPoints[j].ProcessType;
                                destinationXlWorksheet.Cells[i, columncounter + 16] = Math.Sqrt(Math.Pow(weldPoints[j].XPos - spotsFromMPL[indexator].XPos, 2) + Math.Pow(weldPoints[j].YPos - spotsFromMPL[indexator].YPos, 2) + Math.Pow(weldPoints[j].ZPos - spotsFromMPL[indexator].ZPos, 2));
                            }
                            else
                            {
                                errorMessage += "Point " + (weldPoints[j] as WeldpointBMW).Number + " not found in MPL list\r\n";
                            }
                        }
                        columncounter += 6;
                        {
                            Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(columncounter - 6 + 13 + "," + i)];
                            FormatRangeRed(range, FormatType.GreaterThan5);
                        }
                        {
                            Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(columncounter - 6 + 14 + "," + i)];
                            FormatRangeRed(range, FormatType.GreaterThan5);
                        }
                        {
                            Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(columncounter - 6 + 15 + "," + i)];
                            FormatRangeRed(range, FormatType.GreaterThan5);
                        }
                        {
                            Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(columncounter - 6 + 16 + "," + i)];
                            FormatRangeRed(range, FormatType.GreaterThan5);
                        }

                        if (isPointInBoschPresent)
                        {
                            if (pointsInBosch.Keys.Contains((weldPoints[j] as WeldpointBMW).Number))
                            {
                                if ((weldPoints[j] as WeldpointBMW).TypId.ToString() != GetTypIDsFromBosch(pointsInBosch[(weldPoints[j] as WeldpointBMW).Number].TypIDsInBosch))
                                {
                                    destinationXlWorksheet.Cells[i, columncounter + 10] = "TypID in Bosch and in robot are different!";
                                    Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(columncounter + 10 + "," + i)];
                                    range.Interior.ColorIndex = 3;
                                }
                            }
                            else
                            {
                                destinationXlWorksheet.Cells[i, columncounter + 10] = "Point missing in Bosch!";
                                Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(columncounter + 10 + "," + i)];
                                range.Interior.ColorIndex = 3;
                            }
                        }
                        if (isPointInBoschPresent)
                            columncounter = 1;
                        else
                            columncounter = 0;
                    }
                }
                Excel.Range usedRange = destinationXlWorksheet.UsedRange;
                usedRange.Columns.AutoFit();
                FormatAsTable(usedRange, "Table1", "TableStyleMedium15");
                try
                {
                    //destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    if (!File.Exists(savePath) || !CommonLibrary.CommonMethods.IsFileLocked(savePath))
                    {
                        destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        if (errorMessage != "")
                            MessageBox.Show(errorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        MessageBox.Show("Succesfully saved at " + savePath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        destinationXlWorkbook.Close(true, misValue, misValue);
                    }
                    else
                        MessageBox.Show("File " + savePath + " is used by another process. Close the file and try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CleanUpExcel("destination");
                    waitdialog.Close();

                }
                catch
                {
                    CleanUpExcel("destination");
                    waitdialog.Close();
                    MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
            }
        }

        private static void FormatRangeRed(Range range, FormatType type, string val1 = "", string val2 = "")
        {
            Excel.FormatConditions fcs = range.FormatConditions;
            Excel.FormatCondition condition = null;
            if (type == FormatType.GreaterThan5)
            {
                condition = (Excel.FormatCondition)fcs.Add(
                    Type: Excel.XlFormatConditionType.xlCellValue,
                    Operator: Excel.XlFormatConditionOperator.xlGreater,
                    Formula1: 5);
            }
            else if (type == FormatType.Compare2Values)
            {
                    condition = (Excel.FormatCondition)fcs.Add(
                    Type: Excel.XlFormatConditionType.xlCellValue,
                    Operator: Excel.XlFormatConditionOperator.xlNotEqual,
                    Formula1: val1,
                    Formula2: val2);
            }
            condition.Interior.ColorIndex = 3;
        }

        public static string GetTypIDsFromBosch(List<int> typIDsInBosch)
        {
            if (typIDsInBosch == null)
                return "No TypID found!";
            else if (typIDsInBosch.Count == 0)
                return "No TypID found!";
            else if (typIDsInBosch.Count == 1)
                return typIDsInBosch[0].ToString();
            else
            {
                string result = typIDsInBosch[0].ToString();
                for (int i = 1; i < typIDsInBosch.Count; i++)
                    result += ", " + typIDsInBosch[i].ToString();
                return result;
            }
        } 

        public static async Task<IDictionary<string, WeldpointBMW>> FindWeldPoints(List<string> foundBackups)
        {
            IDictionary<string, WeldpointBMW> result = new Dictionary<string, WeldpointBMW>();
            IDictionary<string, List<WeldpointBMW>> dobuleSpots = new Dictionary<string, List<WeldpointBMW>>();
            List<Task<IDictionary<string, WeldpointBMW>>> tasks = new List<Task<IDictionary<string, WeldpointBMW>>>();
            foreach (string backup in foundBackups)
            {
                tasks.Add(Task.Run(() => ScanBackup(backup)));                
            }
            var results = await Task.WhenAll(tasks);
            foreach (var item in results)
            {
                foreach (var item2 in item)
                {
                    if (!result.Keys.Contains(item2.Key))
                        result.Add(item2.Key, item2.Value);
                    else
                    {
                        if (!dobuleSpots.Keys.Contains(item2.Key))
                        {
                            dobuleSpots.Add(item2.Key, new List<WeldpointBMW> { result[item2.Key], item2.Value });
                        }
                        else
                            dobuleSpots[item2.Key].Add(item2.Value);
                    }

                }
            }
            if (dobuleSpots.Count > 0)
            {
                string mess = string.Empty;
                dobuleSpots.Keys.ToList().ForEach(x => mess += x + ",\r\n");
                MessageBox.Show("Double points declarations for points:\r\n" + mess, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                CommonLibrary.CommonMethods.CreateLogFile("Double points declarations for points:\r\n" + mess, "\\logMultiDeclaration.txt");

            }
            return result;
        }

        private static IDictionary<string, WeldpointBMW> ScanBackup(string backup)
        {
            Regex isStosspunkter = new Regex(@"XPick\d+_\d+", RegexOptions.IgnoreCase);
            Regex isOtherApplication = new Regex(@"(Xswh|Xswr|Xsnh|Xrvt|Xfls|Xbrt|Xswp|Xgl|Xarc)\d+_\d+", RegexOptions.IgnoreCase);
            Regex isCont = new Regex(@"X(gl|arc)\d+P\d+_\d+", RegexOptions.IgnoreCase);
            Regex pointNameRegex = new Regex(@"(?<=E6POS\s+)\w+_\d+", RegexOptions.IgnoreCase);
            IDictionary<string, WeldpointBMW> result = new Dictionary<string, WeldpointBMW>();
            using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
            {
                List<ZipArchiveEntry> entries = GetSpotGlobalEntry(archive.Entries);
                foreach(var entry in entries)
                {
                    var spotGlobalFile = archive.GetEntry(entry.ToString());
                    StreamReader reader = new StreamReader(spotGlobalFile.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.ToLower().Contains("e6pos") && (line.ToLower().Contains("xlsp") || isStosspunkter.IsMatch(line) || isOtherApplication.IsMatch(line) || isCont.IsMatch(line)) && line.ToLower().Trim().Substring(0, 1) != ";")
                        {
                            WeldpointBMW currentPoint = GetPoint(line, Path.GetFileNameWithoutExtension(backup));
                            string pointName = currentPoint.ProcessType.ContainsAny(new string[] { "GL", "ARC" } ) ? currentPoint.Number.ToString() + "_" + currentPoint.PointNum.ToString() + "_" + currentPoint.TypId + "_" + currentPoint.ProcessType : currentPoint.Number.ToString() + "_" + currentPoint.TypId.ToString() + "_" + currentPoint.ProcessType;
                            if (!result.Keys.Contains(pointName))
                                result.Add(pointName, currentPoint);
                            else
                                errorMessage += "Double declaration of point " + currentPoint.Number + ". Check robot " + currentPoint.PLC + currentPoint.Robot + "\r\n";
                        }
                    }
                    reader.Close();
                }
            }
            return result;
        }

        public static WeldpointBMW GetPoint(string line, string backup)
        {
            if (line.Contains("551407"))
            { }
            Regex isPoint = new Regex(@".{3}\d+_\d+", RegexOptions.IgnoreCase);
            if (!isPoint.IsMatch(line))
                return null;
            Regex pointNameRegex = new Regex(@"(?<=E6POS\s+)\w+_\d+", RegexOptions.IgnoreCase);
            Regex pointInContRegex = new Regex(@"(?<=X(gl|arc)\d+P)\d+", RegexOptions.IgnoreCase);
            Regex pointNumRegex = new Regex(@"(?<=LSP|swp|Pick|swh|swr|snh|rvt|fls|brt|Xgl|Xarc\s*)\d*", RegexOptions.IgnoreCase);
            int pointNumber = int.Parse(pointNumRegex.Match(line).ToString());
            Regex pointTypIDRegex = new Regex(@"(?<=_\s*)\d*", RegexOptions.IgnoreCase);
            int pointTypID = int.Parse(pointTypIDRegex.Match(line).ToString());
            Regex pointXPosE_Regex = new Regex(@"(?<=X\s+)[0-9-\.]*E-\d+", RegexOptions.IgnoreCase);
            Regex pointYPosE_Regex = new Regex(@"(?<=Y\s+)[0-9-\.]*E-\d+", RegexOptions.IgnoreCase);
            Regex pointZPosE_Regex = new Regex(@"(?<=Z\s+)[0-9-\.]*E-\d+", RegexOptions.IgnoreCase);
            Regex pointAPosE_Regex = new Regex(@"(?<=A\s+)[0-9-\.]*E-\d+", RegexOptions.IgnoreCase);
            Regex pointBPosE_Regex = new Regex(@"(?<=B\s+)[0-9-\.]*E-\d+", RegexOptions.IgnoreCase);
            Regex pointCPosE_Regex = new Regex(@"(?<=C\s+)[0-9-\.]*E-\d+", RegexOptions.IgnoreCase);

            Regex pointXposRegex = new Regex(@"(?<=X\s+)[0-9-\.]*", RegexOptions.IgnoreCase);
            double pointXpos = double.Parse(pointXposRegex.Match(line).ToString().Replace(".", ","));
            if (pointXPosE_Regex.IsMatch(line))
                pointXpos = 0;

            Regex pointYposRegex = new Regex(@"(?<=Y\s+)[0-9-\.]*", RegexOptions.IgnoreCase);
            double pointYpos = double.Parse(pointYposRegex.Match(line).ToString().Replace(".", ","));
            if (pointYPosE_Regex.IsMatch(line))
                pointYpos = 0;

            Regex pointZposRegex = new Regex(@"(?<=Z\s+)[0-9-\.]*", RegexOptions.IgnoreCase);
            double pointZpos = double.Parse(pointZposRegex.Match(line).ToString().Replace(".", ","));
            if (pointZPosE_Regex.IsMatch(line))
                pointZpos = 0;

            Regex pointARegex = new Regex(@"(?<=A\s+)[0-9-\.]*", RegexOptions.IgnoreCase);
            double pointA = double.Parse(pointARegex.Match(line).ToString().Replace(".", ","));
            if (pointAPosE_Regex.IsMatch(line))
                pointA = 0;

            Regex pointBRegex = new Regex(@"(?<=B\s+)[0-9-\.]*", RegexOptions.IgnoreCase);
            double pointB = double.Parse(pointBRegex.Match(line).ToString().Replace(".", ","));
            if (pointBPosE_Regex.IsMatch(line))
                pointB = 0;

            Regex pointCRegex = new Regex(@"(?<=C\s+)[0-9-\.]*", RegexOptions.IgnoreCase);
            double pointC = double.Parse(pointCRegex.Match(line).ToString().Replace(".", ","));
            if (pointCPosE_Regex.IsMatch(line))
                pointC = 0;

            Regex pointStatusRegex = new Regex(@"(?<=S\s+)\d+", RegexOptions.IgnoreCase);
            int pointStatus = int.Parse(pointStatusRegex.Match(line).ToString().Replace(".", ","));
            Regex pointTurnRegex = new Regex(@"(?<=T\s+)\d+", RegexOptions.IgnoreCase);
            int pointTurn = int.Parse(pointTurnRegex.Match(line).ToString().Replace(".", ","));
            Regex getType = new Regex(@"(?<=X)[a-zA-Z]+(?=\d+)", RegexOptions.IgnoreCase);
            string type = getType.Match(line).ToString().ToUpper();
            if (type == "SWR" || type == "SWH" || type == "SNH")
                type = "SWx";
            if (type == "SWP")
                type = "LSP";
            string plc = GetPLC(backup);
            string robot = GetRobot(backup);

            WeldpointBMW result;
            if (type != "GL" && type != "ARC")
               result = new WeldpointBMW(robot, "", plc, pointNumber, pointTypID, pointXpos, pointYpos, pointZpos, pointA, pointB, pointC, type, backup, status: pointStatus, turn: pointTurn) { PointFullName = pointNameRegex.Match(line).ToString()};
            else
                result = new WeldpointBMW(robot, "", plc, pointNumber, pointTypID, pointXpos, pointYpos, pointZpos, pointA, pointB, pointC, type, backup, status: pointStatus, turn: pointTurn) { PointNum = pointInContRegex.IsMatch(line) ? int.Parse(pointInContRegex.Match(line).ToString()) : 0 , PointFullName = pointNameRegex.Match(line).ToString() };
            return result;
        }

        public static string GetRobot(string backup)
        {
            Regex robotnameRegexV8 = new Regex(@"(?<=^\w+\d+st)\d+\w+\d+", RegexOptions.IgnoreCase);
            Regex robotnameRegexL6 = new Regex(@"(?<=^\w+\d+m\d\d)\d+\w+\d+", RegexOptions.IgnoreCase);
            Regex robotnameRegexKRC4 = new Regex(@"(?<=[a-zA-Z0-9]+_)ST\d+_[a-zA-Z]+\d+", RegexOptions.IgnoreCase);
            if (robotnameRegexV8.IsMatch(Path.GetFileName(backup)))
                return robotnameRegexV8.Match(Path.GetFileName(backup)).ToString();
            else if (robotnameRegexL6.IsMatch(backup))
                return robotnameRegexL6.Match(backup).ToString();
            else if (robotnameRegexKRC4.IsMatch(backup))
                return (new Regex(@"(?<=ST)\d+",RegexOptions.IgnoreCase).Match(backup).ToString() + "ir" + new Regex(@"(?<=IR)\d+", RegexOptions.IgnoreCase).Match(backup).ToString());
            return "Robot not found";
        }

        private static string GetPLC(string backup)
        {
            Regex plcKrc4 = new Regex(@"(?<=^\w+\d+)\w+\d+(?=_ST)", RegexOptions.IgnoreCase);
            Regex plcV8Regex = new Regex(@"^\w+\d+(?=st)", RegexOptions.IgnoreCase);
            Regex plcL6Regex = new Regex(@"^[a-zA-Z]+\d+[a-zA-Z]+\d\d", RegexOptions.IgnoreCase);
            if (plcKrc4.IsMatch(backup))
                return plcKrc4.Match(backup).ToString();
            if (plcV8Regex.IsMatch(backup))
                return plcV8Regex.Match(backup).ToString();
            return plcL6Regex.Match(backup).ToString();
        }

        private static List<ZipArchiveEntry> GetSpotGlobalEntry(ReadOnlyCollection<ZipArchiveEntry> entries)
        {
            List<ZipArchiveEntry> filesToReturn = new List<ZipArchiveEntry>();
            foreach (var file in entries)
            {
                if (file.ToString().ToLower().Contains("spot_global"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("pick_global"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a20_swh_global"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a21_swr_global"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("b20_snh_global"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a13_rvt_global.dat"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a22_fls_global.dat"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a27_brt_global.dat"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a04_swp_global.dat"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a08_gl_global.dat"))
                    filesToReturn.Add(file);
                if (file.ToString().ToLower().Contains("a14_arc_global.dat"))
                    filesToReturn.Add(file);
                string filename = Path.GetFileName(file.ToString());
                if ((filename.Length > 5 && filename.Substring(0,5).ToLower()=="glue_" || (filename.Length > 7 && filename.Substring(0,7).ToLower() == "kleben_")) && Path.GetExtension(file.FullName.ToLower()) == ".dat")
                    filesToReturn.Add(file);
                if ((filename.Length > 8 && filename.Substring(0, 8).ToLower() == "arcweld_" || (filename.Length > 4 && filename.Substring(0, 4).ToLower() == "arc_")) && Path.GetExtension(file.FullName.ToLower()) == ".dat")
                    filesToReturn.Add(file);
            }
            return filesToReturn;
        }

        private static void CleanUpExcel(string type)
        {
            if (type == "destination")
            {
                if (destinationXlApp != null)
                {
                    int hWndDest = destinationXlApp.Application.Hwnd;

                    uint processID;

                    GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                    Process.GetProcessById((int)processID).Kill();
                }
                destinationXlWorksheet = null;
                destinationXlWorkbook = null;
                destinationXlApp = null;
            }
            if (type == "mpl")
            {
                if (mplXlApp != null)
                {
                    int hWndDest = mplXlApp.Application.Hwnd;

                    uint processID;

                    GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                    Process.GetProcessById((int)processID).Kill();
                }
                mplXlWorkbook = null;
                mplXlWorkbooks = null;
                mplXlApp = null;
            }
        }

        private static IDictionary<string, IWeldpoint> FindWeldPointsInMPL(string mplPath, string procType, bool isVW)
        {
            IDictionary<string, IWeldpoint> result = new Dictionary<string, IWeldpoint>();
            List<string> doubleDeclerations = new List<string>();
            mplXlApp = new Excel.Application();
            mplXlApp.EnableEvents = false;
            mplXlApp.DisplayAlerts = false;
            mplXlWorkbooks = mplXlApp.Workbooks;
            //mplXlApp.Visible = true;
            Thread.Sleep(500);
            mplXlWorkbook = mplXlWorkbooks.Open(mplPath);
            Thread.Sleep(500);
            //mplXlApp.Visible = false;
            Excel._Worksheet mplxlWorksheet = mplXlWorkbook.Sheets[excelParams.SheetName];
            Excel.Range mplxlRange = mplxlWorksheet.UsedRange;

            for (int i = int.Parse(excelParams.FirstSpotRow) - 1; i <= mplxlRange.Rows.Count; i++)
            {
                if (mplxlRange.Cells[i, excelParams.SpotNumberColumnInMPL].FormulaLocal != "")
                {
                    int spotNum = 0;
                    string spotName = string.Empty;
                    if (isVW)
                    {
                        if (!string.IsNullOrEmpty(mplxlRange.Cells[i, excelParams.SpotIndex].FormulaLocal))
                        {
                            Regex getNameRegex = new Regex(@"P\d+\w+", RegexOptions.IgnoreCase);
                            spotName = mplxlRange.Cells[i, excelParams.SpotNumberColumnInMPL].FormulaLocal;
                            spotName = getNameRegex.Match(spotName).ToString();
                            string spotIndex = string.Empty;
                            WeldpointVW weldpoint = new WeldpointVW("", spotName, double.Parse(mplxlRange.Cells[i, excelParams.XColumnInMPL].FormulaLocal), double.Parse(mplxlRange.Cells[i, excelParams.YColumnInMPL].FormulaLocal), double.Parse(mplxlRange.Cells[i, excelParams.ZColumnInMPL].FormulaLocal), 0, 0, 0, "", "SWP", "", GetSpotIndex(mplxlRange.Cells[i, excelParams.SpotIndex].FormulaLocal));
                            if (!result.Keys.Contains(spotName))
                                result.Add(spotName, weldpoint);
                            else
                                doubleDeclerations.Add(spotName);
                        }
                    }
                    else
                    {
                        bool isSpot = int.TryParse(mplxlRange.Cells[i, excelParams.SpotNumberColumnInMPL].FormulaLocal, out spotNum);
                        string type = string.Empty;
                        if (!string.IsNullOrEmpty(excelParams.Punkttype))
                            type = GetProcessType(mplxlRange.Cells[i, excelParams.Punkttype].FormulaLocal, procType);
                        else
                            type = GetProcessType("", procType);

                        if (isSpot && !result.Keys.Contains(spotNum.ToString() + type))
                        {
                            string currentKey = spotNum.ToString() + "_" + type;
                            if (!result.Keys.Contains(currentKey))
                            {
                                double parseResult = 0;
                                if (double.TryParse(mplxlRange.Cells[i, excelParams.XColumnInMPL].FormulaLocal, out parseResult) && double.TryParse(mplxlRange.Cells[i, excelParams.YColumnInMPL].FormulaLocal, out parseResult) && double.TryParse(mplxlRange.Cells[i, excelParams.ZColumnInMPL].FormulaLocal, out parseResult))
                                    result.Add(currentKey, new WeldpointBMW("", "", "", spotNum, 1, double.Parse(mplxlRange.Cells[i, excelParams.XColumnInMPL].FormulaLocal), double.Parse(mplxlRange.Cells[i, excelParams.YColumnInMPL].FormulaLocal), double.Parse(mplxlRange.Cells[i, excelParams.ZColumnInMPL].FormulaLocal), 0, 0, 0, type, ""));
                                else
                                { }
                            }
                            else
                                doubleDeclerations.Add(currentKey);
                        }
                    }
                }
            }
            if (doubleDeclerations.Count > 0)
            {
                string pointstring = string.Empty;
                doubleDeclerations.ForEach(x => pointstring += x + ", ");
                pointstring = pointstring.Remove(pointstring.Length - 2, 2);
                MessageBox.Show("Multiple declarations of points " + pointstring + " in MPL!", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            CleanUpExcel("mpl");
            return result;
        }

        private static string GetSpotIndex(string index)
        {
            if (index.Length < 4)
                return "No found";
            return index.Substring(index.Length - 4, 4);

        }

        private static string GetProcessType(string processType, string procTypeFromVM)
        {
            if (!string.IsNullOrEmpty(processType))
            {
                if (processType.ToLower().Contains("blindnieten"))
                    return "BRT";
                if (processType.ToLower().Contains("clinchen"))
                    return "CLI";
                if (processType.ToLower().Contains("lochschrauben"))
                    return "FLS";
                if (processType.ToLower().Contains("bolzen"))
                    return "SWx";
                if (processType.ToLower().Contains("punktschweissen"))
                    return "LSP";
                if (processType.ToLower().Contains("stanznieten"))
                    return "RVT";
                if (processType.ToLower().Contains("buckelschw"))
                    return "SWx";
                if (processType.ToLower().Contains("swp"))
                    return "LSP";
                return string.Empty;
            }
            if (procTypeFromVM.ToLower().Contains("swp"))
                 return "LSP";        
            return procTypeFromVM;
        }

        public static void FormatAsTable(Excel.Range SourceRange, string TableName, string TableStyleName)
        {
            SourceRange.Worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange,
            SourceRange, System.Type.Missing, XlYesNoGuess.xlYes, System.Type.Missing).Name =
                TableName;
            SourceRange.Select();
            SourceRange.Worksheet.ListObjects[TableName].TableStyle = TableStyleName;
        }

        public static IDictionary<string, IWeldpoint> ConvertToIWeld(Dictionary<string, WeldpointBMW> tempPoints)
        {
            IDictionary<string, IWeldpoint> result = new Dictionary<string, IWeldpoint>();
            foreach (var item in tempPoints)
                result.Add(item.Key, item.Value);
            return result;
        }
    }
}
