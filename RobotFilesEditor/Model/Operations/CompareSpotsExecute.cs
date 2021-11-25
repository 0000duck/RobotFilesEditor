using CommonLibrary;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace RobotFilesEditor.Model.Operations
{
    public static class CompareSpotsExecute
    {
        private static Excel.Application oXL;
        private static Excel.Workbooks oWBs;
        private static Excel.Workbook oWB;
        private static Excel.Sheets sheets;
        private static Excel.Worksheet oSheet;

        internal static async Task Execute()
        {
            MessageBox.Show("Select first set of backups", "Select directiory", MessageBoxButton.OK, MessageBoxImage.Information);
            string selectedDir1 = CommonMethods.SelectDirOrFile(true);
            MessageBox.Show("Select second set of backups", "Select directiory", MessageBoxButton.OK, MessageBoxImage.Information);
            string selectedDir2 = CommonMethods.SelectDirOrFile(true);
            List<string> foundBackups1 = CommonMethods.FindBackupsInDirectory(selectedDir1);
            List<string> foundBackups2 = CommonMethods.FindBackupsInDirectory(selectedDir2);
            List<SpotComparerClass> backupPairs = GetBackupPair(foundBackups1, foundBackups2);
            Task<IDictionary<string, List<PointsPair>>> pointsDict = GetPointPairsAsync(backupPairs);
            await Task.WhenAll(pointsDict);
            IDictionary<string, List<PointsPair>> filteredPointsDict = FilterPointsDict(pointsDict.Result);
            if (filteredPointsDict.Count > 0)
                WriteExcel(filteredPointsDict);
            else
                MessageBox.Show("No difference between backups found", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void WriteExcel(IDictionary<string, List<PointsPair>> filteredPointsDict)
        {
            try
            {
                int counter = 1;
                oXL = new Excel.Application();
                oWBs = oXL.Workbooks;
                oWB = oWBs.Add("");
                sheets = oWB.Sheets;
                Excel.Range aRange = null;
                oSheet = sheets[1] as Excel.Worksheet;
                foreach (var robot in filteredPointsDict)
                {

                    int rowcounter = 1;
                    if (counter > 1)
                        oWB.Worksheets.Add(oSheet);
                    oSheet = oWB.ActiveSheet;
                    oSheet.Name = robot.Key;
                    oSheet.Cells[rowcounter, 1] = "Point";
                    oSheet.Cells[rowcounter, 2] = "X1";
                    oSheet.Cells[rowcounter, 3] = "Y1";
                    oSheet.Cells[rowcounter, 4] = "Z1";
                    oSheet.Cells[rowcounter, 5] = "A1";
                    oSheet.Cells[rowcounter, 6] = "B1";
                    oSheet.Cells[rowcounter, 7] = "C1";
                    oSheet.Cells[rowcounter, 8] = "X2";
                    oSheet.Cells[rowcounter, 9] = "Y2";
                    oSheet.Cells[rowcounter, 10] = "Z2";
                    oSheet.Cells[rowcounter, 11] = "A2";
                    oSheet.Cells[rowcounter, 12] = "B2";
                    oSheet.Cells[rowcounter, 13] = "C2";
                    oSheet.Cells[rowcounter, 14] = "Diff X";
                    oSheet.Cells[rowcounter, 15] = "Diff Y";
                    oSheet.Cells[rowcounter, 16] = "Diff Z";
                    oSheet.Cells[rowcounter, 17] = "Diff A";
                    oSheet.Cells[rowcounter, 18] = "Diff B";
                    oSheet.Cells[rowcounter, 19] = "Diff C";
                    rowcounter++;
                    foreach (var point in robot.Value)
                    {
                        oSheet.Cells[rowcounter, 1] = point.Name;
                        oSheet.Cells[rowcounter, 2] = point.Point1.Xpos;
                        oSheet.Cells[rowcounter, 3] = point.Point1.Ypos;
                        oSheet.Cells[rowcounter, 4] = point.Point1.Zpos;
                        oSheet.Cells[rowcounter, 5] = point.Point1.A;
                        oSheet.Cells[rowcounter, 6] = point.Point1.B;
                        oSheet.Cells[rowcounter, 7] = point.Point1.C;
                        oSheet.Cells[rowcounter, 8] = point.Point2.Xpos;
                        oSheet.Cells[rowcounter, 9] = point.Point2.Ypos;
                        oSheet.Cells[rowcounter, 10] = point.Point2.Zpos;
                        oSheet.Cells[rowcounter, 11] = point.Point2.A;
                        oSheet.Cells[rowcounter, 12] = point.Point2.B;
                        oSheet.Cells[rowcounter, 13] = point.Point2.C;
                        oSheet.Cells[rowcounter, 14] = Math.Abs(point.Point1.Xpos - point.Point2.Xpos);
                        oSheet.Cells[rowcounter, 15] = Math.Abs(point.Point1.Ypos - point.Point2.Ypos);
                        oSheet.Cells[rowcounter, 16] = Math.Abs(point.Point1.Zpos - point.Point2.Zpos);
                        oSheet.Cells[rowcounter, 17] = Math.Abs(point.Point1.A - point.Point2.A);
                        oSheet.Cells[rowcounter, 18] = Math.Abs(point.Point1.B - point.Point2.B);
                        oSheet.Cells[rowcounter, 19] = Math.Abs(point.Point1.C - point.Point2.C);
                        rowcounter++;
                    }
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    ReadSpotsMethods.FormatAsTable(aRange, robot.Key, "TableStyleMedium15");

                    Excel.Range range = oSheet.Range["N2","P"+(rowcounter-1)];
                    Excel.FormatConditions fcs = range.FormatConditions;
                    Excel.FormatCondition condition = (Excel.FormatCondition)fcs.Add(
                        Type: Excel.XlFormatConditionType.xlCellValue,
                        Operator: Excel.XlFormatConditionOperator.xlGreater,
                        Formula1: 0.5);
                    condition.Interior.ColorIndex = 3;

                    range = oSheet.Range["Q2", "S" + (rowcounter - 1)];
                    fcs = range.FormatConditions;
                    condition = (Excel.FormatCondition)fcs.Add(
                        Type: Excel.XlFormatConditionType.xlCellValue,
                        Operator: Excel.XlFormatConditionOperator.xlGreater,
                        Formula1: 0.1);
                    condition.Interior.ColorIndex = 3;
                    counter++;
                }
                oXL.Visible = true;
                if (aRange!=null)
                    Marshal.FinalReleaseComObject(aRange);
                Marshal.FinalReleaseComObject(oSheet);
                Marshal.FinalReleaseComObject(oWB);
                Marshal.FinalReleaseComObject(oWBs);
                Marshal.FinalReleaseComObject(oXL);
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
            }

        }

        private static IDictionary<string, List<PointsPair>> FilterPointsDict(IDictionary<string, List<PointsPair>> pointsDict)
        {
            try
            {
                IDictionary<string, List<PointsPair>> result = new Dictionary<string, List<PointsPair>>();
                foreach (var robot in pointsDict)
                {
                    List<PointsPair> pointsToAdd = new List<PointsPair>();
                    foreach (var pointPair in robot.Value)
                    {
                        if (Math.Abs(pointPair.Point1.Xpos - pointPair.Point2.Xpos) > 0.5 || Math.Abs(pointPair.Point1.Ypos - pointPair.Point2.Ypos) > 0.5 || Math.Abs(pointPair.Point1.Zpos - pointPair.Point2.Zpos) > 0.5 || Math.Abs(pointPair.Point1.A - pointPair.Point2.A) > 0.1 || Math.Abs(pointPair.Point1.B - pointPair.Point2.B) > 0.1 || Math.Abs(pointPair.Point1.C - pointPair.Point2.C) > 0.1)
                            pointsToAdd.Add(pointPair);

                    }
                    if (pointsToAdd.Count > 0)
                        result.Add(robot.Key, pointsToAdd);
                }
                return result;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                return null;
            }
        }

        private static async Task<IDictionary<string, List<PointsPair>>> GetPointPairsAsync(List<SpotComparerClass> backupPairs)
        {
            try
            {
                IDictionary<string, List<PointsPair>> result = new SortedDictionary<string, List<PointsPair>>();
                IDictionary<string, List<PointKUKA>> pointsWithoutPair = new Dictionary<string, List<PointKUKA>>();
                foreach (var backupPair in backupPairs)
                {
                    List<string> backup1 = new List<string>() { backupPair.Backup1 };
                    IDictionary<string, IWeldpoint> resultWeldPoints1 = new Dictionary<string, IWeldpoint>();
                    IDictionary<string, WeldpointBMW> weldPoints1 = await ReadSpotsMethods.FindWeldPoints(backup1);
                    IDictionary<string, List<SpotsInFile>> usedSpots1 = ReadSpotsMethods.FindUsedPointsInSrcFiles(backup1);
                    IDictionary<string, WeldpointBMW> usedSpotsDict1 = ReadSpotsMethods.CreateDictWithUsedPoints(usedSpots1);
                    var tempPoints = ReadSpotsMethods.CompareGlobalAndSrc(weldPoints1 as dynamic, usedSpotsDict1);
                    resultWeldPoints1 = ReadSpotsMethods.ConvertToIWeld(tempPoints);
                    //resultWeldPoints1 = ReadSpotsMethods.CompareGlobalAndSrc(weldPoints1 as dynamic, usedSpotsDict1);
                    resultWeldPoints1 = ReadSpotsMethods.FindLocalCoords(resultWeldPoints1, Path.GetDirectoryName(backupPair.Backup1));

                    List<string> backup2 = new List<string>() { backupPair.Backup2 };
                    IDictionary<string, IWeldpoint> resultWeldPoints2 = new Dictionary<string, IWeldpoint>();
                    IDictionary<string, WeldpointBMW> weldPoints2 = await ReadSpotsMethods.FindWeldPoints(backup2);
                    IDictionary<string, List<SpotsInFile>> usedSpots2 = ReadSpotsMethods.FindUsedPointsInSrcFiles(backup2);
                    IDictionary<string, WeldpointBMW> usedSpotsDict2 = ReadSpotsMethods.CreateDictWithUsedPoints(usedSpots2);
                    var tempPoints2 = ReadSpotsMethods.CompareGlobalAndSrc(weldPoints2 as dynamic, usedSpotsDict2);
                    resultWeldPoints2 = ReadSpotsMethods.ConvertToIWeld(tempPoints2);
                    resultWeldPoints2 = ReadSpotsMethods.FindLocalCoords(resultWeldPoints2, Path.GetDirectoryName(backupPair.Backup2));

                    if (resultWeldPoints1.Count > 0 || resultWeldPoints2.Count > 0)
                    {
                        result.Add(Path.GetFileName(backupPair.Backup1), new List<PointsPair>());
                        pointsWithoutPair.Add(Path.GetFileName(backupPair.Backup1), new List<PointKUKA>());
                        IDictionary<string, IWeldpoint> copyOfWeldPoints2 = new Dictionary<string, IWeldpoint>();
                        foreach (var item in resultWeldPoints2)
                            copyOfWeldPoints2.Add(item);
                        foreach (var point in resultWeldPoints1)
                        {
                            var pointCopy = point;
                            var pointInOtherBackup = resultWeldPoints2.FirstOrDefault(x => x.Key.Replace("_LOCAL","").ToLower() == point.Key.Replace("_LOCAL", "").ToLower());
                            if (pointInOtherBackup.Key != null)
                            {
                                result[Path.GetFileName(backupPair.Backup1)].Add(new PointsPair(pointCopy.Key, new PointKUKA(pointCopy.Value.XPos, pointCopy.Value.YPos, pointCopy.Value.ZPos, pointCopy.Value.A, pointCopy.Value.B, pointCopy.Value.C), new PointKUKA(pointInOtherBackup.Value.XPos, pointInOtherBackup.Value.YPos, pointInOtherBackup.Value.ZPos, pointInOtherBackup.Value.A, pointInOtherBackup.Value.B, pointInOtherBackup.Value.C)));
                                if (copyOfWeldPoints2.Keys.Contains(pointCopy.Key))
                                    copyOfWeldPoints2.Remove(copyOfWeldPoints2.First(x => x.Key == pointCopy.Key));
                                else
                                {
                                    if (copyOfWeldPoints2.Keys.Contains(pointCopy.Key + "_LOCAL"))
                                        copyOfWeldPoints2.Remove(copyOfWeldPoints2.First(x => x.Key == pointCopy.Key + "_LOCAL"));
                                    else
                                        copyOfWeldPoints2.Remove(copyOfWeldPoints2.First(x => x.Key == pointCopy.Key.Replace("_LOCAL","")));
                                }
                            }
                            else
                            {
                                pointsWithoutPair[Path.GetFileName(backupPair.Backup1)].Add(new PointKUKA(pointCopy.Value.XPos, pointCopy.Value.YPos, pointCopy.Value.ZPos, pointCopy.Value.A, pointCopy.Value.B, pointCopy.Value.C));
                            }
                        }
                        foreach (var point in copyOfWeldPoints2)
                        {
                            var copyOfPoit = point;
                            pointsWithoutPair[Path.GetFileName(backupPair.Backup1)].Add(new PointKUKA(copyOfPoit.Value.XPos, copyOfPoit.Value.YPos, copyOfPoit.Value.ZPos, copyOfPoit.Value.A, copyOfPoit.Value.B, copyOfPoit.Value.C));
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                return null;
            }
        }

        private static List<SpotComparerClass> GetBackupPair(List<string> foundBackups1, List<string> foundBackups2)
        {
            List<SpotComparerClass> result = new List<SpotComparerClass>();
            List<string> copyoffoundBackups2 = new List<string>();
            List<string> backupsWithoutPair = new List<string>();
            foundBackups2.ForEach(x => copyoffoundBackups2.Add(x));
            foreach (var backup in foundBackups1)
            {
                var backupPair = foundBackups2.FirstOrDefault(x => Path.GetFileName(x).ToLower() == Path.GetFileName(backup).ToLower());
                if (backupPair == null)
                    backupsWithoutPair.Add(backup);
                else
                {
                    result.Add(new SpotComparerClass(backup, backupPair));
                    copyoffoundBackups2.Remove(backupPair);
                }
            }
            backupsWithoutPair.AddRange(copyoffoundBackups2);
            return result;
        }

    }
}
