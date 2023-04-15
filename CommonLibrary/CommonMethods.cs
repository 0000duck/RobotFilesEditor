using CommonLibrary.RobKalDatCommon;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Packaging;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public static class CommonMethods
    {
        static string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string[] types = { "int ", "real ", "char ", "bool ", "fdat ", "ldat ", "pdat ", "e6axis ", "e6pos ", "struc ", "signal ", "decl " };
        public static string typeRegex = "INT|CHAR|BOOL|REAL|E6AXIS|E6POS|FDAT|PDAT|LDAT|STRUC|SIGNAL|KRLMSGOPT_T|BRAKE_STATE|P00_COMMAND|FUNCT_TYPE|STOPMESS|KrlMsg_T|KrlMsgPar_T|KRLMSGDLGSK_T|PRESET|FRAME|LOAD";


        public static string SelectDirOrFile(bool isDir, string filter1Descr = "", string filter1 = "", string filter2Descr = "", string filter2 = "")
        {
            var dialog = new CommonOpenFileDialog();
            if (isDir)
                dialog.IsFolderPicker = true;
            else
            {
                dialog.IsFolderPicker = false;
                if (!string.IsNullOrEmpty(filter1))
                    dialog.Filters.Add(new CommonFileDialogFilter(filter1Descr, filter1));
                if (!string.IsNullOrEmpty(filter2))
                    dialog.Filters.Add(new CommonFileDialogFilter(filter2Descr, filter2));
            }
            dialog.EnsurePathExists = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
                return "";
        }

        public static bool IsFileLocked(string stringfile)
        {
            FileInfo file = new FileInfo(stringfile);
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static string GetApplicationConfig()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (userName.Length >= 4 && userName.ToLower().Substring(0, 4) == "aiut" || userName.Length >= 6 && userName.ToLower().Substring(0, 6) == "233-ws" || userName.ToLower().Contains("trojniar"))
                CommonGlobalData.IsAIUTUser = true;
            else
                CommonGlobalData.IsAIUTUser = false;
            Regex regex = new Regex(@"(?<=.*\\).*", RegexOptions.IgnoreCase);
            Match match = regex.Match(userName);
            return "C:\\Users\\" + match.ToString() + "\\AppData\\Local\\RobotFilesHarvester\\Application.config";
        }

        public static string GetEntryName(ReadOnlyCollection<ZipArchiveEntry> entries, string entry)
        {
            foreach (var entr in entries)
            {
                if (entr.ToString().ToLower() == entry.ToLower())
                    return entr.ToString();
            }
            return "";
        }

        public static string GetStringFilledWithZeros(int length, int number)
        {
            if (number.ToString().Length >= length)
                return number.ToString();
            string result = number.ToString();
            while (result.Length < length)
            {
                result = "0" + result;
            }
            return result;
        }

        public static string[] GetArrayBasedOnInt(int arrayLength,int number)
        {
            List<string> tempList = new List<string>();
            for (int i = 1; i <=arrayLength;i++)
            {
                if (number - i >= 0)
                {
                    tempList.Add("TRUE");
                }
                else
                    tempList.Add("FALSE");                
            }
            return tempList.ToArray();
        }

        public static bool StringToBool(string v)
        {
            if (v.ToLower().Trim() == "true")
                return true;
            else
                return false;
        }

        public static FoundVariables FindVarsInBackup(string file, bool isGlobal)
        {
            KukaValidationData data = ReadDataFromKukaBackup(file);
            return FindVarsInBackup(data, isGlobal);
        }

        public static KukaValidationData ReadDataFromKukaBackup(string file)
        {
            Dictionary<string, List<string>> srcFiles = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> datFiles = new Dictionary<string, List<string>>();
            using (ZipArchive archive = ZipFile.Open(file, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries.Where(x => Path.GetExtension(x.FullName).ToLower() == ".src"))
                {
                    List<string> srcFileContent = new List<string>();
                    StreamReader reader = new StreamReader(entry.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        srcFileContent.Add(line);
                    }
                    reader.Close();
                    srcFiles.Add(entry.FullName, srcFileContent);
                }

                foreach (var entry in archive.Entries.Where(x => Path.GetExtension(x.FullName).ToLower() == ".dat"))
                {
                    List<string> datFileContent = new List<string>();
                    StreamReader reader = new StreamReader(entry.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        datFileContent.Add(line);
                    }
                    reader.Close();
                    datFiles.Add(entry.FullName, datFileContent);
                }
                return new KukaValidationData(file, srcFiles, datFiles);
            }
        }

        public static FoundVariables FindVarsInBackup(KukaValidationData data, bool isGlobal)
        {
            Regex globalFileRegex = new Regex(@"(A|B|C)\d+_.*global.dat", RegexOptions.IgnoreCase);
            Regex isVarDeclarationRegex = new Regex(@"^\s*(DECL|)\s*(GLOBAL|)\s*(" + typeRegex + ")", RegexOptions.IgnoreCase);
            FoundVariables foundVars = new FoundVariables();
            Dictionary<string, List<string>> allfiles = new Dictionary<string, List<string>>();
            foreach (var src in data.SrcFiles)
                allfiles.Add(src.Key, src.Value);
            foreach (var dat in data.DatFiles)
                allfiles.Add(dat.Key, dat.Value);
            string doubleDecls = "";
            foreach (var entry in allfiles)
            {
                int lineCounter = 1;
                bool isConfigDat = entry.Key.ToLower().Contains("$config") ? true : false;
                bool tempIsGlobal = entry.Key.ToLower().Contains("$config") ? true : isGlobal;
                //var currentFile = archive.GetEntry(GetEntryName(archive.Entries, entry.ToString()));
                //StreamReader reader = new StreamReader(currentFile.Open());
                foreach (var line in entry.Value)
                {
                    if (isVarDeclarationRegex.IsMatch(line) && CheckGlobal(line, isGlobal, isConfigDat))
                    {
                        var currentVariable = new Variable(line, tempIsGlobal, Path.GetFileNameWithoutExtension(entry.Key), lineCounter);
                        List<KeyValuePair<string, Variable>> currentVarList = new List<KeyValuePair<string, Variable>>();
                        currentVarList.Add(new KeyValuePair<string, Variable>(currentVariable.Name + " " + currentVariable.Localization, currentVariable));
                        if (currentVariable.Names != null)
                        {
                            int counter = 0;
                            foreach (var varName in currentVariable.Names)
                            {
                                if (counter > 0)
                                    currentVarList.Add(new KeyValuePair<string, Variable>(varName + " " + currentVariable.Localization, new Variable(varName, currentVariable.Type, currentVariable.IsGlobal, currentVariable.Localization, line, lineCounter)));
                                counter++;
                            }
                        }

                        foreach (var currentVar in currentVarList)
                        {
                            if (!String.IsNullOrEmpty(currentVar.Key))
                            {
                                if (currentVar.Value.Type == "int")
                                {
                                    if (!foundVars.INTs.Keys.Contains(currentVar.Key))
                                        foundVars.INTs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "real")
                                {
                                    if (!foundVars.REALs.Keys.Contains(currentVar.Key))
                                        foundVars.REALs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "char")
                                {
                                    if (!foundVars.CHARs.Keys.Contains(currentVar.Key))
                                        foundVars.CHARs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "bool")
                                {
                                    if (!foundVars.BOOLs.Keys.Contains(currentVar.Key))
                                        foundVars.BOOLs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "fdat")
                                {
                                    if (!foundVars.FDATs.Keys.Contains(currentVar.Key))
                                        foundVars.FDATs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "ldat")
                                {
                                    if (!foundVars.LDATs.Keys.Contains(currentVar.Key))
                                        foundVars.LDATs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "pdat")
                                {
                                    if (!foundVars.PDATs.Keys.Contains(currentVar.Key))
                                        foundVars.PDATs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "e6axis")
                                {
                                    if (!foundVars.E6AXIS.Keys.Contains(currentVar.Key))
                                        foundVars.E6AXIS.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "e6pos")
                                {
                                    if (!foundVars.E6POS.Keys.Contains(currentVar.Key))
                                        foundVars.E6POS.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "struc")
                                {
                                    if (!foundVars.STRUCTs.Keys.Contains(currentVar.Key))
                                        foundVars.STRUCTs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else if (currentVar.Value.Type == "signal")
                                {
                                    if (!foundVars.SIGNALs.Keys.Contains(currentVar.Key))
                                        foundVars.SIGNALs.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                                else
                                {
                                    if (!foundVars.Others.Keys.Contains(currentVar.Key))
                                        foundVars.Others.Add(currentVar.Key, currentVar.Value);
                                    else
                                        doubleDecls += currentVar.Key + "\r\n";
                                }
                            }
                        }
                    }
                    lineCounter++;
                }
                //if (globalFileRegex.IsMatch(Path.GetFileName(entry.Key)))
                //{
                //    reader = new StreamReader(currentFile.Open());
                //    //foundVars.GlobalDat.Add(Path.GetFileName(currentFile.FullName), reader.ReadToEnd());
                //    reader.Close();
                //}
            }

            return foundVars;
        }

        private static bool CheckGlobal(string line, bool shouldBeGlobal, bool isConfigDat)
        {
            Regex isGlobalRegex = new Regex(@"^\s*(|DECL\s+)GLOBAL", RegexOptions.IgnoreCase);
            if (shouldBeGlobal)
            {
                if (isGlobalRegex.IsMatch(line) || isConfigDat)
                    return true;
                else
                    return false;
            }
            else
            {
                if (isGlobalRegex.IsMatch(line) || isConfigDat)
                    return false;
                else
                    return true;
            }
        }

        private static List<KeyValuePair<string, Variable>> GetVariable(string line, string fileName, bool isGlobal)
        {
            Regex typeRegex = new Regex(@"(?<=^\s*(DECL|)\s*(GLOBAL|)\s*)(INT|CHAR|BOOL|REAL|E6AXIS|E6POS|FDAT|PDAT|LDAT|STRUC|SIGNAL)", RegexOptions.IgnoreCase);
            List<KeyValuePair<string, Variable>> result = new List<KeyValuePair<string, Variable>>();



            return result;
        }

        public static void CopyFileToWorkFolder(string workingfolder, string[] pathElements)
        {
            // { ProjectName, Subfolder, FileName }
            //Assembly.GetEntryAssembly().Location;
            string path = Environment.CurrentDirectory;
            while (path.Length > 6)
            {
                bool valueFound = false;
                var test = Directory.GetDirectories(path);
                foreach (var item in test.Where(x => x.ToLower().Contains(pathElements[0].ToLower())))
                {
                    valueFound = true;
                    break;
                }
                if (valueFound)
                    break;
                path = Path.GetFullPath(Path.Combine(path, ".."));
            }
            //File.Copy(Path.Combine(path, "RobKalDat", "ExternalFiles", "templateBase.xml"), Path.Combine(workingfolder, "templateBase.xml"));
            File.Copy(Path.Combine(path, pathElements[0], pathElements[1], pathElements[2]), Path.Combine(workingfolder, pathElements[2]));
        }

        //public static void CreateLogFile(string filecontent, string path)
        //{
        //    string directory = Path.GetDirectoryName(CommonGlobalData.ConfigurationFileName);
        //    if (filecontent != "")
        //    {
        //        File.Delete(directory + path);
        //        using (StreamWriter sw = File.AppendText(directory + path))
        //        {
        //            sw.Write(filecontent);
        //            sw.Close();
        //            MessageBox.Show("Log file has been created at " + directory + path);
        //            System.Diagnostics.Process.Start(directory + path);

        //        }

        //    }
        //}

        public static string ToExcelCoordinates(string coordinates)
        {
            string first = coordinates.Substring(0, coordinates.IndexOf(','));
            int i = int.Parse(first);
            string second = coordinates.Substring(first.Length + 1);

            string str = string.Empty;
            while (i > 0)
            {
                str = ALPHABET[(i - 1) % 26] + str;
                i /= 26;
            }

            return str + second;
        }

        public static string ToNumericCoordinates(string coordinates)
        {
            string first = string.Empty;
            string second = string.Empty;

            CharEnumerator ce = coordinates.GetEnumerator();
            while (ce.MoveNext())
                if (char.IsLetter(ce.Current))
                    first += ce.Current;
                else
                    second += ce.Current;

            int i = 0;
            ce = first.GetEnumerator();
            while (ce.MoveNext())
                i = (26 * i) + ALPHABET.IndexOf(ce.Current) + 1;

            string str = i.ToString();
            return str + "," + second;
        }

        public static Point CalculateBases(Point robot, Point meas, Point isTCP = null, bool isSafety = false)
        {
            double[,] rotationByZ = null, rotationByY= null, rotationByX = null;
            //wspolrzedne basy po przesunieciu robota do wspolrzednych 0,0,0,0,0,0
            
            if (isTCP == null)
            {
                Point robotMovedTo0Point = new Point(meas.XPos - robot.XPos, meas.YPos - robot.YPos, meas.ZPos - robot.ZPos, ConvertToRadians(robot.RX * -1), ConvertToRadians(robot.RY * -1), ConvertToRadians(robot.RZ * -1));
                // meas1 - robot, meas2 - wspolrzedne bazy
                rotationByZ = MatrixOperations.MultiplyMatrix(new double[,] { { Math.Cos(robotMovedTo0Point.RZ), -Math.Sin(robotMovedTo0Point.RZ), 0 }, { Math.Sin(robotMovedTo0Point.RZ), Math.Cos(robotMovedTo0Point.RZ), 0 }, { 0, 0, 1 } }, new double[,] { { robotMovedTo0Point.XPos }, { robotMovedTo0Point.YPos }, { robotMovedTo0Point.ZPos } });
                rotationByY = MatrixOperations.MultiplyMatrix(new double[,] { { Math.Cos(robotMovedTo0Point.RY), 0, Math.Sin(robotMovedTo0Point.RY) }, { 0, 1, 0 }, { -Math.Sin(robotMovedTo0Point.RY), 0, Math.Cos(robotMovedTo0Point.RY) } }, new double[,] { { rotationByZ[0, 0] }, { rotationByZ[1, 0] }, { rotationByZ[2, 0] } });
                rotationByX = MatrixOperations.MultiplyMatrix(new double[,] { { 1, 0, 0 }, { 0, Math.Cos(robotMovedTo0Point.RX), -Math.Sin(robotMovedTo0Point.RX) }, { 0, Math.Sin(robotMovedTo0Point.RX), Math.Cos(robotMovedTo0Point.RX) } }, new double[,] { { rotationByY[0, 0] }, { rotationByY[1, 0] }, { rotationByY[2, 0] } });
            }
            else
            {
                Point measToRadians = new Point(meas.XPos, meas.YPos, meas.ZPos, ConvertToRadians(meas.RX ), ConvertToRadians(meas.RY), ConvertToRadians(meas.RZ));
                Point tcpToRadians = new Point(isTCP.XPos, isTCP.YPos, isTCP.ZPos, ConvertToRadians(isTCP.RX), ConvertToRadians(isTCP.RY), ConvertToRadians(isTCP.RZ));
                rotationByX = MatrixOperations.MultiplyMatrix(new double[,] { { 1, 0, 0 }, { 0, Math.Cos(tcpToRadians.RX + measToRadians.RX), -Math.Sin(tcpToRadians.RX + measToRadians.RX) }, { 0, Math.Sin(tcpToRadians.RX + measToRadians.RX), Math.Cos(tcpToRadians.RX + measToRadians.RX) } }, new double[,] { { tcpToRadians.XPos }, { tcpToRadians.YPos }, { tcpToRadians.ZPos } });
                rotationByY = MatrixOperations.MultiplyMatrix(new double[,] { { Math.Cos(tcpToRadians.RY + measToRadians.RY), 0, Math.Sin(tcpToRadians.RY + measToRadians.RY) }, { 0, 1, 0 }, { -Math.Sin(tcpToRadians.RY + measToRadians.RY), 0, Math.Cos(tcpToRadians.RY + measToRadians.RY) } }, new double[,] { { rotationByX[0, 0] }, { rotationByX[1, 0] }, { rotationByX[2, 0] } });
                rotationByZ = MatrixOperations.MultiplyMatrix(new double[,] { { Math.Cos(tcpToRadians.RZ + measToRadians.RZ), -Math.Sin(tcpToRadians.RZ + measToRadians.RZ), 0 }, { Math.Sin(tcpToRadians.RZ + measToRadians.RZ), Math.Cos(tcpToRadians.RZ + measToRadians.RZ), 0 }, { 0, 0, 1 } }, new double[,] { { rotationByY[0, 0] }, { rotationByY[1, 0] }, { rotationByY[2, 0] } });
                Point tempresult = new Point(measToRadians.XPos + rotationByZ[0, 0], measToRadians.YPos + rotationByZ[1, 0], measToRadians.ZPos + rotationByZ[2, 0], GetRobKalDatAngle(meas.RZ, robot.RZ), GetRobKalDatAngle(meas.RY, robot.RY), GetRobKalDatAngle(meas.RX, robot.RX));
            }
            
            if (!isSafety)
                return new Point(rotationByX[0, 0], rotationByX[1, 0], rotationByX[2, 0], GetRobKalDatAngle(meas.RZ, robot.RZ), GetRobKalDatAngle(meas.RY, robot.RY), GetRobKalDatAngle(meas.RX, robot.RX));
            else
                return new Point(rotationByX[0, 0], rotationByX[1, 0], rotationByX[2, 0], GetRobKalDatAngle(meas.RX, robot.RX), GetRobKalDatAngle(meas.RY, robot.RY), GetRobKalDatAngle(meas.RZ, robot.RZ));

        }

        public static double GetRobKalDatAngle(double rX1, double rX2)
        {
            double difference = rX1 - rX2;
            if (Math.Abs(difference) > 180)
            {
                if (difference > 180)
                    return difference - 360;
                else
                    return difference + 360;
            }
            return difference;
        }

        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static double ConvertToDegrees(double angle)
        {
            return 180/ Math.PI * angle;
        }

        public static List<string> FindBackupsInDirectory(string selectedDir, bool onlyWelding = true, bool includeSafeRobot = true)
        {
            List<string> result = new List<string>();
            List<string> list = new List<string>();
            List<string> allfiles = Directory.GetFiles(selectedDir, "*.zip", SearchOption.AllDirectories).ToList();
            if (!includeSafeRobot)
            {
                var saferobots = allfiles.Where(x => x.ToLower().Contains("_saferobot.zip"));               
                saferobots.ToList().ForEach(x=>allfiles.Remove(x));
            }
            foreach (string file in allfiles)
            {
                try
                {
                    //
                    using (ZipArchive archive = ZipFile.Open(file, ZipArchiveMode.Read))
                    {
                        //archive.CreateEntryFromFile(newFile, "NewEntry.txt");
                        //archive.ExtractToDirectory(extractPath);


                        // var zp = ZipPackage.Open(file,
                        //                  FileMode.Open,
                        //                  FileAccess.Read,
                        //                  FileShare.Read) as ZipPackage;
                        // list = zp.GetFileList();
                        archive.Entries.ToList().ForEach(x=>list.Add(x.FullName));
                        if (onlyWelding)
                        {
                            if (CheckIfBackupAndWelding(list))
                                result.Add(file);
                        }
                        else
                            result.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Archive " + Path.GetFileNameWithoutExtension(file) + " is corrupted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return result;
        }

        private static bool CheckIfBackupAndWelding(List<string> list)
        {
            bool isWelding = false, isBackup = false;
            foreach (var file in list)
            {
                if (file.Length >= 3 && file.ToLower().Substring(0, 3) == "krc")
                {
                    isBackup = true;
                    break;
                }
            }
            if (isBackup)
            {
                foreach (var file in list)
                {
                    if (file.ToLower().Contains("spot_global.dat") || file.ToLower().Contains("pick_global.dat") || file.ToLower().Contains("a20_swh_global.dat") || file.ToLower().Contains("a21_swr_global.dat") || file.ToLower().Contains("b20_snh_global.dat") || file.ToLower().Contains("a13_rvt_global.dat") || file.ToLower().Contains("a22_fls_global.dat") || file.ToLower().Contains("a27_brt_global.dat") || file.ToLower().Contains("a04_swp_global.dat"))
                    {
                        isWelding = true;
                        break;
                    }
                }
            }
            if (isBackup && isWelding)
                return true;
            return false;
        }

        public static List<string> GetFileList(this ZipPackage zp)
        {
            List<string> list = null;

            try
            {
                var zipArchiveProperty = zp.GetType().GetField("_zipArchive", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);

                var zipArchive = zipArchiveProperty.GetValue(zp);

                var zipFileInfoDictionaryProperty = zipArchive.GetType().GetProperty("ZipFileInfoDictionary", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);

                var zipFileInfoDictionary =
                    zipFileInfoDictionaryProperty.GetValue(zipArchive, null) as Hashtable;

                var query = from System.Collections.DictionaryEntry de in zipFileInfoDictionary
                            select de.Key.ToString();

                list = query.ToList();
            }
            catch (NotSupportedException nse)
            {
                throw;
            }
            return list;
        }

        public static string RemoveComment(string line)
        {
            if (!line.Contains(";"))
                return line;
            else
            {
                Regex removeRegex = new Regex(@";.*", RegexOptions.IgnoreCase);
                return removeRegex.Replace(line, "");
            }

        }

        public static bool ContainsAny(this List<string> currentString, string[] excludedStrings)
        {
            foreach (string substring in excludedStrings)
            {
                foreach (var item in currentString)
                {
                    if (substring.ToLower() == item.ToLower())
                        return true;
                }
            }
            return false;
        }

        public static async Task WriteAllTextAsync(string path, string text)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                await streamWriter.WriteAsync(text);
            }
        }

        public static List<string> GetRenumberedBody(List<string> body)
        {
            Regex lineNumberRegex = new Regex(@"(?<=^\s*)\d+\s*(?=\:)", RegexOptions.IgnoreCase);
            List<string> result = new List<string>();
            int counter = 1;
            foreach (string line in body)
            {
                string tempLine = line.TrimStart();
                if (string.IsNullOrEmpty(line.Trim().Replace(" ", "")))
                {
                    result.Add(AddSpaces(counter.ToString()) + ":   ;");
                }
                else
                    result.Add(lineNumberRegex.Replace(tempLine, AddSpaces(counter.ToString())));
                counter++;
            }

            return result;
        }

        private static string AddSpaces(string numberString)
        {
            string tempstring = string.Empty;
            for (int i = numberString.Length; i < 4; i++)
            {
                tempstring += " ";
            }
            return tempstring + numberString;

        }

        public static List<double> RadToQuat(double heading, double attitude, double bank)
        {
            // Assuming the angles are in radians.
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            double w = c1c2 * c3 - s1s2 * s3;
            double x = c1c2 * s3 + s1s2 * c3;
            double y = s1 * c2 * c3 + c1 * s2 * s3;
            double z = c1 * s2 * c3 - s1 * c2 * s3;
            List<double> quaternion = new List<double>();
            quaternion.Add(w);
            quaternion.Add(y);
            quaternion.Add(z);
            quaternion.Add(-x);
            return quaternion;
        }

        public static List<T> CloneList<T>(List<T> inputList)
        {
            List<T> result = new List<T>();
            foreach (var item in inputList)
                result.Add(item);
            return result;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(List<T> inputList)
        {
            ObservableCollection<T> result = new ObservableCollection<T>();
            foreach (var item in inputList)
                result.Add(item);
            return result;
        }

        public static bool HasSpecialChars(string inputString)
        {
            return inputString.Replace("_", "").Replace("-", "").Any(ch => !Char.IsLetterOrDigit(ch));
        }
    }
}

