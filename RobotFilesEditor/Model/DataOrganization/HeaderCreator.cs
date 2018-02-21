using System;
using System.Collections.Generic;
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
                            if(sourcePaths?.Count>0)
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
                throw ex;
            }
        }
        private static List<ResultInfo> GlobalFileHeader(List<string> sourcePaths, string destinationFilePath, List<ResultInfo>fileContent)
        {
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
                    file = FilesTool.GetSourceFileText(path);

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
                    while (Regex.IsMatch(fileContent[oldHeaderStop].Content, headerBorderBounds) == false && oldHeaderStop+1 < fileContent?.Count)
                    {
                        oldHeaderStop++;
                    }
                                     
                    fileContent.RemoveRange(oldHeaderStart, oldHeaderStop);                   
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
                throw ex;
            }            
        }
        #endregion FileHeaders

        #region GroupsHeaders
        public static List<DataFilterGroup> CreateGroupHeader(GlobalData.HeaderType headerType,  List<DataFilterGroup> filterGroups, GlobalData.SortType sortType)
        {
            try
            {
                switch (headerType)
                {
                    case GlobalData.HeaderType.GroupsHeadersByVariableOrderNumber:
                        {
                            filterGroups = GroupsHeadersByVariableOrderNumber(filterGroups, GlobalData.SortType.OrderByOrderNumber);
                        }
                        break;
                }

                return filterGroups;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static List<DataFilterGroup> GroupsHeadersByVariableOrderNumber(List<DataFilterGroup> filterGroups, GlobalData.SortType sortType = GlobalData.SortType.OrderByOrderNumber)
        {
            List<DataFilterGroup> result = new List<DataFilterGroup>();
            filterGroups=DataContentSortTool.SortData(filterGroups, sortType);

            foreach (DataFilterGroup group in filterGroups)
            {
                List<FileLineProperties> linesBuffer = new List<FileLineProperties>();
                var groups = group.LinesToAddToFile.GroupBy(y => y.VariableOrderNumber);

                groups.OrderBy(x => x.Key);

                foreach (var g in groups)
                {
                    var groupsToWrite = g.ToList();
                    string groupNumber= string.Format("{0:00}", g.Key);
                    string groupSpace = $";----------------------------------------- ST{groupNumber}_ZN{groupNumber} -----------------------------------------";

                    linesBuffer.Add(new FileLineProperties() { LineContent = groupSpace });
                    linesBuffer.AddRange(g);
                    linesBuffer.Add(new FileLineProperties() { LineContent = groupSpace });
                    linesBuffer.Add(new FileLineProperties() { LineContent = string.Empty });
                }

                group.LinesToAddToFile.Clear();
                group.LinesToAddToFile = linesBuffer;
            }

            return result;
        }
        #endregion GroupsHeaders       
    }
}
