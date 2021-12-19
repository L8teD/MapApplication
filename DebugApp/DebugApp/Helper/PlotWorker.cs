using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static DebugApp.Model.Types;

namespace DebugApp.Model
{
    public class PlotWorker
    {
        public static List<PlotData> CreatePlotData(OutputData outputData)
        {
            List<PlotData> plotDatas = new List<PlotData>();

            #region Coordinates Lists
            List<double> latIdeal = new List<double>();
            List<double> lonIdeal = new List<double>();
            List<double> altIdeal = new List<double>();

            List<double> latReal = new List<double>();
            List<double> lonReal = new List<double>();
            List<double> altReal = new List<double>();

            List<double> latError = new List<double>();
            List<double> lonError = new List<double>();
            List<double> altError = new List<double>();

            List<double> latEstimate = new List<double>();
            List<double> lonEstimate = new List<double>();
            List<double> altEstimate = new List<double>();

            List<double> latCorrectError = new List<double>();
            List<double> lonCorrectError = new List<double>();
            List<double> altCorrectError = new List<double>();

            List<double> latCorrectTraj = new List<double>();
            List<double> lonCorrectTraj = new List<double>();
            List<double> altCorrectTraj = new List<double>();
            foreach (PointSet pointSet in outputData.points)
            {
                latIdeal.Add(pointSet.Ideal.Degrees.lat);
                lonIdeal.Add(pointSet.Ideal.Degrees.lon);
                altIdeal.Add(pointSet.Ideal.Degrees.alt);

                latReal.Add(pointSet.Real.Degrees.lat);
                lonReal.Add(pointSet.Real.Degrees.lon);
                altReal.Add(pointSet.Real.Degrees.alt);

                latCorrectTraj.Add(pointSet.CorrectTrajectory.Degrees.lat);
                lonCorrectTraj.Add(pointSet.CorrectTrajectory.Degrees.lon);
                altCorrectTraj.Add(pointSet.CorrectTrajectory.Degrees.alt);

                latError.Add(pointSet.Error.Meters.lat);
                lonError.Add(pointSet.Error.Meters.lon);
                altError.Add(pointSet.Error.Meters.alt);

                latEstimate.Add(pointSet.Estimate.Meters.lat);
                lonEstimate.Add(pointSet.Estimate.Meters.lon);
                altEstimate.Add(pointSet.Estimate.Meters.alt);

                latCorrectError.Add(pointSet.CorrectError.Meters.lat);
                lonCorrectError.Add(pointSet.CorrectError.Meters.lon);
                altCorrectError.Add(pointSet.CorrectError.Meters.alt);
            }
            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Ideal,  latIdeal));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Ideal,  lonIdeal));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Ideal,  altIdeal));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Real,  latReal));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Real,  lonReal));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Real,  altReal));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.CorrectTrajectory,  latCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.CorrectTrajectory,  lonCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.CorrectTrajectory,  altCorrectTraj));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Error,  latError));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Error,  lonError));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Error,  altError));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Estimate,  latEstimate));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Estimate, lonEstimate));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Estimate,  altEstimate));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.CorrectError,  latCorrectError));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.CorrectError,  lonCorrectError));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.CorrectError,  altCorrectError));
            #endregion

            #region Velocities Lists
            List<double> VeIdeal = new List<double>();
            List<double> VnIdeal = new List<double>();
            List<double> VhIdeal = new List<double>();

            List<double> VeReal = new List<double>();
            List<double> VnReal = new List<double>();
            List<double> VhReal = new List<double>();

            List<double> VeError = new List<double>();
            List<double> VnError = new List<double>();
            List<double> VhError = new List<double>();

            List<double> VeEstimate = new List<double>();
            List<double> VnEstimate = new List<double>();
            List<double> VhEstimate = new List<double>();

            List<double> VeCorrectError = new List<double>();
            List<double> VnCorrectError = new List<double>();
            List<double> VhCorrectError = new List<double>();

            List<double> VeCorrectTraj = new List<double>();
            List<double> VnCorrectTraj = new List<double>();
            List<double> VhCorrectTraj = new List<double>();
            foreach (VelocitySet velocitySet in outputData.velocities)
            {
                VeIdeal.Add(velocitySet.Ideal.E);
                VnIdeal.Add(velocitySet.Ideal.N);
                VhIdeal.Add(velocitySet.Ideal.H);

                VeReal.Add(velocitySet.Real.E);
                VnReal.Add(velocitySet.Real.N);
                VhReal.Add(velocitySet.Real.H);

                VeCorrectTraj.Add(velocitySet.CorrectTrajectory.E);
                VnCorrectTraj.Add(velocitySet.CorrectTrajectory.N);
                VhCorrectTraj.Add(velocitySet.CorrectTrajectory.H);

                VeError.Add(velocitySet.Error.E);
                VnError.Add(velocitySet.Error.N);
                VhError.Add(velocitySet.Error.H);

                VeEstimate.Add(velocitySet.Estimate.E);
                VnEstimate.Add(velocitySet.Estimate.N);
                VhEstimate.Add(velocitySet.Estimate.H);

                VeCorrectError.Add(velocitySet.CorrectError.E);
                VnCorrectError.Add(velocitySet.CorrectError.N);
                VhCorrectError.Add(velocitySet.CorrectError.H);
            }
            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Ideal,  VeIdeal));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Ideal,  VnIdeal));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Ideal,  VhIdeal));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Real,  VeReal));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Real,  VnReal));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Real,  VhReal));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.CorrectTrajectory,  VeCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.CorrectTrajectory,  VnCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.CorrectTrajectory,  VhCorrectTraj));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Error,  VeError));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Error,  VnError));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Error,  VhError));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Estimate,  VeEstimate));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Estimate,  VnEstimate));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Estimate,  VhEstimate));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.CorrectError,  VeCorrectError));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.CorrectError,  VnCorrectError));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.CorrectError, VhCorrectError));
            #endregion
            return plotDatas;
        }
        public static PlotData SelectData(PlotName name, PlotCharacter character, List<PlotData> plotDataList)
        {
            return plotDataList.Find(item => item.name == name && item.character == character);
        }
        public static LineSeries CreateLineSeries(List<DataPoint> points)
        {
            LineSeries series = new LineSeries()
            {
                //ItemsSource = data,
                DataFieldX = "x",
                DataFieldY = "Y",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None
            };
            foreach (DataPoint point in points)
            {
                series.Points.Add(point);
            }
            return series;
        }
        public static string SelectPlotAxisName(PlotName name)
        {
            switch (name)
            {
                case PlotName.Latitude:
                    return "Latitude";
                case PlotName.Longitude:
                    return "Longitude";
                case PlotName.Altitude:
                    return "Altitude";
                case PlotName.VelocityEast:
                    return "East Velocity";
                case PlotName.VelocityNorth:
                    return "North Velocity";
                case PlotName.VelocityH:
                    return "Vertical Velocity";
                case PlotName.Heading:
                    return "Heading";
                case PlotName.Roll:
                    return "Roll";
                case PlotName.Pitch:
                    return "Pitch";
                default:
                    return "";
            }
        }
        public static string SelectPlotDimension(PlotName name, PlotCharacter character)
        {
            if (name == PlotName.VelocityEast || name == PlotName.VelocityH || name == PlotName.VelocityNorth)
                return "m/sec";
            else
            {
                if (name == PlotName.Heading || name == PlotName.Pitch || name == PlotName.Roll)
                    return "deg";
                else
                {
                    if (character == PlotCharacter.Ideal || character == PlotCharacter.Real || character == PlotCharacter.CorrectTrajectory)
                        return "deg";
                    else
                        return "m";
                }            
            }
            
        }
       

    }
}

