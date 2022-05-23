using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using RobotFilesEditor.Model.Operations.DataClass;

namespace RobotFilesEditor.Model.Operations.BackupSyntaxValidation
{
    public class FanucSyntaxValidator : IBackupSyntaxValidator
    {
        #region properties
        public List<SovBackupsPreparationResult> ErrorsFound { get; set; }
        #endregion

        #region fields
        private FanucSyntaxValidatorData inputData;
        private string mainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester", "Temp");
        private Regex isCommentRegex = new Regex(@"^\s*\d+\s*:\s*(//|\!)", RegexOptions.IgnoreCase);
        #endregion

        #region ctor

        public FanucSyntaxValidator(string filename)
        {
            ErrorsFound = new List<SovBackupsPreparationResult>();
            CheckMainDirExistsDir();
            inputData = ReadBackup(filename);
            CheckOpenAndClose("IF", "ENDIF");
            CheckLabelsAndJumps();
            CheckProcCalls();
            CheckAbortProgramPresent();
            CheckWorkbook();
            CheckSemicolons();
            CheckProcedureNaming();
            CheckLineNumbers();
            if (ErrorsFound.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("No problems found :-)", GlobalData.SovLogContentInfoTypes.OK));
        }
        #endregion

        #region methods
        private FanucSyntaxValidatorData ReadBackup(string filename)
        {
            Dictionary<string, List<string>> lsFiles = new Dictionary<string, List<string>>();
            List<string> workbookContent = new List<string>();
            using (FileStream zipToOpen = new FileStream(filename, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    var workbook = archive.Entries.Single(x => x.FullName.ToLower().Contains("workbook") && Path.GetExtension(x.FullName).ToLower() == ".xvr");
                    StreamReader reader = new StreamReader(workbook.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        workbookContent.Add(line);
                    }
                    reader.Close();

                    ZipArchiveEntry asciiEntry = archive.Entries.Single(x => x.FullName.ToLower().Contains("ascii"));
                    asciiEntry.ExtractToFile(mainPath + "\\tempAsciiFile.zip", true);
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
            using (FileStream zipToOpen = new FileStream(mainPath + "\\tempAsciiFile.zip", FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries.Where(x => Path.GetExtension(x.FullName).ToLower() == ".ls"))
                    {
                        StreamReader reader = new StreamReader(entry.Open());
                        List<string> lines = new List<string>();
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            lines.Add(line);
                        }
                        reader.Close();
                        lsFiles.Add(entry.Name, lines);
                    }
                    archive.Dispose();
                }
                zipToOpen.Close();
            }
            File.Delete(mainPath + "\\tempAsciiFile.zip");
            return new FanucSyntaxValidatorData(filename, workbookContent, lsFiles);
        }

