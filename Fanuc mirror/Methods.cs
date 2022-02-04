using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLibrary;

namespace Fanuc_mirro
{
    public static class Methods
    {
        public static string toolsToMirrorString;

        internal static CompleteMirror ReadLSFile(string text, bool mirrorWorkbook, int renumberStep, bool removeHeader)
        {
            bool workbookFound = false;
            List<FileAndPath> resultPaths = new List<FileAndPath>();
            FileAndPath result = new FileAndPath();
            FileAndPath workbook = new FileAndPath();
            DirectoryInfo d = new DirectoryInfo(text);
            foreach (var file in d.GetFiles("*.ls"))
            {
                result = MirrorString(file,false, renumberStep);
                if (removeHeader)
                    result = RemoveHeader(result);
                if (result == null)
                    return null;
                resultPaths.Add(result);
            }
            if (mirrorWorkbook)
            {
                foreach (var file in d.GetFiles("workbook.xvr"))
                {
                    workbookFound = true;
                    workbook = MirrorString(file,true, renumberStep);
                }
                if (!workbookFound)
                    MessageBox.Show("No Workbook.xvr found!");
            }

            CompleteMirror results;
            if (mirrorWorkbook)
                results = new CompleteMirror(resultPaths, workbook);
            else
                results = new CompleteMirror(resultPaths);
            return results ;
        }

        private static FileAndPath RemoveHeader(FileAndPath file)
        {
            Regex isOldHeaderPart = new Regex(@"^\s*\d+\s*\:\s*!\s*\*", RegexOptions.IgnoreCase);
            Regex commentRegex = new Regex(@"^\s*\d+\s*\:\s*!", RegexOptions.IgnoreCase);
            Regex emptyLineRegex = new Regex(@"^\s*\d+\s*:\s*;", RegexOptions.IgnoreCase);
            StringReader reader = new StringReader(file.Path);
            string resultString = string.Empty;
            List<string> initialLines = new List<string>();
            List<string> linesToRenumber = new List<string>();

            bool isComment = false, isHeaderEnd = false;
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (!emptyLineRegex.IsMatch(line))
                {
                    if (commentRegex.IsMatch(line))
                        isComment = true;
                    else
                    {
                        if (isComment)
                            isHeaderEnd = true;
                        isComment = false;
                    }
                    if (!isHeaderEnd && !isComment)
                        initialLines.Add(line);
                    if (!isComment && isHeaderEnd && !string.IsNullOrEmpty(line.Trim()))
                        linesToRenumber.Add(line);
                    //resultString += line + "\r\n";
                }
            }
            List<string> renumberedLines = CommonMethods.GetRenumberedBody(linesToRenumber);
            initialLines.ForEach(x => resultString += x + "\r\n");
            renumberedLines.ForEach(x => resultString += x + "\r\n");

            return new FileAndPath(file.FileName, resultString);
        }

