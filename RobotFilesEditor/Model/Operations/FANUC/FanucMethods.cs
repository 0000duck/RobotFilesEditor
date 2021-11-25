using RobotFilesEditor.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.FANUC
{
    public class FanucFilesValidator
    {
        public enum CurrentType { Undef ,Motion, Spot, Glue, Search, ProcCall, FrameDef }
        public List<string> FilesList { get; set; }
        public IDictionary<string, FanucRobot> FilesAndContent { get; set; }
        public string RobotName { get; set; }

        public FanucFilesValidator(List<string> filesList)
        {
            FilesList = filesList;
            GetRobotName();
            FilesAndContent = ReadFiles();
            FilesAndContent = AddHeader();
            FilesAndContent = AddSpaces();
            FilesAndContent = RenumberLines();
        }

        private IDictionary<string, FanucRobot> AddSpaces()
        {
            Regex isMotionPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+\s*(|\:\s*[\w\d-_]+)\s*\]\s+\d+(\%|mm/sec)\s+(FINE|CNT\d+)\s*;", RegexOptions.IgnoreCase);
            Regex isSpotPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*SWP_P", RegexOptions.IgnoreCase);
            Regex isGluePoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*GL_P", RegexOptions.IgnoreCase);
            Regex isSearchPoint = new Regex(@"^\s*\d+\s*\:\s*(J|L)\s*(PR|P)\s*\[\s*\d+.*SEARCH_S_P", RegexOptions.IgnoreCase);
            Regex isProcedureCall = new Regex(@"^\s*\d+\s*\:\s*PR_CALL", RegexOptions.IgnoreCase);
            Regex isFrameDef = new Regex(@"^\s*\d+\s*\:\s*(UFRAME|UTOOL|PAYLOAD)", RegexOptions.IgnoreCase);

            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            foreach (var file in FilesAndContent)
            {
                List<string> lines = RemoveSpaces(file.Value.ProgramSection);
                CurrentType previousType = CurrentType.Undef;
                CurrentType currentType = CurrentType.Undef;
                List<string> currentFileContent = new List<string>();
                foreach (var line in lines)
                {
                    if (isMotionPoint.IsMatch(line))
                        currentType = CurrentType.Motion;
                    else if (isSpotPoint.IsMatch(line))
                        currentType = CurrentType.Spot;
                    else if (isGluePoint.IsMatch(line))
                        currentType = CurrentType.Glue;
                    else if (isSearchPoint.IsMatch(line))
                        currentType = CurrentType.Search;
                    else if (isProcedureCall.IsMatch(line))
                        currentType = CurrentType.ProcCall;
                    else if (isFrameDef.IsMatch(line))
                        currentType = CurrentType.FrameDef;

                    if (currentType != previousType)
                        currentFileContent.Add("666:  ;");
                    currentFileContent.Add(line);
                    
                    previousType = currentType;
                }
                result.Add(file.Key, new FanucRobot(file.Value.InitialSection, currentFileContent, file.Value.DeclarationSection));
            }
            return result;
        }

        private List<string> RemoveSpaces(List<string> programSection)
        {
            Regex isBlankLineRegex = new Regex(@"^\s*\d+\s*\:\s*(|!)\s*;", RegexOptions.IgnoreCase);
            List<string> result = new List<string>();
            foreach (var line in programSection.Where(x => !isBlankLineRegex.IsMatch(x.Trim())))
                result.Add(line);
            return result;
        }

        private IDictionary<string, FanucRobot> RenumberLines()
        {
            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            foreach (var file in FilesAndContent)
            {
                List<string> renumberedProgramSection = RenumberLinesFanucMethods.GetRenumberedBody(file.Value.ProgramSection);
                result.Add(file.Key, new FanucRobot(file.Value.InitialSection, renumberedProgramSection, file.Value.DeclarationSection));
            }
            return result;
        }

        private IDictionary<string, FanucRobot> ReadFiles()
        {
            Regex isprogramSectionStart = new Regex(@"^\d+\s*\:(J|L|\s)", RegexOptions.IgnoreCase);
            Regex isDeclarationSectionStart = new Regex(@"^\/\s*POS\s*$", RegexOptions.IgnoreCase);
            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            foreach (var file in FilesList)
            {
                int sectionNum = 1;
                List<string> initialSection = new List<string>();
                List<string> programSection = new List<string>();
                List<string> declarationSection = new List<string>();
                StreamReader reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (sectionNum ==1 && isprogramSectionStart.IsMatch(line.Trim()) || sectionNum == 2 && isDeclarationSectionStart.IsMatch(line.Trim()))
                        sectionNum++;
                    switch (sectionNum)
                    {                        
                        case 1:
                            initialSection.Add(line);
                            break;
                        case 2:
                            programSection.Add(line);
                            break;
                        case 3:
                            declarationSection.Add(line);
                            break;
                    }
                }
                reader.Close();
                result.Add(file, new FanucRobot(initialSection,programSection,declarationSection));
            }
            return result;
        }

        private IDictionary<string, FanucRobot> AddHeader()
        {
            Regex isOldHeaderPart = new Regex(@"^\s*\d+\s*\:\s*!\s*\*", RegexOptions.IgnoreCase);
            Regex commentRegex = new Regex(@"^\s*\d+\s*\:\s*!", RegexOptions.IgnoreCase);
            IDictionary<string, FanucRobot> result = new Dictionary<string, FanucRobot>();
            foreach (var file in FilesAndContent)
            {
                int headreLinesCounter = 0;
                List<string> header = CreateHeader(Path.GetFileNameWithoutExtension(file.Key));
                List<string> currentFile = new List<string>();
                foreach (var line in file.Value.ProgramSection)
                {
                    if (!commentRegex.IsMatch(line))
                        headreLinesCounter = 2;
                    if (headreLinesCounter >= 2)
                        currentFile.Add(line);
                    if (isOldHeaderPart.IsMatch(line))
                        headreLinesCounter++;
                }
                List<string> newProgramSection = header;
                newProgramSection.AddRange(currentFile);
                result.Add(file.Key, new FanucRobot(file.Value.InitialSection, newProgramSection, file.Value.DeclarationSection));
            }
            return result;
        }

        internal static string GetFileContenetFANUC(FanucRobot files)
        {
            string result = string.Empty;
            foreach (var line in files.InitialSection)
                result += line + "\r\n";
            foreach (var line in files.ProgramSection)
                result += line + "\r\n";
            foreach (var line in files.DeclarationSection)
                result += line + "\r\n";
            return result;
        }

        private List<string> CreateHeader(string progname)
        {
            List<string> header = new List<string>();
            header.Add("   1:  ! *********************************************;");
            header.Add("   2:  ! Prog: "+progname+";");
            header.Add("   3:  ! created on: "+DateTime.Now.ToString("yyyy-MM-dd")+";");
            header.Add("   4:  ! IR: "+RobotName+";");
            header.Add("   5:  ! Creator: "+ConfigurationManager.AppSettings["Ersteller"] + ";");
            header.Add("   6:  ! Last Update: "+ DateTime.Now.ToString("yyyy-MM-dd") + ";");
            header.Add("   7:  ! *********************************************;");

            return header;
        }

        private void GetRobotName()
        {
            NameRoboterViewModel vm = new NameRoboterViewModel();
            var dialog = new NameRobot(vm);
            dialog.ShowDialog();
            RobotName = vm.RobotName == null ? string.Empty : vm.RobotName;
        }
    }
}