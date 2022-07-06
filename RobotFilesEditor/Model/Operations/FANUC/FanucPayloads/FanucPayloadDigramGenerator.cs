using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Configuration;

namespace RobotFilesEditor.Model.Operations.FANUC.FanucPayloads
{
    public class FanucPayloadDigramGenerator
    {
        private static Excel.Application defaultXlApp;
        private static Excel.Workbook defaultXlWorkbook;
        private static Excel.Worksheet defaultXlWorksheet;

        private static Excel.Application destinationXlApp;
        private static Excel.Workbook destinationXlWorkbook;
        private static Excel.Worksheet destinationXlWorksheet;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public FanucPayloadDigramGenerator()
        {
            var dialog = MessageBox.Show("Select backup file from Fanuc robot.\r\nBackups are in form of zip od directory?\r\nYes - zip\r\nNo - Directory", "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            bool isDir;
            string backupfile = string.Empty;
            if (dialog == MessageBoxResult.Yes)
            {
                backupfile = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "Zip File", filter1: "*.zip");
                isDir = false;
            }
            else
            {
                isDir = true;
                backupfile = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            }
            if (string.IsNullOrEmpty(backupfile))
                return;
            List<RobotWithPayloads> payloads = ReadPayloadsFromBackup(GetFilesToScan(backupfile,isDir));
            Excel.Range defaultRange = GetDefaultRangeExcel();
            WriteExcel(payloads, defaultRange);
        }

        private List<string> GetFilesToScan(string backupfile, bool isDir)
        {
            if (!isDir)
                return new List<string>() { backupfile };
            return Directory.GetFiles(backupfile, "*.zip", SearchOption.AllDirectories).ToList();
        }

