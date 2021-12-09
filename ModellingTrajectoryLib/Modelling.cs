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
        public List<AnglesSet> angles { get; private set; }
        public List<DisplayedData> dDataIdeal { get; private set; }
        public List<DisplayedData> dDataError { get; private set; }
        public List<DisplayedData> dDataReal { get; private set; }
        public List<DisplayedData> dDataEstimate { get; private set; }
        public List<P_out> p_Outs { get; set; }
        public Modelling(InputData inputData, InitErrors initErrors)
        {
            double[] inputLatArray = Converter.DegToRad(inputData.latitude);
            double[] inputLonArray = Converter.DegToRad(inputData.longitude);
            double[] inputAltArray = inputData.altitude;
            double[] velocity = inputData.velocity;
            Model.Model(inputLatArray, inputLonArray, inputAltArray, velocity, initErrors);
            //points = Converter.RadToDeg(Model.returnedPoints);
            points = Model.pointsList;
            velocities = Model.velocityList;
            angles = Model.anglesList;
            dDataIdeal = Model.dDataIdeal;
            dDataError = Model.dDataError;
            dDataReal = Model.dDataReal;
            dDataEstimate = Model.dDataEstimate;
            p_Outs = Model.p_Outs;
        }
    }
}
