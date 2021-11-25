using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RobotFilesEditor
{
    public class HeaderCreator
    {
        #region FileHeaders
        public static List<ResultInfo>CreateFileHeader(GlobalData.HeaderType headerType, string destinationFilePath, List<ResultInfo> fileContent, List<string> sourcePaths)
        {
            try
            {
                switch (headerType)
                {
                    case GlobalData.HeaderType.GlobalFileHeader:
                        {
                            if(sourcePaths.Any())
                            {
                                fileContent=GlobalFileHeader(sourcePaths, destinationFilePath, fileContent);
                            }                           
                        }
                        break;                   
                }
                return fileContent;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }
        private static List<ResultInfo> GlobalFileHeader(List<string> sourcePaths, string destinationFilePath, List<ResultInfo>fileContent)
        {
            //TEMPORARY
            return fileContent;
            List<ResultInfo> header = new List<ResultInfo>();
            List<string> file = new List<string>();
            string containheaderPattern = @"^;\*+.*(Programm|Beschreibung|Roboter|Firma|Ersteller|Datum|Aenderungsverlauf|\*{20})";
            string programNameLinesPattern = @"^;\*\x20(Programm|Beschreibung)\x20+\:\x20";
            string headerBorderBounds = @"^;\*+$";
            int i = -1;

            try
            {
                #region CreateNewHeader
                foreach (string path in sourcePaths)
                {
                    file = FilesMenager.GetTextFromFile(path);

                    i = file.FindIndex(x => Regex.IsMatch(x, containheaderPattern));

                    if (i >= 0 && i < file?.Count)
                    {
                        string line = file[i];

                        while (Regex.IsMatch(line, containheaderPattern))
                        {
                            if (Regex.IsMatch(line, programNameLinesPattern))
                            {
                                string match = Regex.Match(line, programNameLinesPattern).Value;
                                line = $"{match}{Path.GetFileNameWithoutExtension(destinationFilePath)}";
                            }

                            if (line.ToLower().Contains("firma"))
                                line = ";* Firma               : AIUT";
                            if (line.ToLower().Contains("ersteller"))
                                line = ";* Ersteller           : " + ConfigurationManager.AppSettings["Ersteller"];
                            header.Add(ResultInfo.CreateResultInfo(line));
                            i++;
                            line = file[i];
                        }                       
                    }

                    if (header?.Count > 0)
                    {
                        break;
                    }
                }
                #endregion CreateNewHeader

                #region RemoveOldHeader                
                int oldHeaderStart = fileContent.FindIndex(x => Regex.IsMatch(x.Content, headerBorderBounds));
                int oldHeaderStop = oldHeaderStart+1;

                if (oldHeaderStart >= 0)
                {
                    while (Regex.IsMatch(fileContent[oldHeaderStop].Content, headerBorderBounds) == false && oldHeaderStop < fileContent?.Count)
                    {
                        oldHeaderStop++;
                    }
                                     
                    fileContent.RemoveRange(oldHeaderStart, oldHeaderStop-oldHeaderStart+1);                   
                }                
                #endregion RemoveOldHeader

                #region WriteNewHeaderToFiles
                if(oldHeaderStart<0)
                {
                    oldHeaderStart = 0;
                }
               
                fileContent.InsertRange(oldHeaderStart, header);              
                
                #endregion WriteNewHeaderToFiles

                return fileContent;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }            
        }
        #endregion FileHeaders

        #region GroupsHeaders
        public static List<FileLineProperties> CreateGroupHeader(GlobalData.HeaderType headerType,  List<FileLineProperties> linesToAddToFile, GlobalData.SortType sortType)
        {
            try
            {
                switch (headerType)
                {
                    case GlobalData.HeaderType.GroupsHeadersByVariableOrderNumber:
                        {
                            linesToAddToFile = GroupsHeadersByVariableOrderNumber(linesToAddToFile, GlobalData.SortType.OrderByOrderNumber);
                        }
                        break;
                }

                return linesToAddToFile;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }
        private static List<FileLineProperties> GroupsHeadersByVariableOrderNumber(List<FileLineProperties> linesToAddToFile, GlobalData.SortType sortType = GlobalData.SortType.OrderByOrderNumber)
        {
            //TEMPORARY
            //return linesToAddToFile;
            List<FileLineProperties> linesBuffer = new List<FileLineProperties>();
            //var groups = linesToAddToFile.GroupBy(y => y.VariableOrderNumber);
            var groups = linesToAddToFile.GroupBy(y => y.LineNumber);

            groups.OrderBy(x => x.Key);

            foreach (var g in groups)
            {
                var groupsToWrite = g.ToList();
                string groupNumber= string.Format("{0:00}", g.Key);
                string groupSpace = $";----------------------------------------- ST{groupNumber}_ZN{groupNumber} -----------------------------------------";

                //linesBuffer.Add(new FileLineProperties() { LineContent = groupSpace });
                linesBuffer.AddRange(g);
                //linesBuffer.Add(new FileLineProperties() { LineContent = groupSpace });
                //linesBuffer.Add(new FileLineProperties() { LineContent = "\n" });
            }               

            return linesBuffer;
        }
        #endregion GroupsHeaders       
    }
}
