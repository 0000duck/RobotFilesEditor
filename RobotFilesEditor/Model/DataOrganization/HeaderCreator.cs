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
        public List<string>CreateFileHeader(GlobalData.HeaderType headerType, string destinationFilePath, List<string> fileContent, string sourcePath)
        {
            try
            {
                switch (headerType)
                {
                    case GlobalData.HeaderType.GlobalFileHeader:
                        {
                            if(string.IsNullOrEmpty(sourcePath)==false)
                            {
                                fileContent=GlobalFileHeader(sourcePath, destinationFilePath, fileContent);
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
        public List<string>GlobalFileHeader(string sourcePath, string destinationFilePath, List<string>fileContent)
        {
            List<string> header = new List<string>();
            List<string>paths= FilesTool.GetAllFilesFromDirectory(sourcePath);
            List<string> file = new List<string>();
            string containheaderPattern = @"^;\*+.*(Programm|Beschreibung|Roboter|Firma|Ersteller|Datum|Aenderungsverlauf|\*{20})";
            string programNameLinesPattern = @"^;\*\x20(Programm|Beschreibung)\x20+\:\x20";
            string headerBorderBounds = @"^;\*+$";
            int i = -1;

            try
            {
                #region CreateNewHeader
                foreach (string path in paths)
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

                            header.Add(line);
                            i++;
                            line = file[i];
                        }
                    }
                }
                #endregion CreateNewHeader

                #region RemoveOldHeader
                i = 0;
                int oldHeaderStart = file.FindIndex(x => Regex.IsMatch(x, headerBorderBounds));

                if (oldHeaderStart >= 0 && oldHeaderStart < fileContent?.Count)
                {
                    string line = file[oldHeaderStart + i];
                    fileContent.Remove(line);

                    while (Regex.IsMatch(line, containheaderPattern) == false)
                    {
                        fileContent.Remove(line);
                        i++;
                        line = file[i];
                    }
                }
                #endregion RemoveOldHeader

                #region WriteNewHeaderToFiles
                if (oldHeaderStart >= 0)
                {
                    fileContent.InsertRange(oldHeaderStart, header);
                }
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
        public List<DataFilterGroup> GroupsHeadersByVariableOrderNumber(List<DataFilterGroup> filterGroups, GlobalData.SortType sortType)
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
        public List<DataFilterGroup> CreateGroupHeader(GlobalData.HeaderType headerType, GlobalData.SortType sortType, List<DataFilterGroup> filterGroups)
        {
            try
            {
                switch (headerType)
                {
                    case GlobalData.HeaderType.GroupsHeadersByVariableOrderNumber:
                        {
                            filterGroups = GroupsHeadersByVariableOrderNumber(filterGroups, sortType);
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
        #endregion GroupsHeaders
       
    }
}
