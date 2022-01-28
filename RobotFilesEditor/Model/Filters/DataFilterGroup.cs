using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;

namespace RobotFilesEditor
{
    public class DataFilterGroup
    {
        
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    _headerType = GlobalData.ChekIfHeaderIsCreatingByMethod(value);
                    if (_headerType != GlobalData.HeaderType.None)
                    {
                        _header = string.Empty;
                    }
                }
            }
        }
        public string Footer
        {
            get { return _footer; }
            set
            {
                if (_footer != value)
                {
                    _footer = value;
                }
            }
        }
        public string TextBefore
        {
            get { return _textBefore; }
            set
            {
                if (_textBefore != value)
                {
                    _textBefore = value;
                }
            }
        }
        public int SpaceBefor
        {
            get { return _spaceBefor; }
            set
            {
                if (_spaceBefor != value)
                {
                    _spaceBefor = value;
                }
            }
        }
        public int SpaceAfter
        {
            get { return _spaceAfter; }
            set
            {
                if (_spaceAfter != value)
                {
                    _spaceAfter = value;
                }
            }
        }
        public Filter Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                }
            }
        }
        public bool OnlyRegex
        {
            get { return _onlyRegex; }
            set
            {

                if (_onlyRegex != value)
                {
                    _onlyRegex = value;
                }
            }
        }
        public List<FileLineProperties> LinesToAddToFile
        {
            get { return _linesToAddToFile; }
            set
            {
                if (_linesToAddToFile != value)
                {
                    _linesToAddToFile = value;
                }
            }
        }

        private string _header;
        private string _footer;
        private string _textBefore;
        private int _spaceBefor;
        private int _spaceAfter;
        private Filter _filter;
        private bool _onlyRegex;
        private GlobalData.HeaderType _headerType;
        private List<FileLineProperties> _linesToAddToFile;

        public DataFilterGroup()
        {
            Filter = new Filter();
            LinesToAddToFile = new List<FileLineProperties>();
        }

        public List<FileLineProperties> CkeckAllFilters(List<FileLineProperties> listToCheck, bool onlyRegex)
        {
            if(listToCheck.Any())
            {
                listToCheck = Filter.CheckAllFilters(listToCheck, onlyRegex);
                if (Filter.Contain.Contains("XHOME"))
                {
                    listToCheck = Model.Operations.SrcValidator.FixMissingExternalAxis(listToCheck);
                    listToCheck = Model.Operations.SrcValidator.CorrectHomeRound(listToCheck);
                }
                if (Filter.Contain.Contains("POZYCJE CENTRALNE"))
                    listToCheck = Model.Operations.SrcValidator.FixMissingExternalAxis(listToCheck);
            }
            return listToCheck;
        }

        public void SetLinesToAddToFile(ref List<string> usedFiles, List<FileLineProperties> filesContent, bool deleteDuplicates=true)
        {
            //dodać zmienną i do konfiguracji
            OnlyRegex = false;
            //TT poprawic
            foreach(string item in Filter.Contain.Where(x => (x == "CouplePos"| x== "TipDressG" | x == "TipCheckG") & (GlobalData.ControllerType == "KRC2 V8" | GlobalData.ControllerType == "KRC2 L6")))
            { 
                OnlyRegex = true;
                break;
            }
            
             LinesToAddToFile=CkeckAllFilters(filesContent, OnlyRegex);
            

            if(usedFiles!=null)
            {
                int counter = 0;
                foreach (var line in LinesToAddToFile)
                {
                    if (OnlyRegex)
                    {
                        string tempstring = line.LineContent.Replace("GLOBAL ", "");
                        LinesToAddToFile[counter].LineContent = tempstring;
                        counter++;
                    }
                    usedFiles.Add(line.FileLinePath);
                }
                usedFiles?.Distinct();
            }          
            
            
            LinesToAddToFile = LinesToAddToFile.DistinctBy(x => x.LineContent).ToList();

            if (Filter.Contain.Contains("POZYCJE CENTRALNE"))
            {
                LinesToAddToFile = Model.Operations.SrcValidator.FilterGlobalFDATs(LinesToAddToFile);
                ModifyLinesToAdd();
            }
            if (Filter.Contain.Contains("g_tipdressg") && GlobalData.isWeldingRobot)
            {
                bool tipdressfound = false;
                foreach (var item in LinesToAddToFile.Where(x => x.VariableName.ToLower().Contains("g_tipdressg")))
                {
                    tipdressfound = true;
                    break;
                }
                if (!tipdressfound)
                    MessageBox.Show("No g_TipDressG definition found! Correct name.","Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
            }

            bool[] fileExist = new bool[] { false, false, false,false };
            if (Filter.Contain.Contains("g_beforeTipDressG") && GlobalData.isWeldingRobot)
            {
                List<FileLineProperties> currentList = new List<FileLineProperties>();
                foreach (var line in LinesToAddToFile)
                {
                    if (line.VariableName.ToLower().Contains("xg_beforetipdressg"))
                        fileExist[0] = true;
                    if (line.VariableName.ToLower().Contains("fg_beforetipdressg"))
                        fileExist[1] = true;
                    if (line.VariableName.ToLower().Contains("xg_centrtipdressg"))
                        fileExist[2] = true;
                    if (line.VariableName.ToLower().Contains("fg_centrtipdressg"))
                        fileExist[3] = true;
                }
                string message = "";
                if (fileExist[0] == false)
                    message += "xg_BeforeTipDressG, ";
                if (fileExist[1] == false)
                    message += "fg_BeforeTipDressG, ";
                if (fileExist[2] == false)
                    message += "xg_CentrTipDressG, ";
                if (fileExist[3] == false)
                    message += "fg_CentrTipDressG, ";
                if (message.Length > 0)
                {
                    message = message.Remove(message.Length - 2, 2);

                    MessageBox.Show("Not definitions for " + message + " found. Correct names", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }   
            }
        }

        private void ModifyLinesToAdd()
        {
            List<FileLineProperties> lines = new List<FileLineProperties>();
            int counter = 1;
            StringReader reader = new StringReader(Properties.Resources.UserVariables);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                    break;
                if (!line.Contains("{LINES}"))
                {
                    if (!line.Contains("{ADD_OR_NOT}"))
                        lines.Add(new FileLineProperties() { LineContent = line.Replace("{USER_NAME}", ConfigurationManager.AppSettings["Ersteller"]).Replace("{DATE}", DateTime.Now.ToString("yyyy.MM.dd")), HasExeption = false, Variable = "CurrentVariable" + counter });
                    else
                    {
                        if (LinesToAddToFile.Count > 0)
                            lines.Add(new FileLineProperties() { LineContent = line.Replace("{ADD_OR_NOT}", "").Replace("{USER_NAME}", ConfigurationManager.AppSettings["Ersteller"]).Replace("{DATE}", DateTime.Now.ToString("yyyy.MM.dd")), HasExeption = false, Variable = "CurrentVariable" + counter });
                    }
                }
                else
                    foreach (var item in LinesToAddToFile)
                        lines.Add(item);
                counter++;
            }
            LinesToAddToFile = lines;
            
        }

        public void DistinctLinesToAddToFile()
        {
            LinesToAddToFile = LinesToAddToFile.DistinctBy(x => x.LineContent).ToList();
        }

        public void PrepareGroupToWrite(ref List<ResultInfo> resultInfos, GlobalData.SortType sortType)
        {
            if (resultInfos == null)
            {
                resultInfos = new List<ResultInfo>();
            }

            if (LinesToAddToFile.Count > 0)
            {              
                if(_headerType!=GlobalData.HeaderType.None)
                {                    
                   LinesToAddToFile = HeaderCreator.CreateGroupHeader(_headerType, LinesToAddToFile, sortType);                   
                }                  

                for (int i = 0; i < SpaceBefor; i++)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(""));
                }

                if (string.IsNullOrEmpty(Header) == false)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(Header));
                }

                foreach(var line in LinesToAddToFile)
                {
                    if (string.IsNullOrEmpty(TextBefore) == false)
                    {
                        var tmp = line;
                        tmp.LineContent = $"{TextBefore} {line.LineContent}";
                        resultInfos.Add(ResultInfo.CreateResultInfo(tmp));
                    }
                    else
                    {
                        resultInfos.Add(ResultInfo.CreateResultInfo(line));
                    }                   
                }
               
                if (string.IsNullOrEmpty(Footer) == false)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(Footer));
                }

                for (int i = 0; i < SpaceAfter; i++)
                {
                    resultInfos.Add(ResultInfo.CreateResultInfo(String.Format("")));
                }              
            }                 
        }

        public void ClearResult()
        {
            LinesToAddToFile = new List<FileLineProperties>();           
        }
    }
}
