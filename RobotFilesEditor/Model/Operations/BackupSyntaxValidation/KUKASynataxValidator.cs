using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RobotFilesEditor.Model.Operations.DataClass;
using CommonLibrary;

namespace RobotFilesEditor.Model.Operations.BackupSyntaxValidation
{
    // TODO:
    // Otwarcie/Zamkniecie switchów i ifów - done
    // znalezienie wywołań nieistniejących funkcji
    // znalezienie brakujących definicji i zmiennych i pozycji
    //
    public class KUKASynataxValidator : IBackupSyntaxValidator
    {
        #region props
        public List<SovBackupsPreparationResult> ErrorsFound { get; set; }
        #endregion

        #region fields       
        string[] types = { "int ", "real ", "char ", "bool ", "fdat ", "ldat ", "pdat ", "e6axis ", "e6pos ", "struc ", "signal ", "decl " };
        string file;
        enum VarTask { MotionType, KrlConstant }
        CommonLibrary.FoundVariables globals;
        CommonLibrary.FoundVariables locals;
        private KukaValidationData inputData;
        #endregion

        #region ctor
        public KUKASynataxValidator(string filename)
        {
            file = filename;
            Execute();
        }
        #endregion

        public void Execute()
        {
            inputData = CommonLibrary.CommonMethods.ReadDataFromKukaBackup(file);
            ErrorsFound = new List<SovBackupsPreparationResult>();
            CheckCellPresent();
            CheckOpeningAndClosing(file,"IF","ENDIF");
            CheckOpeningAndClosing(file, "SWITCH","ENDSWITCH");
            CheckOpeningAndClosing(file, "WHILE","ENDWHILE");
            CheckOpeningAndClosing(file, "FOR","ENDFOR");
            CheckOpeningAndClosing(file, "LOOP", "ENDLOOP");
            globals = CommonLibrary.CommonMethods.FindVarsInBackup(inputData, true);
            locals = CommonLibrary.CommonMethods.FindVarsInBackup(inputData, false);
            List<IProcOrFunc> functionsAndProcedures = FindFunctions();
            List<ProcCall> procedureCalls = FindProcedureCalls();
            CheckProcCalls(functionsAndProcedures, procedureCalls);
            CheckAbortFunctionsBase();
            CheckIniFoldPresent(file);
            CheckVariableUsage(globals, locals);
            if (ErrorsFound.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("No problems found :-)", GlobalData.SovLogContentInfoTypes.OK));
        }

        private void CheckCellPresent()
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();

