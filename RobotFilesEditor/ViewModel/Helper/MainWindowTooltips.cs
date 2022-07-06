using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.ViewModel.Helper
{
    public class MainWindowTooltips
    {
        public string ChangeNameTooltip { get; private set; }
        public string OpenLogTooltip { get; private set; }
        public string OpenDestTooltip { get; private set; }
        public string ConvertTrelloTooltip { get; private set; }
        public string FillExcelTooltip { get; private set; }
        public string CreateOrgsCommandTooltip { get; private set; }
        public string CreateGripperCommandTooltip { get; private set; }
        public string CreateGripperXMLTooltip { get; private set; }
        public string ReadConfigDatTooltip { get; private set; }
        public string MirrorTooltip { get; private set; }
        public string ReadSpotPointsTooltip { get; private set; }
        public string ReadBackupForWBTooltip { get; private set; }
        public string RenumberPointsTooltip { get; private set; }
        public string TypIdChangeTooltip { get; private set; }
        public string CheckBasesTooltip { get; private set; }
        public string RetrieveBackupsTooltip { get; private set; }
        public string PrepareSOVBackupTooltip { get; private set; }
        public string FixPTPandLINTooltip { get; private set; }
        public string ValidateBackupKUKATooltip { get; private set; }
        public string SafetyToolsToolTip { get; private set; }
        public string RobKalDatPropTooltip { get; private set; }
        public string SasFillerFromBackupTooltip { get; private set; }
        public string GetMengeTooltip { get; private set; }
        public string CompareSpotsTooltip { get; private set; }
        public string ReadMessprotokolTooltip { get; private set; }
        public string ABBHelperTooltip { get; private set; }
        public string RenamePointsABBTooltip { get; private set; }
        public string ReadSpotPointsABBTooltip { get; private set; }
        public string GenerateOrgsFanucTooltip { get; private set; }
        public string FanucMirrorTooltip { get; private set; }
        public string RenumberLinesFanucToolTip { get; private set; }
        public string CompareSOVAndOLPToolTip { get; private set; }
        public string ValidateCommentsToolTip { get; private set; }
        public string ValidateBackupFanucToolTip { get; private set; }
        public string DividePathByCollsTooltip { get; private set; }
        public string CleanLibrootTooltip { get; private set; }
        public string ReadSafetyXMLToolTip { get; private set; }
        public string FixSASCollisionsFanucToolTip { get; private set; }
        public string PayloadsFanucToolTip { get; private set; }

        public MainWindowTooltips()
        {
            ChangeNameTooltip = "Change your user name. Your name will be visible in program headers";
            OpenLogTooltip = "Open log from your previous scan of downloaded paths in notepad";
            OpenDestTooltip = "Open the directory containing processed files";
            ConvertTrelloTooltip = "Converts JSon file exported from Trello to Excel file with progess report";
            FillExcelTooltip = "Prepares IBN checklist based on empty input Checklist and scanned paths";

            CreateOrgsCommandTooltip = "Creates organization programms based on scanned paths for KUKA robot";
            CreateGripperCommandTooltip = "Creates .src file with gripper configuration. Function mostly useful for KRC2 robots";
            CreateGripperXMLTooltip = "Creates Gripper.xml file necessary for gripper configuration on KRC4 robots based on a03_grp_user.dat or .src file";
            ReadConfigDatTooltip = "Create Symname.txt file necessary to upload symbols to KRC2 robots based on $config.dat file";
            MirrorTooltip = "Create mirrored paths for KUKA robots";
            ReadSpotPointsTooltip = "Functionality that allows you to read process points from selected backup, compare it with VIP list, and prepare the report in Excel file";
            ReadBackupForWBTooltip = "Reads contents of backup and prepares an Excel report with tools, bases, coliision zones etc";
            RenumberPointsTooltip = "Renumber points using rule P10, P20, P30 etc. and clean .dat files";
            TypIdChangeTooltip = "Change typ Id for spot points";
            CheckBasesTooltip = "Check if bases in robot backups are the same as bases in RobKatDat project";
            RetrieveBackupsTooltip = "Copy all backups from subfolders in selected folder to one folder";
            PrepareSOVBackupTooltip = "Prepare SOV backup based on empty backups prepared in SAS and downloads from PS \"After harvester\"";
            FixPTPandLINTooltip = "Function to convert downloads made in KSS 8.3 to syntax of KSS 8.6";
            ValidateBackupKUKATooltip = "Functionality to check syntax in backup (missing variable declarations, calls of non-existing procedures etc.)";
            SafetyToolsToolTip = "Open safety tools window";
            ReadSafetyXMLToolTip = "Reads safety xml generated from Workvisual and outputs safety zones, tools etc.";
            RobKalDatPropTooltip = "Open tool to calculate bases, read RobKalDat projects etc.";
            SasFillerFromBackupTooltip = "Fill SAS data based on data contained in backup (orgs, areas, jobs etc.)";
            GetMengeTooltip = "Get data from backups necessary to fill Mengelist (amount of process points, glue bead lenths etc.)";
            CompareSpotsTooltip = "Compare spot points positions between two backups of the same robot";
            ReadMessprotokolTooltip = "Read position from messprotokoll and convert them into PS path";

            ABBHelperTooltip = "Open window containing tools for processing ABB robot downloads: adding spaces, mirrorin paths etc.";
            RenamePointsABBTooltip = "Opens tool to rename points in ABB modules based on rules defined by user";
            ReadSpotPointsABBTooltip = "Reads process points from VW backups and compares with VIP list";

            GenerateOrgsFanucTooltip = "Creates organization programms based on scanned paths for FANUC robot";
            FanucMirrorTooltip = "Create mirrored paths for FANUC robots";
            RenumberLinesFanucToolTip = "Tool to renumber points in all .LS files found in selected directory";
            CompareSOVAndOLPToolTip = "Opens tool to compare selected robot programs with content of selected robot backup";
            ValidateCommentsToolTip = "Tool to correct mistakes in comments on Fanuc robots - finds comments longer than 24 signs";
            ValidateBackupFanucToolTip = "Functionality to check syntax in backup (missing variable declarations, calls of non-existing procedures etc.)";

            DividePathByCollsTooltip = "Reads robot program and splits it to smaller programs based on collision requests and releases (works only for KUKA and FANUC robots)";
            CleanLibrootTooltip = "Finds not used elements in psz and libroot.";

            FixSASCollisionsFanucToolTip = "Fixes SAS file to display collision matrix for Fanuc robots";
            PayloadsFanucToolTip = "Get payload values from backup and fill payload diagram excel file.";
        }
    }
}
