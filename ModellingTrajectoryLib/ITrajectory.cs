using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModellingTrajectoryLib
{
    public interface ITrajectory
    {
        //void Model();
        Action FillOutputsData { get; set; }
    }
    public abstract class BaseTrajectory
    {
        protected void InitStartedPoint(ref Parameters parameters, InputData input)
        {
            parameters.point = new Point(input.latitude[0], input.longitude[0], input.altitude[0], Dimension.InRadians);
        }
        protected void InitNextPoint(ref Parameters parameters, List<Parameters> localParams)
        {
            
            parameters.point = localParams[localParams.Count - 1].point;
        }

    }
}
