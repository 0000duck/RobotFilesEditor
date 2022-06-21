using RobotFilesEditor.Dialogs.SignalName;
using RobotFilesEditor.Dialogs.CreateGripper;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RobotFilesEditor.Model.DataInformations.FileValidationData;
using System.Xml.Linq;
using RobotFilesEditor.Dialogs.GripperXMLHelper;
using System.Globalization;

namespace RobotFilesEditor.Model.Operations
{
    public static class CreateGripperMethods
    {
        private static int startAdress, inputUnitLength, outputUnitLength;
        private static string gripperType;
        public enum ClampType { IMPULSE, VACUUM, STATICALLY }; 

        internal static void CreateGripper(ObservableCollection<GripperElementsVM> gripperElements, ObservableCollection<GripperElementSensorsVM> gripperElementSensors, int nrOfInputs, int nrOfOutputs, int gripperNumber, bool hasSoftStart)
        {
            if (!SetSignalNames(gripperElements, gripperElementSensors))
                return;

            string header = "DEF GripperConfig_Grp"+gripperNumber+"()\r\n\r\n";
            string footer = "\r\nEND\r\n";
            string clamps = "";
            string sensors = "";
            string nrOfSignals = "";
            bool success = false;

            if (GlobalData.ControllerType == "KRC2 L6")
            {
                foreach (GripperElementsVM group in gripperElements)
                {
                    clamps += ";****************************\r\n; Signals for group number " + group.SelectedGroup + "\r\n;****************************\r\n\r\n; Inputs for open:\r\n";
                    for (int i = 1; i <= group.SelectedClampsNr; i++)
                    {
                        clamps += "I_GRP_STATE1[" + group.SelectedGroup + "," + i + "]=" + (int.Parse(group.StartAddress) + (i - 1) * 2) + "\r\n";
                    }
                    clamps += "\r\n; Inputs for close:\r\n";
                    for (int i = 1; i <= group.SelectedClampsNr; i++)
                    {
                        clamps += "I_GRP_STATE2[" + group.SelectedGroup + "," + i + "]=" + (int.Parse(group.StartAddress) + (i - 1) * 2 + 1) + "\r\n";
                    }
                    clamps += "\r\n; Output for open:\r\nO_GRP_STATE1[" + group.SelectedGroup + ",1]=" + (group.OutForClose + 1) + "\r\n";
                    clamps += "\r\n; Output for close:\r\nO_GRP_STATE2[" + group.SelectedGroup + ",1]=" + (group.OutForClose) + "\r\n";
                    clamps += "\r\n; Input for pressure/vacuum OK\r\nI_GRP_PR1[" + group.SelectedGroup + "]=1025\r\n";
                    clamps += "\r\n; Input for pressure/vacuum NOK\r\nI_GRP_PR2[" + group.SelectedGroup + "]=1026\r\n";
                    clamps += "\r\n; Set open pos to false\r\nCHECK_OPEN_POS[" + group.SelectedGroup + "]=FALSE\r\n";
                    clamps += "\r\n; Set gripper group type\r\n";
                    if (group.SelectedClampType == "Clamp")
                        clamps += "IMPULS_GRP[" + group.SelectedGroup + "]=TRUE\r\nVA_GRP[" + group.SelectedGroup + "]=FALSE\r\n";
                    else if (group.SelectedClampType == "Vacuum")
                        clamps += "IMPULS_GRP[" + group.SelectedGroup + "]=FALSE\r\nVA_GRP[" + group.SelectedGroup + "]=TRUE\r\n";
                    string name = group.Name;
                    while (name.Length <= 19)
                        name += " ";
                    clamps += "\r\n; Set gripper group name\r\nGRP_NAME" + group.SelectedGroup + "[]=\"" + name + "\"\r\n\r\n";
                }

                foreach (GripperElementSensorsVM sensorGroup in gripperElementSensors)
                {
                    sensors += ";****************************\r\n; Signals for PP checks " + sensorGroup.SelectedGroupSensors + "\r\n;****************************\r\n";
                    if (sensorGroup.SelectedSensor1 != null)
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",1]=" + sensorGroup.SelectedSensor1 + "\r\n";
                    else
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",1]=0\r\n";

                    if (sensorGroup.SelectedSensor2 != null)
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",2]=" + sensorGroup.SelectedSensor2 + "\r\n";
                    else
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",2]=0\r\n";

                    if (sensorGroup.SelectedSensor3 != null)
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",3]=" + sensorGroup.SelectedSensor3 + "\r\n";
                    else
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",3]=0\r\n";

                    if (sensorGroup.SelectedSensor4 != null)
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",4]=" + sensorGroup.SelectedSensor4 + "\r\n";
                    else
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",4]=0\r\n";

                    if (sensorGroup.SelectedSensor5 != null)
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",5]=" + sensorGroup.SelectedSensor5 + "\r\n";
                    else
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",5]=0\r\n";

                    if (sensorGroup.SelectedSensor6 != null)
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",6]=" + sensorGroup.SelectedSensor6 + "\r\n";
                    else
                        sensors += "I_GRP_PP[" + sensorGroup.SelectedGroupSensors + ",6]=0\r\n";

                    string name = sensorGroup.Name;
                    while (name.Length <= 19)
                        name += " ";
                    sensors += "\r\n; PP group name:\r\nGRP_PP_NAME" + sensorGroup.SelectedGroupSensors + "[]=\"" + name + "\"\r\n\r\n";

                }
                nrOfSignals = GetNrOfSignalsL6(gripperElements, gripperElementSensors);
            }

            else if (GlobalData.ControllerType == "KRC2 V8" || GlobalData.ControllerType == "KRC4")
            {
                int softStartAddress = 0;
                string softStartUsedBool = "FALSE";
                if (hasSoftStart)
                {
                    softStartUsedBool = "TRUE";
                    if (GlobalData.ControllerType == "KRC2 V8")
                        softStartAddress = 1041 + (gripperNumber - 1) * 64;
                    else if (GlobalData.ControllerType == "KRC4")
                        softStartAddress = 2049 + (gripperNumber - 1) * 64;
                }
                foreach (GripperElementsVM group in gripperElements)
                {
                    string type = "";
                    string[] usedArray = { "FALSE", "FALSE", "FALSE", "FALSE" };
                    int[] advanceSignalsArray = { 0, 0, 0, 0 }, retractedSingalsArray = { 0, 0, 0, 0 }, outputsArray = { group.OutForClose + 1, group.OutForClose };

                    for (int i = 1; i <= group.SelectedClampsNr; i++)
                    {
                        usedArray[i - 1] = "TRUE";
                        advanceSignalsArray[i - 1] = int.Parse(group.StartAddress) + 1 + 2 * (i - 1);
                        retractedSingalsArray[i - 1] = int.Parse(group.StartAddress) + 2 * (i - 1);
                    }

                    clamps += ";****************************\r\n; Signals for group number " + group.SelectedGroup + "\r\n;****************************\r\n";
                    if (group.SelectedClampType == "Clamp")
                    {
                        type = "#IMPULSE";
                        clamps += "Act[" + gripperNumber + "," + group.SelectedGroup + "]={Num " + group.SelectedGroup + ",NAME[] \"" + group.Name + "\",TYPE " + type + ",IsUsed TRUE,Check TRUE,C1Used " + usedArray[0] + ",C2Used " + usedArray[1] + ",C3Used " + usedArray[2] + ",C4Used " + usedArray[3] + ",I_C1Retracted " + retractedSingalsArray[0] + ",I_C1Advanced " + advanceSignalsArray[0] + ",I_C2Retracted " + retractedSingalsArray[1] + ",I_C2Advanced " + advanceSignalsArray[1] + ",I_C3Retracted " + retractedSingalsArray[2] + ",I_C3Advanced " + advanceSignalsArray[2] + ",I_C4Retracted " + retractedSingalsArray[3] + ",I_C4Advanced " + advanceSignalsArray[3] + ",O_Retracted " + outputsArray[0] + ",O_Advanced " + outputsArray[1] + ",T_ErrWait 1500,T_Retracted 1.0,T_Advanced 1.0,I_VAChnA 0,T_Ret_Pulse 0.0,TOutHandle 0,SetState #SETRetracted}\r\n";
                    }
                    else if (group.SelectedClampType == "Vacuum")
                    {
                        type = "#VACUUM";
                        clamps += "Act[" + gripperNumber + "," + group.SelectedGroup + "]={Num " + group.SelectedGroup + ",NAME[] \"" + group.Name + "\",TYPE " + type + ",IsUsed TRUE,Check TRUE,C1Used TRUE,C2Used FALSE,C3Used FALSE,C4Used FALSE,I_C1Retracted 0,I_C1Advanced 0,I_C2Retracted 0,I_C2Advanced 0,I_C3Retracted 0,I_C3Advanced 0,I_C4Retracted 0,I_C4Advanced 0,O_Retracted " + outputsArray[0] + ",O_Advanced " + outputsArray[1] + ",T_ErrWait 15000,T_Retracted 1.0,T_Advanced 1.0,I_VAChnA "+advanceSignalsArray[0]+",T_Ret_Pulse 0.5,TOutHandle 0,SetState #SETRetracted}\r\n";
                    }
                }

                sensors += ";****************************\r\n; Signals for PP checks \r\n;****************************\r\n";
                int[] sensorsArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int counter = 0;
                foreach (GripperElementSensorsVM sensorGroup in gripperElementSensors)
                {
                    sensorsArray[counter] = sensorGroup.SelectedSensor1.Value;
                    counter++;
                }
                sensors += "Gripper["+gripperNumber+"]={Num "+gripperNumber+",Name[] \"FG00"+gripperNumber+"\",PP1 " + sensorsArray[0] + ",PP2 " + sensorsArray[1] + ",PP3 " + sensorsArray[2] + ",PP4 " + sensorsArray[3] + ",PP5 " + sensorsArray[4] + ",PP6 " + sensorsArray[5] + ",PP7 " + sensorsArray[6] + ",PP8 " + sensorsArray[7] + ",PP9 " + sensorsArray[8] + ",PP10 " + sensorsArray[9] + ",PP11 " + sensorsArray[10] + ",PP12 " + sensorsArray[11] + ",PP13 " + sensorsArray[12] + ",PP14 " + sensorsArray[13] + ",PP15 " + sensorsArray[14] + ",PP16 " + sensorsArray[15] + ",I_PrSwUsed "+softStartUsedBool+",I_PrSw "+softStartAddress+ ",O_SafeValve " + softStartAddress + ",O_CtrlValve 0,I_CtrlValve 0,IsTimeOut FALSE,IsConfig TRUE}\r\n";

                if (GlobalData.ControllerType == "KRC2 V8")
                {
                    nrOfSignals += ";****************************\r\n; Number of inputs and outputs \r\n;****************************\r\n";
                    nrOfSignals += "GrpNumOfIO["+gripperNumber+"]={Inputs " + nrOfInputs + ",Outputs " + nrOfOutputs + "}\r\n";
                }
            }

            string result = header + clamps + sensors + nrOfSignals + footer;

            string destPath = GlobalData.DestinationPath + "\\GripperConfig";
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);
            if (File.Exists(destPath + "\\GripperConfig_Grp" + gripperNumber + ".src"))
            {
                DialogResult dialogResult = MessageBox.Show("File " +destPath + "\\GripperConfig_Grp" + gripperNumber + ".src already exists. Overwrite?", "Overwrite files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    File.Delete(destPath + "\\GripperConfig_Grp" + gripperNumber + ".src");
                    File.WriteAllText(destPath + "\\GripperConfig_Grp" + gripperNumber + ".src", result);
                    success = true;
                }
            }            
            else
            {
                File.WriteAllText(destPath + "\\GripperConfig_Grp" + gripperNumber + ".src", result);
                success = true;
            }
            if (success)
                MessageBox.Show("Gripper config file created", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //WriteAllOrgs(destPath, orgs);
        }

