using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstimateLib;
using MyMatrix;

namespace ModellingTrajectoryLib
{
    public class DesiredTrack : BaseTrajectory, ITrajectory
    {
       protected override int RandomSeed { get { return 1; } set { } }
    }
    public class ActualTrack : BaseTrajectory, ITrajectory
    {
        protected override int RandomSeed { get { return 1; } set { } }
    }
}
