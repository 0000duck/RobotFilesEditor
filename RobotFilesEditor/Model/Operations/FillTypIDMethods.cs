using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace RobotFilesEditor.Model.Operations
{
    public static class FillTypIDMethods
    {
        static string spotNumberColumnInMPL = "S";
        static string typIDColumnInMPL = "FA";
        static int firstSpotRow = 30;
        static string sheetName = "SPT_G2x_03.03.2019";
        static string sheetnameTarget = "Arkusz1";
        static int firstRowTargt = 2;
        static int spotNumTarget = 3;
        static int typIDColumnTarget = 4;

        private static Excel.Application mplXlApp;
        private static Excel.Workbooks mplXlWorkbooks;
        private static Excel.Workbook mplXlWorkbook;
        private static Excel.Application destinationXlApp;
        private static Excel.Workbooks destinationXlWorkbooks;
        private static Excel.Workbook destinationXlWorkbook;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        internal static void Execute()
        {
            string selectedMPL = CommonLibrary.CommonMethods.SelectDirOrFile(false, "MPL List", "*.xlsm; *.xlsx");
            string selectedfile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "TargetFile", "*.xlsm; *.xlsx");
            IDictionary<int, int> pointsAndTypIDFromMPL = GetPointsWithTypIDFromMPL(selectedMPL);
            FillTargetFile(selectedfile, pointsAndTypIDFromMPL);
        }

        private static void FillTargetFile(string selectedfile, IDictionary<int, int> pointsAndTypIDFromMPL)
        {
            int nrofspots = 0;
            destinationXlApp = new Excel.Application();
            destinationXlWorkbooks = destinationXlApp.Workbooks;
            destinationXlWorkbook = destinationXlWorkbooks.Open(selectedfile);
            Excel._Worksheet destinationxlWorksheet = destinationXlWorkbook.Sheets[sheetnameTarget];
            Excel.Range destinationxlRange = destinationxlWorksheet.UsedRange;

            for (int i = firstRowTargt; i <= destinationxlRange.Rows.Count; i++)
            {
                try
                {
                    int currentSpotNum = int.Parse(destinationxlRange.Cells[i, spotNumTarget].FormulaLocal);
                    if (pointsAndTypIDFromMPL.Keys.Contains(currentSpotNum))
                        destinationxlRange.Cells[i, typIDColumnTarget] = pointsAndTypIDFromMPL[currentSpotNum];
                    else
                    { }
                }
                catch
                {
                    nrofspots = i - 1;
                    break;                    
                }
            }
            string outputFileName = Path.GetFileNameWithoutExtension(selectedfile) + "_changed.xlsx";
            string dir = Path.GetDirectoryName(selectedfile);
            object misValue = System.Reflection.Missing.Value;

            destinationXlWorkbook.SaveAs(dir+"\\"+outputFileName, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            MessageBox.Show("Saved at " + dir + "\\" + outputFileName+"\r\nSpots found " + nrofspots, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            CleanUpExcel("destination");
        }

        private static IDictionary<int, int> GetPointsWithTypIDFromMPL(string selectedfile)
        {
            int spotNumberColumnInMPLNumber = ReadSpotsMethods.GetColumnNumber(spotNumberColumnInMPL);
            int typIDColumnInMPLNumber = ReadSpotsMethods.GetColumnNumber(typIDColumnInMPL);
            return FindWeldPointsInMPL(selectedfile, spotNumberColumnInMPLNumber, typIDColumnInMPLNumber);
        }

        private static IDictionary<int, int> FindWeldPointsInMPL(string mplPath, int spotColumn, int typIdColumn)
        {
            IDictionary<int, int> result = new Dictionary<int, int>();

            mplXlApp = new Excel.Application();
            mplXlWorkbooks = mplXlApp.Workbooks;
            mplXlWorkbook = mplXlWorkbooks.Open(mplPath);
            Excel._Worksheet mplxlWorksheet = mplXlWorkbook.Sheets[sheetName];
            Excel.Range mplxlRange = mplxlWorksheet.UsedRange;

            for (int i = firstSpotRow; i <= mplxlRange.Rows.Count; i++)
            {
                if (mplxlRange.Cells[i, spotColumn].FormulaLocal == "")
                    break;
                int spotNum = 0;
                bool isSpot = int.TryParse(mplxlRange.Cells[i, spotColumn].FormulaLocal, out spotNum);
                if (isSpot && !result.Keys.Contains(spotNum))
                {
                    try
                    {
                       result.Add(spotNum, int.Parse(mplxlRange.Cells[i, typIdColumn].FormulaLocal));                            
                    }
                    catch
                    {
                        result.Add(spotNum, 999999);
                    }
                    }
            }
            CleanUpExcel("mpl");
            return result;
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
                destinationXlWorkbook = null;
                destinationXlWorkbooks = null;
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
    }
}
