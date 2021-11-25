using Microsoft.WindowsAPICodePack.Dialogs;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace RobotFilesEditor.Model.Operations
{

    public static class RenumberPointsMethods
    {
        public static void Execute()
        {
            string directory = SelectBackup();
            if (string.IsNullOrEmpty(directory))
                return;
            List<SrcAndDat> files = FindFilesInBackup(directory);
            files = DivideToFolds(files);
            IDictionary<string, List<LineAndParams>> filesBeforeRenumber = GetFilesBeforeRenumber(files);
            SrcValidator.FindUnusedDataInDatFiles(GetUnusedDataInput(filesBeforeRenumber), GetDatFiles(filesBeforeRenumber.Keys));
            var unusedDats = SrcValidator.UnusedDats;
            var usedDats = SrcValidator.UsedDats;
            List<SrcAndDat> resultFiles = RenumberPoints(filesBeforeRenumber, files);
            SaveFiles(resultFiles,directory);
        }

        private static bool CheckIfKRC4(string directory)
        {
            List<string> directories = Directory.GetDirectories(directory,"*.*",SearchOption.AllDirectories).ToList();
            foreach (var item in directories.Where(x => x.ToLower().Contains("\\bmw_program")))
                return true;
            return false;

        }

        private static void SaveFiles(List<SrcAndDat> resultFiles, string dir)
        {
            try
            {
                if (!Directory.Exists(dir + "\\RenumberedFiles"))
                    Directory.CreateDirectory(dir + "\\RenumberedFiles");
                foreach (var file in resultFiles)
                {
                    Regex getDirRegex = new Regex(@"(?<=.*\\KRC\\R1\\).*", RegexOptions.IgnoreCase);
                    string dirname = Path.GetDirectoryName(getDirRegex.Match(file.Src).ToString());
                    string[] dirs = dirname.Split('\\');
                    int counter = 0;
                    string dirToCreate = dir + "\\RenumberedFiles";
                    foreach (var directory in dirs)
                    {
                        dirToCreate = dirToCreate + "\\" + directory;
                        if (!Directory.Exists(dirToCreate))
                            Directory.CreateDirectory(dirToCreate);
                        counter++;
                    }
                }
                foreach (var file in resultFiles)
                {
                    File.WriteAllLines(file.Src.Replace("\\KRC\\R1\\", "\\RenumberedFiles\\"), file.SrcContent);
                    File.WriteAllLines(file.Dat.Replace("\\KRC\\R1\\", "\\RenumberedFiles\\"), file.DatContent);
                }
                MessageBox.Show("Succesfully saved at " + dir + "\\RenumberedFiles", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong\r\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static List<SrcAndDat> RenumberPoints(IDictionary<string, List<LineAndParams>> filesBeforeRenumber, List<SrcAndDat> srcsAndDats)
        {
            try
            {
                List<SrcAndDat> result = new List<SrcAndDat>();
                Regex isHomeRegex = new Regex(@"PTP\s+XHOME\d+", RegexOptions.IgnoreCase);
                List<PointOldAndNew> pointsOldAndNew = new List<PointOldAndNew>();
                IDictionary<string, List<string>> datFiles = GetDats(srcsAndDats);
                foreach (var file in filesBeforeRenumber)
                {
                    List<string> dat = new List<string>(datFiles[file.Key.Replace(".src", ".dat")]);
                    List<string> alreadyChangedTargets = new List<string>();
                    List<string> alreadyChangedNames = new List<string>();
                    List<string> alreadyChangedFDats = new List<string>();
                    List<string> alreadyChangedPDats = new List<string>();
                    List<string> alreadyChangedLDats = new List<string>();
                    List<string> srcContent = new List<string>();
                    int pointnumber = 10;
                    foreach (var fold in file.Value)
                    {
//                        if (fold.Point == null && !isHomeRegex.IsMatch(fold.Line))
                          if (fold.Point == null)
                                srcContent.Add(fold.Line);
                        else if (fold.Point != null && (fold.Point.Target.ToLower().Contains("xlsp")) || (fold.Point.Target.ToLower().Contains("xkln")) || (fold.Point.Target.ToLower().Contains("xsnh")) || (fold.Point.Target.ToLower().Contains("xswr")) || (fold.Point.Target.ToLower().Contains("xswh") || (fold.Point.Target.ToLower().Contains("xswp") || (fold.Point.Target.ToLower().Contains("xgl")))))
                            srcContent.Add(fold.Line);
                        else
                        {
                            bool addPoint = true;
                            PointInSrc newPoint = new PointInSrc();
                            string currentFold = fold.Line;
                            if (!alreadyChangedTargets.Contains(fold.Point.Target))
                            {
                                alreadyChangedTargets.Add(fold.Point.Target);
                                if (!isHomeRegex.IsMatch(currentFold))
                                {
                                    currentFold = currentFold.Replace(fold.Point.Target, "XP" + pointnumber);
                                    newPoint.Target = "XP" + pointnumber;
                                }
                                else
                                {
                                    string homename = isHomeRegex.Match(currentFold).ToString().Split(' ')[1];
                                    currentFold = currentFold.Replace(fold.Point.Target, homename);
                                    newPoint.Target = homename;
                                }
                            }
                            else
                            {
                                addPoint = false;
                                foreach (var point in pointsOldAndNew.Where(x => x.OldPoint.Target == fold.Point.Target))
                                {
                                    if (!isHomeRegex.IsMatch(currentFold))
                                        currentFold = currentFold.Replace(fold.Point.Target, "XP" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(point.NewPoint.Target).ToString()));
                                    else
                                    {
                                        string homename = isHomeRegex.Match(currentFold).ToString().Split(' ')[1];
                                        currentFold = currentFold.Replace(fold.Point.Target, homename);
                                    }
                                }
                            }

                            if (!alreadyChangedNames.Contains(fold.Point.Name))
                            {
                                Regex param1Replace = new Regex(@"Kuka\s*\.\s*PointName=P\d+", RegexOptions.IgnoreCase);
                                alreadyChangedNames.Add(fold.Point.Name);
                                if (!isHomeRegex.IsMatch(currentFold))
                                {
                                    currentFold = currentFold.Replace(" " + fold.Point.Name + " ", " P" + pointnumber + " ");
                                    currentFold = currentFold.Replace(" " + fold.Point.Name + ",", " P" + pointnumber + ",");
                                    currentFold = currentFold.Replace(":" + fold.Point.Name, ":P" + pointnumber);
                                    currentFold = param1Replace.Replace(currentFold, "Kuka.PointName=P" + pointnumber);
                                    newPoint.Name = "P" + pointnumber;
                                }
                                else
                                {
                                    string homename = isHomeRegex.Match(currentFold).ToString().Split(' ')[1];
                                    Regex regex1 = new Regex(@"(?<=;\s*FOLD\s+PTP\s+)\w*", RegexOptions.IgnoreCase);
                                    Regex regex2 = new Regex(@"Kuka\s*\.\s*PointName\s*\=\s*\w*;", RegexOptions.IgnoreCase);
                                    Regex regex3 = new Regex(@"Kuka\s*\.\s*MoveDataPtpName\s*\=\s*\w*;", RegexOptions.IgnoreCase);
                                    Regex regexPdat = new Regex(@"(?<=PDAT_ACT\s*=\s*)\w+", RegexOptions.IgnoreCase);
                                    Regex regex4 = new Regex(@"PDAT_ACT\s*=\s*\w+", RegexOptions.IgnoreCase);
                                    Regex regexFdat = new Regex(@"(?<=FDAT_ACT\s*=\s*)\w+", RegexOptions.IgnoreCase);
                                    Regex regex5 = new Regex(@"FDAT_ACT\s*=\s*\w+", RegexOptions.IgnoreCase);
                                    Regex regex6 = new Regex(@"(?<=Vel\s*\=\s*\d+\s*\%\s*)\w+", RegexOptions.IgnoreCase);

                                    string pdatNameBeforeChange = regexPdat.Match(currentFold).ToString();
                                    string fdatNameBeforeChange = regexFdat.Match(currentFold).ToString();

                                    currentFold = regex1.Replace(currentFold, homename.Substring(1, homename.Length - 1));
                                    currentFold = regex2.Replace(currentFold, "Kuka.PointName=" + homename.Substring(1, homename.Length - 1) + ";");
                                    currentFold = regex3.Replace(currentFold, "Kuka.MoveDataPtpName=P" + homename.Substring(1, homename.Length - 1) + ";");
                                    currentFold = regex4.Replace(currentFold, "PDAT_ACT= PP" + homename.Substring(1, homename.Length - 1));
                                    currentFold = regex5.Replace(currentFold, "FDAT_ACT= F" + homename.Substring(1, homename.Length - 1));
                                    currentFold = regex6.Replace(currentFold, "P" + homename.Substring(1, homename.Length - 1));
                                    newPoint.Name = homename;
                                }
                            }
                            else
                            {
                                addPoint = false;
                                foreach (var point in pointsOldAndNew.Where(x => x.OldPoint.Name == fold.Point.Name))
                                {
                                    if (!isHomeRegex.IsMatch(currentFold))
                                    {
                                        currentFold = currentFold.Replace(" " + fold.Point.Name + " ", " P" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(point.NewPoint.Name).ToString()) + " ");
                                        currentFold = currentFold.Replace(" " + fold.Point.Name + ",", " P" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(point.NewPoint.Name).ToString()) + ",");
                                        currentFold = currentFold.Replace(":" + fold.Point.Name, ":P" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(point.NewPoint.Name).ToString()));
                                    }
                                    else
                                    {
                                        string homename = isHomeRegex.Match(currentFold).ToString().Split(' ')[1];
                                        Regex regex1 = new Regex(@"(?<=;\s*FOLD\s+PTP\s+)\w*", RegexOptions.IgnoreCase);
                                        Regex regex2 = new Regex(@"Kuka\s*\.\s*PointName\s*\=\s*\w*;", RegexOptions.IgnoreCase);
                                        Regex regex3 = new Regex(@"Kuka\s*\.\s*MoveDataPtpName\s*\=\s*\w*;", RegexOptions.IgnoreCase);
                                        Regex regexPdat = new Regex(@"(?<=PDAT_ACT\s*=\s*)\w+", RegexOptions.IgnoreCase);
                                        Regex regex4 = new Regex(@"PDAT_ACT\s*=\s*\w+", RegexOptions.IgnoreCase);
                                        Regex regexFdat = new Regex(@"(?<=FDAT_ACT\s*=\s*)\w+", RegexOptions.IgnoreCase);
                                        Regex regex5 = new Regex(@"FDAT_ACT\s*=\s*\w+", RegexOptions.IgnoreCase);
                                        Regex regex6 = new Regex(@"(?<=Vel\s*\=\s*\d+\s*\%\s*)\w+", RegexOptions.IgnoreCase);

                                        string pdatNameBeforeChange = regexPdat.Match(currentFold).ToString();
                                        string fdatNameBeforeChange = regexFdat.Match(currentFold).ToString();

                                        currentFold = regex1.Replace(currentFold, homename.Substring(1, homename.Length - 1));
                                        currentFold = regex2.Replace(currentFold, "Kuka.PointName=" + homename.Substring(1, homename.Length - 1) + ";");
                                        currentFold = regex3.Replace(currentFold, "Kuka.MoveDataPtpName=P" + homename.Substring(1, homename.Length - 1) + ";");
                                        currentFold = regex4.Replace(currentFold, "PDAT_ACT= PP" + homename.Substring(1, homename.Length - 1));
                                        currentFold = regex5.Replace(currentFold, "FDAT_ACT= F" + homename.Substring(1, homename.Length - 1));
                                        currentFold = regex6.Replace(currentFold, "P" + homename.Substring(1, homename.Length - 1));
                                    }
                                }
                            }
                            Regex fDatRegex = new Regex(@"FDAT_ACT\s*=\s*[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);

                            if (!alreadyChangedFDats.Contains(fold.Point.Fdat))
                            {
                                alreadyChangedFDats.Add(fold.Point.Fdat);
                                if (!isHomeRegex.IsMatch(currentFold))
                                {
                                    currentFold = fDatRegex.Replace(currentFold, "FDAT_ACT= FP" + pointnumber);
                                    newPoint.Fdat = "FP" + pointnumber;
                                }
                                else
                                {
                                    string homename = isHomeRegex.Match(currentFold).ToString().Split(' ')[1];
                                    newPoint.Fdat = "F" + homename.Substring(1, homename.Length - 1);
                                }
                            }
                            else
                            {
                                addPoint = false;
                                foreach (var point in pointsOldAndNew.Where(x => x.OldPoint.Fdat == fold.Point.Fdat))
                                {
                                    if (!isHomeRegex.IsMatch(currentFold))
                                        currentFold = fDatRegex.Replace(currentFold, "FDAT_ACT= FP" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(point.NewPoint.Fdat).ToString()));
                                }
                            }

                            newPoint.Pdat = new List<string>();
                            newPoint.Ldat = new List<string>();

                            if (fold.Point.Type == "PTP")
                            {
                                if (!isHomeRegex.IsMatch(currentFold))
                                {
                                    Regex pDatRegex = new Regex(@"PDAT_ACT\s*=\s*[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                                    Regex pDatRegex2 = new Regex(@"(?<=\s)PP\d+(?=\s)", RegexOptions.IgnoreCase);
                                    Regex pDatRegex3 = new Regex(@"(?<=:\s*)PP\d+", RegexOptions.IgnoreCase);
                                    Regex pDatRegex4 = new Regex(@"Kuka\s*\.\s*MoveData(PtpName|Name)\s*=\s*(P|L)P\d+", RegexOptions.IgnoreCase);
                                    foreach (var pdat in fold.Point.Pdat)
                                    {
                                        alreadyChangedPDats.Add(pdat);
                                        currentFold = pDatRegex.Replace(currentFold, "PDAT_ACT= PPP" + pointnumber);
                                        currentFold = pDatRegex2.Replace(currentFold, "PP" + pointnumber);
                                        currentFold = pDatRegex3.Replace(currentFold, "PP" + pointnumber);
                                        currentFold = pDatRegex4.Replace(currentFold, "Kuka.MoveDataPtpName=PP" + pointnumber);
                                        newPoint.Pdat.Add("PP" + pointnumber);
                                    }
                                }
                                else
                                {
                                    string homename = isHomeRegex.Match(currentFold).ToString().Split(' ')[1];
                                    newPoint.Pdat.Add("PP" + homename.Substring(1, homename.Length - 1));
                                }
                            }
                            else
                            {
                                Regex lDatRegex = new Regex(@"LDAT_ACT\s*=\s*[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                                Regex lDatRegex2 = new Regex(@"(?<=\s)LP\d+(?=\s)", RegexOptions.IgnoreCase);
                                Regex lDatRegex3 = new Regex(@"(?<=:\s*)LP\d+", RegexOptions.IgnoreCase);
                                Regex lDatRegex4 = new Regex(@"Kuka\s*\.\s*MoveData(PtpName|Name)\s*=\s*(P|L)P\d+", RegexOptions.IgnoreCase);
                                Regex pDatRegex = new Regex(@"\s[a-zA-Z0-9_]*PDAT\d*", RegexOptions.IgnoreCase);
                                foreach (var ldat in fold.Point.Ldat)
                                {
                                    alreadyChangedLDats.Add(ldat);
                                    currentFold = lDatRegex.Replace(currentFold, "LDAT_ACT= LLP" + pointnumber);
                                    currentFold = lDatRegex4.Replace(currentFold, "Kuka.MoveDataName=LP" + pointnumber);
                                    string tempText = pDatRegex.Match(currentFold).ToString().Trim();
                                    if (!string.IsNullOrEmpty(tempText))
                                        currentFold = currentFold.Replace(tempText, "LP" + pointnumber);
                                    currentFold = lDatRegex2.Replace(currentFold, "LP" + pointnumber);
                                    newPoint.Ldat.Add("LP" + pointnumber);
                                }
                            }
                            //if (isKRC4)
                            //{
                                currentFold = ReplaceParams(currentFold);
                            //}
                            if (addPoint)
                            {
                                if (!isHomeRegex.IsMatch(currentFold))
                                    pointnumber = pointnumber + 10;
                                newPoint.Type = fold.Point.Type;
                                pointsOldAndNew.Add(new PointOldAndNew(fold.Point, newPoint));
                            }
                            srcContent.Add(currentFold);
                            //dat = ReplaceInDat(dat, pointnumber, fold);                       
                        }
                    }
                    dat = ReplaceOldWithNewInDat(dat, pointsOldAndNew, file.Key);
                    pointsOldAndNew = new List<PointOldAndNew>();
                    result.Add(new SrcAndDat(file.Key, file.Key.Replace(".src", ".dat"), srcContent, dat));
                }
                return result;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                return null;
            }
        }

        private static string ReplaceParams(string currentFold)
        {
            try
            {
                Regex pdatRegex = new Regex(@"(?<=PDAT_ACT\=)[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                Regex ldatRegex = new Regex(@"(?<=LDAT_ACT\=)[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                Regex ptpRegex = new Regex(@"(?<=^;\s*FOLD\s+PTP\s+)[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                Regex linRegex = new Regex(@"(?<=^;\s*FOLD\s+LIN\s+)[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                Regex pointNameReplace = new Regex(@"(?<=Kuka\.PointName\s*\=\s*)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
                Regex pdatReplace = new Regex(@"(?<=Kuka\.MoveDataPtpName\s*\=\s*)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
                Regex ldatReplace = new Regex(@"(?<=Kuka\.MoveDataName\s*\=\s*)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
                Regex headerReplace = new Regex(@"(?<=Vel\s*\=\s*(\d+\.\d+\s*[a-zA-Z\/]+|\d+\s*%)\s+)[a-zA-Z0-9\-_]*", RegexOptions.IgnoreCase);

                int typeOfMovement = 0; // 1 - ptp, 2 - lin, 3 - specjalne
                string result = string.Empty;
                if (currentFold.ToLower().Replace(" ", "").Contains("pdat_act") && currentFold.ToLower().Replace(" ", "").Contains("ptp"))
                    typeOfMovement = 1;
                else if (currentFold.ToLower().Replace(" ", "").Contains("ldat_act") && currentFold.ToLower().Replace(" ", "").Contains("lin"))
                    typeOfMovement = 2;
                else
                    typeOfMovement = 3;

                string headerFold = GetHeader(currentFold);
                string paramsFold = GetParamsFold(currentFold);
                switch (typeOfMovement)
                {
                    case 1:
                        {
                            string newPdat = pdatRegex.Match(currentFold.Replace(" ", "")).ToString();
                            string newPtp = ptpRegex.Match(currentFold).ToString();
                            paramsFold = pointNameReplace.Replace(paramsFold, newPtp);
                            paramsFold = pdatReplace.Replace(paramsFold, newPdat);
                            headerFold = headerReplace.Replace(headerFold, newPdat);
                        }
                        break;
                    case 2:
                        {
                            string newLdat = ldatRegex.Match(currentFold.Replace(" ", "")).ToString();
                            string newLin = linRegex.Match(currentFold).ToString();
                            paramsFold = pointNameReplace.Replace(paramsFold, newLin);
                            paramsFold = ldatReplace.Replace(paramsFold, newLdat);
                            headerFold = headerReplace.Replace(headerFold, newLdat);
                        }
                        break;
                    case 3:
                        {
                            return currentFold;
                        }
                    default:
                        {
                            return currentFold;
                        }
                }
                string tepm = GetParamsFold(currentFold);
                if (currentFold.Contains(GetParamsFold(currentFold)))
                { }

                if (GetParamsFold(currentFold) != string.Empty)
                    result = currentFold.Replace(GetParamsFold(currentFold), paramsFold);
                else
                    result = currentFold;
                result = result.Replace(GetHeader(result), headerFold);
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Linijka: " + currentFold, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private static string GetHeader(string currentFold)
        {
            StringReader reader = new StringReader(currentFold);
            string result = reader.ReadLine();
            reader.Close();
            return result;
        }

        private static string GetParamsFold(string currentFold)
        {
            bool addline = false;
            string paramsFold = string.Empty;
            StringReader reader = new StringReader(currentFold);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Replace(" ", "").Trim().Contains(";foldparameters"))
                    addline = true;
                if (addline)
                    paramsFold += line + "\n";
                if (line.ToLower().Replace(" ", "").Trim().Contains(";endfold"))
                {
                    addline = false;
                    break;
                }
            }
            return paramsFold;
        }

        private static bool CheckLPDats(string lpdat, List<string> alreadyChangedDats)
        {
            List<int> lpdatList = new List<int>();
            Regex removeLetters = new Regex(@"\d+", RegexOptions.IgnoreCase);
            if (!removeLetters.IsMatch(lpdat))
                return false;
            int ldatNumber = int.Parse(removeLetters.Match(lpdat).ToString());
            foreach (string item in alreadyChangedDats)
            {
                if (removeLetters.IsMatch(item))
                {
                    if (int.Parse(removeLetters.Match(item).ToString()) == ldatNumber)
                        return true;
                }
            }
            return false;
        }

        private static List<string> ReplaceOldWithNewInDat(List<string> dat, List<PointOldAndNew> pointsOldAndNew, string filename)
        {
            bool lineAdded = false;
            List<string> result = new List<string>();
            dat = RemoveUnusedData(dat, filename);
            foreach (string line in dat)
            {
                lineAdded = false;
                string lineToLower = line.ToLower();
                if (lineToLower.Contains("decl e6pos"))
                {
                    foreach (var point in pointsOldAndNew.Where(x => lineToLower.Replace(" ", "").Contains(x.OldPoint.Target.ToLower() + "=")))
                    {
                        if (point.OldPoint.Target != point.NewPoint.Target)
                        {
                            lineAdded = true;
                            result.Add(line.Replace(point.OldPoint.Target, point.NewPoint.Target));
                        }
                        break;
                    }
                }
                else if (lineToLower.Contains("decl pdat"))
                {
                    //TODO - usunąć te PDAT i LDAT [0]
                    foreach (var point in pointsOldAndNew.Where(x => x.OldPoint.Pdat.Count>0 && lineToLower.Replace(" ", "").Contains(x.OldPoint.Pdat[0].ToLower() + "=")))
                    {
                        if (point.OldPoint.Pdat[0] != point.NewPoint.Pdat[0])
                        {
                            lineAdded = true;
                            if (point.NewPoint.Pdat.Any(x=>x.ToLower().Contains("phome")))
                                result.Add(line.Replace(point.OldPoint.Pdat[0], point.NewPoint.Pdat[0]));
                            else
                                result.Add(line.Replace(point.OldPoint.Pdat[0], "P" + point.NewPoint.Pdat[0]));
                            //result.Add(line.Replace(point.OldPoint.Ldat, "PP" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(point.NewPoint.Target).ToString())));
                        }
                        break;
                    }
                }
                else if (lineToLower.Contains("decl ldat"))
                {
                    foreach (var point in pointsOldAndNew.Where(x => x.OldPoint.Ldat.Count > 0 && lineToLower.Replace(" ", "").Contains(x.OldPoint.Ldat[0].ToLower() + "=")))
                    {
                        if (point.OldPoint != point.NewPoint)
                        {
                            lineAdded = true;
                            result.Add(line.Replace(point.OldPoint.Ldat[0], "LLP" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(point.NewPoint.Target).ToString())));
                        }
                        break;
                    }
                }
                else if (lineToLower.Contains("decl fdat"))
                {
                    foreach (var point in pointsOldAndNew.Where(x => lineToLower.Replace(" ", "").Contains(x.OldPoint.Fdat.ToLower() + "=")))
                    {
                        if (point.OldPoint.Fdat != point.NewPoint.Fdat)
                        {
                            lineAdded = true;
                            result.Add(line.Replace(point.OldPoint.Fdat, point.NewPoint.Fdat));
                        }
                        break;
                    }
                }
                else
                {
                    lineAdded = true;
                    result.Add(line);
                }
                if (!lineAdded)
                    result.Add(line);
            }
            result = SortData(result);
            return result;
        }

        private static List<string> RemoveUnusedData(List<string> dat, string filename)
        {
            Regex getName = new Regex(@"(?<=(E6POS|FDAT|PDAT|LDAT)\s+)[a-zA-Z0-9-_]*", RegexOptions.IgnoreCase);
            bool addLine = true;
            bool startpathfound = false;
            List<string> result = new List<string>();
            foreach (var item in SrcValidator.UnusedDats.Where(x=>x.Key.ToLower().Contains(filename.ToLower().Replace(".src",".dat"))))
            {
                foreach (string line in dat)
                {
                    addLine = true;

                    if (line.ToLower().Contains(";#") && line.ToLower().Contains("start path"))
                        startpathfound = true;
                    foreach (var e6axis in item.Value.E6AXIS)
                    {
                        if (line.ToLower().Contains("decl e6axis") && getName.Match(line).ToString().ToLower()==e6axis.ToLower())
                            addLine = false;

                    }
                    foreach (var e6pos in item.Value.E6POS)
                    {
                        if (line.ToLower().Contains("decl e6pos") && getName.Match(line).ToString().ToLower() == e6pos.ToLower())
                            addLine = false;
                    }
                    foreach (var pdat in item.Value.PDAT)
                    {
                        if (line.ToLower().Contains("decl pdat") && getName.Match(line).ToString().ToLower() == pdat.ToLower())
                            addLine = false;
                    }
                    foreach (var ldat in item.Value.LDAT)
                    {
                        if (line.ToLower().Contains("decl ldat") && getName.Match(line).ToString().ToLower() == ldat.ToLower())
                            addLine = false;
                    }
                    foreach (var fdat in item.Value.FDAT)
                    {
                        if (line.ToLower().Contains("decl fdat") && getName.Match(line).ToString().ToLower() == fdat.ToLower())
                            addLine = false;
                    }
                    if (line.ToLower().Contains("decl dat_ptp") || line.ToLower().Contains("decl dat_lin") || line.ToLower().Contains("decl basis_sugg_t"))
                        addLine = false;
                    if (startpathfound && string.IsNullOrEmpty(line))
                        addLine = false;

                    if (addLine)
                        result.Add(line);
                }
            }

            return result;
        }

        private static List<string> SortData(List<string> dats)
        {
            List<string> result = new List<string>();
            List<string> dataLines = new List<string>();
            IDictionary<string, PointInSrc> points = new Dictionary<string, PointInSrc>();
            IDictionary<string, PointInSrc> pointsNumbered = new Dictionary<string, PointInSrc>();
            IDictionary<string, PointInSrc> pointsOther = new Dictionary<string, PointInSrc>();
            bool startpathfound = false;
            List<string> header = new List<string>();
            foreach (string line in dats)
            {
                string lineToLower = line.ToLower();

                if (!startpathfound)
                    header.Add(line);
                if (lineToLower.Contains(";#") && lineToLower.Contains("start path"))
                    startpathfound = true;
                if (startpathfound)
                {
                    if (lineToLower.Contains("decl e6pos"))
                    {
                        Regex e6posRegex = new Regex(@"(?<=e6pos x).*(?=\=)", RegexOptions.IgnoreCase);
                        string name = e6posRegex.Match(lineToLower).ToString();
                        if (!points.Keys.Contains(name))
                            points.Add(name, new PointInSrc(name,"",line,null,null,null));
                        else
                            points[name].Target = line;
                    }
                    if (lineToLower.Contains("decl e6axis"))
                    {
                        Regex e6axisRegex = new Regex(@"(?<=e6axis x).*(?=\=)", RegexOptions.IgnoreCase);
                        string name = e6axisRegex.Match(lineToLower).ToString();
                        if (!points.Keys.Contains(name))
                            points.Add(name, new PointInSrc(name, "", line, null, null, null));
                        else
                        {
                            points[name].Target = line;
                        }
                    }
                    if (lineToLower.Contains("decl fdat"))
                    {
                        Regex fdatRegex = new Regex(@"(?<=fdat f).*(?=\=)", RegexOptions.IgnoreCase);
                        string name = fdatRegex.Match(lineToLower).ToString();
                        if (!points.Keys.Contains(name))
                            points.Add(name, new PointInSrc(name, "", null, line, null, null));
                        else
                        {
                            points[name].Fdat = line;
                        }
                    }
                    if (lineToLower.Contains("decl pdat"))
                    {
                        Regex pdatRegex = new Regex(@"(?<=pdat p).*(?=\=)", RegexOptions.IgnoreCase);
                        string name = pdatRegex.Match(lineToLower).ToString();
                        if (!points.Keys.Contains(name))
                        {
                            List<string> pdats = new List<string>();
                            pdats.Add(line);
                            points.Add(name, new PointInSrc(name, "", null, null, pdats, null));
                        }
                        else
                        {
                            if (points[name].Pdat == null)
                                points[name].Pdat = new List<string>();
                            points[name].Pdat.Add(line);
                        }
                    }
                    if (lineToLower.Contains("decl ldat"))
                    {
                        Regex ldatRegex = new Regex(@"(?<=ldat l).*(?=\=)", RegexOptions.IgnoreCase);
                        string name = ldatRegex.Match(lineToLower).ToString();
                        if (!points.Keys.Contains(name))
                        {
                            List<string> ldats = new List<string>();
                            ldats.Add(line);
                            points.Add(name, new PointInSrc(name, "", null, null, null, ldats));
                        }
                        else
                        {
                            if (points[name].Ldat == null)
                                points[name].Ldat = new List<string>();
                            points[name].Ldat.Add(line);
                        }
                    }
                }
            }
            Regex isNumbered = new Regex(@"p\d+", RegexOptions.IgnoreCase);
            foreach (var point in points)
            {
                string match = isNumbered.Match(point.Key).ToString();
                if (match == point.Key)
                    pointsNumbered.Add(point);
                else
                    pointsOther.Add(point);
            }
            SortedDictionary<int, PointInSrc> sortedPoints = new SortedDictionary<int, PointInSrc>();
            foreach (var item in pointsNumbered)
                sortedPoints.Add(int.Parse(item.Key.Replace("p", "")), item.Value);

            foreach (var line in pointsOther.Where(x=>x.Key.ToLower().Contains("home1")))
                dataLines.AddRange(AddPointToData(line.Value));
            foreach (var line in sortedPoints)
                dataLines.AddRange(AddPointToData(line.Value));
            foreach (var line in pointsOther.Where(x => !x.Key.ToLower().Contains("home")))
                dataLines.AddRange(AddPointToData(line.Value));
            foreach (var line in pointsOther.Where(x => x.Key.ToLower().Contains("home2")))
                dataLines.AddRange(AddPointToData(line.Value));

            result.AddRange(header);
            result.AddRange(dataLines);
            bool addEndDat = true;
            foreach (var item in result.Where(x => x.ToLower().Trim().Replace(" ", "") == "enddat"))
                addEndDat = false;
            if (addEndDat)
                result.Add("ENDDAT");
            
            return result;
        }

        private static List<string> AddPointToData(PointInSrc line)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(line.Target))
                result.Add(line.Target);
            if (!string.IsNullOrEmpty(line.Fdat))
                result.Add(line.Fdat);
            if (line.Pdat != null)
            {
                foreach (var pdat in line.Pdat)
                {
                    if (!string.IsNullOrEmpty(pdat))
                        result.Add(pdat);
                }
            }
            if (line.Ldat != null)
            {
                foreach (var ldat in line.Ldat)
                {
                    if (!string.IsNullOrEmpty(ldat))
                        result.Add(ldat);
                }
            }
            return result;
        }

        private static IDictionary<string, List<string>> GetDats(List<SrcAndDat> srcsAndDats)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach(var item in srcsAndDats)
            {
                result.Add(item.Dat, item.DatContent);
            }
            return result;
        }

        private static IDictionary<string, List<LineAndParams>> GetFilesBeforeRenumber(List<SrcAndDat> files)
        {
            Regex isViaPointRegex = new Regex(@"(PTP|LIN)\s+X(P|HOME)\d+", RegexOptions.IgnoreCase);
            IDictionary<string, List<LineAndParams>> result = new Dictionary<string, List<LineAndParams>>();
            foreach (SrcAndDat file in files)
            {
                List<LineAndParams> currentList = new List<LineAndParams>();
                foreach (string fold in file.SrcContent)
                {
                    string currentPoint = isViaPointRegex.Match(fold).ToString();
                    if (!string.IsNullOrEmpty(currentPoint) && fold.ToLower().Contains("fdat_act"))
                    {
                        currentList.Add(new LineAndParams(fold, GetParams(fold)));
                    }
                    else
                        currentList.Add(new LineAndParams(fold, null));
                }
                result.Add(file.Src, currentList);
            }
            return result;
        }

        private static PointInSrc GetParams(string fold)
        {
            string type = "", fdat = "", pdat = "", ldat = "",target = "", name = "";
            List<string> pdats = new List<string>();
            List<string> ldats = new List<string>();
            if (fold.ToLower().Trim().Replace(" ","").Contains("\nlin"))
            {
                type = "LIN";
                //Regex lDatRegex = new Regex(@"(?<=LDAT.*\=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                Regex lDatRegex = new Regex(@"(?<=LDAT_ACT\s*=\s*)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
                ldat = lDatRegex.Match(fold.Replace(" ", "")).ToString();
                //pdat = "PPP" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(ldat).ToString());
                pdat = string.Empty;
            }
            else
            {
                type = "PTP";
                //Regex pDatRegex = new Regex(@"(?<=PDAT.*\=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                Regex pDatRegex = new Regex(@"(?<=PDAT_ACT\s*=\s*)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
                pdat = pDatRegex.Match(fold.Replace(" ", "")).ToString();
                //ldat = "LLP" + (new Regex(@"\d+", RegexOptions.IgnoreCase).Match(pdat).ToString());
                ldat = string.Empty;
            }
            if (!string.IsNullOrEmpty(pdat))
                pdats.Add(pdat);
            if (!string.IsNullOrEmpty(ldat))
                ldats.Add(ldat);
            Regex fDatRegex = new Regex(@"(?<=FDAT.*\=)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
            fdat = fDatRegex.Match(fold.Replace(" ", "")).ToString();
            Regex nameRegex = new Regex(@"(?<=FOLD\s+(Swp\s+Positioning\s+(LIN|PTP)|PTP|LIN|JOB.*|COLLISION.*|SPOT.POS.*|StudSWR.SwrLoad(PTP|LIN)\s+([a-zA-Z]+\s*\=\d+\s*,\s*){2}|StudSWR.SwrControl(PTP|LIN)\s+([a-zA-Z]+\s*\=\d+\s*,\s*){2})\s+)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
            name = nameRegex.Match(fold).ToString();
            Regex targetRegex = new Regex(@"(?<=\n\s*(PTP|LIN)\s+)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
            target = targetRegex.Match(fold).ToString();

            PointInSrc result = new PointInSrc(name,type,target,fdat,pdats,ldats);
            return result;
        }

        private static List<SrcAndDat> FindFilesInBackup(string directory, bool isKrc4 = false)
        {
            List<SrcAndDat> result = new List<SrcAndDat>();
            List<string> foundFiles = new List<string>();
            string workDir = "\\KRC\\R1";
            List<string> dirsInMainDir = Directory.GetDirectories(directory + workDir).ToList();
            int counter = 0, dirscount = dirsInMainDir.Count - 1;
            while (dirscount > 0)
            {
                var currentDir = dirsInMainDir[counter];
                if (Directory.GetDirectories(currentDir).ToList().Count > 0)
                {
                    foreach (var direct in Directory.GetDirectories(currentDir).ToList())
                    {
                        dirsInMainDir.Add(direct);
                        dirscount++;
                    }
                }
                dirscount--;
                counter++;
            }
            dirsInMainDir.Add(directory + workDir);

            foreach (string subDir in dirsInMainDir)
            {
                DirectoryInfo dir = new DirectoryInfo(subDir);
                if (Directory.Exists(dir.ToString()))
                {
                    FileInfo[] srcFiles = dir.GetFiles("*.src");
                    foreach (var file in srcFiles)
                    if (!(file.FullName.Contains("tm_useraction") || file.FullName.ToLower().Contains("braketestreq")) || file.FullName.ToLower().Contains("braketestselftest") || file.FullName.ToLower().Contains("masrefreq"))
                        foundFiles.Add(file.FullName);
                }
            }    
            foreach (var srcFile in foundFiles)
            {
                if (File.Exists(srcFile.Replace(".src", ".dat")))
                    result.Add(new SrcAndDat(srcFile, srcFile.Replace(".src", ".dat")));
            }
            return result;
            
        }

        private static string SelectBackup()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            //dialog.Multiselect = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Properties.Settings.Default.Save();
                return dialog.FileName;
            }
            return "";
        }

        public static List<SrcAndDat> DivideToFolds(List<SrcAndDat> files, ZipArchive archive = null)
        {
            List<SrcAndDat> result = new List<SrcAndDat>();
            foreach (var file in files)
            {
                List<string> lines = new List<string>();
                List<string> datLines = new List<string>();
                StreamReader reader;
                if (archive == null)
                    reader = new StreamReader(file.Src);
                else
                    reader = new StreamReader(archive.Entries.First(x => x.FullName.ToLower() == file.Src.ToLower()).Open());
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lines.Add(line);
                }
                reader.Close();
                if (archive == null)
                    reader = new StreamReader(file.Dat);
                else
                    reader = new StreamReader(archive.Entries.First(x => x.FullName.ToLower() == file.Dat.ToLower()).Open());
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    datLines.Add(line);
                }
                reader.Close();
                List<string> resultFolds = new List<string>();
                //foreach (string line in lines.Where(x => x != "\n" && !string.IsNullOrEmpty(x)))
                int foldcounter = 0;
                Regex isFoldRegex = new Regex(@";\s*fold", RegexOptions.IgnoreCase);
                Regex isEndFoldRegex = new Regex(@";\s*endfold", RegexOptions.IgnoreCase);
                string currentFold = string.Empty;
                foreach (string line in lines)
                {
                    if (foldcounter == 0 && !isFoldRegex.IsMatch(line))
                    {
                        resultFolds.Add(line);
                    }
                    if (isFoldRegex.IsMatch(line))
                    {
                        foldcounter++;
                    }
                    if (foldcounter > 0)
                        currentFold += line + "\r\n";
                    if (isEndFoldRegex.IsMatch(line))
                    {
                        foldcounter--;
                        if (foldcounter == 0)
                        {
                            resultFolds.Add(currentFold.TrimEnd());
                            currentFold = string.Empty;
                        }
                    }
                }
                result.Add(new SrcAndDat(file.Src, file.Dat, resultFolds,datLines));
            }
            return result;
        }


        private static IDictionary<string, List<string>> GetUnusedDataInput(IDictionary<string, List<LineAndParams>> filesBeforeRenumber)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var item in filesBeforeRenumber)
            {
                List<string> templist = new List<string>();
                foreach (var line in item.Value)
                    templist.Add(line.Line);
                result.Add(item.Key, templist);
            }
            return result;
        }

        private static List<string> GetDatFiles(ICollection<string> keys)
        {
            List<string> result = new List<string>();
            foreach (var item in keys)
            {
                if (File.Exists(item.Replace(".src", ".dat")))
                    result.Add(item.Replace(".src", ".dat"));
            }
            return result;
        }
    }
}
