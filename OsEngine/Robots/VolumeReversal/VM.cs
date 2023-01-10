using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.VolumeReversal
{
    public class VM:BaseVM
    {
        public VM(VolumeReversalRobot robot)
        {
            _robot = robot;
        }
        private VolumeReversalRobot _robot;
    }
}
