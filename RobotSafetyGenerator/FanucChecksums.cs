using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotSafetyGenerator
{
    public class FanucChecksums
    {
        public string BaseSum { get; set; }
        public string PosSpeed { get; set; }
        public string IOConnect { get; set; }

        public FanucChecksums(string baseSum, string posSpeed, string ioconnect)
        {
            BaseSum = baseSum;
            PosSpeed = posSpeed;
            IOConnect = ioconnect;
        }
    }
}
