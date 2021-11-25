using CommonLibrary;
using RobotFilesEditor.Model.Operations.BackupSyntaxValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RobotFilesEditor.Model.Operations
{
    public static class FixPtpAndLinMethods
    {
        static string[] omitedProcedures = { "masref" , "bmwbraketestreq" };

        public static void Execute()
        {
            try
            {
                MessageBox.Show("Select KRC4 backup zip file.", "Select backup", MessageBoxButton.OK, MessageBoxImage.Question);
                string backup = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1: "*.zip", filter1Descr: "Zip File");
                if (string.IsNullOrEmpty(backup))
                    return;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                CommonLibrary.FoundVariables globals = CommonLibrary.CommonMethods.FindVarsInBackup(backup, true);
                CommonLibrary.FoundVariables locals = CommonLibrary.CommonMethods.FindVarsInBackup(backup, false);
                IDictionary<string, string> bmw_programs = FixFoldSyntax(backup, "BMW_Program", globals, locals);
                IDictionary<string, string> bmw_utilities = FixFoldSyntax(backup, "BMW_Utilities", globals, locals);
                if (PrepareDirectory(backup))
                {
                    SaveFiles("BMW_Program", bmw_programs, backup);
                    SaveFiles("BMW_Utilities", bmw_utilities, backup);
                }
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("Succesfully saved at " + Path.GetDirectoryName(backup) + "\\" + Path.GetFileNameWithoutExtension(backup), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                MessageBox.Show("Something went wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool PrepareDirectory(string backup)
        {
            string fileName = Path.GetFileNameWithoutExtension(backup);
            string dirName = Path.GetDirectoryName(backup);
            if (!Directory.Exists(dirName + "\\" + fileName+"_Fixed"))
                Directory.CreateDirectory(dirName + "\\" + fileName + "_Fixed");
            else
            {
                System.Windows.Forms.DialogResult dialog = System.Windows.Forms.MessageBox.Show(String.Format("Directory {0} exists.\r\nOverwrite?", dirName + "\\" + fileName + "_Fixed"), "Overwrite?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (dialog == System.Windows.Forms.DialogResult.Yes)
                {
                    Directory.Delete(dirName + "\\" + fileName + "_Fixed", true);
                    Directory.CreateDirectory(dirName + "\\" + fileName + "_Fixed");
                }
                else
                    return false;
            }
            return true;
        }

        private static void SaveFiles(string dir, IDictionary<string, string> files, string backup)
        {
            string fileName = Path.GetFileNameWithoutExtension(backup);
            string dirName = Path.GetDirectoryName(backup);
            if (!Directory.Exists(dirName + "\\" + fileName + "_Fixed" + "\\" + dir))
                Directory.CreateDirectory(dirName + "\\" + fileName + "_Fixed" + "\\" + dir);

            foreach (var file in files)
            {
                File.WriteAllText(dirName + "\\" + fileName + "_Fixed" + "\\" + dir + "\\" + Path.GetFileName(file.Key.Replace("/", "\\")), file.Value);
            }
        }

        private static IDictionary<string, string> FixFoldSyntax(string backup, string directory, CommonLibrary.FoundVariables globals, CommonLibrary.FoundVariables locals)
        {
            Regex isEndFoldRegex = new Regex(@"^;\s*ENDFOLD", RegexOptions.IgnoreCase);
            Regex isParametersFoldRegex = new Regex(@"^;\s*FOLD\s+(Parameters\s*;\s*%\s*|;\s*%\s*\{\s*h\s*\})", RegexOptions.IgnoreCase);
            Regex isPTPMotionPointRegex = new Regex(@"^(PTP|LIN)\s+[a-zA-Z0-9-_]*", RegexOptions.IgnoreCase);
            Regex basRegex = new Regex(@"^BAS\s*\(\s*#.*", RegexOptions.IgnoreCase); 
            Regex isPTPRegex = new Regex(@"^\s*;\s*FOLD\s+(PTP|LIN)\s+", RegexOptions.IgnoreCase);
            Regex isModified = new Regex(@"(?<=(^.*\{\s*PE\s*\})).*", RegexOptions.IgnoreCase);
            Regex firstLineRegex = new Regex(@"^.*\{\s*PE\s*\}", RegexOptions.IgnoreCase);
            Regex getPointNameRegex = new Regex(@"(?<=(PTP|LIN)\s+)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
            Regex motionTypeRegex = new Regex(@"(PTP|LIN)", RegexOptions.IgnoreCase);            
            IDictionary<string, string> result = new Dictionary<string, string>();
            IDictionary<string, string> datFiles = new Dictionary<string, string>();
            using (ZipArchive archive = ZipFile.Open(backup, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries.Where(x => x.FullName.ToLower().Contains(".src") && x.FullName.ToLower().Contains(directory.ToLower())))
                {
                    string previousLine = string.Empty;
                    bool modifiedFile = false, isInsidePtpOrLinFold = false, isParametersFold = false;
                    string currentFileContent = string.Empty;
                    var currentFile = archive.GetEntry(CommonLibrary.CommonMethods.GetEntryName(archive.Entries, entry.ToString()));
                    StreamReader reader = new StreamReader(currentFile.Open());
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (isParametersFoldRegex.IsMatch(line.Trim()))
                        {
                            isParametersFold = true;
                        }
                        if (isEndFoldRegex.IsMatch(line.Trim()))
                        {
                            if (isParametersFold)
                                isInsidePtpOrLinFold = false;
                            else
                                isParametersFold = false;
                        }

                        if (isPTPRegex.IsMatch(line.Trim()))
                        {
                            isInsidePtpOrLinFold = true;
                            if (string.IsNullOrEmpty(isModified.Match(line).ToString().Trim()))
                                currentFileContent += line + "\r\n";
                            else
                            {
                                modifiedFile = true;
                                string modifiedLine = firstLineRegex.Match(line).ToString() + "\r\n";
                                modifiedLine += String.Join(Environment.NewLine, ";FOLD Parameters ;%{h}",
                                    String.Format(";Params IlfProvider=kukaroboter.basistech.inlineforms.movement.old; Kuka.IsGlobalPoint={0}; Kuka.PointName={1}; Kuka.BlendingEnabled={2}; Kuka.APXEnabled=False; {3}; {4}; Kuka.CurrentCDSetIndex=0; Kuka.MovementParameterFieldEnabled=True; IlfCommand={5}",GetGlobal(globals,locals,line,entry.FullName), getPointNameRegex.Match(line), GetBlended(line), GetMoveDataName(line), GetVelocity(line), motionTypeRegex.Match(line)),
                                    ";ENDFOLD",
                                    "");
                                currentFileContent += modifiedLine;
                            }
                        }
                        else if (isPTPMotionPointRegex.IsMatch(line.Trim()) && !previousLine.ToLower().Contains("set_cd_params") &&  basRegex.IsMatch(previousLine.ToLower().Trim()) && isInsidePtpOrLinFold) 
                        {
                            modifiedFile = true;
                            currentFileContent += "SET_CD_PARAMS(0)\r\n" + line + "\r\n";
                        }
                        else
                            currentFileContent += line + "\r\n";
                        previousLine = line;
                    }
                    reader.Close();
                    if (modifiedFile)
                    {
                        bool addfile = true;
                        foreach (string omitedproc in omitedProcedures)
                        {
                            if (entry.FullName.ToLower().Contains(omitedproc))
                            {
                                addfile = false;
                                break;
                            }
                        }
                        if (addfile)
                            result.Add(entry.FullName, currentFileContent);
                    }

                }
                
                foreach (var srcFile in result)
                {
                    foreach (var entry in archive.Entries.Where(x=>x.FullName.ToLower().Contains(srcFile.Key.Replace(".src",".dat").ToLower())))
                    {
                        var currentFile = archive.GetEntry(CommonLibrary.CommonMethods.GetEntryName(archive.Entries, entry.ToString()));
                        StreamReader reader = new StreamReader(currentFile.Open());
                        string filecontent = reader.ReadToEnd();
                        datFiles.Add(entry.FullName, filecontent);
                        reader.Close();
                    }
                }
            }
            foreach (var item in datFiles)
                result.Add(item);
            return result;
        }

        private static object GetBlended(string line)
        {
            if (line.ToLower().Contains(" cont "))
                return "True";
            else
                return "False";
        }

        private static string GetVelocity(string line)
        {
            Regex motionTypeRegex = new Regex(@"(PTP|LIN)", RegexOptions.IgnoreCase);
            Regex velocityRegex = new Regex(@"(?<=Vel\s*\=)\d+(\.\d+|\s*)", RegexOptions.IgnoreCase);
            string vel = velocityRegex.Match(line).ToString();
            string motionType = motionTypeRegex.Match(line).ToString();

            if (motionType.ToLower().Contains("lin"))
                return "Kuka.VelocityPath=" + vel;
            else
                return "Kuka.VelocityPtp=" + vel;
        }

        private static string GetMoveDataName(string line)
        {
            Regex getPointNameRegex = new Regex(@"(?<=(PTP|LIN)\s+)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
            Regex motionTypeRegex = new Regex(@"(PTP|LIN)", RegexOptions.IgnoreCase);
            string pointName = getPointNameRegex.Match(line).ToString();
            string motionType = motionTypeRegex.Match(line).ToString();

            if (motionType.ToLower().Contains("lin"))
                return "Kuka.MoveDataName=L" + pointName;
            else
                return "Kuka.MoveDataPtpName=P" + pointName;
        }

        private static string GetGlobal(FoundVariables globals, FoundVariables locals, string line, string fullName)
        {
            Regex getPointNameRegex = new Regex(@"(?<=(PTP|LIN)\s+)[a-zA-Z0-9_-]*", RegexOptions.IgnoreCase);
            Regex motionTypeRegex = new Regex(@"(PTP|LIN)", RegexOptions.IgnoreCase);
            string pointName = getPointNameRegex.Match(line).ToString();
            string motionType = motionTypeRegex.Match(line).ToString();
            string fixedname = fullName.Replace("/", "\\");
            string file = Path.GetFileNameWithoutExtension(fixedname);
            foreach (var localVar in locals.E6AXIS.Where(x=>x.Key.ToLower().Contains(pointName.ToLower() + " " + file.ToLower())))
            {
                return "False";
            }
            foreach (var localVar in globals.E6AXIS.Where(x => x.Key.ToLower().Contains(pointName.ToLower())))
                return "True";
            foreach (var localVar in locals.E6POS.Where(x => x.Key.ToLower().Contains(pointName.ToLower() + " " + file.ToLower())))
            {
                return "False";
            }
            foreach (var localVar in globals.E6POS.Where(x => x.Key.ToLower().Contains(pointName.ToLower())))
                return "True";
            return "False";
        }
    }
}
