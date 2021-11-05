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
        public List<PointSet> points { get; private set; }
        public List<VelocitySet> velocities { get; private set; }
        public List<DisplayedData> displayedDatasIdeal { get; private set; }
        public List<DisplayedData> displayedDatasError { get; private set; }
        public Modelling(InputData inputData, InitErrors initErrors)
        {
            double[] inputLatArray = Converter.DegToRad(inputData.latitude);
            double[] inputLonArray = Converter.DegToRad(inputData.longitude);
            double[] inputAltArray = inputData.altitude;
            double[] velocity = inputData.velocity;
            Model.Model(inputLatArray, inputLonArray, inputAltArray, velocity, initErrors);
            //points = Converter.RadToDeg(Model.returnedPoints);
            points = Model.outputPointsList;
            velocities = Model.outputVelocityList;
            displayedDatasIdeal = Model.outputDisplayedDataIdeal;
            displayedDatasError = Model.outputDisplayedDataError;
        }
    }
}
