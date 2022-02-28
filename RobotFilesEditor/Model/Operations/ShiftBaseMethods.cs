using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using CommonLibrary.RobKalDatCommon;
using RobotFilesEditor.Dialogs;
using RobotFilesEditor.Dialogs.BaseShifter;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations.DataClass;

namespace RobotFilesEditor.Model.Operations
{
    class SrcAndDatContents
    {
        public IDictionary<string, string> SrcFiles { get; set; }
        public IDictionary<string, string> DatFiles { get; set; }

        public SrcAndDatContents(IDictionary<string, string> srcFiles, IDictionary<string, string> datFiles)
        {
            SrcFiles = srcFiles;
            DatFiles = datFiles;
        }
    }

    class PointBaseAndCoords
    {
        public string Name { get; set; }
        public int BaseNum { get; set; }
        public string FDAT { get; set; }
        public string E6Pos { get; set; }
        public PointKUKA InitialPoint { get; private set; }
        public PointKUKA CalculatedPoint { get; set; }

        public PointBaseAndCoords(string name, string fdat, string e6pos)
        {
            Name = name;
            FDAT = fdat;
            E6Pos = e6pos;
            BaseNum = GetBaseNum(fdat);
            InitialPoint = CalculateInitialPoint(e6pos);
        }

        private int GetBaseNum(string fdat)
        {
            Regex getbaseNumRegex = new Regex(@"(?<=BASE_NO\s+)\d+", RegexOptions.IgnoreCase);
            return int.Parse(getbaseNumRegex.Match(fdat).ToString());
        }

        private PointKUKA CalculateInitialPoint(string e6pos)
        {
            if (string.IsNullOrEmpty(e6pos) || e6pos.ToLower().Contains("e6axis"))
                return null;
            Regex getPointCoorsRegex = new Regex(@"(?<=(X|Y|Z|A|B|C)\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            var matches = getPointCoorsRegex.Matches(e6pos);
            return new PointKUKA(double.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), double.Parse(matches[5].ToString(), CultureInfo.InvariantCulture));
        }
    }

    public class SrcDatPair
    {
        public List<string> SrcFolds { get; set; }
        public string DatContent { get; set; }

        public SrcDatPair(List<string> srcFolds, string datContent)
        {
            SrcFolds = srcFolds;
            DatContent = datContent;
        }
    }

    public static class ShiftBaseMethods
    {
        internal static void Execute()
        {
            MessageBox.Show("Select backup file.", "Select", MessageBoxButton.OK, MessageBoxImage.Information);
            string backupfile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "Zip file", "*.zip");
            if (string.IsNullOrEmpty(backupfile))
                return;
            SrcAndDatContents dataToProcess = GetValidationDataFromBackup(backupfile);
            CommonLibrary.FoundVariables globals = CommonLibrary.CommonMethods.FindVarsInBackup(backupfile, true);
            IDictionary<string, List<string>> resultSrcFiles = SrcValidator.DivideToFolds(dataToProcess.SrcFiles);
            IDictionary<int,BaseDataKUKA> basesInConfigDat = GetBasesFromConfigDat(dataToProcess.DatFiles.FirstOrDefault(x=>x.Key.ToLower().Contains("$config.dat") && x.Key.ToLower().Contains("system")).Value);
            IDictionary<string, SrcDatPair> srcDatPairs = CreateSrcDatPairs(resultSrcFiles, dataToProcess.DatFiles);
            SelectPathsToShiftBaseViewModel vm = new SelectPathsToShiftBaseViewModel(srcDatPairs);
            SelectPathsToShiftBase sW = new SelectPathsToShiftBase(vm);
            var dialog = sW.ShowDialog();
            if (dialog == false)
                return;
            var selectedItems = sW.MyListView.SelectedItems;
            foreach (KeyValuePair<string,SrcDatPair> filePair in selectedItems)
            {
                List<PointBaseAndCoords> pointsFound = ScanInputFiles(filePair, globals);
                List<int> usedBases = new List<int>();
                IDictionary<int, BaseDataKUKA> baseToBeShifted = new Dictionary<int, BaseDataKUKA>();
                foreach (var basee in pointsFound.Where(x=>!usedBases.Contains(x.BaseNum)))
                    usedBases.Add(basee.BaseNum);
                foreach (var basee in usedBases)
                {                    
                    BaseShifterViewModel vmShifter = new BaseShifterViewModel(basee, filePair.Key);
                    BaseShifterWindow sWShifter = new BaseShifterWindow(vmShifter);
                    var dialogShifter = sWShifter.ShowDialog();
                    if (dialogShifter == false)
                        return;
                    Regex getBaseCoorsRegex = new Regex(@"(?<=(X|Y|Z|A|B|C)\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                    var matches = getBaseCoorsRegex.Matches(vmShifter.BaseData);
                    if (matches.Count != 6)
                    {
                        MessageBox.Show("Base data:\r\n" + vmShifter.BaseData + "\r\nIs incorrect. Program will abort", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    baseToBeShifted.Add(basee, new BaseDataKUKA(float.Parse(matches[0].ToString(),CultureInfo.InvariantCulture), float.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[5].ToString(), CultureInfo.InvariantCulture)));
                }
                string resultDatFile = string.Empty;
                StringReader reader = new StringReader(filePair.Value.DatContent);
                Regex pointNameRegex = new Regex(@"(?<=E6POS\s+)[\w_-]+", RegexOptions.IgnoreCase);
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (pointNameRegex.IsMatch(line))
                    {
                        PointBaseAndCoords pointToShift = pointsFound.First(x => x.Name.Equals(pointNameRegex.Match(line).ToString()));
                        resultDatFile += CalculateBaseShift(pointToShift, basesInConfigDat[pointToShift.BaseNum], baseToBeShifted[pointToShift.BaseNum]) + "\r\n";
                    }
                    else
                        resultDatFile += line + "\r\n";
                }
                reader.Close();
                SaveResults(Path.GetDirectoryName(backupfile), filePair, resultDatFile);
            }
        }

