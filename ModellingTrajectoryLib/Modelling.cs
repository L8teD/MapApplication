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
        public OutputData outputData2;

        public Modelling(InputData inputData, InitErrors initErrors)
        {
            double[] inputLatArray = Converter.DegToRad(inputData.latitude);
            double[] inputLonArray = Converter.DegToRad(inputData.longitude);
            double[] inputAltArray = inputData.altitude;
            double[] velocity = inputData.velocity;
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString());
            Model.Model(inputLatArray, inputLonArray, inputAltArray, velocity, initErrors);
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString());
            outputData = Model.outputData;
            outputData2 = Model.outputData2;
        }
    }
}
