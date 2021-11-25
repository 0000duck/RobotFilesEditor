using RobotFilesEditor.Model.DataInformations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace RobotFilesEditor.Model.Operations
{

    public static class CompareBoschMethods
    {
        private static Excel.Application sourceXlApp;
        private static Excel.Workbooks sourceXlWorkbooks;
        private static Excel.Workbook sourceXlWorkbook;
        private static Excel.Application destinationXlApp;
        private static Excel.Workbook destinationXlWorkbook;
        private static Excel.Worksheet destinationXlWorksheet;
        static int correctTypIDColumn = 5;
        static int typIdListFirstRow = 2;
        static int spotNumSource = 3;
        static int robotnamecolumn = 1;

        static string savePath = "";

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        internal static void Execute()
        {
            IDictionary<int, WeldpointBMW> pointsInBosch = ReadSpotsMethods.GetPoitsFromBosch();
            IDictionary<int, WeldpointBMW> pointsInList = GetPointsFromList();
            if (pointsInBosch == null || pointsInList == null)
                return;
            WriteXLS(pointsInBosch, pointsInList);

        }

        private static void WriteXLS(IDictionary<int, WeldpointBMW> pointsInBosch, IDictionary<int, WeldpointBMW> pointsInList)
        {
            int rowsCounter = 2;
            destinationXlApp = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;

            destinationXlWorkbook = destinationXlApp.Workbooks.Add(misValue);
            destinationXlWorksheet = (Excel.Worksheet)destinationXlWorkbook.Worksheets.get_Item(1);

            destinationXlWorksheet.Cells[1, 1] = "Robot";
            destinationXlWorksheet.Cells[1, 1].Font.Bold = true;
            destinationXlWorksheet.Cells[1, 2] = "Spot Num";
            destinationXlWorksheet.Cells[1, 2].Font.Bold = true;
            destinationXlWorksheet.Cells[1, 3] = "TypID Bosch";
            destinationXlWorksheet.Cells[1, 3].Font.Bold = true;
            destinationXlWorksheet.Cells[1, 4] = "TypID List";
            destinationXlWorksheet.Cells[1, 4].Font.Bold = true;
            destinationXlWorksheet.Cells[1, 5] = "Comment";
            destinationXlWorksheet.Cells[1, 5].Font.Bold = true;

            foreach (var pointInBosch in pointsInBosch)
            {
                if (pointsInList.Keys.Contains(pointInBosch.Key))
                {
                    destinationXlWorksheet.Cells[rowsCounter, 1] = pointsInList[pointInBosch.Key].Robot;
                    destinationXlWorksheet.Cells[rowsCounter, 2] = pointInBosch.Key;
                    destinationXlWorksheet.Cells[rowsCounter, 3] = ReadSpotsMethods.GetTypIDsFromBosch(pointInBosch.Value.TypIDsInBosch);
                    destinationXlWorksheet.Cells[rowsCounter, 4] = pointsInList[pointInBosch.Key].TypId;

                    if (pointsInList[pointInBosch.Key].TypId.ToString() != ReadSpotsMethods.GetTypIDsFromBosch(pointInBosch.Value.TypIDsInBosch))
                    {
                        destinationXlWorksheet.Cells[rowsCounter, 5] = "TypID in Bosch and list are different!";
                        Excel.Range range = destinationXlWorksheet.Range[CommonMethods.ToExcelCoordinates(5 +"," + rowsCounter)];
                        range.Interior.ColorIndex = 3;
                    }
                    rowsCounter++;
                }
            }
            Excel.Range usedRange = destinationXlWorksheet.UsedRange;
            usedRange.Columns.AutoFit();
            try
            {
                //destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                if (!File.Exists(savePath) || !CommonLibrary.CommonMethods.IsFileLocked(savePath))
                {
                    destinationXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    MessageBox.Show("Succesfully saved at " + savePath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    destinationXlWorkbook.Close(true, misValue, misValue);
                }
                else
                    MessageBox.Show("File " + savePath + " is used by another process. Close the file and try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CleanUpExcel("destination");
            }
            catch
            {
                CleanUpExcel("destination");
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

            private static IDictionary<int, WeldpointBMW> GetPointsFromList()
        {
            IDictionary<int, WeldpointBMW> result = new Dictionary<int, WeldpointBMW>();
            string selectedfile = CommonMethods.SelectDirOrFile(false, "TargetFile", "*.xlsm; *.xlsx");
            if (string.IsNullOrEmpty(selectedfile))
                return null;
            savePath = Path.GetDirectoryName(selectedfile) + "\\TypIDComparison.xlsx";
            sourceXlApp = new Excel.Application();
            sourceXlWorkbooks = sourceXlApp.Workbooks;
            sourceXlWorkbook = sourceXlWorkbooks.Open(selectedfile);
            Excel._Worksheet sourcexlWorksheet = sourceXlWorkbook.Sheets[1];
            Excel.Range sourcexlRange = sourcexlWorksheet.UsedRange;
            for (int i = typIdListFirstRow; i <= sourcexlRange.Rows.Count; i++)
            {
                try
                {
                    if (sourcexlRange.Cells[i, spotNumSource].FormulaLocal != "")
                    {
                        int currentSpotNum = int.Parse(sourcexlRange.Cells[i, spotNumSource].FormulaLocal);
                        int currenttypID = int.Parse(sourcexlRange.Cells[i, correctTypIDColumn].FormulaLocal);
                        result.Add(currentSpotNum, new WeldpointBMW(sourcexlRange.Cells[i, robotnamecolumn].FormulaLocal, "", "", currentSpotNum, currenttypID, 0, 0, 0,0,0,0,"",""));
                    }
                    else
                        break;
                }
                catch
                { }
            }
            CleanUpExcel("source");
            if (result.Count == 0)
                return null;
            return result;
        }

        private static void CleanUpExcel(string type)
        {
            if (type == "source")
            {
                if (sourceXlApp != null)
                {
                    int hWndDest = sourceXlApp.Application.Hwnd;

                    uint processID;

                    GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                    Process.GetProcessById((int)processID).Kill();
                }
                sourceXlWorkbook = null;
                sourceXlWorkbooks = null;
                sourceXlApp = null;
            }
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
        }
    }
}