            if (!inputData.SrcFiles.Any(x => x.Key.ToLower().Contains("cell.src")))
                errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Cell.src file missing in backup", GlobalData.SovLogContentInfoTypes.Error));

            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Cell file is present", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckVariableUsage(FoundVariables globals, FoundVariables locals)
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            globals.FillAllVariables();
            locals.FillAllVariables();
            Regex valueAssignmentRegex = new Regex(@"(?<=^\s*(?!;))[a-zA-Z\$][a-zA-Z0-9_\-]*(?=\s*\=.*)", RegexOptions.IgnoreCase);
            Regex valueAssgnmentRightSideRegex = new Regex(@"(?<=^\s*(?!;)[a-zA-Z\$][a-zA-Z0-9_\-]+\s*\=\s*)[a-zA-Z\$][a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
            Regex krlSynstaxVaraibleUseRegex = new Regex(@"(?<=^\s*(?!;)\s*(SWITCH|IF|WHILE|FOR)(\s+(?!\()|\s*\())[a-zA-Z\$][a-zA-Z0-9_\-]*",RegexOptions.IgnoreCase);
            Regex variableAsArgumentRegex = new Regex(@"(?<=^\s*(?!;)\s*[a-zA-Z][a-zA-Z0-9_\-]+\s*\(\s*)[a-zA-Z\$][a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
            Regex isProcCallRegex = new Regex(@"[a-zA-Z][a-zA-Z0-9_\-]*\s*\(", RegexOptions.IgnoreCase);
            List<Variable> tempVariableList = new List<Variable>();

            var srcFiles = inputData.SrcFiles.Where(x => !Path.GetDirectoryName(x.Key).ToLower().Contains("bmw_app") && !Path.GetDirectoryName(x.Key).ToLower().Contains("system")).Where(z => !Path.GetDirectoryName(z.Key).ToLower().Contains("c\\krc"));
            foreach (var srcFile in srcFiles)
            {
                int lineCounter = 1;
                foreach(var line in srcFile.Value)
                {
                    string currentViarable = string.Empty;
                    if (valueAssignmentRegex.IsMatch(line))
                    {
                        string varToAdd = valueAssignmentRegex.Match(line).ToString();
                        if (NotExcludedVariables(varToAdd,VarTask.MotionType) && NotSystemVar(varToAdd))
                            tempVariableList.Add(new Variable(varToAdd,"Unknown",false,srcFile.Key,line, lineCounter));
                    }
                    if (valueAssgnmentRightSideRegex.IsMatch(line) && !isProcCallRegex.IsMatch(line))
                    {
                        string varToAdd = valueAssgnmentRightSideRegex.Match(line).ToString();
                        if (NotExcludedVariables(varToAdd, VarTask.KrlConstant))
                            currentViarable = varToAdd;
                    }
                    if (krlSynstaxVaraibleUseRegex.IsMatch(line) && !isProcCallRegex.IsMatch(line))
                        currentViarable = krlSynstaxVaraibleUseRegex.Match(line).ToString();
                    if (variableAsArgumentRegex.IsMatch(line))
                        currentViarable = variableAsArgumentRegex.Match(line).ToString();

                    if (!string.IsNullOrEmpty(currentViarable) && NotExcludedVariables(currentViarable, VarTask.KrlConstant) && NotProcCall(line,currentViarable) && NotSystemVar(currentViarable))
                        tempVariableList.Add(new Variable(currentViarable,"Unknown",false,srcFile.Key,line, lineCounter));
                    lineCounter++;
                }
            }

            foreach (var variable in tempVariableList)
            {
                var varFound = globals.AllVariables.FirstOrDefault(x => x.Name.ToLower() == variable.Name.ToLower());
                if (varFound == null)
                    varFound = locals.AllVariables.Where(x => x.Localization.ToLower() == Path.GetFileNameWithoutExtension(variable.Localization).ToLower()).FirstOrDefault(y => y.Name.ToLower() == variable.Name.ToLower());
                if (varFound == null)
                    errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Variable " + variable.Name + " is used in path, but not declared. Path: " + variable.Localization + ", Line: " + variable.LineNum , GlobalData.SovLogContentInfoTypes.Error));
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Variable structure is ok", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private bool NotSystemVar(string currentViarable)
        {
            if (currentViarable.Trim().Length > 0 && currentViarable.Trim().Substring(0, 1) == "$")
                return false;
            return true;
        }

        private bool NotProcCall(string line, string currentViarable)
        {
            if (new Regex(currentViarable + @"\s*\(").IsMatch(line))
                return false;
            return true;
        }

        private void CheckIniFoldPresent(string file)
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            Regex iniFoldFoundRegex = new Regex(@"(?<=^\s*(?!;))\s*BAS\s*\(#INITMOV\s*,\s*0\s*\)", RegexOptions.IgnoreCase);
            Regex movementFoundRegex = new Regex(@"(?<=^\s*(?!;))(PTP|LIN)", RegexOptions.IgnoreCase);
            var srcFiles = inputData.SrcFiles.Where(x => !Path.GetDirectoryName(x.Key).ToLower().Contains("bmw_app") && !Path.GetDirectoryName(x.Key).ToLower().Contains("system")).Where(z => !Path.GetDirectoryName(z.Key).ToLower().Contains("c\\krc"));
            foreach (var srcFile in srcFiles)
            {
                bool iniFound = false, movementFound = false;
                foreach(var line in srcFile.Value)
                {
                    if (iniFoldFoundRegex.IsMatch(line))
                    {
                        iniFound = true;
                        break;
                    }
                    if (!movementFound && movementFoundRegex.IsMatch(line))
                        movementFound = true;
                }
                if (movementFound && ! iniFound)
                    errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Missing INI fold in path: " + srcFile.Key + ". Check PS Templates!", GlobalData.SovLogContentInfoTypes.Warning));
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("INI Folds are ok", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckAbortFunctionsBase()
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            Regex baseRegex = new Regex(@"(?<=^\s*(?!;).*FDAT.*BASE_NO\s+)\d+", RegexOptions.IgnoreCase);
            var abortAndHomeProgDats = inputData.DatFiles.Where(x => Path.GetDirectoryName(x.Key).ToLower().Contains("abort_programs") || Path.GetDirectoryName(x.Key).ToLower().Contains("home_programs")).ToList();
            foreach (var datFile in abortAndHomeProgDats)
            {
                int lineNum = 1;
                foreach (var line in datFile.Value)
                {
                    if (baseRegex.IsMatch(line) && baseRegex.Match(line).ToString() != "0")
                        errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Point in abort path not in Base 0. Path: " + datFile.Key +", line num: " + lineNum, GlobalData.SovLogContentInfoTypes.Warning));
                    lineNum++;
                }
            }
            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Abort path structure is ok", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private void CheckProcCalls(List<IProcOrFunc> functionsAndProcedures, List<ProcCall> procedureCalls)
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            List<ProcCall> missingDeclarationCalls = new List<ProcCall>();
            foreach (var procedureCall in procedureCalls)
            {
                var procDefinition = functionsAndProcedures.FirstOrDefault(x => x.Name.ToLower().Trim() == procedureCall.Name.ToLower().Trim());
                if (procDefinition == null)
                {
                    if (CheckMissingPath(procedureCall.Name))
                        missingDeclarationCalls.Add(procedureCall);
                }
            }
            foreach (var missingCall in missingDeclarationCalls)
                errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Procedure called but missing declaration! File: " + missingCall.Localization + ", Procedure: " + missingCall.Name, GlobalData.SovLogContentInfoTypes.Error));

            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Procedure calls structure is ok", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private bool CheckMissingPath(string name)
        {
            List<string> notDefinedProcedures = new List<string>() { "rob_stop_release" , "masref_user", "set_cd_params"};
            if (notDefinedProcedures.Any(x => x == name.ToLower()))
                return false;
            return true;
        }

        private List<ProcCall> FindProcedureCalls()
        {
            Regex procCallRegex = new Regex(@"(?<=^\s*[a-zA-Z0-9_\-]*)[a-zA-Z0-9_\-]+\s*\([a-zA-Z0-9_\-\s]*\)", RegexOptions.IgnoreCase);
            List<ProcCall> result = new List<ProcCall>();
            foreach (var entry in inputData.SrcFiles.Where(z => !Path.GetDirectoryName(z.Key).ToLower().Contains("c\\krc")))
            {
                foreach (var line in entry.Value)
                {
                    if (!line.ToLower().Contains("def ") && !line.ToLower().Contains("deffct ") && procCallRegex.IsMatch(line) && ExcludedStart(line))
                    {
                        result.Add(new ProcCall(line, entry.Key));
                    }
                }
            }
            return result;
        }

        private bool ExcludedStart(string line)
        {
            line = line.Trim();
            List<string> excludedPrefixes = new List<string>() { "if" , "until" , "return", "switch" , "while"};
            if (excludedPrefixes.Where(x => line.Length >= x.Length ).Any(y => line.Substring(0, y.Length).ToLower() == y))
                return false;
            return true;
        }

        private bool NotExcludedVariables(string varToAdd, VarTask task)
        {
            //if (varToAdd.ToLower().Contains("not"))
            //{ }
            List<string> excludedVars = new List<string>();
            if (task == VarTask.MotionType)
                excludedVars = new List<string>() { "pdat_act", "ldat_act", "fdat_act" };
            else
                excludedVars = new List<string>() { "true", "false", "not", "abs" };
            
            if (excludedVars.Any(x => x == varToAdd.ToLower()))
                return false;
            return true;
        }

        private void CheckOpeningAndClosing(string file, string startString, string endString)
        {
            List<SovBackupsPreparationResult> errorsFoundInThisSection = new List<SovBackupsPreparationResult>();
            Regex getStartValue = new Regex(@"^" + startString + @"(\s+|\(|$)(?=\s*)", RegexOptions.IgnoreCase);
            Regex getEndValue = new Regex(@"^" + endString + @"(?=(\s*|!))", RegexOptions.IgnoreCase);

            int level = 0, linecounter = 0;
            string beginLines = "", endlines = "";

            foreach (var currentFile in inputData.SrcFiles)
            {
                level = 0;
                linecounter = 0;
                beginLines = "";
                endlines = "";
                foreach (var line in currentFile.Value)
                {
                    linecounter++;
                   
                    if (getStartValue.IsMatch(line.Trim()))
                    {
                        beginLines += linecounter + ",";
                        level++;
                    }
                    if (getEndValue.IsMatch(line.Trim().Replace(" ", "")))
                    {
                        endlines += linecounter + ",";
                        level--;
                    }
                }
                if (level != 0)
                    errorsFoundInThisSection.Add(new SovBackupsPreparationResult("Problem with incorrect structure of " + startString + "\\" + endString + " in file " + Path.GetFileNameWithoutExtension(currentFile.Key) + ".",GlobalData.SovLogContentInfoTypes.Error));
            }

            if (errorsFoundInThisSection.Count == 0)
                ErrorsFound.Add(new SovBackupsPreparationResult("Structure of " + startString + "\\" + endString + " is ok", GlobalData.SovLogContentInfoTypes.OK));
            else
                ErrorsFound.AddRange(errorsFoundInThisSection);
        }

        private List<IProcOrFunc> FindFunctions()
        {
            Regex findFctRegex = new Regex(@"^\s*(|GLOBAL)\s*DEFFCT", RegexOptions.IgnoreCase);
            //Regex getTypeRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*DEFFCT\s+)\w+", RegexOptions.IgnoreCase);
            //Regex getFctNameRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*DEFFCT\s+\w+\s+)\w+", RegexOptions.IgnoreCase);
            Regex findProcRegex = new Regex(@"^\s*(|GLOBAL)\s*DEF\s+", RegexOptions.IgnoreCase);
            //Regex getProcNameRegex = new Regex(@"(?<=^\s*(|GLOBAL)\s*DEF\s+)\w+", RegexOptions.IgnoreCase);
            List<IProcOrFunc> foundFcts = new List<IProcOrFunc>();
            foreach (var entry in inputData.SrcFiles)
            {
                foreach (var line in entry.Value)
                {
                    if (findFctRegex.IsMatch(line))
                        foundFcts.Add(new Function(line, entry.Key));
                    if (findProcRegex.IsMatch(line))
                        foundFcts.Add(new Procedure(line, entry.Key));
                }
            }
            return foundFcts;
        }
    }
}
