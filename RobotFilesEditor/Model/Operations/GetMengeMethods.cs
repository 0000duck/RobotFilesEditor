using RobotFilesEditor.Model.DataInformations;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using Common = CommonLibrary.CommonMethods;
using RobotFilesEditor.Model.Operations.DataClass;
using System.Text.RegularExpressions;

namespace RobotFilesEditor.Model.Operations
{
    public class PointXYZ
    {
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double ZPos { get; set; }

        public PointXYZ(WeldpointBMW pkt)
        {
            XPos = pkt.XPos;
            YPos = pkt.YPos;
            ZPos = pkt.ZPos;
        }
    }

    public interface IProcessPoint
    {
        string Type { get; set; }
        string Path { get; set; }
    }

    public class ProcessPoint : IProcessPoint
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public List<WeldpointBMW> Points { get; set; }

        public ProcessPoint(WeldpointBMW point)
        {
            Type = point.ProcessType;
            Path = point.Path;
            Points = new List<WeldpointBMW>();
        }
    }

    public class ContinousPath : IProcessPoint
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public List<KeyValuePair<int, double>> Beads { get; set; }

        public ContinousPath(string type, string path)
        {
            Type = type;
            Path = path;
            Beads = new List<KeyValuePair<int, double>>();
        }
    }

    public class Safety
    {
        public List<string> SafeZones { get; set; }
        public List<string> SafeTools { get; set; }
        public Safety()
        {
            SafeZones = new List<string>();
            SafeTools = new List<string>();
        }
    }

    public class HandlingPath
    {
        public string Path { get; set; }
        public List<string> Points { get; set; }

        public HandlingPath(string path, List<string> points)
        {
            Path = path;
            Points = points;
        }
    }



    public static class GetMengeMethods
    {
        private static Excel.Application oXL;
        private static Excel.Workbooks oWBs;
        private static Excel.Workbook oWB;
        private static Excel.Sheets sheets;
        private static Excel.Worksheet oSheet;

        internal async static Task Execute()
        {
            try
            {
                MessageBox.Show("Select directory containing backups", "Select File", MessageBoxButton.OK, MessageBoxImage.Information);
                string backupDirectory = CommonLibrary.CommonMethods.SelectDirOrFile(true);
                if (string.IsNullOrEmpty(backupDirectory))
                    return;
                List<string> foundBackups = CommonLibrary.CommonMethods.FindBackupsInDirectory(backupDirectory, false, includeSafeRobot: false);
                if (DoubleBackupsFound(foundBackups))
                    return;
                Task<IDictionary<string, List<IProcessPoint>>> processPoints = GetProcessPointCounter(foundBackups);
                await Task.WhenAll(processPoints);
                IDictionary<string, Safety> safetyConfigs = GetSafety(foundBackups);
                IDictionary<string, List<HandlingPath>> handlingPaths = GetHandlingPaths(foundBackups);
                IDictionary<string, IDictionary<int, string>> collisions = GetCollisions(foundBackups);
                IDictionary<string, List<string>> reteachedPoints = GetReteachedPoints(foundBackups);
                IDictionary<string, int> nrOfHomes = GetNrOfHomes(foundBackups);
                WriteXLS(processPoints, safetyConfigs, handlingPaths,collisions,reteachedPoints,nrOfHomes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void WriteXLS(Task<IDictionary<string, List<IProcessPoint>>> processPoints, IDictionary<string, Safety> safetyConfigs, IDictionary<string, List<HandlingPath>> handlingPaths, IDictionary<string, IDictionary<int, string>> collisions, IDictionary<string, List<string>> reteachedPoints, IDictionary<string,int> nrOfHomes)
        {
            int counter = 1;
            oXL = new Excel.Application();
            oWBs = oXL.Workbooks;
            oWB = oWBs.Add("");
            sheets = oWB.Sheets;
            Excel.Range aRange = null;
            oSheet = sheets[1] as Excel.Worksheet;
            var robots = processPoints.Result.Reverse();
            foreach (var robot in robots)
            {
                int startRow = 0;
                int rowcounter = 0;
                if (counter > 1)
                    oWB.Worksheets.Add(oSheet);
                oSheet = oWB.ActiveSheet;
                oSheet.Name = robot.Key;
                if (robot.Value.Any(x => x is ProcessPoint))
                {
                    rowcounter++;
                    startRow = rowcounter;
                    oSheet.Cells[rowcounter, 1] = "Process Points";
                    aRange = oSheet.Range["A" + rowcounter, "C" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 14;
                    aRange.Merge();
                    aRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowcounter++;
                    aRange = oSheet.Range["A" + rowcounter, "C" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 12;
                    oSheet.Cells[rowcounter, 1] = "Path";
                    oSheet.Cells[rowcounter, 2] = "PointType";
                    oSheet.Cells[rowcounter, 3] = "Count";
                    rowcounter++;
                    foreach (var path in robot.Value.Where(x => x is ProcessPoint))
                    {
                        oSheet.Cells[rowcounter, 1] = path.Path;
                        oSheet.Cells[rowcounter, 2] = path.Type;
                        oSheet.Cells[rowcounter, 3] = (path as ProcessPoint).Points.Count;
                        rowcounter++;
                    }
                    aRange = oSheet.Range["A" + startRow, "C" + (rowcounter-1)];
                    aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                }
                if (robot.Value.Any(x => x is ContinousPath))
                {
                    rowcounter++;
                    startRow = rowcounter;
                    oSheet.Cells[rowcounter, 1] = "Continous Seams";
                    aRange = oSheet.Range["A" + rowcounter, "D"+rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 14;
                    aRange.Merge();
                    aRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowcounter++;
                    oSheet.Cells[rowcounter, 1] = "Path";
                    oSheet.Cells[rowcounter, 2] = "PointType";
                    oSheet.Cells[rowcounter, 3] = "Bead";
                    oSheet.Cells[rowcounter, 4] = "Length [mm]";
                    aRange = oSheet.Range["A" + rowcounter, "D" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 12;

                    rowcounter++;
                    foreach (var path in robot.Value.Where(x => x is ContinousPath))
                    {
                        foreach (var bead in (path as ContinousPath).Beads)
                        {
                            oSheet.Cells[rowcounter, 1] = path.Path;
                            oSheet.Cells[rowcounter, 2] = path.Type;
                            oSheet.Cells[rowcounter, 3] = bead.Key;
                            oSheet.Cells[rowcounter, 4] = bead.Value;
                            rowcounter++;
                        }
                    }
                    aRange = oSheet.Range["A" + startRow, "D" + (rowcounter-1)];
                    aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                }
                if (safetyConfigs.Keys.Contains(robot.Key))
                {
                    if (safetyConfigs[robot.Key].SafeTools.Count > 0)
                    {
                        rowcounter++;
                        startRow = rowcounter;
                        oSheet.Cells[rowcounter, 1] = "Safety Tools";
                        aRange = oSheet.Range["A" + rowcounter];
                        aRange.Font.Bold = true;
                        aRange.Font.Size = 14;

                        rowcounter++;
                        foreach (var tool in safetyConfigs[robot.Key].SafeTools)
                        {
                            oSheet.Cells[rowcounter, 1] = tool;
                            rowcounter++;
                        }
                    }

                    if (startRow > 0 && rowcounter > 1)
                    {
                        aRange = oSheet.Range["A" + startRow, "A" + (rowcounter - 1)];
                        aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                    }
                    if (safetyConfigs[robot.Key].SafeZones.Count > 0)
                    {
                        rowcounter++;
                        startRow = rowcounter;
                        oSheet.Cells[rowcounter, 1] = "Safety Zones";
                        aRange = oSheet.Range["A" + rowcounter];
                        aRange.Font.Bold = true;
                        aRange.Font.Size = 14;

                        rowcounter++;
                        foreach (var zone in safetyConfigs[robot.Key].SafeZones)
                        {
                            oSheet.Cells[rowcounter, 1] = zone;
                            rowcounter++;
                        }
                        aRange = oSheet.Range["A" + startRow, "A" + (rowcounter-1)];
                        aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                    }
                }
                if (handlingPaths.Keys.Contains(robot.Key) && handlingPaths[robot.Key].Count > 0)
                {
                    rowcounter++;
                    startRow = rowcounter;
                    oSheet.Cells[rowcounter, 1] = "Handling Points";
                    aRange = oSheet.Range["A" + rowcounter, "B" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 14;
                    aRange.Merge();
                    aRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowcounter++;
                    oSheet.Cells[rowcounter, 1] = "Path";
                    oSheet.Cells[rowcounter, 2] = "Point name";
                    aRange = oSheet.Range["A" + rowcounter, "B" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 12;

                    rowcounter++;
                    foreach (var path in handlingPaths[robot.Key])
                    {
                        bool firstRow = true;
                        foreach (var point in path.Points)
                        {
                            if (firstRow)
                            {
                                oSheet.Cells[rowcounter, 1] = path.Path;
                                firstRow = false;
                            }
                            oSheet.Cells[rowcounter, 2] = point;
                            rowcounter++;
                        }
                    }
                    aRange = oSheet.Range["A" + startRow, "B" + (rowcounter-1)];
                    aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                }
                if (collisions.Keys.Contains(robot.Key) && collisions[robot.Key].Count > 0)
                {
                    rowcounter++;
                    startRow = rowcounter;
                    oSheet.Cells[rowcounter, 1] = "Collisions";
                    aRange = oSheet.Range["A" + rowcounter, "B" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 14;
                    aRange.Merge();
                    aRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowcounter++;
                    oSheet.Cells[rowcounter, 1] = "CollNum";
                    oSheet.Cells[rowcounter, 2] = "Used in paths:";
                    aRange = oSheet.Range["A" + rowcounter, "B" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 12;
                    rowcounter++;
                    foreach (var coll in collisions[robot.Key])
                    {
                        oSheet.Cells[rowcounter, 1] = coll.Key;
                        oSheet.Cells[rowcounter, 2] = coll.Value;
                        rowcounter++;
                    }
                    aRange = oSheet.Range["A" + startRow, "B" + (rowcounter - 1)];
                    aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                }
                if (reteachedPoints[robot.Key] != null && reteachedPoints[robot.Key].Count > 0)
                {
                    rowcounter++;
                    startRow = rowcounter;
                    oSheet.Cells[rowcounter, 1] = "Retaught process points";
                    aRange = oSheet.Range["A" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 14;
                    aRange.Merge();
                    aRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowcounter++;
                    oSheet.Cells[rowcounter, 1] = "Point Name";
                    aRange = oSheet.Range["A" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 12;
                    rowcounter++;
                    foreach (var point in reteachedPoints[robot.Key])
                    {
                        oSheet.Cells[rowcounter, 1] = point;
                        rowcounter++;
                    }
                    aRange = oSheet.Range["A" + startRow, "A" + (rowcounter - 1)];
                    aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                }
                if (nrOfHomes[robot.Key] != 0)
                {
                    rowcounter++;
                    startRow = rowcounter;
                    oSheet.Cells[rowcounter, 1] = "Nr of Homes";
                    aRange = oSheet.Range["A" + rowcounter];
                    aRange.Font.Bold = true;
                    aRange.Font.Size = 14;
                    aRange.Merge();
                    aRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    rowcounter++;
                    oSheet.Cells[rowcounter, 1] = nrOfHomes[robot.Key].ToString();
                    rowcounter++;
                    aRange = oSheet.Range["A" + startRow, "A" + (rowcounter - 1)];
                    aRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                }

                aRange = oSheet.UsedRange;
                aRange.Columns.AutoFit();
                //ReadSpotsMethods.FormatAsTable(aRange, "Table1", "TableStyleMedium15");
                counter++;
            }
            oXL.Visible = true;
            Marshal.FinalReleaseComObject(aRange);
            Marshal.FinalReleaseComObject(oSheet);
            Marshal.FinalReleaseComObject(oWB);
            Marshal.FinalReleaseComObject(oWBs);
            Marshal.FinalReleaseComObject(oXL);
        }

        private static IDictionary<string, List<HandlingPath>> GetHandlingPaths(List<string> foundBackups)
        {
            IDictionary<string, List<HandlingPath>> result = new SortedDictionary<string, List<HandlingPath>>();            
            foreach (var backup in foundBackups)
            {
                List<SrcAndDat> dividedToFolds = new List<SrcAndDat>();
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    List<SrcAndDat> srcAndDats = new List<SrcAndDat>();
                    foreach (var entry in archive.Entries.Where(x => x.FullName.ToLower().ContainsAny(new string[] { "pick", "drop", "search", "suchen", "stack", "destack", "holen", "ablegen", "entne", "entnahmen", "ablagen" }) && Path.GetExtension(x.FullName) == ".src"))
                    {
                        if (archive.Entries.Any(x => x.FullName.ToLower() == entry.FullName.ToLower().Replace(".src", ".dat")))
                            srcAndDats.Add(new SrcAndDat(entry.FullName, entry.FullName.Remove(entry.FullName.Length - 4, 4) + ".dat"));
                    }                    
                    dividedToFolds = RenumberPointsMethods.DivideToFolds(srcAndDats, archive);
                    archive.Dispose();
                }
                result.Add(ReadSpotsMethods.GetRobot(backup), FindHandlingPoints(dividedToFolds));
            }
            return result;
        }

        private static IDictionary<string, int> GetNrOfHomes(List<string> foundBackups)
        {
            Regex nrOfHomesRegex = new Regex(@"(?<=(PLC_ACTIVE_HOMES|ACTIVE_HOMES)\s*\=\s*)\d+", RegexOptions.IgnoreCase);
            IDictionary<string, int> result = new Dictionary<string, int>();
            foreach (var backup in foundBackups)
            {
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    var entry = archive.Entries.FirstOrDefault(x => Path.GetFileName(x.FullName).ToLower().Contains("a01_plc_user"));
                    if (entry == null)
                    {
                        entry = archive.Entries.FirstOrDefault(x => Path.GetFileName(x.FullName).ToLower().Contains("$config"));
                    }
                    StreamReader reader = new StreamReader(entry.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (nrOfHomesRegex.IsMatch(line))
                        {
                            result.Add(ReadSpotsMethods.GetRobot(backup), int.Parse(nrOfHomesRegex.Match(line).ToString()));
                        }
                    }
                    reader.Close();
                }
            }
            return result;
        }

        private static IDictionary<string, List<string>> GetReteachedPoints(List<string> foundBackups)
        {
            Regex statusNumRegex = new Regex(@"(?<=E6POS.*,\s*S\s+)\d+", RegexOptions.IgnoreCase);
            Regex pointNameRegex = new Regex(@"(?<=E6POS\s+)\w+_\d+", RegexOptions.IgnoreCase);
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var backup in foundBackups)
            {
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    string backupName = ReadSpotsMethods.GetRobot(backup);
                    result.Add(backupName, new List<string>());
                    foreach (var entry in archive.Entries.Where(x => Path.GetExtension(x.FullName) == ".dat" && x.FullName.ToLower().Contains("_global") && x.FullName.ToLower().Contains("bmw_app")))
                    {
                        StreamReader reader = new StreamReader(entry.Open());
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (statusNumRegex.IsMatch(line) && pointNameRegex.IsMatch(line))
                            {
                                int status = int.Parse(statusNumRegex.Match(line).ToString());
                                if (status >= 16)
                                    result[backupName].Add(pointNameRegex.Match(line).ToString());
                            }
                        }
                        reader.Close();
                    }
                }
            }
            return result;
        }


        private static IDictionary<string, IDictionary<int, string>> GetCollisions(List<string> foundBackups)
        {
            IDictionary<string, IDictionary<int, string>> result = new SortedDictionary<string, IDictionary<int, string>>();
            foreach (var backup in foundBackups)
            {
                List<SrcAndDat> dividedToFolds = new List<SrcAndDat>();
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    List<SrcAndDat> srcAndDats = new List<SrcAndDat>();
                    foreach (var entry in archive.Entries.Where(x => Path.GetExtension(x.FullName) == ".src" && !x.FullName.ToLower().Contains("bmw_app")))
                    {
                        if (archive.Entries.Any(x => x.FullName.ToLower() == entry.FullName.ToLower().Replace(".src", ".dat")))
                            srcAndDats.Add(new SrcAndDat(entry.FullName, entry.FullName.Remove(entry.FullName.Length - 4, 4) + ".dat"));
                    }
                    dividedToFolds = RenumberPointsMethods.DivideToFolds(srcAndDats, archive);
                    archive.Dispose();
                }
                result.Add(ReadSpotsMethods.GetRobot(backup), FindCollisions(dividedToFolds));
            }
            return result;
        }

        private static SortedDictionary<int, string> FindCollisions(List<SrcAndDat> dividedToFolds)
        {
            SortedDictionary<int, string> result = new SortedDictionary<int, string>();
            Regex getCollNumRegex = new Regex(@"(?<=Plc_CollSafetyReq\d+\s*\(\s*)\d+", RegexOptions.IgnoreCase);
            foreach (var path in dividedToFolds)
            {
                foreach (var fold in path.SrcContent.Where(x=>getCollNumRegex.IsMatch(x)))
                {
                    int collnum = int.Parse(getCollNumRegex.Match(fold).ToString());
                    if (!result.Keys.Contains(collnum))
                        result.Add(collnum, Path.GetFileNameWithoutExtension(path.Src));
                    else
                        result[collnum] += ", " + Path.GetFileNameWithoutExtension(path.Src);
                }
            }
            return result;

        }

        private static IDictionary<string, Safety> GetSafety(List<string> foundBackups)
        {
            IDictionary<string, Safety> result = new SortedDictionary<string, Safety>();
            foreach (var backup in foundBackups)
            {
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    var entry = archive.Entries.FirstOrDefault(x => x.FullName.ToLower().Contains("safetyconfigdata.xml"));
                    if (entry != null)
                    {
                        var safetyfile = archive.GetEntry(entry.ToString());
                        string entrycontent = new StreamReader(entry.Open()).ReadToEnd();
                        archive.Dispose();
                        XElement safetXML = XElement.Parse(entrycontent);
                        Safety safety = new Safety();
                        foreach (var tool in safetXML.Element("ToolNameItems").Elements("string"))
                            if (!string.IsNullOrEmpty(tool.Value))
                                safety.SafeTools.Add(tool.Value);
                        foreach (var zone in safetXML.Element("MonitoringSpaceNameItems").Elements("string"))
                            if (!string.IsNullOrEmpty(zone.Value))
                                safety.SafeZones.Add(zone.Value);
                        string robot = ReadSpotsMethods.GetRobot(backup);
                        if (!result.Keys.Contains(robot))
                            result.Add(robot, safety);
                        else
                            MessageBox.Show("Double backup for robot " + robot + "!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } 
            }
            return result;
        }

        private static async Task<IDictionary<string, List<IProcessPoint>>> GetProcessPointCounter(List<string> foundBackups)
        {
            try {
                ReadSpotsMethods.errorMessage = string.Empty;
                IDictionary<string, IDictionary<string, IDictionary<int, double>>> pathsLength = null;
                IDictionary<string, IDictionary<string, List<int>>> result = new Dictionary<string, IDictionary<string, List<int>>>();
                string robotType = "KUKA";
                IDictionary<string, WeldpointBMW> resultWeldPoints = new Dictionary<string, WeldpointBMW>();
                if (robotType == "KUKA")
                {
                    IDictionary<string, WeldpointBMW> weldPoints = await ReadSpotsMethods.FindWeldPoints(foundBackups);
                    IDictionary<string, List<SpotsInFile>> usedSpots = ReadSpotsMethods.FindUsedPointsInSrcFiles(foundBackups);
                    IDictionary<string, WeldpointBMW> usedSpotsDict = ReadSpotsMethods.CreateDictWithUsedPoints(usedSpots);
                    resultWeldPoints = ReadSpotsMethods.CompareGlobalAndSrc(weldPoints as dynamic, usedSpotsDict);
                    IDictionary<string, IDictionary<string, IDictionary<int, List<WeldpointBMW>>>> continousPaths = GetContinousPaths(resultWeldPoints);
                    resultWeldPoints = ClearContinousWeldpoints(resultWeldPoints);
                    pathsLength = GetContinousPathsLenght(continousPaths);
                }
                await Task.WhenAll();
                IDictionary<string, List<IProcessPoint>> processPointDict = FillProcessPointDict(resultWeldPoints, pathsLength);
                foreach (var robot in foundBackups.Where(x => !processPointDict.Keys.Contains(ReadSpotsMethods.GetRobot(x))))
                    processPointDict.Add(ReadSpotsMethods.GetRobot(robot), new List<IProcessPoint>());
                if (!string.IsNullOrEmpty(ReadSpotsMethods.errorMessage))
                    MessageBox.Show(ReadSpotsMethods.errorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return processPointDict;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                return null;
            }
        }

        private static IDictionary<string, List<IProcessPoint>> FillProcessPointDict(IDictionary<string, WeldpointBMW> spotPoints, IDictionary<string, IDictionary<string, IDictionary<int, double>>> pathsLength)
        {
            IDictionary<string, List<IProcessPoint>> result = new SortedDictionary<string, List<IProcessPoint>>();
            foreach (var point in spotPoints)
            {
                string type = point.Value.ProcessType;
                if (type == "SWR" || type == "SWH")
                    type = "SWx";
                if (!result.Keys.Contains(point.Value.Robot))
                    result.Add(point.Value.Robot, new List<IProcessPoint>());
                if (!result[point.Value.Robot].Any(x => x.Path == point.Value.Path))
                    result[point.Value.Robot].Add(new ProcessPoint(point.Value));
                if (!result[point.Value.Robot].Any(x => x.Type == point.Value.ProcessType))
                    result[point.Value.Robot].Add(new ProcessPoint(point.Value));
                try
                {
                    (result[point.Value.Robot].Where(x => x.Path == point.Value.Path).Where(y => y.Type == type).First() as ProcessPoint).Points.Add(point.Value);
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                }
            }
            foreach (var robot in pathsLength)
            {
                if (!result.Keys.Contains(robot.Key))
                    result.Add(robot.Key, new List<IProcessPoint>());
                foreach (var path in robot.Value)
                {
                    if (!result[robot.Key].Any(x => x.Path == path.Key))
                    {
                        if (path.Key.ToLower().ContainsAny(new string[] { "arc_", "arcweld_" }))
                            result[robot.Key].Add(new ContinousPath("ARC", path.Key));
                        else if (path.Key.ToLower().ContainsAny(new string[] { "glue_", "kleben_", "kleb_" }))
                            result[robot.Key].Add(new ContinousPath("GL", path.Key));
                        else
                            result[robot.Key].Add(new ContinousPath("Unknown", path.Key));
                    }
                    foreach (var bead in path.Value)
                        (result[robot.Key].Where(x => x.Path == path.Key).Where(y => y.Type == "GL" || y.Type == "ARC" || y.Type == "Unknown").First() as ContinousPath).Beads.Add(new KeyValuePair<int, double>(bead.Key, bead.Value)); 
                }

            }
            return result;
        }

        private static IDictionary<string, WeldpointBMW> ClearContinousWeldpoints(IDictionary<string, WeldpointBMW> resultWeldPoints)
        {
            IDictionary<string, WeldpointBMW> result = new Dictionary<string, WeldpointBMW>();
            foreach (var item in resultWeldPoints.Where(x => x.Value.ProcessType != "GL" && x.Value.ProcessType != "ARC"))
                result.Add(item);
            return result;
        }

        private static IDictionary<string, IDictionary<string,IDictionary<int, double>>> GetContinousPathsLenght(IDictionary<string, IDictionary<string, IDictionary<int, List<WeldpointBMW>>>> continousPaths)
        {
            IDictionary<string, IDictionary<string, IDictionary<int, double>>> result = new Dictionary<string, IDictionary<string, IDictionary<int, double>>>();
            foreach (var robot in continousPaths)
            {
                foreach (var continouspath in robot.Value)
                {
                    foreach (var bead in continouspath.Value)
                    {
                        double beadLength = 0.0;
                        PointXYZ previousPoint = null;
                        foreach (var point in bead.Value)
                        {
                            if (previousPoint == null)
                                previousPoint = new PointXYZ(point);
                            else
                            {
                                PointXYZ currentPoint = new PointXYZ(point);
                                beadLength += CalculateDistance(previousPoint, currentPoint);
                                previousPoint = currentPoint;
                            }
                        }
                        if (!result.Keys.Contains(robot.Key))
                            result.Add(robot.Key, new Dictionary<string, IDictionary<int, double>>());
                        if (!result[robot.Key].Keys.Contains(continouspath.Key))
                            result[robot.Key].Add(continouspath.Key, new Dictionary<int, double>());
                        result[robot.Key][continouspath.Key].Add(bead.Key, Math.Round(beadLength, 2));
                    }

                }
            }
            return result;
        }

        private static double CalculateDistance(PointXYZ previousPoint, PointXYZ currentPoint)
        {
            double result = Math.Sqrt( Math.Pow((currentPoint.XPos-previousPoint.XPos),2) + Math.Pow((currentPoint.YPos - previousPoint.YPos), 2)+ Math.Pow((currentPoint.ZPos - previousPoint.ZPos), 2));
            return result;
        }

        private static IDictionary<string, IDictionary<string, IDictionary<int, List<WeldpointBMW>>>> GetContinousPaths(IDictionary<string, WeldpointBMW> resultWeldPoints)
        {
            IDictionary<string, IDictionary<string, IDictionary<int, List<WeldpointBMW>>>> result = new Dictionary<string, IDictionary<string, IDictionary<int, List<WeldpointBMW>>>>();
            foreach (var point in resultWeldPoints.Where(x=>x.Value.ProcessType == "GL" || x.Value.ProcessType == "ARC"))
            {   
                if (!result.Keys.Contains(point.Value.Robot))
                    result.Add(point.Value.Robot, new Dictionary<string, IDictionary<int, List<WeldpointBMW>>>());
                if (!result[point.Value.Robot].Keys.Contains(point.Value.Path))
                    result[point.Value.Robot].Add(point.Value.Path, new Dictionary<int, List<WeldpointBMW>>());
                if (!result[point.Value.Robot][point.Value.Path].Keys.Contains(point.Value.Number))
                    result[point.Value.Robot][point.Value.Path].Add(point.Value.Number, new List<WeldpointBMW>());
                result[point.Value.Robot][point.Value.Path][point.Value.Number].Add(point.Value);
            }

            return result;
        }

        //private static List<HandlingPath> FindHandlingPoints(List<SrcAndDat> dividedToFolds)
        //{
        //    Regex pointNameRegex = new Regex(@"(?<=;\s*FOLD\s+(PTP|LIN)\s+)[\w_-]+", RegexOptions.IgnoreCase);
        //    Regex pointPorVIARegex = new Regex(@"^(P\d+|via\d*|HOME\d+)", RegexOptions.IgnoreCase);
        //    Regex gripperChkRegex = new Regex(@";\s*FOLD\s+Grp\s+PartChk.*PlcControl\s*:\s*Control", RegexOptions.IgnoreCase);
        //    Regex gripperSetRegex = new Regex(@";\s*FOLD\s+Grp\s+(PosRet|PosAdv)", RegexOptions.IgnoreCase);
        //    Regex gripperSetTranferRegex = new Regex(@";\s*FOLD\s+Grp\s+(PosRet|PosAdv).*Part\s*:\s*transfered", RegexOptions.IgnoreCase);
        //    Regex searchRegex = new Regex(@"(?<=;\s*FOLD\s+(Search_S)[\w\s:]*(LIN\s+|FirstPart\s*:\s*))[\w_-]+", RegexOptions.IgnoreCase);
        //    List<HandlingPath> result = new List<HandlingPath>();
        //    foreach (var file in dividedToFolds)
        //    {
        //        List<string> handlinPoints = new List<string>();
        //        bool possibleHandlingPoint = false;
        //        string possibleHandlingPointName = string.Empty;
        //        foreach (var fold in file.SrcContent)
        //        {
        //            if (pointNameRegex.IsMatch(fold) || searchRegex.IsMatch(fold))
        //            {
        //                string tempPointName = string.Empty;
        //                if (pointNameRegex.IsMatch(fold))
        //                    tempPointName = pointNameRegex.Match(fold).ToString();
        //                else if (searchRegex.IsMatch(fold))
        //                    tempPointName = searchRegex.Match(fold).ToString();
        //                if (!pointPorVIARegex.IsMatch(tempPointName))
        //                {
        //                    possibleHandlingPointName = tempPointName;
        //                    possibleHandlingPoint = true;
        //                }
        //                else
        //                    possibleHandlingPoint = false;
        //            }
        //            if (gripperSetRegex.IsMatch(fold) && possibleHandlingPoint)
        //            {
        //                //if (!handlinPoints.Contains(possibleHandlingPointName))
        //                handlinPoints.Add(possibleHandlingPointName);
        //                possibleHandlingPoint = false;
        //                possibleHandlingPointName = string.Empty;
        //            }
        //            if (gripperChkRegex.IsMatch(fold))
        //                possibleHandlingPoint = false;
        //        }
        //        if (handlinPoints.Count>0)
        //            result.Add(new HandlingPath(Path.GetFileNameWithoutExtension(file.Src), handlinPoints));
        //    }
        //    return result;
        //}

        private static List<HandlingPath> FindHandlingPoints(List<SrcAndDat> dividedToFolds)
        {
            Regex pointNameRegex = new Regex(@"(?<=;\s*FOLD\s+(PTP|LIN)\s+)[\w_-]+", RegexOptions.IgnoreCase);
            Regex pointPorVIARegex = new Regex(@"^HOME\d+", RegexOptions.IgnoreCase);
            Regex gripperSetTranferRegex = new Regex(@";\s*FOLD\s+Grp\s+(PosRet|PosAdv).*Part\s*:\s*transfered", RegexOptions.IgnoreCase);
            Regex searchRegex = new Regex(@"(?<=;\s*FOLD\s+(Search_S)[\w\s:]*(LIN\s+|FirstPart\s*:\s*))[\w_-]+", RegexOptions.IgnoreCase);
            List<HandlingPath> result = new List<HandlingPath>();
            foreach (var file in dividedToFolds)
            {
                List<string> handlinPoints = new List<string>();
                string possibleHandlingPointName = string.Empty;
                foreach (var fold in file.SrcContent)
                {
                    if ((pointNameRegex.IsMatch(fold) || searchRegex.IsMatch(fold)) && !pointPorVIARegex.IsMatch(fold))
                    {
                        string tempPointName = string.Empty;
                        if (pointNameRegex.IsMatch(fold))
                            tempPointName = pointNameRegex.Match(fold).ToString();
                        else if (searchRegex.IsMatch(fold))
                            tempPointName = searchRegex.Match(fold).ToString();
                        possibleHandlingPointName = tempPointName;                        
                    }
                    if (gripperSetTranferRegex.IsMatch(fold))
                    {
                        if (!handlinPoints.Contains(possibleHandlingPointName))
                            handlinPoints.Add(possibleHandlingPointName);
                        //possibleHandlingPointName = string.Empty;
                    }
                }
                if (handlinPoints.Count > 0)
                    result.Add(new HandlingPath(Path.GetFileNameWithoutExtension(file.Src), handlinPoints));
            }
            return result;
        }

        private static bool DoubleBackupsFound(List<string> foundBackups)
        {
            List<string> backups = new List<string>();
            foreach (var backup in foundBackups)
            {
                string name = ReadSpotsMethods.GetRobot(backup);
                if (!backups.Contains(name))
                    backups.Add(name);
                else
                {
                    MessageBox.Show("Double backup found " + name + "!\r\nProgram will abort!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
            }
            return false;
        }
    }
}
