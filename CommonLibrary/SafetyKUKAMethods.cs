using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public static class SafetyKUKAMethods
    {
        public static SafetyConfig GetSafetyConfig(string directory)
        {
            string safetyConfigFile = directory + "\\C\\KRC\\HMI\\PlugIns\\SafeRobot\\Config\\KUKASafeRobot.config";
            if (File.Exists(safetyConfigFile) && CheckIfSafeRobot(safetyConfigFile))
            {
                string cellspaceString = GetCellSpaceString(safetyConfigFile);
                List<string> safeSpacesStrings = GetSafeSpaceStrings(safetyConfigFile);
                Cellspace cellspace = GetCellspaceCoords(cellspaceString);
                List<ISafeSpace> safeSpaces = GetSafeSpacesCoord(safeSpacesStrings);
                return new SafetyConfig(cellspace, safeSpaces);
            }

            return null;
        }

        private static string GetCellSpaceString(string safetyConfigFile)
        {
            string result = "";
            bool addLine = false;
            StreamReader reader = new StreamReader(safetyConfigFile);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("<cellspace>"))
                    addLine = true;
                if (addLine)
                    result += line + "\r\n";
                if (line.ToLower().Contains("</cellspace>"))
                    break;
            }
            reader.Close();
            return result;
        }

        private static List<string> GetSafeSpaceStrings(string safetyConfigFile)
        {
            List<string> result = new List<string>();
            string currentString = "";
            bool addLine = false;
            StreamReader reader = new StreamReader(safetyConfigFile);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("<workspacemonitoring number"))
                    addLine = true;
                if (addLine)
                    currentString += line + "\r\n";
                if (line.ToLower().Contains("</workspacemonitoring>"))
                {
                    result.Add(currentString);
                    currentString = "";
                    addLine = false;
                }
                if (line.ToLower().Contains("</rangemonitoring"))
                    break;
            }
            reader.Close();
            return result;
        }

        private static bool CheckIfSafeRobot(string safetyConfigFile)
        {
            StreamReader reader = new StreamReader(safetyConfigFile);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("<saferobotdisabled"))
                {
                    if (line.ToLower().Contains("false"))
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        private static Cellspace GetCellspaceCoords(string cellspaceString)
        {
            string previousSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (!CultureInfo.CurrentCulture.IsReadOnly)               
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ",";
            Cellspace result = new Cellspace();
            result.Points = new Dictionary<int, PointInSafety>();
            Regex findNumber = new Regex(@"(?<=>)((-\d+\.\d+)|(\d+\.\d+)|(\d+)|(-\d+))", RegexOptions.IgnoreCase);
            double currentX = 9999999, currentY = 0;
            int counter = 1;
            bool bothValuesFound = false;
            StringReader reader = new StringReader(cellspaceString);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (line.ToLower().Contains("<x unit="))
                {
                    currentX = double.Parse((findNumber.Match(line).ToString().Replace(".", ",")));
                    bothValuesFound = false;
                }
                if (line.ToLower().Contains("<y unit="))
                {
                    currentY = double.Parse((findNumber.Match(line).ToString().Replace(".", ",")));
                    bothValuesFound = true;
                }
                if (line.ToLower().Contains("<ispolygonnodeactive"))
                {
                    if (line.ToLower().Contains("true") && bothValuesFound)
                    {
                        result.Points.Add(counter, new PointInSafety(currentX, currentY, 0));
                        counter++;
                    }
                    bothValuesFound = false;
                    currentX = 0;
                    currentY = 0;
                }

            }
            reader.Close();
            if (!CultureInfo.CurrentCulture.IsReadOnly)
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = previousSeparator;
            return result;
        }

        private static List<ISafeSpace> GetSafeSpacesCoord(List<string> safeSpacesStrings)
        {
            string previousSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (!CultureInfo.CurrentCulture.IsReadOnly)
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = ",";
            List<ISafeSpace> result = new List<ISafeSpace>();
            Regex getNumber = new Regex(@"\d+", RegexOptions.IgnoreCase);
            Regex findCoordinate = new Regex(@"(?<=>)((-\d+\.\d+)|(\d+\.\d+)|(\d+)|(-\d+))", RegexOptions.IgnoreCase);
            foreach (string safeSpacestring in safeSpacesStrings)
            {
                int currentNumber = 0;
                bool used = false;
                double x1 = 0, x2 = 0, y1 = 0, y2 = 0, z1 = 0, z2 = 0;
                StringReader reader = new StringReader(safeSpacestring);
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (line.ToLower().Contains("<workspacemonitoring number"))
                        currentNumber = int.Parse(getNumber.Match(line).ToString());
                    if (line.ToLower().Contains("<isprotectedspace"))
                        if (line.ToLower().Contains("true"))
                            used = true;
                    if (line.ToLower().Contains("<x1 unit"))
                        x1 = double.Parse((findCoordinate.Match(line).ToString().Replace(".", ",")));
                    if (line.ToLower().Contains("<x2 unit"))
                        x2 = double.Parse((findCoordinate.Match(line).ToString().Replace(".", ",")));
                    if (line.ToLower().Contains("<y1 unit"))
                        y1 = double.Parse((findCoordinate.Match(line).ToString().Replace(".", ",")));
                    if (line.ToLower().Contains("<y2 unit"))
                        y2 = double.Parse((findCoordinate.Match(line).ToString().Replace(".", ",")));
                    if (line.ToLower().Contains("<z1 unit"))
                        z1 = double.Parse((findCoordinate.Match(line).ToString().Replace(".", ",")));
                    if (line.ToLower().Contains("<z2 unit"))
                        z2 = double.Parse((findCoordinate.Match(line).ToString().Replace(".", ",")));
                }
                if (used)
                {
                    SafeSpace2points currentSafetySpace = new SafeSpace2points(currentNumber, new PointInSafety(x1, y1, z1), new PointInSafety(x2, y2, z2));
                    result.Add(currentSafetySpace);
                }
                reader.Close();
            }
            if (!CultureInfo.CurrentCulture.IsReadOnly)
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = previousSeparator;
            return result;
        }
    }
}
