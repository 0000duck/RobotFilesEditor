using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.XML
{
    public class DestAndSource
    {
        public string DestPath { get; set; }
        public string SourcePath { get; set; }

        public DestAndSource(string destPath, string sourcePath)
        {
            DestPath = destPath;
            SourcePath = sourcePath;
        }
    }
}
