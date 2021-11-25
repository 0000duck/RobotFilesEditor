using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.Operations
{
    public static class GetGripperFromConfigDatMethods
    {
        private static SortedDictionary<int, string> actuatorType;
        private static DataInformations.FileValidationData.Userbits result; 

        public static void ReadConfigDat(string configDatFile = null, bool generatesymname = true)
        {
            if (string.IsNullOrEmpty(configDatFile))
                configDatFile = SelectFile();
            if (File.Exists(configDatFile))
            {
                result = new DataInformations.FileValidationData.Userbits(new SortedDictionary<int, List<string>>(), new SortedDictionary<int, List<string>>());
                DetectActuatorType(configDatFile);
                GetSignalsFromConfigDat(configDatFile, generatesymname);
            }
        }

        private static void DetectActuatorType(string configDatFile)
        {
            actuatorType = new SortedDictionary<int, string>();
            StreamReader reader = new StreamReader(configDatFile);
            if (configDatFile.Substring(configDatFile.Length - 4, 4) == ".dat" || configDatFile.Substring(configDatFile.Length - 4, 4) == ".src")
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.ToLower().Contains("impuls_grp") && line.ToLower().Contains("true"))
                    {
                        Regex getActuatorNr = new Regex(@"(?<=\[)\d+", RegexOptions.IgnoreCase);
                        string actuator = getActuatorNr.Match(line).ToString();
                        if (!actuatorType.Keys.Contains(int.Parse(actuator)))
                            actuatorType.Add(int.Parse(actuator), "Spanner ");
                        else
                            MessageBox.Show("Actuator " + actuator + " is definded both as impuls and vacuum!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    if (line.ToLower().Contains("va_grp") && line.ToLower().Contains("true"))
                    {
                        Regex getActuatorNr = new Regex(@"(?<=\[)\d+", RegexOptions.IgnoreCase);
                        string actuator = getActuatorNr.Match(line).ToString();
                        if (!actuatorType.Keys.Contains(int.Parse(actuator)))
                            actuatorType.Add(int.Parse(actuator), "Vacuum ");
                        else
                        {
                            MessageBox.Show("Actuator " + actuator + " is definded both as impuls and vacuum!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            actuatorType[int.Parse(actuator)] = "Vacuum ";
                        }
                    }
                }
            //}
            //else if (configDatFile.Substring(configDatFile.Length - 4, 4) == ".src")
            //{
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string type = (new Regex(@"#[a-zA-Z0-9]*", RegexOptions.IgnoreCase)).Match(line).ToString();
                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "#IMPULSE")
                        actuatorType.Add(int.Parse((new Regex(@"(?<=Act\s*\[\d+\s*,\s*)\d+", RegexOptions.IgnoreCase).Match(line)).ToString()), "Spanner");
                    else
                        actuatorType.Add(int.Parse((new Regex(@"(?<=Act\s*\[\d+\s*,\s*)\d+", RegexOptions.IgnoreCase).Match(line)).ToString()), "Vacuum");
                }
                    
            }
            for (int i = 1; i <= 48; i++)
            {
                if (!actuatorType.Keys.Contains(i))
                    actuatorType.Add(i, "Spanner ");
            }
            }
            else
            {
            MessageBox.Show("Wrong file format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
            }
        }

        private static void GetSignalsFromConfigDat(string configDatFile, bool generatesymname)
        {
            try
            {
                List<DataClass.V8Clamp> v8Clamps = new List<DataClass.V8Clamp>();
                List<int> v8PPChecks = new List<int>();
                SortedDictionary<int, string> PPs = new SortedDictionary<int, string>();
                //if (GlobalData.SignalNames == null)
                GlobalData.SignalNames = new DataInformations.FileValidationData.Userbits(new SortedDictionary<int, List<string>>(), new SortedDictionary<int, List<string>>());
                StreamReader reader = new StreamReader(configDatFile);
                int PPnr = 11;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if ((line.ToLower().Contains("i_grp_state") || line.ToLower().Contains("o_grp_state") || line.ToLower().Contains("i_grp_pp")) && !line.ToLower().Contains("int "))
                    {
                        Regex getInputRegex = new Regex(@"(?<=\=)\d+", RegexOptions.IgnoreCase);
                        int foundnumber = int.Parse(getInputRegex.Match(line).ToString());
                        if (foundnumber > 0)
                        {
                            Regex getClampGroupRegex = new Regex(@"(?<=\[)\d+", RegexOptions.IgnoreCase);
                            Regex getActuatorRegex = new Regex(@"(?<=,)\d+", RegexOptions.IgnoreCase);
                            Regex getSignalNrRegex = new Regex(@"(?<=\=)\d+", RegexOptions.IgnoreCase);
                            int clampGroup = int.Parse(getClampGroupRegex.Match(line).ToString());
                            int actuator = int.Parse(getActuatorRegex.Match(line).ToString());
                            int signal = int.Parse(getSignalNrRegex.Match(line).ToString());
                            List<string> signals = new List<string>();
                            if (line.ToLower().Contains("i_grp_state1"))
                            {
                                signals.Add(actuatorType[clampGroup] + "Z" + (clampGroup + 10) + "." + actuator + " rueck");
                                if (!GlobalData.SignalNames.Inputs.Keys.Contains(signal))
                                    GlobalData.SignalNames.Inputs.Add(signal, signals);
                                else
                                    MessageBox.Show("Definition for signal " + signal + " already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (line.ToLower().Contains("i_grp_state2"))
                            {
                                signals.Add(actuatorType[clampGroup] + "Z" + (clampGroup + 10) + "." + actuator + " vor");
                                if (!GlobalData.SignalNames.Inputs.Keys.Contains(signal))
                                    GlobalData.SignalNames.Inputs.Add(signal, signals);
                                else
                                    MessageBox.Show("Definition for signal " + signal + " already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (line.ToLower().Contains("o_grp_state2") && signal != 999)
                            {
                                signals.Add(actuatorType[clampGroup] + "Z" + (clampGroup + 10) + " vor");
                                if (!GlobalData.SignalNames.Outputs.Keys.Contains(signal))
                                    GlobalData.SignalNames.Outputs.Add(signal, signals);
                                else
                                    MessageBox.Show("Definition for signal " + signal + " already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (line.ToLower().Contains("o_grp_state1") && signal != 999)
                            {
                                signals.Add(actuatorType[clampGroup] + "Z" + (clampGroup + 10) + " rueck");
                                if (!GlobalData.SignalNames.Outputs.Keys.Contains(signal))
                                    GlobalData.SignalNames.Outputs.Add(signal, signals);
                                else
                                    MessageBox.Show("Definition for signal " + signal + " already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (line.ToLower().Contains("i_grp_pp"))
                            {
                                if (!PPs.Keys.Contains(signal))
                                    PPs.Add(signal, "");
                                //else
                                    //MessageBox.Show("Definition for signal " + signal + " already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                //PPnr++;
                                //GlobalData.SignalNames.Inputs.Add(signal, signals);
                            }
                            else { }

                        }

                    }
                    else if (line.Replace(" ", "").ToLower().Contains("act[") || (line.Replace(" ", "").ToLower().Contains("gripper[")))
                    {
                        if (line.Replace(" ", "").ToLower().Contains("act["))
                        {
                            Regex getClampGroupRegex = new Regex(@"(?<=Act\s*\[\d+\s*,\s*)\d+", RegexOptions.IgnoreCase);
                            Regex getUsedRegex = new Regex(@"(?<=C\d+Used\s+)[a-zA-Z]*", RegexOptions.IgnoreCase);
                            Regex getRetractedRegex = new Regex(@"(?<=C\d+Retracted\s+)\d+", RegexOptions.IgnoreCase);
                            Regex getAdvancedRegex = new Regex(@"(?<=C\d+Advanced\s+)\d+", RegexOptions.IgnoreCase);
                            Regex getOutputAdvancedRegex = new Regex(@"(?<=O_Advanced\s+)\d+", RegexOptions.IgnoreCase);
                            Regex getOutputRetractedRegex = new Regex(@"(?<=O_Retracted\s+)\d+", RegexOptions.IgnoreCase);

                            int number = int.Parse(getClampGroupRegex.Match(line).ToString());
                            SortedDictionary<int, bool> usedActuators = new SortedDictionary<int, bool>();
                            SortedDictionary<int, int> retractInputs = new SortedDictionary<int, int>();
                            SortedDictionary<int, int> advancedInputs = new SortedDictionary<int, int>();
                            int outForClose = int.Parse(getOutputAdvancedRegex.Match(line).ToString());
                            int outForOpen = int.Parse(getOutputRetractedRegex.Match(line).ToString());

                            int counter = 1;
                            foreach (var match in getUsedRegex.Matches(line))
                            {
                                usedActuators.Add(counter, ConvertToBool(match.ToString()));
                                counter++;
                            }
                            counter = 1;
                            foreach (var match in getRetractedRegex.Matches(line))
                            {
                                retractInputs.Add(counter, int.Parse(match.ToString()));
                                counter++;
                            }
                            counter = 1;
                            foreach (var match in getAdvancedRegex.Matches(line))
                            {
                                advancedInputs.Add(counter, int.Parse(match.ToString()));
                                counter++;
                            }
                            v8Clamps.Add(new DataClass.V8Clamp(number, retractInputs, advancedInputs, usedActuators, outForClose, outForOpen));
                        }
                        else
                        {
                            Regex getPPChecks = new Regex(@"(?<=PP\d+\s+)\d+", RegexOptions.IgnoreCase);
                            foreach (var pp in getPPChecks.Matches(line))
                                v8PPChecks.Add(int.Parse(pp.ToString()));
                        }
                    }

                }
                if (PPs.Count > 0)
                {
                    int counter = 1;
                    foreach (var item in PPs)
                    {
                        List<string> tempList = new List<string>();
                        tempList.Add("Teilkontrolle BG" + (10 + counter));
                        if (!GlobalData.SignalNames.Inputs.Keys.Contains(item.Key))
                            GlobalData.SignalNames.Inputs.Add(item.Key, tempList);
                        else
                            MessageBox.Show("PP check at address " + item.Key + " overlaps one of signal clamps!!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        counter++;
                    }
                }
                reader.Close();
                if (generatesymname)
                {
                    CreateGripperMethods.CreateSymName(GlobalData.DestinationPath + "\\C\\KRC\\Roboter\\Data", v8Clamps, v8PPChecks);
                    MessageBox.Show("Gripper signal read from $config.dat and saved at " + GlobalData.DestinationPath + "\\C\\KRC\\Roboter\\Data", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private static bool ConvertToBool(string v)
        {
            if (v.ToLower().Contains("true"))
                return true;
            else
                return false;
        }

        private static string SelectFile()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.Filters.Add(new CommonFileDialogFilter("Dat file", ".dat"));
            dialog.Filters.Add(new CommonFileDialogFilter("Src file", ".src"));
            IDictionary<int, string> result = new Dictionary<int, string>();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
                return "";
        }
    }
    
}
