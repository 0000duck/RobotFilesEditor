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
        public enum ControlerTypes {none, KRC2, KRC4};
        public enum Action {None, Move, Remove, Copy}
        public GlobalData()
        { }

        

    }
}
