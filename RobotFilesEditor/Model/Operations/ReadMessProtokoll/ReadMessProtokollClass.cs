using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using RobotFilesEditor.Dialogs.OptionSelector;

namespace RobotFilesEditor.Model.Operations.ReadMessProtokoll
{
    public class ReadMessProtokollClass
    {
        #region fields
        private string messprotokollFile;
        private Excel.Application messprotokollXlApp;
        private Excel.Workbooks messprotokollXlWorkbooks;
        private Excel.Workbook messprotokollXlWorkbook;
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        int rowCounterFanuc;

        const int startRow = 14;
        const int startColumn = 1;
        const string sheetName = "Messprotokoll";
        #endregion 

        #region ctor
        public ReadMessProtokollClass(GlobalData.RobotController robotType)
        {
            messprotokollFile = string.Empty;
            string type = string.Empty;
            if (robotType == GlobalData.RobotController.KUKA)
            {
                OptionSelectorVM vm = new OptionSelectorVM(new List<string>() { "KSS 8.3", "KSS 8.6" }, "KSS version:");
                OptionSelectorWindow window = new OptionSelectorWindow(vm);
                var dialog = window.ShowDialog();
                type = vm.Result;
            }
            List<RobotPointMessprotokoll> pointsInMessProtokoll = GetPointsInMessprotokoll();
            CreatePaths(pointsInMessProtokoll, robotType, type);
        }
        #endregion

