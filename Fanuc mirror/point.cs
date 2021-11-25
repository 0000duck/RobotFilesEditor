using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanuc_mirro
{
    public class Point
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string W { get; set; }
        public string P { get; set; }
        public string R { get; set; }
    }
    public class FileAndPath
    {
        public FileInfo FileName { get; set; }
        public string Path { get; set; }

        public FileAndPath(FileInfo fileName, string path)
        {
            FileName = fileName;
            Path = path;
        }
        public FileAndPath()
        { }
    }

    public class CompleteMirror
    {
        public List<FileAndPath> Paths { get; set; }
        public FileAndPath Workbook { get; set; }

        public CompleteMirror(List<FileAndPath> paths, FileAndPath workbook = null)
        {
            Paths = paths;
            Workbook = workbook;

        }
        public CompleteMirror()
        { }
    }
    public class RobotConf
    {
        public string Arg1;
        public string Arg2;
        public string Arg3;
        public int Arg4;
        public int Arg5;
        public int Arg6;

        public RobotConf(string arg1, string arg2, string arg3, int arg4, int arg5, int arg6)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }
    }
}
