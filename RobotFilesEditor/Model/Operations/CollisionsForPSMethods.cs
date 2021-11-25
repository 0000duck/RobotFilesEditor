using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WinForms = System.Windows.Forms;
using static RobotFilesEditor.Model.DataInformations.FileValidationData;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RobotFilesEditor.Model.Operations
{
    public class CollisionsItems
    {
        public string Robot1 { get; set; }
        public string Robot2 { get; set; }
    }

    public static class CollisionsForPSMethods
    {
        private static Excel.Application destinationXlApp;
        private static Excel.Workbook destinationXlWorkbook;
        private static Excel.Worksheet destinationXlWorksheet;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static IDictionary<string, List<string>> signalsToAdd;
        public static List<CollisionWithoutDescr> collisions;

        internal static void Execute()
        {
            signalsToAdd = new Dictionary<string, List<string>>();
            MessageBox.Show("Select folder with paths from all robots", "Select folder", MessageBoxButton.OK, MessageBoxImage.Information);
            string dir = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            if (string.IsNullOrEmpty(dir))
                return;
            string destinationDir = Path.Combine(dir, "PathsWithCollisions");
            if (!DestinationDirectoryCreator(destinationDir))
                return;
            List<string> foundsrcFiles = Directory.GetFiles(dir, "*.src", SearchOption.AllDirectories).ToList();
            IDictionary<string, string> srcFiles = new Dictionary<string, string>();
            foundsrcFiles.ForEach(x => srcFiles.Add(x,File.ReadAllText(x)));
            string contrType = DetectControllerType(srcFiles);
            if (contrType == string.Empty)
                return;
            //collisions = SrcValidator.GetCollisions(srcFiles);
            //collisionsWithDescription = SrcValidator.GetCollisionsWithDescription(srcFiles,contrType);
            IDictionary<string, List<int>> collisionsOnRobots = FindCollisions(srcFiles, contrType);
            IDictionary<int, CollisionsItems> collisionsAndRobots = AssignCollsToRobots(collisionsOnRobots);
            IDictionary<string, string> resultFiles =  AddCollLines(srcFiles, collisionsAndRobots);
            if (resultFiles.Count > 0)
            {
                foreach (var file in resultFiles)
                {
                    string robotname = FindRobotName(file.Key);
                    string dirWithoutBegin = Path.Combine(destinationDir, Path.GetDirectoryName(file.Key.Replace(dir + "\\", "")));
                    Directory.CreateDirectory(dirWithoutBegin);
                    File.WriteAllText(Path.Combine(dirWithoutBegin, Path.GetFileName(file.Key)),file.Value);
                    //File.Copy(file.Key.Replace(".src", ".dat"), Path.Combine(dirWithoutBegin, Path.GetFileName(file.Key).Replace(".src", ".dat")));
                    Directory.GetFiles(Path.GetDirectoryName(file.Key)).ToList().Where(x => Path.GetExtension(x).Equals(".dat", StringComparison.OrdinalIgnoreCase) && x.ToLower().Contains(Path.GetFileNameWithoutExtension(file.Key.ToLower()))).ToList().ForEach(y => File.Copy(y, Path.Combine(dirWithoutBegin, Path.GetFileName(y))));
                    BuildSignalList(Path.GetDirectoryName(Path.Combine(dirWithoutBegin, Path.GetFileName(file.Key))),robotname);
                }             
            }
        }

        private static void BuildSignalList(string dirToSave, string currentRobot)
        {
            try
            {
                if (!File.Exists(Path.Combine(dirToSave, currentRobot + ".xls")))
                {
                    int startAddress = 4, counter = 0;
                    destinationXlApp = new Excel.Application();
                    object misValue = System.Reflection.Missing.Value;

                    destinationXlWorkbook = destinationXlApp.Workbooks.Add(misValue);
                    destinationXlWorksheet = (Excel.Worksheet)destinationXlWorkbook.Worksheets.get_Item(1);

                    destinationXlWorksheet.Cells[1, 1] = "PREFIX";
                    destinationXlWorksheet.Cells[3, 1] = "InterfaceName";
                    destinationXlWorksheet.Cells[3, 2] = "RobotInternalName";
                    destinationXlWorksheet.Cells[3, 3] = "I_Q";
                    destinationXlWorksheet.Cells[3, 4] = "TYPE";
                    destinationXlWorksheet.Cells[3, 5] = "Address";
                    destinationXlWorksheet.Cells[3, 6] = "External_Connection";
                    destinationXlWorksheet.Cells[3, 7] = "Comment";
                    foreach (var signal in signalsToAdd[currentRobot])
                    {
                        destinationXlWorksheet.Cells[startAddress + counter, 1] = signal;
                        destinationXlWorksheet.Cells[startAddress + counter, 2] = signal;
                        destinationXlWorksheet.Cells[startAddress + counter, 3] = "I";
                        destinationXlWorksheet.Cells[startAddress + counter, 4] = "BOOL";
                        destinationXlWorksheet.Cells[startAddress + counter, 5] = "No Address";
                        counter++;
                    }
                    foreach (var signal in signalsToAdd[currentRobot])
                    {
                        destinationXlWorksheet.Cells[startAddress + counter, 1] = signal;
                        destinationXlWorksheet.Cells[startAddress + counter, 2] = signal;
                        destinationXlWorksheet.Cells[startAddress + counter, 3] = "Q";
                        destinationXlWorksheet.Cells[startAddress + counter, 4] = "BOOL";
                        destinationXlWorksheet.Cells[startAddress + counter, 5] = "No Address";
                        counter++;
                    }
                    destinationXlWorkbook.SaveAs(Path.Combine(dirToSave, currentRobot + ".xls"), Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    destinationXlWorkbook.Close(true, misValue, misValue);
                    CleanUpExcel();
                }
            }
            catch (Exception ex)
            {
                CleanUpExcel();
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool DestinationDirectoryCreator(string destinationDir)
        {         
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);
            else
            {
                WinForms.DialogResult dialogResult = WinForms.MessageBox.Show("Directory " + destinationDir + " already exists. Overwrite?", "Overwrite dir?", WinForms.MessageBoxButtons.YesNo, WinForms.MessageBoxIcon.Question);
                if (dialogResult == WinForms.DialogResult.Yes)
                {
                    DirectoryInfo dirToRemove = new DirectoryInfo(destinationDir);
                    List<string> files = Directory.GetFiles(destinationDir, "*.*", SearchOption.AllDirectories).ToList();
                    files.ForEach(x => File.Delete(x));
                    dirToRemove.Delete(true);
                    Directory.CreateDirectory(destinationDir);
                }
                else
                    return false;
            }
            return true;
        }

        private static IDictionary<string, string> AddCollLines(IDictionary<string, string> srcFiles, IDictionary<int, CollisionsItems> collisionsAndRobots)
        {
            Regex numberRegex = new Regex(@"(?<=ZoneNum\s*:|Zone\s*=\s*)\d+", RegexOptions.IgnoreCase);
            IDictionary<string, string> resultFiles = new Dictionary<string, string>();
            foreach (var file in srcFiles)
            {
                string currentFileContent = string.Empty;
                StringReader reader = new StringReader(file.Value);
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (line.ToLower().Contains("fold") && (line.ToLower().Contains("collzone") || line.ToLower().Contains("collision_zone")) && line.ToLower().Contains("request"))
                    {
                        int number = int.Parse(numberRegex.Match(line).ToString());
                        if (collisionsAndRobots.Keys.Contains(number))
                        {
                            string robotCurrentRobot = FindRobotName(file.Key);
                            string robotToAdd = string.Empty;
                            if (collisionsAndRobots[number].Robot1 == robotCurrentRobot)
                                robotToAdd = collisionsAndRobots[number].Robot2;
                            else
                                robotToAdd = collisionsAndRobots[number].Robot1;
                            line = String.Format("# WaitSignal coll{0} 0\r\n# Destination {1}\r\n# Send coll{0} 1\r\n", number,robotToAdd)+line;
                        }
                    }
                    if (line.ToLower().Contains("fold") && (line.ToLower().Contains("collzone") || line.ToLower().Contains("collision_zone")) && (line.ToLower().Contains("release") || line.ToLower().Contains("clear")))
                    {
                        int number = int.Parse(numberRegex.Match(line).ToString());
                        if (collisionsAndRobots.Keys.Contains(number))
                        {
                            string robotCurrentRobot = FindRobotName(file.Key);
                            string robotToAdd = string.Empty;
                            if (collisionsAndRobots[number].Robot1 == robotCurrentRobot)
                                robotToAdd = collisionsAndRobots[number].Robot2;
                            else
                                robotToAdd = collisionsAndRobots[number].Robot1;
                            line = String.Format("# Destination {1}\r\n# Send coll{0} 0\r\n", number, robotToAdd) + line;
                        }
                    }
                    currentFileContent += line + "\r\n";
                }
                resultFiles.Add(file.Key, currentFileContent);
            }
            return resultFiles;
        }

        private static IDictionary<int, CollisionsItems> AssignCollsToRobots(IDictionary<string, List<int>> collisionsOnRobots)
        {
            IDictionary<int, CollisionsItems> result = new Dictionary<int, CollisionsItems>();
            foreach (var robot in collisionsOnRobots)
            {
                foreach (var coll in robot.Value)
                {
                    if (!result.Keys.Contains(coll))
                        result.Add(coll, new CollisionsItems() { Robot1 = robot.Key });
                    else
                    {
                        if (result[coll].Robot2 == null)
                            result[coll].Robot2 = robot.Key;
                        else
                        {
                            MessageBox.Show(String.Format("Collision {0} is assigned to more than 2 robots. Program will abort", coll), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return null;
                        }
                    }
                }
            }
            IDictionary<int, CollisionsItems> resultFiltered = new Dictionary<int, CollisionsItems>(result);
            foreach (var item in result.Where(x => x.Value.Robot2 == null))
                resultFiltered.Remove(item);
            return resultFiltered;
        }

        private static IDictionary<string, List<int>> FindCollisions(IDictionary<string, string> srcFiles, string contrType)
        {
            IDictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            foreach (var file in srcFiles)
            {
                string robotname = FindRobotName(file.Key);
                List<int> collisions = FindCollisions(file.Value);
                if (!result.Keys.Contains(robotname))
                    result.Add(robotname, collisions);
                else
                    result[robotname] = FilterCollZones(result[robotname], collisions);
            }
            foreach (var item in result)
            {
                signalsToAdd.Add(item.Key, new List<string>());
                item.Value.ForEach(x => signalsToAdd[item.Key].Add("coll" + x));
            }
            return result;
        }

        private static List<int> FilterCollZones(List<int> list, List<int> collisions)
        {
            foreach (int num in collisions)
            {
                if (!list.Contains(num))
                    list.Add(num);
            }
            list.Sort();
            return list;
        }

        private static List<int> FindCollisions(string fileContent)
        {
            Regex regexNr = new Regex(@"(?<=(COLL_SAFETY_|Plc_CollSafetyReq\d*|Plc_CollSafetyClear\d*)[a-zA-Z_]*\s*\(\s*)[0-9]*(?=\s*\))", RegexOptions.IgnoreCase);
            List<int> result = new List<int>();
            StringReader reader = new StringReader(fileContent);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;
                if (regexNr.IsMatch(line))
                {
                    int match = int.Parse(regexNr.Match(line).ToString());
                    if (!result.Contains(match))
                        result.Add(match);
                }
            }
            reader.Close();
            result.Sort();
            return result;
        }

        private static string FindRobotName(string filecontent)
        {
            Regex robotLine = new Regex(@"(?<=\s*;\s*\*\s*Robot\s*\:\s*).*", RegexOptions.IgnoreCase);
            StreamReader reader = new StreamReader(filecontent);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (robotLine.IsMatch(line))
                {
                    reader.Close();
                    return robotLine.Match(line).ToString().Trim();
                }
            }
            reader.Close();
            return string.Empty;
        }

        private static string DetectControllerType(IDictionary<string, string> srcFiles)
        {
            Regex isKRC4 = new Regex(@"plc_collsafetyreq1\s*\(\s*\d+\s*\)", RegexOptions.IgnoreCase);
            Regex isKRC2 = new Regex(@"COLL_SAFETY_REQ\s*\(\s*\d+\s*\)", RegexOptions.IgnoreCase);
            foreach (var file in srcFiles)
            {
                if (isKRC4.IsMatch(file.Value))
                    return "KRC4";
                if (isKRC2.IsMatch(file.Value))
                    return "KRC2_V8";
            }
            return string.Empty;
                

        }

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
