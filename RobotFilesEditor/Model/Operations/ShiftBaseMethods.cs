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
using MathNet.Numerics.LinearAlgebra;

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
        internal static void Execute(string controller)
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
            List<PointBaseAndCoords> globalPoints = new List<PointBaseAndCoords>();
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
                        PointBaseAndCoords pointToShift;
                        string pointName = pointNameRegex.Match(line).ToString();
                        pointToShift = pointsFound.FirstOrDefault(x => x.Name.Equals(pointName));
                        if (pointToShift != null)
                            resultDatFile += CalculateBaseShift(pointToShift, basesInConfigDat[pointToShift.BaseNum], baseToBeShifted[pointToShift.BaseNum]) + "\r\n";
                        pointsFound.Remove(pointToShift);
                    }
                    else
                        resultDatFile += line + "\r\n";
                }
                pointsFound.Where(x => x.InitialPoint != null).ToList().ForEach(y => globalPoints.Add(y));
                reader.Close();
                SaveResults(Path.GetDirectoryName(backupfile), filePair, resultDatFile);
                globals.GlobalDat = UpdateGlobalFile(globalPoints, globals.GlobalDat, basesInConfigDat, baseToBeShifted);
            }
            if (globals.GlobalDat.Count > 0)
            {

            }

            
        }

        private static Dictionary<string,string> UpdateGlobalFile(List<PointBaseAndCoords> globalPoints, Dictionary<string, string> globalDat, IDictionary<int, BaseDataKUKA> basesInConfigDat, IDictionary<int, BaseDataKUKA> baseToBeShifted)
        {
            Dictionary<string, string> result = globalDat;
            foreach (var globalPoint in globalPoints)
            {
                var globalFileContaining = result.FirstOrDefault(x => x.Value.ToLower().Contains(globalPoint.Name.ToLower()));
                string e6PosLine = new Regex(@"^.*GLOBAL\s+E6POS\s+" +globalPoint.Name+ @".*$", RegexOptions.Multiline | RegexOptions.IgnoreCase).Match(globalFileContaining.Value).ToString();
                string modifiedGlobal = globalFileContaining.Value.Replace(e6PosLine, CalculateBaseShift(globalPoint, basesInConfigDat[globalPoint.BaseNum], baseToBeShifted[globalPoint.BaseNum]));
                result[globalFileContaining.Key] = modifiedGlobal;
            }

            return result;
        }

        private static string CalculateBaseShift(PointBaseAndCoords pointToShift, BaseDataKUKA initialBase, BaseDataKUKA finalBase)
        {
            string result = pointToShift.E6Pos.Trim();

            Matrix<double> htmInintialBase = BuildHTMMatrix(initialBase);
            Matrix<double> htmPointToShift = BuildHTMMatrix(ConvertToBaseDataKuka(pointToShift.E6Pos));
            Matrix<double> htmFinalBase = BuildHTMMatrix(finalBase);

            Matrix<double> base0HTM = htmInintialBase.Multiply(htmPointToShift);
            Matrix<double> htmFinalBaseInv = htmFinalBase.Inverse();
            Matrix<double> resultPoint = htmFinalBaseInv.Multiply(base0HTM);

            PointKUKA pointCalc = GetCalculatedPoint(resultPoint);

            Regex regexX = new Regex(@"(?<=\{\s*X\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexY = new Regex(@"(?<=,\s*Y\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexZ = new Regex(@"(?<=,\s*Z\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexA = new Regex(@"(?<=,\s*A\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexB = new Regex(@"(?<=,\s*B\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex regexC = new Regex(@"(?<=,\s*C\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);

            result = regexX.Replace(result, pointCalc.Xpos.ToString("0.##", CultureInfo.InvariantCulture));
            result = regexY.Replace(result, pointCalc.Ypos.ToString("0.##", CultureInfo.InvariantCulture));
            result = regexZ.Replace(result, pointCalc.Zpos.ToString("0.##", CultureInfo.InvariantCulture));
            result = regexA.Replace(result, pointCalc.A.ToString("0.###", CultureInfo.InvariantCulture));
            result = regexB.Replace(result, pointCalc.B.ToString("0.###", CultureInfo.InvariantCulture));
            result = regexC.Replace(result, pointCalc.C.ToString("0.###", CultureInfo.InvariantCulture));

            return result;
        }

        private static BaseDataKUKA ConvertToBaseDataKuka(string e6Pos)
        {
            Regex regex = new Regex(@"((?<=X )-[0-9]*\.[0-9]*)|((?<=X )-[0-9]*)|((?<=X )[0-9]*\.[0-9]*|((?<=X )[0-9]*)|(?<=Y )-[0-9]*\.[0-9]*)|((?<=Y )-[0-9]*)|((?<=Y )[0-9]*\.[0-9]*|((?<=Y )[0-9]*)|(?<=Z )-[0-9]*\.[0-9]*)|((?<=Z )-[0-9]*)|((?<=Z )[0-9]*\.[0-9]*|((?<=Z )[0-9]*)|(?<=A )-[0-9]*\.[0-9]*)|((?<=A )-[0-9]*)|((?<=A )[0-9]*\.[0-9]*|((?<=A )[0-9]*)|(?<=B )-[0-9]*\.[0-9]*)|((?<=B )-[0-9]*)|((?<=B )[0-9]*\.[0-9]*|((?<=B )[0-9]*)|(?<=C )-[0-9]*\.[0-9]*)|((?<=C )-[0-9]*)|((?<=C )[0-9]*\.[0-9]*|((?<=C )[0-9]*))", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(e6Pos);
            BaseDataKUKA currentData = new BaseDataKUKA(float.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[5].ToString(), CultureInfo.InvariantCulture));
            return currentData;
        }

        private static PointKUKA GetCalculatedPoint(Matrix<double> resultPoint)
        {
            PointKUKA result = new PointKUKA();
            result.Xpos = resultPoint[0, 3];
            result.Ypos = resultPoint[1, 3];
            result.Zpos = resultPoint[2, 3];
            result.C = CommonLibrary.CommonMethods.ConvertToDegrees(-0.01 < resultPoint[2, 1] && resultPoint[2, 1] < 0.01 && -0.01 < resultPoint[2, 2] && resultPoint[2, 2] < 0.01 ? 0 : Math.Atan2(resultPoint[2, 1], resultPoint[2, 2]));
            result.B = CommonLibrary.CommonMethods.ConvertToDegrees(Math.Asin(-resultPoint[2, 0]));
            result.A = CommonLibrary.CommonMethods.ConvertToDegrees(-0.01 < resultPoint[1, 0] && resultPoint[1, 0] < 0.01 && -0.01 < resultPoint[0, 0] && resultPoint[0, 0] < 0.01 ? 0 : Math.Atan2(resultPoint[1, 0], resultPoint[0, 0]));

            return result;
        }

        private static Matrix<double> BuildHTMMatrix(BaseDataKUKA frame)
        {
            Matrix<double> frameRotMatZ = BuildRotationMatrix("Z", frame.A);
            Matrix<double> frameRotMatY = BuildRotationMatrix("Y", frame.B);
            Matrix<double> frameRotMatX = BuildRotationMatrix("X", frame.C);
            Matrix<double> frameRotMatZYX = frameRotMatZ.Multiply(frameRotMatY).Multiply(frameRotMatX);

            Matrix<double> resultMatrix = Matrix<double>.Build.Dense(4, 4);
            resultMatrix[0, 0] = frameRotMatZYX[0, 0];
            resultMatrix[0, 1] = frameRotMatZYX[0, 1];
            resultMatrix[0, 2] = frameRotMatZYX[0, 2];
            resultMatrix[1, 0] = frameRotMatZYX[1, 0];
            resultMatrix[1, 1] = frameRotMatZYX[1, 1];
            resultMatrix[1, 2] = frameRotMatZYX[1, 2];
            resultMatrix[2, 0] = frameRotMatZYX[2, 0];
            resultMatrix[2, 1] = frameRotMatZYX[2, 1];
            resultMatrix[2, 2] = frameRotMatZYX[2, 2];

            resultMatrix[0, 3] = frame.Xpos;
            resultMatrix[1, 3] = frame.Ypos;
            resultMatrix[2, 3] = frame.Zpos;
            resultMatrix[3, 0] = 0;
            resultMatrix[3, 1] = 0;
            resultMatrix[3, 2] = 0;
            resultMatrix[3, 3] = 1;

            return resultMatrix;

        }

        private static Matrix<double> BuildRotationMatrix(string axis, double rotationValueInDegress)
        {
            double rotationValueInRad = CommonLibrary.CommonMethods.ConvertToRadians(rotationValueInDegress);
            Matrix<double> resultMatrix = Matrix<double>.Build.Dense(3, 3);
            switch (axis)
            {
                case "X":
                    resultMatrix[0, 0] = 1;
                    resultMatrix[0, 1] = 0;
                    resultMatrix[0, 2] = 0;
                    resultMatrix[1, 0] = 0;
                    resultMatrix[1, 1] = Math.Cos(rotationValueInRad);
                    resultMatrix[1, 2] = -Math.Sin(rotationValueInRad);
                    resultMatrix[2, 0] = 0;
                    resultMatrix[2, 1] = Math.Sin(rotationValueInRad);
                    resultMatrix[2, 2] = Math.Cos(rotationValueInRad);
                    break;
                case "Y":
                    resultMatrix[0, 0] = Math.Cos(rotationValueInRad);
                    resultMatrix[0, 1] = 0;
                    resultMatrix[0, 2] = Math.Sin(rotationValueInRad);
                    resultMatrix[1, 0] = 0;
                    resultMatrix[1, 1] = 1;
                    resultMatrix[1, 2] = 0;
                    resultMatrix[2, 0] = -Math.Sin(rotationValueInRad);
                    resultMatrix[2, 1] = 0;
                    resultMatrix[2, 2] = Math.Cos(rotationValueInRad);
                    break;
                case "Z":
                    resultMatrix[0, 0] = Math.Cos(rotationValueInRad);
                    resultMatrix[0, 1] = -Math.Sin(rotationValueInRad);
                    resultMatrix[0, 2] = 0;
                    resultMatrix[1, 0] = Math.Sin(rotationValueInRad);
                    resultMatrix[1, 1] = Math.Cos(rotationValueInRad);
                    resultMatrix[1, 2] = 0;
                    resultMatrix[2, 0] = 0;
                    resultMatrix[2, 1] = 0;
                    resultMatrix[2, 2] = 1;
                    break;
            }
            return resultMatrix;
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
            Regex replaceFWithXRegex = new Regex(@"^\s*F", RegexOptions.IgnoreCase);
            Regex reloadRegex = new Regex(@"Reload\d+_\d+_\d+", RegexOptions.IgnoreCase);
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
                    if (globals.FDATs.Values.Any(x => e6PosPointName.Equals(replaceFWithXRegex.Replace(x.Name,"X"), StringComparison.OrdinalIgnoreCase)))
                        fdatForCtor = globals.FDATs.Values.First(x => e6PosPointName.Equals(replaceFWithXRegex.Replace(x.Name, "X"), StringComparison.OrdinalIgnoreCase)).Line;
                }
                else
                    fdatForCtor = fdatInDat;
                result.Add(new PointBaseAndCoords(isMotionRegex.Match(fold).ToString(), fdatForCtor, e6PosForCtor));
                if (reloadRegex.IsMatch(fold))
                {
                    List<string> fdatReloads = GetReloadVar("FDAT", fold);
                    List<string> e6PosReloads = GetReloadVar("E6POS", fold);
                    int counter = 0;
                    foreach (var fdatReload in fdatReloads)
                    {
                        string e6PosInDatReload = (new Regex("^.*e6pos\\s+" + e6PosReloads[counter] + ".*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Match(filePair.Value.DatContent).ToString();
                        string fdatInDatReload = (new Regex("^.*fdat\\s+" + fdatReload + ".*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)).Match(filePair.Value.DatContent).ToString();
                        result.Add(new PointBaseAndCoords(e6PosReloads[counter], fdatInDatReload, e6PosInDatReload));
                        counter++;
                    }
                }
            }
            return result;
        }

        private static List<string> GetReloadVar(string type, string fold)
        {
            Regex getVarNameRegex = new Regex(type == "FDAT" ? @"F[\w_]+Reload\d+_\d+_\d+" : @"X[\w_]+Reload\d+_\d+_\d+", RegexOptions.IgnoreCase);
            MatchCollection matches = getVarNameRegex.Matches(fold);
            List<string> result = new List<string>();
            foreach (var match in matches)
                result.Add(match.ToString());
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