        private static string GetNrOfSignalsL6(ObservableCollection<GripperElementsVM> gripperElements, ObservableCollection<GripperElementSensorsVM> gripperElementSensors)
        {
            int maxClamp = 0, maxPP = 0; 
            foreach(var clamp in gripperElements)
            {
                if (clamp.SelectedGroup > maxClamp)
                    maxClamp = clamp.SelectedGroup;
            }
            foreach (var pp in gripperElementSensors)
            {
                if (pp.SelectedGroupSensors > maxPP)
                    maxPP = pp.SelectedGroupSensors;
            }
            return "GRP_MAX=" + maxClamp + "\r\nPP_MAX=" + maxPP + "\r\n";
        }

        private static bool SetSignalNames(ObservableCollection<GripperElementsVM> gripperElements, ObservableCollection<GripperElementSensorsVM> gripperElementSensors)
        {
            if (GlobalData.SignalNames != null)
                ClearGlobalInputsAndOutputs();
            else
                GlobalData.SignalNames = new DataInformations.FileValidationData.Userbits(new SortedDictionary<int, List<string>>(), new SortedDictionary<int, List<string>>());
            SortedDictionary<int, List<string>> tempOutputs = new SortedDictionary<int, List<string>>();
            foreach (var clamp in gripperElements)
            {
                string type = "";
                if (clamp.SelectedClampType == "Clamp")
                    type = "Spanner";
                else
                    type = "Vacuum";
                if (!tempOutputs.Keys.Contains(clamp.OutForClose))
                    tempOutputs.Add(clamp.OutForClose, new List<string>());
                tempOutputs[clamp.OutForClose].Add(type + " Z" + (clamp.SelectedGroup + 10) + " vor");
                if (!tempOutputs.Keys.Contains(clamp.OutForClose + 1))
                    tempOutputs.Add(clamp.OutForClose + 1, new List<string>());
                tempOutputs[clamp.OutForClose + 1].Add(type + " Z" + (clamp.SelectedGroup + 10) + " rueck");

            }
            if (ValidateOutpusts(tempOutputs))
                foreach (var item in tempOutputs)
                    GlobalData.SignalNames.Outputs.Add(item.Key, item.Value);
            else
            {
                MessageBox.Show("Output signals overlap!\r\nGenration failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            foreach (var clamp in gripperElements)
            {
                string type = "";
                if (clamp.SelectedClampType == "Clamp")
                    type = "Spanner";
                else
                    type = "Vacuum";
                int cnt = 1;
                for (int i = int.Parse(clamp.StartAddress); i < int.Parse(clamp.StartAddress) + clamp.SelectedClampsNr * 2; i = i + 2)
                {
                    if (!GlobalData.SignalNames.Inputs.Keys.Contains(i))
                        GlobalData.SignalNames.Inputs.Add(i, new List<string>());
                    GlobalData.SignalNames.Inputs[i].Add(type + " " + (clamp.SelectedGroup + 10) + "." + cnt.ToString() + " rueck");
                    cnt++;
                }
                cnt = 1;
                for (int i = int.Parse(clamp.StartAddress) + 1; i < int.Parse(clamp.StartAddress) + clamp.SelectedClampsNr * 2; i = i + 2)
                {
                    if (!GlobalData.SignalNames.Inputs.Keys.Contains(i))
                        GlobalData.SignalNames.Inputs.Add(i, new List<string>());
                    GlobalData.SignalNames.Inputs[i].Add(type + " " + (clamp.SelectedGroup + 10) + "." + cnt.ToString() + " vor");
                    cnt++;
                }
            }

            SortedDictionary<int, List<string>> tempPPChecks = new SortedDictionary<int, List<string>>();
            foreach (var sensor in gripperElementSensors)
            {
                if (sensor.SelectedSensor1 != null && !tempPPChecks.Keys.Contains(ConvertToInt(sensor.SelectedSensor1)))
                    tempPPChecks.Add(ConvertToInt(sensor.SelectedSensor1), new List<string>());
                if (sensor.SelectedSensor2 != null && !tempPPChecks.Keys.Contains(ConvertToInt(sensor.SelectedSensor2)))
                    tempPPChecks.Add(ConvertToInt(sensor.SelectedSensor2), new List<string>());
                if (sensor.SelectedSensor3 != null && !tempPPChecks.Keys.Contains(ConvertToInt(sensor.SelectedSensor3)))
                    tempPPChecks.Add(ConvertToInt(sensor.SelectedSensor3), new List<string>());
                if (sensor.SelectedSensor4 != null && !tempPPChecks.Keys.Contains(ConvertToInt(sensor.SelectedSensor4)))
                    tempPPChecks.Add(ConvertToInt(sensor.SelectedSensor4), new List<string>());
                if (sensor.SelectedSensor5 != null && !tempPPChecks.Keys.Contains(ConvertToInt(sensor.SelectedSensor5)))
                    tempPPChecks.Add(ConvertToInt(sensor.SelectedSensor5), new List<string>());
                if (sensor.SelectedSensor6 != null && !tempPPChecks.Keys.Contains(ConvertToInt(sensor.SelectedSensor6)))
                    tempPPChecks.Add(ConvertToInt(sensor.SelectedSensor6), new List<string>());
            }

            int counter = 1;
            foreach (int sensor in tempPPChecks.Keys)
            {
                if (!GlobalData.SignalNames.Inputs.Keys.Contains(sensor))
                    GlobalData.SignalNames.Inputs.Add(sensor, new List<string>());
                GlobalData.SignalNames.Inputs[sensor].Add("Teilekontrolle BG" + (10 + counter).ToString());
                counter++;
            }
            ValidateSignalNames();
            return true;
        }

        internal static void CreateGripperXMLExecute()
        {
            MessageBox.Show("Select gripper config src file", "Select", MessageBoxButtons.OK, MessageBoxIcon.Information);
            string file = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "Dat file", filter1: "*.dat", filter2Descr: "Src file", filter2:"*.src");
            if (!string.IsNullOrEmpty(file))
            {
                CreateGripperViewModel vm;
                if (Path.GetExtension(file).ToLower() == ".src")
                {
                    vm = ReadConfigFromSRC(file);
                    CreateGripperXML(vm.GripperElements, vm.GripperElementSensors, vm.NrOfInputs, vm.NrOfOutputs, vm.SelectedGripperNumber, vm.HasSoftStart, vm.StartAddresses.First().Value, vm.OutsForClose.First().Value);
                }
                else
                {
                    List<GripperFromUserFile> config = ReadConfigFromDat(file);
                    GripperXMLHelperVM vmHelper = new GripperXMLHelperVM();
                    var window = new GripperXMLHelperWindow(vmHelper);
                    window.ShowDialog();

                    startAdress = int.Parse(vmHelper.StartAddress);
                    inputUnitLength = int.Parse(vmHelper.NrOfInputs);
                    outputUnitLength = int.Parse(vmHelper.NrOfOutputs); ;
                    gripperType = vmHelper.FGSelected ? "FG" : "FC";

                    CreateGripperXMLfromDat(config);
                }
            }
        }

        private static void CreateGripperXMLfromDat(List<GripperFromUserFile> config)
        {
            MessageBox.Show("Select directory to save files", "Select dir", MessageBoxButtons.OK, MessageBoxIcon.Information);
            string savePaths = string.Empty;
            string savePath = CommonLibrary.CommonMethods.SelectDirOrFile(true);
            XDocument document = new XDocument();
            foreach (var gripper in config)
            {
                XElement gripperXml = new XElement("Gripper");
                gripperXml.Add(new XElement("Num", new XAttribute("VarValue", "1"), new XAttribute("VarSelection", "1"), new XAttribute("VarInputType", "NoInput")));
                gripperXml.Add(new XElement("Name", new XAttribute("VarValue", "Gripper_" + gripper.Number), new XAttribute("VarSelection", "Gripper_" + gripper.Number), new XAttribute("VarInputType", "Text")));
                gripperXml.Add(new XElement("I_PrSwUsed", new XAttribute("VarValue", gripper.Sensors.SoftStartUsed ? "TRUE" : "FALSE"), new XAttribute("VarSelection" , gripper.Sensors.SoftStartUsed ? "TRUE" : "FALSE"), new XAttribute("VarInputType", "CbBool")));
                if (gripper.Sensors.SoftStartUsed)
                {
                    gripperXml.Add(new XElement("I_PrSw", new XAttribute("VarValue", gripper.Sensors.InputForSoftStart), new XAttribute("VarSelection", CreateSignalName(gripper.Number, gripper.Sensors.InputForSoftStart, true)), new XAttribute("VarInputType", "CbI")));
                    gripperXml.Add(new XElement("O_SafeValve", new XAttribute("VarValue", gripper.Sensors.OutputForSoftStart), new XAttribute("VarSelection", CreateSignalName(gripper.Number, gripper.Sensors.OutputForSoftStart, false)), new XAttribute("VarInputType", "CbO")));
                }
                bool isCtrlValveUsed = gripper.Sensors.OutputForCtrlValve == 0 ? false : true;
                gripperXml.Add(new XElement("UseCtrlValve", new XAttribute("VarValue", isCtrlValveUsed ? "TRUE" : "FALSE"), new XAttribute("VarSelection", isCtrlValveUsed ? "TRUE" : "FALSE"), new XAttribute("VarInputType", "CbBool")));
                if (isCtrlValveUsed)
                {
                    gripperXml.Add(new XElement("I_CtrlValve", new XAttribute("VarValue", gripper.Sensors.InputForCtrlValve), new XAttribute("VarSelection", CreateSignalName(gripper.Number,gripper.Sensors.InputForCtrlValve,true)), new XAttribute("VarInputType", "CbI")));
                    gripperXml.Add(new XElement("O_CtrlValve", new XAttribute("VarValue", gripper.Sensors.OutputForCtrlValve), new XAttribute("VarSelection", CreateSignalName(gripper.Number, gripper.Sensors.OutputForCtrlValve, false)), new XAttribute("VarInputType", "CbO")));

                }
                int counter = 1;
                foreach (var pp in gripper.Sensors.Inputs)
                {
                    gripperXml.Add(new XElement("PP" + counter, new XAttribute("VarValue", pp), new XAttribute("VarSelection", CreateSignalName(gripper.Number, pp, true)), new XAttribute("VarInputType", "CbI")));
                    counter++;
                }
                gripperXml.Add(new XElement("PNetModules"));
                gripperXml.Element("PNetModules").Add(new XElement("Module_1", new XAttribute("VarValue", gripperType + "00" + gripper.Number + "_KF70"), new XAttribute("VarSelection", gripperType + "00" + gripper.Number + "_KF70"), new XAttribute("VarInputType", "NoInput")));
                gripperXml.Element("PNetModules").Add(new XElement("I_Address_1", new XAttribute("VarValue", startAdress), new XAttribute("VarSelection", startAdress), new XAttribute("VarInputType", "Integer")));
                gripperXml.Element("PNetModules").Add(new XElement("O_Address_1", new XAttribute("VarValue", startAdress), new XAttribute("VarSelection", startAdress), new XAttribute("VarInputType", "Integer")));
                gripperXml.Element("PNetModules").Add(new XElement("InUnit_1", new XAttribute("VarValue", inputUnitLength), new XAttribute("VarSelection", inputUnitLength), new XAttribute("VarInputType", "Integer")));
                gripperXml.Element("PNetModules").Add(new XElement("OutUnit_1", new XAttribute("VarValue", outputUnitLength), new XAttribute("VarSelection", outputUnitLength), new XAttribute("VarInputType", "Integer")));

                counter = 1;
                foreach (var acturator in gripper.Actuators)
                {
                    XElement currentActuatorXml = new XElement("Actuator");
                    currentActuatorXml.Add(new XElement("Num", new XAttribute("VarValue", counter), new XAttribute("VarSelection", counter), new XAttribute("VarInputType", "NoInput")));
                    currentActuatorXml.Add(new XElement("Name", new XAttribute("VarValue", acturator.Name), new XAttribute("VarSelection", acturator.Name), new XAttribute("VarInputType", "Text")));
                    string isUsedString = acturator.IsUsed ? "TRUE" : "FALSE";
                    currentActuatorXml.Add(new XElement("IsUsed", new XAttribute("VarValue", isUsedString), new XAttribute("VarSelection", isUsedString), new XAttribute("VarInputType", "CbBool")));
                    if (acturator.IsUsed)
                    {
                        if (acturator.Type == ClampType.IMPULSE)
                        {
                            currentActuatorXml.Add(new XElement("Type", new XAttribute("VarValue", GetActuatorType(acturator.Type)), new XAttribute("VarSelection", GetActuatorType(acturator.Type)), new XAttribute("VarInputType", "CbType")));
                            string isCheckString = acturator.IsCheck ? "TRUE" : "FALSE";
                            currentActuatorXml.Add(new XElement("Check", new XAttribute("VarValue", isCheckString), new XAttribute("VarSelection", isCheckString), new XAttribute("VarInputType", "CbBool")));
                            for (int i = 1; i <= 4; i++)
                            {
                                string isClampUsed = acturator.Clamps[i - 1].IsUsed ? "TRUE" : "FALSE";
                                currentActuatorXml.Add(new XElement("C" + i.ToString() + "Used", new XAttribute("VarValue", isClampUsed), new XAttribute("VarSelection", isClampUsed), new XAttribute("VarInputType", "CbBool")));
                            }
                            currentActuatorXml.Add(new XElement("O_Advanced", new XAttribute("VarValue", acturator.OutputAdvance), new XAttribute("VarSelection", CreateSignalName(gripper.Number, acturator.OutputAdvance, false)), new XAttribute("VarInputType", "CbO")));
                            currentActuatorXml.Add(new XElement("O_Retracted", new XAttribute("VarValue", acturator.OutputRetract), new XAttribute("VarSelection", CreateSignalName(gripper.Number, acturator.OutputRetract, false)), new XAttribute("VarInputType", "CbO")));
                            for (int i = 1; i <= 4; i++)
                            {
                                currentActuatorXml.Add(new XElement("I_C" + i.ToString() + "Advanced", new XAttribute("VarValue", acturator.Clamps[i - 1].InputAdvanced), new XAttribute("VarSelection", CreateSignalName(gripper.Number, acturator.Clamps[i - 1].InputAdvanced, true)), new XAttribute("VarInputType", "CbI")));
                                currentActuatorXml.Add(new XElement("I_C" + i.ToString() + "Retracted", new XAttribute("VarValue", acturator.Clamps[i - 1].InputRetracted), new XAttribute("VarSelection", CreateSignalName(gripper.Number, acturator.Clamps[i - 1].InputRetracted, true)), new XAttribute("VarInputType", "CbI")));
                            }
                            currentActuatorXml.Add(new XElement("T_ErrWait", new XAttribute("VarValue", acturator.TErrWait), new XAttribute("VarSelection", acturator.TErrWait), new XAttribute("VarInputType", "Real")));
                            currentActuatorXml.Add(new XElement("T_Advanced", new XAttribute("VarValue", acturator.T_Advanced), new XAttribute("VarSelection", acturator.T_Advanced), new XAttribute("VarInputType", "Real")));
                            currentActuatorXml.Add(new XElement("T_Retracted", new XAttribute("VarValue", acturator.T_Retracted), new XAttribute("VarSelection", acturator.T_Retracted), new XAttribute("VarInputType", "Real")));
                        }
                        else if (acturator.Type == ClampType.VACUUM)
                        {
                            currentActuatorXml.Add(new XElement("Type", new XAttribute("VarValue", GetActuatorType(acturator.Type)), new XAttribute("VarSelection", GetActuatorType(acturator.Type)), new XAttribute("VarInputType", "CbType")));
                            currentActuatorXml.Add(new XElement("O_Advanced", new XAttribute("VarValue", acturator.OutputAdvance), new XAttribute("VarSelection", CreateSignalName(gripper.Number, acturator.OutputAdvance, false)), new XAttribute("VarInputType", "CbO")));
                            currentActuatorXml.Add(new XElement("O_Retracted", new XAttribute("VarValue", acturator.OutputRetract), new XAttribute("VarSelection", CreateSignalName(gripper.Number, acturator.OutputRetract, false)), new XAttribute("VarInputType", "CbO")));
                            currentActuatorXml.Add(new XElement("T_ErrWait", new XAttribute("VarValue", acturator.TErrWait), new XAttribute("VarSelection", acturator.TErrWait), new XAttribute("VarInputType", "Real")));
                            currentActuatorXml.Add(new XElement("I_VAChnA", new XAttribute("VarValue", acturator.VacuumReachedInput), new XAttribute("VarSelection", CreateSignalName(gripper.Number,acturator.VacuumReachedInput,true)), new XAttribute("VarInputType", "CbI")));
                            currentActuatorXml.Add(new XElement("T_Ret_Pulse", new XAttribute("VarValue", acturator.T_Ret_Pulse), new XAttribute("VarSelection", acturator.T_Ret_Pulse), new XAttribute("VarInputType", "Real")));
                        }
                    }
                    gripperXml.Add(currentActuatorXml);
                    counter++;
                }
                document.Add(gripperXml);
                document.Save(Path.Combine(savePath, "Gripper"+gripper.Number+".xml"));
                savePath += Path.Combine(savePath, "Gripper" + gripper.Number + ".xml");
            }
            MessageBox.Show("Files saved successfully!\r\n" + savePath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static object GetActuatorType(ClampType type)
        {
            switch (type)
            {
                case ClampType.IMPULSE:
                    return "#IMPULSE";
                case ClampType.STATICALLY:
                    return "#STATICALLY";
                case ClampType.VACUUM:
                    return "#VACUUM";
                default:
                    return "ERROR";
            }
        }

        private static string CreateSignalName(int gripperNumber, int signalNumber, bool isInput)
        {
            if (signalNumber == 0)
                return "Not configured";
            int signalNum = signalNumber - startAdress;
            string signalNumString = string.Empty;
            if (signalNum.ToString().Length == 2)
                signalNumString = signalNum.ToString();
            else
                signalNumString = "0" + signalNum.ToString();
            return gripperType + "00" + gripperNumber + "_KF70_d" + (isInput ? "i" : "o") + "_" + signalNumString; 
        }

        private static List<GripperFromUserFile> ReadConfigFromDat(string file)
        {
            Regex isActRegex = new Regex(@"^\s*Act\s*\[\s*\d+\s*,\s*\d+\s*\]", RegexOptions.IgnoreCase);
            Regex isGripperRegex = new Regex(@"^\s*Gripper\s*\[\s*\d+\s*\]", RegexOptions.IgnoreCase);
            List<GripperFromUserFile> grippers = new List<GripperFromUserFile>();
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (isActRegex.IsMatch(line))
                {
                    Actuator currentActuator = new Actuator(line);
                    if (!grippers.Any(x => x.Number == currentActuator.GripperNumber))
                        grippers.Add(new GripperFromUserFile(currentActuator.GripperNumber));
                    grippers.Single(x => x.Number == currentActuator.GripperNumber).Actuators.Add(currentActuator);
                }
                
                if (isGripperRegex.IsMatch(line))
                {
                    Sensors sensors = new Sensors(line);
                    if (sensors.GripperNumber > 0)
                    {
                        if (!grippers.Any(x => x.Number == sensors.GripperNumber))
                            grippers.Add(new GripperFromUserFile(sensors.GripperNumber));
                        grippers.Single(x => x.Number == sensors.GripperNumber).Sensors = sensors;
                    }
                }
            }
            reader.Close();
            grippers = FilterGrippers(grippers);

            return grippers;
        }

        private static List<GripperFromUserFile> FilterGrippers(List<GripperFromUserFile> grippers)
        {
            List<GripperFromUserFile> result = new List<GripperFromUserFile>();
            foreach (var gripper in grippers)
            {
                if (gripper.Actuators.Any(x=>x.IsUsed == true) || gripper.Sensors != null)
                    result.Add(gripper);
            }
            return result;
        }

        private static CreateGripperViewModel ReadConfigFromSRC(string file)
        {
            List<int> startAdresses = new List<int>();
            List<int> outsForClose = new List<int>();
            ObservableCollection<DataOrganization.IntItem> startAddressesIntItems = new ObservableCollection<DataOrganization.IntItem>();
            ObservableCollection<DataOrganization.IntItem> outsForCloseIntItems = new ObservableCollection<DataOrganization.IntItem>();
            Regex ppRegex = new Regex(@"(?<=Gripper\s*\[\s*)\d+", RegexOptions.IgnoreCase);
            Regex isActuatorRegex = new Regex(@"^Act\s*\[\s*\d+\s*,\s*\d+\s*\]", RegexOptions.IgnoreCase);
            Regex hasSoftStartRegex = new Regex(@"(?<=I_PrSwUsed\s+)\w+", RegexOptions.IgnoreCase);
            CreateGripperViewModel result = new CreateGripperViewModel();
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (isActuatorRegex.IsMatch(line))
                {
                    GripperElementsVM currentActuator = GetActuator(line);
                    if (int.Parse(currentActuator.StartAddress) > 0)
                        startAdresses.Add(int.Parse(currentActuator.StartAddress));
                    if (currentActuator.OutForClose > 0)
                        outsForClose.Add(currentActuator.OutForClose);
                    result.GripperElements.Add(currentActuator);
                }
                if (ppRegex.IsMatch(line))
                {
                    result.SelectedGripperNumber = int.Parse(ppRegex.Match(line).ToString());
                    result.GripperElementSensors = GetSensors(line);
                    foreach (var sensor in result.GripperElementSensors)
                    {
                        if (sensor.SelectedSensor1.Value > 0)
                            startAdresses.Add(sensor.SelectedSensor1.Value);
                    }
                    result.HasSoftStart = CommonLibrary.CommonMethods.StringToBool(hasSoftStartRegex.Match(line).ToString());
                }
            }
            reader.Close();

            startAdresses.Sort();
            outsForClose.Sort();
            foreach (int address in startAdresses)
            {
                DataOrganization.IntItem item = new DataOrganization.IntItem();
                item.Value = address;
                startAddressesIntItems.Add(item);
            }
            foreach (int address in outsForClose)
            {
                DataOrganization.IntItem item = new DataOrganization.IntItem();
                item.Value = address;
                outsForCloseIntItems.Add(item);
            }
            result.StartAddresses = startAddressesIntItems;
            result.OutsForClose = outsForCloseIntItems;
            result.NrOfInputs = GetNrOrSignals(result.GripperElements,result.GripperElementSensors, true, result.HasSoftStart, 16);
            result.NrOfOutputs = GetNrOrSignals(result.GripperElements, result.GripperElementSensors, false, result.HasSoftStart, 16);

            return result;
        }

        private static int GetNrOrSignals(ObservableCollection<GripperElementsVM> gripperElements, ObservableCollection<GripperElementSensorsVM> gripperElementSensors, bool isInput, bool hasSoftStart, int cardSize)
        {
            int result = 0;
            List<int> foundSignals = new List<int>();
            if (isInput)
            {
                foreach (var actuator in gripperElements)
                {
                    List<int> tempList = new List<int>();
                    for (int i = int.Parse(actuator.StartAddress); i < int.Parse(actuator.StartAddress) + 2 * actuator.SelectedClampsNr; i++)
                        tempList.Add(i);
                    foundSignals.AddRange(tempList);
                }
                foreach (var pp in gripperElementSensors)
                {
                    foundSignals.Add(pp.SelectedSensor1.Value);
                }
                int lowestInput = foundSignals.Min();
                int startAddress = GetStartAddress(lowestInput, 16);
                for (int i = startAddress; i<foundSignals.Max();i++)
                {
                    if (!foundSignals.Contains(i))
                        foundSignals.Add(i);
                }
                foundSignals.Sort();
                result = (int)Math.Ceiling((double)foundSignals.Count / cardSize) * cardSize;
            }
            else
            {
                if (hasSoftStart)
                    result = 40;
                else
                    result = 32;
            }


            return result;
        }

        private static int GetStartAddress(int lowestInput, int signalsPerCard)
        {
            int nrOfBytes = (int)Math.Floor((double)lowestInput / signalsPerCard);
            int result = nrOfBytes * signalsPerCard + 1;
            return result;
        }

        private static ObservableCollection<GripperElementSensorsVM> GetSensors(string line)
        {
            Regex ppChecks = new Regex(@"(?<=PP\d+\s+)\d+", RegexOptions.IgnoreCase);
            ObservableCollection<GripperElementSensorsVM> result = new ObservableCollection<GripperElementSensorsVM>();
            foreach (var pp in ppChecks.Matches(line))
            {
                if (pp.ToString() != "0")
                {
                    int number = int.Parse(pp.ToString());
                    GripperElementSensorsVM currentElement = new GripperElementSensorsVM();
                    currentElement.SelectedSensor1 = number;
                    result.Add(currentElement);
                }
            }
            return result;
        }


        private static GripperElementsVM GetActuator(string line)
        {
            GripperElementsVM result = new GripperElementsVM();
            Regex groupNameRegex = new Regex("(?<=NAME\\s*\\[\\s*\\]\\s+\")[a-zA-Z0-9\\s_-]*(?=\")", RegexOptions.IgnoreCase);
            Regex typeRegex = new Regex(@"(?<=TYPE\s+)[a-zA-Z0-9#_]*", RegexOptions.IgnoreCase);
            Regex numRegex = new Regex(@"(?<=Num\s+)\d+", RegexOptions.IgnoreCase);
            Regex usedRegex = new Regex(@"(?<=C\dUsed\s+)\w+", RegexOptions.IgnoreCase); // to ma byc matchcollection
            Regex clampStartAdressRegex = new Regex(@"(?<=(I_C1Retracted|I_VAChnA)\s+)\d+", RegexOptions.IgnoreCase);
            Regex outForCloseRegex = new Regex(@"(?<=O_Advanced\s+)\d+", RegexOptions.IgnoreCase);
            result.Name = groupNameRegex.Match(line).ToString();
            if (typeRegex.Match(line).ToString().ToLower().Trim().Replace(" ", "") == "#impulse")
                result.SelectedClampType = "Clamp";
            else
                result.SelectedClampType = "Vacuum";
            result.SelectedGroup = int.Parse(numRegex.Match(line).ToString());
            result.SelectedClampsNr = GetNrOfClamps(usedRegex.Matches(line));
            if (clampStartAdressRegex.Matches(line).Count > 0)
            {
                foreach (var match in clampStartAdressRegex.Matches(line))
                {
                    if (match.ToString().Trim() != "0")
                    {
                        result.StartAddress = match.ToString();
                        break;
                    }
                }
            }
            else
                result.StartAddress = clampStartAdressRegex.Match(line).ToString();
            result.OutForClose = int.Parse(outForCloseRegex.Match(line).ToString());

            return result;
        }

        private static int GetNrOfClamps(MatchCollection matchCollection)
        {
            int result = 0;
            foreach(var used in matchCollection)
            {
                if (used.ToString().ToLower().Trim() == "true")
                    result++;
            }
            return result;
        }

        private static void ClearGlobalInputsAndOutputs()
        {
            SortedDictionary<int, List<string>> filteredInputs = new SortedDictionary<int, List<string>>();
            int start = 0, stop = 0;
            if (GlobalData.ControllerType.Contains("KRC2"))
            {
                start = 1040; stop = 1552;
            }
            else if (GlobalData.ControllerType == "KRC4")
            {
                start = 2048; stop = 2560;
            }
            foreach (var input in GlobalData.SignalNames.Inputs.Where(x => x.Key < start || x.Key > stop))
                filteredInputs.Add(input.Key, input.Value);
            GlobalData.SignalNames.Inputs = filteredInputs;
            SortedDictionary<int, List<string>> filteredOutputs = new SortedDictionary<int, List<string>>();
            foreach (var input in GlobalData.SignalNames.Outputs.Where(x => x.Key < start || x.Key > stop))
                filteredOutputs.Add(input.Key, input.Value);
            GlobalData.SignalNames.Outputs = filteredOutputs;
        }

        private static bool ValidateOutpusts(SortedDictionary<int, List<string>> tempOutputs)
        {
            foreach (var signal in tempOutputs.Where(x => x.Value.Count > 1))
                return false;
            return true;
        }

        private static void ValidateSignalNames()
        {
            SortedDictionary<int, List<string>> temporaryInputs = new SortedDictionary<int, List<string>>();
            foreach (var sensor in GlobalData.SignalNames.Inputs.Where(x => x.Value.Count > 1))
            {
                var vm = new SignalNameViewModel(sensor);
                SignalName sW = new SignalName(vm);
                var dialogResult = sW.ShowDialog();
                temporaryInputs.Add(sensor.Key, new List<string>());
                temporaryInputs[sensor.Key].Add(vm.CorrectedName);
            }
            ClearSignalNames(temporaryInputs);
        }

        private static void ClearSignalNames(SortedDictionary<int, List<string>> temporaryInputs)
        {
            foreach (var input in temporaryInputs)
            {
                GlobalData.SignalNames.Inputs[input.Key] = new List<string>();
                GlobalData.SignalNames.Inputs[input.Key].Add(input.Value[0]);
            }
        }

        private static int ConvertToInt(int? nullableInt)
        {
            if (nullableInt != null)
                return nullableInt.Value;
            else
                return 0;
        }

        internal static bool ValidateData(CreateGripperViewModel vm, bool? dialogResult)
        {
            if (dialogResult == false)
                return false;
            if (vm.GripperElements.Count == 0)
            {
                MessageBox.Show("Clamps definition empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (vm.NrOfInputs == 0 && GlobalData.ControllerType == "KRC2 V8")
            {
                MessageBox.Show("Number of inputs not set!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (vm.NrOfOutputs == 0 && GlobalData.ControllerType == "KRC2 V8")
            {
                MessageBox.Show("Number of outputs not set!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            foreach (var element in vm.GripperElements)
            {
                if (element.SelectedGroup == 0)
                {
                    MessageBox.Show("Gripper clamps group number invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (element.SelectedClampsNr == 0)
                {
                    MessageBox.Show("Invalid number of clamps on clamp group " + element.SelectedGroup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrEmpty(element.StartAddress))
                {
                    MessageBox.Show("Invalid start address for clamp group " + element.SelectedGroup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (element.OutForClose == 0)
                {
                    MessageBox.Show("Invalid out for close on clamp group " + element.SelectedGroup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrEmpty(element.SelectedClampType))
                {
                    MessageBox.Show("Invalid gripper type for clamp group " + element.SelectedGroup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrEmpty(element.Name))
                {
                    MessageBox.Show("Invalid gripper name for clamp group " + element.SelectedGroup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (vm.GripperElementSensors.Count == 0)
            {
                MessageBox.Show("Sensors definition empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            foreach (var sensor in vm.GripperElementSensors)
            {
                if (sensor.SelectedGroupSensors == 0 && GlobalData.ControllerType == "KRC2 L6")
                {
                    MessageBox.Show("Sensors group number invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (sensor.SelectedSensor1 == null && sensor.SelectedSensor2 == null && sensor.SelectedSensor3 == null && sensor.SelectedSensor4 == null && sensor.SelectedSensor5 == null && sensor.SelectedSensor6 == null)
                {
                    MessageBox.Show("No sensors selected for group " + sensor.SelectedGroupSensors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(sensor.Name) && GlobalData.ControllerType == "KRC2 L6")
                {
                    MessageBox.Show("Invalid name for sensor group " + sensor.SelectedGroupSensors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        internal static void CreateSymName(string destination, List<V8Clamp> v8Clamps = null, List<int> v8PPs = null)
        {
            Userbits existingBits = FindExistingSymName();             
            destination = destination.Replace(GlobalData.DestinationPath, "");
            string inputs = "", outputs = "";
            if (v8Clamps == null || v8PPs == null || v8Clamps.Count == 0 || v8PPs.Count == 0)
            {
                foreach (KeyValuePair<int,List<string>> existinSignal in existingBits.Inputs)
                {
                    if (!GlobalData.SignalNames.Inputs.Keys.Contains(existinSignal.Key))
                        GlobalData.SignalNames.Inputs.Add(existinSignal.Key,existinSignal.Value);
                }
                foreach (KeyValuePair<int, List<string>> existinSignal in existingBits.Outputs)
                {
                    if (!GlobalData.SignalNames.Outputs.Keys.Contains(existinSignal.Key))
                        GlobalData.SignalNames.Outputs.Add(existinSignal.Key, existinSignal.Value);
                }
                foreach (var signal in GlobalData.SignalNames.Inputs.Where(x => x.Value.Count > 0))
                    inputs += "IN_" + signal.Key.ToString() + " " + signal.Value[0] + "\r\n";
                foreach (var signal in GlobalData.SignalNames.Outputs.Where(x => x.Value.Count > 0))
                    outputs += "OUT_" + signal.Key.ToString() + " " + signal.Value[0] + "\r\n";
            }
            else
            {
                inputs = GetSymNameInsStringV8(v8Clamps, v8PPs,existingBits);
                outputs = GetSymNameOutsStringV8(v8Clamps, existingBits);
            }
            if (!Directory.Exists(GlobalData.DestinationPath + destination))
                Directory.CreateDirectory(GlobalData.DestinationPath + destination);
            File.WriteAllText(GlobalData.DestinationPath + destination + "\\SymName.txt", inputs + "\r\n" + outputs);
        }

        private static Userbits FindExistingSymName()
        {
            Userbits result = new Userbits(new SortedDictionary<int, List<string>>(), new SortedDictionary<int, List<string>>());
            if (!(File.Exists(GlobalData.DestinationPath + "\\C\\KRC\\Roboter\\Init\\SymName.txt") || File.Exists(GlobalData.DestinationPath + "\\C\\KRC\\Roboter\\Data\\SymName.txt")))
            {
                return result;
            }
            string foundFile = "";
            if (File.Exists(GlobalData.DestinationPath + "\\C\\KRC\\Roboter\\Init\\SymName.txt"))
                foundFile = GlobalData.DestinationPath + "\\C\\KRC\\Roboter\\Init\\SymName.txt";
            else
                foundFile = GlobalData.DestinationPath + "\\C\\KRC\\Roboter\\Data\\SymName.txt";

            StreamReader reader = new StreamReader(foundFile);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    int number = int.Parse(new Regex(@"\d+").Match(line).ToString());
                    List<string> foundDescr = new List<string>();
                    foundDescr.Add(new Regex(@"\s.*").Match(line).ToString().Trim());
                    if (line.ToLower().Contains("in_"))
                        result.Inputs.Add(number, foundDescr);
                    else
                        result.Outputs.Add(number, foundDescr);
                }
            }
            reader.Close();
            return result;
        }

        internal static void CreateGripperXML(ObservableCollection<GripperElementsVM> gripperElements, ObservableCollection<GripperElementSensorsVM> gripperElementSensors, int nrOfInputs, int nrOfOutputs, int selectedGripperNumber, bool hasSoftStart, int startAddressIn, int startAddressOut)
        {
            int softstartInput = 0, softstartOutput = 0,gripperStartAddress = 0, ppCounter = 0;

            string softstartpresent = "FALSE";
            if (hasSoftStart)
            {
                softstartpresent = "TRUE";
                softstartInput = gripperStartAddress = startAddressIn - 2;
                softstartOutput = startAddressOut - 8;
            }
            else
                gripperStartAddress = startAddressIn;

            string header = String.Join(Environment.NewLine, "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>", "<Gripper>", String.Format("<Num VarValue=\"{0}\" VarSelection=\"{1}\" VarInputType=\"NoInput\" />",selectedGripperNumber,selectedGripperNumber),String.Format("<Name VarValue=\"Gripper_{0}\" VarSelection=\"Gripper_{1}\" VarInputType=\"Text\" />",selectedGripperNumber,selectedGripperNumber),"");
            string softstartstring = String.Join(Environment.NewLine,String.Format("<I_PrSwUsed VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"CbBool\" />", softstartpresent), String.Format("<I_PrSw VarValue=\"{0}\" VarSelection=\"FG00{1}_KF70_di_00\" VarInputType=\"CbI\" />", softstartInput, selectedGripperNumber),String.Format("<O_SafeValve VarValue=\"{0}\" VarSelection=\"FG00{1}_KF70_do_00\" VarInputType=\"CbO\" />",softstartOutput,selectedGripperNumber), "<UseCtrlValve VarValue=\"FALSE\" VarSelection=\"FALSE\" VarInputType=\"CbBool\" />","");
            string ppChecks = String.Empty;
            foreach (var pp in gripperElementSensors)
            {
                KeyValuePair<int, string> currentAFandInNum = GetAFandInNum(pp.SelectedSensor1, gripperStartAddress,hasSoftStart,nrOfInputs,true);
                ppCounter++;
                ppChecks += String.Format("<PP{0} VarValue=\"{1}\" VarSelection=\"FG00{2}_KF70_di_{3}\" VarInputType=\"CbI\" />\r\n", ppCounter, pp.SelectedSensor1, selectedGripperNumber, currentAFandInNum.Value);
            }
            ppCounter++;
            for (int i = ppCounter; i <= 16; i++)
            {
                ppChecks += String.Format("<PP{0} VarValue=\"0\" VarSelection=\"Not configured\" VarInputType=\"CbI\" />\r\n", i);
            }
            string pnetModule = String.Join(Environment.NewLine, "<PNetModules>", String.Format("<Module_1 VarValue=\"FG00{0}_KF70\" VarSelection=\"FG00{0}_KF70\" VarInputType=\"NoInput\" />", selectedGripperNumber), String.Format("<I_Address_1 VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"Integer\" />", gripperStartAddress), String.Format("<O_Address_1 VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"Integer\" />", gripperStartAddress), String.Format("<InUnit_1 VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"Integer\" />", nrOfInputs),String.Format("<OutUnit_1 VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"Integer\" />",nrOfOutputs), "</PNetModules>", "");
            string clamps = String.Empty;
            GripperElementsVM currentClamp = new GripperElementsVM();
            for (int i = 1;i <= 16;i++)
            {
                currentClamp = new GripperElementsVM();
                bool clampFound = false, isClamp = false, isVacuum = false;
                foreach (var clamp in gripperElements.Where(x=>x.SelectedGroup == i))
                {
                    currentClamp = clamp;
                    clampFound = true;
                    if (clamp.SelectedClampType == "Clamp")
                        isClamp = true;
                    else
                        isVacuum = true;
                }
                if (clampFound)
                {
                    if (isClamp) // klampa
                    {
                        string[] usedArray = CommonLibrary.CommonMethods.GetArrayBasedOnInt(4,currentClamp.SelectedClampsNr);
                        clamps += String.Join(Environment.NewLine,
                            "<Actuator>",
                            String.Format("<Num VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"NoInput\" />", i),
                            String.Format("<Name VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"Text\" />", currentClamp.Name),
                            "<IsUsed VarValue=\"TRUE\" VarSelection=\"TRUE\" VarInputType=\"CbBool\" />",
                            "<Type VarValue=\"#IMPULSE\" VarSelection=\"#IMPULSE\" VarInputType=\"CbType\" />",
                            "<Check VarValue=\"TRUE\" VarSelection=\"TRUE\" VarInputType=\"CbBool\" />",
                            String.Format("<C1Used VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"CbBool\" />",usedArray[0]),
                            String.Format("<C2Used VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"CbBool\" />", usedArray[1]),
                            String.Format("<C3Used VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"CbBool\" />", usedArray[2]),
                            String.Format("<C4Used VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"CbBool\" />", usedArray[3]),
                            String.Format("<O_Advanced VarValue=\"{0}\" VarSelection=\"FG00{1}_KF70_do_{2}\" VarInputType=\"CbO\" />",currentClamp.OutForClose,selectedGripperNumber, GetAFandInNum(currentClamp.OutForClose, gripperStartAddress, hasSoftStart, nrOfInputs, false).Value),
                            String.Format("<O_Retracted VarValue=\"{0}\" VarSelection=\"FG00{1}_KF70_do_{2}\" VarInputType=\"CbO\" />",currentClamp.OutForClose+1,selectedGripperNumber, GetAFandInNum(currentClamp.OutForClose+1, gripperStartAddress, hasSoftStart, nrOfInputs, false).Value),
                            "");
                        for (int j=1; j<=4; j++)
                        {
                            if (currentClamp.SelectedClampsNr - j >= 0)
                            {
                                clamps += String.Format("<I_C{0}Advanced VarValue=\"{1}\" VarSelection=\"FG00{2}_KF70_di_{3}\" VarInputType=\"CbI\" />",j,int.Parse(currentClamp.StartAddress) + 2*(j-1) + 1,selectedGripperNumber, GetAFandInNum(int.Parse(currentClamp.StartAddress) + 2 * (j - 1) + 1, gripperStartAddress, hasSoftStart, nrOfInputs, true).Value) + "\r\n";
                                clamps += String.Format("<I_C{0}Retracted VarValue=\"{1}\" VarSelection=\"FG00{2}_KF70_di_{3}\" VarInputType=\"CbI\" />", j, int.Parse(currentClamp.StartAddress) + 2 * (j - 1), selectedGripperNumber, GetAFandInNum(int.Parse(currentClamp.StartAddress) + 2 * (j - 1), gripperStartAddress, hasSoftStart, nrOfInputs, true).Value) + "\r\n";
                            }
                            else
                            {
                                clamps += String.Format("<I_C{0}Advanced VarValue=\"0\" VarSelection=\"Not configured\" VarInputType=\"CbI\" />", j) + "\r\n";
                                clamps += String.Format("<I_C{0}Retracted VarValue=\"0\" VarSelection=\"Not configured\" VarInputType=\"CbI\" />", j) + "\r\n";
                            }
                        }
                        clamps += String.Join(Environment.NewLine, "<T_ErrWait VarValue=\"5.0\" VarSelection=\"5.0\" VarInputType=\"Real\" />", "<T_Advanced VarValue=\"1.0\" VarSelection=\"1.0\" VarInputType=\"Real\" />", "<T_Retracted VarValue=\"1.0\" VarSelection=\"1.0\" VarInputType=\"Real\" />","</Actuator>","");
                    }
                    else if (isVacuum) // ssawki
                    {
                        clamps += String.Join(Environment.NewLine,
                            "<Actuator>",
                            String.Format("<Num VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"NoInput\" />", i),
                            String.Format("<Name VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"Text\" />", currentClamp.Name),
                            "<IsUsed VarValue=\"TRUE\" VarSelection=\"TRUE\" VarInputType=\"CbBool\" />",
                            "<Type VarValue=\"#VACUUM\" VarSelection=\"#VACUUM\" VarInputType=\"CbType\" />",
                            String.Format("<O_Advanced VarValue=\"{0}\" VarSelection=\"FG00{1}_KF70_do_{2}\" VarInputType=\"CbO\" />", currentClamp.OutForClose+1, selectedGripperNumber, GetAFandInNum(currentClamp.OutForClose+1, gripperStartAddress, hasSoftStart, nrOfInputs, false).Value),
                            String.Format("<O_Retracted VarValue=\"{0}\" VarSelection=\"FG00{1}_KF70_do_{2}\" VarInputType=\"CbO\" />", currentClamp.OutForClose, selectedGripperNumber, GetAFandInNum(currentClamp.OutForClose, gripperStartAddress, hasSoftStart, nrOfInputs, false).Value),
                            "<T_ErrWait VarValue=\"5.0\" VarSelection=\"5.0\" VarInputType=\"Real\" />",
                            String.Format("<I_VAChnA VarValue=\"{0}\" VarSelection=\"FG00{1}_KF70_di_{2}\" VarInputType=\"CbI\" />",currentClamp.StartAddress,selectedGripperNumber,GetAFandInNum(int.Parse(currentClamp.StartAddress),gripperStartAddress, hasSoftStart, nrOfInputs, true).Value),
                            "<T_Ret_Pulse VarValue=\"0.2\" VarSelection=\"0.2\" VarInputType=\"Real\" />",
                            "</Actuator>","");
                    }
                }
                else // pusty actuator
                {
                    clamps += String.Join(Environment.NewLine,
                        "<Actuator>",
                        String.Format("<Num VarValue=\"{0}\" VarSelection=\"{0}\" VarInputType=\"NoInput\" />", i),
                        String.Format("<Name VarValue=\"Actuator{0}\" VarSelection=\"Actuator{0}\" VarInputType=\"Text\" />", i),
                        "<IsUsed VarValue=\"FALSE\" VarSelection=\"FALSE\" VarInputType=\"CbBool\" />",
                        "</Actuator>", ""
                        );
                }
            }
            clamps += "</Gripper>\r\n";

            bool success = false;
            string resultString = header + softstartstring + ppChecks+ pnetModule + clamps;
            string destPath = GlobalData.DestinationPath + "\\GripperConfig";
            if (!Directory.Exists(GlobalData.DestinationPath + "\\GripperConfig"))
                Directory.CreateDirectory(GlobalData.DestinationPath + "\\GripperConfig");
            if (File.Exists(destPath + "\\Gripper" + selectedGripperNumber + ".xml"))
            {
                DialogResult dialogResult = MessageBox.Show("File " + destPath + "\\Gripper" + selectedGripperNumber + ".xml already exists. Overwrite?", "Overwrite files?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    File.Delete(destPath + "\\Gripper" + selectedGripperNumber + ".xml");
                    File.WriteAllText(destPath + "\\Gripper" + selectedGripperNumber + ".xml", resultString);
                    success = true;
                }
            }
            else
            {
                File.WriteAllText(destPath + "\\Gripper" + selectedGripperNumber + ".xml", resultString);
                success = true;
            }
            if (success)
                MessageBox.Show("Gripper xml file created", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static KeyValuePair<int, string> GetAFandInNum(int? selectedSensor1, int gripperStartAddress,bool hasSoftStart,int nrOfInputs, bool isInput)
        {
            int tempNum = (selectedSensor1.Value - gripperStartAddress) / 16;
            int nrOfCards = tempNum;
            //int signalsPerCard = 16;
            int signalsPerCard = 0;
            int startCard = 2;
            if (!isInput)
            {
                signalsPerCard = 0;
                tempNum = 0;
                if (hasSoftStart)
                {
                    //gripperStartAddress += 8;
                    startCard = 3 + nrOfInputs / 16;
                }
                else
                    startCard = 2 + nrOfInputs / 16;
            }
            return new KeyValuePair<int, string>(startCard+tempNum,CommonLibrary.CommonMethods.GetStringFilledWithZeros(2, (selectedSensor1.Value - gripperStartAddress) - signalsPerCard *nrOfCards));
        }

        private static string GetSymNameOutsStringV8(List<V8Clamp> v8Clamps, Userbits existingBits)
        {
            string result = "";
            SortedDictionary<int, string> foundOutputs = new SortedDictionary<int, string>();
            foreach (var item in existingBits.Outputs)
                foundOutputs.Add(item.Key, item.Value[0]);
            foreach (var clamp in v8Clamps)
            {
                if (!foundOutputs.Keys.Contains(clamp.OutForClose))
                    foundOutputs.Add(clamp.OutForClose, "Spanner Z" + (10 + clamp.Number) + " vor");
                if (!foundOutputs.Keys.Contains(clamp.OutForOpen))
                    foundOutputs.Add(clamp.OutForOpen, "Spanner Z" + (10 + clamp.Number) + " rueck");
            }
            foreach (var output in foundOutputs)
            {
                result += "OUT_" + output.Key + " " + output.Value + "\r\n";
            }

            return result;
        }

        private static string GetSymNameInsStringV8(List<V8Clamp> v8Clamps, List<int> v8PPs, Userbits existingBits)
        {
            string result = "";
            SortedDictionary<int, string> foundInputs = new SortedDictionary<int, string>();
            foreach (var item in existingBits.Inputs)
                foundInputs.Add(item.Key, item.Value[0]);
            foreach (var clamp in v8Clamps)
            {
                foreach (var input in clamp.RetractInputs.Where(x=>x.Value > 0))
                {
                    if (!foundInputs.Keys.Contains(input.Key))
                        foundInputs.Add(input.Value, "Spanner Z" + (10 + clamp.Number) + "." + input.Key + " rueck");
                }
                foreach (var input in clamp.AdvanceInputs.Where(x => x.Value > 0))
                {
                    if (!foundInputs.Keys.Contains(input.Key))
                        foundInputs.Add(input.Value, "Spanner Z" + (10 + clamp.Number) + "." + input.Key + " vor");
                }
            }
            int counter = 1;
            foreach (int pp in v8PPs.Where(x=>x > 0))
            {
                foundInputs.Add(pp, "Teilkontrolle BG" + (10+counter));
                counter++;
            }
            foreach (var input in foundInputs)
            {
                result += "IN_" + input.Key + " " + input.Value + "\r\n";
            }

            return result;
        }
    }

    public class GripperFromUserFile
    {
        public int Number { get; set; }
        public List<Actuator> Actuators { get; set; }
        public Sensors Sensors { get; set; }

        public GripperFromUserFile(int number)
        {
            Number = number;
            Actuators = new List<Actuator>();
        }
    }

    public class Actuator
    {
        public int GripperNumber { get; set; }
        public string Name { get; set; }
        public CreateGripperMethods.ClampType Type { get; set; }
        public bool IsUsed { get; set; }
        public bool IsCheck { get; set; }
        public List<Clamp> Clamps { get; set; }
        public int VacuumReachedInput { get; set; }
        public int OutputRetract { get; set; }
        public int OutputAdvance { get; set; }
        public double TErrWait { get; set; }
        public double T_Retracted { get; set; }
        public double T_Advanced { get; set; }
        public double T_Ret_Pulse { get; set; }
        
        public Actuator(string line)
        {
            Regex gripperNumberRegex = new Regex(@"(?<=^\s*Act\s*\[\s*)\d+", RegexOptions.IgnoreCase);
            Regex nameRegex = new Regex("(?<=Name\\s*\\[\\s*\\]\\s+\"\\s*)[a-zA-Z0-9_\\-\\s]*", RegexOptions.IgnoreCase);
            Regex typeRegex = new Regex(@"(?<=Type\s+#)\w+", RegexOptions.IgnoreCase);
            Regex isUsedRegex = new Regex(@"(?<=IsUsed\s+)\w+", RegexOptions.IgnoreCase);
            Regex isCheckRegex = new Regex(@"(?<=Check\s+)\w+", RegexOptions.IgnoreCase);
            Regex vacuumReachedInputRegex = new Regex(@"(?<=I_VAChnA\s+)\d+", RegexOptions.IgnoreCase);
            Regex outputRetractRegex = new Regex(@"(?<=O_Retracted\s+)\d+", RegexOptions.IgnoreCase);
            Regex outputAdvanceRegex = new Regex(@"(?<=O_Advanced\s+)\d+", RegexOptions.IgnoreCase);
            Regex tErrWaitRegex = new Regex(@"(?<=T_ErrWait\s+)\d+", RegexOptions.IgnoreCase);
            Regex t_RetractedRegex = new Regex(@"(?<=T_Retracted\s+)\d+\.\d+", RegexOptions.IgnoreCase);
            Regex t_AdvancedRegex = new Regex(@"(?<=T_Advanced\s+)\d+\.\d+", RegexOptions.IgnoreCase);
            Regex t_Ret_PulseRegex = new Regex(@"(?<=T_Ret_Pulse\s+)\d+\.\d+", RegexOptions.IgnoreCase);


            GripperNumber = int.Parse(gripperNumberRegex.Match(line).ToString());
            Name = nameRegex.Match(line).ToString();
            switch (typeRegex.Match(line).ToString().ToLower())
            {
                case "impulse":
                    {
                        Type = CreateGripperMethods.ClampType.IMPULSE;
                        break;
                    }
                case "vacuum":
                    {
                        Type = CreateGripperMethods.ClampType.VACUUM;
                        break;
                    }
                case "statically":
                    {
                        Type = CreateGripperMethods.ClampType.STATICALLY;
                        break;
                    }
            }
            IsUsed = isUsedRegex.Match(line).ToString().ToLower() == "true" ? true : false;
            IsCheck = isCheckRegex.Match(line).ToString().ToLower() == "true" ? true : false;
            VacuumReachedInput = int.Parse(vacuumReachedInputRegex.Match(line).ToString());
            OutputRetract = int.Parse(outputRetractRegex.Match(line).ToString());
            OutputAdvance = int.Parse(outputAdvanceRegex.Match(line).ToString());
            TErrWait = double.Parse(tErrWaitRegex.Match(line).ToString()) / 1000;
            T_Retracted = double.Parse(t_RetractedRegex.Match(line).ToString(),CultureInfo.InvariantCulture);
            T_Advanced = double.Parse(t_AdvancedRegex.Match(line).ToString(), CultureInfo.InvariantCulture);
            T_Ret_Pulse = double.Parse(t_Ret_PulseRegex.Match(line).ToString(), CultureInfo.InvariantCulture);

            List<Clamp> clamps = new List<Clamp>();
            for (int i=1;i<=4;i++)
            {
                Regex usedRegex = new Regex(@"(?<=C" + i.ToString() + @"Used\s+)\w+", RegexOptions.IgnoreCase);
                Regex inputRetractedRegex = new Regex(@"(?<=I_C" + i.ToString() + @"Retracted\s+)\d+", RegexOptions.IgnoreCase);
                Regex inputAdvancedRegex = new Regex(@"(?<=I_C" + i.ToString() + @"Advanced\s+)\d+", RegexOptions.IgnoreCase);
                clamps.Add(new Clamp(i, usedRegex.Match(line).ToString().ToLower() == "true" ? true : false, int.Parse(inputRetractedRegex.Match(line).ToString()), int.Parse(inputAdvancedRegex.Match(line).ToString())));
            }
            Clamps = clamps;
        }
    }

    public class Clamp
    {
        public int Number { get; set; }
        public bool IsUsed { get; set; }
        public int InputRetracted { get; set; }
        public int InputAdvanced { get; set; }

        public Clamp(int number, bool isUsed, int inputRetracted, int inputAdvanced)
        {
            Number = number;
            IsUsed = isUsed;
            InputRetracted = inputRetracted;
            InputAdvanced = inputAdvanced;
        }
    }

    public class Sensors
    {
        public string Name { get; set; }
        public int GripperNumber { get; set; }
        public List<int> Inputs { get; set; }
        public bool SoftStartUsed { get; set; }
        public int InputForSoftStart { get; set; }
        public int OutputForSoftStart { get; set; }
        public int InputForCtrlValve { get; set; }
        public int OutputForCtrlValve { get; set; }

        public Sensors(string line)
        {
            Regex nameRegex = new Regex("(?<=Name\\s*\\[\\s*\\]\\s+\"\\s*)[a-zA-Z0-9_\\-\\s]*", RegexOptions.IgnoreCase);
            Regex grpNumberRegex = new Regex(@"(?<=Num\s+)\d+", RegexOptions.IgnoreCase);
            Regex softStartUsedRegex = new Regex(@"(?<=I_PrSwUsed\s+)\w+", RegexOptions.IgnoreCase);
            Regex inputForSoftStartRegex = new Regex(@"(?<=I_PrSw\s+)\d+", RegexOptions.IgnoreCase);
            Regex outputForSoftStartRegex = new Regex(@"(?<=O_SafeValve\s+)\d+", RegexOptions.IgnoreCase);
            Regex inputForCtrlValveRegex = new Regex(@"(?<=I_CtrlValve\s+)\d+", RegexOptions.IgnoreCase);
            Regex outputForCtrlValveRegex = new Regex(@"(?<=O_CtrlValve\s+)\d+", RegexOptions.IgnoreCase);

            Name = nameRegex.Match(line).ToString();
            GripperNumber = int.Parse(grpNumberRegex.Match(line).ToString());
            SoftStartUsed = softStartUsedRegex.Match(line).ToString().ToLower() == "true" ? true : false;
            InputForSoftStart = int.Parse(inputForSoftStartRegex.Match(line).ToString());
            OutputForSoftStart = int.Parse(outputForSoftStartRegex.Match(line).ToString());
            InputForCtrlValve = int.Parse(inputForCtrlValveRegex.Match(line).ToString());
            OutputForCtrlValve = int.Parse(outputForCtrlValveRegex.Match(line).ToString());
            List<int> inputs = new List<int>();
            for (int i = 1; i <= 16; i++)
            {
                Regex ppInputRegex = new Regex(@"(?<=PP" +i + @"\s+)\d+", RegexOptions.IgnoreCase);
                inputs.Add(int.Parse(ppInputRegex.Match(line).ToString()));
            }
            Inputs = inputs;
        }
    }
}
