using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class GlobalData
    {
        public const string ConfigurationFileName = "Application.config";       
        public enum Action {None, Move, Remove, Copy, CopyData, MoveData, RemoveData}
        public GlobalData()
        { }

        

    }
}
