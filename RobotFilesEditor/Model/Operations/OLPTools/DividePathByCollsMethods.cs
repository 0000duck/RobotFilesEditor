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
            string filePath = Common.SelectDirOrFile(false, "SRC file", "*.src", "LS file", ".ls");
            if (string.IsNullOrEmpty(filePath))
                return;
            if (Path.GetExtension(filePath).ToLower() == ".src")
                ExecuteKUKA(filePath);
            else
                ExecuteFanuc(filePath);
        }

        private void ExecuteFanuc(string lsFile)
        {
            List<string> warningContent = new List<string>();
            List<string> beginingContent = new List<string>();
            List<string> currentSegment = new List<string>();
            Dictionary<string, List<string>> collSegments = new Dictionary<string, List<string>>();
            Regex isMNLine = new Regex(@"^\s*/\s*MN", RegexOptions.IgnoreCase);
            Regex getRequestNum = new Regex("(?<=PR_CALL CMN(|_P)\\s*\\(\\s*\"\\s*CollZone\\s*\"\\s*\\=\\s*\\d+\\s*,\\s*\"\\s*Request.*ZoneNo\\s*\"\\s*\\=\\s*)\\d+", RegexOptions.IgnoreCase);
            Regex getReleaseNum = new Regex("(?<=PR_CALL CMN(|_P)\\s*\\(\\s*\"\\s*CollZone\\s*\"\\s*\\=\\s*\\d+\\s*,\\s*\"\\s*Release.*ZoneNo\\s*\"\\s*\\=\\s*)\\d+", RegexOptions.IgnoreCase);
            Regex isCommentOrEmptyLine = new Regex(@"^\s*\d+\s*:\s*(!|;)");
            var fileContent = ReadFile(lsFile);
            Dictionary<string, string> points = GetPoints(fileContent);
            List<int> currentCollisions = new List<int>();
            bool isProgramSection = false, collisionSegmentActive = false, initialCollSetFound = false;
            foreach (var line in fileContent)
            {
                if (getRequestNum.IsMatch(line))
                {
                    collisionSegmentActive = true;
                    int currentColl = int.Parse(getRequestNum.Match(line).ToString());
                    if (!currentCollisions.Contains(currentColl))
                        currentCollisions.Add(currentColl);
                    else
                        warningContent.Add("Double collision request. Collision: " + currentColl.ToString());
                }
                else
                {
                    if (!isCommentOrEmptyLine.IsMatch(line) && collisionSegmentActive)
                    {
                        collisionSegmentActive = false;
                        collSegments.Add(GetCollSegmentName(currentCollisions), currentSegment);
                    }
                }
                if (getReleaseNum.IsMatch(line))
                {
                    int currentColl = int.Parse(getReleaseNum.Match(line).ToString());
                    if (currentCollisions.Contains(currentColl))
                        currentCollisions.Remove(currentColl);
                    else
                        warningContent.Add("Collision released but not requested. Collision: " + currentColl.ToString());
                }
                if (!isProgramSection)
                    beginingContent.Add(line);
                else
                    currentSegment.Add(line);
                if (isMNLine.IsMatch(line))
                    isProgramSection = true;
                
            }


        }

        private void ExecuteKUKA(string srcfile)
        {
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(srcfile), Path.GetFileNameWithoutExtension(srcfile) + ".dat")))
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
            bool startAddingLines = false, previousCollReq = false, previousCollClr = false, isFirstCycle = true, wasLastCollCont = false;
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
            SaveFiles(result, Path.GetDirectoryName(srcfile), datFile);
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

        private Dictionary<string, string> GetPoints(List<string> fileContent)
        {
            Regex isPosRegisterStart = new Regex(@"^\s*/\s*POS", RegexOptions.IgnoreCase);
            Regex isEndRegex = new Regex(@"^\s*/\s*END", RegexOptions.IgnoreCase);
            Regex pointNameRegex = new Regex("^\\s*P\\s*\\[\\s*\\d+\\s*(|:\\s*\"[a-zA-Z0-9]+\\s*\")\\](?=\\s*{)", RegexOptions.IgnoreCase);
            Dictionary<string, string> result = new Dictionary<string, string>();
            bool isPosRegister = false;
            string lastPointName = string.Empty;
            string pointcontent = string.Empty;
            foreach (var line in fileContent)
            {
                if (isPosRegister)
                {
                    if (pointNameRegex.IsMatch(line))
                    {
                        if (!string.IsNullOrEmpty(lastPointName))
                            result.Add(lastPointName, pointcontent);
                        lastPointName = pointNameRegex.Match(line).ToString();
                        pointcontent = line + "\r\n";
                    }
                    else if (isEndRegex.IsMatch(line))
                    {
                        isPosRegister = false;
                        if (!string.IsNullOrEmpty(lastPointName))
                            result.Add(lastPointName, pointcontent);
                    }
                    else
                    {
                        pointcontent += line + "\r\n";
                    }
                }
                if (isPosRegisterStart.IsMatch(line))
                    isPosRegister = true;
            }
            return result;
        }

        private string GetCollSegmentName(List<int> currentCollisions)
        {
            throw new NotImplementedException();
        }

    }
}
