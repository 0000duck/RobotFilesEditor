﻿using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotFilesEditor
{
    public static class FilesMenager
    {
        public static string CreateDestinationFolderPath(string path, string folder)
        {
            string destination = Path.Combine(path, folder);

            try
            {
                if (Directory.Exists(destination) == false)
                {
                    Directory.CreateDirectory(destination);
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }

            return destination;
        }

        public static bool CheckFilesCorrectness(string path, List<string>files)
        {
            List<string> resultFiles = new List<string>();

            try
            {
                resultFiles = Directory.GetFiles(path).ToList();

                if (files.Exists(s => resultFiles.Exists(r => Path.GetFileName(r) == Path.GetFileName(s)) == false))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }

        public static Dictionary<string, string> RemoveFile(Dictionary<string, string> files)
        {           
            IDictionary<string, string> filesIterator = new Dictionary<string, string>(files);

            foreach (var file in filesIterator)
            {
                try
                {
                    File.Delete(file.Key);
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    files.Remove(file.Key);
                    files.Add(file.Key, ex.Message);                   
                }
            }
                return files;    
        }

        public static Dictionary<string, string> MoveFiles(Dictionary<string, string> files, string destination)
        {
            IDictionary<string, string> filesIterator = new Dictionary<string, string>(files);

            foreach (var file in filesIterator)
            {
                try
                {
                    string filePath = Path.Combine(destination, Path.GetFileName(file.Key));

                    if (File.Exists(filePath))
                    {
                        throw new IOException($"File \"{Path.GetFileName(file.Key)}\" already exist!");
                    }

                    File.Move(file.Key, filePath);
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    files.Remove(file.Key);
                    files.Add(file.Key, ex.Message);
                }
            }

            return files;
        }

        public static IDictionary<string, string>CopyFiles(IDictionary<string, string> sourceFiles, string destination)
        {
            SrcValidator.CopiedFiles = false;
            IDictionary<string, string> filesIterator = new Dictionary<string, string>(sourceFiles);
            var result = new Dictionary<string, string>(sourceFiles);

            foreach (var file in filesIterator)
            {
                try
                {
                    string destinationFilePath = Path.Combine(destination, Path.GetFileName(file.Key));

                    if (File.Exists(destinationFilePath))
                    {
                        throw new IOException($"File \"{Path.GetFileName(file.Key)}\" already exist!");
                    }
                    else
                        SrcValidator.CopiedFiles = true;

                    if (file.Value == "")

                    {
                        File.Copy(file.Key, destinationFilePath);
                        System.Threading.Thread.Sleep(50);
                    }
                    else
                    {
                        if (Path.GetExtension(file.Key).ToLower() == ".dat" && GlobalData.LocalHomesFound)
                        {
                            File.WriteAllText(destinationFilePath, SrcValidator.RemoveLocalHomes(file.Value));
                        }
                        else
                            File.WriteAllText(destinationFilePath, file.Value);
                        System.Threading.Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    sourceFiles[file.Key]=ex.Message;
                }
            }

            return sourceFiles;
        }

        public static List<string> GetAllFilesFromDirectoryAndIncludedNested(string path)
        {
            List<string> files = new List<string>();          

            try
            {
                string[] paths = Directory.GetFileSystemEntries(path);

                foreach (var p in paths)
                {
                    if (string.IsNullOrEmpty(Path.GetExtension(p)))
                    {
                        var temp = GetAllFilesFromDirectoryAndIncludedNested(p);

                        if (temp.Count > 0)
                        {
                            files.AddRange(temp);
                        }
                    }
                    else
                    {
                        files.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
            }

            return files;
        }

        public static void WriteTextToFile(List<string> fileText, string path)
        {
            try
            {
                if (fileText?.Count > 0)
                {
                    using (StreamWriter fileStream = new StreamWriter(path))
                    {
                        fileText.ForEach(x => fileStream.WriteLine(x));
                    }
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }

        public static bool WriteTextToExistingFile(string destination, string source, List<string>textToWrite, string befor, string after)
        {
            bool writed = false;
            string filePath = "";

            try
            {
                filePath = GetSourceFilePath(source, destination);
                CreateDestinationFile(filePath, destination);

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    List<string> newFileText = new List<string>();
                    List<string> fileContent = new List<string>();
                    fileContent = GetFileLinePropertiesFromFile(filePath).Select(x => x.LineContent).ToList();

                    ValidateText.ValidateReapitingTextWhitExistContent(fileContent, ref textToWrite);

                    if (textToWrite?.Count > 0 && fileContent?.Count > 0)
                    {
                        if (string.IsNullOrEmpty(after) == false)
                        {
                            foreach (string line in fileContent)
                            {
                                if (line.Contains(after))
                                {
                                    newFileText.Add(line);
                                    newFileText.AddRange(textToWrite);
                                    writed = true;
                                }
                                else
                                {
                                    newFileText.Add(line);
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(befor) == false)
                            {
                                foreach (string line in fileContent)
                                {
                                    if (line.Contains(befor))
                                    {
                                        newFileText.AddRange(textToWrite);
                                        newFileText.Add(line);
                                        writed = true;
                                    }
                                    else
                                    {
                                        newFileText.Add(line);
                                    }
                                }
                            }
                        }                       
                    }

                    if (writed!=true)
                    {
                        newFileText.AddRange(fileContent);
                        newFileText.AddRange(textToWrite);
                    }
                   
                    WriteTextToFile(newFileText, filePath);
                    return true;
                }else
                {
                    filePath = CombineFilePath(source, Path.GetDirectoryName(destination));
                    WriteTextToFile(textToWrite, destination);
                    return true;
                }
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
            return false;
        }

        public static List<FileLineProperties> GetFileLinePropertiesFromFiles(List<string> filesPaths)
        {
            List<FileLineProperties> filesContent = new List<FileLineProperties>();        
            try
            {
                foreach (string path in filesPaths)
                {
                    filesContent.AddRange(GetFileLinePropertiesFromFile(path));
                }

                return filesContent;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }           
        }

        public static List<FileLineProperties>GetFileLinePropertiesFromFile(string path)
        {
            List<FileLineProperties> fileContent = new List<FileLineProperties>();
            FileLineProperties fileLineProperties;
            string[] file;
            int lineNumber=1;

            try
            {
                file = File.ReadAllLines(path);

                foreach (string line in file)
                {
                    fileLineProperties = new FileLineProperties();
                    fileLineProperties.FileLinePath = path;
                    fileLineProperties.LineContent = line;
                    fileLineProperties.LineNumber = lineNumber;
                    lineNumber++;
                    fileContent.Add(fileLineProperties);
                }

                return fileContent;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
           

        }

        public static List<string> GetTextFromFile(string path)
        {
            List<string> text = new List<string>();

            try
            {
                if (string.IsNullOrEmpty(path)==false)
                {
                    text = File.ReadAllLines(path).ToList();
                }

                return text;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }         
        }

        public static string GetSourceFilePath(string sourcePath, string destinationPath)
        {
            string destination = CombineFilePath(sourcePath, destinationPath);

            try
            {
                if (File.Exists(destination))
                {
                    return destination;
                }

                if (File.Exists(sourcePath))
                {
                    return sourcePath;
                }

                return null;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex; 
            }          
        }

        public static void CreateDestinationFile(string sourcePath, string destinationPath)
        {          
            try
            {
                if (string.IsNullOrEmpty(destinationPath) || string.IsNullOrEmpty(sourcePath))
                {
                    throw new Exception($"Destination file {destinationPath} exeption!");
                }

                string sourceFilePath = GetSourceFilePath(sourcePath, destinationPath);
                string destinationFilePath = CombineFilePath(sourcePath, destinationPath);

                if (Directory.Exists(destinationPath) == false)
                {
                    Directory.CreateDirectory(destinationPath);
                }

                if (string.IsNullOrEmpty(sourceFilePath))
                {
                    File.CreateText(destinationFilePath).Close();
                    return;
                }              

                if (Directory.Exists(Path.GetDirectoryName(destinationFilePath))==false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                }               

                if (sourceFilePath.Equals(destinationFilePath))
                {
                    return;
                }

                File.Copy(sourcePath, destinationFilePath);
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }          
        }

        public static string CombineFilePath(string sourcePath, string destinationPath)
        {
            return Path.Combine(destinationPath, Path.GetFileName(sourcePath));
        }

        public static bool DeleteFromFile(string filePath, string fragmentToRemove)
        {
            try
            {
                List<string> fileContent = new List<string>();
                var file = GetFileLinePropertiesFromFile(filePath);
                fileContent = file.Select(x => x.LineContent).ToList();
                if (fileContent.Exists(x => x.Contains(fragmentToRemove)))
                {
                    fileContent.RemoveAll(x => x.Contains(fragmentToRemove));
                    WriteTextToFile(fileContent, filePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }           
        }       

        public static List<string> CutData(List<string>deletedFromPaths, string destinationSourceFilePath, string destinationPastPath, string cutedFragment, string writeBefor, string writeAfter)
        {
            List<string> changedFiles = new List<string>();
            string filePath = FilesMenager.CombineFilePath(destinationSourceFilePath, destinationPastPath);
            deletedFromPaths.Remove(filePath);

            try
            {
                if (string.IsNullOrEmpty(cutedFragment)==false)
                {
                    bool result = WriteTextToExistingFile(destinationPastPath, destinationSourceFilePath, new List<string>() { cutedFragment }, writeBefor, writeAfter);

                    if (result == true)
                    {
                        foreach (string path in deletedFromPaths)
                        {
                            if(DeleteFromFile(path, cutedFragment))
                            {
                                changedFiles.Add(path);
                            }
                        }
                    }
                }

                return changedFiles;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }
    }
}