        private void CheckMainDirExistsDir()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester")))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester"));
            if (!Directory.Exists(mainPath))
                Directory.CreateDirectory(mainPath);
        }

        private void CheckOpenAndClose(string startString, string endString)
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>(); 
            Regex oneLinerRegex = new Regex(@"^\s*\d+\s*:\s*"+ startString + @".*,.*\s*;", RegexOptions.IgnoreCase);
            Regex startRegex = new Regex(@"^\s*\d+\s*:\s*" + startString, RegexOptions.IgnoreCase);
            Regex endRegex = new Regex(@"^\s*\d+\s*:\s*" + endString, RegexOptions.IgnoreCase);
            foreach (var lsFile in inputData.LSFiles)
            {
                int level = 0;
                foreach (var line in lsFile.Value)
                {
                    if (startRegex.IsMatch(line) && !oneLinerRegex.IsMatch(line))
                        level++;
                    if (endRegex.IsMatch(line))
                        level--;
                }
                if (level != 0)
                    errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Structure of " + startString + "/" + endString + " is incorrect in file " + Path.GetFileName(lsFile.Key), GlobalData.SovLogContentInfoTypes.Error));
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("IF/ENDIF structure is correct.", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckLabelsAndJumps()
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            Regex lblRegex = new Regex(@"(?<=^\s*\d+\s*:\s*LBL\s*\[)\d+(?=.*\])", RegexOptions.IgnoreCase);
            Regex jumpRegex = new Regex(@"(?<=^\s*\d+\s*:\s*.*JMP\s+LBL\s*\[)\d+(?=.*\])", RegexOptions.IgnoreCase);            
            foreach (var lsfile in inputData.LSFiles)
            {
                List<int> lblFound = new List<int>();
                List<int> jumpFound = new List<int>();
                foreach (var line in lsfile.Value)
                {
                    if (lblRegex.IsMatch(line))
                    {
                        int currentLabel = int.Parse(lblRegex.Match(line).ToString());
                        if (!lblFound.Contains(currentLabel))
                            lblFound.Add(currentLabel);
                        else
                            errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Multiple labels with number: " + currentLabel.ToString() + " File: " + Path.GetFileName(lsfile.Key), GlobalData.SovLogContentInfoTypes.Error));
                    }
                    if (jumpRegex.IsMatch(line) && !isCommentRegex.IsMatch(line))
                    {
                        int currentJump = int.Parse(jumpRegex.Match(line).ToString());
                        if (!jumpFound.Contains(currentJump))
                            jumpFound.Add(currentJump);
                    }
                }
                foreach (var jump in jumpFound)
                {
                    if (!lblFound.Any(x => x==jump))
                        errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Jump to non-existing label found, Label: " + jump.ToString() + " File: " + Path.GetFileName(lsfile.Key), GlobalData.SovLogContentInfoTypes.Error));
                }
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Labels and jumps structure is correct.", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckProcCalls()
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            List<string> exlusionList = new List<string>() { "waitforever", "breaktest" };
            Regex procCallRegex = new Regex(@"(?<=^\s*\d+\s*:\s+(?!.*(\!|PR_)).*CALL\s+)[\w_\-]+", RegexOptions.IgnoreCase);
            foreach (var lsfile in inputData.LSFiles)
            {
                int counter = 1;
                foreach (var line in lsfile.Value)
                {
                    
                    if (procCallRegex.IsMatch(line) && !isCommentRegex.IsMatch(line))
                    {
                        string currentCall = procCallRegex.Match(line).ToString();
                        if (!inputData.LSFiles.Any(x => Path.GetFileName(x.Key).ToLower().Contains(currentCall.ToLower())) && !exlusionList.Any(x => currentCall.ToLower() == x.ToLower()))
                            errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Not existing procedure " +currentCall+ " is called in file: " + Path.GetFileName(lsfile.Key) + ", line: "+counter.ToString() , GlobalData.SovLogContentInfoTypes.Error));
                    }
                    counter++;
                }
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Procedure call structure is correct.", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckAbortProgramPresent()
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            Regex abortPathNrRegex = new Regex("(?<=^\\s*\\d+\\s*:\\s*PR_CALL\\s+CMN\\s*\\(\\s*\"\\s*JobRequest.*AbortPath.*Arg\\s*\"\\s*\\=\\s*)\\d+", RegexOptions.IgnoreCase);
            Regex isOrgPathRegex = new Regex(@"PROG\d+", RegexOptions.IgnoreCase);
            foreach (var org in inputData.LSFiles.Where(x => isOrgPathRegex.IsMatch(x.Key)))
            {
                foreach (var abortPathNum in org.Value.Where(x => abortPathNrRegex.IsMatch(x)))
                {
                    string number = abortPathNrRegex.Match(abortPathNum).ToString();
                    if (!inputData.LSFiles.Keys.Any(x => Path.GetFileNameWithoutExtension(x).ToLower() == "a_path_" + number))
                        errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Abort program number " + number + " is called in procedure " + Path.GetFileNameWithoutExtension(org.Key) + " but no program A_Path_" + number + " was found.", GlobalData.SovLogContentInfoTypes.Error));
                }
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Abort programs structure is correct.", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckWorkbook()
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            Regex homeNumRegex = new Regex(@"(?<=\$REFPOS1\[)\d+", RegexOptions.IgnoreCase);
            string xmlString = string.Empty;
            inputData.WorkbookContent.ForEach(x => xmlString += x + "\r\n");
            if (CheckXMLConsistency(xmlString))
            {
                List<HomeFanuc> homesFanuc = new List<HomeFanuc>();
                XDocument doku = XDocument.Parse(xmlString);
                var systemNode = doku.Element("XMLVAR").Elements("PROG").Single(x => x.Attribute("name").Value == "*SYSTEM*");
                foreach (var homeNode in systemNode.Elements("ARRAY").Where(x => x.Attribute("name").Value.Contains("$REFPOS")))
                {
                    List<double> radvalues = new List<double>();
                    int homenum = int.Parse(homeNumRegex.Match(homeNode.Attribute("name").Value).ToString());
                    var perchPosNode = homeNode.Elements("FIELD").Single(x => x.Attribute("name").Value == "$PERCHPOS");
                    radvalues.Add(double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[1]").Value, CultureInfo.InvariantCulture));
                    radvalues.Add(double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[2]").Value, CultureInfo.InvariantCulture));
                    radvalues.Add(double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[3]").Value, CultureInfo.InvariantCulture));
                    radvalues.Add(double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[4]").Value, CultureInfo.InvariantCulture));
                    radvalues.Add(double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[5]").Value, CultureInfo.InvariantCulture));
                    radvalues.Add(double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[6]").Value, CultureInfo.InvariantCulture));
                    radvalues.Add(999999.0);
                    radvalues.Add(999999.0);
                    if (perchPosNode.Elements("ARRAY").Any(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[7]"))
                        radvalues[6] = double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[7]").Value, CultureInfo.InvariantCulture);
                    if (perchPosNode.Elements("ARRAY").Any(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[8]"))
                        radvalues[7] = double.Parse(perchPosNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$REFPOS1[" + homenum.ToString() + "].$PERCHPOS[8]").Value, CultureInfo.InvariantCulture);
                    homesFanuc.Add(new HomeFanuc(homenum, radvalues));
                }
                if (homesFanuc.Count > 0)
                {
                    var posRegNode = doku.Element("XMLVAR").Elements("PROG").Single(x => x.Attribute("name").Value == "*POSREG*").Elements("VAR").Single(x => x.Attribute("name").Value == "$POSREG").Elements("ARRAY").Single(x => x.Attribute("name").Value == "$POSREG[1]");
                    foreach (var home in homesFanuc)
                    {
                        if (!posRegNode.Elements("ARRAY").Any(x => x.Attribute("name").Value == "$POSREG[1," + home.Number + "]"))
                            errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Workbook.xvr: Home " + home.Number.ToString() + " declaration missing in $POSREG[1] section.", GlobalData.SovLogContentInfoTypes.Error));
                        else
                        {
                            var currentHomeNode = posRegNode.Elements("ARRAY").Single(x => x.Attribute("name").Value == "$POSREG[1," + home.Number + "]");
                            HomeFanuc updatedHome = home.UpdateHome(currentHomeNode.Value);
                            if (!string.IsNullOrEmpty(updatedHome.ErrorState))
                                errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Workbook.xvr: Radian and degrees values for home " + home.Number.ToString() + " do not match for axes: " + updatedHome.ErrorState, GlobalData.SovLogContentInfoTypes.Error));
                        }
                    }
                }
                else
                    errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Workbook.xvr: Refpos for home positions is missing.", GlobalData.SovLogContentInfoTypes.Error));
            }
            else
                errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Workbook.xvr structure is incorrect - XML damaged?", GlobalData.SovLogContentInfoTypes.Error));

            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Workbook.xvr structure is correct.", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);

        }

        private void CheckSemicolons()
        {
            Regex isSemicolonOk = new Regex(@"^.*;\s*$");
            Regex isMNLine = new Regex(@"^\s*/MN\s*$");
            Regex isPosLine = new Regex(@"^\s*/POS\s*$");
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            foreach (var file in inputData.LSFiles)
            {
                bool checkActive = false;
                int lineCounter = 1;
                foreach (var line in file.Value)
                {
                    if (isPosLine.IsMatch(line))
                        break;
                    if (checkActive && !string.IsNullOrEmpty(line.Trim()) && !isSemicolonOk.IsMatch(line))
                        errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Semicolon missing. Procedure: " + Path.GetFileNameWithoutExtension(file.Key) + ", line: " + lineCounter.ToString() , GlobalData.SovLogContentInfoTypes.Error));
                    if (isMNLine.IsMatch(line))
                        checkActive = true;
                    lineCounter++;
                }
            }

            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Semicolons at the end of lines are correct.", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }


        private void CheckProcedureNaming()
        {
            Regex procNameRegex = new Regex(@"(?<=^\s*/\s*PROG\s+).*$", RegexOptions.IgnoreCase);
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            foreach (var file in inputData.LSFiles)
            {
                if (Path.GetFileNameWithoutExtension(file.Key).ToLower() != procNameRegex.Match(file.Value[0]).ToString().ToLower().Trim())
                    errorsFoundInThisSection.Add(new SovBackupsPreparationResult("File name is different than PROC name in line 1 of file: " + Path.GetFileNameWithoutExtension(file.Key), GlobalData.SovLogContentInfoTypes.Error));
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("File names and headers are consistent", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckLineNumbers()
        {
            Regex isMNLine = new Regex(@"^\s*/MN\s*$");
            Regex isPosLine = new Regex(@"^\s*/POS\s*$");
            Regex lineNumRegex = new Regex(@"(?<=^\s*)\d+(?=\s*:)", RegexOptions.IgnoreCase);
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            foreach (var file in inputData.LSFiles)
            {
                bool checkActive = false;
                int lineCounter = 1;
                foreach (var line in file.Value)
                {
                    if (isPosLine.IsMatch(line))
                        break;
                    if (checkActive && !string.IsNullOrEmpty(line.Trim()) && lineNumRegex.IsMatch(line))
                    {
                        if (lineCounter != int.Parse(lineNumRegex.Match(line).ToString()))
                        {
                            errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Line numbering incorrect in file: " + Path.GetFileNameWithoutExtension(file.Key) + " after line " + lineCounter.ToString(), GlobalData.SovLogContentInfoTypes.Warning));
                            break;
                        }
                        lineCounter++;
                    }
                    if (isMNLine.IsMatch(line))
                        checkActive = true;

                }
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Line numbering is correct", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection); 
        }

        private bool CheckXMLConsistency(string xml)
        {   
            try
            {
                XDocument doku = XDocument.Parse(xml);
                return true;
            }
            catch (System.Xml.XmlException e)
            {
                return false;
            }

        }
        #endregion
    }
}
