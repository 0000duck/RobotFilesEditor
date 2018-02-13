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
           
    }
}
