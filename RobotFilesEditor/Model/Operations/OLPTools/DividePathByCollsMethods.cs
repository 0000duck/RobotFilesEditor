using CommonLibrary;
using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Common = CommonLibrary.CommonMethods;

namespace RobotFilesEditor.Model.Operations.OLPTools
{
    public class DividePathByCollsMethods : BaseClasses
    {        
        internal void Execute()
        {
            MessageBox.Show("Select path to divide", "Select path", MessageBoxButton.OK, MessageBoxImage.Information);
            string srcfile = Common.SelectDirOrFile(false, "SRC file", "*.src");
            if (string.IsNullOrEmpty(srcfile))
                return;
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(srcfile), Path.GetFileNameWithoutExtension(srcfile)+ ".dat")))
                return;
            IDictionary<string, string> result = new Dictionary<string, string>();
            Regex isCollReqRegex = new Regex(@"(?<=(COLL_SAFETY_REQ|Plc_CollSafetyReq\d)\s*\(\s*)\d+", RegexOptions.IgnoreCase);
            Regex isCollClrRegex = new Regex(@"(?<=(COLL_SAFETY_CLEAR|COLL_SAFETY_CLEAR_CONT|Plc_CollSafetyClear\d)\s*\(\s*)\d+", RegexOptions.IgnoreCase);
            Regex isPTPRegex = new Regex(@"(PTP|LIN)\s+.*", RegexOptions.IgnoreCase);
            Regex isCont = new Regex(@"(PTP|LIN)\s+[a-zA-Z0-9_\-]+\s+(C_DIS|C_PTP)", RegexOptions.IgnoreCase);
            string datFile = Path.Combine(Path.GetDirectoryName(srcfile), Path.GetFileNameWithoutExtension(srcfile) + ".dat");
            string lastMotionFold = string.Empty;
            int currentPathCounter = 1;
            List<int> currentCollList = new List<int>();
            List<int> currentCollListReqBuffer = new List<int>();
            List<int> currentCollListClrBuffer = new List<int>();
            List<string> currentFoldList = new List<string>();
            List<string> collStringsBuffer = new List<string>();
            List<SrcAndDat> srcAndDats = RenumberPointsMethods.DivideToFolds(new List<SrcAndDat>() { new SrcAndDat(srcfile, datFile) });
            bool startAddingLines = false, previousCollReq = false, previousCollClr = false, isFirstCycle = true, wasLastCollCont = false ;
            foreach (var fold in srcAndDats[0].SrcContent)
            {
                if (!string.IsNullOrEmpty(Common.RemoveComment(fold)))
                {
                    if (isPTPRegex.IsMatch(Common.RemoveComment(fold)) || isCollReqRegex.IsMatch(fold) || isCollClrRegex.IsMatch(fold))
                        startAddingLines = true;
                    if (isCollReqRegex.IsMatch(fold))
                    {
                        wasLastCollCont = isCont.IsMatch(fold) ? true : false;
                        previousCollReq = true;
                        int currentCollNum = int.Parse(isCollReqRegex.Match(fold).ToString());
                        currentCollListReqBuffer.Add(currentCollNum);
                        collStringsBuffer.Add(fold);
                    }
                    else if (isCollClrRegex.IsMatch(fold))
                    {
                        wasLastCollCont = isCont.IsMatch(fold) ? true : false;
                        previousCollClr = true;
                        int currentCollNum = int.Parse(isCollClrRegex.Match(fold).ToString());
                        currentCollListClrBuffer.Add(currentCollNum);
                        collStringsBuffer.Add(fold);
                    }
                    else
                    {
                        if (previousCollReq || previousCollClr)
                        {
                            if (isFirstCycle)
                            {
                                isFirstCycle = false;
                                collStringsBuffer = new List<string>();
                            }
                            else
                            {
                                List<string> listToAdd = new List<string>();
                                currentCollList.Sort();
                                string pathName = Path.GetFileNameWithoutExtension(srcAndDats[0].Src) + "_" + currentPathCounter + "_" + "coll" + GetCollNumsString(currentCollList);
                                listToAdd.Add("DEF " + pathName + "()");
                                listToAdd.AddRange(currentFoldList);
                                listToAdd.Add("END");
                                string strToAdd = string.Empty;
                                Action<List<string>> listToStr = x => x.ForEach(y => strToAdd += y + "\r\n");
                                listToStr.Invoke(listToAdd);
                                result.Add(pathName, strToAdd);
                                currentFoldList = new List<string>();
                                if (!wasLastCollCont)
                                    currentFoldList.Add(lastMotionFold);
                                currentFoldList.AddRange(collStringsBuffer);
                                collStringsBuffer = new List<string>();
                                currentPathCounter++;
                            }
                        }
                        previousCollReq = false;
                        previousCollClr = false;
                    }
                    if (isPTPRegex.IsMatch(Common.RemoveComment(fold)) && !previousCollClr && !previousCollReq)
                    {
                        lastMotionFold = fold;
                        if (currentCollListReqBuffer.Count > 0)
                        {
                            currentCollList.AddRange(currentCollListReqBuffer);
                            currentCollListReqBuffer = new List<int>();
                        }
                        if (currentCollListClrBuffer.Count > 0)
                        {
                            currentCollListClrBuffer.ForEach(x => currentCollList.Remove(x));
                            currentCollListClrBuffer = new List<int>();
                        }
                    }
                    if (startAddingLines)
                        currentFoldList.Add(fold);
                }
            }
            SaveFiles(result, Path.GetDirectoryName(srcfile),datFile);
        }

        private void SaveFiles(IDictionary<string, string> files, string dir, string datFile)
        {
            Regex replaceInDatRegex = new Regex(@"DEFDAT\s+[\w]+", RegexOptions.IgnoreCase);
            string datContent = File.ReadAllText(datFile);
            string dirToSave = Path.Combine(dir, "DividedPaths");
            CreateDirectoriesToSave(dirToSave);
            try
            {
                foreach (var file in files)
                {
                    File.WriteAllText(Path.Combine(dirToSave, file.Key + ".src"), file.Value);
                    string datToSave = replaceInDatRegex.Replace(datContent, "DEFDAT " + file.Key);
                    File.WriteAllText(Path.Combine(dirToSave, file.Key + ".dat"), datToSave);
                }
                MessageBox.Show("Successfuly saved at " + dirToSave, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void CreateDirectoriesToSave(string dirToSave)
        {
            base.CreateDirectoriesToSave(dirToSave);
            if (Directory.GetDirectories(dirToSave).ToList().Count > 0 || Directory.GetFiles(dirToSave).ToList().Count > 0)
            {
                Directory.Delete(dirToSave, true);
                Directory.CreateDirectory(dirToSave);
            }
        }

        private string GetCollNumsString(List<int> currentCollList)
        {
            if (currentCollList.Count == 0) return "_nocoll";
            string result = string.Empty;
            currentCollList.ForEach(x => result += x + "_");
            return result.Substring(0, result.Length - 1);
        }
    }
}
