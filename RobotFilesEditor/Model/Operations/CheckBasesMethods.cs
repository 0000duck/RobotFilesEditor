using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using CommonLibrary.RobKalDatCommon;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CommonLibrary;
using System.Xml.Linq;
using System.Globalization;

namespace RobotFilesEditor.Model.Operations
{
    public static class CheckBasesMethods
    {
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static Excel.Application destinationXlApp;
        private static Excel.Workbook destinationXlWorkbook;
        private static Excel.Worksheet destinationXlWorksheet;

        internal static void Execute()
        {
            IDictionary<string, List<BaseData>> basesInRobKalDat = new Dictionary<string, List<BaseData>>();
            List<string> foundBackups = new List<string>();
            string project = CommonMethods.SelectDirOrFile(false, "XML File", "*.xml");
            if (!string.IsNullOrEmpty(project))
            {
                basesInRobKalDat = GetBasesFromProject(project);
                if (basesInRobKalDat == null)
                    return;
            }
            else
                return;
            string selectedDir = CommonMethods.SelectDirOrFile(true);
            if (!string.IsNullOrEmpty(selectedDir))
                foundBackups = CommonLibrary.CommonMethods.FindBackupsInDirectory(selectedDir, false);
            else
                return;
            IDictionary<string, List<BaseData>> basesInBackups = GetBasesFromBackups(foundBackups);
            if (basesInBackups == null)
                return;
            CompareBackupAndRobKalDat(basesInRobKalDat, basesInBackups, selectedDir);


        }

        private static void CompareBackupAndRobKalDat(IDictionary<string, List<BaseData>> basesInRobKalDat, IDictionary<string, List<BaseData>> basesInBackups, string saveDir)
        {
            List<RobkaldatAndBackupPair> robkaldatAndBackupPairs = GetRobkadatAndBackupPairs(basesInRobKalDat, basesInBackups);
            WriteExcel(saveDir, robkaldatAndBackupPairs);
        }