        private List<RobotWithPayloads> ReadPayloadsFromBackup(List<string> backupfiles)
        {
            Dialogs.PleaseWait.PleaseWaitDialog dialog = new Dialogs.PleaseWait.PleaseWaitDialog();
            dialog.Show();
            string errorFiles = string.Empty;
            List<RobotWithPayloads> result = new List<RobotWithPayloads>();
            foreach (var file in backupfiles)
            {
                try
                {
                    if (!CommonLibrary.CommonMethods.IsFileLocked(file))
                    {
                        using (FileStream zipToOpen = new FileStream(file, FileMode.Open))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                            {
                                List<FanucPayload> payloads = new List<FanucPayload>();
                                var mainEntry = archive.Entries.SingleOrDefault(x => x.FullName.ToLower().Contains("workbook.xvr"));
                                if (mainEntry != null)
                                {
                                    StreamReader reader = new StreamReader(mainEntry.Open());
                                    string workbookContent = reader.ReadToEnd();
                                    reader.Close();
                                    XDocument docu = XDocument.Parse(workbookContent);
                                    string robotName = docu.Element("XMLVAR").Elements("PROG").Single(x => x.Attribute("name").Value == "*SYSTEM*").Elements("VAR").Single(x => x.Attribute("name").Value == "$ROBOT_NAME").Value;
                                    string robotType = docu.Element("XMLVAR").Elements("PROG").Single(x => x.Attribute("name").Value == "*SYSTEM*").Elements("VAR").Single(x => x.Attribute("name").Value == "$SCR_GRP[1].$ROBOT_ID").Value;
                                    var payloadsXML = docu.Element("XMLVAR").Elements("PROG").Single(x => x.Attribute("name").Value == "*SYSTEM*").Elements("VAR").Single(x => x.Attribute("name").Value == "$PLST_GRP1").Elements("ARRAY");
                                    foreach (var payload in payloadsXML.Where(x => x.Elements("FIELD").Single(y => y.Attribute("name").Value == "$COMMENT").Value != "********"))
                                    {
                                        string name = payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$COMMENT").Value;
                                        double mass = Math.Round(double.Parse(payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PAYLOAD").Value, CultureInfo.InvariantCulture), 2);
                                        double xpos = Math.Round(double.Parse(payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PAYLOAD_X").Value, CultureInfo.InvariantCulture), 2);
                                        double ypos = Math.Round(double.Parse(payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PAYLOAD_Y").Value, CultureInfo.InvariantCulture), 2);
                                        double zpos = Math.Round(double.Parse(payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PAYLOAD_Z").Value, CultureInfo.InvariantCulture), 2);
                                        double ix_pos = double.Parse(payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PAYLOAD_IX").Value, CultureInfo.InvariantCulture);
                                        double iy_pos = double.Parse(payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PAYLOAD_IY").Value, CultureInfo.InvariantCulture);
                                        double iz_pos = double.Parse(payload.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PAYLOAD_IZ").Value, CultureInfo.InvariantCulture);

                                        payloads.Add(new FanucPayload(name, mass, xpos, ypos, zpos, ix_pos, iy_pos, iz_pos));
                                    }
                                    result.Add(new RobotWithPayloads(robotName, file, robotType, 10, payloads));
                                }
                                archive.Dispose();
                            }
                            zipToOpen.Close();
                        }
                        dialog.Close();
                    }
                    else
                    {
                        MessageBox.Show("File " + Path.GetFileName(file) + " is blocked by another process! Close file and try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Close();
                    //MessageBox.Show("Error while reading file : " + file, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    errorFiles += file + "\r\n";
                }
            }
            if (!string.IsNullOrEmpty(errorFiles))
                MessageBox.Show("Error while reading files:\r\n" + errorFiles, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return result;
        }

        private Excel.Range GetDefaultRangeExcel()
        {
            Excel.Range range;
            try
            {
                var projectDir = System.AppDomain.CurrentDomain.BaseDirectory;
                var xlsFile = Directory.GetFiles( Directory.GetDirectories(projectDir).First(x => Path.GetFileName(x).ToLower().Contains("resources"))).Single(x => Path.GetFileName(x).ToLower() == "fanuc_payload_excel.xlsx");
                defaultXlApp = new Excel.Application();
                defaultXlWorkbook = defaultXlApp.Workbooks.Open(xlsFile);
                defaultXlWorksheet = (Excel.Worksheet)defaultXlWorkbook.Worksheets.get_Item(1);
                range = defaultXlWorksheet.Range["A1", "E11"];
                //CleanUpExcel("default");
            }
            catch (Exception ex)
            {
                range = null;
                CleanUpExcel("default");
            }
            return range;
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
            if (type == "default")
            {
                if (defaultXlApp != null)
                {
                    int hWndDest = defaultXlApp.Application.Hwnd;

                    uint processID;

                    GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                    Process.GetProcessById((int)processID).Kill();
                }
                defaultXlWorksheet = null;
                defaultXlWorkbook = null;
                defaultXlApp = null;
            }
        }

        private void WriteExcel(List<RobotWithPayloads> robotsWithPayloads, Excel.Range defaultRange)
        {
            object misValue = System.Reflection.Missing.Value;
            var clipboardContent = CopyFromClipBoard();
            Dialogs.PleaseWait.PleaseWaitDialog dialog = new Dialogs.PleaseWait.PleaseWaitDialog();
            dialog.Show();

            try
            {
                defaultRange.Copy();
                destinationXlApp = new Excel.Application();
                destinationXlWorkbook = destinationXlApp.Workbooks.Add();

                foreach (var robot in robotsWithPayloads.OrderByDescending(x => x.Name))
                {
                    destinationXlWorksheet = (Excel.Worksheet)destinationXlWorkbook.Worksheets.Add();
                    destinationXlWorksheet.Name = robot.Name;
                    int counter = 0;
                    foreach (var payload in robot.Payloads)
                    {
                        int tempRow = counter == 0 ? 1 : (counter * 12 + 2);
                        tempRow = counter == 1 ? tempRow - 1 : tempRow;
                        Excel.Range currentRange = destinationXlWorksheet.get_Range("A" + tempRow.ToString());
                        currentRange.PasteSpecial(Excel.XlPasteType.xlPasteValues);

                        destinationXlWorksheet.Cells[tempRow, 2] = robot.RobotType;
                        destinationXlWorksheet.Cells[(tempRow + 2), 2] = payload.Mass.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[(tempRow + 3), 2] = robot.ArmLoad.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[(tempRow + 5), 2] = payload.PosX.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[(tempRow + 6), 2] = payload.PosY.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[(tempRow + 7), 2] = payload.PosZ.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[(tempRow + 8), 2] = payload.Ix_SI.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[(tempRow + 9), 2] = payload.Iy_SI.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[(tempRow + 10), 2] = payload.Iz_SI.ToString(CultureInfo.InvariantCulture);

                        destinationXlWorksheet.Cells[tempRow, 5] = robot.Name;
                        destinationXlWorksheet.Cells[(tempRow + 1), 5] = payload.Name;
                        destinationXlWorksheet.Cells[(tempRow + 2), 5] = robot.Station;
                        destinationXlWorksheet.Cells[(tempRow + 3), 5] = ConfigurationManager.AppSettings["Ersteller"];

                        counter++;
                    }
                    Excel.Range usedRange = destinationXlWorksheet.UsedRange;
                    usedRange.Columns.AutoFit();
                }

                //ADD SUMMARY Sheet
                destinationXlWorksheet = (Excel.Worksheet)destinationXlWorkbook.Worksheets.Add();
                destinationXlWorksheet.Name = "Summary";
                int rowcounter = 2;
                destinationXlWorksheet.Cells[1, 1] = "#";
                destinationXlWorksheet.Cells[1, 2] = "Robot E#";
                destinationXlWorksheet.Cells[1, 3] = "Tool #";
                destinationXlWorksheet.Cells[1, 4] = "Station #";
                destinationXlWorksheet.Cells[1, 5] = "Robot Type";
                destinationXlWorksheet.Cells[1, 6] = "Analysis Type";
                destinationXlWorksheet.Cells[1, 7] = "Engineer";
                destinationXlWorksheet.Cells[1, 8] = "Payload(kg)";
                destinationXlWorksheet.Cells[1, 9] = "J3 Arm Load(kg)";
                destinationXlWorksheet.Cells[1, 10] = "J3 Casing Load(kg)";
                destinationXlWorksheet.Cells[1, 15] = "COMMENTS";
                destinationXlWorksheet.Cells[1, 16] = "Robot code";
                destinationXlWorksheet.Cells[1, 17] = "Payload code";
                destinationXlWorksheet.Cells[1, 18] = "X/M4";
                destinationXlWorksheet.Cells[1, 19] = "Y/M5";
                destinationXlWorksheet.Cells[1, 20] = "Z/M6";
                destinationXlWorksheet.Cells[1, 21] = "Ix/I4";
                destinationXlWorksheet.Cells[1, 22] = "Iy/I5";
                destinationXlWorksheet.Cells[1, 23] = "Iz/I6";
                Excel.Range aRange = destinationXlWorksheet.UsedRange;
                aRange.Font.Bold = true;

                foreach (var robot in robotsWithPayloads)
                {

                    foreach (var payload in robot.Payloads)
                    {
                        destinationXlWorksheet.Cells[rowcounter, 1] = rowcounter-1;
                        destinationXlWorksheet.Cells[rowcounter, 2] = robot.Name;
                        destinationXlWorksheet.Cells[rowcounter, 3] = payload.Name;
                        destinationXlWorksheet.Cells[rowcounter, 4] = robot.Station;
                        destinationXlWorksheet.Cells[rowcounter, 5] = robot.RobotType;
                        destinationXlWorksheet.Cells[rowcounter, 6] = "CofG - PID";
                        destinationXlWorksheet.Cells[rowcounter, 7] = ConfigurationManager.AppSettings["Ersteller"];
                        destinationXlWorksheet.Cells[rowcounter, 8] = payload.Mass.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[rowcounter, 9] = robot.ArmLoad.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[rowcounter, 10] = "0";
                        destinationXlWorksheet.Cells[rowcounter, 15] = payload.Name; 
                        destinationXlWorksheet.Cells[rowcounter, 16] = GetRobotTypeCode(robot.RobotType);
                        destinationXlWorksheet.Cells[rowcounter, 17] = "2";
                        destinationXlWorksheet.Cells[rowcounter, 18] = payload.PosX.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[rowcounter, 19] = payload.PosY.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[rowcounter, 20] = payload.PosZ.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[rowcounter, 21] = payload.Ix_SI.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[rowcounter, 22] = payload.Iy_SI.ToString(CultureInfo.InvariantCulture);
                        destinationXlWorksheet.Cells[rowcounter, 23] = payload.Iy_SI.ToString(CultureInfo.InvariantCulture);
                        rowcounter++;
                    }
                }
                aRange = destinationXlWorksheet.UsedRange;
                aRange.Columns.AutoFit();
                Excel.Borders border = aRange.Borders;
                border.LineStyle = Excel.XlLineStyle.xlContinuous;
                //END Summary sheet

                dialog.Close();
                MessageBox.Show("Select destination file", "Select", MessageBoxButton.OK, MessageBoxImage.Information);
                string savePath = CommonLibrary.CommonMethods.SelectDirOrFile(false, "xlsx file", "*.xlsx");
                if (string.IsNullOrEmpty(savePath))
                    return;
                if (string.IsNullOrEmpty(Path.GetExtension(savePath)))
                    savePath += ".xlsx";
                destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                destinationXlWorkbook.Close(true, misValue, misValue);
                Clipboard.Clear();
                MessageBoxResult dialog2 = MessageBox.Show("Successfuly saved at " + savePath+"\r\nOpen file?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog2 == MessageBoxResult.Yes)
                    Process.Start(savePath);

            }
            catch (Exception ex)
            {
                dialog.Close();
                MessageBox.Show("File was not created.\r\nMessage: " + ex.Message +"\r\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetClipBoard(clipboardContent);
                CleanUpExcel("destination");
                CleanUpExcel("default");
            }
        }

        private string GetRobotTypeCode(string robotType)
        {
            switch (robotType)
            {
                case "R-2000iC/210F":
                    {
                        return "10141";
                    }
                case "R-2000iC/210L":
                    {
                        return "10220";
                    }
                case "M-900iB/360":
                    {
                        return "10164";
                    }
            }
            return "Code not found";
        }

        private dynamic CopyFromClipBoard()
        {
            dynamic result = null;
            if (Clipboard.ContainsAudio())
                result = Clipboard.GetAudioStream();
            if (Clipboard.ContainsFileDropList())
                result = Clipboard.GetFileDropList();
            if (Clipboard.ContainsImage())
                result = Clipboard.GetImage();
            if (Clipboard.ContainsText())
                result = Clipboard.GetText();
            return result; 
        }

        private void SetClipBoard(dynamic oldClipboard)
        {
            if (oldClipboard == null)
                return;
            if (oldClipboard is System.IO.Stream)
                Clipboard.SetAudio((System.IO.Stream)oldClipboard);
            if (oldClipboard is System.Collections.Specialized.StringCollection)
                Clipboard.SetFileDropList((System.Collections.Specialized.StringCollection)oldClipboard);
            if (oldClipboard is System.Windows.Media.Imaging.BitmapSource)
                Clipboard.SetImage((System.Windows.Media.Imaging.BitmapSource)oldClipboard);
            if (oldClipboard is string)
                Clipboard.SetText((string)oldClipboard);

        }
    }
}