        private static string CalculateBaseShift(PointBaseAndCoords pointToShift, BaseDataKUKA initialBase, BaseDataKUKA finalBase)
        {
            string result = pointToShift.E6Pos.Trim();
            // TODO
            // KALKULACJE
            var pointCalc = CommonLibrary.CommonMethods.CalculateBases(ToCommonPoint(initialBase), ToCommonPoint(finalBase), ToCommonPoint(pointToShift.InitialPoint));

            PointKUKA point = ToPointKuka(pointCalc);
            Regex regexX = new Regex(@"(?<=\{\s*X\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexY = new Regex(@"(?<=,\s*Y\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexZ = new Regex(@"(?<=,\s*Z\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexA = new Regex(@"(?<=,\s*A\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexB = new Regex(@"(?<=,\s*B\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexC = new Regex(@"(?<=,\s*C\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);

            result = regexX.Replace(result, point.Xpos.ToString(CultureInfo.InvariantCulture));
            result = regexY.Replace(result, point.Ypos.ToString(CultureInfo.InvariantCulture));
            result = regexZ.Replace(result, point.Zpos.ToString(CultureInfo.InvariantCulture));
            result = regexA.Replace(result, point.A.ToString(CultureInfo.InvariantCulture));
            result = regexB.Replace(result, point.B.ToString(CultureInfo.InvariantCulture));
            result = regexC.Replace(result, point.C.ToString(CultureInfo.InvariantCulture));

            return result;
        }

        private static PointKUKA ToPointKuka(CommonLibrary.RobKalDatCommon.Point pointCalc)
        {
            return new PointKUKA(pointCalc.XPos, pointCalc.YPos, pointCalc.ZPos, pointCalc.RZ, pointCalc.RY, pointCalc.RX);
        }

        private static CommonLibrary.RobKalDatCommon.Point ToCommonPoint(dynamic initialBase)
        {
            if (initialBase is BaseDataKUKA)
                return new CommonLibrary.RobKalDatCommon.Point((initialBase as BaseDataKUKA).Xpos, (initialBase as BaseDataKUKA).Ypos, (initialBase as BaseDataKUKA).Zpos, (initialBase as BaseDataKUKA).C, (initialBase as BaseDataKUKA).B, (initialBase as BaseDataKUKA).A);
            if (initialBase is PointKUKA)
                return new CommonLibrary.RobKalDatCommon.Point((initialBase as PointKUKA).Xpos, (initialBase as PointKUKA).Ypos, (initialBase as PointKUKA).Zpos, (initialBase as PointKUKA).C, (initialBase as PointKUKA).B, (initialBase as PointKUKA).A);
            return null;
        }

        private static SrcAndDatContents GetValidationDataFromBackup(string backupfile)
        {
            IDictionary<string, string> srcFiles = new Dictionary<string, string>();
            IDictionary<string, string> datFiles = new Dictionary<string, string>();
            using (ZipArchive archive = ZipFile.Open(backupfile, ZipArchiveMode.Read))
            {
                archive.Entries.ToList().Where(x => Path.GetExtension(x.Name).ToLower() == ".src").ToList().ForEach(y => srcFiles.Add(y.FullName, new StreamReader(y.Open()).ReadToEnd()));
                archive.Entries.ToList().Where(x => Path.GetExtension(x.Name).ToLower() == ".dat").ToList().ForEach(y => datFiles.Add(y.FullName, new StreamReader(y.Open()).ReadToEnd()));
            }
            return new SrcAndDatContents(srcFiles,datFiles);
        }

        private static IDictionary<int,BaseDataKUKA> GetBasesFromConfigDat(string configDatContent)
        {
            Regex baseDataRegex = new Regex(@"(?<=BASE_DATA.*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex baseTypeRegex = new Regex(@"(?<=BASE_TYPE\s*\[\s*)\d+(?=.*#.+)", RegexOptions.IgnoreCase);
            IDictionary<int, BaseDataKUKA> result = new SortedDictionary<int, BaseDataKUKA>();
            StringReader reader = new StringReader(configDatContent);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (baseDataRegex.IsMatch(line))
                {
                    var items = baseDataRegex.Matches(line);
                    if (items.Count == 7)
                        result.Add(int.Parse(items[0].ToString()), new BaseDataKUKA(float.Parse(items[1].ToString(), CultureInfo.InvariantCulture), float.Parse(items[2].ToString(), CultureInfo.InvariantCulture), float.Parse(items[3].ToString(), CultureInfo.InvariantCulture), float.Parse(items[4].ToString(), CultureInfo.InvariantCulture), float.Parse(items[5].ToString(), CultureInfo.InvariantCulture), float.Parse(items[6].ToString(), CultureInfo.InvariantCulture)));
                }
                if (baseTypeRegex.IsMatch(line))
                {
                    int baseNum = int.Parse(baseTypeRegex.Match(line).ToString());
                    if (line.ToLower().Contains("#none"))
                        result.Remove(baseNum);
                }
            }
            reader.Close();
            return result;
        }

        private static IDictionary<string, SrcDatPair> CreateSrcDatPairs(IDictionary<string, List<string>> srcFiles, IDictionary<string, string> datFiles)
        {
            IDictionary<string, SrcDatPair> result = new Dictionary<string, SrcDatPair>();
            foreach (var src in srcFiles)
            {
                var complementaryDat = datFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.Key).ToLower() == Path.GetFileNameWithoutExtension(src.Key).ToLower());
                if (complementaryDat.Key != null)
                {
                    if (ProcedureContainsMotion(src.Value))
                        result.Add(Path.GetFileNameWithoutExtension(src.Key), new SrcDatPair(src.Value, complementaryDat.Value));
                }
            }
            return result;
        }

        private static bool ProcedureContainsMotion(List<string> folds)
        {
            Regex isMotionRegex = new Regex(@"(PTP|LIN)\s+[\w_-]+", RegexOptions.IgnoreCase);
            foreach (var fold in folds)
            {
                if (isMotionRegex.IsMatch(fold))
                    return true;
            }
            return false;
        }

        private static List<PointBaseAndCoords> ScanInputFiles(KeyValuePair<string,SrcDatPair> filePair, CommonLibrary.FoundVariables globals)
        {
            PointBaseAndCoords point;
            Regex isMotionRegex = new Regex(@"(?<=^(PTP|LIN)\s+)[\w_-]+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            List<PointBaseAndCoords> result = new List<PointBaseAndCoords>();
            foreach (var fold in filePair.Value.SrcFolds.Where(x => isMotionRegex.IsMatch(x)))
            {
                point = null;
                string e6PosForCtor = string.Empty, fdatForCtor = string.Empty;

                string e6PosPointName = isMotionRegex.Match(fold).ToString();
                string fdat = (new Regex(@"(?<=^FDAT_ACT\s*\=\s*)[\w_-]+", RegexOptions.IgnoreCase | RegexOptions.Multiline).Match(fold).ToString());
                string e6PosInDat = (new Regex("^.*e6pos\\s+" + e6PosPointName + ".*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Match(filePair.Value.DatContent).ToString();
                string fdatInDat = (new Regex("^.*fdat\\s+" + fdat + ".*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Match(filePair.Value.DatContent).ToString();
                if (string.IsNullOrEmpty(e6PosInDat))
                {
                    if (globals.E6AXIS.Values.Any(x => e6PosPointName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                        e6PosForCtor = globals.E6AXIS.Values.First(x => e6PosPointName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).Line;
                    else if (globals.E6POS.Values.Any(x => e6PosPointName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                        e6PosForCtor = globals.E6POS.Values.First(x => e6PosPointName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).Line;
                }
                else
                    e6PosForCtor = e6PosInDat;
                if (string.IsNullOrEmpty(fdatInDat))
                {
                    if (globals.FDATs.Values.Any(x => e6PosPointName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                        fdatForCtor = globals.FDATs.Values.First(x => e6PosPointName.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).Line;
                }
                else
                    fdatForCtor = fdatInDat;
                result.Add(new PointBaseAndCoords(isMotionRegex.Match(fold).ToString(), fdatForCtor, e6PosForCtor));
            }
            return result;
        }

        private static void SaveResults(string dirToSave, KeyValuePair<string, SrcDatPair> filePair, string resultDatFile)
        {
            try
            {
                dirToSave = Path.Combine(dirToSave, "ShiftedBases");
                if (!Directory.Exists(dirToSave))
                    Directory.CreateDirectory(dirToSave);
                List<string> lines = new List<string>();
                if (File.Exists(Path.Combine(dirToSave, filePair.Key + ".src")))
                    File.Delete(Path.Combine(dirToSave, filePair.Key + ".src"));
                File.WriteAllLines(Path.Combine(dirToSave, filePair.Key + ".src"), lines);
                if (File.Exists(Path.Combine(dirToSave, filePair.Key + ".dat")))
                    File.Delete(Path.Combine(dirToSave, filePair.Key + ".dat"));
                File.WriteAllText(Path.Combine(dirToSave, filePair.Key + ".dat"), resultDatFile);
                MessageBox.Show("Files saved succesfully at: " + dirToSave, "Success",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