        private static void WriteExcel(string saveDir, List<RobkaldatAndBackupPair> robkaldatAndBackupPairs)
        {
            int rowsCount = GetExcelRowsCount(robkaldatAndBackupPairs);
            object[,] arr = new object[rowsCount,23];
            string savePath = saveDir + "\\BasesComapred.xlsx";
            int rowsCounter = 1;
            destinationXlApp = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;

            destinationXlWorkbook = destinationXlApp.Workbooks.Add(misValue);
            destinationXlWorksheet = (Excel.Worksheet)destinationXlWorkbook.Worksheets.get_Item(1);

            arr[0, 0] = "Robot";
            arr[0, 1] = "Base Number";
            arr[0, 2] = "Base Name in RobKalDat";
            arr[0, 3] = "Base Name in Backup";
            arr[0, 4] = "X in RobKalDat";
            arr[0, 5] = "Y in RobKalDat";
            arr[0, 6] = "Z in RobKalDat";
            arr[0, 7] = "A in RobKalDat";
            arr[0, 8] = "B in RobKalDat";
            arr[0, 9] = "C in RobKalDat";
            arr[0, 10] = "X in Backup";
            arr[0, 11] = "Y in Backup";
            arr[0, 12] = "Z in Backup";
            arr[0, 13] = "A in Backup";
            arr[0, 14] = "B in Backup";
            arr[0, 15] = "C in Backup";
            arr[0, 16] = "Diff X";
            arr[0, 17] = "Diff Y";
            arr[0, 18] = "Diff Z";
            arr[0, 19] = "Diff RX";
            arr[0, 20] = "Diff RY";
            arr[0, 21] = "Diff RZ";
            arr[0, 22] = "Comment";
            
            foreach (RobkaldatAndBackupPair basePair in robkaldatAndBackupPairs)
            {
                foreach (var robotBase in basePair.BasesInBackups)
                {
                    arr[rowsCounter, 0] = basePair.Robot;
                    arr[rowsCounter, 1] = robotBase.Key;
                    arr[rowsCounter, 3] = robotBase.Value.Name;
                    arr[rowsCounter, 10] = robotBase.Value.CaluculatedBase.XPos;
                    arr[rowsCounter, 11] = robotBase.Value.CaluculatedBase.YPos;
                    arr[rowsCounter, 12] = robotBase.Value.CaluculatedBase.ZPos;
                    arr[rowsCounter, 13] = robotBase.Value.CaluculatedBase.RX;
                    arr[rowsCounter, 14] = robotBase.Value.CaluculatedBase.RY;
                    arr[rowsCounter, 15] = robotBase.Value.CaluculatedBase.RZ;
                    if (basePair.BasesInRobKalDat.Keys.Contains(robotBase.Key))
                    {
                        arr[rowsCounter, 2] = basePair.BasesInRobKalDat[robotBase.Key].Name;
                        arr[rowsCounter, 4] = basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.XPos;
                        arr[rowsCounter, 5] = basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.YPos;
                        arr[rowsCounter, 6] = basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.ZPos;
                        arr[rowsCounter, 7] = basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.RX;
                        arr[rowsCounter, 8] = basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.RY;
                        arr[rowsCounter, 9] = basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.RZ;
                        arr[rowsCounter, 16] = Math.Abs(basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.XPos - robotBase.Value.CaluculatedBase.XPos);
                        arr[rowsCounter, 17] = Math.Abs(basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.YPos - robotBase.Value.CaluculatedBase.YPos);
                        arr[rowsCounter, 18] = Math.Abs(basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.ZPos - robotBase.Value.CaluculatedBase.ZPos);
                        arr[rowsCounter, 19] = Math.Abs(basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.RX - robotBase.Value.CaluculatedBase.RX);
                        arr[rowsCounter, 20] = Math.Abs(basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.RY - robotBase.Value.CaluculatedBase.RY);
                        arr[rowsCounter, 21] = Math.Abs(basePair.BasesInRobKalDat[robotBase.Key].CaluculatedBase.RZ - robotBase.Value.CaluculatedBase.RZ);
                    }
                    arr[rowsCounter, 22] = "";                   
                    rowsCounter++;
                }
            }
            Excel.Range c1 = (Excel.Range)destinationXlWorksheet.Cells[1, 1];
            Excel.Range c2 = (Excel.Range)destinationXlWorksheet.Cells[rowsCounter, 23];
            Excel.Range aRange = destinationXlWorksheet.get_Range(c1, c2);
            aRange.Value = arr;

            Excel.Range usedRange = destinationXlWorksheet.UsedRange;
            usedRange.Columns.AutoFit();
            ReadSpotsMethods.FormatAsTable(usedRange, "Table1", "TableStyleMedium15");

            Excel.Range range = destinationXlWorksheet.Range["Q2","V"+ rowsCounter.ToString()];
            Excel.FormatConditions fcs = range.FormatConditions;
            Excel.FormatCondition condition = (Excel.FormatCondition)fcs.Add(
                Type: Excel.XlFormatConditionType.xlCellValue,
                Operator: Excel.XlFormatConditionOperator.xlGreater,
                Formula1: 1);
            condition.Interior.ColorIndex = 3;

            try
            {
                if (!File.Exists(savePath) || !CommonLibrary.CommonMethods.IsFileLocked(savePath))
                {
                    destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    System.Windows.MessageBox.Show("Succesfully saved at " + savePath, "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    destinationXlWorkbook.Close(true, misValue, misValue);
                }
                else
                    System.Windows.MessageBox.Show("File " + savePath + " is used by another process. Close the file and try again", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                CleanUpExcel();

            }
            catch
            {
                CleanUpExcel();
                System.Windows.MessageBox.Show("Something went wrong", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

        }

        private static int GetExcelRowsCount(List<RobkaldatAndBackupPair> robkaldatAndBackupPairs)
        {
            int result = 1;
            robkaldatAndBackupPairs.ForEach(x => result += x.BasesInBackups.Count);
            return result;
        }

        private static List<RobkaldatAndBackupPair> GetRobkadatAndBackupPairs(IDictionary<string, List<BaseData>> basesInRobKalDat, IDictionary<string, List<BaseData>> basesInBackups)
        {

            List<RobkaldatAndBackupPair> result = new List<RobkaldatAndBackupPair>();
            foreach (string robotInRobkalDat in basesInRobKalDat.Keys)
            {
                int stationInRobKalDat = GetStationNumber(robotInRobkalDat, true);
                int robotNumInRobKalDat = GetRobotNumber(robotInRobkalDat, true);
                RobkaldatAndBackupPair robkaldatAndBackupPair = new RobkaldatAndBackupPair();
                robkaldatAndBackupPair.Station = stationInRobKalDat;
                robkaldatAndBackupPair.RobotNumber = robotNumInRobKalDat;
                robkaldatAndBackupPair.Robot = robotInRobkalDat;
                robkaldatAndBackupPair.BasesInRobKalDat = new Dictionary<int, BaseData>();
                foreach (var robotBase in basesInRobKalDat[robotInRobkalDat])
                {
                    robkaldatAndBackupPair.BasesInRobKalDat.Add(robotBase.Number, robotBase);
                }

                robkaldatAndBackupPair.BasesInBackups = new Dictionary<int, BaseData>();

                foreach (var robotBackup in basesInBackups)
                {
                    if (GetStationNumber(robotBackup.Key, false) == stationInRobKalDat && GetRobotNumber(robotBackup.Key, false) == robotNumInRobKalDat)
                    {
                        foreach (var robotBaseInBackup in robotBackup.Value)
                        {
                            robkaldatAndBackupPair.BasesInBackups.Add(robotBaseInBackup.Number, robotBaseInBackup);                            
                        }
                        break;
                    }
                }
                result.Add(robkaldatAndBackupPair);
            }
            return result;
        }


        private static int GetStationNumber(string robotBase, bool isRobKalDat)
        {
            Regex stationInRobKalDatRegex = new Regex(@"\d+(?=(|\+)IR)", RegexOptions.IgnoreCase);
            Regex stationInBackupRegex = new Regex(@"\d+(?=(IR|R))", RegexOptions.IgnoreCase);
            if (isRobKalDat)
                return int.Parse(stationInRobKalDatRegex.Match(robotBase).ToString());
            else
                return int.Parse(stationInBackupRegex.Match(robotBase).ToString());
        }

        private static int GetRobotNumber(string robotBase, bool isRobKalDat)
        {
            Regex robotNumberInRobKalDatRegex = new Regex(@"(?<=IR)\d+", RegexOptions.IgnoreCase);
            Regex robotNumberInBackupRegex = new Regex(@"(?<=(IR|R))\d+", RegexOptions.IgnoreCase);
            if (isRobKalDat)
                return int.Parse(robotNumberInRobKalDatRegex.Match(robotBase).ToString());
            else
                return int.Parse(robotNumberInBackupRegex.Match(robotBase).ToString());
        }

        private static IDictionary<string, List<BaseData>> GetBasesFromBackups(List<string> foundBackups)
        {
            IDictionary<string, List<BaseData>> result = new Dictionary<string, List<BaseData>>();
            foreach(string backup in foundBackups)
            {
                string robotName = GetRobotNameFromBackup(backup);
                using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries.Where(x => x.FullName.ToLower().Contains("$config.dat") && !x.FullName.ToLower().Contains("steu")))
                    {
                        List<CommonLibrary.IRobotPoint> bases = ReadBackupsForWBMethods.GetBaseDatas(entry: entry);
                        List<DataClass.BaseName> baseNames = ReadBackupsForWBMethods.GetBaseNames(entry: entry);
                        List<DataClass.BaseType> baseTypes = ReadBackupsForWBMethods.GetBaseTypes(entry: entry);
                        List<BaseData> resultBaseData = CreateBaseDataFromBackup(bases, baseNames, baseTypes, robotName);
                        if (!result.Keys.Contains(robotName))
                        {
                            result.Add(robotName, resultBaseData);
                        }
                        else
                        {
                            //result[robotName].Add(resultBaseData);
                            System.Windows.MessageBox.Show("Double backup of robot " + robotName + "\r\nProgram will abort!", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                            return null;
                        }
                    }
                }
            }

            return result;
        }

        private static string GetRobotNameFromBackup(string backup)
        {
            string filename = Path.GetFileNameWithoutExtension(backup);
            Regex getRobotNameRegex = new Regex(@"\d+(ir|_ir)\d+", RegexOptions.IgnoreCase);
            //string result = filename.Substring(filename.Length - 5, 5);
            string result = getRobotNameRegex.Match(filename).ToString().Replace("_","");
            return result;
        }

        private static List<BaseData> CreateBaseDataFromBackup(List<CommonLibrary.IRobotPoint> bases, List<DataClass.BaseName> baseNames, List<DataClass.BaseType> baseTypes, string robotname)
        {
            List<BaseData> result = new List<BaseData>();
            int counter = 0;
            foreach (var robotbase in bases)
            {
                bool? baseType = GetExtBase(baseTypes[counter]);
                if (baseType.HasValue)
                {
                    //if (baseType.Value == false)
                    //{
                        BaseData baseData = new BaseData(baseNames[counter].Name, counter + 1, GetExtBase(baseTypes[counter]), robotname, baseNames[0].Name, new Point(), new Point());
                        baseData.CaluculatedBase = new Point(bases[counter].X, bases[counter].Y, bases[counter].Z, (bases[counter] as DataClass.BaseDataKUKA).A, (bases[counter] as DataClass.BaseDataKUKA).B, (bases[counter] as DataClass.BaseDataKUKA).C);
                        result.Add(baseData);
                    //}
                    //else
                   // {

                   // }
                }
                
                counter++;
            }
            return result;
        }

        private static bool? GetExtBase(DataClass.BaseType baseType)
        {
            if (baseType.Type.ToLower().Contains("#base"))
                return false;
            else if (baseType.Type.ToLower().Contains("#tcp"))
                return true;
            else
                return null;
        }

        private static IDictionary<string, List<BaseData>> GetBasesFromProject(string project)
        {
            List<string> baseStrings = GetBaseStrings(project);
            //IDictionary<string, List<BaseData>> result = GetBases(baseStrings);
            IDictionary<string, List<BaseData>> result = GetBasesNew(project);
            if (result == null)
                return null;
            result = CalculateBases(result);
            return result;
        }

        private static IDictionary<string, List<BaseData>> GetBasesNew(string project)
        {
            XDocument docu = XDocument.Load(project);
            IDictionary<string, List<BaseData>> result = new Dictionary<string, List<BaseData>>();
            var basexmls = docu.Element("Project").Element("Bases").Elements("Base");
            foreach (var basexml in basexmls)
            {
                string name = basexml.Element("Base_name").Value;
                int number = int.Parse(basexml.Element("Base_Nr").Value);
                bool extbase = basexml.Element("Ext_Base").Value == "False" ? false : true;
                string robot = basexml.Element("Obj1").Attribute("Name").Value;
                string worktool = basexml.Element("Obj2").Attribute("Name").Value;
                double x1 = double.Parse(basexml.Element("Obj1").Attribute("X").Value, CultureInfo.InvariantCulture);
                double y1 = double.Parse(basexml.Element("Obj1").Attribute("Y").Value, CultureInfo.InvariantCulture);
                double z1 = double.Parse(basexml.Element("Obj1").Attribute("Z").Value, CultureInfo.InvariantCulture);
                double rx1 = double.Parse(basexml.Element("Obj1").Attribute("RX").Value, CultureInfo.InvariantCulture);
                double ry1 = double.Parse(basexml.Element("Obj1").Attribute("RY").Value, CultureInfo.InvariantCulture);
                double rz1 = double.Parse(basexml.Element("Obj1").Attribute("RZ").Value, CultureInfo.InvariantCulture);

                string worktoolXMLName = extbase ? "Adjusted_TCP" : "Obj2";
                double x2 = double.Parse(basexml.Element(worktoolXMLName).Attribute("X").Value, CultureInfo.InvariantCulture);
                double y2 = double.Parse(basexml.Element(worktoolXMLName).Attribute("Y").Value, CultureInfo.InvariantCulture);
                double z2 = double.Parse(basexml.Element(worktoolXMLName).Attribute("Z").Value, CultureInfo.InvariantCulture);
                double rx2 = double.Parse(basexml.Element(worktoolXMLName).Attribute("RX").Value, CultureInfo.InvariantCulture);
                double ry2 = double.Parse(basexml.Element(worktoolXMLName).Attribute("RY").Value, CultureInfo.InvariantCulture);
                double rz2 = double.Parse(basexml.Element(worktoolXMLName).Attribute("RZ").Value, CultureInfo.InvariantCulture);

                //BaseData basedata = new BaseData()
                if (!result.Keys.Contains(robot))
                    result.Add(robot, new List<BaseData>());
                result[robot].Add(new BaseData(name, number, extbase, robot, worktool, new Point(x1, y1, z1, rx1, ry1, rz1), new Point(x2, y2, z2, rx2, ry2, rz2)));
            }
            return result;
            
        }

        private static List<string> GetBaseStrings(string project)
        {
            string currentstring = "";
            bool addLine = false;
            List<string> result = new List<string>();
            StreamReader reader = new StreamReader(project);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("<base id"))
                    addLine = true;
                if (addLine)
                    currentstring += line + "\r\n";
                if (line.ToLower().Contains("</base>"))
                {
                    addLine = false;
                    result.Add(currentstring);
                    currentstring = "";
                }
            }
            reader.Close();
            return result;
        }

        //private static IDictionary<string, List<BaseData>> GetBases(List<string> baseStrings)
        //{
        //    string line = string.Empty;
        //    try
        //    {
        //        Regex nameRegex = new Regex(@"(?<=Base_name>).*(?=<)", RegexOptions.IgnoreCase);
        //        Regex numberRegex = new Regex(@"(?<=Base_Nr>)\d+", RegexOptions.IgnoreCase);
        //        Regex extBaseRegex = new Regex(@"(?<=Ext_Base>).*(?=<)", RegexOptions.IgnoreCase);
        //        Regex robotRegex = new Regex("(?<=Obj1\\s+Name=\\s*\")[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
        //        Regex worktoolRegex = new Regex("(?<=Obj2\\s+Name=\\s*\")[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
        //        Regex xRegex = new Regex("(?<=\\s+X=\")(|-)(\\d+.\\d+|\\d+)", RegexOptions.IgnoreCase);
        //        Regex yRegex = new Regex("(?<=\\s+Y=\")(|-)(\\d+.\\d+|\\d+)", RegexOptions.IgnoreCase);
        //        Regex zRegex = new Regex("(?<=\\s+Z=\")(|-)(\\d+.\\d+|\\d+)", RegexOptions.IgnoreCase);
        //        Regex rxRegex = new Regex("(?<=\\s+RX=\")(|-)(\\d+.\\d+|\\d+)", RegexOptions.IgnoreCase);
        //        Regex ryRegex = new Regex("(?<=\\s+RY=\")(|-)(\\d+.\\d+|\\d+)", RegexOptions.IgnoreCase);
        //        Regex rzRegex = new Regex("(?<=\\s+RZ=\")(|-)(\\d+.\\d+|\\d+)", RegexOptions.IgnoreCase);

        //        IDictionary<string, List<BaseData>> result = new Dictionary<string, List<BaseData>>();
        //        foreach (string baseString in baseStrings)
        //        {

        //            string name = nameRegex.Match(baseString).ToString();
        //            int number = int.Parse(numberRegex.Match(baseString).ToString());
        //            bool extBase = false;
        //            if (extBaseRegex.Match(baseString).ToString().ToLower().Contains("true"))
        //                extBase = true;
        //            string robot = robotRegex.Match(baseString).ToString();
        //            string worktool = worktoolRegex.Match(baseString).ToString();
        //            double x1 = 0, y1 = 0, z1 = 0, rx1 = 0, ry1 = 0, rz1 = 0, x2 = 0, y2 = 0, z2 = 0, rx2 = 0, ry2 = 0, rz2 = 0;
        //            double? xExt = null, yExt = null, zExt = null, rxExt = null, ryExt = null, rzExt = null;

        //            StringReader reader = new StringReader(baseString);
        //            while (true)
        //            {
        //                line = reader.ReadLine();
        //                if (line == null)
        //                    break;
        //                if (line.ToLower().Contains("<obj1"))
        //                {
        //                    x1 = double.Parse(xRegex.Match(line).ToString().Replace(".", ","));
        //                    y1 = double.Parse(yRegex.Match(line).ToString().Replace(".", ","));
        //                    z1 = double.Parse(zRegex.Match(line).ToString().Replace(".", ","));
        //                    rx1 = double.Parse(rxRegex.Match(line).ToString().Replace(".", ","));
        //                    ry1 = double.Parse(ryRegex.Match(line).ToString().Replace(".", ","));
        //                    rz1 = double.Parse(rzRegex.Match(line).ToString().Replace(".", ","));
        //                }
        //                if (line.ToLower().Contains("<obj2"))
        //                {
        //                    x2 = double.Parse(xRegex.Match(line).ToString().Replace(".", ","));
        //                    y2 = double.Parse(yRegex.Match(line).ToString().Replace(".", ","));
        //                    z2 = double.Parse(zRegex.Match(line).ToString().Replace(".", ","));
        //                    rx2 = double.Parse(rxRegex.Match(line).ToString().Replace(".", ","));
        //                    ry2 = double.Parse(ryRegex.Match(line).ToString().Replace(".", ","));
        //                    rz2 = double.Parse(rzRegex.Match(line).ToString().Replace(".", ","));
        //                }
        //                if (line.ToLower().Contains("<adjusted_tcp"))
        //                {
        //                    x2 = double.Parse(xRegex.Match(line).ToString().Replace(".", ","));
        //                    y2 = double.Parse(yRegex.Match(line).ToString().Replace(".", ","));
        //                    z2 = double.Parse(zRegex.Match(line).ToString().Replace(".", ","));
        //                    rx2 = double.Parse(rxRegex.Match(line).ToString().Replace(".", ","));
        //                    ry2 = double.Parse(ryRegex.Match(line).ToString().Replace(".", ","));
        //                    rz2 = double.Parse(rzRegex.Match(line).ToString().Replace(".", ","));
        //                }
        //            }
        //            reader.Close();
        //            if (!result.Keys.Contains(robot))
        //                result.Add(robot, new List<BaseData>());
        //            //if (!xExt.HasValue)
        //            result[robot].Add(new BaseData(name, number, extBase, robot, worktool, new Point(x1, y1, z1, rx1, ry1, rz1), new Point(x2, y2, z2, rx2, ry2, rz2)));
        //            //else
        //            //    result[robot].Add(new BaseData(name, number, extBase, robot, worktool, new Point(x1, y1, z1, rx1, ry1, rz1), new Point(x2, y2, z2, rx2, ry2, rz2),"",new Point(xExt.Value,yExt.Value, zExt.Value, rxExt.Value, ryExt.Value, rzExt.Value)));
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show("Problem w linii:\r\n" + line + "\r\nBłąd: " + ex.Message, "Error" , System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        //        return null;
        //    }
        //}

        private static IDictionary<string, List<BaseData>> CalculateBases(IDictionary<string, List<BaseData>> inputDict)
        {
            IDictionary<string, List<BaseData>> result = new Dictionary<string, List<BaseData>>();
            foreach (var robot in inputDict)
            {
                result.Add(robot.Key, new List<BaseData>());
                foreach (var robotBase in robot.Value)
                {
                    Point calculatedBase = new Point();
                    //if (robotBase.ExtBase == false)
                        calculatedBase = CommonMethods.CalculateBases(robotBase.RobotCoordinates, robotBase.WorktoolCoordinates);
                    //else
                    //{
                        //TODO - extrenalowy base
                    //}
                    BaseData baseData = robotBase;
                    baseData.CaluculatedBase = calculatedBase;
                    result[robot.Key].Add(baseData);
                }
            }
            return result;
        }

        //private static double GetRobKalDatAngle(double rX1, double rX2)
        //{
        //    double difference = rX1 - rX2;
        //    if (Math.Abs(difference) > 180)
        //    {
        //        if (difference > 180)
        //            return difference - 360;
        //        else
        //            return difference + 360;
        //    }
        //    return difference;
        //}

        //public static double ConvertToRadians(double angle)
        //{
        //    return (Math.PI / 180) * angle;
        //}

        private static void CleanUpExcel()
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

    }
}
