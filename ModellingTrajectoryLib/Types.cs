using ModellingTrajectoryLib.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    public class Types
    {
        public struct Wind
        {
            public double angle { get; set; }
            public double speed { get; set; }
            public double gust { get; set; }
        }

    }
}