        private static FileAndPath MirrorString(FileInfo file, bool isWorkbook, int renumberStep)
        {
            Regex processpointRegex = new Regex("(?<=(P\\s*\\[\\s*\\d+\\s*:\\s*(|\")(swp|LSB|scr)|\"\\s*(SpotNo|BeadNo|SeamNumber|SeamNo|ScrewNo)\\s*\"\\s*\\=\\s*))\\d+", RegexOptions.IgnoreCase);
            Regex numbers = new Regex(@"\d+");
            Regex gluePtNumRegex = new Regex(@"(?<=^\s*\d+\s*:\s*\w\s+\w\s*\[\s*\d+\s*:\s*[a-zA-Z_-]*)\d+", RegexOptions.IgnoreCase);
            Regex pointNameinDefSection = new Regex("(?<=^\\s*\\w\\s*\\[\\s*\\d+\\s*:\\s*\"\\s*)[\\w_-]+", RegexOptions.IgnoreCase);
            Regex isMutool = new Regex(@"(?<=^\s*<\s*ARRAY.*\$MNUTOOL\s*\[\s*\d+\s*,\s*)\d+", RegexOptions.IgnoreCase);
            Regex isMnuframe = new Regex(@"(?<=^\s*<\s*ARRAY.*\$MNUFRAME\s*\[\s*\d+\s*,\s*)\d+", RegexOptions.IgnoreCase);
            Regex isPosRegStartRegex = new Regex(@"^\s*<.*\$POSREG", RegexOptions.IgnoreCase);
            Regex isPosRegStopRegex = new Regex(@"^\s*<\s*/\s*VAR\s*>", RegexOptions.IgnoreCase);
            Regex joint123Regex = new Regex(@"(?<=J(1|2|3)\s*=\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex joint456Regex = new Regex(@"(?<=J(4|5|6)\s*=\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex joint4Regex = new Regex(@"(?<=J4\s*=\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            Regex joint6Regex = new Regex(@"(?<=J6\s*=\s*)(-\d+\.\d+|-\d+|\d+\.\d+|\d+)", RegexOptions.IgnoreCase);
            string arg1, arg2, regex1, regex2;
            List<int> mirrorTools = new List<int>();
            List<string> gluePonts = new List<string>();
            var matches = numbers.Matches(toolsToMirrorString);
            if (isWorkbook)
            {
                foreach (var match in matches)
                {
                    mirrorTools.Add(int.Parse(match.ToString()));
                }
                arg1 = "Y: ";
                arg2 = "P: ";
                regex1 = @"(?<=(X|Y|Z)\s*\:\s*)[\w-+\.e]*";
                regex2 = @"(?<=(W|P|R)\s*\:\s*)[\w-+\.e]*";
                //regex1 = @"((?<=X.*)-[0-9]*\.[0-9]*)|((?<=X.*)[0-9]*\.[0-9]*)|((?<=X.*)-[0-9]*)|((?<=X.*)[0-9]*)|((?<=Y.*)-[0-9]*\.[0-9]*)|((?<=Y.*)[0-9]*\.[0-9]*)|((?<=Y.*)-[0-9]*)|((?<=Y.*)[0-9]*)|((?<=Z.*)-[0-9]*\.[0-9]*)|((?<=Z.*)[0-9]*\.[0-9]*)|((?<=Z.*)-[0-9]*)|((?<=Z.*)[0-9]*)";
                //regex2 = @"((?<=W.*)-[0-9]*\.[0-9]*)|((?<=W.*)[0-9]*\.[0-9]*)|((?<=W.*)-[0-9]*)|((?<=W.*)[0-9]*)|((?<=P.*)-[0-9]*\.[0-9]*)|((?<=P.*)[0-9]*\.[0-9]*)|((?<=P.*)-[0-9]*)|((?<=P.*)[0-9]*)|((?<=R.*)-[0-9]*\.[0-9]*)|((?<=R.*)[0-9]*\.[0-9]*)|((?<=R.*)-[0-9]*)|((?<=R.*)[0-9]*)";
            }
            else
            {
                arg1 = "Y = ";
                arg2 = "W = ";
                //regex1 = @"((?<=X.*\=.*)-[0-9]*\.[0-9]*)|((?<=X.*\=.*)[0-9]*\.[0-9]*)|((?<=X.*\=.*)-[0-9]*)|((?<=X.*\=.*)[0-9]*)|((?<=Y.*\=.*)-[0-9]*\.[0-9]*)|((?<=Y.*\=.*)[0-9]*\.[0-9]*)|((?<=Y.*\=.*)-[0-9]*)|((?<=Y.*\=.*)[0-9]*)|((?<=Z.*\=.*)-[0-9]*\.[0-9]*)|((?<=Z.*\=.*)[0-9]*\.[0-9]*)|((?<=Z.*\=.*)-[0-9]*)|((?<=Z.*\=.*)[0-9]*)";
                //regex2 = @"((?<=W.*\=.*)-[0-9]*\.[0-9]*)|((?<=W.*\=.*)[0-9]*\.[0-9]*)|((?<=W.*\=.*)-[0-9]*)|((?<=W.*\=.*)[0-9]*)|((?<=P.*\=.*)-[0-9]*\.[0-9]*)|((?<=P.*\=.*)[0-9]*\.[0-9]*)|((?<=P.*\=.*)-[0-9]*)|((?<=P.*\=.*)[0-9]*)|((?<=R.*\=.*)-[0-9]*\.[0-9]*)|((?<=R.*\=.*)[0-9]*\.[0-9]*)|((?<=R.*\=.*)-[0-9]*)|((?<=R.*\=.*)[0-9]*)";
                regex1 = @"(?<=(X|Y|Z)\s*\=\s*)[\w-+\.e]*";
                regex2 = @"(?<=(W|P|R)\s*\=\s*)[\w-+\.e]*";
            }
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            string resultString ="";
            Regex regex;
            float X = 0, YInvert = 0, Z = 0, WInvert = 0, P =0,RInvert = 0;
            string currentLine = "";
            List<string> foundValues = new List<string>();
            var reader = new StreamReader(file.FullName);
            bool modifyLine = false, isPosReg = false, isTool = false, isUframe = false;
            int currentToolNum = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (isMutool.IsMatch(line))
                {
                    isTool = true;
                    currentToolNum = int.Parse(isMutool.Match(line).ToString());
                }
                if (isMnuframe.IsMatch(line))
                {
                    isTool = false;
                    currentToolNum = 0;
                }
                if (isPosRegStartRegex.IsMatch(line))
                    isPosReg = true;
                if (isPosRegStopRegex.IsMatch(line))
                    isPosReg = false;
                if (isWorkbook && joint123Regex.IsMatch(line))
                {
                    double j1modified = double.Parse(joint123Regex.Matches(line)[0].ToString(), CultureInfo.InvariantCulture);
                    currentLine = joint123Regex.Replace(line, (-j1modified).ToString(nfi), 1, 0) + "\r\n";
                    modifyLine = true;
                }
                if (isWorkbook && joint456Regex.IsMatch(line) && !modifyLine)
                {
                    double j4modified = double.Parse(joint456Regex.Matches(line)[0].ToString(), CultureInfo.InvariantCulture);
                    double j6modified = double.Parse(joint456Regex.Matches(line)[2].ToString(), CultureInfo.InvariantCulture);
                    currentLine = joint4Regex.Replace(line, (-j4modified).ToString(nfi));
                    currentLine = joint6Regex.Replace(currentLine, (-j6modified).ToString(nfi)) + "\r\n";
                    modifyLine = true;
                }
                if (line.ToLower().Replace(" ","").Contains("config:") && !isPosReg && !modifyLine)
                {
                    if (isTool && mirrorTools.Contains(currentToolNum) || !isTool)
                    {
                        regex = new Regex(@"^.*CONFIG\s*:\s*", RegexOptions.IgnoreCase);
                        Match m = regex.Match(line);
                        string templine = m.ToString();
                        foundValues = new List<string>();
                        //regex = new Regex(@"((?<=CONFIG :..).(?=\ ))")|;
                        //regex = new Regex(@"((?<=CONFIG.*).(?=\ ))|((?<= CONFIG.*).(?=\,))");
                        //string configString = (new Regex(@"(?<=')[\w\d-]*", RegexOptions.IgnoreCase)).Match(line.Replace(" ", "").Replace(",", "").Replace("\t", "")).ToString();
                        string configString = (new Regex(@"(?<=CONFIG\s*:\s*)[',\s\w_-]*", RegexOptions.IgnoreCase)).Match(line.Replace(" ", "").Replace(",", "").Replace("\t", "").Replace("'", "")).ToString();
                        regex = new Regex(@"(\w|-\d|\d)", RegexOptions.IgnoreCase);
                        foreach (Match currentMatch in regex.Matches(configString))
                        {
                            foundValues.Add(currentMatch.ToString());
                        }
                        if (foundValues.Count != 6)
                        {
                            MessageBox.Show("Niewłaściwa liczba argumentów w konfiguracji osi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                        RobotConf conf = new RobotConf(foundValues[0], foundValues[1], foundValues[2], int.Parse(foundValues[3]), int.Parse(foundValues[4]), int.Parse(foundValues[5]));
                        //if (conf.Arg1 == "N")
                        //    conf.Arg1 = "F";
                        //else if (conf.Arg1 == "F")
                        //    conf.Arg1 = "N";
                        if (conf.Arg4 != 0)
                            conf.Arg4 = -conf.Arg4;
                        if (conf.Arg5 != 0)
                            conf.Arg5 = -conf.Arg5;
                        if (conf.Arg6 != 0)
                            conf.Arg6 = -conf.Arg6;
                        currentLine = templine + (isWorkbook ? "" : "'") + conf.Arg1 + " " + conf.Arg2 + " " + conf.Arg3 + "," + conf.Arg4.ToString() + "," + conf.Arg5.ToString() + "," + conf.Arg6.ToString() + (isWorkbook ? "" : "',") + "\r\n";
                        modifyLine = true;
                    }
                }
                if (line.Contains(arg1))
                {
                    if (isTool && mirrorTools.Contains(currentToolNum) || !isTool)
                    {
                        foundValues = new List<string>();
                        regex = new Regex(regex1);
                        foreach (Match currentMatch in regex.Matches(line))
                        {
                            foundValues.Add(currentMatch.ToString());
                        }
                        foundValues = Methods.Filter(foundValues);
                        X = float.Parse(foundValues[0], CultureInfo.InvariantCulture);
                        YInvert = -float.Parse(foundValues[1], CultureInfo.InvariantCulture);
                        Z = float.Parse(foundValues[2], CultureInfo.InvariantCulture);
                        if (isWorkbook)
                            currentLine = "  X:   " + X.ToString(nfi) + "   Y:  " + YInvert.ToString(nfi) + "   Z:   " + Z.ToString(nfi) + "\n";
                        else
                            currentLine = "        X = " + X.ToString(nfi) + " mm,    Y = " + YInvert.ToString(nfi) + " mm,    Z = " + Z.ToString(nfi) + " mm,\n";
                        modifyLine = true;
                    }
                }
                if (line.Contains(arg2))
                {
                    if (isTool && mirrorTools.Contains(currentToolNum) || !isTool)
                    {
                        foundValues = new List<string>();
                        regex = new Regex(regex2);
                        foreach (Match currentMatch in regex.Matches(line))
                        {
                            foundValues.Add(currentMatch.ToString());
                        }
                        foundValues = Methods.Filter(foundValues);
                        WInvert = -float.Parse(foundValues[0], CultureInfo.InvariantCulture);
                        P = float.Parse(foundValues[1], CultureInfo.InvariantCulture);
                        RInvert = -float.Parse(foundValues[2], CultureInfo.InvariantCulture);
                        if (isWorkbook)
                            currentLine = "  W:   " + WInvert.ToString(nfi) + "   P:  " + P.ToString(nfi) + "   R:   " + RInvert.ToString(nfi) + "\r\n";
                        else
                            currentLine = "        W = " + (WInvert.ToString(nfi) + " deg,    P = " + P.ToString(nfi) + " deg,    R = " + RInvert.ToString(nfi) + " deg\n");
                        modifyLine = true;
                    }
                }
                if (pointNameinDefSection.IsMatch(line))
                {
                    if (gluePonts.Contains(pointNameinDefSection.Match(line).ToString()))
                    {
                        Regex replaceRegex = new Regex("(?<=^\\s*\\w\\s*\\[\\s*\\d+\\s*:\\s*\"\\s*[a-zA-Z_-]*)\\d+", RegexOptions.IgnoreCase);
                        string foundNum = replaceRegex.Match(line).ToString();
                        int numberLength = foundNum.Length;
                        int numberModified = int.Parse(foundNum) + renumberStep;
                        string numToStringToAdd = numberModified.ToString();
                        if (numberModified.ToString().Length != numberLength)
                        {
                            numToStringToAdd = FillStringWithZeros(numToStringToAdd, numberLength);
                        }
                        currentLine = replaceRegex.Replace(line, numToStringToAdd) + "\r\n";
                        modifyLine = true;
                    }
                }
                if (processpointRegex.IsMatch(line))
                {
                    Regex isGlueRegex = new Regex(@"^\s*\d+.*PR_CALL\s+GL_P", RegexOptions.IgnoreCase);
                    Regex gluPointNameRegex = new Regex(@"(?<=^\s*\d+\s*:\s*\w\s+\w\s*\[\s*\d+\s*:\s*)[\w_-]*", RegexOptions.IgnoreCase);

                    string foundNum = processpointRegex.Match(line).ToString();
                    //var temp = processpointRegex.Matches(line);
                    int numberLength = foundNum.Length;
                    int numberModified = int.Parse(foundNum) + renumberStep;
                    string numToStringToAdd = numberModified.ToString();
                    if (numberModified.ToString().Length != numberLength)
                    {
                        numToStringToAdd = FillStringWithZeros(numToStringToAdd, numberLength);
                    }
                    currentLine = processpointRegex.Replace(line, numToStringToAdd) + "\r\n";
                    if (isGlueRegex.IsMatch(line))
                    {
                        gluePonts.Add(gluPointNameRegex.Match(line).ToString());
                        currentLine = gluePtNumRegex.Replace(currentLine,numToStringToAdd);
                    }
                    modifyLine = true;
                }
                if (!modifyLine)
                {
                    currentLine = line+"\n";
                }

                resultString = resultString + currentLine;
                modifyLine = false;
            }
            reader.Close();
            FileAndPath result = new FileAndPath(file,resultString);
            
            return result;
        }

        private static string FillStringWithZeros(string numToStringToAdd, int numberLength)
        {
            string result = numToStringToAdd;
            while(result.Length != numberLength)
            {
                result = "0" + result;
            }

            return result;
        }

        private static List<string> Filter(List<string> foundValues)
        {
            List<string> result = new List<string>();
            foreach (string element in foundValues.Where(item=>item != ""))
            {
                if (element.ToLower().Contains("e-") || element.ToLower().Contains("e+"))
                    result.Add("0");
                else
                    result.Add(element);
            }
            return result;
        }
    }
}
