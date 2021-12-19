using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static ModellingErrorsLib3.Types;

namespace ModellingTrajectoryLib
{
    public class Modelling
    {
        TrajectoryModel Model = new TrajectoryModel();
        public OutputData outputData;
        public List<P_out> p_Outs { get; set; }
        public List<X_dot_out> x_Dot_Outs{ get; set; }
        public List<MatlabData> matlabData { get; set; }
        public Modelling(InputData inputData, InitErrors initErrors)
        {
            double[] inputLatArray = Converter.DegToRad(inputData.latitude);
            double[] inputLonArray = Converter.DegToRad(inputData.longitude);
            double[] inputAltArray = inputData.altitude;
            double[] velocity = inputData.velocity;
            Model.Model(inputLatArray, inputLonArray, inputAltArray, velocity, initErrors);

            outputData = Model.outputData;

            p_Outs = Model.p_Outs;
            x_Dot_Outs = Model.x_Dot_Outs;
            matlabData = Model.matlabData;
        }
    }
}
