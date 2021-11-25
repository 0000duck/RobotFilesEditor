using RobotFilesEditor.Dialogs;
using RobotFilesEditor.Model.DataInformations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonLibrary;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;

namespace RobotFilesEditor.Model.Operations
{

    public class TypIdChangeMethods : BaseClasses
    {
        private static string errorMessage;
        private static volatile LoadingMessageVM loadVM;
        private static volatile LoadingMessage loadsW;

        internal async Task Execute()
        {
            try
            {
                string directory = CommonMethods.SelectDirOrFile(true);
                if (string.IsNullOrEmpty(directory))
                    return;
                bool checkKRC4 = CheckKRC4(directory);
                if (!CreateDirectories(directory, "CorrectTypID", true, false, true, false, checkKRC4))
                    return;

                string spotGlobal = GetSpotGlobal(directory);
                if (!string.IsNullOrEmpty(spotGlobal))
                {
                    loadVM = new LoadingMessageVM();
                    loadsW = new LoadingMessage(loadVM);
                    loadVM.Progress = 25;
                    loadsW.Show();
                    IDictionary<int, WeldpointBMW> spotPointsInSpotGlobal = FindWeldPoints(spotGlobal, Path.GetFileName(directory));
                    IDictionary<string, List<SpotsInFile>> usedSpots = await FindUsedPointsInSrcFiles(directory);
                    IDictionary<string, WeldpointBMW> usedSpotsDict = ReadSpotsMethods.CreateDictWithUsedPoints(usedSpots);
                    var templist = ReadSpotsMethods.CompareGlobalAndSrc(spotPointsInSpotGlobal as dynamic, usedSpotsDict);
                    IDictionary<string, WeldpointBMW> resultWeldPointsAsString = templist;
                    IDictionary<int, WeldpointBMW> resultWeldPoints = ConvertSrtToInt(resultWeldPointsAsString);
                    loadVM.Progress = 50;
                    TypIdChangerTypeWindowVM vm1 = new TypIdChangerTypeWindowVM();
                    TypIdChangerTypeWindow sw1 = new TypIdChangerTypeWindow(vm1);
                    var dialogResult = sw1.ShowDialog();
                    List<string> pointList = new List<string>();
                    if (vm1.Ispointbypoint)
                    {
                        ChangeTypIdViewModel vm = new ChangeTypIdViewModel(resultWeldPoints.ToList(),this);
                        ChangeTypIdList sW = new ChangeTypIdList(vm);
                        dialogResult = sW.ShowDialog();  // WT
                        pointList = vm.PointsList;
                    }
                    else
                    {
                        TypIDBySameIDWindowVM vm2 = new TypIDBySameIDWindowVM(resultWeldPoints.ToList());
                        TypIDBySameIDWindow sW2 = new TypIDBySameIDWindow(vm2);
                        dialogResult = sW2.ShowDialog(); // WT
                        pointList = vm2.PointsList;
                    }
                    loadVM.Progress = 75;

                    IDictionary<int, WeldpointBMW> changedSpots = new Dictionary<int, WeldpointBMW>();
                    if (dialogResult.Value == true)
                    {
                        changedSpots = UpdatePointsList(pointList, resultWeldPoints);
                        await UpdateDatFile(spotGlobal, changedSpots, directory + "\\CorrectTypID", true, checkKRC4);
                        await UpdateDatsFilesInProgram(directory, changedSpots, checkKRC4);
                        await UpdateSrcFiles(directory, changedSpots, checkKRC4);
                        //await Task.Run(() => UpdateProgress(100));
                        loadVM.Progress = 100;

                        MessageBox.Show("Files with new TypIDs have been saved at: \n\r" + directory + "\\CorrectTypID\\", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    loadsW.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        override public List<string> ReadFile(string file)
        {
            //List<string> result = new List<string>() { "TEST POLIMORFIZMU"};
            //result.AddRange(base.ReadFile(file));
            //return result;
            return base.ReadFile(file);
        }

        private bool CheckKRC4(string directory)
        {
            List<string> dirs = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories).ToList();
            if (dirs.Any(x => x.ToLower().Contains("bmw_app")))
                return true;
            return false;
        }

        private IDictionary<int, WeldpointBMW> ConvertSrtToInt(IDictionary<string, WeldpointBMW> resultWeldPointsAsString)
        {
            Regex splitTypId = new Regex(@"^\d+", RegexOptions.IgnoreCase);
            IDictionary<int, WeldpointBMW> result = new Dictionary<int, WeldpointBMW>();
            foreach (var item in resultWeldPointsAsString)
            {
                result.Add(int.Parse(splitTypId.Match(item.Key).ToString()), item.Value);
            }
            return result;
        }

        private async Task UpdateSrcFiles(string directory, IDictionary<int, WeldpointBMW> changedSpots, bool isKRC4)
        {
            string[] srcFiles;
            if (isKRC4)
                srcFiles = Directory.GetFiles(directory + "\\KRC\\R1\\BMW_Program", "*.src", SearchOption.AllDirectories);
            else
                srcFiles = Directory.GetFiles(directory + "\\KRC\\R1\\Program", "*.src", SearchOption.AllDirectories);
            foreach (string file in srcFiles)
            {
                await UpdateSrcFile(file, changedSpots, directory + "\\CorrectTypID", false, isKRC4);
            }
        }

        private async Task<bool> UpdateSrcFile(string file, IDictionary<int, WeldpointBMW> changedSpots, string saveDirectory, bool alwaysSave, bool isKRC4)
        {
            bool isChanged;
            bool changed = false;
            string resultFileContent = "";
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string outPdat = string.Empty;
                string outLdat = string.Empty;
                string outFdat = string.Empty;
                string outPtpt = string.Empty;
                string outDelta = string.Empty;
                string lineToAdd = "";
                string line = await reader.ReadLineAsync();
                string lineToLower = line.ToLower();

                // COMMON
                if (lineToLower.Contains("deltamfg") && lineToLower.ContainsAnyRet(new string[] { "lsp", "swp", "swr", "swh" }, out outDelta))
                {
                    lineToAdd = GetLineToAdd(outDelta, changedSpots, line, 9, out isChanged);
                    changed = changed == true ? true : isChanged;
                }


                else if (lineToLower.Contains("fold") && lineToLower.ContainsAny(new string[] { "spot" , "spotpoint" }) && (lineToLower.Contains("ptp") || lineToLower.Contains("lin")) && (lineToLower.Contains("typid") || lineToLower.Contains("typeid")))
                {
                    foreach (var point in changedSpots)
                    {
                        if (line.Contains(point.Key.ToString()))
                        {
                            Regex foldRegex = new Regex(@"(?<=TypID\s*=)\d*", RegexOptions.IgnoreCase);
                            if (foldRegex.IsMatch(line))
                            {
                                lineToAdd = Regex.Replace(line, @"TypID\s*=\s*\d*", "TypID=" + point.Value.TypId, RegexOptions.IgnoreCase);
                                if (int.Parse(foldRegex.Match(line).ToString()) != point.Value.TypId)
                                    changed = true;
                            }
                            else
                            {
                                foldRegex = new Regex(@"(?<=TypeId\s*:\s*)\d*", RegexOptions.IgnoreCase);
                                lineToAdd = Regex.Replace(line, @"TypeId\s*:\s*\d*", "TypeId:" + point.Value.TypId, RegexOptions.IgnoreCase);
                                if (int.Parse(foldRegex.Match(line).ToString()) != point.Value.TypId)
                                    changed = true;
                            }
                            break;
                        }
                    }
                }
                else if (lineToLower.Contains("swp_typeid") && lineToLower.Contains("swp_spotno"))
                {
                    Regex pointNumRegex = new Regex(@"(?<=swp_spotno\s*\=\s*)\d+", RegexOptions.IgnoreCase);
                    Regex typIdRegex = new Regex(@"(?<=swp_typeid\s*\=\s*)\d+", RegexOptions.IgnoreCase);
                    lineToAdd = typIdRegex.Replace(lineToLower, changedSpots[int.Parse(pointNumRegex.Match(lineToLower).ToString())].TypId.ToString());
                }
                else if ((lineToLower.Contains("ptp") && !lineToLower.Contains("reload_ptp") || lineToLower.Contains("lin") && !lineToLower.Contains("reload_lin")) && lineToLower.ContainsAnyRet(new string[] { "xlsp", "xswp", "xswh", "xswr" }, out outPtpt))
                {
                    lineToAdd = GetLineToAdd(outPtpt, changedSpots, line, 1, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if ((lineToLower.Contains("pdat_act") && lineToLower.ContainsAnyRet(new string[] { "plsp", "pswh", "pswp", "pswr", "preload" }, out outPdat)) || (lineToLower.Contains("ldat_act") && lineToLower.ContainsAnyRet(new string[] { "llsp", "lswh", "lswp", "lswr", "lreload" }, out outLdat)))
                {
                    string outParam = !string.IsNullOrEmpty(outPdat) ? outPdat : outLdat;
                    lineToAdd = GetLineToAdd(outParam, changedSpots, line, 3, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("fdat_act") && lineToLower.ContainsAnyRet(new string[] { "flsp", "fswp", "fswr", "fswh", "freload" }, out outFdat))
                {
                    lineToAdd = GetLineToAdd(outFdat, changedSpots, line, 2, out isChanged);
                    changed = changed == true ? true : isChanged;
                }

                // SWH
                else if (lineToLower.Contains("fold") && lineToLower.Contains("swh") && lineToLower.ContainsAny(new string[] { "studweld", "reloadpos" }) && lineToLower.ContainsAny(new string[] { "ptp", "lin" }) && lineToLower.Contains("typid"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 6, out isChanged);
                    changed = changed == true ? true : isChanged;
                }

                else if (lineToLower.Contains("swh_studno") && lineToLower.Contains("swh_typid"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 7, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("swh_reloadpos "))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 8, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("swh_pre"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 4, out isChanged);
                    changed = changed == true ? true : isChanged;
                }

                else if (lineToLower.Contains("swh_exe"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 5, out isChanged);
                    changed = changed == true ? true : isChanged;
                }

                else if (lineToLower.Contains("swp_exe"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 16, out isChanged);
                    changed = changed == true ? true : isChanged;
                }

                // SWR
                else if (lineToLower.Contains("fold") && lineToLower.Contains("swr") && lineToLower.ContainsAny(new string[] { "studweld", "reloadpos", "control", "studload" }) && lineToLower.ContainsAny(new string[] { "ptp", "lin" }) && lineToLower.Contains("typid"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 6, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("swr_studno") && lineToLower.Contains("swr_typid"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 10, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                //else if (lineToLower.Contains("swr_control") && !lineToLower.Contains("swr_control_after"))
                else if (lineToLower.ContainsAny(new string[] { "swr_control", "swr_studloadpre" }))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 11, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("swr_studload"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 15, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("swr_pre"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 13, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("swr_exe"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 12, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                else if (lineToLower.Contains("swr_reloadpos"))
                {
                    lineToAdd = GetLineToAdd(string.Empty, changedSpots, line, 14, out isChanged);
                    changed = changed == true ? true : isChanged;
                }
                // LSP
                else if (lineToLower.Contains("advspot"))
                {
                    foreach (var point in changedSpots)
                    {
                        if (line.Contains(point.Key.ToString()))
                        {
                            Regex advSpotRegex = new Regex(@"(?<=,)\d*(?=,)", RegexOptions.IgnoreCase);
                            lineToAdd = Regex.Replace(line, @"(?<=,)\d*(?=,)", point.Value.TypId.ToString(), RegexOptions.IgnoreCase);
                            if (int.Parse(advSpotRegex.Match(line).ToString()) != point.Value.TypId)
                                changed = true;
                            break;
                        }
                    }
                }
                else if (lineToLower.Contains("spot_exe"))
                {
                    foreach (var point in changedSpots)
                    {
                        if (line.Contains(point.Key.ToString()))
                        {
                            Regex advSpotRegex = new Regex(@"(?<=\().*(?=\))", RegexOptions.IgnoreCase);
                            string spotexeArgs = advSpotRegex.Match(line).ToString().Replace(" ", "");
                            List<string> args = spotexeArgs.Split(',').Select(s => s.Trim()).ToList();
                            lineToAdd = "SPOT_EXE  ( " + args[0] + "," + point.Value.TypId.ToString() + "," + args[2] + "," + args[3] + "," + args[4] + "," + args[5] + "," + args[6] + " )";
                            if (int.Parse(args[1]) != point.Value.TypId)
                                changed = true;
                            break;
                        }
                    }
                }
                else
                    lineToAdd = line;
                resultFileContent += lineToAdd + "\r\n";
            }
            reader.Close();
            if (alwaysSave || changed)
            {
                List<string> files = new List<string>() { Path.GetFileName(file) };
                await SaveFile(resultFileContent, saveDirectory, files, isKRC4);
            }
            return changed;
        }

        private string GetLineToAdd(string vartype, IDictionary<int, WeldpointBMW> changedSpots, string line, int caseNum, out bool isChanged)
        {
            // casenum - 1 - ptp, 2 - fdat, 3 - pdat/ldat, 4 - swh_pre, 5 - swh_exe, 6 - swh fold, 7 - swh param, 8 - Swh_ReloadPos, 9 - deltamfg, 10 - swr param, 11 - swr_control, 12 - swr_exe, 13 - swr_pre, 14 - swr_reloadPos, 16 - swp_exe
            int pointNum = 0;
            string oldTypId = string.Empty;
            string lineToAdd = string.Empty;
            isChanged = false;
            Regex typIdRegex;
            Regex pointNumRegex;
            switch (caseNum)
            {
                case 1:
                    typIdRegex = new Regex("(?<=" + vartype + "\\d+_)\\d+", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex("(?<=" + vartype + ")\\d+", RegexOptions.IgnoreCase);
                    break;
                case 2:
                    typIdRegex = new Regex("(?<=" + vartype + "\\d+_)\\d+", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex("(?<=" + vartype + ")\\d+", RegexOptions.IgnoreCase);
                    break;
                case 3:
                    typIdRegex = new Regex("(?<=" + vartype + "\\d+_)\\d+", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex("(?<=" + vartype + ")\\d+", RegexOptions.IgnoreCase);
                    break;
                case 4:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"(?<=swh_pre\s*\(\s*\d+,\s*)\d+", RegexOptions.IgnoreCase);
                    MatchCollection matches = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches[2].ToString());
                    oldTypId = matches[1].ToString();
                    break;
                case 5:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"(?<=swh_pre\s*\(\s*\d+,\s*)\d+", RegexOptions.IgnoreCase);
                    MatchCollection matches1 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches1[0].ToString());
                    oldTypId = matches1[1].ToString();
                    break;
                case 6:
                    typIdRegex = new Regex(@"(?<=TypId\s*\:\s*)\d+", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex(@"(?<=StudNo\s*\:\s*)\d+", RegexOptions.IgnoreCase);
                    break;
                case 7:
                    typIdRegex = new Regex(@"(?<=swh_typid\s*\=\s*)\d+", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex(@"(?<=swh_studno\s*\=\s*)\d+", RegexOptions.IgnoreCase);
                    break;
                case 8:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"\d+");
                    MatchCollection matches2 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches2[0].ToString());
                    oldTypId = matches2[1].ToString();
                    break;
                case 9:
                    typIdRegex = new Regex("(?<=" + vartype + "\\d+_)\\d+", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex("(?<=" + vartype + ")\\d+", RegexOptions.IgnoreCase);
                    break;
                case 10:
                    typIdRegex = new Regex(@"(?<=swr_typid\s*\=\s*)\d+", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex(@"(?<=swr_studno\s*\=\s*)\d+", RegexOptions.IgnoreCase);
                    break;
                case 11:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"\d+");
                    MatchCollection matches3 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches3[1].ToString());
                    oldTypId = matches3[2].ToString();
                    break;
                case 12:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"(?<=swr_exe\s*\(\s*\d+,\s*)\d+", RegexOptions.IgnoreCase);
                    MatchCollection matches4 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches4[0].ToString());
                    oldTypId = matches4[1].ToString();
                    break;
                case 13:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"\d+");
                    MatchCollection matches5 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches5[2].ToString());
                    oldTypId = matches5[1].ToString();
                    break;
                case 14:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"\d+");
                    MatchCollection matches6 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches6[0].ToString());
                    oldTypId = matches6[1].ToString();
                    break;
                case 15:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"\d+");
                    MatchCollection matches7 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches7[3].ToString());
                    oldTypId = matches7[4].ToString();
                    break;
                case 16:
                    pointNumRegex = new Regex(@"\d+");
                    typIdRegex = new Regex(@"\d+");
                    MatchCollection matches8 = pointNumRegex.Matches(line);
                    pointNum = int.Parse(matches8[2].ToString());
                    oldTypId = matches8[1].ToString();
                    break;
                default:
                    typIdRegex = new Regex("", RegexOptions.IgnoreCase);
                    pointNumRegex = new Regex("", RegexOptions.IgnoreCase);
                    break;
            }
            if (string.IsNullOrEmpty(oldTypId))
            {
                oldTypId = typIdRegex.Match(line).ToString();
                pointNum = int.Parse(pointNumRegex.Match(line).ToString());
            }
            var point = changedSpots.FirstOrDefault(x => x.Key == pointNum);
            if (point.Key == 0)
                lineToAdd = line;
            else
            {
                string newTypId = point.Value.TypId.ToString();
                if (caseNum == 5 || caseNum == 12)
                {
                    Regex replaceRegex = new Regex(pointNum.ToString() + "_" + oldTypId, RegexOptions.IgnoreCase);
                    Regex replaceRegex2 = new Regex("(?<=,\\s*)" + oldTypId + "(?=\\s*,)", RegexOptions.IgnoreCase);
                    lineToAdd = replaceRegex.Replace(line, pointNum.ToString() + "_" + newTypId);
                    lineToAdd = replaceRegex2.Replace(lineToAdd, newTypId);
                }
                else if (caseNum == 8 || caseNum == 14)
                {
                    Regex replaceRegex = new Regex("(?<=_)" + oldTypId + "(?=_)", RegexOptions.IgnoreCase);
                    lineToAdd = replaceRegex.Replace(line, newTypId);
                }
                else if (caseNum == 11 || caseNum == 13 || caseNum == 15)
                {
                    Regex replaceRegex = new Regex(pointNum.ToString() + "\\s*,\\s*\\d+", RegexOptions.IgnoreCase);
                    lineToAdd = replaceRegex.Replace(line, pointNum + "," + newTypId);
                }
                else if (caseNum == 16)
                {
                    Regex replaceRegex = new Regex(oldTypId + "\\s*,\\s*\\d+", RegexOptions.IgnoreCase);
                    lineToAdd = replaceRegex.Replace(line, newTypId+","+pointNum);
                }
                else
                {
                    lineToAdd = typIdRegex.Replace(line, newTypId);
                }
                if (oldTypId != newTypId)
                    isChanged = true;
            }
            return lineToAdd;
        }

        private async Task UpdateDatsFilesInProgram(string directory, IDictionary<int, WeldpointBMW> changedSpots, bool isKRC4)
        {
            string[] datFiles;
            if (isKRC4)
                datFiles = Directory.GetFiles(directory + "\\KRC\\R1\\BMW_Program", "*.dat", SearchOption.AllDirectories);
            else
                datFiles = Directory.GetFiles(directory + "\\KRC\\R1\\Program", "*.dat", SearchOption.AllDirectories);
            foreach (string file in datFiles)
            {
                await UpdateDatFile(file, changedSpots, directory + "\\CorrectTypID", false, isKRC4);
            }
        }

        private async Task<bool> UpdateDatFile(string datFile, IDictionary<int, WeldpointBMW> changedSpots, string directory, bool alwaysSave, bool isKRC4)
        {
            List<string> filenames = new List<string>();
            bool changed = false;
            string resultFileContent = "", outPathType;
            StreamReader reader = new StreamReader(datFile);
            while (!reader.EndOfStream)
            {
                string lineToAdd = "";
                string line = await reader.ReadLineAsync();
                string lineToLower = line.ToLower();
                if ((lineToLower.ContainsAnyRet(new string[] { "xlsp", "xswp", "xswh", "xswr", "xreload" }, out outPathType) && lineToLower.Contains("e6pos")) || (lineToLower.ContainsAny(new string[] { "flsp", "fswp", "fswh", "fswr", "freload" }) && lineToLower.Contains("fdat")) || (lineToLower.ContainsAny(new string[] { "plsp", "pswp", "pswh", "pswr", "preload" }) && lineToLower.Contains("pdat")) || (lineToLower.ContainsAny(new string[] { "llsp", "lswp", "lswh", "lswr", "lreload" }) && lineToLower.Contains("ldat")) || (lineToLower.Contains("deltamfg")))
                {
                    string fileName = GetFileByType(outPathType, Path.GetFileName(datFile));
                    if (!string.IsNullOrEmpty(fileName) && !filenames.Contains(fileName))
                        filenames.Add(fileName);
                    Regex reloadRegex = new Regex(@".*sw.(Reload\d+|WELD|_PRELOAD\s*\=)", RegexOptions.IgnoreCase);
                    Regex getPointNum = new Regex(@"(?<=(deltamfg|\sX|\sf|\sl|\sp)[a-zA-Z\s]+)\d+", RegexOptions.IgnoreCase);
                    if (!reloadRegex.IsMatch(line))
                    {
                        int num;
                        if (!int.TryParse(getPointNum.Match(line).ToString(), out num))
                            lineToAdd = line;
                        else
                        {
                            var spotToChange = changedSpots.FirstOrDefault(x => x.Key == num);
                            if (spotToChange.Key > 0)
                            {

                                lineToAdd = line.Replace("_" + GetPoitnsTypID(line), "_" + spotToChange.Value.TypId);
                                if (!changed && int.Parse(GetPoitnsTypID(line)) != spotToChange.Value.TypId)
                                    changed = true;
                                // break;
                            }
                            else
                                lineToAdd = line;
                        }
                    }
                    else
                        lineToAdd = line;
                }
                else
                    lineToAdd = line;
                resultFileContent += lineToAdd + "\r\n";

            }
            reader.Close();
            if (alwaysSave || changed)
                await SaveFile(resultFileContent, directory, filenames, isKRC4);

            return changed;
        }

        private string GetFileByType(string outPathType, string datFile)
        {
            if (outPathType == "xlsp")
                return "spot_global.dat";
            if (outPathType == "xswp")
                return "a04_swp_global.dat";
            if (outPathType == "xswh")
                return "a20_swh_global.dat";
            if (outPathType == "xswr")
                return "a21_swr_global.dat";
            return datFile.ToLower();
        }

        private async Task SaveFile(string resultFileContent, string directory, List<string> filenames, bool isKRC4)
        {
            foreach (var file in filenames)
            {
                if (file.ContainsAny(new string[] { "a04_swp_global", "a20_swh_global", "a21_swr_global" }))
                    directory = Path.Combine(directory, "BMW_App");
                else
                    if (isKRC4)
                    directory = Path.Combine(directory, "BMW_Program");
                else
                    directory = Path.Combine(directory, "Program");
                //                File.WriteAllText(directory + "\\" + file, resultFileContent);
                await CommonMethods.WriteAllTextAsync(directory + "\\" + file, resultFileContent);
            }
        }

        private string GetPoitnsTypID(string line)
        {
            Regex pointTypIDRegex = new Regex(@"(?<=_\s*)\d*", RegexOptions.IgnoreCase);
            List<string> matchesString = new List<string>();
            var matches = pointTypIDRegex.Matches(line);
            foreach (var match in matches)
            {
                if (!string.IsNullOrEmpty(match.ToString()))
                    matchesString.Add(match.ToString());
            }

            return matchesString[0];
        }

        private bool CreateDirectories(string directory, string dirName, bool createStd, bool createStdUser, bool createProgram, bool createTP, bool isKRC4 = false)
        {
            List<string> directories = new List<string>();
            if (isKRC4)
                directories = new List<string>() { "\\BMW_App", "\\BMW_Utilities", "\\BMW_Program", "\\TP\\SafeRobot" };
            else
                directories = new List<string>() { "\\BMW_Std", "\\BMW_Std_User", "\\Program", "\\TP\\SafeRobot" };
            bool result = true;
            if (Directory.Exists(directory + "\\" + dirName))
            {
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Selected backup already contains folder "+dirName+"\r\nRemove "+dirName+" folder?", "Remove "+dirName+" folder?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(directory + "\\"+dirName, Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    result = true;
                }
                else
                {
                    MessageBox.Show("Process changing aborted", "Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                    result = false;
                }
            }
            Directory.CreateDirectory(directory + "\\" + dirName);

            if (createStd)
                if (!Directory.Exists(directory + "\\" + dirName + directories[0]))
                    Directory.CreateDirectory(directory + "\\" + dirName + directories[0]);
            if (createStdUser)
                if (!Directory.Exists(directory + "\\" + dirName + directories[1]))
                    Directory.CreateDirectory(directory + "\\" + dirName + directories[1]);
            if (createProgram)
                if (!Directory.Exists(directory + "\\" + dirName + directories[2]))
                    Directory.CreateDirectory(directory + "\\" + dirName + directories[2]);
            if (createTP)
                if (!Directory.Exists(directory + "\\" + dirName + directories[3]))
                    Directory.CreateDirectory(directory + "\\" + dirName + directories[3]);
            return result;
        }

        private IDictionary<int, WeldpointBMW> UpdatePointsList(List<string> pointsList, IDictionary<int, WeldpointBMW> initialWeldPoints)
        {
            IDictionary<int, WeldpointBMW> result = new Dictionary<int, WeldpointBMW>();
            List<WeldpointShort> updatedPointsShort = new List<WeldpointShort>();
            foreach (string point in pointsList)
                updatedPointsShort.Add(GetPointShort(point));

            foreach (WeldpointShort pointShort in updatedPointsShort)
            {
                foreach (var pointLong in initialWeldPoints)
                {
                    if (pointLong.Value.Number == pointShort.Number)
                    {
                        result.Add(pointLong.Value.Number, new WeldpointBMW(pointLong.Value.Robot, pointLong.Value.Path, pointLong.Value.PLC, pointLong.Value.Number, pointShort.TypId, pointLong.Value.XPos, pointLong.Value.YPos, pointLong.Value.ZPos, pointLong.Value.A, pointLong.Value.B, pointLong.Value.C, pointLong.Value.ProcessType, pointLong.Value.PathToBackup));
                        break;
                    }
                }
            }
            return result;
        }

        private IDictionary<int, WeldpointBMW> FindWeldPoints(string spotGlobal, string dir)
        {
            IDictionary<int, WeldpointBMW> result = new Dictionary<int, WeldpointBMW>();
            TypIdChangeMethods a = new TypIdChangeMethods();
            List<string> fileContent = a.ReadFile(spotGlobal);
            StreamReader reader = new StreamReader(spotGlobal);
            foreach (var line in fileContent)
            {
                if (line.ToLower().Contains("e6pos") && (line.ToLower().Contains("xlsp") || ((line.ToLower().Contains("xswh") || line.ToLower().Contains("xswr")) && !line.ToLower().Contains("reload") && !line.ToLower().Contains("weld"))) && line.ToLower().Trim().Substring(0, 1) != ";")
                {
                    WeldpointBMW currentPoint = ReadSpotsMethods.GetPoint(line, dir);
                    if (currentPoint != null)
                    {
                        if (!result.Keys.Contains(currentPoint.Number))
                            result.Add(currentPoint.Number, currentPoint);
                        else
                            errorMessage += "Double declaration of point " + currentPoint.Number + ". Check robot " + currentPoint.PLC + currentPoint.Robot + "\r\n";
                    }
                }
            }
            return result;
        }

        internal List<string> UpdatePointsList(List<string> pointsList, WeldpointShort currentPoint)
        {
            Regex getNumbers = new Regex(@"\d+");
            List<string> result = new List<string>();
            foreach (var item in pointsList)
            {
                MatchCollection matches = getNumbers.Matches(item);
                if (currentPoint.Number == int.Parse(matches[0].ToString()))
                    result.Add("Point: " + currentPoint.Number.ToString() + " TypID: " + currentPoint.TypId.ToString());
                else
                    result.Add(item);
            }
            return result;
        }

        internal WeldpointShort ChangeTypId(string value)
        {
            WeldpointShort currentPoint = GetPointShort(value);
            TypIdChangerViewModel vm = new TypIdChangerViewModel(currentPoint.Number, currentPoint.TypId);
            TypIdChanger sW = new TypIdChanger(vm);
            var dialogResult = sW.ShowDialog();
            return new WeldpointShort(vm.Number, vm.TypId);
        }

        private WeldpointShort GetPointShort(string value)
        {
            Regex pointNumRegex = new Regex(@"(?<=Point: )\d+", RegexOptions.IgnoreCase);
            int number = int.Parse(pointNumRegex.Match(value).ToString());
            Regex typNumRegex = new Regex(@"(?<=TypID: )\d+", RegexOptions.IgnoreCase);
            int typID = int.Parse(typNumRegex.Match(value).ToString());
            return new WeldpointShort(number, typID);
        }

        internal List<string> ConvertSpotPointsToList(List<KeyValuePair<int, WeldpointBMW>> points)
        {
            List<string> result = new List<string>();
            foreach (var point in points)
                result.Add("Point: " + point.Value.Number + " TypID: " + point.Value.TypId);
            return result;
        }

        private string GetSpotGlobal(string backup)
        {
            string[] allfiles = Directory.GetFiles(backup, "*.*", SearchOption.AllDirectories);
            foreach (string path in allfiles.Where(x => x.ToLower().Contains("spot_global.dat")))
                return path;
            if (allfiles.ToList().Any(x => x.ToLower().Contains("a20_swh_global.dat") || x.ToLower().Contains("a21_swr_global.dat")))
                return allfiles.ToList().First(x => x.ToLower().Contains("a20_swh_global.dat") || x.ToLower().Contains("a21_swr_global.dat"));
            if (allfiles.ToList().Any(x => x.ToLower().Contains("a04_swp_global.dat")))
                return allfiles.ToList().First(x => x.ToLower().Contains("a04_swp_global.dat"));
            return "";
        }

        private async Task<IDictionary<string, List<SpotsInFile>>> FindUsedPointsInSrcFiles(string backup)
        {
            IDictionary<string, List<SpotsInFile>> result = new Dictionary<string, List<SpotsInFile>>();
            List<SpotsInFile> spotsInCurrentFile = new List<SpotsInFile>();
            string[] allfiles = Directory.GetFiles(backup, "*.src", SearchOption.AllDirectories);
            List<string> allFilesFiltered = new List<string>();
            foreach (var fil in allfiles.Where(x => !x.ToLower().Contains("bmw_app") && !x.ToLower().Contains("bmw_std")))
                allFilesFiltered.Add(fil);
            string[] searchstrings = new string[] { "advspot", "swh_Exe", "swr_Exe" };
            foreach (var file in allFilesFiltered)
            {
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    if (line.ToLower().ContainsAny(searchstrings) && !line.ToLower().Contains("def ") && !line.ToLower().Contains("end"))
                    {
                        SpotsInFile currentPoint = ReadSpotsMethods.GetPointFropSrc(line, Path.GetFileNameWithoutExtension(file), "");
                        spotsInCurrentFile.Add(currentPoint);
                    }
                }
                reader.Close();
            }

            CheckLocalSpots(backup, Directory.GetFiles(backup, "*.dat", SearchOption.AllDirectories), spotsInCurrentFile);

            result.Add(backup, spotsInCurrentFile);
            return result;
        }

        private void CheckLocalSpots(string backup, string[] files, List<SpotsInFile> spotsInCurrentFile)
        {
            foreach (string item in files)
            {
                if (!item.ToLower().Contains("spot_global"))
                    foreach (SpotsInFile spot in spotsInCurrentFile)
                    {
                        StreamReader reader = new StreamReader(item);
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line.ToLower().Contains("e6pos") && line.Contains(spot.SpotNr + "_" + spot.TypID))
                                errorMessage += "Local definition of point " + spot.SpotNr + "_" + spot.TypID + " in robot " + Path.GetFileNameWithoutExtension(backup) + " path " + spot.PathName;
                        }
                        reader.Close();
                    }
            }
        }
    }
}


