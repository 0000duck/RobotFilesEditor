using Microsoft.WindowsAPICodePack.Dialogs;
using RobotFilesEditor.Model.DataInformations;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace RobotFilesEditor.Model.Operations
{
    public static class StoppingDistanceMethods
    {
        private static string savePath;
        private static Excel.Application destinationXlApp;
        private static Excel.Workbook destinationXlWorkbook;
        private static Excel.Worksheet destinationXlWorksheet;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        internal static void Execute()
        {
            string selectedFile = SelectFile();
            if (string.IsNullOrEmpty(selectedFile))
                return;
            SST foundPoints = GetPointsFromDatFile(selectedFile);
            if (VerifyFoundPoints(foundPoints))
            {
                List<E6AxisPoint> calculatedDiff = CalculatDiff(foundPoints);
                CreateResultExcel(foundPoints, calculatedDiff, Path.GetFileName(selectedFile));
            }
        }

        private static string SelectFile()
        {
            savePath = "";
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.Filters.Add(new CommonFileDialogFilter("Dat file (*.dat)", ".dat"));
            IDictionary<int, string> result = new Dictionary<int, string>();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                savePath = (dialog.FileName).Replace(".dat", ".xls");
                return dialog.FileName;
            }
            else
                return "";
        }


        private static SST GetPointsFromDatFile(string selectedFile)
        {
            List<E6AxisPoint> t1 = new List<E6AxisPoint>();
            List<E6AxisPoint> t2 = new List<E6AxisPoint>();
            StreamReader reader = new StreamReader(selectedFile);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("e6axis"))
                {
                    E6AxisPoint foundPoint = GetPointFromString(line);
                    if (foundPoint.Name.ToLower().Contains("sst") && foundPoint.Name.ToLower().Contains("t1"))
                        t1.Add(foundPoint);
                    else if (foundPoint.Name.ToLower().Contains("sst") && foundPoint.Name.ToLower().Contains("t2"))
                        t2.Add(foundPoint);                      
                }
            }
            reader.Close();

            return new SST(t1,t2);
        }

        private static E6AxisPoint GetPointFromString(string line)
        {
            Regex regexName = new Regex(@"(?<=E6AXIS\s+)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
            string name = regexName.Match(line).ToString();
            Regex regexA1 = new Regex(@"(?<=A1\s+)[a-zA-Z0-9_\-\.]*", RegexOptions.IgnoreCase);
            double a1 = double.Parse(regexA1.Match(line).ToString().Replace(".",","));
            Regex regexA2 = new Regex(@"(?<=A2\s+)[a-zA-Z0-9_\-\.]*", RegexOptions.IgnoreCase);
            double a2 = double.Parse(regexA2.Match(line).ToString().Replace(".", ","));
            Regex regexA3 = new Regex(@"(?<=A3\s+)[a-zA-Z0-9_\-\.]*", RegexOptions.IgnoreCase);
            double a3 = double.Parse(regexA3.Match(line).ToString().Replace(".", ","));
            Regex regexA4 = new Regex(@"(?<=A4\s+)[a-zA-Z0-9_\-\.]*", RegexOptions.IgnoreCase);
            double a4 = double.Parse(regexA4.Match(line).ToString().Replace(".", ","));
            Regex regexA5 = new Regex(@"(?<=A5\s+)[a-zA-Z0-9_\-\.]*", RegexOptions.IgnoreCase);
            double a5 = double.Parse(regexA5.Match(line).ToString().Replace(".", ","));
            Regex regexA6 = new Regex(@"(?<=A6\s+)[a-zA-Z0-9_\-\.]*", RegexOptions.IgnoreCase);
            double a6 = double.Parse(regexA6.Match(line).ToString().Replace(".", ","));
            Regex regexE1 = new Regex(@"(?<=E1\s+)[a-zA-Z0-9_\-\.]*", RegexOptions.IgnoreCase);
            double e1 = double.Parse(regexE1.Match(line).ToString().Replace(".", ","));

            return new E6AxisPoint(name, a1, a2, a3, a4, a5, a6, e1);
        }

        private static bool VerifyFoundPoints(SST foundPoints)
        {
            if (foundPoints.T1.Count == foundPoints.T2.Count)
                return true;
            else
            {
                MessageBox.Show("Number of T1 and T2 points not equal, program will abort!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static List<E6AxisPoint> CalculatDiff(SST foundPoints)
        {
            List<E6AxisPoint> result = new List<E6AxisPoint>();
            for (int i = 0; i < foundPoints.T1.Count; i++)
            {
                E6AxisPoint currentDiff = new E6AxisPoint("Difference " +i ,Math.Abs(foundPoints.T1[i].A1 - foundPoints.T2[i].A1), Math.Abs(foundPoints.T1[i].A2 - foundPoints.T2[i].A2), Math.Abs(foundPoints.T1[i].A3 - foundPoints.T2[i].A3), Math.Abs(foundPoints.T1[i].A4 - foundPoints.T2[i].A4), Math.Abs(foundPoints.T1[i].A5 - foundPoints.T2[i].A5), Math.Abs(foundPoints.T1[i].A6 - foundPoints.T2[i].A6), Math.Abs(foundPoints.T1[i].E1 - foundPoints.T2[i].E1));
                result.Add(currentDiff);
            }
            return result;
        }

        private static void CreateResultExcel(SST foundPoints, List<E6AxisPoint> calculatedDiff, string filename)
        {
            int offset = 13;
            destinationXlApp = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;

            destinationXlWorkbook = destinationXlApp.Workbooks.Add(misValue);
            destinationXlWorksheet = (Excel.Worksheet)destinationXlWorkbook.Worksheets.get_Item(1);

            //destinationXlWorksheet.Cells[1, 1] = "ID";
            //destinationXlWorksheet.Cells[1, 2] = "Name";
            //destinationXlWorksheet.Cells[2, 1] = "1";
            //destinationXlWorksheet.Cells[2, 2] = "One";
            //destinationXlWorksheet.Cells[3, 1] = "2";
            //destinationXlWorksheet.Cells[3, 2] = "Two";

            destinationXlWorksheet.Cells[1, 1] = filename;
            destinationXlWorksheet.Cells[1, 1].Font.Bold = true;
            for (int i = 0; i < foundPoints.T1.Count; i++)
            {
                destinationXlWorksheet.Cells[2 + i * offset, 1] = "Path number " + (i+1);
                destinationXlWorksheet.Cells[2 + i * offset, 1].Font.Bold = true;
                destinationXlWorksheet.Cells[3 + i*offset , 1] = "Position in T1";
                destinationXlWorksheet.Cells[3 + i * offset, 1].Font.Bold = true;
                destinationXlWorksheet.Cells[4 + i * offset, 1] = "A1";
                destinationXlWorksheet.Cells[4 + i * offset, 1].EntireRow.Font.Bold = true;
                destinationXlWorksheet.Cells[4 + i * offset, 2] = "A2";
                destinationXlWorksheet.Cells[4 + i * offset, 3] = "A3";
                destinationXlWorksheet.Cells[4 + i * offset, 4] = "A4";
                destinationXlWorksheet.Cells[4 + i * offset, 5] = "A5";
                destinationXlWorksheet.Cells[4 + i * offset, 6] = "A6";
                destinationXlWorksheet.Cells[4 + i * offset, 7] = "E1";
                destinationXlWorksheet.Cells[5 + i * offset, 1] = foundPoints.T1[i].A1;
                destinationXlWorksheet.Cells[5 + i * offset, 2] = foundPoints.T1[i].A2;
                destinationXlWorksheet.Cells[5 + i * offset, 3] = foundPoints.T1[i].A3;
                destinationXlWorksheet.Cells[5 + i * offset, 4] = foundPoints.T1[i].A4;
                destinationXlWorksheet.Cells[5 + i * offset, 5] = foundPoints.T1[i].A5;
                destinationXlWorksheet.Cells[5 + i * offset, 6] = foundPoints.T1[i].A6;
                destinationXlWorksheet.Cells[5 + i * offset, 7] = foundPoints.T1[i].E1;
                destinationXlWorksheet.Cells[6 + i * offset, 1] = "Position in T2";
                destinationXlWorksheet.Cells[6 + i * offset, 1].Font.Bold = true;
                destinationXlWorksheet.Cells[7 + i * offset, 1] = "A1";
                destinationXlWorksheet.Cells[7 + i * offset, 1].EntireRow.Font.Bold = true;
                destinationXlWorksheet.Cells[7 + i * offset, 2] = "A2";
                destinationXlWorksheet.Cells[7 + i * offset, 3] = "A3";
                destinationXlWorksheet.Cells[7 + i * offset, 4] = "A4";
                destinationXlWorksheet.Cells[7 + i * offset, 5] = "A5";
                destinationXlWorksheet.Cells[7 + i * offset, 6] = "A6";
                destinationXlWorksheet.Cells[7 + i * offset, 7] = "E1";
                destinationXlWorksheet.Cells[8 + i * offset, 1] = foundPoints.T2[i].A1;
                destinationXlWorksheet.Cells[8 + i * offset, 2] = foundPoints.T2[i].A2;
                destinationXlWorksheet.Cells[8 + i * offset, 3] = foundPoints.T2[i].A3;
                destinationXlWorksheet.Cells[8 + i * offset, 4] = foundPoints.T2[i].A4;
                destinationXlWorksheet.Cells[8 + i * offset, 5] = foundPoints.T2[i].A5;
                destinationXlWorksheet.Cells[8 + i * offset, 6] = foundPoints.T2[i].A6;
                destinationXlWorksheet.Cells[8 + i * offset, 7] = foundPoints.T2[i].E1;
                destinationXlWorksheet.Cells[9 + i * offset, 1].Font.Bold = true;
                destinationXlWorksheet.Cells[9 + i * offset, 1] = "Difference between T1 and T2";
                destinationXlWorksheet.Cells[10 + i * offset, 1] = "A1";
                destinationXlWorksheet.Cells[10 + i * offset, 1].EntireRow.Font.Bold = true;
                destinationXlWorksheet.Cells[10 + i * offset, 2] = "A2";
                destinationXlWorksheet.Cells[10 + i * offset, 3] = "A3";
                destinationXlWorksheet.Cells[10 + i * offset, 4] = "A4";
                destinationXlWorksheet.Cells[10 + i * offset, 5] = "A5";
                destinationXlWorksheet.Cells[10 + i * offset, 6] = "A6";
                destinationXlWorksheet.Cells[10 + i * offset, 7] = "E1";
                destinationXlWorksheet.Cells[11 + i * offset, 1] = calculatedDiff[i].A1;
                destinationXlWorksheet.Cells[11 + i * offset, 2] = calculatedDiff[i].A2;
                destinationXlWorksheet.Cells[11 + i * offset, 3] = calculatedDiff[i].A3;
                destinationXlWorksheet.Cells[11 + i * offset, 4] = calculatedDiff[i].A4;
                destinationXlWorksheet.Cells[11 + i * offset, 5] = calculatedDiff[i].A5;
                destinationXlWorksheet.Cells[11 + i * offset, 6] = calculatedDiff[i].A6;
                destinationXlWorksheet.Cells[11 + i * offset, 7] = calculatedDiff[i].E1;
            }
            try
            {
                destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                destinationXlWorkbook.Close(true, misValue, misValue);
                CleanUpExcel();
                MessageBox.Show("Succesfully saved at " + savePath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                CleanUpExcel();
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
