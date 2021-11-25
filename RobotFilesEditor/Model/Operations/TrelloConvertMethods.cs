using CommonLibrary;
using Newtonsoft.Json;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace RobotFilesEditor.Model.Operations
{
    public static class TrelloConvertMethods
    {
        private static string[] removeableTabs = { "template", "add ons" };
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private static Excel.Application oXL;
        private static Excel.Workbooks oWBs;
        private static Excel.Workbook oWB;
        private static Excel.Sheets sheets;
        private static Excel.Worksheet oSheet;
        private static int counter;
        private static string logbook;

        internal static void Execute()
        {
            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                logbook = string.Empty;
                var robotName = string.Empty;
                var idListy = string.Empty;
                counter = 0;
                string file = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1: "*.json", filter1Descr: "JSON");
                if (string.IsNullOrEmpty(file))
                    return;
                string filecontent = File.ReadAllText(file);
                XNode node = JsonConvert.DeserializeXNode(filecontent, "Root");
                //File.WriteAllText("D:\\temp\\temp.xml", node.ToString());
                XElement currentXML = ToXELement(node);
                IEnumerable<XElement> checklists = currentXML.Elements("checklists"); // checklista wewenątrz kazdego robota
                IEnumerable<XElement> cards = currentXML.Elements("cards"); // kazdy robot
                IEnumerable<XElement> lists = currentXML.Elements("lists"); // kazda linia
                IEnumerable<XElement> name = currentXML.Elements("name");

                Dictionary<string, SortedDictionary<string, SortedDictionary<string, List<TrelloReportDatas>>>> data = new Dictionary<string, SortedDictionary<string, SortedDictionary<string, List<TrelloReportDatas>>>>();  // słownik Linia słownik Robot i dane
                Dictionary<string, string> lineNameAndId = new Dictionary<string, string>();
                Dictionary<string, KeyValuePair<string, string>> robotNameAndId = new Dictionary<string, KeyValuePair<string, string>>(); // idCard, idListy, robotname                

                foreach (var line in lists)
                {
                    var lineName = line.Element("name").FirstNode.ToString();
                    var isClosed = line.Element("closed").FirstNode.ToString();
                    data.Add(lineName, new SortedDictionary<string, SortedDictionary<string, List<TrelloReportDatas>>>());
                    if (!lineNameAndId.Keys.Contains(line.Element("id").FirstNode.ToString()) && isClosed.Trim().ToLower() != "true")
                        lineNameAndId.Add(line.Element("id").FirstNode.ToString(), lineName);
                }
                foreach (var robot in cards)
                {
                    if (robot.Element("closed").FirstNode.ToString() == "false")
                    {
                        robotName = robot.Element("name").FirstNode.ToString();
                        var listID = robot.Element("idList").FirstNode.ToString();
                        if (!robotNameAndId.Keys.Contains(robot.Element("id").FirstNode.ToString()))
                            robotNameAndId.Add(robot.Element("id").FirstNode.ToString(), new KeyValuePair<string, string>(listID, robotName));
                        if (lineNameAndId.Keys.Contains(listID))
                        {
                            if (!data[lineNameAndId[listID]].Keys.Contains(robotName))
                                data[lineNameAndId[listID]].Add(robotName, new SortedDictionary<string, List<TrelloReportDatas>>());
                        }
                    }
                }

                foreach (var checklist in checklists)
                {
                    counter++;                    
                    List<TrelloReportDatas> currentList = new List<TrelloReportDatas>();
                    //Dictionary<string,TrelloReportDatas> currentList = new Dictionary<string,TrelloReportDatas>();
                    int pos = int.Parse(checklist.Element("pos").FirstNode.ToString().Trim());
                    string idCard = checklist.Element("idCard").FirstNode.ToString();
                    string checklistName = checklist.Element("name").FirstNode.ToString();
                    IEnumerable<XElement> tasks = checklist.Elements("checkItems");
                    foreach (var task in tasks)
                    {
                        currentList = new List<TrelloReportDatas>();
                        bool completed = false;
                        var taskName = task.Element("name").FirstNode.ToString();
                        if (task.Element("state").FirstNode.ToString().Trim().ToLower() == "complete")
                            completed = true;
                        TrelloReportDatas currentListTask = new TrelloReportDatas();
                        currentListTask.Complete = completed;
                        currentListTask.Task = taskName;
                        currentListTask.WhoCompleted = "";
                        if (robotNameAndId.Keys.Contains(idCard))
                        {
                            idListy = robotNameAndId[idCard].Key;
                            robotName = robotNameAndId[idCard].Value;
                            if (robotName == "ST080_IR002")
                            { }
                            if (lineNameAndId.Keys.Contains(idListy))
                            {
                                if (!data[lineNameAndId[idListy]][robotName].Keys.Contains(pos + "_position_" + checklistName))
                                    data[lineNameAndId[idListy]][robotName].Add(pos + "_position_" + checklistName, new List<TrelloReportDatas>());

                                data[lineNameAndId[idListy]][robotName][pos + "_position_" + checklistName].Add(currentListTask);
                            }
                            else
                            {
                                logbook += string.Format("Task named \"{0}\" cannot be assigned to any robot\r\n", task.Element("name").FirstNode.ToString());
                            }
                        }                      
                    }
                }

                Dictionary<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>> dataCleared = SortAndClear(data);

                if (!string.IsNullOrEmpty(logbook))
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                    CommonLibrary.CommonMethods.CreateLogFile(logbook, "\\logTrello.txt");
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                }
                WriteExcel(dataCleared);
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static Dictionary<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>> SortAndClear(Dictionary<string, SortedDictionary<string, SortedDictionary<string, List<TrelloReportDatas>>>> data)
        {
            Regex replacementRegex = new Regex(@"^\d+_position_", RegexOptions.IgnoreCase);
            var result = new Dictionary<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>>();
            foreach (var list in data)
            {
                result.Add(list.Key, new SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>());
                foreach (var robot in list.Value)
                {
                    Dictionary<string, List<TrelloReportDatas>> clearedList = new Dictionary<string, List<TrelloReportDatas>>();
                    foreach (var phase in robot.Value)
                    {
                        string phaseNameCleared = replacementRegex.Replace(phase.Key, "");
                        clearedList.Add(phaseNameCleared, phase.Value);
                    }
                    result[list.Key].Add(robot.Key, clearedList);
                }
            }
            return result;
        }

        private static void WriteExcel(Dictionary<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>> data)
        {
            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                int lastRowNum = 0;
                data = FilterLines(data);
                var reversedData = data.Reverse();
                int iteration = 1;
                oXL = new Excel.Application();
                oXL.DisplayAlerts = false;
                oWBs = oXL.Workbooks;
                oWB = oWBs.Add("");
                sheets = oWB.Sheets;
                oSheet = sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;

                if (data == null)
                    return;
                Excel.Range aRange = null;
                foreach (var line in reversedData.Where(x => x.Value.Count > 0))
                {
                    Dictionary<string, int> linesAndProgress = GetLinesProgresses(line);
                    int robotCounter = 0;
                    if (iteration > 1)
                    {
                        oWB.Worksheets.Add(oSheet);
                        oSheet = oWB.ActiveSheet;
                    }
                    oSheet.Name = line.Key;
                    oSheet.Cells[1, 1] = "Robot";
                    oSheet.Cells[1, 2] = "Etap";
                    oSheet.Cells[1, 3] = "Stopień zaawansowania";
                    oSheet.Cells[1, 5] = "Całościowy progress";
                    oSheet.Cells[2, 5] = "Etap";
                    oSheet.Cells[2, 6] = "Stopień zaawansowania";
                    aRange = oSheet.Range["A1", "C1"];
                    aRange.Font.Bold = true;
                    aRange = oSheet.Range["E1", "F1"];
                    aRange.Merge();
                    aRange.Font.Bold = true;
                    aRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    aRange = oSheet.Range["E2", "F2"];
                    aRange.Font.Bold = true;
                    int lineCounter = 0;
                    foreach (var phase in linesAndProgress)
                    {
                        oSheet.Cells[3 + lineCounter, 5] = phase.Key;
                        aRange = oSheet.Range[CommonMethods.ToExcelCoordinates(5+","+ (3 + lineCounter))];
                        aRange.BorderAround2();
                        oSheet.Cells[3 + lineCounter, 6] = phase.Value;
                        aRange = oSheet.Range[CommonMethods.ToExcelCoordinates(6+"," + (3 + lineCounter))];
                        aRange.BorderAround2();
                        lineCounter++;
                    }
                    aRange = oSheet.Range["F3", "F" + (2 + lineCounter).ToString()];
                    Excel.FormatConditions fcs1 = SetConditions(aRange);
                    foreach (var robot in line.Value)
                    {
                        int phaseCounter = 0;
                        foreach (var phase in robot.Value)
                        {
                            oSheet.Cells[2 + robotCounter * robot.Value.Count + phaseCounter, 2] = phase.Key;
                            aRange = oSheet.Range[CommonMethods.ToExcelCoordinates(2 + "," + (2 + robotCounter * robot.Value.Count + phaseCounter))];
                            aRange.BorderAround2();
                            oSheet.Cells[2 + robotCounter * robot.Value.Count + phaseCounter, 3] = GetProgress(phase.Value);
                            aRange = oSheet.Range[CommonMethods.ToExcelCoordinates(3 + "," + (2 + robotCounter * robot.Value.Count + phaseCounter))];
                            aRange.BorderAround2();
                            phaseCounter++;
                        }
                        aRange = oSheet.Range["A" + (2 + robotCounter * robot.Value.Count).ToString(), "A" + (2 + robotCounter * robot.Value.Count + phaseCounter - 1).ToString()];
                        aRange.Merge();
                        aRange.BorderAround2();
                        aRange.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        oSheet.Cells[2 + robotCounter * robot.Value.Count, 1] = robot.Key;
                        lastRowNum = 1 + robotCounter * robot.Value.Count + phaseCounter;
                        robotCounter++;
                    }
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    aRange = oSheet.Range["C2", "C" + lastRowNum.ToString()];
                    Excel.FormatConditions fcs2 = SetConditions(aRange);                        
                    //FormatAsTable(aRange, "Table1", "TableStyleMedium15");
                    iteration++;
                }
                oXL.Visible = true;
                Marshal.FinalReleaseComObject(aRange);
                Marshal.FinalReleaseComObject(oSheet);
                Marshal.FinalReleaseComObject(oWB);
                Marshal.FinalReleaseComObject(oWBs);
                Marshal.FinalReleaseComObject(oXL);
                //CleanUpExcel();
            }
            catch (Exception ex)
            {
                CleanUpExcel();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static Excel.FormatConditions SetConditions(Excel.Range aRange)
        {
            Excel.FormatConditions result =  aRange.FormatConditions;
            Excel.FormatCondition condition1 = (Excel.FormatCondition)result.Add(
                Type: Excel.XlFormatConditionType.xlCellValue,
                Operator: Excel.XlFormatConditionOperator.xlEqual,
                Formula1: 0);
            condition1.Interior.ColorIndex = 3;
            Excel.FormatCondition condition2 = (Excel.FormatCondition)result.Add(
                Type: Excel.XlFormatConditionType.xlCellValue,
                Operator: Excel.XlFormatConditionOperator.xlBetween,
                Formula1: 1,
                Formula2: 29);
            condition2.Interior.ColorIndex = 46;
            Excel.FormatCondition condition3 = (Excel.FormatCondition)result.Add(
                Type: Excel.XlFormatConditionType.xlCellValue,
                Operator: Excel.XlFormatConditionOperator.xlBetween,
                Formula1: 30,
                Formula2: 69);
            condition3.Interior.ColorIndex = 44;
            Excel.FormatCondition condition4 = (Excel.FormatCondition)result.Add(
                Type: Excel.XlFormatConditionType.xlCellValue,
                Operator: Excel.XlFormatConditionOperator.xlBetween,
                Formula1: 70,
                Formula2: 99);
            condition4.Interior.ColorIndex = 43;
            Excel.FormatCondition condition5 = (Excel.FormatCondition)result.Add(
                Type: Excel.XlFormatConditionType.xlCellValue,
                Operator: Excel.XlFormatConditionOperator.xlEqual,
                Formula1: 100);
            condition5.Interior.ColorIndex = 4;

            return result;
        }

        private static Dictionary<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>> FilterLines(Dictionary<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>> data)
        {
            var result = new Dictionary<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>>();
            bool addLine = true;
            foreach (var item in data)
            {
                addLine = true;
                foreach (var removableTab in removeableTabs)
                {
                    if (item.Key.ToLower().Trim() == removableTab)
                        addLine = false;
                }
                if (addLine)
                    result.Add(item.Key,item.Value);
            }
            return result;
        }

        private static Dictionary<string, int> GetLinesProgresses(KeyValuePair<string, SortedDictionary<string, Dictionary<string, List<TrelloReportDatas>>>> line)
        {
            var currentLineProgresses = new Dictionary<string, List<int>>();
            foreach (var robot in line.Value)
            {
                foreach (var phase in robot.Value)
                {
                    if (!currentLineProgresses.Keys.Contains(phase.Key))
                    {
                        currentLineProgresses.Add(phase.Key, new List<int>());
                    }
                    currentLineProgresses[phase.Key].Add(GetProgress(phase.Value));
                }
            }
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var phase in currentLineProgresses)
            {
                result.Add(phase.Key, GetOverallProgress(phase.Value));
            }
            return result;
        }

        private static int GetOverallProgress(List<int> value)
        {
            int sum = value.Sum();
            double hundredProc = 100.0 * value.Count;
            int result = Convert.ToInt32(Math.Ceiling(100.0 * sum / hundredProc));
            if (result > 100)
                result = 100;
            return result;
        }

        private static int GetProgress(List<TrelloReportDatas> value)
        {
            int completedTasks = 0;
            double percentPerTask = Math.Round((100.0 / value.Count),2);
            foreach (var task in value.Where(x=>x.Complete == true))
            {
                completedTasks++;
            }
            int result = Convert.ToInt32(Math.Ceiling(completedTasks * percentPerTask));
            if (result > 100)
                result = 100;
            return result;

        }

        private static XElement ToXELement(XNode node)
        {
            return XElement.Parse(node.ToString());
        }

        private static void CleanUpExcel()
        {
            if (oXL != null)
            {
                int hWndDest = oXL.Application.Hwnd;

                uint processID;

                GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                Process.GetProcessById((int)processID).Kill();
            }
            oWBs = null;
            oWB = null;
            oSheet = null;
        }

        public static void FormatAsTable(Excel.Range SourceRange, string TableName, string TableStyleName)
        {
            SourceRange.Worksheet.ListObjects.Add(Excel.XlListObjectSourceType.xlSrcRange,
            SourceRange, System.Type.Missing, Excel.XlYesNoGuess.xlYes, System.Type.Missing).Name =
                TableName;
            SourceRange.Select();
            SourceRange.Worksheet.ListObjects[TableName].TableStyle = TableStyleName;
        }
    }
}
