using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RobotFilesEditor
{
    public static class FilesTool
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
                    files.Remove(file.Key);
                    files.Add(file.Key, ex.Message);
                }
            }

            return files;
        }

        public static Dictionary<string, string>CopyFiles(Dictionary<string, string> files, string destination)
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

                    File.Copy(file.Key, filePath);
                }
                catch (Exception ex)
                {
                    files.Remove(file.Key);
                    files.Add(file.Key, ex.Message);
                }
            }

            return files;
        }

        public static List<string> GetAllFilesFromDirectory(string path)
        {
            List<string> files = new List<string>();

            try
            {
                string[] paths = Directory.GetFileSystemEntries(path);

                foreach (var p in paths)
                {
                    if (string.IsNullOrEmpty(Path.GetExtension(p)))
                    {
                        var temp = GetAllFilesFromDirectory(p);

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
                throw ex;
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
                throw ex;
            }
        }

        public static List<FileLineProperties> LoadTextFromFiles(List<string> filesPaths)
        {
            List<FileLineProperties> filesContent = new List<FileLineProperties>();
            FileLineProperties fileLineProperties;
            string[] fileContent;
            int lineNumber;

            try
            {
                foreach (string path in filesPaths)
                {
                    lineNumber = 1;
                    fileContent = File.ReadAllLines(path);

                    foreach (string line in fileContent)
                    {
                        fileLineProperties = new FileLineProperties();
                        fileLineProperties.FileLinePath = path;
                        fileLineProperties.LineContent = line;
                        fileLineProperties.LineNumber = lineNumber;
                        lineNumber++;
                        filesContent.Add(fileLineProperties);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return filesContent;
        }

        public static List<string> GetSourceFileText(string path)
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
                throw ex;
            }         
        }

        public static string GetSourceFilePath(string sourcePath, string destinationPath)
        {
            destinationPath = CombineFilePath(sourcePath, destinationPath);

            try
            {
                if (File.Exists(destinationPath))
                {
                    return destinationPath;
                }

                if (File.Exists(sourcePath))
                {
                    return sourcePath;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex; 
            }          
        }

        public static void CreateDestinationFile(string sourcePath, string destinationPath)
        {
            try
            {
                if (string.IsNullOrEmpty(destinationPath))
                {
                    throw new Exception($"Destination file {destinationPath} exeption!");
                }

                if (Directory.Exists(Path.GetDirectoryName(destinationPath))==false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                }

                if (string.IsNullOrEmpty(sourcePath))
                {
                    File.CreateText(destinationPath).Close();
                    return;
                }

                if (sourcePath.Equals(destinationPath))
                {
                    return;
                }
                File.Copy(sourcePath, destinationPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }

        public static string CombineFilePath(string sourcePath, string destinationPath)
        {
            return Path.Combine(destinationPath, Path.GetFileName(sourcePath));
        }
    }
}
