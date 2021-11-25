using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Excel = Microsoft.Office.Interop.Excel;

namespace RobotFilesEditor.Model.Operations
{
    public static class FillExcelMethods
    {
        private static Excel.Application sourceXlApp;
        private static Excel.Workbooks sourceXlWorkbooks;
        private static Excel.Workbook sourceXlWorkbook;
        private static Excel.Application destinationXlApp;
        //private static Excel.Workbooks destinationXlWorkbooks;
        private static Excel.Workbook destinationXlWorkbook;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        internal static void FillExcel()
        {
            if (!(GlobalData.AllFiles == null || GlobalData.AllFiles.Count <= 0))
            {
                string excelFile = SelectExcelFile();
                if (!string.IsNullOrEmpty(excelFile))
                {
                    ReadExcel(excelFile);
                    WriteExcel(excelFile);
                }
            }
            else
                MessageBox.Show("No paths found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void WriteExcel(string file)
        {
            try
            {
                destinationXlApp = new Excel.Application();
                destinationXlWorkbook = sourceXlWorkbook;
                // destinationXlWorkbook.Worksheets.Add(sheets[0]);

                if (!Directory.Exists(Path.GetDirectoryName(file) + "\\Filled"))
                    Directory.CreateDirectory(Path.GetDirectoryName(file) + "\\Filled");
                destinationXlWorkbook.SaveAs(Path.GetDirectoryName(file) + "\\Filled\\"+ Path.GetFileName(file), Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled);
                destinationXlWorkbook.Close();
                CleanUpExcel();
                MessageBox.Show("Checklist saved at " + GlobalData.DestinationPath + "\\CheckList_Online\\Checklist.xlsm", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                CleanUpExcel();
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static void ReadExcel(string excelFile)
        {
            sourceXlApp = new Excel.Application();
            sourceXlWorkbooks = sourceXlApp.Workbooks;
            sourceXlWorkbook = sourceXlWorkbooks.Open(excelFile);
            //List<Excel.Worksheet> sheets = new List<Excel.Worksheet>();
            foreach (Excel.Worksheet sheet in sourceXlWorkbook.Worksheets)
            {
                Excel.Range xlRange = sheet.UsedRange;
                int counter = 0;
                foreach (var job in GlobalData.SrcPathsAndJobs)
                {
                    
                    sheet.Rows[113 + counter].Insert();
                    sheet.Cells[113 + counter, 4] = job.Key;
                    sheet.Range["D"+(113 + counter).ToString()].Font.Name = "Calibri";
                    sheet.Range["D" + (113 + counter).ToString()].Font.Size = 11;
                    sheet.Range["D" + (113 + counter).ToString()].Font.Bold = false;
                    sheet.Range["D" + (113 + counter).ToString()].Font.Color = System.Drawing.Color.Black;
                    sheet.Range["D" + (113 + counter).ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    counter++;
                }
                counter = 0;
                foreach (var job in GlobalData.SrcPathsAndJobs)
                {

                    sheet.Rows[158 + GlobalData.SrcPathsAndJobs.Count + counter].Insert();
                    sheet.Cells[158 + GlobalData.SrcPathsAndJobs.Count + counter, 4] = job.Key;
                    sheet.Range["D" +(158 + GlobalData.SrcPathsAndJobs.Count + counter).ToString()].Font.Name = "Calibri";
                    sheet.Range["D" + (158 + GlobalData.SrcPathsAndJobs.Count + counter).ToString()].Font.Size = 11;
                    sheet.Range["D" + (158 + GlobalData.SrcPathsAndJobs.Count + counter).ToString()].Font.Bold = false;
                    sheet.Range["D" + (158 + GlobalData.SrcPathsAndJobs.Count + counter).ToString()].Font.Color = System.Drawing.Color.Black;
                    sheet.Range["D" + (158 + GlobalData.SrcPathsAndJobs.Count + counter).ToString()].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                    counter++;
                }

                //sheets.Add(sheet);
            }
           // return sheets;
            
        }

        private static string SelectExcelFile()
        {            
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.Filters.Add(new CommonFileDialogFilter("Excel file (*.xlsm)", ".xlsm"));
            IDictionary<int, string> result = new Dictionary<int, string>();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
                return "";
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
            if (sourceXlApp != null)
            {
                int hWndSrc = sourceXlApp.Application.Hwnd;

                uint processID;

                GetWindowThreadProcessId((IntPtr)hWndSrc, out processID);
                Process.GetProcessById((int)processID).Kill();
            }
            sourceXlWorkbook = null;
            sourceXlWorkbooks = null;
            sourceXlApp = null;
            destinationXlWorkbook = null;
            destinationXlApp = null;
        }

    }
}
