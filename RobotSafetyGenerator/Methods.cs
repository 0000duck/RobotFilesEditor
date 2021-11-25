using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using CommonLibrary;

namespace RobotSafetyGenerator
{

    public static class Methods
    {
        public static string resultString;
        #region XML Build
        public static string BuildXMLABB(SafetyConfigABB config, string selectedRobot)
        {
            int loopCounter = 0;
            resultString = "";
            bool isPermanent;
            string header = "<SimplifiedSafetyConfiguration version=\"1.0\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:abb-robotics-simplified-safety-controller-configuration\">\n\t<DriveModuleConfiguration DriveModuleId=\"1\">\n";
            resultString = resultString + header;
            if (config.Elbow != null)
                CreateElbowABBXML(config.Elbow, config.RobotName, selectedRobot);
            if (config.SafeTools.Count > 1)
                isPermanent = false;
            else
                isPermanent = true;
            foreach (SafeToolABB tool in config.SafeTools)
            {
                CreateToolABBXml(tool, isPermanent, loopCounter);
                loopCounter += 1;
            }
            foreach (SafeZoneABB zone in config.SafeZones)
                CreateZoneABBXML(zone);
            resultString = resultString + "\t</DriveModuleConfiguration>\n</SimplifiedSafetyConfiguration>";
            return resultString;
        }

        internal static string BuildXMLFanuc(SafetyConfigFanuc configFanuc)
        {
            if (configFanuc == null)
            {
                MessageBox.Show("Safety Configuration does not exist. Reload the XML file");
                return "";
            }

            resultString = "";
            string currentString = "";
            string header = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n<XMLVAR version=\"V8.3607        2/8/2017\">\n <PROG name=\"*SYSTEM*\">\n  <VAR name=\"$DCSS_T1SC\">\n   <ARRAY name=\"$DCSS_T1SC[1]\">\n    <FIELD name=\"$ENABLE\" prot=\"RW\">1</FIELD>\n    <FIELD name=\"$SPD_LIM\" prot=\"RW\">2.500000000e+02</FIELD>\n   </ARRAY>\n  </VAR>\n  <VAR name=\"$DCSS_TCP\">\n   <ARRAY name=\"$DCSS_TCP[1]\" />\n  </VAR>\n  <VAR name=\"$DCSS_UFRM\">\n   <ARRAY name=\"$DCSS_UFRM[1]\">\n";
            resultString = resultString + header;
            for (int i = 1; i <= 9; i++)
            {
                if (i <= configFanuc.UserFrames.Count)
                {
                    currentString = currentString + String.Join(
                        Environment.NewLine,
                        "    <ARRAY name=\"$DCSS_UFRM[1," + i.ToString() + "]\">",
                        "     <FIELD name=\"$COMMENT\" prot=\"RW\"></FIELD>",
                        "     <FIELD name=\"$UFRM_NUM\" prot=\"RW\">" + ((configFanuc.UserFrames[i - 1].Number) + 900).ToString() + "</FIELD>",
                        "     <FIELD name=\"$X\" prot=\"RW\">" + configFanuc.UserFrames[i - 1].Point.Xpos + "</FIELD>",
                        "     <FIELD name=\"$Y\" prot=\"RW\">" + configFanuc.UserFrames[i - 1].Point.Ypos + "</FIELD>",
                        "     <FIELD name=\"$Z\" prot=\"RW\">" + configFanuc.UserFrames[i - 1].Point.Zpos + "</FIELD>",
                        "     <FIELD name=\"$W\" prot=\"RW\">" + configFanuc.UserFrames[i - 1].Point.A + "</FIELD>",
                        "     <FIELD name=\"$P\" prot=\"RW\">" + configFanuc.UserFrames[i - 1].Point.B + "</FIELD>",
                        "     <FIELD name=\"$R\" prot=\"RW\">" + configFanuc.UserFrames[i - 1].Point.C + "</FIELD>",
                        "	</ARRAY>\n"

                        );
                }
                else
                {
                    currentString = currentString + String.Join(
                        Environment.NewLine,
                        "    <ARRAY name=\"$DCSS_UFRM[1," + i.ToString() + "]\">",
                        "     <FIELD name=\"$COMMENT\" prot=\"RW\"></FIELD>",
                        "     <FIELD name=\"$UFRM_NUM\" prot=\"RW\">" + (i + 900).ToString() + "</FIELD>",
                        "     <FIELD name=\"$X\" prot=\"RW\">0.000000000e+00</FIELD>",
                        "     <FIELD name=\"$Y\" prot=\"RW\">0.000000000e+00</FIELD>",
                        "     <FIELD name=\"$Z\" prot=\"RW\">0.000000000e+00</FIELD>",
                        "     <FIELD name=\"$W\" prot=\"RW\">0.000000000e+00</FIELD>",
                        "     <FIELD name=\"$P\" prot=\"RW\">0.000000000e+00</FIELD>",
                        "     <FIELD name=\"$R\" prot=\"RW\">0.000000000e+00</FIELD>",
                        "	</ARRAY>\n"

                        );
                }
            }
            resultString += (currentString + "   </ARRAY>\n" + "  </VAR>\n");
            CreateDCSS_MODELFanucXML(configFanuc.SafeTools, configFanuc.Elbow);
            CreateDCSS_CPCFanucXML(configFanuc.SafeZones);
            resultString = resultString + " </PROG>\n</XMLVAR>";
            return resultString;
        }

