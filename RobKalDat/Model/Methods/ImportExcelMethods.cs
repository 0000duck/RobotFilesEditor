using GalaSoft.MvvmLight.Messaging;
using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace RobKalDat.Model.Methods
{
    public static class ExcelMethods
    {
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static Excel.Application messprotokollXlApp;
        private static Excel.Workbooks messprotokollXlWorkbooks;
        private static Excel.Workbook messprotokollXlWorkbook;

        private static Excel.Application exportprotokollXlApp;
        private static Excel.Workbooks exportprotokollXlWorkbooks;
        private static Excel.Workbook exportprotokollXlWorkbook;

        private static object misValue = System.Reflection.Missing.Value;

        const int startRow = 14;
        const int startColumn = 1;
        const string sheetName = "Messprotokoll";

        internal static void Execute()
        {
            try
            {
                ObservableCollection<Measurement> result = new ObservableCollection<Measurement>();
                string excelFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1: "*.xls", filter2: "*.xlsx", filter1Descr: "Excel File .xls", filter2Descr: "Excel File .xlsx");
                if (string.IsNullOrEmpty(excelFile))
                    return;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                int rowCounter = startRow;
                messprotokollXlApp = new Excel.Application();
                messprotokollXlWorkbooks = messprotokollXlApp.Workbooks;
                messprotokollXlApp.Visible = true;
                messprotokollXlWorkbook = messprotokollXlWorkbooks.Open(excelFile);
                messprotokollXlApp.Visible = false;
                Excel._Worksheet messprotokollxlWorksheet = messprotokollXlWorkbook.Sheets[sheetName];
                Excel.Range messprotokollxlRange = messprotokollxlWorksheet.UsedRange;

                while (!string.IsNullOrEmpty(messprotokollxlRange.Cells[rowCounter, startColumn].FormulaLocal))
                {
                    Measurement currentMeas = new Measurement() { Name = messprotokollxlRange.Cells[rowCounter, startColumn].FormulaLocal };
                    double XSoll, YSoll, ZSoll, RXSoll, RYSoll, RZSoll, XIst, YIst, ZIst, RXIst, RYIst, RZIst;

                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 1].FormulaLocal, out XSoll))
                        currentMeas.XSoll = XSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 2].FormulaLocal, out YSoll))
                        currentMeas.YSoll = YSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 3].FormulaLocal, out ZSoll))
                        currentMeas.ZSoll = ZSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 4].FormulaLocal, out RXSoll))
                        currentMeas.RXSoll = RXSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 5].FormulaLocal, out RYSoll))
                        currentMeas.RYSoll = RYSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 6].FormulaLocal, out RZSoll))
                        currentMeas.RZSoll = RZSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 7].FormulaLocal, out XIst))
                        currentMeas.XIst = XIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 8].FormulaLocal, out YIst))
                        currentMeas.YIst = YIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 9].FormulaLocal, out ZIst))
                        currentMeas.ZIst = ZIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 10].FormulaLocal, out RXIst))
                        currentMeas.RXIst = RXIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 11].FormulaLocal, out RYIst))
                        currentMeas.RYIst = RYIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 12].FormulaLocal, out RZIst))
                        currentMeas.RZIst = RZIst;
                    result.Add(currentMeas);
                    rowCounter++;
                }
                CleanUpExcel("messprotokoll");
                List<int> indexes = new List<int>();
                foreach (var item in result.Where(x=>x.HasRealValues == null))
                    indexes.Add(result.IndexOf(item));
                foreach (var item in indexes)
                    result[item].HasRealValues = "false";
                Messenger.Default.Send(result, "foundMeas");
                Messenger.Default.Send(result, "importFromExcel");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                CleanUpExcel("messprotokoll");
            }
        }

        internal static void ExportToExcel(ObservableCollection<Measurement> measurements)
        {
            string savePath = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "XLS file", filter1: "*.xls");
            if (string.IsNullOrEmpty(savePath))
                return;
            string workingfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AiutBaseCalculator");
            if (!Directory.Exists(workingfolder))
                Directory.CreateDirectory(workingfolder);
            if (!File.Exists(Path.Combine(workingfolder, "Messprotokoll_template.xls")))
            {
                File.WriteAllBytes(Path.Combine(workingfolder, "Messprotokoll_template.xls"), RobKalResources.Messprotokoll_template);
            }
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            int rowCounter = startRow;

            savePath = string.IsNullOrEmpty(Path.GetExtension(savePath)) ? savePath + ".xls" : savePath;
            try
            {
                exportprotokollXlApp = new Excel.Application();
                exportprotokollXlWorkbooks = exportprotokollXlApp.Workbooks;
                Thread.Sleep(200);
                exportprotokollXlWorkbook = exportprotokollXlWorkbooks.Open(Path.Combine(workingfolder, "Messprotokoll_template.xls"));
                Thread.Sleep(200);
                Excel._Worksheet exportprotokollxlWorksheet = exportprotokollXlWorkbook.Sheets[sheetName];
                Excel.Range exportprotokollxlRange = exportprotokollxlWorksheet.UsedRange;
                foreach (var meas in measurements)
                {
                    string status = "-> ** Keine oder nicht korrekte Ist-Werte !!! **" ;
                    bool isInTolerance = true;
                    if (meas.HasRealValues.ToLower() == "true")
                    {
                        isInTolerance = CheckTolerance(meas);
                        status = isInTolerance ? "-> Werte OK" : "-> ********  Abweichung zu groß !!!  *************";
                    }
                    exportprotokollxlWorksheet.Cells[rowCounter, 1] = meas.Name;
                    exportprotokollxlWorksheet.Cells[rowCounter, 2] = meas.XSoll;
                    exportprotokollxlWorksheet.Cells[rowCounter, 3] = meas.YSoll;
                    exportprotokollxlWorksheet.Cells[rowCounter, 4] = meas.ZSoll;
                    exportprotokollxlWorksheet.Cells[rowCounter, 5] = meas.RXSoll;
                    exportprotokollxlWorksheet.Cells[rowCounter, 6] = meas.RYSoll;
                    exportprotokollxlWorksheet.Cells[rowCounter, 7] = meas.RZSoll;
                    exportprotokollxlWorksheet.Cells[rowCounter, 8] = meas.HasRealValues.ToLower() == "true" ? meas.XIst.ToString("F2").Replace(",", ".") : string.Empty;
                    exportprotokollxlWorksheet.Cells[rowCounter, 9] = meas.HasRealValues.ToLower() == "true" ? meas.YIst.ToString("F2").Replace(",", ".") : string.Empty; ;
                    exportprotokollxlWorksheet.Cells[rowCounter, 10] = meas.HasRealValues.ToLower() == "true" ? meas.ZIst.ToString("F2").Replace(",", ".") : string.Empty; ;
                    exportprotokollxlWorksheet.Cells[rowCounter, 11] = meas.HasRealValues.ToLower() == "true" ? meas.RXIst.ToString("F4").Replace(",", ".") : string.Empty; ;
                    exportprotokollxlWorksheet.Cells[rowCounter, 12] = meas.HasRealValues.ToLower() == "true" ? meas.RYIst.ToString("F4").Replace(",", ".") : string.Empty; ;
                    exportprotokollxlWorksheet.Cells[rowCounter, 13] = meas.HasRealValues.ToLower() == "true" ? meas.RZIst.ToString("F4").Replace(",", ".") : string.Empty; ;
                    exportprotokollxlWorksheet.Cells[rowCounter, 14] = status;
                    exportprotokollxlWorksheet.Cells[rowCounter, 15] = meas.HasRealValues.ToLower() == "true" ? (meas.XIst - meas.XSoll).ToString("F2").Replace(",", ".") : string.Empty;
                    exportprotokollxlWorksheet.Cells[rowCounter, 16] = meas.HasRealValues.ToLower() == "true" ? (meas.YIst - meas.YSoll).ToString("F2").Replace(",", ".") : string.Empty;
                    exportprotokollxlWorksheet.Cells[rowCounter, 17] = meas.HasRealValues.ToLower() == "true" ? (meas.ZIst - meas.ZSoll).ToString("F2").Replace(",", ".") : string.Empty;
                    exportprotokollxlWorksheet.Cells[rowCounter, 18] = meas.HasRealValues.ToLower() == "true" ? GetAngle(meas.RXIst, meas.RXSoll) : string.Empty;
                    exportprotokollxlWorksheet.Cells[rowCounter, 19] = meas.HasRealValues.ToLower() == "true" ? GetAngle(meas.RYIst, meas.RYSoll) : string.Empty;
                    exportprotokollxlWorksheet.Cells[rowCounter, 20] = meas.HasRealValues.ToLower() == "true" ? GetAngle(meas.RZIst, meas.RZSoll) : string.Empty;
                    //exportprotokollxlWorksheet.Cells[rowCounter, 20] = meas.HasRealValues.ToLower() == "true" ? (meas.RZIst - meas.RZSoll).ToString("F2").Replace(",", ".") : string.Empty;
                    if (!isInTolerance)
                    {
                        Excel.Range range = exportprotokollxlWorksheet.Range["A" + rowCounter, "T" + rowCounter];
                        range.Interior.Color = Excel.XlRgbColor.rgbYellow;
                    }
                    rowCounter++;
                }

                if (File.Exists(savePath))
                {
                    System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("File " + savePath + " already exists. Overwrite?", "Overwrite files?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        File.Delete(savePath);
                        exportprotokollXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Succesfully saved at " + savePath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    exportprotokollXlWorkbook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    MessageBox.Show("Succesfully saved at " + savePath, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                CleanUpExcel("erroranalyze");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                
            }

            catch (Exception ex)
            {                
                CleanUpExcel("erroranalyze");
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string GetAngle(double val1, double val2)
        {

            if (Math.Abs(val1 - val2) < 180.0)
                return (val1 - val2).ToString("F2").Replace(",", ".");            
            if (val1 - val2 > 180.0)
                return (val1 - val2 - 360.0).ToString("F2").Replace(",", ".");
            if (val1 - val2 <= 180.0)
                return (val1 - val2 + 360.0).ToString("F2").Replace(",", ".");
            return string.Empty;
        }

        private static bool CheckTolerance(Measurement meas)
        {
            double toleranceTrans = 20.0, toleranceRot = 1.0;
            if (Math.Abs(meas.XIst - meas.XSoll) > toleranceTrans)
                return false;
            if (Math.Abs(meas.YIst - meas.YSoll) > toleranceTrans)
                return false;
            if (Math.Abs(meas.ZIst - meas.ZSoll) > toleranceTrans)
                return false;
            if (double.Parse(GetAngle(meas.RXIst, meas.RXSoll),CultureInfo.InvariantCulture) > toleranceRot)
                return false;
            if (double.Parse(GetAngle(meas.RYIst, meas.RYSoll), CultureInfo.InvariantCulture) > toleranceRot)
                return false;
            if (double.Parse(GetAngle(meas.RZIst, meas.RZSoll), CultureInfo.InvariantCulture) > toleranceRot)
                return false;
            return true;

        }

        private static void CleanUpExcel(string app)
        {
            switch (app)
            {
                case "messprotokoll":
                    if (messprotokollXlApp != null)
                    {
                        int hWndDest = messprotokollXlApp.Application.Hwnd;

                        uint processID;

                        GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                        Process.GetProcessById((int)processID).Kill();
                    }
                    messprotokollXlWorkbook = null;
                    messprotokollXlWorkbooks = null;
                    messprotokollXlApp = null;
                    break;
                case "erroranalyze":
                    if (exportprotokollXlApp != null)
                    {
                        int hWndDest = exportprotokollXlApp.Application.Hwnd;

                        uint processID;

                        GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                        Process.GetProcessById((int)processID).Kill();
                    }
                    exportprotokollXlWorkbook = null;
                    exportprotokollXlWorkbooks = null;
                    exportprotokollXlApp = null;
                break;
            }
        }
    }      
    
}
