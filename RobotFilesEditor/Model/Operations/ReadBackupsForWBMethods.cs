using CommonLibrary;
using Microsoft.Office.Interop.Excel;
using Microsoft.WindowsAPICodePack.Dialogs;
using RobotFilesEditor.Dialogs.SetRobotStartAddress;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace RobotFilesEditor.Model.Operations
{
    public static class ReadBackupsForWBMethods
    {
        static int robotStartAddress;

        static RobotData data;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private static Microsoft.Office.Interop.Excel.Application oXL;
        private static Workbooks oWBs;
        private static Workbook oWB;
        private static Microsoft.Office.Interop.Excel.Sheets sheets;
        private static Microsoft.Office.Interop.Excel.Worksheet oSheet;

        public static void Execute(string robotType)
        {
            XElement element = XElement.Load(@"C:\Users\ttrojniar\Desktop\82HRL2\main.xml");
            if (robotType == "KUKA")
                TypeRobotStartAddress();            
            ReadFile(robotType);
            if (data != null)
                WriteXLS(robotType);
        }

        private static void TypeRobotStartAddress()
        {
            bool reopen = true;
            while (reopen)
            {
                var vm = new SetRobotStartAddressVM();
                SetRobotStartAddress sW = new SetRobotStartAddress(vm);
                var dialogResult = sW.ShowDialog();
                int.TryParse(vm.Address, out robotStartAddress);
                if (robotStartAddress == 0)
                    reopen = true;
                else
                    reopen = false;
            }
        }

        private static List<int> FindExistingTools(List<ToolName> toolNames)
        {
            List<int> existingTools = new List<int>();
            for (int i = 0; i < toolNames.Count; i++)
            {
                if (toolNames[i].Name != " ")
                    existingTools.Add(i + 1);
            }
            return existingTools;
        }

        private static List<int> FindExistingLoadVars(List<ILoadData> loadvars)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < loadvars.Count; i++)
            {
                if (loadvars[i].Mass != -1)
                    result.Add(i + 1);
            }
            return result;
        }

        private static List<int> FindExistingBases(List<BaseName> baseNames)
        {
            List<int> existingTools = new List<int>();
            for (int i = 0; i < baseNames.Count; i++)
            {
                if (baseNames[i].Name != " ")
                    existingTools.Add(i + 1);
            }
            return existingTools;
        }


        private static void ReadFile(string controller)
        {
            data = null;
            var dialog = new CommonOpenFileDialog();
            //dialog.Filters.Add(new CommonFileDialogFilter("dat file (*.dat)", ".dat"));
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Properties.Settings.Default.Save();
                if (controller == "KUKA")
                    data = ReadBackupKUKA(dialog.FileName);
                else if (controller == "ABB")
                    data = ReadBackupABB(dialog.FileName);
            }
        }

        private static RobotData ReadBackupABB(string directory)
        {
            RobotData data = new RobotData();
            List<ILoadData> toolloadDatas;
            List<ILoadData> loadDatas;
            List<ToolName> toolnames;
            List<IBaseData> baseDatas;
            List<BaseName> basenames;
            List<HomePos> homePoses;
            List<HomeNameLong> homeNamesLong;
            List<HomeNameShort> homeNamesShort;
            List<IUserBit> userbitsOut;
            List<IUserBit> userbitsIn;
            List<IUserBit> typBits;
            List<IUserBit> jobEnables;

            string file = Path.Combine(directory,"RAPID","TASK1", "PROGMOD", "ComUser.mod");
            if (!File.Exists(file))
                return null;
            data.GripperConfig = GetGripperConfigAbb(directory);
            data.SafetyConfig = GetSafetyABB(directory);
            data.ToolDatas = GetToolDatasABB(directory, out toolloadDatas, out loadDatas, out toolnames, out baseDatas, out basenames, out homePoses, out homeNamesLong, out homeNamesShort, out userbitsIn, out userbitsOut, out typBits, out jobEnables);
            data.ToolNames = toolnames;
            data.LoadDatas = toolloadDatas;
            data.LoadVars = loadDatas;
            data.BaseDatas = baseDatas;
            data.BaseNames = basenames;
            data.HomePoses = homePoses;
            data.UserbitsIn = userbitsIn;
            data.UserbitsOut = userbitsOut;
            data.TypBits = typBits;
            data.JobEnables = jobEnables;
            data.HomeNameLongs = homeNamesLong;
            data.HomeNameShorts = homeNamesShort;
            data.Programs = GetProgramsABB(directory);
            data.CollZones = GetCollZonesABB(directory);
            

            return data;
        }

        private static RobotData ReadBackupKUKA(string directory)
        {
            string file = directory + "\\KRC\\R1\\System\\$config.dat";

            RobotData data = new RobotData();
            if (File.Exists(file))
            {
                GetGripperFromConfigDatMethods.ReadConfigDat(file, false);
                data.GripperConfig = GlobalData.SignalNames;
                data.SafetyConfig = CommonLibrary.SafetyKUKAMethods.GetSafetyConfig(directory);
                data.ToolDatas = GetToolDatas(file);
                data.ToolNames = GetToolNames(file);
                data.LoadDatas = GetLoadDatas(file, "LOADDATA");
                data.LoadVarNames = GetLoadVarNames(file);
                data.LoadVars = GetLoadDatas(file, "LOADVAR");
                data.ToolTypes = GetToolTypes(file);
                data.BaseDatas = GetBaseDatas(file);
                data.BaseNames = GetBaseNames(file);
                data.BaseTypes = GetBaseTypes(file);
                data.HomePoses = GetHomePoses(file);
                data.HomeNameLongs = GetHomeNameLongs(file);
                data.HomeNameShorts = GetHomeNameShorts(file);
            }
            else
            {
                data.ToolNames = new List<ToolName>();
                data.LoadVars = new List<ILoadData>();
                data.BaseNames = new List<BaseName>();
            }
            data.Programs = GetProgramsKUKA(directory);
            List<FileInfo> files = GetFileList(directory);
            data.CollZones = GetCollZones(files, true);
            data.CollZonesFromCommnad = GetCollZones(files, false);
            data.UserbitsOut = GetUserBit(files, true);
            data.UserbitsIn = GetUserBit(files, false);

            if (data.CollZones.Count != data.CollZonesFromCommnad.Count)
                data.CollZonesWithoutDescr = GetCollZonesWithoutDescr(data.CollZones, data.CollZonesFromCommnad);
            file = directory + "\\KRC\\R1\\InputData.src";
            if (File.Exists(file))
            {
                data.BaseDatasInputData = GetBaseDatas(file);
                data.BaseNamesInputData = GetBaseNames(file);
                data.BaseNumbersInputData = GetBaseNrs(file);
            }
            else
            {
                file = directory + "\\InputData.src";
                if (File.Exists(file))
                {
                    data.BaseDatasInputData = GetBaseDatas(file);
                    data.BaseNamesInputData = GetBaseNames(file);
                    data.BaseNumbersInputData = GetBaseNrs(file);
                }
                else
                {
                    System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("InputData not found in " + file + ". Do you want to continue without offline values of bases?", "InputData not found!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        data.BaseDatasInputData = new List<IBaseData>();
                        data.BaseNamesInputData = new List<BaseName>();
                        data.BaseNumbersInputData = new List<int>();
                    }
                    else
                        return null;
                }
            }

            return data;
        }

        private static List<CollZone> SortCollZones(List<CollZone> collZones)
        {
            List<int> collList = new List<int>();
            int counter = 0;
            
            foreach (var collzone in collZones)
            {
                collList.Add(int.Parse(collzone.Number));
            }
            int[] colls = collList.ToArray();
            Array.Sort(colls);
            List<CollZone> result = new List<CollZone>();
            foreach (int collNum in colls)
            {
                foreach (CollZone zone in collZones.Where(x => x.Number == collNum.ToString()))
                    result.Add(zone);
            }
            return result;
        }       

        private static List<Program> GetProgramsKUKA(string directory)
        {
            List<Program> result = new List<Program>();
            directory = directory + "\\KRC\\R1\\";
            if (!Directory.Exists(directory))
                return result;
            string tempdirectory = directory + "Program";
            List<string> filesInProgram = Directory.GetFiles(tempdirectory).ToList();
            List<string> filteredPrograms = new List<string>();
            foreach (string file in filesInProgram.Where(x => !x.Contains(".dat") && !x.Contains("tm_useraction")))
            {
                filteredPrograms.Add(Path.GetFileNameWithoutExtension(file));
            }

            foreach (string program in filteredPrograms)
            {
                bool descrFound = false;
                Regex getWerkzuegRegex = new Regex(@"(?<=_)[a-zA-Z0-9]*(?=_)", RegexOptions.IgnoreCase);
                if (program.ToLower().Contains("pick") && !program.ToLower().Contains("pickpos") && !program.ToLower().Contains("pick_drop") && !descrFound)
                {
                    result.Add(new Program(program, "Holen von Werkzeug " + getWerkzuegRegex.Match(program).ToString()));
                    descrFound = true;
                }
                if (program.ToLower().Contains("drop") && !program.ToLower().Contains("droppos") && !program.ToLower().Contains("pick_drop") && !descrFound)
                {
                    result.Add(new Program(program, "Ablegen in Werkzeug " + getWerkzuegRegex.Match(program).ToString()));
                    descrFound = true;
                }
                if (program.ToLower().Contains("spot") && !program.ToLower().Contains("spotpos") && !descrFound)
                {
                    result.Add(new Program(program, "Schweissen in Werkzeug " + getWerkzuegRegex.Match(program).ToString()));
                    descrFound = true;
                }
                if (program.ToLower().Contains("search") && !program.ToLower().Contains("searchpos") && !descrFound)
                {
                    result.Add(new Program(program, "Suchen in Werkzeug " + getWerkzuegRegex.Match(program).ToString()));
                    descrFound = true;
                }
                if (program.ToLower().Contains("stack") && !program.ToLower().Contains("stackpos") && !descrFound)
                {
                    result.Add(new Program(program, "Palletieren in Werkzeug " + getWerkzuegRegex.Match(program).ToString()));
                    descrFound = true;
                }
                if (program.ToLower().Contains("stud") && !program.ToLower().Contains("studpos") && !descrFound)
                {
                    result.Add(new Program(program, "Bolzenschweissen in Werkzeug " + getWerkzuegRegex.Match(program).ToString()));
                    descrFound = true;
                }
                if (program.ToLower().Contains("glue") && !program.ToLower().Contains("gluepos") && !descrFound)
                {
                    result.Add(new Program(program, "Kleben in Werkzeug " + getWerkzuegRegex.Match(program).ToString()));
                    descrFound = true;
                }
                if (new Regex(@"h\d+.*pos\d*", RegexOptions.IgnoreCase).IsMatch(program))
                {
                    result.Add(new Program(program, "von Grundsterllung zu ZentralPos fahren"));
                    descrFound = true;
                }
                if (new Regex(@".*pos\d*_h\d+", RegexOptions.IgnoreCase).IsMatch(program))
                {
                    result.Add(new Program(program, "von ZentralPos zu Grundsterllung fahren"));
                    descrFound = true;
                }
                if (!descrFound)
                    result.Add(new Program(program, "Beschreibung nicht gefunden"));
            }

            tempdirectory = directory + "BMW_Std_User";
            filesInProgram = Directory.GetFiles(tempdirectory).ToList();
            foreach (string file in filesInProgram.Where(x => !x.Contains(".dat")))
            {
                if (file.ToLower().Contains("service_pr"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Wartung"));
                if (file.ToLower().Contains("grp_change_pr"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Greifer wechseln"));
                if (file.ToLower().Contains("tip_dress_pr"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Kappenfraesen"));
                if (file.ToLower().Contains("tip_change_pr"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Kappenwechseln"));
                if (file.ToLower().Contains("dock") && !file.ToLower().Contains("undock"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Werkzeug Andocken"));
                if (file.ToLower().Contains("undock"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Werkzeug Abdocken"));
                if (file.ToLower().Contains("gun_change_pr"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Schweisszange Wechseln"));
                if (file.ToLower().Contains("resistorscaling_pr"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Wiederstand Skalierung"));
                if (file.ToLower().Contains("pruefplatz"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Klebe Prufen"));
                if (file.ToLower().Contains("gl_system_purge"))
                    result.Add(new Program(Path.GetFileNameWithoutExtension(file), "Spulen"));
            }

            return result;
        }

        private static List<FileInfo> GetFileList(string directory)
        {
            List<FileInfo> files = new List<FileInfo>();
            string currentDir = directory + "\\KRC\\R1\\Program";
            string currentDirFromOLP = directory + "\\Program";
            if (Directory.Exists(currentDir))
                files.AddRange(GetFilesFromDir(currentDir, "*.src"));
            else
            {
                if (Directory.Exists(currentDirFromOLP))
                    files.AddRange(GetFilesFromDir(currentDirFromOLP, "*.src"));
                else
                    MessageBox.Show("Directiories " + currentDir + " and " + currentDirFromOLP + " do not exist!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            currentDir = directory + "\\KRC\\R1\\BMW_Std";
            currentDirFromOLP = directory + "BMW_Std";
            if (Directory.Exists(currentDir))
                files.AddRange(GetFilesFromDir(currentDir, "*.src"));
            else
            {
                if (Directory.Exists(currentDirFromOLP))
                    files.AddRange(GetFilesFromDir(currentDirFromOLP, "*.src"));
                else
                    MessageBox.Show("Directiories " + currentDir + " and " + currentDirFromOLP + " do not exist!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            currentDir = directory + "\\KRC\\R1\\BMW_Std_User";
            currentDirFromOLP = directory + "\\BMW_Std_User";
            if (Directory.Exists(currentDir))
                files.AddRange(GetFilesFromDir(currentDir, "*.src"));
            else
            {
                if (Directory.Exists(currentDirFromOLP))
                    files.AddRange(GetFilesFromDir(currentDirFromOLP, "*.src"));
                else
                    MessageBox.Show("Directiories " + currentDir + " and " + currentDirFromOLP + " do not exist!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            currentDir = directory + "\\KRC\\R1\\TP\\SafeRobot";
            currentDirFromOLP = directory + "\\TP\\SafeRobot";
            if (Directory.Exists(currentDir))
                files.AddRange(GetFilesFromDir(currentDir, "*.src"));
            else
            {
                if (Directory.Exists(currentDirFromOLP))
                    files.AddRange(GetFilesFromDir(currentDirFromOLP, "*.src"));
                else
                    MessageBox.Show("Directiories " + currentDir + " and " + currentDirFromOLP + " do not exist!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return files;
        }

        private static FileInfo[] GetFilesFromDir(string currentDir, string extension)
        {
            DirectoryInfo dir = new DirectoryInfo(currentDir);
            FileInfo[] filesProgram = dir.GetFiles(extension);
            return filesProgram;
        }

        private static List<int> GetBaseNrs(string file)
        {
            List<int> result = new List<int>();
            Regex regex = new Regex(@"((?<=BASE_DATA\[)[0-9]*(?=\]))", RegexOptions.IgnoreCase);
            var reader = new StreamReader(file);
            int currentNumber = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("BASE_DATA[") & !line.Contains("DECL"))
                {
                    Match match = regex.Match(line);
                    currentNumber = int.Parse(match.ToString());
                    result.Add(currentNumber);
                }
            }
            return result;
        }

        private static List<IToolData> GetToolDatas(string file)
        {
            List<IToolData> result = new List<IToolData>();
            List<string> foundValues = new List<string>();
            Regex regex = new Regex(@"((?<=X )-[0-9]*\.[0-9]*)|((?<=X )[0-9]*\.[0-9]*|(?<=Y )-[0-9]*\.[0-9]*)|((?<=Y )[0-9]*\.[0-9]*|(?<=Z )-[0-9]*\.[0-9]*)|((?<=Z )[0-9]*\.[0-9]*|(?<=A )-[0-9]*\.[0-9]*)|((?<=A )[0-9]*\.[0-9]*|(?<=B )-[0-9]*\.[0-9]*)|((?<=B )[0-9]*\.[0-9]*|(?<=C )-[0-9]*\.[0-9]*)|((?<=C )[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                foundValues = new List<string>();
                var line = reader.ReadLine();
                if (line.Contains("TOOL_DATA[") & !line.Contains("DECL"))
                {
                    foreach (Match currentMatch in regex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    ToolDataKuka currentData = new ToolDataKuka(float.Parse(foundValues[0].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[1].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[2].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[3].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[4].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[5].ToString(), CultureInfo.InvariantCulture));
                    result.Add(currentData);
                }
            }
            return result;
        }

        private static List<ToolName> GetToolNames(string file)
        {
            List<ToolName> result = new List<ToolName>();
            Regex regex = new Regex("(?<=\").*(?=\")", RegexOptions.IgnoreCase);
            Match match;
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("TOOL_NAME[") & !line.Contains("DECL"))
                {
                    match = regex.Match(line);
                    ToolName name = new ToolName(match.ToString());
                    result.Add(name);
                }
            }
            return result;
        }

        private static List<ILoadData> GetLoadDatas(string file, string type)
        {
            List<ILoadData> result = new List<ILoadData>();
            List<int> loadVarSaves = new List<int>();
            List<string> foundValues = new List<string>();
            string searchPhrase = "";
            if (type == "LOADVAR")
            {
                searchPhrase = "LOAD_VAR[";
            }
            else
            {
                loadVarSaves = GetLoadVarSaves(file);
                searchPhrase = "LOAD_DATA[";
            }
            Regex regex = new Regex(@"((?<=M )-[0-9]*\.[0-9]*)|((?<=M )[0-9]*\.[0-9]*|(?<=X )-[0-9]*\.[0-9]*)|((?<=X )[0-9]*\.[0-9]*|(?<=Y )-[0-9]*\.[0-9]*)|((?<=Y )[0-9]*\.[0-9]*|(?<=Z )-[0-9]*\.[0-9]*)|((?<=Z )[0-9]*\.[0-9]*|(?<=A )-[0-9]*\.[0-9]*)|((?<=A )[0-9]*\.[0-9]*|(?<=B )-[0-9]*\.[0-9]*)|((?<=B )[0-9]*\.[0-9]*|(?<=C )-[0-9]*\.[0-9]*)|((?<=C )[0-9]*\.[0-9]*|(?<=X )-[0-9]*\.[0-9]*)|((?<=X )[0-9]*\.[0-9]*|(?<=Y )-[0-9]*\.[0-9]*)|((?<=Y )[0-9]*\.[0-9]*|(?<=Z )-[0-9]*\.[0-9]*)|((?<=Z )[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                foundValues = new List<string>();
                var line = reader.ReadLine();
                if (line.Contains(searchPhrase) & !line.Contains("DECL"))
                {
                    foreach (Match currentMatch in regex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    LoadDataKuka currentData = new LoadDataKuka(float.Parse(foundValues[0].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[1].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[2].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[3].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[4].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[5].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[6].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[7].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[8].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[9].ToString(), CultureInfo.InvariantCulture));
                    result.Add(currentData);
                }
            }

            foreach (int loaddatasave in loadVarSaves)
            {
                reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains("load_data_save[" + loaddatasave))
                    {
                        foundValues = new List<string>();
                        foreach (Match currentMatch in regex.Matches(line))
                        {
                            foundValues.Add(currentMatch.ToString());
                        }
                        LoadDataKuka currentData = new LoadDataKuka(float.Parse(foundValues[0].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[1].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[2].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[3].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[4].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[5].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[6].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[7].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[8].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[9].ToString(), CultureInfo.InvariantCulture));
                        result[loaddatasave - 1] = currentData;
                    }
                }
            }
            reader.Close();

            return result;
        }

        private static IDictionary<int, string> GetLoadVarNames(string file)
        {
            IDictionary<int, string> result = new Dictionary<int, string>();
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("load_var_name") && !line.ToLower().Contains("decl"))
                {
                    Regex getnNumber = new Regex(@"(?<=NAME)\d+", RegexOptions.IgnoreCase);
                    int number = int.Parse(getnNumber.Match(line).ToString());
                    Regex getLoadVarName = new Regex("(?<=\").*(?=\")", RegexOptions.IgnoreCase);
                    string name = getLoadVarName.Match(line).ToString().TrimEnd();
                    if (number > 0)
                        result.Add(number, name);
                }
            }
            reader.Close();
            return result;
        }

        private static List<int> GetLoadVarSaves(string file)
        {
            List<int> result = new List<int>();
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("load_save") && line.ToLower().Contains("true"))
                {
                    Regex getNumber = new Regex(@"\d+", RegexOptions.IgnoreCase);
                    result.Add(int.Parse(getNumber.Match(line).ToString()));
                }
            }
            reader.Close();
            return result;
        }

        private static List<ToolType> GetToolTypes(string file)
        {
            List<ToolType> result = new List<ToolType>();
            Regex regex = new Regex("(?<==).*", RegexOptions.IgnoreCase);
            Match match;
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("TOOL_TYPE[") & !line.Contains("DECL"))
                {
                    match = regex.Match(line);
                    ToolType type = new ToolType(match.ToString());
                    result.Add(type);
                }
            }
            return result;
        }

        public static List<IBaseData> GetBaseDatas(string file = null, ZipArchiveEntry entry = null)
        {
            List<IBaseData> result = new List<IBaseData>();
            List<string> foundValues = new List<string>();
            Regex regex = new Regex(@"((?<=X )-[0-9]*\.[0-9]*)|((?<=X )-[0-9]*)|((?<=X )[0-9]*\.[0-9]*|((?<=X )[0-9]*)|(?<=Y )-[0-9]*\.[0-9]*)|((?<=Y )-[0-9]*)|((?<=Y )[0-9]*\.[0-9]*|((?<=Y )[0-9]*)|(?<=Z )-[0-9]*\.[0-9]*)|((?<=Z )-[0-9]*)|((?<=Z )[0-9]*\.[0-9]*|((?<=Z )[0-9]*)|(?<=A )-[0-9]*\.[0-9]*)|((?<=A )-[0-9]*)|((?<=A )[0-9]*\.[0-9]*|((?<=A )[0-9]*)|(?<=B )-[0-9]*\.[0-9]*)|((?<=B )-[0-9]*)|((?<=B )[0-9]*\.[0-9]*|((?<=B )[0-9]*)|(?<=C )-[0-9]*\.[0-9]*)|((?<=C )-[0-9]*)|((?<=C )[0-9]*\.[0-9]*|((?<=C )[0-9]*))", RegexOptions.IgnoreCase);
            StreamReader reader = null;
            if (file != null)
                reader = new StreamReader(file);
            else if (entry != null)
                reader = new StreamReader(entry.Open());
            while (!reader.EndOfStream)
            {
                foundValues = new List<string>();
                var line = reader.ReadLine();
                if (line.Contains("BASE_DATA[") && !line.Contains("DECL") && line.Replace(" ", "").Substring(0, 1) != ";")
                {
                    foreach (Match currentMatch in regex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    BaseDataKUKA currentData = new BaseDataKUKA(float.Parse(foundValues[0].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[1].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[2].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[3].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[4].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[5].ToString(), CultureInfo.InvariantCulture));
                    result.Add(currentData);
                }
            }
            return result;
        }

        public static List<BaseName> GetBaseNames(string file = null, ZipArchiveEntry entry = null)
        {
            List<BaseName> result = new List<BaseName>();
            Regex regex = new Regex("(?<=\").*(?=\")", RegexOptions.IgnoreCase);
            Match match;
            StreamReader reader = null;
            if (file!=null)
                reader = new StreamReader(file);
            else if (entry != null)
                reader = new StreamReader(entry.Open());
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("BASE_NAME[") & !line.Contains("DECL"))
                {
                    match = regex.Match(line);
                    BaseName name = new BaseName(match.ToString());
                    result.Add(name);
                }
            }
            return result;
        }

        public static List<BaseType> GetBaseTypes(string file = null, ZipArchiveEntry entry = null)
        {
            List<BaseType> result = new List<BaseType>();
            Regex regex = new Regex("(?<==).*", RegexOptions.IgnoreCase);
            Match match;
            StreamReader reader = null;
            if (file != null)
                reader = new StreamReader(file);
            else if (entry != null)
                reader = new StreamReader(entry.Open());
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("BASE_TYPE[") & !line.Contains("DECL"))
                {
                    match = regex.Match(line);
                    BaseType type = new BaseType(match.ToString());
                    result.Add(type);
                }
            }
            return result;
        }

        private static List<HomePos> GetHomePoses(string file)
        {
            List<HomePos> result = new List<HomePos>();
            List<string> foundValues = new List<string>();
            Regex regex = new Regex(@"((?<=A1 )-[0-9]*\.[0-9]*)|((?<=A1 )[0-9]*\.[0-9]*|(?<=A2 )-[0-9]*\.[0-9]*)|((?<=A2 )[0-9]*\.[0-9]*|(?<=A3 )-[0-9]*\.[0-9]*)|((?<=A3 )[0-9]*\.[0-9]*|(?<=A4 )-[0-9]*\.[0-9]*)|((?<=A4 )[0-9]*\.[0-9]*|(?<=A5 )-[0-9]*\.[0-9]*)|((?<=A5 )[0-9]*\.[0-9]*|(?<=A6 )-[0-9]*\.[0-9]*)|((?<=A6 )[0-9]*\.[0-9]*|(?<=E1 )-[0-9]*\.[0-9]*)|((?<=E1 )[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                foundValues = new List<string>();
                var line = reader.ReadLine();
                if (line.Contains("XHOME") & !line.Contains("XHOME="))
                {
                    foreach (Match currentMatch in regex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    float extAxis = 0;
                    if (foundValues.Count > 6)
                        extAxis = float.Parse(foundValues[6].ToString(), CultureInfo.InvariantCulture);
                    HomePos currentData = new HomePos(float.Parse(foundValues[0].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[1].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[2].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[3].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[4].ToString(), CultureInfo.InvariantCulture), float.Parse(foundValues[5].ToString(), CultureInfo.InvariantCulture), extAxis);
                    result.Add(currentData);
                }
            }
            return result;
        }

        private static List<HomeNameLong> GetHomeNameLongs(string file)
        {
            List<HomeNameLong> result = new List<HomeNameLong>();
            Regex regex = new Regex("(?<=MSG_TXT.. \").*(?=\")", RegexOptions.IgnoreCase);
            Match match;
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("HomeLgMSG[") & !line.Contains("DECL"))
                {
                    match = regex.Match(line);
                    HomeNameLong name = new HomeNameLong(match.ToString());
                    result.Add(name);
                }
            }
            return result;
        }

        private static List<HomeNameShort> GetHomeNameShorts(string file)
        {
            List<HomeNameShort> result = new List<HomeNameShort>();
            Regex regex = new Regex("(?<=SK_TXT.. \").*(?=\")", RegexOptions.IgnoreCase);
            Match match;
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("HOMESK[") & !line.Contains("DECL"))
                {
                    match = regex.Match(line);
                    HomeNameShort name = new HomeNameShort(match.ToString());
                    result.Add(name);
                }
            }
            return result;
        }

        private static List<CollZone> GetCollZones(List<FileInfo> Files, bool isCollFromComment)
        {
            List<CollZone> collisionZone = new List<CollZone>();
            List<CollZone> result = new List<CollZone>();
            string currentFile = "";
            foreach (FileInfo file in Files)
            {
                currentFile = file.FullName;
                collisionZone = GetCollZoneFromSrc(currentFile, isCollFromComment);
                foreach (CollZone zone in collisionZone)
                    result.Add(zone);
            }
            result = DeleteMultiDeclaration(result);
            result = SortCollZones(result);
            return result;
        }

        private static List<CollZone> GetCollZoneFromSrc(string currentFile, bool isCollFromComment)
        {
            var reader = new StreamReader(currentFile);
            List<CollZone> result = new List<CollZone>();
            Regex regexNr;
            CollZone zone = new CollZone();
            if (isCollFromComment)
                regexNr = new Regex("(?<= )[0-9]*(?= )", RegexOptions.IgnoreCase);
            else
                regexNr = new Regex("(?<=COLL_SAFETY_REQ . )[0-9]*(?= )", RegexOptions.IgnoreCase);
            Regex regexDescr = new Regex(@"(?<=\s*-\s*).*", RegexOptions.IgnoreCase);
            Match match;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                //if ((line.Contains("; Coll ") & isCollFromComment) | (line.Contains("COLL_SAFETY_REQ") & !isCollFromComment))
                if (((line.ToLower().Replace(" ", "").Contains(";kollisionsbereich") || line.ToLower().Replace(" ", "").Contains(";koll") || line.ToLower().Replace(" ", "").Contains(";collision") || line.ToLower().Replace(" ", "").Contains(";coll")) && isCollFromComment) || (line.Contains("COLL_SAFETY_REQ") & !isCollFromComment))
                {
                    match = regexNr.Match(line);
                    string number = match.ToString();
                    match = regexDescr.Match(line);
                    string description = "";
                    if (isCollFromComment)
                        description = match.ToString();
                    zone = new CollZone(number, description);
                    if (!string.IsNullOrEmpty(zone.Number))
                        result.Add(zone);
                }
            }
            result = SortCollZones(result);
            return result;
        }

        private static List<CollZone> GetCollZonesWithoutDescr(List<CollZone> collZones, List<CollZone> collZonesFromCommnad)
        {
            List<CollZone> result = new List<CollZone>();
            bool addItem = true;
            foreach (CollZone zoneFromCommand in collZonesFromCommnad)
            {
                addItem = true;
                foreach (CollZone zoneFromDescr in collZones)
                {
                    if (zoneFromCommand.Number == zoneFromDescr.Number)
                    {
                        addItem = false;
                        break;
                    }
                }
                if (addItem)
                    result.Add(zoneFromCommand);
            }
            return result;
        }


        private static List<CollZone> DeleteMultiDeclaration(List<CollZone> collZoneList)
        {
            bool addZone = true;
            List<CollZone> result = new List<CollZone>();
            foreach (CollZone zone in collZoneList)
            {
                foreach (CollZone resultZone in result.Where(item => item.Number == zone.Number))
                {
                    addZone = false;
                }
                if (addZone)
                    result.Add(zone);
                addZone = true;
            }
            return result;
        }

        private static List<IUserBit> GetUserBit(List<FileInfo> files, bool isOutput)
        {
            List<IUserBit> outputList = new List<IUserBit>();
            List<IUserBit> currentBits = new List<IUserBit>();
            foreach (FileInfo file in files)
            {
                //currentFile = file.FullName;
                currentBits = GetUserBitFromSrc(file, isOutput);
                foreach (var bit in currentBits)
                    outputList.Add(bit);
            }
            List<UserBitKUKA> userbits = new List<UserBitKUKA>();
            foreach (UserBitKUKA userbit in outputList.Where(item => (item as UserBitKUKA).Number != null & (item as UserBitKUKA).Number != ""))
            {
                UserBitKUKA copyOfBit = new UserBitKUKA(userbit);
                copyOfBit.NumberInPLC = CalculatedBitInPLC(userbit, robotStartAddress);
                userbits.Add(copyOfBit);
            }
            List<IUserBit> result = SortList(userbits);
            return result;
        }

        private static string CalculatedBitInPLC(UserBitKUKA userbit, int robotStartAddress)
        {
            double bitInByte = (double.Parse(userbit.Number) / 8) - 0.125;
            double fullByte = Math.Floor(bitInByte);
            double decimalVal = bitInByte - fullByte;
            double bitVal = decimalVal * 8;

            return "'" + (robotStartAddress + fullByte).ToString() + "." + bitVal.ToString();
        }

        private static List<IUserBit> GetUserBitFromSrc(FileInfo currentFile, bool isOutput)
        {
            List<IUserBit> result = new List<IUserBit>();
            UserBitKUKA currentUserBit = new UserBitKUKA();
            Regex regex, regexNr;
            if (isOutput)
                regexNr = new Regex(@"(?<= OUT )[0-9]*", RegexOptions.IgnoreCase);
            else
                regexNr = new Regex(@"(?<=( IN )|(\(IN ))[0-9]*", RegexOptions.IgnoreCase);

            var reader = new StreamReader(currentFile.FullName);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if ((line.ToLower().Contains(" out ") & isOutput) || (((line.ToLower().Contains(" in ") || line.ToLower().Contains("(in ")) & !isOutput)) && line.ToLower().Contains("wait for"))
                {
                    currentUserBit = new UserBitKUKA();
                    currentUserBit.Number = regexNr.Match(line).ToString();
                    regex = new Regex("(?<=').*(?=')", RegexOptions.IgnoreCase);
                    currentUserBit.Description = regex.Match(line).ToString();
                    currentUserBit.Path = currentFile.Name;
                    result.Add(currentUserBit);
                }
            }
            return result;
        }

        private static List<IUserBit> SortList(List<UserBitKUKA> userbits)
        {
            List<IUserBit> sortedList = new List<IUserBit>();
            int[] intArray = new int[userbits.Count];
            int loopCounter = 0;
            foreach (UserBitKUKA bit in userbits)
            {
                intArray[loopCounter] = int.Parse(bit.Number);
                loopCounter++;
            }
            int[] sortedCopy = (from element in intArray orderby element ascending select element).ToArray();

            for (int i = 1; i <= 32; i++)
            {
                foreach (UserBitKUKA zone in userbits.Where(item => int.Parse((item as UserBitKUKA).Number) == i))
                    sortedList.Add(zone);
                if (sortedList.Count == userbits.Count)
                    break;
            }

            return sortedList;
        }

        private static void WriteXLS(string robotType)
        {
            try
            {
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oWBs = oXL.Workbooks;
                oWB = oWBs.Add("");
                sheets = oWB.Sheets;
                oSheet = sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;

                if (data == null)
                    return;
                List<int> existingTools = FindExistingTools(data.ToolNames);
                List<int> existingLoadVars = FindExistingLoadVars(data.LoadVars);
                List<int> existingBases = FindExistingBases(data.BaseNames);
                Range aRange;
                Borders border;
                int counter = 0;

                //Sheet tools
                oSheet = oWB.ActiveSheet;
                oSheet.Name = "Tool_Daten";
                aRange = null;
                if (robotType == "KUKA")
                {
                    oSheet.Cells[1, 1] = "Programmname";
                    oSheet.Cells[1, 2] = "Beschreibung";
                    oSheet.Range["B1", "C1"].Merge();
                    oSheet.Range["D1", "U1"].Merge();
                    int programCounter = 2;
                    foreach (var item in data.Programs)
                    {
                        oSheet.Cells[programCounter, 2] = item.Name;
                        oSheet.Cells[programCounter, 4] = item.Description;
                        oSheet.Range["B" + programCounter.ToString(), "C" + programCounter.ToString()].Merge();
                        oSheet.Range["D" + programCounter.ToString(), "U" + programCounter.ToString()].Merge();
                        programCounter++;
                    }
                    oSheet.Cells[programCounter + 2, 1] = "Tool Daten";
                    oSheet.Cells[programCounter + 3, 1] = "";
                    oSheet.Cells[programCounter + 3, 2] = "Bezeichnung";
                    oSheet.Cells[programCounter + 3, 3] = "FM -Nummer";
                    oSheet.Cells[programCounter + 3, 4] = "X";
                    oSheet.Cells[programCounter + 3, 5] = "Y";
                    oSheet.Cells[programCounter + 3, 6] = "Z";
                    oSheet.Cells[programCounter + 3, 7] = "A";
                    oSheet.Cells[programCounter + 3, 8] = "B";
                    oSheet.Cells[programCounter + 3, 9] = "C";

                    aRange = oSheet.Range["A" + (programCounter + 2).ToString(), "O" + (programCounter + 3).ToString()];
                    aRange.Font.Name = "Arial";
                    aRange.Font.Size = 10;
                    aRange.Font.Bold = true;
                    aRange = oSheet.Range["D" + programCounter + 2, "U1"];
                    aRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                    if (existingTools.Count > 0)
                    {
                        for (int i = 0; i < existingTools.Count; i++)
                        {
                            int currentTool = existingTools[i];
                            oSheet.Cells[4 + i + programCounter, 1] = existingTools[i];
                            oSheet.Cells[4 + i + programCounter, 2] = data.ToolNames[currentTool - 1].Name;
                            oSheet.Cells[4 + i + programCounter, 4] = data.ToolDatas[currentTool - 1].Xpos;
                            oSheet.Cells[4 + i + programCounter, 5] = data.ToolDatas[currentTool - 1].Ypos;
                            oSheet.Cells[4 + i + programCounter, 6] = data.ToolDatas[currentTool - 1].Zpos;
                            oSheet.Cells[4 + i + programCounter, 7] = (data.ToolDatas[currentTool - 1] as ToolDataKuka).A;
                            oSheet.Cells[4 + i + programCounter, 8] = (data.ToolDatas[currentTool - 1] as ToolDataKuka).B;
                            oSheet.Cells[4 + i + programCounter, 9] = (data.ToolDatas[currentTool - 1] as ToolDataKuka).C;

                            counter = i + 7 + programCounter;
                        }

                        oSheet.Cells[counter - 1, 1] = "Last Daten";
                        oSheet.Cells[counter, 1] = "";
                        oSheet.Cells[counter, 2] = "Bezeichnung";
                        oSheet.Cells[counter, 3] = "Gewicht";
                        oSheet.Cells[counter, 4] = "X";
                        oSheet.Cells[counter, 5] = "Y";
                        oSheet.Cells[counter, 6] = "Z";
                        oSheet.Cells[counter, 7] = "A";
                        oSheet.Cells[counter, 8] = "B";
                        oSheet.Cells[counter, 9] = "C";
                        oSheet.Cells[counter, 10] = "Ixx";
                        oSheet.Cells[counter, 11] = "Iyy";
                        oSheet.Cells[counter, 12] = "Izz";

                        aRange = oSheet.Range["A" + (counter - 1).ToString(), "U" + (counter).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = true;

                        int currentcounter = counter;
                        for (int i = 0; i < existingTools.Count; i++)
                        {
                            int j = i + counter - 1;
                            int currentTool = existingTools[i];
                            oSheet.Cells[2 + j, 1] = existingTools[i];
                            oSheet.Cells[2 + j, 2] = data.ToolNames[currentTool - 1].Name;
                            oSheet.Cells[2 + j, 3] = data.LoadDatas[currentTool - 1].Mass;
                            oSheet.Cells[2 + j, 4] = data.LoadDatas[currentTool - 1].Xpos;
                            oSheet.Cells[2 + j, 5] = data.LoadDatas[currentTool - 1].Ypos;
                            oSheet.Cells[2 + j, 6] = data.LoadDatas[currentTool - 1].Zpos;
                            oSheet.Cells[2 + j, 7] = (data.LoadDatas[currentTool - 1] as LoadDataKuka).A;
                            oSheet.Cells[2 + j, 8] = (data.LoadDatas[currentTool - 1] as LoadDataKuka).B;
                            oSheet.Cells[2 + j, 9] = (data.LoadDatas[currentTool - 1] as LoadDataKuka).C;
                            oSheet.Cells[2 + j, 10] = data.LoadDatas[currentTool - 1].JXpos;
                            oSheet.Cells[2 + j, 11] = data.LoadDatas[currentTool - 1].JYpos;
                            oSheet.Cells[2 + j, 12] = data.LoadDatas[currentTool - 1].JZpos;
                            currentcounter = j;
                        }
                        oSheet.Cells[currentcounter + 4, 1] = "Load Var";
                        oSheet.Cells[currentcounter + 5, 1] = "";
                        oSheet.Cells[currentcounter + 5, 2] = "Bezeichnung";
                        oSheet.Cells[currentcounter + 5, 3] = "Gewicht";
                        oSheet.Cells[currentcounter + 5, 4] = "X";
                        oSheet.Cells[currentcounter + 5, 5] = "Y";
                        oSheet.Cells[currentcounter + 5, 6] = "Z";
                        oSheet.Cells[currentcounter + 5, 7] = "A";
                        oSheet.Cells[currentcounter + 5, 8] = "B";
                        oSheet.Cells[currentcounter + 5, 9] = "C";
                        oSheet.Cells[currentcounter + 5, 10] = "Ixx";
                        oSheet.Cells[currentcounter + 5, 11] = "Iyy";
                        oSheet.Cells[currentcounter + 5, 12] = "Izz";

                        aRange = oSheet.Range["A" + (currentcounter + 4).ToString(), "U" + (currentcounter + 5).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = true;

                        for (int i = 0; i < existingLoadVars.Count; i++)
                        {
                            int j = i + currentcounter + 6;
                            int currentLoadVar = existingLoadVars[i];
                            oSheet.Cells[j, 1] = i + 1;
                            oSheet.Cells[j, 2] = data.LoadVarNames[currentLoadVar];
                            oSheet.Cells[j, 3] = data.LoadVars[currentLoadVar - 1].Mass;
                            oSheet.Cells[j, 4] = data.LoadVars[currentLoadVar - 1].Xpos;
                            oSheet.Cells[j, 5] = data.LoadVars[currentLoadVar - 1].Ypos;
                            oSheet.Cells[j, 6] = data.LoadVars[currentLoadVar - 1].Zpos;
                            oSheet.Cells[j, 7] = (data.LoadVars[currentLoadVar - 1] as LoadDataKuka).A;
                            oSheet.Cells[j, 8] = (data.LoadVars[currentLoadVar - 1] as LoadDataKuka).B;
                            oSheet.Cells[j, 9] = (data.LoadVars[currentLoadVar - 1] as LoadDataKuka).C;
                            oSheet.Cells[j, 10] = data.LoadVars[currentLoadVar - 1].JXpos;
                            oSheet.Cells[j, 11] = data.LoadVars[currentLoadVar - 1].JYpos;
                            oSheet.Cells[j, 12] = data.LoadVars[currentLoadVar - 1].JZpos;
                        }

                        aRange = oSheet.Range["C" + (counter).ToString(), "U" + (counter + existingTools.Count).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = true;

                        aRange = oSheet.Range["C" + (currentcounter + 6).ToString(), "U" + (currentcounter + 6 + existingLoadVars.Count).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = true;

                        aRange = oSheet.Range["D" + (counter).ToString(), "U" + (counter).ToString()];
                        aRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
                        aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;

                    }
                }
                else if (robotType == "ABB")
                {
                    counter = 2;
                    oSheet.Cells[1, 2] = "Bezeichnung/Description";
                    oSheet.Cells[1, 3] = "robhold";
                    oSheet.Cells[1, 4] = "X";
                    oSheet.Cells[1, 5] = "Y";
                    oSheet.Cells[1, 6] = "Z";
                    oSheet.Cells[1, 7] = "q1";
                    oSheet.Cells[1, 8] = "q2";
                    oSheet.Cells[1, 9] = "q3";
                    oSheet.Cells[1, 10] = "q4";
                    oSheet.Cells[1, 11] = "Mass";
                    oSheet.Cells[1, 12] = "X";
                    oSheet.Cells[1, 13] = "Y";
                    oSheet.Cells[1, 14] = "Z";
                    oSheet.Cells[1, 15] = "q1";
                    oSheet.Cells[1, 16] = "q2";
                    oSheet.Cells[1, 17] = "q3";
                    oSheet.Cells[1, 18] = "q4";
                    oSheet.Cells[1, 19] = "ix";
                    oSheet.Cells[1, 20] = "iy";
                    oSheet.Cells[1, 21] = "iz";

                    foreach (var tool in data.ToolDatas)
                    {
                        ToolDataABB toolData = tool as ToolDataABB;
                        LoadDataABB currentToolLoadData = (data.LoadDatas.FirstOrDefault(x => (x as LoadDataABB).Name == toolData.Name) as LoadDataABB);
                        oSheet.Cells[counter, 1] = counter - 1;
                        oSheet.Cells[counter, 2] = toolData.Name;
                        oSheet.Cells[counter, 3] = toolData.Robhold;
                        oSheet.Cells[counter, 4] = toolData.Xpos;
                        oSheet.Cells[counter, 5] = toolData.Ypos;
                        oSheet.Cells[counter, 6] = toolData.Zpos;
                        oSheet.Cells[counter, 7] = toolData.Q1;
                        oSheet.Cells[counter, 8] = toolData.Q2;
                        oSheet.Cells[counter, 9] = toolData.Q3;
                        oSheet.Cells[counter, 10] = toolData.Q4;
                        if (currentToolLoadData != null)
                        {
                            oSheet.Cells[counter, 11] = currentToolLoadData.Mass;
                            oSheet.Cells[counter, 12] = currentToolLoadData.Xpos;
                            oSheet.Cells[counter, 13] = currentToolLoadData.Ypos;
                            oSheet.Cells[counter, 14] = currentToolLoadData.Zpos;
                            oSheet.Cells[counter, 15] = currentToolLoadData.Q1;
                            oSheet.Cells[counter, 16] = currentToolLoadData.Q2;
                            oSheet.Cells[counter, 17] = currentToolLoadData.Q3;
                            oSheet.Cells[counter, 18] = currentToolLoadData.Q4;
                            oSheet.Cells[counter, 19] = currentToolLoadData.JXpos;
                            oSheet.Cells[counter, 20] = currentToolLoadData.JYpos;
                            oSheet.Cells[counter, 21] = currentToolLoadData.JZpos;
                        }
                        counter++;
                    }
                    counter += 2;

                    oSheet.Cells[counter, 2] = "Bezeichnung/Description";
                    oSheet.Cells[counter, 3] = "Masse";
                    oSheet.Cells[counter, 4] = "X";
                    oSheet.Cells[counter, 5] = "Y";
                    oSheet.Cells[counter, 6] = "Z";
                    oSheet.Cells[counter, 7] = "q1";
                    oSheet.Cells[counter, 8] = "q2";
                    oSheet.Cells[counter, 9] = "q3";
                    oSheet.Cells[counter, 10] = "q4";
                    oSheet.Cells[counter, 11] = "ix";
                    oSheet.Cells[counter, 12] = "iy";
                    oSheet.Cells[counter, 13] = "iz";
                    counter++;
                    int lp = 1;
                    foreach (var load in data.LoadDatas)
                    {
                        LoadDataABB loadData = load as LoadDataABB;
                        oSheet.Cells[counter, 1] = lp;
                        oSheet.Cells[counter, 2] = loadData.Name;
                        oSheet.Cells[counter, 3] = loadData.Mass;
                        oSheet.Cells[counter, 4] = loadData.Xpos;
                        oSheet.Cells[counter, 5] = loadData.Ypos;
                        oSheet.Cells[counter, 6] = loadData.Zpos;
                        oSheet.Cells[counter, 7] = loadData.Q1;
                        oSheet.Cells[counter, 8] = loadData.Q2;
                        oSheet.Cells[counter, 9] = loadData.Q3;
                        oSheet.Cells[counter, 10] = loadData.Q4;
                        oSheet.Cells[counter, 11] = loadData.JXpos;
                        oSheet.Cells[counter, 12] = loadData.JYpos;
                        oSheet.Cells[counter, 13] = loadData.JZpos;
                        lp++;
                        counter++;
                    }
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    border = aRange.Borders;
                    border.LineStyle = XlLineStyle.xlContinuous;
                }
                // SAFETY
                if (data.SafetyConfig != null)
                {
                    oWB.Worksheets.Add(oSheet);
                    oSheet = oWB.ActiveSheet;
                    oSheet.Name = "Sicherheitsparamter";

                    if (robotType == "KUKA")
                    {
                        oSheet.Cells[1, 2] = "Überwachunsraum 1";

                        oSheet.Cells[2, 2] = "Stützpunkt 1";
                        oSheet.Cells[2, 4] = "X-Koordinate";
                        oSheet.Cells[2, 5] = data.SafetyConfig.Cellspace.Points[1].Xpos.ToString();
                        oSheet.Cells[2, 6] = "mm";
                        oSheet.Cells[2, 9] = "Stützpunkt 2";
                        oSheet.Cells[2, 14] = "X-Koordinate";
                        oSheet.Cells[2, 16] = data.SafetyConfig.Cellspace.Points[2].Xpos.ToString();
                        oSheet.Cells[2, 17] = "mm";
                        oSheet.Range["I2", "L2"].Merge();
                        oSheet.Range["N2", "O2"].Merge();

                        oSheet.Cells[3, 4] = "Y-Koordinate";
                        oSheet.Cells[3, 5] = data.SafetyConfig.Cellspace.Points[1].Ypos.ToString();
                        oSheet.Cells[3, 6] = "mm";
                        oSheet.Cells[3, 14] = "Y-Koordinate";
                        oSheet.Cells[3, 16] = data.SafetyConfig.Cellspace.Points[2].Ypos.ToString();
                        oSheet.Cells[3, 17] = "mm";
                        oSheet.Range["I3", "L3"].Merge();
                        oSheet.Range["N3", "O3"].Merge();

                        oSheet.Cells[4, 2] = "Stützpunkt 3";
                        oSheet.Cells[4, 4] = "X-Koordinate";
                        oSheet.Cells[4, 5] = data.SafetyConfig.Cellspace.Points[3].Xpos.ToString();
                        oSheet.Cells[4, 6] = "mm";
                        oSheet.Cells[4, 9] = "Stützpunkt 4";
                        oSheet.Cells[4, 14] = "X-Koordinate";
                        if (data.SafetyConfig.Cellspace.Points.Count >= 4)
                            oSheet.Cells[4, 16] = data.SafetyConfig.Cellspace.Points[4].Xpos.ToString();
                        oSheet.Cells[4, 17] = "mm";
                        oSheet.Range["I4", "L4"].Merge();
                        oSheet.Range["N4", "O4"].Merge();

                        oSheet.Cells[5, 4] = "Y-Koordinate";
                        oSheet.Cells[5, 5] = data.SafetyConfig.Cellspace.Points[3].Ypos.ToString();
                        oSheet.Cells[5, 6] = "mm";
                        oSheet.Cells[5, 14] = "Y-Koordinate";
                        if (data.SafetyConfig.Cellspace.Points.Count >= 4)
                            oSheet.Cells[5, 16] = data.SafetyConfig.Cellspace.Points[4].Ypos.ToString();
                        oSheet.Cells[5, 17] = "mm";
                        oSheet.Range["I5", "L5"].Merge();
                        oSheet.Range["N5", "O5"].Merge();

                        oSheet.Cells[6, 2] = "Stützpunkt 5";
                        oSheet.Cells[6, 4] = "X-Koordinate";
                        if (data.SafetyConfig.Cellspace.Points.Count >= 5)
                            oSheet.Cells[6, 5] = data.SafetyConfig.Cellspace.Points[5].Xpos.ToString();
                        oSheet.Cells[6, 6] = "mm";
                        oSheet.Cells[6, 9] = "Stützpunkt 6";
                        oSheet.Cells[6, 14] = "X-Koordinate";
                        if (data.SafetyConfig.Cellspace.Points.Count >= 6)
                            oSheet.Cells[6, 16] = data.SafetyConfig.Cellspace.Points[6].Xpos.ToString();
                        oSheet.Cells[6, 17] = "mm";
                        oSheet.Range["I6", "L6"].Merge();
                        oSheet.Range["N6", "O6"].Merge();

                        oSheet.Cells[7, 4] = "Y-Koordinate";
                        if (data.SafetyConfig.Cellspace.Points.Count >= 5)
                            oSheet.Cells[7, 5] = data.SafetyConfig.Cellspace.Points[5].Ypos.ToString();
                        oSheet.Cells[7, 6] = "mm";
                        oSheet.Cells[7, 14] = "Y-Koordinate";
                        if (data.SafetyConfig.Cellspace.Points.Count >= 6)
                            oSheet.Cells[7, 16] = data.SafetyConfig.Cellspace.Points[6].Ypos.ToString();
                        oSheet.Cells[7, 17] = "mm";
                        oSheet.Range["I7", "L7"].Merge();
                        oSheet.Range["N7", "O7"].Merge();

                        int startRow = 9;
                        int rowsToAdd = 6;
                        int zonecounter = 0;
                        foreach (var safeZone in data.SafetyConfig.SafeSpaces)
                        {
                            oSheet.Range["B" + (startRow + rowsToAdd * zonecounter).ToString(), "C" + (startRow + rowsToAdd * zonecounter).ToString()].Merge();
                            oSheet.Range["C" + (startRow + rowsToAdd * zonecounter + 1).ToString(), "E" + (startRow + rowsToAdd * zonecounter + 1).ToString()].Merge();
                            oSheet.Range["H" + (startRow + rowsToAdd * zonecounter + 2).ToString(), "J" + (startRow + rowsToAdd * zonecounter + 2).ToString()].Merge();
                            oSheet.Range["N" + (startRow + rowsToAdd * zonecounter + 2).ToString(), "O" + (startRow + rowsToAdd * zonecounter + 2).ToString()].Merge();
                            oSheet.Range["H" + (startRow + rowsToAdd * zonecounter + 3).ToString(), "J" + (startRow + rowsToAdd * zonecounter + 3).ToString()].Merge();
                            oSheet.Range["N" + (startRow + rowsToAdd * zonecounter + 3).ToString(), "O" + (startRow + rowsToAdd * zonecounter + 3).ToString()].Merge();
                            oSheet.Range["H" + (startRow + rowsToAdd * zonecounter + 4).ToString(), "J" + (startRow + rowsToAdd * zonecounter + 4).ToString()].Merge();
                            oSheet.Range["N" + (startRow + rowsToAdd * zonecounter + 4).ToString(), "O" + (startRow + rowsToAdd * zonecounter + 4).ToString()].Merge();

                            oSheet.Cells[startRow + rowsToAdd * zonecounter, 2] = "Überwachunsraum " + (zonecounter + 2);

                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 1, 2] = "Art";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 1, 3] = "Arbeitsraum/Schutzraum";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 1, 6] = "Vmax";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 1, 8] = "mm/s";

                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 2] = "Ursprung";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 4] = "X- Koordinate";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 5] = "0";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 6] = "mm";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 8] = "Y- Koordinate";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 11] = "0";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 12] = "mm";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 14] = "Z- Koordinate";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 16] = "0";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 2, 17] = "mm";

                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 4] = "untere X- Ebene";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 5] = (safeZone as SafeSpace2points).Origin.Xpos.ToString();
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 6] = "mm";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 8] = "untere Y- Ebene";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 11] = (safeZone as SafeSpace2points).Origin.Ypos.ToString();
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 12] = "mm";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 14] = "Z- Koordinate";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 16] = (safeZone as SafeSpace2points).Origin.Zpos.ToString();
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 3, 17] = "mm";

                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 4] = "obere X- Ebene";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 5] = (safeZone as SafeSpace2points).Max.Xpos.ToString();
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 6] = "mm";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 8] = "obere Y-Ebene";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 11] = (safeZone as SafeSpace2points).Max.Ypos.ToString();
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 12] = "mm";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 14] = "Z- Koordinate";
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 16] = (safeZone as SafeSpace2points).Max.Zpos.ToString();
                            oSheet.Cells[startRow + rowsToAdd * zonecounter + 4, 17] = "mm";

                            zonecounter++;
                        }

                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;
                    }
                    else if (robotType == "ABB")
                    {
                        counter = 1;
                        foreach (var tool in data.SafetyConfig.SafeTools)
                        {
                            int lp = 1;
                            oSheet.Cells[counter, 1] = "TOOL";
                            counter++;
                            oSheet.Cells[counter, 1] = "Position";
                            oSheet.Cells[counter, 2] = "X (in mm)";
                            oSheet.Cells[counter, 3] = "Y (in mm)";
                            oSheet.Cells[counter, 4] = "Z (in mm)";
                            counter++;
                            foreach (var sphere in tool.Spheres)
                            {
                                oSheet.Cells[counter, 1] = lp;
                                oSheet.Cells[counter, 2] = sphere.Coordinates.Xpos;
                                oSheet.Cells[counter, 3] = sphere.Coordinates.Ypos;
                                oSheet.Cells[counter, 4] = sphere.Coordinates.Zpos;
                                lp++; counter++;
                            }
                            counter += 2;
                        }

                        foreach (var zone in data.SafetyConfig.SafeSpaces)
                        {
                            var zoneABB = zone as SafeSpaceMultiPoints;
                            oSheet.Cells[counter, 1] = "ZONE " + zoneABB.Number;
                            counter++;
                            oSheet.Cells[counter, 1] = "Height";
                            oSheet.Cells[counter, 2] = zoneABB.MaxHeight;
                            counter++;
                            oSheet.Cells[counter, 1] = "X";
                            oSheet.Cells[counter, 2] = "Y";
                            oSheet.Cells[counter, 3] = "Z";
                            counter++;
                            foreach (var point in zoneABB.Points)
                            {
                                oSheet.Cells[counter, 1] = point.Xpos;
                                oSheet.Cells[counter, 2] = point.Ypos;
                                oSheet.Cells[counter, 3] = point.Zpos;
                                counter++;
                            }
                        }
                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;
                    }
                }
                //! SAFETY

                //BASEDATEN IST

                oWB.Worksheets.Add(oSheet);
                oSheet = oWB.ActiveSheet;
                oSheet.Name = "Base_Daten Ist";

                if (robotType == "KUKA")
                {
                    if (existingBases.Count > 1)
                    {
                        oSheet.Cells[1, 1] = "";
                        oSheet.Cells[1, 2] = "Bezeichnung";
                        oSheet.Cells[1, 3] = "FM -Nummer";
                        oSheet.Cells[1, 4] = "X";
                        oSheet.Cells[1, 5] = "Y";
                        oSheet.Cells[1, 6] = "Z";
                        oSheet.Cells[1, 7] = "A";
                        oSheet.Cells[1, 8] = "B";
                        oSheet.Cells[1, 9] = "C";

                        aRange = oSheet.Range["A1", "U1"];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = true;
                        aRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
                        aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                        for (int i = 0; i < existingBases.Count; i++)
                        {
                            int currentBase = existingBases[i];
                            oSheet.Cells[2 + i, 1] = existingBases[i];
                            oSheet.Cells[2 + i, 2] = data.BaseNames[currentBase - 1].Name;
                            oSheet.Cells[2 + i, 4] = data.BaseDatas[currentBase - 1].Xpos;
                            oSheet.Cells[2 + i, 5] = data.BaseDatas[currentBase - 1].Ypos;
                            oSheet.Cells[2 + i, 6] = data.BaseDatas[currentBase - 1].Zpos;
                            oSheet.Cells[2 + i, 7] = (data.BaseDatas[currentBase - 1] as BaseDataKUKA).A;
                            oSheet.Cells[2 + i, 8] = (data.BaseDatas[currentBase - 1] as BaseDataKUKA).B;
                            oSheet.Cells[2 + i, 9] = (data.BaseDatas[currentBase - 1] as BaseDataKUKA).C;
                        }

                        aRange = oSheet.Range["A2", "U" + ((existingBases.Count) + 1).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = false;

                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;
                    }
                    // sheet basedata Soll
                    oWB.Worksheets.Add(oSheet);
                    oSheet = oWB.ActiveSheet;
                    oSheet.Name = "Base_Daten Soll";

                    if (data.BaseNumbersInputData.Count > 0)
                    {
                        oSheet.Cells[1, 1] = "";
                        oSheet.Cells[1, 2] = "Bezeichnung";
                        oSheet.Cells[1, 3] = "FM -Nummer";
                        oSheet.Cells[1, 4] = "X";
                        oSheet.Cells[1, 5] = "Y";
                        oSheet.Cells[1, 6] = "Z";
                        oSheet.Cells[1, 7] = "A";
                        oSheet.Cells[1, 8] = "B";
                        oSheet.Cells[1, 9] = "C";

                        aRange = oSheet.Range["A1", "U1"];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = true;
                        aRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
                        aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;


                        for (int i = 0; i < data.BaseNumbersInputData.Count; i++)
                        {
                            oSheet.Cells[2 + i, 1] = data.BaseNumbersInputData[i];
                            oSheet.Cells[2 + i, 2] = data.BaseNamesInputData[i].Name;
                            oSheet.Cells[2 + i, 4] = data.BaseDatasInputData[i].Xpos;
                            oSheet.Cells[2 + i, 5] = data.BaseDatasInputData[i].Ypos;
                            oSheet.Cells[2 + i, 6] = data.BaseDatasInputData[i].Zpos;
                            oSheet.Cells[2 + i, 7] = (data.BaseDatasInputData[i] as BaseDataKUKA).A;
                            oSheet.Cells[2 + i, 8] = (data.BaseDatasInputData[i] as BaseDataKUKA).B;
                            oSheet.Cells[2 + i, 9] = (data.BaseDatasInputData[i] as BaseDataKUKA).C;
                        }

                        aRange = oSheet.Range["A2", "U" + ((data.BaseNumbersInputData.Count) + 1).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = false;


                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;
                    }
                }
                else if (robotType == "ABB")
                {
                    counter = 3;
                    oSheet.Cells[1, 2] = "wobjdata";
                    oSheet.Cells[1, 6] = "uframe";
                    oSheet.Cells[1, 13] = "oframe";
                    oSheet.Cells[2, 2] = "Bezeichnung/Description";
                    oSheet.Cells[2, 3] = "robhold";
                    oSheet.Cells[2, 4] = "ufprog";
                    oSheet.Cells[2, 5] = "ufmec";
                    oSheet.Cells[2, 6] = "X";
                    oSheet.Cells[2, 7] = "Y";
                    oSheet.Cells[2, 8] = "Z";
                    oSheet.Cells[2, 9] = "q1";
                    oSheet.Cells[2, 10] = "q2";
                    oSheet.Cells[2, 11] = "q3";
                    oSheet.Cells[2, 12] = "q4";
                    oSheet.Cells[2, 13] = "X";
                    oSheet.Cells[2, 14] = "Y";
                    oSheet.Cells[2, 15] = "Z";
                    oSheet.Cells[2, 16] = "q1";
                    oSheet.Cells[2, 17] = "q2";
                    oSheet.Cells[2, 18] = "q3";
                    oSheet.Cells[2, 19] = "q4";
                    int lp = 1;
                    foreach (var basedata in data.BaseDatas)
                    {
                        var baseDataABB = basedata as WobjDataABB;
                        oSheet.Cells[counter, 1] = lp;
                        oSheet.Cells[counter, 2] = baseDataABB.Name;
                        oSheet.Cells[counter, 3] = baseDataABB.Robhold;
                        oSheet.Cells[counter, 4] = "TRUE";
                        oSheet.Cells[counter, 5] = "";
                        oSheet.Cells[counter, 6] = baseDataABB.Xpos;
                        oSheet.Cells[counter, 7] = baseDataABB.Ypos;
                        oSheet.Cells[counter, 8] = baseDataABB.Zpos;
                        oSheet.Cells[counter, 9] = baseDataABB.Q1_UF;
                        oSheet.Cells[counter, 10] = baseDataABB.Q2_UF;
                        oSheet.Cells[counter, 11] = baseDataABB.Q3_UF;
                        oSheet.Cells[counter, 12] = baseDataABB.Q4_UF;
                        oSheet.Cells[counter, 13] = baseDataABB.Xpos_OF;
                        oSheet.Cells[counter, 14] = baseDataABB.Ypos_OF;
                        oSheet.Cells[counter, 15] = baseDataABB.Zpos_OF;
                        oSheet.Cells[counter, 16] = baseDataABB.Q1_OF;
                        oSheet.Cells[counter, 17] = baseDataABB.Q2_OF;
                        oSheet.Cells[counter, 18] = baseDataABB.Q3_OF;
                        oSheet.Cells[counter, 19] = baseDataABB.Q4_OF;

                        counter++; lp++;
                    }
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    border = aRange.Borders;
                    border.LineStyle = XlLineStyle.xlContinuous;
                }
                // sheet Homes

                oWB.Worksheets.Add(oSheet);
                oSheet = oWB.ActiveSheet;
                oSheet.Name = "Home_Programmablauf";

                if (robotType == "KUKA")
                {
                    if (data.HomePoses != null && data.HomePoses.Count > 0)
                    {
                        for (int i = 1; i <= (data.HomePoses.Count) + 1; i++)
                        {
                            oSheet.Range["B" + i.ToString(), "C" + i.ToString()].Merge();
                            oSheet.Range["D" + i.ToString(), "R" + i.ToString()].Merge();
                            oSheet.Range["S" + i.ToString(), "U" + i.ToString()].Merge();
                        }
                        oSheet.Cells[1, 1] = "Home";
                        oSheet.Cells[1, 2] = "SoftKeyText";
                        oSheet.Cells[1, 4] = "Beschreibung (Melde Text)";
                        oSheet.Cells[1, 19] = "SPS  OUT";
                        //  oSheet.Range["A1", "U1"].AutoFit();

                        for (int i = 0; i < data.HomePoses.Count; i++)
                        {
                            oSheet.Cells[2 + i, 1] = i + 1;
                            oSheet.Cells[2 + i, 2] = data.HomeNameShorts[i].Name;
                            oSheet.Cells[2 + i, 4] = data.HomeNameLongs[i].Name;
                            oSheet.Cells[2 + i, 19] = (183 + i).ToString();
                        }
                        aRange = oSheet.Range["A1", "U1"];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = true;
                        aRange.Columns.AutoFit();
                        aRange = oSheet.Range["A2", "U" + ((data.HomePoses.Count) + 1).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 10;
                        aRange.Font.Bold = false;

                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;
                    }
                }
                else if (robotType == "ABB")
                {
                    counter = 2;
                    oSheet.Cells[1, 1] = "Home";
                    oSheet.Cells[1, 2] = "Home Text";
                    oSheet.Cells[1, 3] = "Beschreibung (Melde Text) /Description (Stored Text)";
                    oSheet.Cells[1, 4] = "SPS  OUT";
                    for (int i = 1; i <= 5; i++)
                    {
                        oSheet.Cells[counter, 1] = i;
                        oSheet.Cells[counter, 2] = "Home" + i;
                        oSheet.Cells[counter, 3] = data.HomeNameLongs[i-1].Name;
                        oSheet.Cells[counter, 4] = "SYS_do_Home_" + i;
                        counter++;
                    }
                    counter += 2;
                    oSheet.Cells[counter, 1] = "Home";
                    oSheet.Cells[counter, 2] = "ProgNr.";
                    oSheet.Cells[counter, 3] = "ProgName";
                    oSheet.Cells[counter, 4] = "Beschreibung/Description";
                    counter++;
                    foreach (var program in data.Programs)
                    {
                        Regex getProgNum = new Regex(@"\d+", RegexOptions.IgnoreCase);
                        oSheet.Cells[counter, 1] = program.StartHome;
                        oSheet.Cells[counter, 2] = getProgNum.Match(program.Name).ToString();
                        oSheet.Cells[counter, 3] = program.Name;
                        oSheet.Cells[counter, 4] = program.Description;
                        counter++;
                    }
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    border = aRange.Borders;
                    border.LineStyle = XlLineStyle.xlContinuous;
                }
                
                // sheet Collisions
                oWB.Worksheets.Add(oSheet);
                oSheet = oWB.ActiveSheet;
                oSheet.Name = "Kollisionsschutz";

                if (data.CollZones.Count > 0)
                {
                    if (robotType == "KUKA")
                    {
                        for (int i = 1; i <= (data.CollZones.Count) + 1; i++)
                        {
                            oSheet.Range["C" + i.ToString(), "Q" + i.ToString()].Merge();
                            oSheet.Range["R" + i.ToString(), "U" + i.ToString()].Merge();
                        }
                        oSheet.Cells[1, 2] = "Bereich\n Nr.";
                        oSheet.Cells[1, 3] = "Bereichsbeschreibung";
                        oSheet.Cells[1, 18] = "Kollision zu\n Roboter";

                        for (int i = 0; i < data.CollZones.Count; i++)
                        {
                            oSheet.Cells[2 + i, 2] = data.CollZones[i].Number;
                            oSheet.Cells[2 + i, 3] = data.CollZones[i].Description;
                            //oSheet.Cells[2 + i, 18] = data.HomeNameLongs[i].Name;
                        }

                        if (data.CollZonesWithoutDescr != null)
                        {
                            oSheet.Cells[data.CollZones.Count + 4, 2] = "Collision zones without description";
                            aRange = oSheet.Range["B" + (data.CollZones.Count + 4).ToString(), "B" + (data.CollZones.Count + 4).ToString()];
                            aRange.EntireRow.Interior.Color = ColorTranslator.ToOle(Color.Red);
                            for (int i = 0; i < data.CollZonesWithoutDescr.Count; i++)
                            {
                                oSheet.Cells[data.CollZones.Count + 5 + i, 2] = data.CollZonesWithoutDescr[i].Number;
                                //oSheet.Cells[2 + i, 18] = data.HomeNameLongs[i].Name;
                            }
                        }

                        aRange = oSheet.Range["A1", "U1"];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 12;
                        aRange.Font.Bold = true;
                        aRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
                        aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                        aRange = oSheet.Range["A2", "U" + (data.CollZones.Count + 1).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 12;
                        aRange.Font.Bold = false;
                        aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;


                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;
                    }
                    else if (robotType == "ABB")
                    {
                        counter = 2;
                        oSheet.Cells[1, 1] = "Bereich/Area Nr.";
                        oSheet.Cells[1, 2] = "Bereichsbeschreibung/Area Description";
                        oSheet.Cells[1, 3] = "Anticollision with Robot /Kollision zu Roboter";

                        foreach (var collision in data.CollZones)
                        {
                            oSheet.Cells[counter, 1] = collision.Number;
                            oSheet.Cells[counter, 2] = collision.Description;
                            counter++;
                        }
                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;
                    }
                }
                // sheet Userbits
                oWB.Worksheets.Add(oSheet);
                oSheet = oWB.ActiveSheet;
                oSheet.Name = "E-A Zuordnung";
                int counterForHandling = 0;
                if (robotType == "KUKA")
                {
                    for (int i = 1; i <= (data.UserbitsIn.Count) + (data.UserbitsOut.Count) + 4; i++)
                    {
                        oSheet.Range["B" + i.ToString(), "C" + i.ToString()].Merge();
                        oSheet.Range["D" + i.ToString(), "R" + i.ToString()].Merge();
                        oSheet.Range["S" + i.ToString(), "U" + i.ToString()].Merge();
                    }
                    oSheet.Cells[1, 1] = "IN";
                    oSheet.Cells[1, 2] = "Name Langtext";
                    oSheet.Cells[1, 4] = "Beschreibung";
                    oSheet.Cells[1, 19] = "SPS  OUT";
                    oSheet.Cells[1, 22] = "Path";

                    int userInCounter = 0;
                    for (int i = 0; i < data.UserbitsIn.Count; i++)
                    {
                        oSheet.Cells[2 + i, 1] = (data.UserbitsIn[i] as UserBitKUKA).Number;
                        (oSheet.Cells[2 + i, 1]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        oSheet.Cells[2 + i, 2] = data.UserbitsIn[i].Description;
                        oSheet.Cells[2 + i, 4] = data.UserbitsIn[i].Description;
                        oSheet.Cells[2 + i, 19] = (data.UserbitsIn[i] as UserBitKUKA).NumberInPLC;
                        oSheet.Cells[2 + i, 22] = data.UserbitsIn[i].Path;
                        userInCounter = i + 2;
                    }

                    if (userInCounter == 0)
                        userInCounter = 2;
                    aRange = oSheet.Range["A2", "V" + userInCounter.ToString()];
                    aRange.Font.Name = "Arial";
                    aRange.Font.Size = 12;
                    aRange.Font.Bold = false;

                    oSheet.Cells[(data.UserbitsIn.Count) + 4, 1] = "OUT";
                    oSheet.Cells[(data.UserbitsIn.Count) + 4, 2] = "Name Langtext";
                    oSheet.Cells[(data.UserbitsIn.Count) + 4, 4] = "Beschreibung";
                    oSheet.Cells[(data.UserbitsIn.Count) + 4, 19] = "SPS  IN";
                    oSheet.Cells[(data.UserbitsIn.Count) + 4, 22] = "Path";

                    counterForHandling = 0;
                    for (int i = 0; i < data.UserbitsOut.Count; i++)
                    {
                        oSheet.Cells[((data.UserbitsIn.Count) + 5 + i), 1] = (data.UserbitsOut[i] as UserBitKUKA).Number;
                        (oSheet.Cells[(data.UserbitsIn.Count + 5 + i), 1]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        oSheet.Cells[((data.UserbitsIn.Count) + 5 + i), 2] = data.UserbitsOut[i].Description;
                        oSheet.Cells[((data.UserbitsIn.Count) + 5 + i), 4] = data.UserbitsOut[i].Description;
                        oSheet.Cells[((data.UserbitsIn.Count) + 5 + i), 19] = (data.UserbitsOut[i] as UserBitKUKA).NumberInPLC;
                        oSheet.Cells[((data.UserbitsIn.Count) + 5 + i), 22] = data.UserbitsOut[i].Path;
                        counterForHandling = (data.UserbitsIn.Count) + 7 + i;
                    }
                    if (counterForHandling > 0)
                    {
                        aRange = oSheet.Range["A" + ((data.UserbitsIn.Count) + 5).ToString(), "V" + (counterForHandling - 1).ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 12;
                        aRange.Font.Bold = false;
                    }
                }
                else if (robotType == "ABB")
                {
                    counter = 2;
                    oSheet.Cells[1, 1] = "IN";
                    oSheet.Cells[1, 2] = "Name, Langtext";
                    oSheet.Cells[1, 3] = "SPS  OUT";

                    foreach (var item in data.UserbitsIn)
                    {
                        UserBitABB user = item as UserBitABB;
                        oSheet.Cells[counter, 1] = user.Name;
                        oSheet.Cells[counter, 2] = user.Description;
                        counter++;
                    }
                    foreach (var item in data.TypBits)
                    {
                        UserBitABB typbit = item as UserBitABB;
                        oSheet.Cells[counter, 1] = typbit.Name;
                        oSheet.Cells[counter, 2] = typbit.Description;
                        counter++;
                    }

                    foreach (var item in data.JobEnables)
                    {
                        UserBitABB job = item as UserBitABB;
                        oSheet.Cells[counter, 1] = job.Name;
                        oSheet.Cells[counter, 2] = job.Description;
                        counter++;
                    }

                    counter += 2;
                    oSheet.Cells[counter, 1] = "OUT";
                    oSheet.Cells[counter, 2] = "Name, Langtext";
                    oSheet.Cells[counter, 3] = "SPS  OUT";
                    counter++;
                    foreach (var item in data.UserbitsOut)
                    {
                        UserBitABB user = item as UserBitABB;
                        oSheet.Cells[counter, 1] = user.Name;
                        oSheet.Cells[counter, 2] = user.Description;
                        counter++;
                    }
                    foreach (var item in data.TypBits)
                    {
                        UserBitABB typbit = item as UserBitABB;
                        oSheet.Cells[counter, 1] = typbit.Name.Replace("_di_", "_do_");
                        oSheet.Cells[counter, 2] = typbit.Description;
                        counter++;
                    }
                    foreach (var item in data.JobEnables)
                    {
                        UserBitABB job = item as UserBitABB;
                        oSheet.Cells[counter, 1] = job.Name.Replace("_di_", "_do_");
                        oSheet.Cells[counter, 2] = job.Description;
                        counter++;
                    }
                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    border = aRange.Borders;
                    border.LineStyle = XlLineStyle.xlContinuous;
                }
                // HANDLING

                if (robotType == "KUKA")
                {
                    if (data.GripperConfig != null && ((data.GripperConfig as FileValidationData.Userbits).Inputs.Count > 0 || (data.GripperConfig as FileValidationData.Userbits).Outputs.Count > 0))
                    {
                        oSheet.Cells[counterForHandling, 5] = "Roboter-Handling  1";
                        aRange = oSheet.Range["E" + counterForHandling.ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 18;

                        oSheet.Cells[counterForHandling + 2, 1] = "IN";
                        oSheet.Cells[counterForHandling + 2, 2] = "Name Langtext";
                        oSheet.Cells[counterForHandling + 2, 4] = "Beschreibung";
                        oSheet.Cells[counterForHandling + 2, 19] = "Anschluß";
                        oSheet.Range["B" + (counterForHandling + 2).ToString(), "C" + (counterForHandling + 2).ToString()].Merge();
                        oSheet.Range["D" + (counterForHandling + 2).ToString(), "R" + (counterForHandling + 2).ToString()].Merge();
                        oSheet.Range["S" + (counterForHandling + 2).ToString(), "U" + (counterForHandling + 2).ToString()].Merge();

                        int i = 1;
                        int currentRow = 0;
                        foreach (var signalIn in (data.GripperConfig as FileValidationData.Userbits).Inputs)
                        {
                            currentRow = counterForHandling + 2 + i;
                            oSheet.Cells[currentRow, 1] = "IN " + signalIn.Key;
                            oSheet.Cells[currentRow, 2] = signalIn.Value[0];
                            oSheet.Cells[currentRow, 4] = signalIn.Value[0];
                            oSheet.Cells[currentRow, 19] = CalculateAnschluss(signalIn.Key, "I");

                            oSheet.Range["B" + (currentRow).ToString(), "C" + (currentRow).ToString()].Merge();
                            oSheet.Range["D" + (currentRow).ToString(), "R" + (currentRow).ToString()].Merge();
                            oSheet.Range["S" + (currentRow).ToString(), "U" + (currentRow).ToString()].Merge();
                            i++;
                        }
                        foreach (var signalOut in (data.GripperConfig as FileValidationData.Userbits).Outputs)
                        {
                            currentRow = counterForHandling + 2 + i;
                            oSheet.Cells[currentRow, 1] = "OUT " + signalOut.Key;
                            oSheet.Cells[currentRow, 2] = signalOut.Value[0];
                            oSheet.Cells[currentRow, 4] = signalOut.Value[0];
                            oSheet.Cells[currentRow, 19] = CalculateAnschluss(signalOut.Key, "O");
                            oSheet.Range["B" + (currentRow).ToString(), "C" + (currentRow).ToString()].Merge();
                            oSheet.Range["D" + (currentRow).ToString(), "R" + (currentRow).ToString()].Merge();
                            oSheet.Range["S" + (currentRow).ToString(), "U" + (currentRow).ToString()].Merge();
                            i++;
                        }

                        aRange = oSheet.Range["A" + (counterForHandling + 2).ToString(), "U" + currentRow.ToString()];
                        aRange.Font.Name = "Arial";
                        aRange.Font.Size = 12;
                        aRange.Font.Bold = false;
                    }

                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                    border = aRange.Borders;
                    border.LineStyle = XlLineStyle.xlContinuous;
                    aRange.Columns.AutoFit();


                    aRange = oSheet.Range["A1", "U1"];
                    aRange.Font.Name = "Arial";
                    aRange.Font.Size = 12;
                    aRange.Font.Bold = true;
                    aRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                    aRange = oSheet.Range["A" + ((data.UserbitsIn.Count) + 4).ToString(), "U" + ((data.UserbitsIn.Count) + 4).ToString()];
                    aRange.Font.Name = "Arial";
                    aRange.Font.Size = 12;
                    aRange.Font.Bold = true;
                    aRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                    aRange = oSheet.UsedRange;
                    aRange.Columns.AutoFit();
                }
                else if (robotType == "ABB")
                {
                    if (data.GripperConfig != null && ((data.GripperConfig as GripperConfigABB).Inputs.Count > 0 || (data.GripperConfig as GripperConfigABB).Outputs.Count > 0))
                    {
                        oWB.Worksheets.Add(oSheet);
                        oSheet = oWB.ActiveSheet;
                        oSheet.Name = "IO-Handling(EA-Greifer)";
                        counter = 2;
                        oSheet.Cells[1, 1] = "IN";
                        oSheet.Cells[1, 2] = "Name,  Langtext";
                        oSheet.Cells[1, 3] = "GRP_X";
                        foreach (var input in (data.GripperConfig as GripperConfigABB).Inputs)
                        {
                            oSheet.Cells[counter, 1] = input.Key;
                            oSheet.Cells[counter, 2] = input.Value;
                            oSheet.Cells[counter, 3] = input.Key.Length>=4 ? input.Key.Substring(0,4) : string.Empty;
                            counter++;
                        }
                        counter += 2;
                        oSheet.Cells[counter, 1] = "OUT";
                        oSheet.Cells[counter, 2] = "Name,  Langtext";
                        oSheet.Cells[counter, 3] = "GRP_X";
                        counter++;
                        foreach (var output in (data.GripperConfig as GripperConfigABB).Outputs)
                        {
                            oSheet.Cells[counter, 1] = output.Key;
                            oSheet.Cells[counter, 2] = output.Value;
                            oSheet.Cells[counter, 3] = output.Key.Length >= 4 ? output.Key.Substring(0, 4) : string.Empty;
                            counter++;
                        }
                        aRange = oSheet.UsedRange;
                        aRange.Columns.AutoFit();
                        border = aRange.Borders;
                        border.LineStyle = XlLineStyle.xlContinuous;

                    }
                }
                //if (oWB.Saved)
                //MessageBox.Show("Data saved at D:\\Projekty\\G14\\ExtractedData\\DataForWorkbook.xls");
                oXL.Visible = true;
                Marshal.FinalReleaseComObject(aRange);
                Marshal.FinalReleaseComObject(oSheet);
                Marshal.FinalReleaseComObject(oWB);
                Marshal.FinalReleaseComObject(oWBs);
                Marshal.FinalReleaseComObject(oXL);
            }
            catch (Exception ex)
            {
                CleanUpExcel();
                MessageBox.Show("Coś poszło nie tak", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string CalculateAnschluss(int key, string sigType)
        {
            int currentnumber = key - 1041;
            int offset = 0;
            if (sigType == "I")
                offset = 15;
            else
                offset = 31;


            while (currentnumber > offset)
            {
                currentnumber = currentnumber - (offset + 1);
            }
            return sigType + currentnumber.ToString();
        }

        private static void CleanUpExcel()
        {
            if (oXL != null)
            {
                int hWndDest = oXL.Application.Hwnd;

                uint processID;

                GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                Process.GetProcessById((int)processID).Kill();
            }
            oWBs = null;
            oWB = null;
            oSheet = null;
        }

        private static GripperConfigABB GetGripperConfigAbb(string fileName)
        {
            Regex grpConfigRegex = new Regex(@"(?<=grp_config.*\[).*(?=\])", RegexOptions.IgnoreCase);
            Regex grpPartRegex = new Regex(@"(?<=grp_part.*\[).*(?=\])", RegexOptions.IgnoreCase);
            Regex grpConfigNameRegex = new Regex(@"(?<=grp_config\s+)[\w_-]+", RegexOptions.IgnoreCase);
            Regex grpPartNameRegex = new Regex(@"(?<=grp_part\s+)[\w_-]+", RegexOptions.IgnoreCase);
            GripperConfigABB result = new GripperConfigABB();
            var files = Directory.GetFiles(fileName, "*.*", SearchOption.AllDirectories);
            var grpDatFile = files.First(x => x.ToLower().Contains("grpdata.mod"));
            StreamReader reader = new StreamReader(grpDatFile);
            while (!reader.EndOfStream)
            {
                string line = RemoveCommentABB(reader.ReadLine());
                if (grpConfigRegex.IsMatch(line))
                {
                    var currentgrpName = grpConfigNameRegex.Match(line).ToString();
                    var currentgrpValues = grpConfigRegex.Match(line).ToString().Replace("\"","").Split(',');
                    result.AddItem("Output",currentgrpValues[6], currentgrpValues[4].Replace("R", ""));
                    result.AddItem("Output",currentgrpValues[5], currentgrpValues[4].Replace("A", ""));
                    if (!string.IsNullOrEmpty(currentgrpValues[13]))
                        result.AddItem("Input",currentgrpValues[13], currentgrpValues[11]);                        
                    if (!string.IsNullOrEmpty(currentgrpValues[14]))
                        result.AddItem("Input",currentgrpValues[14], currentgrpValues[12]);
                    if (!string.IsNullOrEmpty(currentgrpValues[18]))
                        result.AddItem("Input",currentgrpValues[18], currentgrpValues[16]);
                    if (!string.IsNullOrEmpty(currentgrpValues[19]))
                        result.AddItem("Input",currentgrpValues[19], currentgrpValues[17]);
                    if (!string.IsNullOrEmpty(currentgrpValues[23]))
                        result.AddItem("Input",currentgrpValues[23], currentgrpValues[21]);
                    if (!string.IsNullOrEmpty(currentgrpValues[24]))
                        result.AddItem("Input",currentgrpValues[24], currentgrpValues[22]);
                    if (!string.IsNullOrEmpty(currentgrpValues[28]))
                        result.AddItem("Input",currentgrpValues[18], currentgrpValues[26]);
                    if (!string.IsNullOrEmpty(currentgrpValues[29]))
                        result.AddItem("Input",currentgrpValues[29], currentgrpValues[27]);
                    if (!string.IsNullOrEmpty(currentgrpValues[56]))
                        result.AddItem("Input", currentgrpValues[56], currentgrpValues[55]);
                }
                if (grpPartRegex.IsMatch(line))
                {
                    var currentgrpName = grpPartNameRegex.Match(line).ToString();
                    var currentgrpValues = grpPartRegex.Match(line).ToString().Replace("\"", "").Split(',');
                    result.AddItem("Input",currentgrpValues[5], currentgrpValues[4]);
                }
            }
            return result;
        }

        private static string RemoveCommentABB(string v)
        {
            try
            {
                if (v.Trim().Length > 0 && v.Trim().Replace(" ", "").Substring(0, 1) != "!")
                    return v;
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in line:\r\n" + v, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }


        private static SafetyConfig GetSafetyABB(string directory)
        {
            SafetyConfig result = new SafetyConfig() { Cellspace = new Cellspace(), SafeSpaces = new List<ISafeSpace>(), SafeTools = new List<SafeTool>() };
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            var safetyXMLFile = files.First(x => x.ToLower().Contains("psc_user_1.sxml"));
            XElement safetyXML = XElement.Load(safetyXMLFile);
            List<XElement> safetools = new List<XElement>();
            List<XElement> safeZoneSupervision = new List<XElement>();
            List<XElement> safeZoneMonitoring = new List<XElement>();
            if (safetyXML.Element("SafetyControllerUserData").Elements("Flange_Super_Pose").Any())
                safetools = safetyXML.Element("SafetyControllerUserData").Elements("Flange_Super_Pose").ToList();
            if (safetyXML.Element("SafetyControllerUserData").Elements("Supervision").Any())
                if (safetyXML.Element("SafetyControllerUserData").Element("Supervision").Elements("ToolZone_Supervision").Any());
                    if (safetyXML.Element("SafetyControllerUserData").Element("Supervision").Element("ToolZone_Supervision").Elements("Zone").Any())
                        safeZoneSupervision = safetyXML.Element("SafetyControllerUserData").Element("Supervision").Element("ToolZone_Supervision").Elements("Zone").ToList();
            if (safetyXML.Element("SafetyControllerUserData").Elements("Monitoring").Any())
                if (safetyXML.Element("SafetyControllerUserData").Element("Monitoring").Elements("ToolZone_Monitor").Any())
                    if (safetyXML.Element("SafetyControllerUserData").Element("Monitoring").Element("ToolZone_Monitor").Elements("Zone").Any())
                        safeZoneMonitoring = safetyXML.Element("SafetyControllerUserData").Element("Monitoring").Element("ToolZone_Monitor").Elements("Zone").ToList();
            foreach (var tool in safetools)
            {
                int number = int.Parse(tool.Attribute("ToolID").Value);
                List<Sphere> spheres = new List<Sphere>();
                foreach (var sphere in tool.Elements("Pos"))
                {
                    Sphere point = new Sphere();
                    point.Number = int.Parse(sphere.Attribute("PosID").Value);
                    point.Coordinates = new PointInSafety(double.Parse(sphere.Attribute("Pos_X").Value,CultureInfo.InvariantCulture), double.Parse(sphere.Attribute("Pos_Y").Value, CultureInfo.InvariantCulture), double.Parse(sphere.Attribute("Pos_Z").Value, CultureInfo.InvariantCulture));
                    point.Radius = 0;
                    //if (point.Coordinates.Xpos != 0 && point.Coordinates.Ypos != 0 && point.Coordinates.Zpos != 0)
                    spheres.Add(point);
                }
                PointInSafety tcp = new PointInSafety(double.Parse(tool.Attribute("x_0").Value, CultureInfo.InvariantCulture), double.Parse(tool.Attribute("y_0").Value, CultureInfo.InvariantCulture), double.Parse(tool.Attribute("z_0").Value, CultureInfo.InvariantCulture));
                if (IsUsedTool(spheres))
                    result.SafeTools.Add(new SafeTool() { Number = number, Spheres = spheres, TCP = tcp});
            }
            foreach (var zone in safeZoneSupervision)
            {
                List<PointInSafety> points = new List<PointInSafety>();
                int number = int.Parse(zone.Attribute("ZoneID").Value);
                double minHeight = double.Parse(zone.Element("Dimension_Z").Attribute("Pos_Z_lower").Value,CultureInfo.InvariantCulture);
                double maxHeight = double.Parse(zone.Element("Dimension_Z").Attribute("Pos_Z_upper").Value, CultureInfo.InvariantCulture);
                foreach (var point in zone.Elements("Pos"))
                {
                    points.Add(new PointInSafety(double.Parse(point.Attribute("Pos_X").Value, CultureInfo.InvariantCulture), double.Parse(point.Attribute("Pos_Y").Value, CultureInfo.InvariantCulture), minHeight));
                }
                result.SafeSpaces.Add(new SafeSpaceMultiPoints(number, points, maxHeight));
            }

            return result;
        }

        private static bool IsUsedTool(List<Sphere> spheres)
        {
            foreach (var sphere in spheres)
            {
                if (sphere.Coordinates.Xpos != 0 || sphere.Coordinates.Ypos != 0 || sphere.Coordinates.Zpos != 0)
                    return true;
            }
            return false;
        }

        private static List<IToolData> GetToolDatasABB(string directory, out List<ILoadData> toolloadDatas, out List<ILoadData> loadDatas, out List<ToolName> toolnames, out List<IBaseData> baseDatas, out List<BaseName> basenames, out List<HomePos> homeposes, out List<HomeNameLong> homenamesLong, out List<HomeNameShort> homenamesShort, out List<IUserBit> userBitsIn, out List<IUserBit> userBitsOut, out List<IUserBit> typbits, out List<IUserBit> jobEnables)
        {
            try
            {
                Regex tooldataRegex = new Regex(@"(?<=(CONST|PERS)\s+tooldata\s+)[\w_-]+", RegexOptions.IgnoreCase);
                Regex loaddataRegex = new Regex(@"(?<=(CONST|PERS)\s+loaddata\s+)[\w_-]+", RegexOptions.IgnoreCase);
                Regex wobjdataRegex = new Regex(@"(?<=(CONST|PERS)\s+wobjdata\s+)[\w_-]+", RegexOptions.IgnoreCase);
                Regex getnumregex = new Regex(@"(?<=\=.*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
                Regex getHomeRegex = new Regex(@"(?<=(CONST|PERS)\s+jointtarget\s+)jpHome\d+", RegexOptions.IgnoreCase);
                Regex homeNameRegex = new Regex(@"(CONST|PERS)\s+string\s+st_Home_label", RegexOptions.IgnoreCase);
                Regex homenumRegex = new Regex(@"(?<=\[|,).*?(?=,|\])", RegexOptions.IgnoreCase);
                Regex userBitInRegex = new Regex(@"(?<=^PLC_Release.*Wait_\s*:\s*=\s*)[\w_-]+", RegexOptions.IgnoreCase);
                Regex userBitOutRegex = new Regex(@"(?<=^PLC_Release.*Set_\s*:\s*=\s*)[\w_-]+", RegexOptions.IgnoreCase);
                Regex userBitDescr = new Regex("(?<=^PLC_Release\\s*\")[\\w\\s_\\-\\/]*", RegexOptions.IgnoreCase);
                Regex progNameRegex = new Regex(@"(?<=PROC\s+).+(?=\(\))", RegexOptions.IgnoreCase);
                Regex endprogRegex = new Regex(@"^ENDPROC", RegexOptions.IgnoreCase);
                Regex jobEnableRegex = new Regex(@"PLC_di_Job\d+_Enable", RegexOptions.IgnoreCase);
                Regex typBitRegex = new Regex(@"PLC_di_Typbit_\d+", RegexOptions.IgnoreCase);
                Regex robholdRegex = new Regex(@"(False|True)+?", RegexOptions.IgnoreCase);
                List<IToolData> result = new List<IToolData>();
                loadDatas = new List<ILoadData>();
                toolloadDatas = new List<ILoadData>();
                toolnames = new List<ToolName>();
                basenames = new List<BaseName>();
                baseDatas = new List<IBaseData>();
                homeposes = new List<HomePos>();
                homenamesLong = new List<HomeNameLong>();
                homenamesShort = new List<HomeNameShort>();
                userBitsIn = new List<IUserBit>();
                userBitsOut = new List<IUserBit>();
                typbits = new List<IUserBit>();
                jobEnables = new List<IUserBit>();
                string currentPath = string.Empty;
                var files = Directory.GetFiles(directory, "*.mod*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    string procName = string.Empty, lastJobEnable = string.Empty;
                    StreamReader reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        string line = RemoveCommentABB(reader.ReadLine());
                        if (currentPath.ToLower().Contains("program_") && CheckIfIsProc(RemoveCommentABB(line), out procName) && !string.IsNullOrEmpty(lastJobEnable))
                            jobEnables.Add(new UserBitABB(lastJobEnable, currentPath, procName));
                        if (jobEnableRegex.IsMatch(line))
                            lastJobEnable = jobEnableRegex.Match(line).ToString();
                        if (progNameRegex.IsMatch(line))
                            currentPath = progNameRegex.Match(RemoveCommentABB(line)).ToString();
                        if (typBitRegex.IsMatch(line))
                        {
                            MatchCollection matches = typBitRegex.Matches(line);
                            foreach (var match in matches)
                            {
                                if (!typbits.Any(x => (x as UserBitABB).Name == match.ToString()))
                                    typbits.Add(new UserBitABB(match.ToString(), currentPath, "Typbit"));
                            }
                        }
                        if (endprogRegex.IsMatch(RemoveCommentABB(line)))
                        {
                            currentPath = string.Empty;
                            lastJobEnable = string.Empty;
                        }
                        if (tooldataRegex.IsMatch(line))
                        {
                            string name = tooldataRegex.Match(line).ToString();
                            MatchCollection matches = getnumregex.Matches(line);
                            if (!result.Any(x => (x as ToolDataABB).Name == name))
                            {

                                result.Add(new ToolDataABB(name, float.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[5].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[6].ToString(), CultureInfo.InvariantCulture), robholdRegex.Match(line).ToString()));
                                toolnames.Add(new ToolName(name));
                            }
                            if (!toolloadDatas.Any(x => (x as LoadDataABB).Name == name))
                                toolloadDatas.Add(new LoadDataABB(name, float.Parse(matches[7].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[8].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[9].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[10].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[11].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[12].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[13].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[14].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[15].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[16].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[17].ToString(), CultureInfo.InvariantCulture)));
                        }
                        if (loaddataRegex.IsMatch(line))
                        {
                            string name = loaddataRegex.Match(line).ToString();
                            MatchCollection matches = getnumregex.Matches(line);
                            if (!loadDatas.Any(x => (x as LoadDataABB).Name == name))
                                loadDatas.Add(new LoadDataABB(name, float.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[5].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[6].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[7].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[8].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[9].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[10].ToString(), CultureInfo.InvariantCulture)));
                        }
                        if (wobjdataRegex.IsMatch(line))
                        {
                            string name = wobjdataRegex.Match(line).ToString();
                            MatchCollection matches = getnumregex.Matches(line);
                            if (!baseDatas.Any(x=> (x as WobjDataABB).Name == name))
                            {
                                baseDatas.Add(new WobjDataABB(name, float.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[5].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[6].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[7].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[8].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[9].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[10].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[11].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[12].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[13].ToString(), CultureInfo.InvariantCulture), robholdRegex.Match(line).ToString()));
                                basenames.Add(new BaseName(name));
                            }
                        }
                        if (getHomeRegex.IsMatch(line))
                        {
                            string name = getHomeRegex.Match(line).ToString();
                            MatchCollection matches = getnumregex.Matches(line);
                            float e1;
                            homeposes.Add(new HomePos(float.Parse(matches[0].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[1].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[2].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[3].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[4].ToString(), CultureInfo.InvariantCulture), float.Parse(matches[5].ToString(), CultureInfo.InvariantCulture), matches[6].ToString() != "9" && matches[7].ToString() != "09" &&  float.TryParse(matches[6].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out e1) ? e1 : 0));
                        }
                        if (homeNameRegex.IsMatch(line))
                        {
                            MatchCollection matches = homenumRegex.Matches(line);
                            foreach (var match in matches)
                            {
                                homenamesLong.Add(new HomeNameLong(match.ToString()));
                                homenamesShort.Add(new HomeNameShort(match.ToString()));
                            }
                        }
                        if (userBitInRegex.IsMatch(RemoveCommentABB(line)))
                        {
                            string descr = string.Empty;
                            if (userBitDescr.IsMatch(line))
                                descr = userBitDescr.Match(line).ToString().Trim();
                            userBitsIn.Add(new UserBitABB(userBitInRegex.Match(line).ToString(), currentPath, descr));
                        }
                        if (userBitOutRegex.IsMatch(RemoveCommentABB(line)))
                        {
                            string descr = string.Empty;
                            if (userBitDescr.IsMatch(line))
                                descr = userBitDescr.Match(line).ToString().Trim();
                            userBitsOut.Add(new UserBitABB(userBitOutRegex.Match(line).ToString(), currentPath, descr));
                        }
                    }
                    reader.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                toolloadDatas = null;
                loadDatas = null;
                toolnames = null;
                baseDatas = null;
                basenames = null;
                homeposes = null;
                homenamesShort = null;
                homenamesLong = null;
                userBitsIn = null;
                userBitsOut = null;
                typbits = null;
                jobEnables = null;
                return null;
            }
        }

        private static List<Program> GetProgramsABB(string directory)
        {           
            Regex programStartRegex = new Regex(@"PROC\s+Program_\d+\s*\(\s*\)", RegexOptions.IgnoreCase);
            Regex progNameRegex = new Regex(@"(?<=PROC\s+)Program_\d+(?=\s*\(\s*\))", RegexOptions.IgnoreCase);
            List<Program> result = new List<Program>();
            var files = Directory.GetFiles(directory, "*.mod*", SearchOption.AllDirectories);
            Dictionary<string, int> startHomeDict = GetStartHomesABB(files);
            foreach (var file in files)
            {
                string firstProc = string.Empty;
                bool progStarted = false;
                StreamReader reader = new StreamReader(file);
                string currentProgramName = string.Empty, procName = string.Empty;
                List<string> foundPaths = new List<string>();
                while (!reader.EndOfStream)
                {
                    string line = RemoveCommentABB(reader.ReadLine());
                    if (progStarted)
                    {
                        if (CheckIfIsProc(line, out procName))
                        {
                            foundPaths.Add(procName);
                            if (string.IsNullOrEmpty(firstProc))
                                firstProc = procName.Replace(";","");
                        }
                    }
                    if (programStartRegex.IsMatch(line))
                    {
                        progStarted = true;
                        currentProgramName = progNameRegex.Match(line).ToString();
                    }
                    if (progStarted && line.Trim().Length>=7 && line.ToLower().Trim().Substring(0,7)== "endproc")
                    {
                        progStarted = false;
                        string resultPaths = string.Empty;
                        foundPaths.ForEach(x => resultPaths += x + " ");
                        if (foundPaths.Count > 0)
                        {
                            if (currentProgramName.ToLower().Contains("program_61") || currentProgramName.ToLower().Contains("program_62"))
                                result.Add(new Program(currentProgramName, resultPaths, "1"));
                            else
                                result.Add(new Program(currentProgramName, resultPaths, startHomeDict.Keys.Any(x=>x.ToLower() == firstProc.ToLower()) ? startHomeDict[firstProc].ToString() : "?"));
                            firstProc = string.Empty;
                        }
                        else
                            result.Add(new Program(currentProgramName, "Unknown"));
                        foundPaths = new List<string>();
                    }
                }
                reader.Close();
            }
            return result;
        }

        private static bool CheckIfIsProc(string line, out string procName)
        {
            Regex isProcRegex = new Regex(@"^[\w_-]+\s*;", RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(line))
            { procName = string.Empty; return false; }
            string[] listOfOmits = new string[] { "waituntil ", "if ", "endif", "elseif", "else", "while", "endwhile", "tpwrite", "waittime", "tperase", "endwhile", "test", "endtest", "endproc", "move", "stop", "return" };
            foreach (var keyword in listOfOmits)
            {
                if (line.Trim().Length >= keyword.Length && line.Trim().Substring(0, keyword.Length).ToLower() == keyword)
                {
                    procName = string.Empty;
                    return false;
                }
            }
            if (isProcRegex.IsMatch(line.TrimStart()))
            {
                procName = isProcRegex.Match(line.TrimStart()).ToString();
                return true;
            }
            else
            {
                procName = string.Empty;
                return false;
            }
        }


        private static List<CollZone> GetCollZonesABB(string directory)
        {
            Regex isCollRegex = new Regex(@"(?<=Move(J|L)_Coll(Req|Clr)\s+)", RegexOptions.IgnoreCase);
            Regex collDescrs = new Regex("(?<=Coll_Desc(_.|)\\s*:\\s*=\\s*\")[\\w\\s\\-_,;/()]*", RegexOptions.IgnoreCase);
            Regex getCollNums = new Regex(@"(?<=(Coll(Req|Clr)|:=)\s*)\d+", RegexOptions.IgnoreCase);
            List<CollZone> result = new List<CollZone>();
            var files = Directory.GetFiles(directory, "*.mod*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                StreamReader reader = new StreamReader(file);
                string previousLine = string.Empty, line = string.Empty;
                while (!reader.EndOfStream)
                {
                    previousLine = line;
                    line = reader.ReadLine();
                    if (isCollRegex.IsMatch(RemoveCommentABB(line)))
                    {
                        MatchCollection matchesCollNr = getCollNums.Matches(line);
                        MatchCollection matchesCollDesc = collDescrs.Matches(line);
                        if (matchesCollNr.Count == matchesCollDesc.Count)
                        {
                            int counter = 0;
                            foreach(var coll in matchesCollNr)
                            {
                                if (!result.Any(x=>x.Number == coll.ToString()))
                                {
                                    result.Add(new CollZone(coll.ToString(), matchesCollDesc[counter].ToString()));
                                    counter++;
                                }
                            }
                        }
                        else
                        {
                            foreach (var coll in matchesCollNr)
                                if (!result.Any(x => x.Number == coll.ToString()))
                                    result.Add(new CollZone(coll.ToString(), AddCollComment(previousLine)));
                        }
                    }
                }
                reader.Close();
            }
            result.OrderBy(x => x.Number);
            return result;
        }

        private static string AddCollComment(string previousLine)
        {
            previousLine = previousLine.Trim();
            if (previousLine.Length > 1 && previousLine.Substring(0, 1) == "!")
                return previousLine.Substring(1, previousLine.Length-1);
            else
                return previousLine;
        }

        private static Dictionary<string, int> GetStartHomesABB(string[] files)
        {
            Regex programStartRegex = new Regex(@"(?<=PROC\s+)[\w_-]*(?=\(\s*\))", RegexOptions.IgnoreCase);
            Regex homeNumRegex = new Regex(@"(?<=MoveAbsJ\s+jpHome)\d+", RegexOptions.IgnoreCase);
            Dictionary<string, int> result = new Dictionary<string, int>();
            string currentProcedure = string.Empty;
            bool homeFound = false;
            foreach (var file in files)
            {
                StreamReader reader = new StreamReader(file);
                while(!reader.EndOfStream)
                {
                    string line = RemoveCommentABB(reader.ReadLine());
                    if (programStartRegex.IsMatch(line))
                    {
                        currentProcedure = programStartRegex.Match(line).ToString();
                    }
                    if (!homeFound && homeNumRegex.IsMatch(line))
                    {
                        result.Add(currentProcedure, int.Parse(homeNumRegex.Match(line).ToString()));
                        homeFound = true;
                    }
                    if (line.Trim().Length >= 7 && line.ToLower().Trim().Substring(0, 7) == "endproc")
                    {
                        currentProcedure = string.Empty;
                        homeFound = false;
                    }
                }
                reader.Close();
            }
            return result;
        }
    }
}
