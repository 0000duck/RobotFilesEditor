using CommonLibrary.DataClasses;
using GalaSoft.MvvmLight.Messaging;
using MS.WindowsAPICodePack.Internal;
using ProgramTextFormat.Model.Rules;
using RobotFilesEditor.Model.Operations.CheckGrpperXML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RobotFilesEditor.Model.Operations
{
    internal class CheckGripperXML
    {
        GripperXMLSerializer.Gripper grpConfig;
        InitInHomeContent initInHomeContent;
        internal void Execute()
        {
            Messenger.Default.Send<LogResult>(new LogResult("Comparison of gripper.Xml with InitInHome started", LogResultTypes.Information), "AddLog");
            DeserializeGripperXML();
            ReadInitInHome();
            Compare();
        }

        private void Compare()
        {
            List<string> log = new List<string>();
            for (int i = 1;i<=16;i++)
            {
                System.Reflection.PropertyInfo prop = typeof(GripperXMLSerializer.Gripper).GetProperty("PP" + i);
                object value = prop.GetValue(grpConfig);
                if ((value as GripperXMLSerializer.PPBase).VarSelection != "Not configured")
                {
                    if (!initInHomeContent.Home1.Sensors.Contains(i))
                        log.Add($"Sensor {i} not checked in Home1");
                    if (!initInHomeContent.Home2.Sensors.Contains(i))
                        log.Add($"Sensor {i} not checked in Home2");
                }
            }
            int clampCounter = 1;
            foreach (var actuator in grpConfig.Actuator)
            {
               
                if (actuator.IsUsed.VarValue.Equals("true",StringComparison.OrdinalIgnoreCase))
                {
                    if (!initInHomeContent.Home1.Clamps.Contains(clampCounter))
                        log.Add($"Clamp {clampCounter} not checked in Home1");
                    if (!initInHomeContent.Home2.Clamps.Contains(clampCounter))
                        log.Add($"Clamp {clampCounter} not checked in Home2");
                }
                clampCounter++;
            }
            if (log.Count == 0)
                Messenger.Default.Send<LogResult>(new LogResult("Comparison finished. All clamps and sensors are used in InitInHome.", LogResultTypes.OK), "AddLog");
            else
                log.ForEach(x => Messenger.Default.Send<LogResult>(new LogResult(x, LogResultTypes.Warning), "AddLog"));
        }

        private void DeserializeGripperXML()
        {
            string gripperXML = CommonLibrary.CommonMethods.SelectDirOrFile(false, "Gripper xml", "*.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(GripperXMLSerializer.Gripper));
            using (Stream reader = new FileStream(gripperXML, FileMode.Open))
                grpConfig = (GripperXMLSerializer.Gripper)serializer.Deserialize(reader);

        }
        private void ReadInitInHome()
        {
            initInHomeContent = new InitInHomeContent();
            string currentHome = string.Empty;
            Regex isifHome1Regex = new Regex(@"^\s*IF\s+\$IN_HOME1\s+THEN", RegexOptions.IgnoreCase);
            Regex isifHome2Regex = new Regex(@"^\s*IF\s+\$IN_HOME2\s+THEN", RegexOptions.IgnoreCase);
            Regex partCheckRegex = new Regex(@"(?<=^\s*Grp_ChkPart\s*\(\s*\d+\s*,\s*\w+\s*,\s*)(\d+,){5}", RegexOptions.IgnoreCase);
            Regex grpSetRegex = new Regex(@"(?<=^\s*Grp_GrpSet\s*\(\s*\d+\s*,\s*\#\w+\s*,\s*\"")(\d+,\s*){4}\d", RegexOptions.IgnoreCase);
            string initinhomeFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "Init in home", "*.src");
            using (StreamReader reader = new StreamReader(initinhomeFile))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (isifHome1Regex.IsMatch(line))
                        currentHome = "Home1";
                    if (isifHome2Regex.IsMatch(line))
                        currentHome = "Home2";
                    if (partCheckRegex.IsMatch (line))
                    {
                        var matches = partCheckRegex.Match(line).ToString().Split(',');
                        foreach (var ppcheck in matches.Where(x => !string.IsNullOrEmpty(x)))
                        {
                            var parsed = int.TryParse(ppcheck, out int currPP);
                            if (parsed && currPP > 0)
                                if (currentHome == "Home1" && !initInHomeContent.Home1.Sensors.Contains(currPP))
                                    initInHomeContent.Home1.Sensors.Add(currPP);
                                else if (currentHome == "Home2" && !initInHomeContent.Home2.Sensors.Contains(currPP))
                                    initInHomeContent.Home2.Sensors.Add(currPP);
                        }
                    }
                    if (grpSetRegex.IsMatch(line))
                    {
                        var matches = grpSetRegex.Match(line).ToString().Split(',');
                        foreach (var clamp in matches.Where(x => !string.IsNullOrEmpty(x)))
                        {
                            var parsed = int.TryParse(clamp, out int currClamp);
                            if (parsed && currClamp > 0)
                                if (currentHome == "Home1" && !initInHomeContent.Home1.Clamps.Contains(currClamp))
                                    initInHomeContent.Home1.Clamps.Add(currClamp);
                                else if (currentHome == "Home2" && !initInHomeContent.Home2.Clamps.Contains(currClamp))
                                    initInHomeContent.Home2.Clamps.Add(currClamp);
                        }
                    }
                }
            }
        }
    }
}
