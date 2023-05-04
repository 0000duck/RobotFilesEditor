using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public static class CommonGlobalData
    {
        public static bool IsAIUTUser { get; set; }
        public static string ConfigurationFileName = CommonMethods.GetApplicationConfig();

        public const string serverPath = @"\\alfa\Dzialy\automatyka\Robotyka\BMW\02_Tools\RobotFilesHarvester\"; 
        //public const string serverPath = @"C:\Projekty\inne\Harvester\";
    }
}