        #region methods
        private void CreatePaths(List<RobotPointMessprotokoll> pointsInMessProtokoll, GlobalData.RobotController robotType, string type)
        {
            try
            {
                if (pointsInMessProtokoll == null)
                    return;
                string resultFile = string.Empty, resultDatFileSoll = string.Empty, resultDatFileIst = string.Empty;
                rowCounterFanuc = 1;
                foreach (var point in pointsInMessProtokoll)
                {
                    switch (robotType)
                    {
                        case GlobalData.RobotController.ABB:
                            {
                                var quatSoll = CommonLibrary.CommonMethods.RadToQuat(point.CSoll, point.BSoll, point.ASoll);
                                var quatIst = CommonLibrary.CommonMethods.RadToQuat(point.CIst, point.BIst, point.AIst);
                                resultFile += string.Join(Environment.NewLine,
                                    "MoveJ "+point.Name+@",vmax,fine,tool0\Wobj:=wobj99;",
                                    "");
                                resultDatFileSoll += string.Join(Environment.NewLine,
                                    "CONST robtarget "+point.Name+":=[["+ point.XSoll.ToString(CultureInfo.InvariantCulture) + ","+ point.YSoll.ToString(CultureInfo.InvariantCulture) + ","+ point.ZSoll.ToString(CultureInfo.InvariantCulture) + "],["+quatSoll[0].ToString(CultureInfo.InvariantCulture)+ "," + quatSoll[1].ToString(CultureInfo.InvariantCulture) + "," + quatSoll[2].ToString(CultureInfo.InvariantCulture) + "," + quatSoll[3].ToString(CultureInfo.InvariantCulture) + "],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];",
                                    "");
                                resultDatFileIst += string.Join(Environment.NewLine,
                                    "CONST robtarget " + point.Name + ":=[[" + point.XIst.ToString(CultureInfo.InvariantCulture) + "," + point.YIst.ToString(CultureInfo.InvariantCulture) + "," + point.ZIst.ToString(CultureInfo.InvariantCulture) + "],[" + quatIst[0].ToString(CultureInfo.InvariantCulture) + "," + quatIst[1].ToString(CultureInfo.InvariantCulture) + "," + quatIst[2].ToString(CultureInfo.InvariantCulture) + "," + quatIst[3].ToString(CultureInfo.InvariantCulture) + "],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];",
                                    "");

                                break;
                            }
                        case GlobalData.RobotController.FANUC:
                            {
                                resultFile += string.Join(Environment.NewLine,
                                    "  "+(rowCounterFanuc + 2)+":J P["+ rowCounterFanuc + ":"+point.Name+"] 100% FINE;",
                                    "");
                                resultDatFileSoll  += string.Join(Environment.NewLine,
                                    "P["+ rowCounterFanuc + ":\""+point.Name+"\"] {",
                                    "   GP1 :",
                                    "        UF : 99, UT : 0,    CONFIG : 'F U T,0,0,0',",
                                    "        X = "+ point.XSoll.ToString(CultureInfo.InvariantCulture) + " mm,    Y = "+ point.YSoll.ToString(CultureInfo.InvariantCulture) + " mm,    Z = "+ point.ZSoll.ToString(CultureInfo.InvariantCulture) + " mm,",
                                    "        W = "+ point.CSoll.ToString(CultureInfo.InvariantCulture) + " deg,    P = "+ point.BSoll.ToString(CultureInfo.InvariantCulture) + " deg,    R = "+ point.ASoll.ToString(CultureInfo.InvariantCulture) + " deg",
                                    "};",
                                    "");
                                resultDatFileIst += string.Join(Environment.NewLine,
                                    "P[" + rowCounterFanuc + ":\"" + point.Name + "\"] {",
                                    "   GP1 :",
                                    "        UF : 99, UT : 0,    CONFIG : 'F U T,0,0,0',",
                                    "        X = " + point.XIst.ToString(CultureInfo.InvariantCulture) + " mm,    Y = " + point.YIst.ToString(CultureInfo.InvariantCulture) + " mm,    Z = " + point.ZIst.ToString(CultureInfo.InvariantCulture) + " mm,",
                                    "        W = " + point.CIst.ToString(CultureInfo.InvariantCulture) + " deg,    P = " + point.BIst.ToString(CultureInfo.InvariantCulture) + " deg,    R = " + point.AIst.ToString(CultureInfo.InvariantCulture) + " deg",
                                    "};",
                                    "");
                                rowCounterFanuc++;
                                break;
                            }
                        case GlobalData.RobotController.KUKA:
                            {
                                switch (type)
                                {
                                    case "KSS 8.6":
                                        {
                                            resultFile += string.Join(Environment.NewLine,
                                            ";FOLD PTP " + point.Name + " Vel=100 % P" + point.Name + " Tool[0]:Tool0 Base[99]:ANLANGENNULL;%{PE}",
                                            ";FOLD Parameters ;%{h}",
                                            ";Params IlfProvider=kukaroboter.basistech.inlineforms.movement.old; Kuka.IsGlobalPoint = False; Kuka.PointName=" + point.Name + "; Kuka.BlendingEnabled=True; Kuka.MoveDataPtpName=P" + point.Name + "; Kuka.VelocityPtp=100; Kuka.CurrentCDSetIndex=0; Kuka.MovementParameterFieldEnabled=True; IlfCommand=PTP",
                                            ";ENDFOLD",
                                            "$BWDSTART=FALSE",
                                            "PDAT_ACT=P" + point.Name + "",
                                            "FDAT_ACT=F" + point.Name + "",
                                            "BAS(#PTP_PARAMS,100)",
                                            "SET_CD_PARAMS(0)",
                                            "PTP X" + point.Name,
                                            ";ENDFOLD"
                                            , ""
                                            );
                                            break;
                                        }
                                    case "KSS 8.3":
                                        {
                                            resultFile += string.Join(Environment.NewLine,
                                                ";FOLD PTP " + point.Name + " Vel=100 % P" + point.Name + " Tool[0]:Gripper1 Base[99]:ANLANGENNULL;%{PE}%R 8.3.48,%MKUKATPBASIS,%CMOVE,%VPTP,%P 1:PTP, 2:" + point.Name + ", 3:, 5:100, 7:P" + point.Name + "",
                                                "$BWDSTART=FALSE",
                                                "PDAT_ACT=P" + point.Name + "",
                                                "FDAT_ACT=F" + point.Name + "",
                                                "BAS(#PTP_PARAMS,100)",
                                                "PTP X" + point.Name,
                                                ";ENDFOLD"
                                                , ""
                                            );
                                            break;
                                        }
                                }
                                
                                resultDatFileSoll += string.Join(Environment.NewLine,
                                    "DECL FDAT F" + point.Name + "={TOOL_NO 0,BASE_NO 99,IPO_FRAME #BASE,POINT2[] \" \"}",
                                    "DECL PDAT P" + point.Name + "={VEL 100.000,ACC 100.000,APO_DIST 0.0,GEAR_JERK 50.0000}",
                                    "DECL E6POS X" + point.Name + "={X " + point.XSoll.ToString(CultureInfo.InvariantCulture) + ",Y " + point.YSoll.ToString(CultureInfo.InvariantCulture) + ",Z " + point.ZSoll.ToString(CultureInfo.InvariantCulture) + ",A " + point.ASoll.ToString(CultureInfo.InvariantCulture) + ",B " + point.BSoll.ToString(CultureInfo.InvariantCulture) + ",C " + point.CSoll.ToString(CultureInfo.InvariantCulture) + ",S 2,T 10,E1 0.0,E2 0.0,E3 0.0,E4 0.0,E5 0.0,E6 0.0}"
                                    ,""
                                    );
                                resultDatFileIst += string.Join(Environment.NewLine,
                                    "DECL FDAT F" + point.Name + "={TOOL_NO 0,BASE_NO 99,IPO_FRAME #BASE,POINT2[] \" \"}",
                                    "DECL PDAT P" + point.Name + "={VEL 100.000,ACC 100.000,APO_DIST 0.0,GEAR_JERK 50.0000}",
                                    "DECL E6POS X" + point.Name + "={X " + point.XIst.ToString(CultureInfo.InvariantCulture) + ",Y " + point.YIst.ToString(CultureInfo.InvariantCulture) + ",Z " + point.ZIst.ToString(CultureInfo.InvariantCulture) + ",A " + point.AIst.ToString(CultureInfo.InvariantCulture) + ",B " + point.BIst.ToString(CultureInfo.InvariantCulture) + ",C " + point.CIst.ToString(CultureInfo.InvariantCulture) + ",S 2,T 10,E1 0.0,E2 0.0,E3 0.0,E4 0.0,E5 0.0,E6 0.0}"
                                    ,""
                                    );
                                break;
                            }
                    }
                }
                SaveFiles(resultFile, resultDatFileSoll, resultDatFileIst, robotType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveFiles(string resultFile, string resultDatFileSoll, string resultDatFileIst, GlobalData.RobotController robotType)
        {
            if (messprotokollFile.Length > 18)
            {
                Dialogs.RenamePointDialog.RenamePointDialogVM vm = new Dialogs.RenamePointDialog.RenamePointDialogVM(messprotokollFile, 18, GlobalData.RenameWindowType.Path, new List<string>());
                Dialogs.RenamePointDialog.RenamePointDialog dialog = new Dialogs.RenamePointDialog.RenamePointDialog(vm);
                dialog.ShowDialog();
                if (dialog.DialogResult == true)
                    messprotokollFile = vm.OutputName;
            }
            MessageBox.Show("Select directory to save files.", "Select directory", MessageBoxButton.OK, MessageBoxImage.Information);
            string dirToSave = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            if (string.IsNullOrEmpty(dirToSave))
                return;
            if (robotType == GlobalData.RobotController.KUKA)
            {
                if (!string.IsNullOrEmpty(resultDatFileSoll))
                    File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Soll.dat"), Properties.Resources.KUKA_DAT_TEMPLATE.Replace("{PATHNAME}", messprotokollFile + "_Soll").Replace("{PATH_DAT_CONTENT}", resultDatFileSoll));
                if (!string.IsNullOrEmpty(resultDatFileIst))
                    File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Ist.dat"), Properties.Resources.KUKA_DAT_TEMPLATE.Replace("{PATHNAME}", messprotokollFile + "_Ist").Replace("{PATH_DAT_CONTENT}", resultDatFileIst));
                File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Soll.src"), Properties.Resources.KUKA_SRC_TEMPLATE.Replace("{PATHNAME}", messprotokollFile + "_Soll").Replace("{PATH_SRC_CONTENT}", resultFile));
                File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Ist.src"), Properties.Resources.KUKA_SRC_TEMPLATE.Replace("{PATHNAME}", messprotokollFile + "_Ist").Replace("{PATH_SRC_CONTENT}", resultFile));
            }
            else if (robotType == GlobalData.RobotController.ABB)
            {
                string outPutFileSoll = Properties.Resources.ABB_MOD_TEMPLATE1.Replace("{PATHNAME}", messprotokollFile + "_Soll").Replace("{ABB_ROBTARGETS}", resultDatFileSoll).Replace("{ABB_PROCEDURE}",resultFile);
                string outPutFileIst = Properties.Resources.ABB_MOD_TEMPLATE1.Replace("{PATHNAME}", messprotokollFile + "_Ist").Replace("{ABB_ROBTARGETS}", resultDatFileIst).Replace("{ABB_PROCEDURE}", resultFile);
                File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Soll.mod"), outPutFileSoll);
                File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Ist.mod"), outPutFileIst);

            }
            else if (robotType == GlobalData.RobotController.FANUC)
            {
                string outPutFileSoll = Properties.Resources.FANUC_LS_TEMPLATE.Replace("{PATHNAME}", messprotokollFile + "_Soll").Replace("{ LINE_COUNT}", (rowCounterFanuc + 1).ToString()).Replace("{POS_MOVEMENT}", resultFile).Replace("{POS_DECLARATIONS}", resultDatFileSoll);
                string outPutFileIst = Properties.Resources.FANUC_LS_TEMPLATE.Replace("{PATHNAME}", messprotokollFile + "_Ist").Replace("{LINE_COUNT}", (rowCounterFanuc + 1).ToString()).Replace("{POS_MOVEMENT}", resultFile).Replace("{POS_DECLARATIONS}", resultDatFileIst);
                File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Soll.ls"), outPutFileSoll);
                File.WriteAllText(Path.Combine(dirToSave, messprotokollFile + "_Ist.ls"), outPutFileIst);
            }
            var dialogSuccess = System.Windows.Forms.MessageBox.Show("Successfuly saved at: " + dirToSave + ".\r\nWould you like to open directory?", "Success", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dialogSuccess == System.Windows.Forms.DialogResult.Yes)
                Process.Start(dirToSave);
        }

        private List<RobotPointMessprotokoll> GetPointsInMessprotokoll()
        {
            List<RobotPointMessprotokoll> result = new List<RobotPointMessprotokoll>();
            try
            {
                List<string> alreadyAdded = new List<string>();
                Regex isNumber = new Regex(@"^\s*\d", RegexOptions.IgnoreCase);
                Regex replaceRegex = new Regex(@"[^\w_]", RegexOptions.IgnoreCase);
                MessageBox.Show("Select messprotokoll file.", "Select file", MessageBoxButton.OK, MessageBoxImage.Information);
                string excelFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1: "*.xls", filter2: "*.xlsx", filter1Descr: "Excel File .xls", filter2Descr: "Excel File .xlsx");
                if (string.IsNullOrEmpty(excelFile))
                    return null;
                messprotokollFile = Path.GetFileNameWithoutExtension(excelFile);
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
                    RobotPointMessprotokoll currentMeas = new RobotPointMessprotokoll();

                    double XSoll, YSoll, ZSoll, RXSoll, RYSoll, RZSoll, XIst, YIst, ZIst, RXIst, RYIst, RZIst;

                    //currentMeas.Name = messprotokollxlRange.Cells[rowCounter, startColumn + 0].FormulaLocal.ToString().Replace("-","_").Replace("°","").Replace(" ","_").Replace("+","");
                    currentMeas.Name = messprotokollxlRange.Cells[rowCounter, startColumn + 0].FormulaLocal.ToString();
                    if (replaceRegex.IsMatch(currentMeas.Name))
                        currentMeas.Name = replaceRegex.Replace(currentMeas.Name, "");
                    if (currentMeas.Name.Length > 0 && isNumber.IsMatch(currentMeas.Name))
                        currentMeas.Name = "P" + currentMeas.Name;
                    if (currentMeas.Name.Length > 23)
                    {
                        Dialogs.RenamePointDialog.RenamePointDialogVM vm = new Dialogs.RenamePointDialog.RenamePointDialogVM(currentMeas.Name,23, GlobalData.RenameWindowType.Point, alreadyAdded);
                        Dialogs.RenamePointDialog.RenamePointDialog dialog = new Dialogs.RenamePointDialog.RenamePointDialog(vm);
                        dialog.ShowDialog();
                        if (dialog.DialogResult == true)
                            currentMeas.Name = vm.OutputName;
                    }
                    alreadyAdded.Add(currentMeas.Name);

                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 1].FormulaLocal, out XSoll))
                        currentMeas.XSoll = XSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 2].FormulaLocal, out YSoll))
                        currentMeas.YSoll = YSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 3].FormulaLocal, out ZSoll))
                        currentMeas.ZSoll = ZSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 4].FormulaLocal, out RXSoll))
                        currentMeas.CSoll = RXSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 5].FormulaLocal, out RYSoll))
                        currentMeas.BSoll = RYSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 6].FormulaLocal, out RZSoll))
                        currentMeas.ASoll = RZSoll;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 7].FormulaLocal, out XIst))
                        currentMeas.XIst = XIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 8].FormulaLocal, out YIst))
                        currentMeas.YIst = YIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 9].FormulaLocal, out ZIst))
                        currentMeas.ZIst = ZIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 10].FormulaLocal, out RXIst))
                        currentMeas.CIst = RXIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 11].FormulaLocal, out RYIst))
                        currentMeas.BIst = RYIst;
                    if (double.TryParse(messprotokollxlRange.Cells[rowCounter, startColumn + 12].FormulaLocal, out RZIst))
                        currentMeas.AIst = RZIst;
                    result.Add(currentMeas);
                    rowCounter++;
                }
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                CleanUpExcel("messprotokoll");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                CleanUpExcel("messprotokoll");
            }

            return result;
        }

        private void CleanUpExcel(string app)
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
            }
        }
        #endregion
    }
}