        private static void CreateDCSS_MODELFanucXML(List<SafeToolFanuc> tools, ElbowFanuc elbow)
        {
            int counter = 1;
            string currentString = "  <VAR name=\"$DCSS_MODEL\">\n";
            foreach (SafeToolFanuc tool in tools)
            {
                counter = 1;
                int nrOfElements = tool.Boxes.Count + tool.Capsules.Count + tool.Spheres.Count;
                currentString = currentString + String.Join(
                            Environment.NewLine,
                            "   <ARRAY name = \"$DCSS_MODEL[" + tool.Number.ToString() + "]\">",
                            "    <FIELD name=\"$COMMENT\" prot=\"RW\">" + tool.Name + "</FIELD>",
                            "    <FIELD name=\"$ELEM\">\n");
                while (counter <= nrOfElements)
                {
                    foreach (SphereFanuc sphere in tool.Spheres)
                    {
                        currentString = currentString + String.Join(
                            Environment.NewLine,
                            "     <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "]\">",
                            "      <FIELD name=\"$USE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$LINK_NO\" prot=\"RW\">99</FIELD>",
                            "      <FIELD name=\"$LINK_TYPE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$UTOOL_NUM\" prot=\"RW\">0</FIELD>",
                            "      <FIELD name=\"$SHAPE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$SIZE\">",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$SIZE[1]\" prot=\"RW\">" + sphere.Radius.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$SIZE[2]\" prot=\"RW\">0.000000000e+00</ARRAY>",
                            "      </FIELD>",
                            "      <FIELD name=\"$DATA\">",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[1]\" prot=\"RW\">" + sphere.CenterPoint.Xpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[2]\" prot=\"RW\">" + sphere.CenterPoint.Ypos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[3]\" prot=\"RW\">" + sphere.CenterPoint.Zpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[4]\" prot=\"RW\">0.000000000e+00</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[5]\" prot=\"RW\">0.000000000e+00</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[6]\" prot=\"RW\">0.000000000e+00</ARRAY>",
                            "      </FIELD>",
                            "     </ARRAY>\n");
                        counter++;
                    }
                    foreach (CapsuleFanuc capsule in tool.Capsules)
                    {
                        currentString = currentString + String.Join(
                            Environment.NewLine,
                            "     <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "]\">",
                            "      <FIELD name=\"$USE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$LINK_NO\" prot=\"RW\">99</FIELD>",
                            "      <FIELD name=\"$LINK_TYPE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$UTOOL_NUM\" prot=\"RW\">0</FIELD>",
                            "      <FIELD name=\"$SHAPE\" prot=\"RW\">2</FIELD>",
                            "      <FIELD name=\"$SIZE\">",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$SIZE[1]\" prot=\"RW\">" + capsule.Radius.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$SIZE[2]\" prot=\"RW\">0.000000000e+00</ARRAY>",
                            "      </FIELD>",
                            "      <FIELD name=\"$DATA\">",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[1]\" prot=\"RW\">" + capsule.Point1.Xpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[2]\" prot=\"RW\">" + capsule.Point1.Ypos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[3]\" prot=\"RW\">" + capsule.Point1.Zpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[4]\" prot=\"RW\">" + capsule.Point2.Xpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[5]\" prot=\"RW\">" + capsule.Point2.Ypos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[6]\" prot=\"RW\">" + capsule.Point2.Zpos.ToString() + "</ARRAY>",
                            "      </FIELD>",
                            "     </ARRAY>\n");
                        counter++;
                    }
                    foreach (BoxFanuc box in tool.Boxes)
                    {
                        currentString = currentString + String.Join(
                            Environment.NewLine,
                            "     <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "]\">",
                            "      <FIELD name=\"$USE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$LINK_NO\" prot=\"RW\">99</FIELD>",
                            "      <FIELD name=\"$LINK_TYPE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$UTOOL_NUM\" prot=\"RW\">0</FIELD>",
                            "      <FIELD name=\"$SHAPE\" prot=\"RW\">4</FIELD>",
                            "      <FIELD name=\"$DATA\">",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[1]\" prot=\"RW\">" + box.CenterPoint.Xpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[2]\" prot=\"RW\">" + box.CenterPoint.Ypos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[3]\" prot=\"RW\">" + box.CenterPoint.Zpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[4]\" prot=\"RW\">" + box.Width.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[5]\" prot=\"RW\">" + box.Height.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[6]\" prot=\"RW\">" + box.Thickness.ToString() + "</ARRAY>",
                            "      </FIELD>",
                            "     </ARRAY>\n");
                        counter++;
                    }
                    if (elbow.Elbow != null)
                    {
                        currentString = currentString + String.Join(
                            Environment.NewLine,
                            "     <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "]\">",
                            "      <FIELD name=\"$USE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$LINK_NO\" prot=\"RW\">3</FIELD>",
                            "      <FIELD name=\"$LINK_TYPE\" prot=\"RW\">1</FIELD>",
                            "      <FIELD name=\"$UTOOL_NUM\" prot=\"RW\">0</FIELD>",
                            "      <FIELD name=\"$SHAPE\" prot=\"RW\">2</FIELD>",
                            "      <FIELD name=\"$SIZE\">",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$SIZE[1]\" prot=\"RW\">" + elbow.Elbow.Radius.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$SIZE[2]\" prot=\"RW\">0.000000000e+00</ARRAY>",
                            "      </FIELD>",
                            "      <FIELD name=\"$DATA\">",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[1]\" prot=\"RW\">" + elbow.Elbow.Point1.Xpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[2]\" prot=\"RW\">" + elbow.Elbow.Point1.Ypos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[3]\" prot=\"RW\">" + elbow.Elbow.Point1.Zpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[4]\" prot=\"RW\">" + elbow.Elbow.Point2.Xpos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[5]\" prot=\"RW\">" + elbow.Elbow.Point2.Ypos.ToString() + "</ARRAY>",
                            "       <ARRAY name=\"$DCSS_MODEL[" + tool.Number.ToString() + "].$ELEM[" + counter.ToString() + "].$DATA[6]\" prot=\"RW\">" + elbow.Elbow.Point2.Zpos.ToString() + "</ARRAY>",
                            "      </FIELD>",
                            "     </ARRAY>\n");
                    }
                }

                currentString = currentString + "    </FIELD>\n   </ARRAY>\n";
            }
            currentString = currentString + "  </VAR>\n";
            resultString = resultString + currentString;
        }

        internal static IDictionary<string, string> MirrorSafety(IDictionary<string, string> resultString)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            result.Add("src", resultString["src"]);
            string resultDat = string.Empty;
            StringReader reader = new StringReader(resultString["dat"]);
            Regex pointRegex = new Regex(@"(?<=E6POS.*(X|Y|Z|A|B|C)\s+)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;
                if (pointRegex.IsMatch(line))
                {
                    MatchCollection matches = pointRegex.Matches(line);
                    if (matches.Count == 6)
                    {
                        Regex pointNameRegex = new Regex(@"(?<=E6POS\s+)[\w-_]+", RegexOptions.IgnoreCase);
                        double YValMirrored = (double.Parse(matches[1].ToString(), CultureInfo.InvariantCulture)) * (-1);
                        double AValMirrored = (double.Parse(matches[3].ToString(), CultureInfo.InvariantCulture)) * (-1);
                        double CValMirrored = (double.Parse(matches[5].ToString(), CultureInfo.InvariantCulture)) * (-1);
                        resultDat += "DECL E6POS " + pointNameRegex.Match(line).ToString() + "={X " + matches[0].ToString() + ",Y " + YValMirrored + ",Z " + matches[2].ToString() + ",A " + AValMirrored + ",B " + matches[4].ToString() + ",C " + CValMirrored + "}\r\n";
                    }
                }
                else
                    resultDat += line + "\r\n";
            }
            result.Add("dat", resultDat);
            reader.Close();
            return result;
        }

        internal static SafetyConfigKukaKRC4 ConvertKukaConfigurations(SafetyConfig safetyConfig)
        {
            List<SafeToolKUKA> tools = new List<SafeToolKUKA>();
            List<PointEuler> pointsInCellspace = new List<PointEuler>();
            List<SafeZoneKUKA> safeZones = new List<SafeZoneKUKA>(); 
            safetyConfig.Cellspace.Points.Values.ToList().ForEach(x => pointsInCellspace.Add(new PointEuler(Convert.ToSingle(x.Xpos), Convert.ToSingle(x.Ypos), Convert.ToSingle(x.Zpos), 0, 0, 0)));
            safetyConfig.SafeSpaces.ForEach(x => safeZones.Add(new SafeZoneKUKA((x as SafeSpace2points).Number, "Zone" + (x as SafeSpace2points).Number, new PointEuler(Convert.ToSingle((x as SafeSpace2points).Origin.Xpos), Convert.ToSingle((x as SafeSpace2points).Origin.Ypos), Convert.ToSingle((x as SafeSpace2points).Origin.Zpos), 0, 0, 0), new PointEuler(Convert.ToSingle((x as SafeSpace2points).Max.Xpos), Convert.ToSingle((x as SafeSpace2points).Max.Ypos), Convert.ToSingle((x as SafeSpace2points).Max.Zpos), 0, 0, 0), GetPointsAtSafeZoneBase(new PointEuler(Convert.ToSingle((x as SafeSpace2points).Origin.Xpos), Convert.ToSingle((x as SafeSpace2points).Origin.Ypos), Convert.ToSingle((x as SafeSpace2points).Origin.Zpos), 0, 0, 0), new PointEuler(Convert.ToSingle((x as SafeSpace2points).Max.Xpos), Convert.ToSingle((x as SafeSpace2points).Max.Ypos), Convert.ToSingle((x as SafeSpace2points).Max.Zpos), 0, 0, 0)))));
            return new SafetyConfigKukaKRC4(tools, new CellSpaceKuka(pointsInCellspace, 5000), safeZones, "Robot");
        }

        internal static SafetyConfigABB ConvertMilimetersInTools(SafetyConfigABB configABB)
        {
            SafetyConfigABB result = new SafetyConfigABB();
            ElbowABB elbow = new ElbowABB();

            return result;
        }

        internal static string ReadCompleteData(string fileName)
        {
            bool addLine = false;
            string result = "<!--W.E.S.T. GmbH 2019, Safety Robots, Version 3.0.0.2-->\r\n<!--Creation Date: 17.01.19 13:16:46-->\r\n<!--Documentation Version: -->\r\n<SimplifiedSafetyConfiguration version=\"1.0\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:abb-robotics-simplified-safety-controller-configuration\">\r\n";
            StreamReader reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("drivemoduleconfiguration"))
                {
                    Regex reg = new Regex("\\s+maxSpeedManualMode\\=\\s*\"\\d+\\.\\d+\"\\s*", RegexOptions.IgnoreCase);
                    line = reg.Replace(line, "");
                    addLine = true;
                }
                if (line.ToLower().Contains("<baseframe>"))
                    addLine = false;
                if (line.ToLower().Contains("<elbowoffset"))
                    addLine = true;
                if (line.ToLower().Contains("<upperarmgeometry"))
                    addLine = true;
                if (line.ToLower().Contains("<joint"))
                    addLine = false;
                if (line.ToLower().Contains("</robot>"))
                    addLine = true;
                if (line.ToLower().Contains("<tool "))
                    addLine = true;
                if (line.ToLower().Contains("<synccheck"))
                    addLine = false;
                if (line.ToLower().Contains("<cyclicbrakecheck"))
                    addLine = false;
                if (line.ToLower().Contains("<safezone "))
                    addLine = true;
                if (line.ToLower().Contains("<toolpositionsupervision"))
                    addLine = false;
                if (line.ToLower().Contains("</drivemoduleconfiguration>"))
                    addLine = true;
                if (line.ToLower().Contains("</safetycfg>") || line.ToLower().Contains("</configurationseal>") || line.ToLower().Contains("</safetyconfiguration>"))
                    addLine = false;
                if (addLine)
                    result += line + "\r\n";
            }
            result += "</SimplifiedSafetyConfiguration>";
            reader.Close();
            return result;
        }

        private static void CreateDCSS_CPCFanucXML(SafeZonesFanuc safeZones)
        {
            int pointCounter = 1, zoneCounter = 901;
            string currentString = "  <VAR name=\"$DCSS_CPC\">\n";
            foreach (SafeZoneFanucTwoPoints zoneDiagonal in safeZones.SafeZonesTwoPoints)
            {
                string mode = "", uframe = "";
                if (zoneDiagonal.IsDI & zoneDiagonal.Number == 32)
                { mode = "0"; uframe = "0"; }
                else if (zoneDiagonal.IsDI & zoneDiagonal.Number != 32)
                { mode = "0"; uframe = ((zoneDiagonal.UFrame.Number) + 900).ToString(); }
                else
                { mode = "1"; uframe = ((zoneDiagonal.UFrame.Number) + 900).ToString(); }
                currentString = currentString + String.Join(
                    Environment.NewLine,
                    "   <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "]\">",
                    "    <FIELD name=\"$COMMENT\" prot=\"RW\">" + zoneDiagonal.Name + "</FIELD>",
                    "    <FIELD name=\"$ENABLE\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$MODE\" prot=\"RW\">" + mode + "</FIELD>",
                    "    <FIELD name=\"$GRP_NUM\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$MODEL_NUM\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$MODEL_NUM[1]\" prot=\"RW\">-2</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$MODEL_NUM[2]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$MODEL_NUM[3]\" prot=\"RW\">0</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$X\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$X[1]\" prot=\"RW\">" + zoneDiagonal.StartPoint.Xpos.ToString() + "</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$X[2]\" prot=\"RW\">" + zoneDiagonal.EndPoint.Xpos.ToString() + "</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$Y\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$Y[1]\" prot=\"RW\">" + zoneDiagonal.StartPoint.Ypos.ToString() + "</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$Y[2]\" prot=\"RW\">" + zoneDiagonal.EndPoint.Ypos.ToString() + "</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$Z1\" prot=\"RW\">" + zoneDiagonal.StartPoint.Zpos.ToString() + "</FIELD>",
                    "    <FIELD name=\"$Z2\" prot=\"RW\">" + zoneDiagonal.EndPoint.Zpos.ToString() + "</FIELD>",
                    "    <FIELD name=\"$UFRM_NUM\" prot=\"RW\">" + uframe + "</FIELD>",
                    "    <FIELD name=\"$STOP_TYP\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$DSBIO_TY\" prot=\"RW\">10</FIELD>",
                    "    <FIELD name=\"$DSBIO_IDX\" prot=\"RW\">33</FIELD>",
                    "    <FIELD name=\"$ENBL_CALMD\" prot=\"RW\">0</FIELD>",
                    "    <FIELD name=\"$USE_PREDICT\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$DELAY_TIME\" prot=\"RW\">0</FIELD>",
                    "    <FIELD name=\"$SPEED_CTRL\" prot=\"RW\">0</FIELD>",
                    "    <FIELD name=\"$OVR_LIMT\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$OVR_LIMT[1]\" prot=\"RW\">100</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$OVR_LIMT[2]\" prot=\"RW\">100</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$OVR_LIMT[3]\" prot=\"RW\">100</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$OVR_LIMT[4]\" prot=\"RW\">100</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$SPD_LIMT\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$SPD_LIMT[1]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$SPD_LIMT[2]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$SPD_LIMT[3]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$SPD_LIMT[4]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$DSBL_TYP\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_TYP[1]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_TYP[2]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_TYP[3]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_TYP[4]\" prot=\"RW\">0</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$DSBL_IDX\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_IDX[1]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_IDX[2]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_IDX[3]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneDiagonal.Number.ToString() + "].$DSBL_IDX[4]\" prot=\"RW\">0</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$UTOOL_NUM\" prot=\"RW\">0</FIELD>",
                    "   </ARRAY>\n"
                    );
                zoneCounter++;
                if (zoneCounter == 910)
                    MessageBox.Show("Number of used UserFrames exceeds 909 - .XVR has to be corrected manualy");
            }
            foreach (SafeZoneFanucMultiPoints zoneMultiPoints in safeZones.SafeZonesMultiPoints)
            {
                int mode;
                if (zoneMultiPoints.IsLI)
                    mode = 2;
                else
                    mode = 3;

                currentString = currentString + String.Join(
                    Environment.NewLine,
                    "   <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "]\">",
                    "    <FIELD name=\"$COMMENT\" prot=\"RW\">" + zoneMultiPoints.Name + "</FIELD>",
                    "    <FIELD name=\"$ENABLE\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$MODE\" prot=\"RW\">" + mode.ToString() + "</FIELD>",
                    "    <FIELD name=\"$GRP_NUM\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$MODEL_NUM\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$MODEL_NUM[1]\" prot=\"RW\">-2</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$MODEL_NUM[2]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$MODEL_NUM[3]\" prot=\"RW\">0</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$NUM_VTX\" prot=\"RW\">" + zoneMultiPoints.Points.Count.ToString() + "</FIELD>",
                    "    <FIELD name=\"$X\">\n");
                foreach (Point point in zoneMultiPoints.Points)
                {
                    currentString = currentString + "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$X[" + pointCounter.ToString() + "]\" prot=\"RW\">" + point.Xpos.ToString() + "</ARRAY>\n";
                    pointCounter++;
                }
                pointCounter = 1;

                currentString = currentString + String.Join(
                    Environment.NewLine,
                    "    </FIELD>",
                    "    <FIELD name=\"$Y\">\n");

                foreach (Point point in zoneMultiPoints.Points)
                {
                    currentString = currentString + "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$Y[" + pointCounter.ToString() + "]\" prot=\"RW\">" + point.Ypos.ToString() + "</ARRAY>\n";
                    pointCounter++;
                }
                pointCounter = 1;

                currentString = currentString + String.Join(
                Environment.NewLine,
                    "    </FIELD>",
                    "    <FIELD name=\"$Z1\" prot=\"RW\">" + zoneMultiPoints.Bottom.ToString() + "</FIELD>",
                    "    <FIELD name=\"$Z2\" prot=\"RW\">" + zoneMultiPoints.Top.ToString() + "</FIELD>",
                    "    <FIELD name=\"$UFRM_NUM\" prot=\"RW\">0</FIELD>",
                    "    <FIELD name=\"$STOP_TYP\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$DSBIO_TY\" prot=\"RW\">10</FIELD>",
                    "    <FIELD name=\"$DSBIO_IDX\" prot=\"RW\">33</FIELD>",
                    "    <FIELD name=\"$ENBL_CALMD\" prot=\"RW\">0</FIELD>",
                    "    <FIELD name=\"$USE_PREDICT\" prot=\"RW\">1</FIELD>",
                    "    <FIELD name=\"$DELAY_TIME\" prot=\"RW\">0</FIELD>",
                    "    <FIELD name=\"$SPEED_CTRL\" prot=\"RW\">0</FIELD>",
                    "    <FIELD name=\"$OVR_LIMT\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$OVR_LIMT[1]\" prot=\"RW\">100</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$OVR_LIMT[2]\" prot=\"RW\">100</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$OVR_LIMT[3]\" prot=\"RW\">100</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$OVR_LIMT[4]\" prot=\"RW\">100</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$SPD_LIMT\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$SPD_LIMT[1]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$SPD_LIMT[2]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$SPD_LIMT[3]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$SPD_LIMT[4]\" prot=\"RW\">2.500000000e+02</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$DSBL_TYP\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_TYP[1]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_TYP[2]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_TYP[3]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_TYP[4]\" prot=\"RW\">0</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$DSBL_IDX\">",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_IDX[1]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_IDX[2]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_IDX[3]\" prot=\"RW\">0</ARRAY>",
                    "     <ARRAY name=\"$DCSS_CPC[" + zoneMultiPoints.Number.ToString() + "].$DSBL_IDX[4]\" prot=\"RW\">0</ARRAY>",
                    "    </FIELD>",
                    "    <FIELD name=\"$UTOOL_NUM\" prot=\"RW\">0</FIELD>",
                    "   </ARRAY>\n"
                    );
            }
            zoneCounter = 901;
            currentString = currentString + "  </VAR>\n";
            resultString = resultString + currentString;
        }

        private static void CreateElbowABBXML(ElbowABB elbow, string robotName, string selectedRobot)
        {
            string tempstring = "";
            double xpos = 0, ypos = 0, zpos = 0;
            foreach (Robot robot in RobotList.RobotsElbow.Where(item => item.RobotName == selectedRobot))
            {
                xpos = robot.Xpos / 1000;
                ypos = robot.Ypos / 1000;
                zpos = robot.Zpos / 1000;
                break;
            }
            resultString = resultString + "\t\t<Robot name=\"Rob_1\" startSpeedOffset=\"0.1\">\n\t\t\t<ElbowOffset x=\"" + xpos.ToString() + "\" y=\"" + ypos.ToString() + "\" z=\"" + zpos.ToString() + "\" />\n";
            if (elbow.Capsules != null)
                foreach (CapsuleABB capsule in elbow.Capsules)
                {
                    {
                        tempstring = "";
                        tempstring = String.Join(
                            Environment.NewLine,
                                "\t\t\t\t<UpperArmGeometry xs:type=\"Capsule\" name=\"Capsule" + capsule.Number.ToString() + "\" radius=\"" + capsule.Radius.ToString() + "\" >",
                                "\t\t\t\t\t<Start x=\"" + capsule.Point1.Xpos.ToString() + "\" y=\"" + capsule.Point1.Ypos.ToString() + " \" z=\"" + capsule.Point1.Zpos.ToString() + "\" />",
                                "\t\t\t\t\t<End x=\"" + capsule.Point2.Xpos.ToString() + "\" y = \"" + capsule.Point2.Ypos.ToString() + "\" z = \"" + capsule.Point2.Zpos.ToString() + "\" />",
                                "\t\t\t\t</UpperArmGeometry>",
                            "");
                        resultString = resultString + tempstring;
                    }
                }
            if (elbow.Lozenges != null)
            {
                foreach (LozengeABB lozenge in elbow.Lozenges)
                {
                    tempstring = "";
                    tempstring = String.Join(
                        Environment.NewLine,
                            "\t\t\t\t<UpperArmGeometry xs:type=\"Lozenge\" name=\"Lozenge" + lozenge.Number.ToString() + "\" radius=\"" + lozenge.Radius.ToString() + "\" width=\"" + lozenge.Width.ToString() + "\" height=\"" + lozenge.Height.ToString() + "\" >",
                            "\t\t\t\t\t<Pose>",
                            "\t\t\t\t\t\t<Translation x=\"" + lozenge.CenterPoint.Xpos.ToString() + "\" y=\"" + lozenge.CenterPoint.Ypos.ToString() + " \" z=\"" + lozenge.CenterPoint.Zpos.ToString() + "\" />",
                            "\t\t\t\t\t\t<Quaternion q1=\"" + lozenge.CenterPoint.Quat1.ToString() + "\" q2=\"" + lozenge.CenterPoint.Quat2.ToString() + " \" q3=\"" + lozenge.CenterPoint.Quat3.ToString() + "\" q4=\"" + lozenge.CenterPoint.Quat4.ToString() + "\" />",
                            "\t\t\t\t\t</Pose>",
                            "\t\t\t\t</UpperArmGeometry>",
                        "");
                    resultString = resultString + tempstring;
                }
            }
            if (elbow.Lozenges != null)
            {
                foreach (SphereABB sphere in elbow.Spheres)
                {
                    tempstring = "";
                    tempstring = String.Join(
                        Environment.NewLine,
                            "\t\t\t\t<UpperArmGeometry xs:type=\"Sphere\" name=\"Sphere" + sphere.Number.ToString() + "\" radius=\"" + sphere.Radius.ToString() + "\" >",
                            "\t\t\t\t\t<Center x=\"" + sphere.CenterPoint.Xpos.ToString() + "\" y=\"" + sphere.CenterPoint.Ypos.ToString() + " \" z=\"" + sphere.CenterPoint.Zpos.ToString() + "\" />",
                            "\t\t\t\t</UpperArmGeometry>",
                        "");
                    resultString = resultString + tempstring;
                }
            }
            resultString = resultString + "\t\t</Robot>\n";
        }

        public static SafetyConfigABB ReadXmlFile(string file, bool isSafetyRobotXML = false)
        {
            string robotName = "";
            if (!IsABBRobot(file))
            {
                MessageBox.Show("Selected SafetyRobot.xml does not contain configuration for ABB robot");
                return null;
            }
            List<SafeToolABB> safeTools = GetSafeToolsFromXML(file,isSafetyRobotXML);
            List<SafeZoneABB> safeZones = GetSafeZonesFromXML(file);
            ElbowABB elbow = GetElbowABBFromXML(file, isSafetyRobotXML);

            SafetyConfigABB config = new SafetyConfigABB(safeTools, safeZones, elbow, robotName);
            return config;
        }

        private static ElbowABB GetElbowABBFromXML(string file,bool isSafetyRobotXML)
        {
            string elbowString = GetToolFromXML(file, false);
            ElbowABB result = GetElbowABBFromString(elbowString, isSafetyRobotXML);
            return result;                
        }

        private static ElbowABB GetElbowABBFromString(string elbowString,bool isSafetyRobotXML)
        {
            SafeToolABB tool = GetToolFromToolString(elbowString, isSafetyRobotXML);
            ElbowABB result = new ElbowABB(new Point(), tool.Capsules, tool.Spheres, tool.Lozenges);
            return result;
        }

        private static bool IsABBRobot(string file)
        {
            {
                bool isValid = false, isFanuc = false;
                var reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("<WestSafetyRobot Version") || line.ToLower().Contains("simplifiedsafetyconfiguration"))
                        isValid = true;
                    if (line.Contains("RobotType=\"1\""))
                        isFanuc = true;
                    if (isValid & isFanuc)
                        break;
                }
                return isFanuc & isValid;
            }
        }

        private static List<SafeZoneABB> GetSafeZonesFromXML(string file)
        {
            SafeZoneABB currentZone = new SafeZoneABB();
            List<string> zoneStrings = GetZoneFromXML(file);
            List<SafeZoneABB> zones = new List<SafeZoneABB>();
            foreach (string zoneString in zoneStrings)
            {
                currentZone = new SafeZoneABB();
                currentZone = GetZoneFromString(zoneString);
                zones.Add(currentZone);
            }

            return zones;

        }

        private static SafeZoneABB GetZoneFromString(string zoneString)
        {
            int zoneNum = 0;
            float zoneTop = 0, zoneBottom = 0;
            Regex currentRegex;
            Match match;
            List<string> foundValues = new List<string>();
            List<Point> zonePoints = new List<Point>();
            Point currentPoint = new Point();
            StringReader reader = new StringReader(zoneString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    if (line.Contains("<ZoneNumber>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        zoneNum = int.Parse(match.ToString());
                    }

                    if (line.Contains("<ZoneHightTopValue>"))
                    {
                        currentRegex = new Regex(@"(?<=>)(-\d*|\d*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line.Replace(" ", ""));
                        zoneTop = float.Parse(match.ToString());
                    }
                    if (line.Contains("<ZoneHightBottomValue>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=\>)[0-9]*\.[0-9]*)|((?<=\>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        zoneBottom = float.Parse(match.ToString());
                    }
                    if (line.Contains("<ZonePointXY>"))
                    {
                        //currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=_)[0-9]*)|((?<=_)[0-9]*\.[0-9]*)|(-[0-9]*)|(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);                      
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            foundValues.Add(currentMatch.ToString());
                        }
                        currentPoint = new Point((float.Parse(foundValues[0].ToString())) / 1000, (float.Parse(foundValues[1].ToString())) / 1000, zoneBottom / 1000, 1, 0, 0, 0);
                        zonePoints.Add(currentPoint);
                        foundValues = new List<string>();
                    }
                    if (line.Contains("</SafetyGeometryZoneABB>"))
                    {
                        break;
                    }
                }
            }
            SafeZoneABB resultZone = new SafeZoneABB(zoneNum, "Zone_" + zoneNum.ToString(), zonePoints, zoneTop / 1000 + Math.Abs(zoneBottom / 1000));
            return resultZone;
        }

        private static List<string> GetZoneFromXML(string file)
        {
            List<string> resultStrings = new List<string>();
            bool addLine = false;
            string resultString = "";
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("</SafetyRobotData>"))
                {
                    break;
                }
                if (line.Contains("<SafetyObjectZoneABB Id"))
                {
                    addLine = true;
                }
                if (addLine == true)
                {
                    resultString = resultString + line + "\n";
                }
                if (line.Contains("</SafetyObjectZoneABB>"))
                {
                    addLine = false;
                    resultStrings.Add(resultString);
                    resultString = "";
                }
            }
            reader.Close();
            return resultStrings;

        }

        private static List<SafeToolABB> GetSafeToolsFromXML(string file, bool isSafetyRobotXML)
        {
            string toolString = GetToolFromXML(file);
            SafeToolABB tool = GetToolFromToolString(toolString, isSafetyRobotXML);
            List<SafeToolABB> safeTools = new List<SafeToolABB>();
            safeTools.Add(tool);
            return safeTools;
        }

        private static SafeToolABB GetToolFromToolString(string toolString, bool isSafetyRobotXML)
        {
            int capsuleCounter = 0, sphereCounter = 0, lozengeCounter = 0;
            bool getLine = false;
            CapsuleABB currentCapsule = new CapsuleABB();
            SphereABB currentSphere = new SphereABB();
            LozengeABB currentLozenge = new LozengeABB();
            List<CapsuleABB> capsules = new List<CapsuleABB>();
            List<SphereABB> spheres = new List<SphereABB>();
            List<LozengeABB> lozenges = new List<LozengeABB>();
            string tempstring = "";
            SafeToolABB tool = new SafeToolABB();
            StringReader reader = new StringReader(toolString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    if (line.Contains("<SafetyGeometryCapsuleABB Id") | line.Contains("<SafetyGeometrySphereABB Id") | line.Contains("<SafetyGeometryLozengeABB Id"))
                    {
                        getLine = true;
                    }
                    if (getLine)
                    {
                        tempstring = tempstring + line + "\n";
                    }
                    if (line.Contains("</SafetyGeometryCapsuleABB>"))
                    {
                        capsuleCounter++;
                        getLine = false;
                        currentCapsule = new CapsuleABB();
                        currentCapsule = GetCapsuleFromToolString(tempstring, capsuleCounter, isSafetyRobotXML);
                        capsules.Add(currentCapsule);
                        tempstring = "";
                    }
                    if (line.Contains("</SafetyGeometrySphereABB>"))
                    {
                        sphereCounter++;
                        getLine = false;
                        currentSphere = new SphereABB();
                        currentSphere = GetSphereFromToolString(tempstring, sphereCounter, isSafetyRobotXML);
                        spheres.Add(currentSphere);
                        tempstring = "";
                    }
                    if (line.Contains("</SafetyGeometryLozengeABB>"))
                    {
                        lozengeCounter++;
                        getLine = false;
                        currentLozenge = new LozengeABB();
                        currentLozenge = GetLozengeFromToolString(tempstring, lozengeCounter, isSafetyRobotXML);
                        lozenges.Add(currentLozenge);
                        tempstring = "";
                    }
                    if (line.Contains("</SafetyObjectToolABB>") || line.Contains("</SafetyObjectUpperArmABB>"))
                    {
                        break;
                    }


                }
            }
            tool.Number = 1;
            tool.TCP = new Point(0, 0, 0, 1, 0, 0, 0);
            tool.Capsules = capsules;
            tool.Lozenges = lozenges;
            tool.Spheres = spheres;
            return tool;
        }

        private static List<double> RadToQuat(double heading, double attitude, double bank)
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

        private static float RadToDeg(float rad)
        {
            float result = (rad * 180) / (float)(Math.PI);
            return result;
        }

        private static LozengeABB GetLozengeFromToolString(string lozengeString, int lozengeCounter, bool isSafetyRobotXML)
        {
            LozengeABB currentLozenge = new LozengeABB();
            Regex currentRegex;
            Match match;
            List<string> stringsCenter = new List<string>();
            StringReader reader = new StringReader(lozengeString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    if (line.Contains("<Radius>"))
                    {
                        currentRegex = new Regex(@"(?<=<Radius>)[0-9]*\.[0-9]*", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        if (!isSafetyRobotXML)
                            currentLozenge.Radius = (float.Parse(match.ToString())) / 1000;
                        else
                        {
                            double temp = double.Parse(match.ToString());
                            temp = Math.Round(temp);
                            currentLozenge.Radius = float.Parse(temp.ToString());
                        }
                    }
                    if (line.Contains("<Width>"))
                    {
                        currentRegex = new Regex(@"(?<=<Width>)[0-9]*\.[0-9]*", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        if (!isSafetyRobotXML)
                            currentLozenge.Width = (float.Parse(match.ToString())) / 1000;
                        else
                        {
                            double temp = double.Parse(match.ToString());
                            temp = Math.Round(temp);
                            currentLozenge.Width = float.Parse(temp.ToString());
                        }
                    }
                    if (line.Contains("<Height>"))
                    {
                        currentRegex = new Regex(@"(?<=<Height>)[0-9]*\.[0-9]*", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        if (!isSafetyRobotXML)
                            currentLozenge.Height = (float.Parse(match.ToString())) / 1000;
                        else
                        {
                            double temp = double.Parse(match.ToString());
                            temp = Math.Round(temp);
                            currentLozenge.Height = float.Parse(temp.ToString());
                        }
                    }
                    if (line.Contains("<Position>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=_)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        //!currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=_)[0-9]*\.[0-9]*)|(-[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            if (currentMatch.ToString() != "")
                                stringsCenter.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("</SafetyGeometryLozengeABB>"))
                    {
                        break;
                    }
                }
            }
            currentLozenge.Number = lozengeCounter;
            List<double> quaternions = RadToQuat(double.Parse(stringsCenter[3]), double.Parse(stringsCenter[4]), double.Parse(stringsCenter[5]));
            Point center = new Point((float.Parse(stringsCenter[0])) / 1000, (float.Parse(stringsCenter[1])) / 1000, (float.Parse(stringsCenter[2])) / 1000, Convert.ToSingle(quaternions[0]), Convert.ToSingle(quaternions[1]), Convert.ToSingle(quaternions[2]), Convert.ToSingle(quaternions[3]));
            currentLozenge.CenterPoint = center;
            return currentLozenge;
        }

        private static SphereABB GetSphereFromToolString(string sphereString, int sphereCounter, bool isSafetyRobotXML)
        {
            SphereABB currentSphere = new SphereABB();
            Regex currentRegex;
            Match match;
            List<string> stringsCenter = new List<string>();
            StringReader reader = new StringReader(sphereString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    if (line.Contains("<Radius>"))
                    {
                        currentRegex = new Regex(@"(?<=<Radius>)[0-9]*\.[0-9]*", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        if (!isSafetyRobotXML)
                            currentSphere.Radius = (float.Parse(match.ToString())) / 1000;
                        else
                        {
                            double temp = double.Parse(match.ToString());
                            temp = Math.Round(temp);
                            currentSphere.Radius = float.Parse(temp.ToString());
                        }
                    }
                    if (line.Contains("<Center>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        //!currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=_)[0-9]*\.[0-9]*)|(-[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            stringsCenter.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("</SafetyGeometrySphereABB>"))
                    {
                        break;
                    }
                }
            }
            currentSphere.Number = sphereCounter;
            Point center = new Point((float.Parse(stringsCenter[0])) / 1000, (float.Parse(stringsCenter[1])) / 1000, (float.Parse(stringsCenter[2])) / 1000, 1, 0, 0, 0);
            currentSphere.CenterPoint = center;
            return currentSphere;
        }

        private static CapsuleABB GetCapsuleFromToolString(string capsuleString, int capsuleCounter, bool isSafetyRobotXML)
        {
            CapsuleABB currentCapsule = new CapsuleABB();
            Regex currentRegex;
            Match match;
            List<string> stringsStart = new List<string>();
            List<string> stringsEnd = new List<string>();
            StringReader reader = new StringReader(capsuleString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    if (line.Contains("<Radius>"))
                    {
                        currentRegex = new Regex(@"(?<=<Radius>)[0-9]*\.[0-9]*", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        if (!isSafetyRobotXML)
                            currentCapsule.Radius = (float.Parse(match.ToString())) / 1000;
                        else
                        {
                            double temp = double.Parse(match.ToString());
                            temp = Math.Round(temp);
                            currentCapsule.Radius = float.Parse(temp.ToString());
                            
                        }
                    }
                    if (line.Contains("<SphereStart>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        //!currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=_)[0-9]*\.[0-9]*)|(-[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            stringsStart.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("<SphereEnd>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        //currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=_)[0-9]*\.[0-9]*)|(-[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            stringsEnd.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("</SafetyGeometryCapsuleABB>"))
                    {
                        break;
                    }
                }
            }
            currentCapsule.Number = capsuleCounter;
            Point startPoint = new Point((float.Parse(stringsStart[0])) / 1000, (float.Parse(stringsStart[1])) / 1000, (float.Parse(stringsStart[2])) / 1000, 1, 0, 0, 0);
            Point endPoint = new Point((float.Parse(stringsEnd[0])) / 1000, (float.Parse(stringsEnd[1])) / 1000, (float.Parse(stringsEnd[2])) / 1000, 1, 0, 0, 0);
            currentCapsule.Point1 = startPoint;
            currentCapsule.Point2 = endPoint;
            return currentCapsule;

        }

        private static string GetToolFromXML(string file, bool isTool = true)
        {
            bool addLine = false;
            string resultString = "", startstring = "", endstring = "";
            if (isTool)
            {
                startstring = "<SafetyObjectToolABB Id";
                endstring = "</SafetyObjectToolABB>";
            }
            else
            {
                startstring = "<SafetyObjectUpperArmABB Id";
                endstring = "</SafetyObjectUpperArmABB>";
            }
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains(startstring))
                {
                    addLine = true;
                }
                if (addLine == true)
                {
                    resultString = resultString + line + "\n";
                }
                if (line.Contains(endstring))
                {
                    break;
                }

            }
            reader.Close();
            return resultString;
        }

        private static void CreateZoneABBXML(SafeZoneABB zone)
        {
            string tempString = "";
            var stringSafeZone = String.Join(
                Environment.NewLine,
                "\t\t<SafeZone id=\"" + zone.SafeZoneNumber.ToString() + "\" name=\"" + zone.Name + "\" top=\"" + (zone.Height + zone.SafeZonePoints[0].Zpos).ToString() + "\" bottom=\"" + zone.SafeZonePoints[0].Zpos + "\" speedLimitPriority =\"NORMAL\">",
                "");
            resultString = resultString + stringSafeZone;

            foreach (Point point in zone.SafeZonePoints)
            {
                tempString = "\t\t\t<Point x=\"" + point.Xpos.ToString() + "\" y=\"" + point.Ypos.ToString() + "\"/>\n";
                resultString = resultString + tempString;
            }
            resultString = resultString + "\t\t</SafeZone>\n";
        }

        private static void CreateToolABBXml(SafeToolABB tool, bool isPermanent, int loopCounter)
        {
            string activationString = "";
            if (isPermanent)
                activationString = "Permanent";
            else
                if (loopCounter == 0)
                activationString = "AutomaticMode";
            else
                activationString = "SafetyEnable";
            var stringTCP = String.Join(
                Environment.NewLine,
                "\t\t<Tool id=\"" + tool.Number.ToString() + "\" name=\"Tool_0" + tool.Number.ToString() + "\" activationSignal=\"" + activationString + "\">",
                "\t\t\t<TCP x =\"" + tool.TCP.Xpos.ToString() + "\" y=\"" + tool.TCP.Ypos.ToString() + "\" z=\"" + tool.TCP.Zpos.ToString() + "\" />",
                "\t\t\t<ToolOrientation q1=\"" + tool.TCP.Quat1.ToString() + "\" q2=\"" + tool.TCP.Quat2.ToString() + "\" q3=\"" + tool.TCP.Quat3.ToString() + "\" q4=\"" + tool.TCP.Quat4.ToString() + "\" />",
                "");
            resultString = resultString + stringTCP;

            var stringSphere = "";
            if (tool.Spheres != null)
            {
                foreach (SphereABB sphere in tool.Spheres)
                {
                    stringSphere = "";
                    stringSphere = String.Join(
                        Environment.NewLine,
                        "\t\t\t<ToolGeometry xs:type = \"Sphere\" name = \"Sphere" + sphere.Number.ToString() + "\" radius = \"" + sphere.Radius.ToString() + "\">",
                        "\t\t\t\t<Center x = \"" + sphere.CenterPoint.Xpos.ToString() + "\" y = \"" + sphere.CenterPoint.Ypos.ToString() + "\" z = \"" + sphere.CenterPoint.Zpos.ToString() + "\" />",
                        "\t\t\t</ToolGeometry>",
                        "");
                    resultString = resultString + stringSphere;
                }
            }
            var stringCapsule = "";
            if (tool.Capsules != null)
            {
                foreach (CapsuleABB capsule in tool.Capsules)
                {
                    stringCapsule = "";
                    stringCapsule = String.Join(
                        Environment.NewLine,
                        "\t\t\t<ToolGeometry xs:type=\"Capsule\" name=\"Capsule" + capsule.Number.ToString() + "\" radius=\"" + capsule.Radius.ToString() + "\">",
                        "\t\t\t\t<Start x=\"" + capsule.Point1.Xpos.ToString() + "\" y=\"" + capsule.Point1.Ypos.ToString() + "\" z=\"" + capsule.Point1.Zpos.ToString() + "\" />",
                        "\t\t\t\t<End x=\"" + capsule.Point2.Xpos.ToString() + "\" y=\"" + capsule.Point2.Ypos.ToString() + "\" z=\"" + capsule.Point2.Zpos.ToString() + "\" />",
                        "\t\t\t</ToolGeometry>",
                        "");
                    resultString = resultString + stringCapsule;
                }
            }
            var stringLozenge = "";
            if (tool.Lozenges != null)
            {
                foreach (LozengeABB lozenge in tool.Lozenges)
                {
                    stringLozenge = "";
                    stringLozenge = String.Join(
                        Environment.NewLine,
                        "\t\t\t<ToolGeometry xs:type=\"Lozenge\" name=\"SSV" + lozenge.Number.ToString() + "\" radius=\"" + lozenge.Radius.ToString() + "\" width=\"" + lozenge.Width.ToString() + "\" height=\"" + lozenge.Height.ToString() + "\" >",
                        "\t\t\t\t<Pose>",
                        "\t\t\t\t\t<Translation x=\"" + lozenge.CenterPoint.Xpos.ToString() + "\" y=\"" + lozenge.CenterPoint.Ypos.ToString() + "\" z=\"" + lozenge.CenterPoint.Zpos.ToString() + "\" />",
                        "\t\t\t\t\t<Quaternion q1=\"" + lozenge.CenterPoint.Quat1.ToString() + "\" q2=\"" + lozenge.CenterPoint.Quat2.ToString() + "\" q3=\"" + lozenge.CenterPoint.Quat3.ToString() + "\" q4=\"" + lozenge.CenterPoint.Quat4.ToString() + "\" />",
                        "\t\t\t\t</Pose>",
                        "\t\t\t</ToolGeometry>",
                        "");
                    resultString = resultString + stringLozenge;
                }
            }
            resultString = resultString + "\t\t</Tool>\n";

        }

        #endregion

        #region Readind mod file
        public static SafetyConfigABB ReadModFile(string file)
        {
            List<SafeToolABB> safeTools = new List<SafeToolABB>();
            List<SafeZoneABB> safeZones = new List<SafeZoneABB>();
            ElbowABB elbow = new ElbowABB();
            SafeToolABB currentTool = new SafeToolABB();
            SafeZoneABB currentZone = new SafeZoneABB();
            elbow = FindElbowABB(file);
            ToolAndZoneNumbers toolAndZoneNumbers = GetToolsAndZoneNumbers(file);
            foreach (int toolNumber in toolAndZoneNumbers.SafeToolNumbers)
            {
                currentTool = FindTool(toolNumber, file);
                safeTools.Add(currentTool);
            }
            foreach (int zoneNumber in toolAndZoneNumbers.SafeZones)
            {
                currentZone = FindZone(zoneNumber, file);
                safeZones.Add(currentZone);
            }
            string robotName = GetRobotName(file);
            SafetyConfigABB config = new SafetyConfigABB(safeTools, safeZones, elbow, robotName);
            return config;
        }

        private static string GetRobotName(string file)
        {
            string result = "";
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("!# ROBOT"))
                {
                    Regex regName = new Regex(@"[0-9]*IR[0-9]*", RegexOptions.IgnoreCase);
                    Match mResult = regName.Match(line);
                    result = mResult.ToString();
                    break;
                }
            }
            reader.Close();
            if (string.IsNullOrEmpty(result))
                result = Path.GetFileNameWithoutExtension(file);
            return result;
        }

        private static SafeZoneABB FindZone(int zoneNumber, string file)
        {
            Regex isZoneRegex = new Regex("(sp"+zoneNumber.ToString()+"|wc_|"+zoneNumber.ToString()+"_Point_\\d+)", RegexOptions.IgnoreCase);
            Regex isHeight = new Regex(@"(?<=PROC.*)\d+(?=_Height\d+)", RegexOptions.IgnoreCase); 
            bool isWorkCell = false;
            //string searchString;
            //if (zoneNumber != 16)
            //    searchString = "sp" + zoneNumber.ToString() + "_";
            //else
            //    searchString = "wc_";
            string name = "";
            var reader = new StreamReader(file);
            float zoneHeight = 0;
            List<Point> points = new List<Point>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().ToLower();
                if (line.Contains("const robtarget"))
                {
                    if (isZoneRegex.IsMatch(line) & !line.Contains("ts"))
                    {
                        if (!(line.ToLower().Contains("wc_p") && zoneNumber != 16))
                        {
                            Point point = GetPoint(line);
                            int pointNumber = GetPointNumber(line);
                            if (pointNumber == 1)
                            {
                                name = GetZoneName(line);
                                zoneHeight = GetZoneHeight(line);
                            }
                            points.Add(point);
                        }
                    }               
                }
                if (zoneHeight == 999999 && isHeight.IsMatch(line))
                {
                    Regex getHeightRegex = new Regex(@"(?<=Height)\d+", RegexOptions.IgnoreCase);
                    zoneHeight = int.Parse(getHeightRegex.Match(line).ToString())/1000;
                }
            }
            if (zoneNumber == 16)
                isWorkCell = true;
            SafeZoneABB result = new SafeZoneABB(zoneNumber, name, points, zoneHeight, isWorkCell);
            reader.Close();
            return result;
        }

        private static string GetZoneName(string line)
        {
            string result = "";
            if (line.Contains("wc_"))
            {
                result = "WorkCell";
            }
            else
            {
                Regex getName = new Regex(@"(?<=_p1_)(.*?)(?=_)", RegexOptions.IgnoreCase);
                Match match = getName.Match(line);
                result = match.ToString();
            }
            return result;
        }

        private static SafeToolABB FindTool(int toolNumber, string file)
        {
            Regex isToolRegex = new Regex("\\s+(ts|tool)"+toolNumber.ToString(), RegexOptions.IgnoreCase);
            // string searchString = "ts" + toolNumber.ToString() + "_";
            string name = "";
            var reader = new StreamReader(file);
            float capsuleRadius = 0;
            int capsuleNumber = 0;
            Point foundPoint, cap1stPos = null, cap2ndPos = null;
            List<LozengeABB> lozenges = new List<LozengeABB>();
            List<SphereABB> spheres = new List<SphereABB>();
            List<CapsuleABB> capsules = new List<CapsuleABB>();
            Point tcp = new Point(0, 0, 0, 1, 0, 0, 0);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().ToLower();
                if (line.Contains("const robtarget"))
                {
                    if (isToolRegex.IsMatch(line))
                    {
                        name = GetToolName(line);
                        if (line.Contains("lozenge"))
                        {
                            foundPoint = GetPoint(line);
                            LozengeABB lozenge = CreateLozenge(line, foundPoint);
                            lozenges.Add(lozenge);
                        }
                        if (line.Contains("sphere"))
                        {
                            foundPoint = GetPoint(line);
                            SphereABB sphere = CreateSphere(line, foundPoint);
                            spheres.Add(sphere);
                        }
                        if (line.Contains("capsule"))
                        {
                            if (line.Contains("_p1"))
                            {
                                capsuleRadius = GetCapsuleRadius(line);
                                capsuleNumber = GetCapsuleNumber(line);
                                cap1stPos = GetPoint(line);
                            }
                            if (line.Contains("_p2"))
                            {
                                cap2ndPos = GetPoint(line);
                            }
                            if (cap1stPos != null & cap2ndPos != null)
                            {
                                CapsuleABB capsule = CreateCapsule(cap1stPos, cap2ndPos, capsuleRadius, capsuleNumber);
                                capsules.Add(capsule);
                                cap1stPos = null;
                                cap2ndPos = null;
                            }
                        }
                        if (line.Contains("_tcp"))
                            tcp = GetPoint(line);
                    }
                }
            }
            SafeToolABB result = new SafeToolABB(toolNumber, name, tcp, spheres, lozenges, capsules);
            reader.Close();
            return result;

        }

        private static string GetToolName(string line)
        {
            Regex regName = new Regex(@"(?<=_)(.*?)(?=_)", RegexOptions.IgnoreCase);
            Match match = regName.Match(line);
            //regName = new Regex("^(.*)_");
            //match1 = regName.Match(match1.ToString());
            return match.ToString();
        }

        private static ToolAndZoneNumbers GetToolsAndZoneNumbers(string file)
        {
            Regex isPointInZone = new Regex(@"\d+_point_\d+", RegexOptions.IgnoreCase);
            Regex isTool = new Regex(@"tool\d+_",RegexOptions.IgnoreCase);
            Regex zoneNumRegex = new Regex(@"\d+(?=_Point_\d+)", RegexOptions.IgnoreCase);
            List<int> toolNumbers = new List<int>();
            List<int> zoneNumbers = new List<int>();
            var reader = new StreamReader(file);
            bool addNewTool = true;
            bool addNewZone = true;
            bool addNewWorkCell = true;
            int toolNumber = 0;
            int zoneNumber = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("ts") || isTool.IsMatch(line))
                {
                    toolNumber = ReadToolOrZoneNumber(line, true);
                    addNewTool = true;
                    foreach (int toolNr in toolNumbers.Where(item => item == toolNumber))
                        addNewTool = false;
                    if (addNewTool)
                        toolNumbers.Add(toolNumber);
                }

                if (((line.Contains("sp") & !line.Contains("sphere")) | line.Contains("wc_")) & !line.Contains("ts") || isPointInZone.IsMatch(line))
                {
                    if (line.Contains("wc_") & addNewWorkCell)
                    {
                        zoneNumbers.Add(16);
                        addNewWorkCell = false;
                    }
                    else
                    {
                        if (!line.Contains("wc_"))
                        {
                            if (isPointInZone.IsMatch(line))
                                zoneNumber = int.Parse(zoneNumRegex.Match(line).ToString());
                            else
                                zoneNumber = ReadToolOrZoneNumber(line, false);
                            addNewZone = true;
                            foreach (int zoneNr in zoneNumbers.Where(item => item == zoneNumber))
                                addNewZone = false;
                            if (addNewZone)
                                zoneNumbers.Add(zoneNumber);
                        }

                    }
                }
            }
            ToolAndZoneNumbers result = new ToolAndZoneNumbers(toolNumbers, zoneNumbers);
            reader.Close();
            return result;
        }

        private static ElbowABB FindElbowABB(string file)
        {
            Point cap1stPos = null, cap2ndPos = null;
            List<CapsuleABB> capsules = new List<CapsuleABB>();
            List<LozengeABB> lozenges = new List<LozengeABB>();
            List<SphereABB> spheres = new List<SphereABB>();
            float capsuleRadius = 0;
            int capsuleNumber = 0;
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().ToLower();
                if (line.Contains("const robtarget"))
                {
                    if (line.Contains(" el") & line.Contains("capsule"))
                    {
                        if (line.Contains("_p1"))
                        {
                            capsuleRadius = GetCapsuleRadius(line);
                            capsuleNumber = GetCapsuleNumber(line);
                            cap1stPos = GetPoint(line);
                        }
                        if (line.Contains("_p2"))
                        {
                            cap2ndPos = GetPoint(line);
                        }
                        if (cap1stPos != null & cap2ndPos != null)
                        {
                            CapsuleABB capsule = CreateCapsule(cap1stPos, cap2ndPos, capsuleRadius, capsuleNumber);
                            capsules.Add(capsule);
                            cap1stPos = null;
                            cap2ndPos = null;
                        }
                    }
                    if (line.Contains("lozenge") & line.Contains(" el"))
                    {
                        LozengeABB currentLozenge = CreateLozenge(line, GetPoint(line));
                        lozenges.Add(currentLozenge);
                    }
                    if (line.Contains("sphere") & line.Contains(" el"))
                    {
                        SphereABB currentSphere = CreateSphere(line, GetPoint(line));
                        spheres.Add(currentSphere);
                    }
                }
            }
            ElbowABB elbow = new ElbowABB(null, capsules, spheres, lozenges);
            reader.Close();
            return elbow;
        }

        private static int ReadToolOrZoneNumber(string line, bool isToolNr)
        {
            Regex regNr = new Regex(@"(?<=(ts|tool))\d+", RegexOptions.IgnoreCase);
            if (!isToolNr)
                regNr = new Regex(@"(?<=sp)\d+", RegexOptions.IgnoreCase);
            Match mNumber = regNr.Match(line);
            if (mNumber.ToString() == "")
            {
                MessageBox.Show("Invalid tool or zone number\nCorrect input file and try again");
                return 0;
            }
            int number = int.Parse(mNumber.ToString(), CultureInfo.InvariantCulture);
            return number;
        }

        private static int GetCapsuleNumber(string line)
        {
            Regex regNr = new Regex(@"capsule[0-9]*", RegexOptions.IgnoreCase);
            Match mNumber = regNr.Match(line);
            if (mNumber.ToString() == "")
            {
                MessageBox.Show("Invalid capsule number\nCorrect input file and try again");
                return 0;
            }
            int number = int.Parse(mNumber.ToString().Remove(0, 7), CultureInfo.InvariantCulture);
            return number;
        }

        private static float GetCapsuleRadius(string line)
        {
            Regex regRadius = new Regex(@"_r[0-9]*", RegexOptions.IgnoreCase);
            Match mRadius = regRadius.Match(line);
            if (mRadius.ToString() == "")
            {
                MessageBox.Show("Invalid capsule radius\nCorrect input file and try again");
                return 0;
            }
            float radius = float.Parse(mRadius.ToString().Remove(0, 2), CultureInfo.InvariantCulture);
            return radius;
        }

        private static float GetZoneHeight(string line)
        {
            float resultZoneHeight;
            Regex getZoneNumber = new Regex(@"_h[0-9]+", RegexOptions.IgnoreCase);
            Match match = getZoneNumber.Match(line);
            string zoneString = match.ToString();
            if (zoneString == "")
            {
                //MessageBox.Show("Invalid zone height\nCorrect input file and try again");
                return 999999;
            }

            zoneString = zoneString.Remove(0, 2);
            resultZoneHeight = float.Parse(zoneString);
            return resultZoneHeight / 1000;
        }

        private static int GetPointNumber(string line)
        {
            int resultPointNumber;
            Regex getZoneNumber = new Regex(@"(?<=(_p|_point_))\d+", RegexOptions.IgnoreCase);
            Match match = getZoneNumber.Match(line);
            if (match.ToString() == "")
            {
                MessageBox.Show("Invalid point number\nCorrect input file and try again");
                return 0;
            }
            string zoneString = match.ToString();
            //zoneString = zoneString.Remove(0, 2);
            int.TryParse(zoneString, out resultPointNumber);
            return resultPointNumber;
        }

        private static int GetZoneNumber(string line)
        {
            int resultZoneNumber;
            Regex getZoneNumber = new Regex(@"sp[0-9]+", RegexOptions.IgnoreCase);
            Match match = getZoneNumber.Match(line);
            if (match.ToString() == "")
            {
                MessageBox.Show("Invalid zone number\nCorrect input file and try again");
                return 0;
            }
            string zoneString = match.ToString();
            zoneString = zoneString.Remove(0, 2);
            int.TryParse(zoneString, out resultZoneNumber);
            return resultZoneNumber;
        }

        public static Point GetPoint(string line)
        {
            List<string> foundValues = new List<string>();
            //Regex foundPointRegex = new Regex(@"(^\d{ 1, 2 }$)| (^\d{ 0,2}[.]\d{1,2}$)", RegexOptions.IgnoreCase);
            Regex foundPointRegex = new Regex(@"([0-9]+\.[0-9]+)|([0-9]+,)|([0-9]+])|(-[0-9]+\.[0-9]+)|(-[0-9]+,)|(-[0-9]+])", RegexOptions.IgnoreCase);
            var mc = foundPointRegex.Matches(line);
            string currentString;
            foreach (Match match in mc)
            {
                currentString = match.ToString();
                currentString = currentString.TrimEnd(',', ']');
                foundValues.Add(currentString);
            }
            if (foundValues[0] == null | foundValues[1] == null | foundValues[2] == null | foundValues[3] == null | foundValues[4] == null | foundValues[5] == null | foundValues[6] == null)
                return new Point(0, 0, 0, 1, 0, 0, 0);
            float xpos = float.Parse(foundValues[0], CultureInfo.InvariantCulture);
            xpos /= 1000;
            float ypos = float.Parse(foundValues[1], CultureInfo.InvariantCulture);
            ypos /= 1000;
            float zpos = float.Parse(foundValues[2], CultureInfo.InvariantCulture);
            zpos /= 1000;
            float quat1 = float.Parse(foundValues[3], CultureInfo.InvariantCulture);
            float quat2 = float.Parse(foundValues[4], CultureInfo.InvariantCulture);
            float quat3 = float.Parse(foundValues[5], CultureInfo.InvariantCulture);
            float quat4 = float.Parse(foundValues[6], CultureInfo.InvariantCulture);

            Point point = new Point(xpos, ypos, zpos, quat1, quat2, quat3, quat4);
            return point;
        }

        private static LozengeABB CreateLozenge(string line, Point foundPoint)
        {
            LozengeABB emptyLozenge = new LozengeABB(0, 0, 0, 0, new Point(0, 0, 0, 1, 0, 0, 0));
            Regex regWidth = new Regex(@"w[0-9]+", RegexOptions.IgnoreCase);
            Match mWidth = regWidth.Match(line);
            if (mWidth.ToString() == "")
            {
                MessageBox.Show("Invalid lozenge width\nCorrect input file and try again");
                return emptyLozenge;
            }
            float width = float.Parse(mWidth.ToString().Remove(0, 1), CultureInfo.InvariantCulture);
            width /= 1000;
            Regex regRadius = new Regex(@"_r[0-9]*", RegexOptions.IgnoreCase);
            Match mRadius = regRadius.Match(line);
            if (mRadius.ToString() == "")
            {
                MessageBox.Show("Invalid lozenge radius\nCorrect input file and try again");
                return emptyLozenge;
            }
            float radius = float.Parse(mRadius.ToString().Remove(0, 2), CultureInfo.InvariantCulture);
            radius /= 1000;
            Regex regHeight = new Regex(@"(_h|_l)[0-9]+", RegexOptions.IgnoreCase);
            Match mHeight = regHeight.Match(line);
            if (mHeight.ToString() == "")
            {
                MessageBox.Show("Invalid lozenge height\nCorrect input file and try again");
                return emptyLozenge;
            }
            float height = float.Parse(mHeight.ToString().Remove(0, 2), CultureInfo.InvariantCulture);
            height /= 1000;
            Regex regNr = new Regex(@"lozenge[0-9]*", RegexOptions.IgnoreCase);
            Match mNumber = regNr.Match(line);
            int number = int.Parse(mNumber.ToString().Remove(0, 7), CultureInfo.InvariantCulture);
            LozengeABB lozenge = new LozengeABB(number, radius, height, width, foundPoint);
            return lozenge;
        }

        private static SphereABB CreateSphere(string line, Point foundPoint)
        {
            SphereABB emptySphere = new SphereABB(0, new Point(0, 0, 0, 1, 0, 0, 0), 0);
            Regex regNr = new Regex(@"sphere[0-9]*", RegexOptions.IgnoreCase);
            Match mNumber = regNr.Match(line);
            if (mNumber.ToString() == "")
            {
                MessageBox.Show("Invalid sphere number\nCorrect input file and try again");
                return emptySphere;
            }
            int number = int.Parse(mNumber.ToString().Remove(0, 6), CultureInfo.InvariantCulture);
            Regex regRadius = new Regex(@"_r[0-9]*", RegexOptions.IgnoreCase);
            Match mRadius = regRadius.Match(line);
            if (mRadius.ToString() == "")
            {
                MessageBox.Show("Invalid sphere radius\nCorrect input file and try again");
                return emptySphere;
            }
            float radius = float.Parse(mRadius.ToString().Remove(0, 2), CultureInfo.InvariantCulture);
            radius /= 1000;
            SphereABB sphere = new SphereABB(number, foundPoint, radius);
            return sphere;
        }

        private static CapsuleABB CreateCapsule(Point cap1stPos, Point cap2ndPos, float radius, int number)
        {
            radius /= 1000;
            CapsuleABB capsule = new CapsuleABB(number, cap1stPos, cap2ndPos, radius);
            return capsule;
        }

        internal static SafetyConfigABB ReadXmlFileFromBackup(string fileName, bool oldConfig, string robotType = "")
        {
            List<string> zoneStrings = new List<string>();
            List<string> toolStrings = new List<string>();
            Point elbowOrigin = new Point();            
            List<SafeZoneABB> safetyZones = new List<SafeZoneABB>();
            List<SafeToolOldABB> safetyTools = new List<SafeToolOldABB>();
            List<SafeToolABB> safetyToolsNew = new List<SafeToolABB>();
            SafeZoneABB safetyZone = new SafeZoneABB();
            SafeToolOldABB safetyTool = new SafeToolOldABB();
            SafeToolABB safetyToolNew = new SafeToolABB();
            zoneStrings = GetZonestringsFromBackup(fileName);
            toolStrings = GetToolstringsFromBackup(fileName);
            if (!string.IsNullOrEmpty(robotType))
                elbowOrigin = GetElbowOriginABB(robotType);
            string elbowString = GetElbowStrings(fileName);
            ElbowABB elbow = GetSafetyToolOfElbowFromFile(elbowString, true, elbowOrigin);
            foreach (string zoneString in zoneStrings)
            {
                safetyZone = new SafeZoneABB();
                safetyZone = GetSafetyZoneABBFromBackup(zoneString);
                safetyZones.Add(safetyZone);
            }
            foreach (string toolsString in toolStrings)
            {
                safetyTool = new SafeToolOldABB();
                if (toolsString.ToLower().Contains("<tool id=\""))
                {
                    safetyToolNew = GetSafetyToolOfElbowFromFile(toolsString, false);
                    safetyToolsNew.Add(safetyToolNew);
                }
                else
                {
                    safetyTool = GetSafetyToolABBFromBackup(toolsString);
                    safetyTools.Add(safetyTool);
                }
            }

            SafetyConfigABB config = new SafetyConfigABB();
            if (oldConfig)
            {
                List<SafeToolOldABB> correctedTools = new List<SafeToolOldABB>();
                foreach (SafeToolOldABB tool in safetyTools)
                {
                    List<Point> toolPoints = new List<Point>();
                    foreach (Point point in tool.Points)
                    {
                        float x = point.Xpos / 1000;
                        float y = point.Ypos / 1000;
                        float z = point.Zpos / 1000;
                        toolPoints.Add(new Point(x, y, z, 1, 0, 0, 0));
                    }
                    correctedTools.Add(new SafeToolOldABB(tool.Number, toolPoints));

                }
                List<SafeZoneABB> correctedZones = new List<SafeZoneABB>();
                foreach (SafeZoneABB zone in safetyZones)
                {
                    List<Point> correctedZonePoints = new List<Point>();
                    foreach (Point point in zone.SafeZonePoints)
                    {
                        float x = point.Xpos / 1000;
                        float y = point.Ypos / 1000;
                        float z = point.Zpos / 1000;
                        correctedZonePoints.Add(new Point(x, y, z, 1, 0, 0, 0));
                    }
                    correctedZones.Add(new SafeZoneABB(zone.SafeZoneNumber, zone.Name, correctedZonePoints, zone.Height / 1000));
                }
                config.SafeToolsOldABB = correctedTools;
                config.SafeZones = correctedZones;
            }
            else
            {
                config.SafeTools = safetyToolsNew;
                config.SafeToolsOldABB = safetyTools;
                config.SafeZones = safetyZones;
                config.Elbow = elbow;
            }
            return config;

        }

        private static ElbowABB GetElbowABB(string elbowString, Point elbowOrigin)
        {
            throw new NotImplementedException();
        }

        private static string GetElbowStrings(string fileName)
        {
            string result = "";
            bool addLine = false;
            StreamReader reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("<upperarmgeometry"))
                    addLine = true;
                if (addLine)
                    result += line + "\r\n";
                if (line.ToLower().Contains("</upperarmgeometry>"))
                {
                    addLine = false;
                }
            }
            return result;
        }

        private static Point GetElbowOriginABB(string robotType)
        {
            foreach (Robot robot in RobotList.RobotsElbow.Where(x => x.RobotName == robotType))
                return new Point((float)robot.ElbowOriginX, (float)robot.ElbowOriginY, (float)robot.ElbowOriginZ, 1, 0, 0, 0);
            return null;
        }

        private static dynamic GetSafetyToolOfElbowFromFile(string toolsString, bool isElbow, Point elbowOrigin = null)
        {
            string name = "", typeString = "", typeStringEnd = "";
            if (isElbow)
            {
                typeString = "<upperarmgeometry";
                typeStringEnd = "</upperarmgeometry>";
                
            }
            else
            {
                typeString = "<toolgeometry";
                typeStringEnd = "</toolgeometry>";
            }
            bool isLozenge = false, isCapsule = false, isSphere = false;
            List<SphereABB> spheres = new List<SphereABB>();
            List<LozengeABB> lozenges = new List<LozengeABB>();
            List<CapsuleABB> capsules = new List<CapsuleABB>();
            LozengeABB currentLozenge = new LozengeABB();
            SphereABB currentSphere = new SphereABB();
            CapsuleABB currentCapsule = new CapsuleABB();
            int toolNr = 0;
            StringReader reader = new StringReader(toolsString);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains("<tool id="))
                {
                    Regex toolNrRegex = new Regex("(?<=id\\s*=\\s*\")\\d+", RegexOptions.IgnoreCase);
                    toolNr = int.Parse(toolNrRegex.Match(line).ToString());
                }
                if (line.ToLower().Contains(typeString))
                {
                    if (line.ToLower().Contains("lozenge"))
                    {
                        isLozenge = true;
                    }
                    else if (line.ToLower().Contains("capsule"))
                    {
                        isCapsule = true;
                    }
                    else if (line.ToLower().Contains("sphere"))
                    {
                        isSphere = true;
                    }
                }

                if (line.ToLower().Contains(typeStringEnd))
                {
                    if (isLozenge)
                        lozenges.Add(currentLozenge);
                    if (isCapsule)
                        capsules.Add(currentCapsule);
                    if (isSphere)
                        spheres.Add(currentSphere);
                    isCapsule = false; isLozenge = false; isSphere = false;
                    currentLozenge = new LozengeABB();
                    currentSphere = new SphereABB();
                    currentCapsule = new CapsuleABB();
                }

                if (isLozenge)
                {
                    if (line.ToLower().Contains(typeString))
                    {
                        currentLozenge.Number = lozenges.Count + 1;
                        Regex nameRegex = new Regex("(?<=name\\s*=\\s*\")[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                        Regex radiusRegex = new Regex("(?<=radius\\s*=\\s*\")((\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.Radius = (float)Math.Round(float.Parse(radiusRegex.Match(line).ToString()) * 1000);
                        Regex widthRegex = new Regex("(?<=width\\s*=\\s*\")((\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.Width = (float)Math.Round(float.Parse(widthRegex.Match(line).ToString()) * 1000);
                        Regex heightRegex = new Regex("(?<=height\\s*=\\s*\")((\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.Height = (float)Math.Round(float.Parse(heightRegex.Match(line).ToString()) * 1000);
                    }
                    if (line.ToLower().Contains("translation"))
                    {
                        currentLozenge.CenterPoint = new Point();
                        Regex xRegex = new Regex("(?<=x\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.CenterPoint.Xpos = float.Parse(xRegex.Match(line).ToString()) * 1000;
                        Regex yRegex = new Regex("(?<=y\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.CenterPoint.Ypos = float.Parse(yRegex.Match(line).ToString()) * 1000;
                        Regex zRegex = new Regex("(?<=z\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.CenterPoint.Zpos = float.Parse(zRegex.Match(line).ToString()) * 1000;
                    }
                    if (line.ToLower().Contains("quaternion"))
                    {
                        Regex q1Regex = new Regex("(?<=q1\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.CenterPoint.Quat1 = float.Parse(q1Regex.Match(line).ToString());
                        Regex q2Regex = new Regex("(?<=q2\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.CenterPoint.Quat2 = float.Parse(q2Regex.Match(line).ToString());
                        Regex q3Regex = new Regex("(?<=q3\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.CenterPoint.Quat3 = float.Parse(q3Regex.Match(line).ToString());
                        Regex q4Regex = new Regex("(?<=q4\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentLozenge.CenterPoint.Quat4 = float.Parse(q4Regex.Match(line).ToString());
                    }
                }
                if (isCapsule)
                {
                    if (line.ToLower().Contains(typeString))
                    {
                        currentCapsule.Number = capsules.Count + 1;
                        Regex radiusRegex = new Regex("(?<=radius\\s*=\\s*\")((\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentCapsule.Radius = (float)Math.Round(float.Parse(radiusRegex.Match(line).ToString()) * 1000);
                    }
                    if (line.ToLower().Contains("<start"))
                    {
                        currentCapsule.Point1 = new Point();
                        Regex xRegex = new Regex("(?<=x\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentCapsule.Point1.Xpos = float.Parse(xRegex.Match(line).ToString()) * 1000;
                        Regex yRegex = new Regex("(?<=y\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentCapsule.Point1.Ypos = float.Parse(yRegex.Match(line).ToString()) * 1000;
                        Regex zRegex = new Regex("(?<=z\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentCapsule.Point1.Zpos = float.Parse(zRegex.Match(line).ToString()) * 1000;
                    }
                    if (line.ToLower().Contains("<end"))
                    {
                        currentCapsule.Point2 = new Point();
                        Regex xRegex = new Regex("(?<=x\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentCapsule.Point2.Xpos = float.Parse(xRegex.Match(line).ToString()) * 1000;
                        Regex yRegex = new Regex("(?<=y\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentCapsule.Point2.Ypos = float.Parse(yRegex.Match(line).ToString()) * 1000;
                        Regex zRegex = new Regex("(?<=z\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentCapsule.Point2.Zpos = float.Parse(zRegex.Match(line).ToString()) * 1000;
                    }
                }
                if (isSphere)
                {
                    if (line.ToLower().Contains(typeString))
                    {
                        currentSphere.Number = spheres.Count + 1;
                        Regex radiusRegex = new Regex("(?<=radius\\s*=\\s*\")((\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentSphere.Radius = (float)Math.Round(float.Parse(radiusRegex.Match(line).ToString()) * 1000);
                    }
                    if (line.ToLower().Contains("<center"))
                    {
                        currentSphere.CenterPoint = new Point();
                        Regex xRegex = new Regex("(?<=x\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentSphere.CenterPoint.Xpos = float.Parse(xRegex.Match(line).ToString()) * 1000;
                        Regex yRegex = new Regex("(?<=y\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentSphere.CenterPoint.Ypos = float.Parse(yRegex.Match(line).ToString()) * 1000;
                        Regex zRegex = new Regex("(?<=z\\s*=\\s*\")((-\\d+\\.\\d+)|(-\\d+)|(\\d+\\.\\d+)|(\\d+))", RegexOptions.IgnoreCase);
                        currentSphere.CenterPoint.Zpos = float.Parse(zRegex.Match(line).ToString()) * 1000;
                    }
                }
            }

            if (!isElbow)
            {
                SafeToolABB tool = new SafeToolABB(toolNr, "Tool" + toolNr, new Point(0, 0, 0, 1, 0, 0, 0), spheres, lozenges, capsules);
                return tool;
            }
            ElbowABB elbow = new ElbowABB(elbowOrigin, capsules, spheres, lozenges);
            return elbow;
        }

        private static SafeToolOldABB GetSafetyToolABBFromBackup(string toolString)
        {
            SafeToolOldABB tool = new SafeToolOldABB();
            Regex regex;
            StringReader reader = new StringReader(toolString);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains(" toolid="))
                {
                    regex = new Regex(@"(?<=ToolID.*)\d+", RegexOptions.IgnoreCase);
                    tool.Number = int.Parse(regex.Match(line).ToString());                    
                }
                if (line.ToLower().Contains("pos posid"))
                {                   
                    regex = new Regex(@"(?<=Pos_X.*)(-\d+\.\d*|\d+\.\d*|-\d+|\d+)", RegexOptions.IgnoreCase);
                    float x = float.Parse(regex.Match(line).ToString());
                    regex = new Regex(@"(?<=Pos_Y.*)(-\d+\.\d*|\d+\.\d*|-\d+|\d+)", RegexOptions.IgnoreCase);
                    float y = float.Parse(regex.Match(line).ToString());
                    regex = new Regex(@"(?<=Pos_Z.*)(-\d+\.\d*|\d+\.\d*|-\d+|\d+)", RegexOptions.IgnoreCase);
                    float z = float.Parse(regex.Match(line).ToString());
                    Point currentPoint = new Point(x, y, z, 1, 0, 0, 0);
                    if (tool.Points == null)
                        tool.Points = new List<Point>();
                    tool.Points.Add(currentPoint);
                }
            }


            return tool;

        }

        private static List<string> GetToolstringsFromBackup(string fileName)
        {
            string currentString = "";
            List<string> resultStrings = new List<string>();
            bool addLine = false;
            var reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.ToLower().Contains("<flange_super_pose") || line.ToLower().Contains("tool id="))
                {
                    addLine = true;
                }
                if (addLine)
                {
                    currentString = currentString + line + "\n";
                }
                if (line.ToLower().Contains("</flange_super_pose>") || line.ToLower().Contains("</tool>"))
                {
                    resultStrings.Add(currentString);
                    currentString = "";
                    addLine = false;
                }
            }

            return resultStrings;
        }

        private static SafeZoneABB GetSafetyZoneABBFromBackup(string zoneString)
        {
            float top = 0, bottom = 0;
            SafeZoneABB resultZone = new SafeZoneABB();
            var reader = new StringReader(zoneString);
            Regex currentRegex;
            Match match;
            List<Point> resultPoints = new List<Point>();
            Point currentPoint = new Point();
            while (true)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    if (line.Contains("<SafeZone "))
                    {
                        //currentRegex = new Regex(@"([0-9]+\.[0-9]+)|([0-9]+,)|([0-9]+])|(-[0-9]+\.[0-9]+)|(-[0-9]+,)|(-[0-9]+])", RegexOptions.IgnoreCase);
                        currentRegex = new Regex(@"(?<=id..)[0-9]+", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        resultZone.SafeZoneNumber = int.Parse(match.ToString());

                        currentRegex = new Regex(@"(?<=name..).*(?=..top)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        resultZone.Name = match.ToString();

                        currentRegex = new Regex(@"(?<=top..).*(?=..bottom)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        top = float.Parse(match.ToString());

                        currentRegex = new Regex(@"(?<=bottom..).*(?=..speedLimitPriority)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        bottom = float.Parse(match.ToString());

                        resultZone.Height = top - bottom;

                        if (resultZone.SafeZoneNumber == 16)
                            resultZone.IsWorkCell = true;
                        else
                            resultZone.IsWorkCell = false;
                    }
                    if (line.ToLower().Contains("<zone zoneid"))
                    {
                        currentRegex = new Regex(@"(?<=ZoneID.*)\d+", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        resultZone.SafeZoneNumber = int.Parse(match.ToString());
                    }

                    if (line.ToLower().Contains("<dimension_z "))
                    {
                        currentRegex = new Regex(@"(?<=Pos_Z_lower.*)(-\d+\.\d*|\d+\.\d*|-\d+|\d+)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        bottom = float.Parse(match.ToString());
                        currentRegex = new Regex(@"(?<=Pos_Z_upper.*)(-\d+\.\d*|\d+\.\d*|-\d+|\d+)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        top = float.Parse(match.ToString());
                        resultZone.Height = top - bottom;
                    }

                    if (line.Contains("<Point "))
                    {
                        currentRegex = new Regex("(?<=x\\=\")[0-9\\.-]*", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        float x = float.Parse(match.ToString());
                        currentRegex = new Regex("(?<=y\\=\")[0-9\\.-]*", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        float y = float.Parse(match.ToString());
                        currentPoint = new Point(x, y, bottom, 1, 0, 0, 0);
                        resultPoints.Add(currentPoint);
                    }

                    if (line.ToLower().Contains("<pos posi"))
                    {
                        currentRegex = new Regex(@"(?<=Pos_X.*)(-\d+\.\d*|\d+\.\d*|-\d+|\d+)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        float x = float.Parse(match.ToString());
                        currentRegex = new Regex(@"(?<=Pos_Y.*)(-\d+\.\d*|\d+\.\d*|-\d+|\d+)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        float y = float.Parse(match.ToString());
                        if (resultPoints.Count == 0)
                            currentPoint = new Point(x, y, bottom, 1, 0, 0, 0);
                        else
                            currentPoint = new Point(x, y, top, 1, 0, 0, 0);
                        resultPoints.Add(currentPoint);
                    }

                    if (line.Contains("</SafeZone>") || line.ToLower().Contains("</zone>"))
                        break;
                }
            }
            resultZone.SafeZonePoints = resultPoints;
            return resultZone;
        }

        private static List<string> GetZonestringsFromBackup(string fileName)
        {
            string currentString = "";
            List<string> resultStrings = new List<string>();
            bool addLine = false;
            var reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<SafeZone id") || line.Contains("<Zone ZoneID"))
                {
                    addLine = true;
                }
                if (addLine)
                {
                    currentString = currentString + line + "\n";
                }
                if (line.Contains("</SafeZone>") || line.Contains("</Zone>"))
                {
                    resultStrings.Add(currentString);
                    currentString = "";
                    addLine = false;
                }
            }

            return resultStrings;
        }
        
        internal static string BuildModFileFromBackup(SafetyConfigABB configABBfromBackup, bool oldConfig)
        {
            string currentString = "";
            if (oldConfig)
                currentString = "MODULE PSC_Define_Safety_Zones\n\n";
            else
                currentString = "MODULE Safety\n\n";
            int pointCounter = 1;
            foreach (SafeZoneABB zone in configABBfromBackup.SafeZones)
            {
                pointCounter = 1;
                foreach (Point point in zone.SafeZonePoints)
                {
                    if (oldConfig)
                    {
                        currentString += "CONST robtarget pSTZ_Set_" + zone.SafeZoneNumber + "_Pos_" + pointCounter + ":=[[" + point.Xpos * 1000 + "," + point.Ypos * 1000 + "," + point.Zpos * 1000 + "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                    }
                    else
                        currentString = currentString + "CONST robtarget " + zone.Name + "_Point_" + pointCounter.ToString() + ":=[[" + (point.Xpos * 1000).ToString() + "," + (point.Ypos * 1000).ToString() + "," + (point.Zpos * 1000).ToString() + "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \n";
                    pointCounter++;
                }
            }

            if (oldConfig)
            {
                foreach (SafeToolOldABB tool in configABBfromBackup.SafeToolsOldABB)
                {
                    pointCounter = 1;
                    foreach (var point in tool.Points)
                    {
                        currentString += "CONST robtarget posPSCTool" + tool.Number + "_" + pointCounter + ":=[[" + point.Xpos * 1000 + "," + point.Ypos * 1000 + "," + point.Zpos * 1000 + "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                        pointCounter++;
                    }
                }
            }
            else
            {
                foreach (SafeToolABB tool in configABBfromBackup.SafeTools)
                {
                    foreach (LozengeABB lozenge in tool.Lozenges)
                    {
                        currentString += "CONST robtarget tool" + tool.Number + "_Lozenge" + lozenge.Number + "_r"+ lozenge.Radius + "_w" + lozenge.Height + "_l" + lozenge.Width  + ":=[[" + lozenge.CenterPoint.Xpos + "," + lozenge.CenterPoint.Ypos + "," + lozenge.CenterPoint.Zpos + "],["+lozenge.CenterPoint.Quat1+","+ lozenge.CenterPoint.Quat2 + ","+ lozenge.CenterPoint.Quat3 + ","+ lozenge.CenterPoint.Quat4 + "],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                    }
                    foreach (CapsuleABB capsule in tool.Capsules)
                    {
                        currentString += "CONST robtarget tool" + tool.Number + "_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p1:=[[" + capsule.Point1.Xpos + "," + capsule.Point1.Ypos + "," + capsule.Point1.Zpos + "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                        currentString += "CONST robtarget tool" + tool.Number + "_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p2:=[[" + capsule.Point2.Xpos + "," + capsule.Point2.Ypos + "," + capsule.Point2.Zpos + "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                    }
                    foreach (SphereABB sphere in tool.Spheres)
                    {
                        currentString += "CONST robtarget tool" + tool.Number + "_Sphere" + sphere.Number + "_r" + sphere.Radius + ":=[[" + sphere.CenterPoint.Xpos + "," + sphere.CenterPoint.Ypos + "," + sphere.CenterPoint.Zpos + "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                    }
                }
                if (configABBfromBackup.Elbow != null)
                {
                    foreach (LozengeABB lozenge in configABBfromBackup.Elbow.Lozenges)
                    {
                        currentString += "CONST robtarget elbow_Lozenge" + lozenge.Number + "_r" + lozenge.Radius + "_w" + lozenge.Height + "_l" + lozenge.Width + ":=[[" + (lozenge.CenterPoint.Xpos + configABBfromBackup.Elbow.Origin.Xpos) + "," + (lozenge.CenterPoint.Ypos + configABBfromBackup.Elbow.Origin.Ypos) + "," + (lozenge.CenterPoint.Zpos + configABBfromBackup.Elbow.Origin.Zpos) + "],[" + lozenge.CenterPoint.Quat1 + "," + lozenge.CenterPoint.Quat2 + "," + lozenge.CenterPoint.Quat3 + "," + lozenge.CenterPoint.Quat4 + "],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                    }
                    foreach (CapsuleABB capsule in configABBfromBackup.Elbow.Capsules)
                    {
                        currentString += "CONST robtarget elbow_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p1:=[[" + (capsule.Point1.Xpos + configABBfromBackup.Elbow.Origin.Xpos) + "," + (capsule.Point1.Ypos + configABBfromBackup.Elbow.Origin.Ypos) + "," + (capsule.Point1.Zpos + configABBfromBackup.Elbow.Origin.Zpos) + "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                        currentString += "CONST robtarget elbow_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p2:=[[" + (capsule.Point2.Xpos + configABBfromBackup.Elbow.Origin.Xpos) + "," + (capsule.Point2.Ypos + configABBfromBackup.Elbow.Origin.Ypos) + "," + (capsule.Point2.Zpos + configABBfromBackup.Elbow.Origin.Zpos)+ "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                    }
                    foreach (SphereABB sphere in configABBfromBackup.Elbow.Spheres)
                    {
                        currentString += "CONST robtarget elbow_Sphere" + sphere.Number + "_r" + sphere.Radius + ":=[[" + (sphere.CenterPoint.Xpos + configABBfromBackup.Elbow.Origin.Xpos) + "," + (sphere.CenterPoint.Ypos + configABBfromBackup.Elbow.Origin.Ypos)+ "," + (sphere.CenterPoint.Zpos + configABBfromBackup.Elbow.Origin.Zpos)+ "],[1,0,0,0],[1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]]; \r\n";
                    }
                }
            }

            if (!oldConfig)
            {
                currentString = currentString + "\nPERS tooldata tool0:=[TRUE,[[0,0,0],[1,0,0,0]],[0.001,[0,0,0.001],[1,0,0,0],0,0,0]];\r\n";
                currentString += "PERS tooldata tool_base:=[FALSE,[[0,0,0],[1,0,0,0]],[0.001,[0,0,0.0],[1,0,0,0],0,0,0]];\r\n";
                currentString += "PERS wobjdata wobj_flange:=[True,True,\"\",[[0,0,0],[1,0,0,0]],[[0,0,0],[1,0,0,0]]];\r\n\r\n\r\n";
                //currentString = currentString + "\nPROC SafetyConfig()\n";
                foreach (SafeZoneABB zone in configABBfromBackup.SafeZones)
                {
                    pointCounter = 1;
                    currentString = currentString + "PROC " + zone.Name + "_Height" + (Math.Round(zone.Height * 1000)).ToString() + "()\r\n";
                    foreach (Point point in zone.SafeZonePoints)
                    {
                        currentString = currentString + "MoveJ " + zone.Name + "_Point_" + pointCounter.ToString() + ",v5000,fine,tool0\\Wobj:=wobj0;\r\n";
                        pointCounter++;
                    }
                    currentString += "ENDPROC\r\n\r\n";
                }
                foreach (SafeToolABB tool in configABBfromBackup.SafeTools)
                {
                    currentString = currentString + "PROC " + tool.Name + "()\r\n";
                    foreach (LozengeABB lozenge in tool.Lozenges)
                    {
                        currentString = currentString + "MoveJ tool" + tool.Number + "_Lozenge" + lozenge.Number + "_r"+ lozenge.Radius + "_w" + lozenge.Height + "_l" + lozenge.Width + ",v5000,fine,tool_base\\Wobj:=wobj_flange;\r\n";
                    }
                    foreach (CapsuleABB capsule in tool.Capsules)
                    {
                        currentString = currentString + "MoveJ tool" + +tool.Number + "_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p1,v5000,fine,tool_base\\Wobj:=wobj_flange;\r\n";
                        currentString = currentString + "MoveJ tool" + +tool.Number + "_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p2,v5000,fine,tool_base\\Wobj:=wobj_flange;\r\n";
                    }
                    foreach (SphereABB sphere in tool.Spheres)
                    {
                        currentString += "MoveJ tool" + tool.Number + "_Sphere" + sphere.Number + "_r" + sphere.Radius + ",v5000,fine,tool_base\\Wobj:=wobj_flange;\r\n";
                    }
                    currentString += "ENDPROC\r\n\r\n";
                }
                if (configABBfromBackup.Elbow != null)
                {
                    currentString = currentString + "PROC Elbow()\r\n";
                    foreach (LozengeABB lozenge in configABBfromBackup.Elbow.Lozenges)
                    {
                        currentString = currentString + "MoveJ elbow_Lozenge" + lozenge.Number + "_r" + lozenge.Radius + "_w" + lozenge.Height + "_l" + lozenge.Width + ",v5000,fine,tool0\\Wobj:=wobj0;\r\n";
                    }
                    foreach (CapsuleABB capsule in configABBfromBackup.Elbow.Capsules)
                    {
                        currentString = currentString + "MoveJ elbow_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p1,v5000,fine,tool0\\Wobj:=wobj0;\r\n";
                        currentString = currentString + "MoveJ elbow_Capsule" + capsule.Number + "_r" + capsule.Radius + "_p2,v5000,fine,tool0\\Wobj:=wobj0;\r\n";
                    }
                    foreach (SphereABB sphere in configABBfromBackup.Elbow.Spheres)
                    {
                        currentString += "MoveJ elbow_Sphere" + sphere.Number + "_r" + sphere.Radius + ",v5000,fine,tool0\\Wobj:=wobj0;\r\n";
                    }
                    currentString += "ENDPROC\r\n\r\n";
                }
            }
            else
            {
                currentString += "\r\n\r\nCONST wobjdata wobj_flansh:=[True,True,\"\",[[0,0,0],[1,0,0,0]],[[0,0,0],[1,0,0,0]]];\r\n\r\n\r\n";
                currentString += "PROC MOV_PSC_Define_Safety_Zones()\r\n\r\n";
                foreach (SafeZoneABB zone in configABBfromBackup.SafeZones)
                {
                    pointCounter = 1;
                    foreach (Point point in zone.SafeZonePoints)
                    {
                        currentString += "MoveJ pSTZ_Set_"+zone.SafeZoneNumber+"_Pos_"+pointCounter+",vmax,fine,tool0\\Wobj:=wobj0;\r\n";
                        pointCounter++;
                    }
                }
                foreach (SafeToolOldABB tool in configABBfromBackup.SafeToolsOldABB)
                {
                    pointCounter = 1;
                    foreach (var point in tool.Points)
                    {
                        currentString += "MoveJ posPSCTool"+tool.Number+"_"+pointCounter+",vmax,fine,tool0\\Wobj:=wobj_flansh; \r\n";
                        pointCounter++;
                    }
                }
                currentString = currentString + "ENDPROC\n\n";
            }
            
            currentString = currentString + "\nENDMODULE";
            return currentString;


        }


        #endregion

        #region FANUC

        internal static SafetyConfigFanuc ReadXMLFileFanuc(string xmlFile, bool addZone32)
        {
            List<SafeToolFanuc> tools = new List<SafeToolFanuc>();
            List<UserFrame> userframes = new List<UserFrame>();
            SafeZonesFanuc safeZones = new SafeZonesFanuc();
            ElbowFanuc elbow = new ElbowFanuc();
            string robotName = "";
            if (!ValidSafetyConfigFanuc(xmlFile))
            {
                MessageBox.Show("Selected SafetyRobot.xml does not contain configuration for Fanuc robot");
                return null;
            }
            tools = GetSafeToolsFanuc(xmlFile);
            userframes = GetUserFramesFanuc(xmlFile);
            safeZones = GetSafeZonesFanuc(xmlFile, addZone32);
            elbow = GetElbowFanuc(xmlFile);

            SafetyConfigFanuc fanucConfig = new SafetyConfigFanuc(tools, userframes, safeZones, elbow, robotName);
            return fanucConfig;
        }

        private static List<UserFrame> GetUserFramesFanuc(string xmlFile)
        {
            List<string> uFrameStrings = new List<string>();
            uFrameStrings = GetUserFrameStrings(xmlFile);
            List<UserFrame> result = GetUserFramesFromStrings(uFrameStrings);
            return result;
        }

        private static bool ValidSafetyConfigFanuc(string xmlFile)
        {
            bool isValid = false, isFanuc = false;
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<WestSafetyRobot Version"))
                    isValid = true;
                if (line.Contains("RobotType=\"3\""))
                    isFanuc = true;
                if (isValid & isFanuc)
                    break;
            }
            return isFanuc & isValid;
        }

        #region elbow Fanuc
        private static ElbowFanuc GetElbowFanuc(string xmlFile)
        {
            ElbowFanuc result = new ElbowFanuc();
            Regex currentRegex;
            Match match;
            List<string> stringsStart = new List<string>();
            List<string> stringsEnd = new List<string>();
            bool elbowPresent = false;
            float radius = 0;
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<LinkNumber>3</LinkNumber>"))
                {
                    elbowPresent = true;
                }
                if (elbowPresent & line.Contains("<Radius>"))
                {
                    currentRegex = new Regex(@"(?<=<Radius>)([0-9]*)|([0-9]*\.[0-9]*)", RegexOptions.IgnoreCase);
                    match = currentRegex.Match(line);
                    radius = (float.Parse(match.ToString()));
                }
                if (elbowPresent & line.Contains("<SphereStart>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    foreach (Match currentMatch in currentRegex.Matches(line))
                    {
                        stringsStart.Add(currentMatch.ToString());
                    }
                }
                if (elbowPresent & line.Contains("<SphereEnd>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    foreach (Match currentMatch in currentRegex.Matches(line))
                    {
                        stringsEnd.Add(currentMatch.ToString());
                    }
                }
                if (elbowPresent & line.Contains("</SafetyGeometryUserModelLineSegFANUC>"))
                {
                    break;
                }
            }
            reader.Close();
            if (elbowPresent)
            {
                Point startPoint = new Point(float.Parse(stringsStart[0]), float.Parse(stringsStart[1]), float.Parse(stringsStart[2]), 1, 0, 0, 0);
                Point endPoint = new Point(float.Parse(stringsEnd[0]), float.Parse(stringsEnd[1]), float.Parse(stringsEnd[2]), 1, 0, 0, 0);
                result.Elbow = new CapsuleFanuc(1, startPoint, endPoint, radius);
                stringsStart.Clear();
                stringsEnd.Clear();
            }
            return result;
        }
        #endregion

        #region SafeZone
        private static SafeZonesFanuc GetSafeZonesFanuc(string xmlFile, bool addZone32)
        {
            FanucZoneStrings zonestrings = new FanucZoneStrings();
            SafeZonesFanuc resultZones = new SafeZonesFanuc();
            resultZones.SafeZonesTwoPoints = new List<SafeZoneFanucTwoPoints>();
            resultZones.SafeZonesMultiPoints = new List<SafeZoneFanucMultiPoints>();
            List<string> UFrameStrings = new List<string>();
            UFrameStrings = GetUserFrameStrings(xmlFile);
            List<UserFrame> userFrames = new List<UserFrame>();
            userFrames = GetUserFramesFromStrings(UFrameStrings);
            List<FanucZoneStrings> safeZoneStrings = new List<FanucZoneStrings>();
            safeZoneStrings = FindSafeZoneStrings(xmlFile);
            foreach (FanucZoneStrings zoneString in safeZoneStrings.Where(item => item.SafeZonesTwoPoints != ""))
            {
                SafeZoneFanucTwoPoints safeZone = new SafeZoneFanucTwoPoints();
                safeZone = GetSafeZoneTwoPointsFromXml(zoneString.SafeZonesTwoPoints, userFrames);
                resultZones.SafeZonesTwoPoints.Add(safeZone);
            }
            resultZones.Sort();
            foreach (FanucZoneStrings zoneString in safeZoneStrings.Where(item => item.SafeZonesMultiPoints != ""))
            {
                SafeZoneFanucMultiPoints safeZone = new SafeZoneFanucMultiPoints();
                safeZone = GetSafeZoneMultiPointsFromXml(zoneString.SafeZonesMultiPoints);
                resultZones.SafeZonesMultiPoints.Add(safeZone);
            }
            if (addZone32)
                resultZones.SafeZonesTwoPoints.Add(new SafeZoneFanucTwoPoints(true, 32, "Zone32", new Point(-30000, -30000, -30000, 0, 0, 0, 0), new Point(30000, 30000, 30000, 0, 0, 0, 0), new UserFrame()));
            return resultZones;
        }

        private static List<string> GetUserFrameStrings(string xmlFile)
        {
            List<string> result = new List<string>();
            string currentString = "";
            bool addLine = false;
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<UserFrameFANUC Id="))
                {
                    addLine = true;
                }
                if (addLine)
                {
                    currentString = currentString + line + "\n";
                }
                if (line.Contains("</UserFrameFANUC>"))
                {
                    addLine = false;
                    result.Add(currentString);
                    currentString = "";
                }
            }
            return result;
        }

        private static List<UserFrame> GetUserFramesFromStrings(List<string> uFrameStrings)
        {
            List<UserFrame> result = new List<UserFrame>();
            Regex currentRegex;
            UserFrame currentFrame = null;
            int currentNumber = 0, currentUserUFrameNumber = 0;
            PointEuler currentPoint = new PointEuler();
            List<string> foundValues = new List<string>();
            foreach (string uframeString in uFrameStrings)
            {
                var reader = new StringReader(uframeString);
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("<UserFrameNumber>"))
                    {
                        currentFrame = new UserFrame();
                        currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        currentNumber = int.Parse(match.ToString());
                        currentFrame.Number = currentNumber;
                    }
                    if (line.Contains("<UserFramePosition>"))
                    {
                        //currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|((?<=>)[0-9]*)|((?<=_)-[0-9]*\.[0-9]*)|((?<=_)[0-9]*\.[0-9]*)|((?<=_)-[0-9]*)|((?<=_)[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            foundValues.Add(currentMatch.ToString());
                        }
                        currentPoint = new PointEuler(float.Parse(foundValues[0]), float.Parse(foundValues[1]), float.Parse(foundValues[2]), RadToDeg(float.Parse(foundValues[3])), RadToDeg(float.Parse(foundValues[4])), RadToDeg(float.Parse(foundValues[5])));
                        foundValues = new List<string>();
                        currentFrame.Point = currentPoint;
                    }
                    if (line.Contains("<UserFrameUFrameNumber>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        currentUserUFrameNumber = int.Parse(match.ToString());
                        currentFrame.UserFrameUFrameNumber = currentUserUFrameNumber;
                    }
                    if (line.Contains("</UserFrameFANUC>"))
                    {
                        currentFrame.IsValidUserFrame = true;
                        result.Add(currentFrame);
                        currentFrame = new UserFrame();
                        break;
                    }

                }
            }
            if (result.Count == 0)
                result.Add(new UserFrame(new PointEuler(0, 0, 0, 0, 0, 0), 1, 11, false));
            return result;
        }

        private static List<FanucZoneStrings> FindSafeZoneStrings(string xmlFile)
        {
            List<FanucZoneStrings> result = new List<FanucZoneStrings>();
            string currentString = "";
            bool addLine = false;
            bool isTwoPointZone = false;
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<SafetyObjectCartesianPositionCheckFANUC Id="))
                {
                    addLine = true;
                }
                if (addLine)
                {
                    currentString = currentString + line + "\n";
                }
                if (line.Contains("<SafetyGeometryZoneLinesFANUC Id="))
                {
                    isTwoPointZone = false;
                }
                if (line.Contains("<SafetyGeometryZoneDiagonalFANUC Id="))
                {
                    isTwoPointZone = true;
                }
                if (line.Contains("</SafetyObjectCartesianPositionCheckFANUC>"))
                {
                    addLine = false;
                    FanucZoneStrings currentZonestring = new FanucZoneStrings();
                    if (isTwoPointZone)
                    {
                        currentZonestring.SafeZonesTwoPoints = currentString;
                        currentZonestring.SafeZonesMultiPoints = "";
                        result.Add(currentZonestring);
                    }
                    else
                    {
                        currentZonestring.SafeZonesTwoPoints = "";
                        currentZonestring.SafeZonesMultiPoints = currentString;
                        result.Add(currentZonestring);
                    }
                    currentString = "";
                }
            }
            reader.Close();
            return result;
        }

        private static SafeZoneFanucTwoPoints GetSafeZoneTwoPointsFromXml(string zoneString, List<UserFrame> userFrames)
        {
            int counter = 0;
            int[] zonenumbers = new int[userFrames.Count];
            foreach (UserFrame uframe in userFrames)
            {
                zonenumbers[counter] = uframe.Number;
                counter++;
            }
            int maxValue = zonenumbers.Max();
            string name = "";
            bool isDi = false;
            Point startPoint = new Point();
            Point endPoint = new Point();
            Regex currentRegex;
            List<string> foundValues = new List<string>();
            int number = 0, mode, uframenum = 0;
            var reader = new StringReader(zoneString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line.Contains("<Description>"))
                {
                    currentRegex = new Regex(@"((?<=>).*(?=\<))", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    name = match.ToString();
                }
                if (line.Contains("<CPCMethod>"))
                {
                    currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    mode = int.Parse(match.ToString());
                    if (mode == 0)
                        isDi = true;
                }
                if (line.Contains("<CPCUserFrameNum>"))
                {
                    currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    uframenum = int.Parse(match.ToString());
                }
                if (line.Contains("<CPCNumber>"))
                {
                    currentRegex = new Regex(@"((?<=<CPCNumber>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    number = int.Parse(match.ToString());
                }
                if (line.Contains("<ZoneDiagonalPoint1>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    foreach (Match currentMatch in currentRegex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    startPoint = new Point((float.Parse(foundValues[0].ToString())), (float.Parse(foundValues[1].ToString())), (float.Parse(foundValues[2].ToString())), 0, 0, 0, 0);
                    foundValues = new List<string>();
                }

                if (line.Contains("<ZoneDiagonalPoint2>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    foreach (Match currentMatch in currentRegex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    endPoint = new Point((float.Parse(foundValues[0].ToString())), (float.Parse(foundValues[1].ToString())), (float.Parse(foundValues[2].ToString())), 0, 0, 0, 0);
                    foundValues = new List<string>();
                }
                if (line.Contains("</SafetyGeometryZoneDiagonalFANUC>"))
                    break;
            }
            reader.Close();
            UserFrame currentUserFrame = new UserFrame();
            foreach (UserFrame uframe in userFrames.Where(item => item.Number == uframenum))
            {
                currentUserFrame = uframe;
                break;
            }
            //foreach (UserFrame uframe in userFrames.Where(item => item.IsValidUserFrame == false))
            //{
            //    currentUserFrame = uframe;
            //    break;
            //}
            if (currentUserFrame.IsValidUserFrame == false)
            {
                MessageBox.Show("Zone number " + number.ToString() + " is assigned to UserFrame 0 or assigned UserFrame does not exist. UserFrame will be set to frame number " + (maxValue + 1).ToString() + " located at robot's base.", "WARNING", MessageBoxButtons.OK);
                if (uframenum == 0)
                    currentUserFrame.Number = maxValue + 1;
                currentUserFrame.Point = new PointEuler(0, 0, 0, 0, 0, 0);
                currentUserFrame.UserFrameUFrameNumber = currentUserFrame.Number + 10;
                //currentUserFrame.IsValidUserFrame = true;
            }
            SafeZoneFanucTwoPoints result = new SafeZoneFanucTwoPoints(isDi, number, name, startPoint, endPoint, currentUserFrame);
            return result;
        }

        private static SafeZoneFanucMultiPoints GetSafeZoneMultiPointsFromXml(string zoneString)
        {
            bool isLi = true;
            string name = "";
            List<Point> points = new List<Point>();
            List<string> foundValues = new List<string>();
            Point currentPoint = new Point();
            int number = 0, mode = 0;
            float top = 0, bottom = 0;
            Regex currentRegex;
            var reader = new StringReader(zoneString);
            while (true)
            {
                var line = reader.ReadLine();

                if (line.Contains("<Description>"))
                {
                    currentRegex = new Regex(@"((?<=>).*(?=\<))", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    name = match.ToString();
                }
                if (line.Contains("<CPCNumber>"))
                {
                    currentRegex = new Regex(@"((?<=<CPCNumber>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    number = int.Parse(match.ToString());
                }
                if (line.Contains("<CPCMethod>"))
                {
                    currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    mode = int.Parse(match.ToString());
                    if (mode != 2)
                        isLi = false;
                }
                if (line.Contains("<ZoneLineZ1Value>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    top = float.Parse(match.ToString());
                }
                if (line.Contains("<ZoneLineZ2Value>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    bottom = float.Parse(match.ToString());
                }
                if (line.Contains("<ZoneLinePointXY>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    foreach (Match currentMatch in currentRegex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    currentPoint = new Point((float.Parse(foundValues[0].ToString())), (float.Parse(foundValues[1].ToString())), 0, 0, 0, 0, 0);
                    points.Add(currentPoint);
                    foundValues = new List<string>();
                }
                if (line.Contains("</SafetyGeometryZoneLinesFANUC>"))
                    break;
            }
            reader.Close();
            SafeZoneFanucMultiPoints result = new SafeZoneFanucMultiPoints(number, name, points, bottom, top, isLi);
            return result;

        }
        #endregion

        #region safeTools
        private static List<SafeToolFanuc> GetSafeToolsFanuc(string xmlFile)
        {
            List<string> toolStrings = new List<string>();
            List<SafeToolFanuc> resultTools = new List<SafeToolFanuc>();
            toolStrings = FindSafeToolStringsFanuc(xmlFile);
            foreach (string toolString in toolStrings)
            {
                SafeToolFanuc safeTool = new SafeToolFanuc();
                safeTool = GetToolFromToolsStringFanuc(toolString);
                resultTools.Add(safeTool);
            }

            return resultTools;
        }

        private static List<string> FindSafeToolStringsFanuc(string xmlFile)
        {
            List<string> resultStrings = new List<string>();
            bool addLine = false;
            string currentString = "";
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<SafetyObjectUserModelFANUC Id="))
                {
                    addLine = true;
                }
                //if (line.Contains("<LinkNumber>3</LinkNumber>"))
                //{
                //    addLine = false;
                //    currentString = "";
                //}
                if (addLine)
                {
                    currentString = currentString + line + "\n";
                }
                if (addLine & line.Contains("</SafetyObjectUserModelFANUC>"))
                {
                    addLine = false;
                    resultStrings.Add(currentString);
                    currentString = "";
                }
            }
            reader.Close();
            return resultStrings;
        }

        private static SafeToolFanuc GetToolFromToolsStringFanuc(string toolString)
        {
            Regex currentRegex;
            int number = 0;
            string name = "";
            Point currentPoint = new Point();
            List<string> foundValues = new List<string>();
            List<CapsuleFanuc> resultCapsules = GetCapsulesFromToolStringFanuc(toolString);
            List<SphereFanuc> resultSpheres = GetSphereFromToolStringFanuc(toolString);
            List<BoxFanuc> resultBoxes = GetBoxesFromToolsStringFanuc(toolString);

            var reader = new StringReader(toolString);
            while (true)
            {
                var line = reader.ReadLine();

                if (line.Contains("<UserModelNumber>"))
                {
                    currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    number = int.Parse(match.ToString());
                }
                if (line.Contains("<Description>"))
                {
                    currentRegex = new Regex(@"((?<=>).*(?=\<))", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    name = match.ToString();
                }
                if (line.Contains("<ZoneLinePointXY>"))
                {
                    currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    foreach (Match currentMatch in currentRegex.Matches(line))
                    {
                        foundValues.Add(currentMatch.ToString());
                    }
                    currentPoint = new Point((float.Parse(foundValues[0].ToString())), (float.Parse(foundValues[1].ToString())), 0, 0, 0, 0, 0);
                    foundValues = new List<string>();
                }
                if (line.Contains("</SafetyObjectUserModelFANUC>"))
                    break;
            }
            reader.Close();
            SafeToolFanuc resultTool = new SafeToolFanuc(number, name, new Point(0, 0, 0, 1, 0, 0, 0), resultCapsules, resultSpheres, resultBoxes);
            return resultTool;
        }


        private static List<CapsuleFanuc> GetCapsulesFromToolStringFanuc(string toolsString)
        {
            List<string> capsuleStrings = GetCapsuleStringsFanuc(toolsString);
            List<CapsuleFanuc> resultCapsules = new List<CapsuleFanuc>();
            foreach (string capsuleString in capsuleStrings)
            {
                Regex currentRegex;
                Match match;
                bool addItem = true;
                var reader = new StringReader(capsuleString);
                int number = 0;
                float radius = 0;
                List<string> stringsStart = new List<string>();
                List<string> stringsEnd = new List<string>();
                while (true)
                {
                    var line = reader.ReadLine();

                    if (line.Contains("<Number>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        number = int.Parse(match.ToString());
                    }
                    if (line.Contains("<LinkNumber>3</LinkNumber>"))
                    {
                        addItem = false;
                    }
                    if (line.Contains("<LinkNumber>99</LinkNumber>"))
                    {
                        addItem = true;
                    }
                    if (line.Contains("<Radius>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        radius = (float.Parse(match.ToString()));
                    }
                    if (line.Contains("<SphereStart>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            stringsStart.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("<SphereEnd>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            stringsEnd.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("</SafetyGeometryUserModelLineSegFANUC>"))
                    {
                        Point startPoint = new Point(float.Parse(stringsStart[0]), float.Parse(stringsStart[1]), float.Parse(stringsStart[2]), 1, 0, 0, 0);
                        Point endPoint = new Point(float.Parse(stringsEnd[0]), float.Parse(stringsEnd[1]), float.Parse(stringsEnd[2]), 1, 0, 0, 0);
                        CapsuleFanuc currentCapsule = new CapsuleFanuc(number, startPoint, endPoint, radius);
                        if (addItem)
                            resultCapsules.Add(currentCapsule);
                        stringsStart.Clear();
                        stringsEnd.Clear();
                        break;
                    }
                }
            }
            return resultCapsules;
        }


        private static List<BoxFanuc> GetBoxesFromToolsStringFanuc(string toolString)
        {
            List<string> boxStrings = GetBoxStringsFanuc(toolString);
            List<BoxFanuc> resultBoxes = new List<BoxFanuc>();
            int number = 0;
            float height = 0, width = 0, thickness = 0;

            foreach (string boxString in boxStrings)
            {
                Regex currentRegex;
                Match match;
                bool addItem = true;
                List<string> pointStrings = new List<string>();
                var reader = new StringReader(boxString);
                while (true)
                {
                    var line = reader.ReadLine();

                    if (line.Contains("<Number>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        number = int.Parse(match.ToString());
                    }
                    if (line.Contains("<LinkNumber>3</LinkNumber>"))
                    {
                        addItem = false;
                    }
                    if (line.Contains("<LinkNumber>99</LinkNumber>"))
                    {
                        addItem = true;
                    }
                    if (line.Contains("<BoxOffset>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            pointStrings.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("<BoxHeigth>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        height = (float.Parse(match.ToString()));
                    }
                    if (line.Contains("<BoxWidth>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        width = (float.Parse(match.ToString()));
                    }
                    if (line.Contains("<BoxThick>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        thickness = (float.Parse(match.ToString()));
                    }
                    if (line.Contains("</SafetyGeometryUserModelBoxFANUC>"))
                    {
                        Point centerPoint = new Point(float.Parse(pointStrings[0]), float.Parse(pointStrings[1]), float.Parse(pointStrings[2]), 1, 0, 0, 0);
                        BoxFanuc currentBox = new BoxFanuc(number, centerPoint, height, width, thickness);
                        if (addItem)
                            resultBoxes.Add(currentBox);
                        pointStrings.Clear();
                        break;
                    }
                }
            }

            return resultBoxes;
        }

        private static List<SphereFanuc> GetSphereFromToolStringFanuc(string toolString)
        {
            List<string> sphereStrings = GetSphereStringsFanuc(toolString);
            List<SphereFanuc> resultSpheres = new List<SphereFanuc>();
            List<string> pointStrings = new List<string>();
            int number = 0;
            float radius = 0;

            foreach (string sphereString in sphereStrings)
            {
                Regex currentRegex;
                Match match;
                bool addItem = true;
                var reader = new StringReader(sphereString);
                while (true)
                {
                    var line = reader.ReadLine();

                    if (line.Contains("<Number>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        number = int.Parse(match.ToString());
                    }
                    if (line.Contains("<LinkNumber>3</LinkNumber>"))
                    {
                        addItem = false;
                    }
                    if (line.Contains("<LinkNumber>99</LinkNumber>"))
                    {
                        addItem = true;
                    }
                    if (line.Contains("<Radius>"))
                    {
                        currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        radius = (float.Parse(match.ToString()));
                    }

                    if (line.Contains("<Center>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in currentRegex.Matches(line))
                        {
                            pointStrings.Add(currentMatch.ToString());
                        }
                    }
                    if (line.Contains("</SafetyGeometryUserModelPointFANUC>"))
                    {
                        Point centerPoint = new Point(float.Parse(pointStrings[0]), float.Parse(pointStrings[1]), float.Parse(pointStrings[2]), 1, 0, 0, 0);
                        SphereFanuc currentBox = new SphereFanuc(number, centerPoint, radius);
                        if (addItem)
                            resultSpheres.Add(currentBox);
                        pointStrings.Clear();
                        break;
                    }
                }

            }
            return resultSpheres;
        }

        private static List<string> GetCapsuleStringsFanuc(string toolsString)
        {
            List<string> capsuleStrings = new List<string>();
            bool addLine = false;
            string currentString = "";
            var reader = new StringReader(toolsString);
            while (true)
            {
                var line = reader.ReadLine();

                if (line.Contains("<SafetyGeometryUserModelLineSegFANUC Id="))
                {
                    addLine = true;
                }
                if (addLine)
                    currentString = currentString + line + "\n";
                if (line.Contains("</SafetyGeometryUserModelLineSegFANUC>"))
                {
                    addLine = false;
                    capsuleStrings.Add(currentString);
                    currentString = "";
                }
                if (line.Contains("</SafetyObjectUserModelFANUC>"))
                    break;
            }
            reader.Close();
            return capsuleStrings;
        }

        private static List<string> GetSphereStringsFanuc(string toolsString)
        {
            List<string> sphereStrings = new List<string>();
            bool addLine = false;
            string currentString = "";
            var reader = new StringReader(toolsString);
            while (true)
            {
                var line = reader.ReadLine();

                if (line.Contains("<SafetyGeometryUserModelPointFANUC Id="))
                {
                    addLine = true;
                }
                if (addLine)
                    currentString = currentString + line + "\n";
                if (line.Contains("</SafetyGeometryUserModelPointFANUC>"))
                {
                    addLine = false;
                    sphereStrings.Add(currentString);
                    currentString = "";
                }
                if (line.Contains("</SafetyObjectUserModelFANUC>"))
                    break;
            }
            reader.Close();
            return sphereStrings;
        }

        private static List<string> GetBoxStringsFanuc(string toolsString)
        {
            List<string> boxStrings = new List<string>();
            bool addLine = false;
            string currentString = "";
            var reader = new StringReader(toolsString);
            while (true)
            {
                var line = reader.ReadLine();

                if (line.Contains("<SafetyGeometryUserModelBoxFANUC Id="))
                {
                    addLine = true;
                }
                if (addLine)
                    currentString = currentString + line + "\n";
                if (line.Contains("</SafetyGeometryUserModelBoxFANUC>"))
                {
                    addLine = false;
                    boxStrings.Add(currentString);
                    currentString = "";
                }
                if (line.Contains("</SafetyObjectUserModelFANUC>"))
                    break;
            }
            reader.Close();
            return boxStrings;
        }


        #endregion
        #endregion

        #region KUKA

        internal static SafetyConfigKukaKRC4 ReadXmlFileKUKA(string xmlFile)
        {
            List<SafeToolKUKA> safeTools = new List<SafeToolKUKA>();
            CellSpaceKuka cellspace = new CellSpaceKuka();
            List<SafeZoneKUKA> safeSpaces = new List<SafeZoneKUKA>();

            safeTools = GetSafeToolsKUKA(xmlFile);
            cellspace = GetCellSpaceKUKA(xmlFile);
            safeSpaces = GetSafeSpacesKUKA(xmlFile);
            string robotname = "";
            SafetyConfigKukaKRC4 result = new SafetyConfigKukaKRC4(safeTools, cellspace, safeSpaces, robotname);
            return result;
        }

        private static List<SafeZoneKUKA> GetSafeSpacesKUKA(string xmlFile)
        {
            List<string> safeSpaceStrings = GetSafeSpaceString(xmlFile);
            List<SafeZoneKUKA> result = new List<SafeZoneKUKA>();
            foreach (string safetyzone in safeSpaceStrings.Where(x => !x.Contains("<Activation>255")))
            {
                string name = "";
                int number = 0;
                float x=0, y = 0, z = 0, a = 0, b = 0, c = 0, x1 = 0, x2 = 0, y1 = 0, y2 = 0, z1 = 0, z2 = 0;
                Regex currentRegex;
                var reader = new StringReader(safetyzone);
                IDictionary<string, float> coordinates = new Dictionary<string, float>();
                while (true)
                {
                    var line = reader.ReadLine();
                    
                    if (line.Contains("<WorkspaceMonitoring Number="))
                    {
                        currentRegex = new Regex(@"((?<=<WorkspaceMonitoring Number=.)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        number = int.Parse(match.ToString());
                        currentRegex = new Regex(@"((?<=Name=.).*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        name = match.ToString();
                        name = name.Replace("\">", "");
                    }
                    if (line.Contains("<X>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        x = float.Parse(match.ToString());
                        coordinates.Add("x", x);
                    }
                    if (line.Contains("<Y>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        y = float.Parse(match.ToString());
                        coordinates.Add("y", y);
                    }
                    if (line.Contains("<Z>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        z = float.Parse(match.ToString());
                        coordinates.Add("z", z);
                    }
                    if (line.Contains("<A>"))
                    {
                        //currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        a = float.Parse(match.ToString());
                        coordinates.Add("a", a);
                    }
                    if (line.Contains("<B>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        b = float.Parse(match.ToString());
                        coordinates.Add("b", b);
                    }
                    if (line.Contains("<C>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        c = float.Parse(match.ToString());
                        coordinates.Add("c", c);
                    }
                    if (line.Contains("<X1>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        x1 = float.Parse(match.ToString());
                        coordinates.Add("x1", x1);
                    }
                    if (line.Contains("<X2>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        x2 = float.Parse(match.ToString());
                        coordinates.Add("x2", x2);
                    }
                    if (line.Contains("<Y1>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        y1 = float.Parse(match.ToString());
                        coordinates.Add("y1", y1);
                    }
                    if (line.Contains("<Y2>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        y2 = float.Parse(match.ToString());
                        coordinates.Add("y2", y2);
                    }
                    if (line.Contains("<Z1>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        z1 = float.Parse(match.ToString());
                        coordinates.Add("z1", z1);
                    }
                    if (line.Contains("<Z2>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        Match match = currentRegex.Match(line);
                        z2 = float.Parse(match.ToString());
                        coordinates.Add("z2", z2);
                    }

                    if (line.Contains("</WorkspaceMonitoring>"))
                    {
                        PointEuler origin = new PointEuler(x + x1, y + y1, z + z1, a, b, c);
                        PointEuler max = GetSafeZoneMax(coordinates);
                        SafeZoneKUKA currentZone = new SafeZoneKUKA(number, name, origin, max, GetPointsAtSafeZoneBase(origin,max));
                        result.Add(currentZone);
                        break;
                    }

                }
            }
            return result;
        }

        private static List<PointEuler> GetPointsAtSafeZoneBase(PointEuler origin, PointEuler max)
        {
            //double dx = origin.Xpos - max.Xpos;
            //double dy = origin.Ypos - max.Ypos;
            //double distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            //double dalfa = Math.Atan(dy / dx) * (180 / Math.PI);
            //double alfa2 = 90 - origin.A - dalfa;


            List<PointEuler> result = new List<PointEuler>();
            result.Add(origin);
            PointEuler point2 = new PointEuler(0, 0, origin.Zpos, 0, 0, 0);
            PointEuler point3 = new PointEuler(max.Xpos, max.Ypos, origin.Zpos, 0, 0, 0);
            return result;
        }

        private static PointEuler GetSafeZoneMax(IDictionary<string, float> coordinates)
        {
            float dx = coordinates["x2"] - (coordinates["x1"] + coordinates["x"]);
            float dy = coordinates["y2"] - (coordinates["y1"] + coordinates["y"]);
            float dz = coordinates["z2"] - (coordinates["z1"] + coordinates["z"]);

            double dalfa = Math.Atan(dy / dx)*(180/Math.PI);            
            double alfa2 = 90 - coordinates["a"] - dalfa;
            double c = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            double ddy = (Math.Cos(Math.PI * alfa2 / 180) * c);
            double ddx = (Math.Sin(Math.PI * alfa2 / 180) * c);


            double xresult = ddx + (coordinates["x"] + coordinates["x1"]);
            double yresult = ddy + (coordinates["y"] + coordinates["y1"]);


            //PointEuler result = new PointEuler((float)xresult,(float)yresult,dz+coordinates["z"],0,0,0);
            PointEuler result = new PointEuler((float)xresult, (float)yresult, dz + coordinates["z"] + coordinates["z1"], 0, 0, 0);
            return result;

        }

        private static Point GetSafeZoneMax3d(IDictionary<string, float> coordinates)
        {
            //TODO
            float dx = coordinates["x2"] - coordinates["x1"];
            float dy = coordinates["y2"] - coordinates["y1"];
            float dz = coordinates["z2"] - coordinates["z1"];

            double dalfa = Math.Atan(dy / dx) * (180 / Math.PI);
            double alfa2 = 90 - coordinates["a"] - dalfa;
            double c = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dz,2));
            double ddy = (Math.Cos(Math.PI * alfa2 / 180) * c);
            double ddx = (Math.Sin(Math.PI * alfa2 / 180) * c);


            double xresult = ddx + coordinates["x"];
            double yresult = ddy + coordinates["y"];


            Point result = new Point((float)xresult, (float)yresult, 0, 1, 0, 0, 0);
            return result;

        }
        private static List<string> GetSafeSpaceString(string xmlFile)
        {
            List<string> resultStrings = new List<string>();
            bool addLine = false;
            string currentString = "";
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<WorkspaceMonitoring Number="))
                {
                    addLine = true;
                }
                if (addLine)
                {
                    currentString = currentString + line + "\n";
                }
                if (addLine & line.Contains("</WorkspaceMonitoring>"))
                {
                    addLine = false;
                    resultStrings.Add(currentString);
                    currentString = "";
                }
            }
            reader.Close();
            return resultStrings;
        }

        private static CellSpaceKuka GetCellSpaceKUKA(string xmlFile)
        {
            Regex currentRegex;
            Match match;
            string cellspaceString = GetCellSpaceStringKUKA(xmlFile);
            float bottom = 0,top = 0;
            bottom = GetCellSpaceTopOrBottom(cellspaceString, "<Zmin>");
            top = GetCellSpaceTopOrBottom(cellspaceString, "<Zmax>");
            List<string> polygons = GetCellSpacePoints(cellspaceString);
            List<PointEuler> cellSpacePoints = new List<PointEuler>();

            foreach (string polygon in polygons)
            {
                float x = 0, y = 0;
                var reader = new StringReader(polygon);
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("<X>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        x = float.Parse(match.ToString());
                    }
                    if (line.Contains("<Y>"))
                    {
                        currentRegex = new Regex(@"(-[0-9]*\.[0-9]*)|((?<=>)[0-9]*\.[0-9]*)|(-[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                        match = currentRegex.Match(line);
                        y = float.Parse(match.ToString());
                    }
                    if (line.Contains("</Polygon>"))
                    {
                        cellSpacePoints.Add(new PointEuler(x, y, bottom, 0, 0, 0));
                        break;
                    }                        
                }
            }

            CellSpaceKuka cellSpace = new CellSpaceKuka(cellSpacePoints, top);
            return cellSpace;
        }

        private static float GetCellSpaceTopOrBottom(string cellspaceString, string searchstring)
        {
            float result = 0;
            var reader = new StringReader(cellspaceString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line.Contains(searchstring))
                {
                    Regex currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    result = float.Parse(match.ToString());
                    break;
                }
            }
            return result;
        }


        private static List<string> GetCellSpacePoints(string cellspaceString)
        {
            bool addLine = false;
            string currentString = "";
            List<string> result = new List<string>();
            var reader = new StringReader(cellspaceString);
            while (true)
            {
                var line = reader.ReadLine();
                if (line.Contains("<Polygon "))
                {
                    addLine = true;
                }
                if (addLine)
                    currentString = currentString + line + "\n";

                if (line.Contains("</Polygon>"))
                {
                    if (!currentString.Contains("<IsPolygonNodeActive>0"))
                    result.Add(currentString);
                    currentString = "";
                    addLine = false;
                }
                if (line.Contains("</CellSpace>"))
                    break;
            }
            return result;
        }

        private static string GetCellSpaceStringKUKA(string xmlFile)
        {
            string result = "";
            bool addLine = false;
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<CellSpace>"))
                    addLine = true;
                if (addLine)
                    result = result + line + "\n";
                if (line.Contains("</CellSpace>"))
                    break;
            }
            return result;
        }

        private static List<SafeToolKUKA> GetSafeToolsKUKA(string xmlFile)
        {
            List<string> toolStrings = new List<string>();
            List<SafeToolKUKA> resultTools = new List<SafeToolKUKA>();
            toolStrings = FindSafeToolStringsKUKA(xmlFile);
            foreach (string toolString in toolStrings.Where(x=>x.Contains("<ToolEnabled>1")))
            {
                SafeToolKUKA safeTool = new SafeToolKUKA();
                safeTool = GetToolFromToolsStringKUKA(toolString);
                resultTools.Add(safeTool);
            }
            return resultTools;

            #endregion
        }

        private static List<string> FindSafeToolStringsKUKA(string xmlFile)
        {
            List<string> resultStrings = new List<string>();
            bool addLine = false;
            string currentString = "";
            var reader = new StreamReader(xmlFile);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Contains("<Tool Number="))
                {
                    addLine = true;
                }
                if (addLine)
                {
                    currentString = currentString + line + "\n";
                }
                if (addLine & line.Contains("</Tool>"))
                {
                    addLine = false;
                    resultStrings.Add(currentString);
                    currentString = "";
                }
            }
            reader.Close();
            return resultStrings;
        }

        private static SafeToolKUKA GetToolFromToolsStringKUKA(string toolString)
        {
            Regex currentRegex;
            int number = 0;
            string name = "";
            Point currentPoint = new Point();
            List<string> foundValues = new List<string>(); ;
            List<SphereKuka> resultSpheres = GetSphereFromToolStringKUKA(toolString);

            var reader = new StringReader(toolString);
            while (true)
            {
                var line = reader.ReadLine();

                if (line.Contains("<Tool Number"))
                {
                    currentRegex = new Regex(@"((?<=<Tool Number=.)[0-9]*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    number = int.Parse(match.ToString());
                }
                if (line.Contains("Name="))
                {
                    currentRegex = new Regex(@"((?<=Name=.).*)", RegexOptions.IgnoreCase);
                    Match match = currentRegex.Match(line);
                    name = match.ToString();
                    name = name.Replace("\">", "");
                }
                if (line.Contains("</Tool>"))
                    break;
            }
            reader.Close();
            SafeToolKUKA resultTool = new SafeToolKUKA(number, name, new PointEuler(0, 0, 0, 0, 0, 0), resultSpheres);
            return resultTool;

        }

        private static List<SphereKuka> GetSphereFromToolStringKUKA(string toolString)
        {
            {
                List<string> sphereStrings = GetSphereStringsKUKA(toolString);
                List<SphereKuka> resultSpheres = new List<SphereKuka>();
                List<string> pointStrings = new List<string>();
                int number = 0;
                float radius = 0;

                foreach (string sphereString in sphereStrings)
                {
                    Regex currentRegex;
                    Match match;
                    bool addItem = true;
                    var reader = new StringReader(sphereString);
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line.Contains("<Sphere Number"))
                        {
                            currentRegex = new Regex(@"((?<=<Sphere Number=.)[0-9]*)", RegexOptions.IgnoreCase);
                            match = currentRegex.Match(line);
                            number = int.Parse(match.ToString());
                        }
                        if (line.Contains("<Radius>"))
                        {
                            currentRegex = new Regex(@"((?<=>)[0-9]*\.[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                            match = currentRegex.Match(line);
                            radius = (float.Parse(match.ToString()));
                        }

                        if (line.Contains("<X>") | line.Contains("<Y>") | line.Contains("<Z>"))
                        {
                            currentRegex = new Regex(@"(-[0-9]*)|((?<=_)[0-9]*)|((?<=>)[0-9]*)", RegexOptions.IgnoreCase);
                            pointStrings.Add(currentRegex.Match(line).ToString());
                        }
                        if (line.Contains("</Sphere>"))
                        {
                            PointEuler centerPoint = new PointEuler(float.Parse(pointStrings[0]), float.Parse(pointStrings[1]), float.Parse(pointStrings[2]), 0, 0, 0);
                            SphereKuka currentBox = new SphereKuka(number, centerPoint, radius);
                            if (addItem)
                                resultSpheres.Add(currentBox);
                            pointStrings.Clear();
                            break;
                        }
                    }

                }
                return resultSpheres;
            }
        }

        private static List<string> GetSphereStringsKUKA(string toolString)
        {
            List<string> sphereStrings = new List<string>();
            bool addLine = false, addSphere = true;
            string currentString = "";
            var reader = new StringReader(toolString);
            while (true)
            {
                var line = reader.ReadLine();

                if (line.Contains("<SphereEnabled>0"))
                {
                    addSphere = false;
                }
                if (line.Contains("<Sphere Number"))
                {
                    addLine = true;
                }
                if (addLine)
                    currentString = currentString + line + "\n";
                if (line.Contains("</Sphere>"))
                {
                    if (addSphere)
                        sphereStrings.Add(currentString);
                    addSphere = true;
                    currentString = "";
                }
                if (line.Contains("</Tool>"))
                    break;
            }
            reader.Close();
            return sphereStrings;
        }

        internal static IDictionary<string, string> BuildSrcDat(SafetyConfigKukaKRC4 configKukaKRC4)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            result.Add("src", BuildSrcFile(configKukaKRC4));
            result.Add("dat", BuildDatFile(configKukaKRC4));
            return result;
        }

        private static string BuildDatFile(SafetyConfigKukaKRC4 configKukaKRC4)
        {
            string result = String.Join(
                Environment.NewLine,
                "DEFDAT Safety",
                ";FOLD EXTERNAL DECLARATIONS;%{PE}%MKUKATPBASIS,%CEXT,%VCOMMON,%P",
                ";FOLD BASISTECH EXT;%{PE}%MKUKATPBASIS,%CEXT,%VEXT,%P",
                "EXT  BAS (BAS_COMMAND  :IN,REAL  :IN )",
                "DECL INT SUCCESS",
                ";ENDFOLD (BASISTECH EXT)",
                ";FOLD USER EXT;%{E}%MKUKATPUSER,%CEXT,%VEXT,%P",
                ";Make your modifications here",
                ";ENDFOLD (USER EXT)",
                ";ENDFOLD (EXTERNAL DECLARATIONS)",
                ";# --------- START PATH : Safety ---------"
                );
            int counter = 1;
            foreach (PointEuler point in configKukaKRC4.CellSpace.Points)
            {
                string currentString = BuildDat("cs" + counter.ToString(), point);
                result = result + "\n" + currentString;
                counter++;

                if (counter-1 == configKukaKRC4.CellSpace.Points.Count)
                {
                    PointEuler csmax = new PointEuler();
                    csmax = point;
                    csmax.Zpos = configKukaKRC4.CellSpace.Top;
                    result = result + "\n" + BuildDat("cs_max",csmax);
                }
            }

            foreach (SafeToolKUKA safetool in configKukaKRC4.SafeTools)
            {
                foreach (var point in safetool.Spheres)
                {
                    string spherename = "ts" + safetool.Number.ToString() + "_sphere" + point.Number.ToString() + "_" + point.Radius.ToString();
                    string currentString = BuildDat(spherename,point.CenterPoint);
                    result = result + "\n" + currentString;
                }
            }

            foreach (SafeZoneKUKA zone in configKukaKRC4.SafeZones)
            {
                string origin = "sp" + zone.Number.ToString() + "_origin";
                string max = "sp" + zone.Number.ToString() + "_max";
                result = result + "\n" + BuildDat(origin, zone.Origin);
                result = result + "\n" + BuildDat(max, zone.Max);
            }

            result = result + "\nENDDAT\n\r";
            return result;
        }

        private static string BuildDat(string name, PointEuler point)
        {
            string result = String.Join(
                Environment.NewLine,
                "DECL E6POS X" + name + "={X " + point.Xpos + ",Y " + point.Ypos + ",Z " + point.Zpos + ",A " + point.A + ",B " + point.B +",C "+point.C+"}",
                "DECL FDAT F"+name+"={TOOL_NO 0,BASE_NO 0,IPO_FRAME #BASE,POINT2[] \" \",TQ_STATE FALSE}",
                "DECL PDAT PP"+name+"={VEL 100,ACC 100,APO_DIST 0,GEAR_JERK 50}"
                );
            return result;
        }

        private static string BuildSrcFile(SafetyConfigKukaKRC4 configKukaKRC4)
        {
            string result = String.Join(
                Environment.NewLine,
                "&COMMENT ",
                "DEF  Safety( )",
                ";FOLD Moduleparameters;%{h}",
                ";Params ",
                ";ENDFOLD Moduleparameters",
                ";FOLD INI",
                ";FOLD BASISTECH INI",
                ";FOLD IF_PLC_CHK_INIT",
                "IF PLC_CHK_INIT() THEN",
                ";ENDFOLD (IF_PLC_CHK_INIT)",
                "GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM ( )",
                "INTERRUPT ON 3 ",
                "BAS (#INITMOV,0 )",
                ";ENDFOLD (BASISTECH INI)",
                ";FOLD USER INI",
                ";Make your modifications here",
                ";FOLD APPLICATION_INI",
                "APPLICATION_INI ( )",
                ";ENDFOLD (APPLICATION_INI)",
                ";ENDFOLD (USER INI)",
                ";FOLD ENDIF_PLC_CHK_INIT",
                "ENDIF",
                ";ENDFOLD (ENDIF_PLC_CHK_INIT)",
                ";ENDFOLD (INI)",
                ";# --------- START PATH : Safety ---------"
                );

            int counter = 1;
            foreach (PointEuler point in configKukaKRC4.CellSpace.Points)
            {
                string currentString = BuildPTP("cs" + counter.ToString());
                result = result + "\r\n" + currentString;
                counter++;
            }

            result = result + "\n" + BuildPTP("cs_max");

            foreach (SafeToolKUKA safetool in configKukaKRC4.SafeTools)
            {
                foreach (var point in safetool.Spheres)
                {
                    string spherename = "ts" + safetool.Number.ToString() + "_sphere" + point.Number.ToString() + "_" + point.Radius.ToString();
                    string currentString = BuildPTP(spherename);
                    result = result + "\r\n" + currentString;
                }
            }

            foreach (SafeZoneKUKA zone in configKukaKRC4.SafeZones)
            {
                string origin = "sp" + zone.Number.ToString() + "_origin";
                string max = "sp" + zone.Number.ToString() + "_max";
                result = result + "\r\n" + BuildPTP(origin);
                result = result + "\r\n" + BuildPTP(max);
            }

            result = result + "\nEND\n\r";
            return result;
        }

        public static string BuildPTP(string inputFile)
        {
            string result = String.Join(
                            Environment.NewLine,
                            ";FOLD PTP " + inputFile + " Vel=100 % P" + inputFile + " Tool[0] Base[0];%{PE}%R 8.3.21,%MKUKATPBASIS,%CMOVE,%VPTP,%P 1:PTP, 2:" + inputFile + " , 3:, 5:100, 7:P" + inputFile,
                            "$BWDSTART = FALSE",
                            "PDAT_ACT= PP" + inputFile,
                            "FDAT_ACT= F" + inputFile,
                            "BAS(#PTP_PARAMS,100)",
                            "PTP X" + inputFile,
                            ";ENDFOLD"
                            );
            return result;
        }
    }


}
