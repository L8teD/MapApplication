using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;

namespace MapApplication.Model
{
    public class PlotWorker
    {
        public static void CreatePlotData(ref DisplayGraphicData graphicData, TrackData data)
        {
            if (graphicData == null)
                graphicData = new DisplayGraphicData();

            graphicData.INS = CreatePlotData(data.INS, Source.INS);
            graphicData.GNSS = CreatePlotData(data.GNSS, Source.GNSS);
            graphicData.KVS = CreatePlotData(data.KVS, Source.KVS);
            graphicData.SwitchSource(Source.INS);
        }
        public static List<PlotData> CreatePlotData(OutputData outputData, Source source)
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
                if (pointSet.Ideal != null)
                {
                    latIdeal.Add(pointSet.Ideal.GetValueOrDefault().Degrees.lat);
                    lonIdeal.Add(pointSet.Ideal.GetValueOrDefault().Degrees.lon);
                    altIdeal.Add(pointSet.Ideal.GetValueOrDefault().Degrees.alt);
                }
                    
                if (pointSet.Real != null)
                {
                    latReal.Add(pointSet.Real.GetValueOrDefault().Degrees.lat);
                    lonReal.Add(pointSet.Real.GetValueOrDefault().Degrees.lon);
                    altReal.Add(pointSet.Real.GetValueOrDefault().Degrees.alt);
                }

                if (pointSet.CorrectTrajectory != null)
                {
                    latCorrectTraj.Add(pointSet.CorrectTrajectory.GetValueOrDefault().Degrees.lat);
                    lonCorrectTraj.Add(pointSet.CorrectTrajectory.GetValueOrDefault().Degrees.lon);
                    altCorrectTraj.Add(pointSet.CorrectTrajectory.GetValueOrDefault().Degrees.alt);
                }
                if (pointSet.Error != null)
                {
                    latError.Add(pointSet.Error.GetValueOrDefault().Meters.lat);
                    lonError.Add(pointSet.Error.GetValueOrDefault().Meters.lon);
                    altError.Add(pointSet.Error.GetValueOrDefault().Meters.alt);
                }
                if (pointSet.Estimate != null)
                {
                    latEstimate.Add(pointSet.Estimate.GetValueOrDefault().Meters.lat);
                    lonEstimate.Add(pointSet.Estimate.GetValueOrDefault().Meters.lon);
                    altEstimate.Add(pointSet.Estimate.GetValueOrDefault().Meters.alt);
                }
                if (pointSet.CorrectError != null)
                {
                    latCorrectError.Add(pointSet.CorrectError.GetValueOrDefault().Meters.lat);
                    lonCorrectError.Add(pointSet.CorrectError.GetValueOrDefault().Meters.lon);
                    altCorrectError.Add(pointSet.CorrectError.GetValueOrDefault().Meters.alt);
                }
            }
            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Ideal, source, latIdeal));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Ideal, source, lonIdeal));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Ideal, source, altIdeal));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Real, source, latReal));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Real, source, lonReal));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Real, source, altReal));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.CorrectTrajectory, source, latCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.CorrectTrajectory, source, lonCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.CorrectTrajectory, source, altCorrectTraj));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Error, source, latError));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Error, source, lonError));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Error, source, altError));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.Estimate, source, latEstimate));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.Estimate, source, lonEstimate));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.Estimate, source, altEstimate));

            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.CorrectError, source, latCorrectError));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.CorrectError, source, lonCorrectError));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.CorrectError, source, altCorrectError));
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
                if (velocitySet.Ideal != null)
                {
                    VeIdeal.Add(velocitySet.Ideal.GetValueOrDefault().E);
                    VnIdeal.Add(velocitySet.Ideal.GetValueOrDefault().N);
                    VhIdeal.Add(velocitySet.Ideal.GetValueOrDefault().H);
                }
                if (velocitySet.Real != null)
                {
                    VeReal.Add(velocitySet.Real.GetValueOrDefault().E);
                    VnReal.Add(velocitySet.Real.GetValueOrDefault().N);
                    VhReal.Add(velocitySet.Real.GetValueOrDefault().H);
                }
                if (velocitySet.CorrectTrajectory != null)
                {
                    VeCorrectTraj.Add(velocitySet.CorrectTrajectory.GetValueOrDefault().E);
                    VnCorrectTraj.Add(velocitySet.CorrectTrajectory.GetValueOrDefault().N);
                    VhCorrectTraj.Add(velocitySet.CorrectTrajectory.GetValueOrDefault().H);
                }
                if (velocitySet.Error != null)
                {
                    VeError.Add(velocitySet.Error.GetValueOrDefault().E);
                    VnError.Add(velocitySet.Error.GetValueOrDefault().N);
                    VhError.Add(velocitySet.Error.GetValueOrDefault().H);
                }
                if (velocitySet.Estimate != null)
                {
                    VeEstimate.Add(velocitySet.Estimate.GetValueOrDefault().E);
                    VnEstimate.Add(velocitySet.Estimate.GetValueOrDefault().N);
                    VhEstimate.Add(velocitySet.Estimate.GetValueOrDefault().H);
                }
                if (velocitySet.CorrectError != null)
                {
                    VeCorrectError.Add(velocitySet.CorrectError.GetValueOrDefault().E);
                    VnCorrectError.Add(velocitySet.CorrectError.GetValueOrDefault().N);
                    VhCorrectError.Add(velocitySet.CorrectError.GetValueOrDefault().H);
                }
            }
            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Ideal, source, VeIdeal));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Ideal, source, VnIdeal));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Ideal, source, VhIdeal));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Real, source, VeReal));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Real, source, VnReal));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Real, source, VhReal));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.CorrectTrajectory, source, VeCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.CorrectTrajectory, source, VnCorrectTraj));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.CorrectTrajectory, source, VhCorrectTraj));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Error, source, VeError));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Error, source, VnError));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Error, source, VhError));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.Estimate, source, VeEstimate));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.Estimate, source, VnEstimate));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.Estimate, source, VhEstimate));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.CorrectError, source, VeCorrectError));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.CorrectError, source, VnCorrectError));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.CorrectError, source, VhCorrectError));
            #endregion

            #region Angles Lists
            List<double> headingIdeal = new List<double>();
            List<double> pitchIdeal = new List<double>();
            List<double> rollIdeal = new List<double>();

            List<double> headingReal = new List<double>();
            List<double> pitchReal = new List<double>();
            List<double> rollReal = new List<double>();

            List<double> headingError = new List<double>();
            List<double> pitchError = new List<double>();
            List<double> rollError = new List<double>();

            foreach (AnglesSet anglesSet in outputData.angles)
            {
                if (anglesSet.Ideal != null)
                {
                    headingIdeal.Add(anglesSet.Ideal.GetValueOrDefault().Degrees.heading);
                    pitchIdeal.Add(anglesSet.Ideal.GetValueOrDefault().Degrees.pitch);
                    rollIdeal.Add(anglesSet.Ideal.GetValueOrDefault().Degrees.roll);
                }
                if (anglesSet.Real != null)
                {
                    headingReal.Add(anglesSet.Real.GetValueOrDefault().Degrees.heading);
                    pitchReal.Add(anglesSet.Real.GetValueOrDefault().Degrees.pitch);
                    rollReal.Add(anglesSet.Real.GetValueOrDefault().Degrees.roll);
                }
                if (anglesSet.Error != null)
                {
                    headingError.Add(anglesSet.Error.GetValueOrDefault().Degrees.heading);
                    pitchError.Add(anglesSet.Error.GetValueOrDefault().Degrees.pitch);
                    rollError.Add(anglesSet.Error.GetValueOrDefault().Degrees.roll);
                }
            }
            plotDatas.Add(new PlotData(PlotName.Heading, PlotCharacter.Ideal, source, headingIdeal));
            plotDatas.Add(new PlotData(PlotName.Pitch, PlotCharacter.Ideal, source, pitchIdeal));
            plotDatas.Add(new PlotData(PlotName.Roll, PlotCharacter.Ideal, source, rollIdeal));

            plotDatas.Add(new PlotData(PlotName.Heading, PlotCharacter.Real, source, headingReal));
            plotDatas.Add(new PlotData(PlotName.Pitch, PlotCharacter.Real, source, pitchReal));
            plotDatas.Add(new PlotData(PlotName.Roll, PlotCharacter.Real, source, rollReal));

            plotDatas.Add(new PlotData(PlotName.Heading, PlotCharacter.Error, source, headingError));
            plotDatas.Add(new PlotData(PlotName.Pitch, PlotCharacter.Error, source, pitchError));
            plotDatas.Add(new PlotData(PlotName.Roll, PlotCharacter.Error, source, rollError));

            #endregion

            #region P Lists
            List<double> long_P = new List<double>();
            List<double> lat_P = new List<double>();
            List<double> alt_P = new List<double>();

            List<double> Ve_P = new List<double>();
            List<double> Vn_P = new List<double>();
            List<double> Vh_P = new List<double>();


            foreach (P_out p in outputData.p_OutList)
            {
                long_P.Add(p.lon);
                lat_P.Add(p.lat);
                alt_P.Add(p.alt);

                Ve_P.Add(p.ve);
                Vn_P.Add(p.vn);
                Vh_P.Add(p.vh);
            }
            plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.P, source, long_P));
            plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.P, source, lat_P));
            plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.P, source, alt_P));

            plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.P, source, Ve_P));
            plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.P, source, Vn_P));
            plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.P, source, Vh_P));

            #endregion

            #region Course Air Lists
            //List<double> long_Air = new List<double>();
            //List<double> lat_Air = new List<double>();
            //List<double> alt_Air = new List<double>();

            //List<double> Ve_Air = new List<double>();
            //List<double> Vn_Air = new List<double>();
            //List<double> Vh_Air = new List<double>();


            //foreach (AirData airData in outputData.airData)
            //{
            //    long_Air.Add(airData.point.lon);
            //    lat_Air.Add(airData.point.lat);
            //    alt_Air.Add(airData.point.alt);

            //    Ve_Air.Add(airData.airSpeed.E);
            //    Vn_Air.Add(airData.airSpeed.N);
            //    Vh_Air.Add(airData.airSpeed.H);
            //}
            //plotDatas.Add(new PlotData(PlotName.Latitude, PlotCharacter.CourseAir, lat_Air));
            //plotDatas.Add(new PlotData(PlotName.Longitude, PlotCharacter.CourseAir, long_Air));
            //plotDatas.Add(new PlotData(PlotName.Altitude, PlotCharacter.CourseAir, alt_Air));

            //plotDatas.Add(new PlotData(PlotName.VelocityEast, PlotCharacter.CourseAir, Ve_Air));
            //plotDatas.Add(new PlotData(PlotName.VelocityNorth, PlotCharacter.CourseAir, Vn_Air));
            //plotDatas.Add(new PlotData(PlotName.VelocityH, PlotCharacter.CourseAir, Vh_Air));



            #endregion

            return plotDatas;
        }
        public static List<PlotData> DublicatePlotData(List<PlotData> original)
        {
            if (original == null) return new List<PlotData>();
            List<PlotData> copy = new List<PlotData>();

            copy.AddRange(original);

            //for (int i = 0; i < original.Count; i++)
            //{
            //    List<double> doubleList = new List<double>();
            //    foreach (DataPoint point in original[i].values)
            //        doubleList.Add(point.Y);
            //    PlotData data = new PlotData(original[i].name, original[i].character, doubleList);
            //    copy.Add(data);
            //}
            return copy;
        }
        public static PlotData SelectData(PlotName name, PlotCharacter character, List<PlotData> plotDataList)
        {
            if (plotDataList == null) return null;
            return plotDataList.Find(item => item.name == name && item.character == character);
        }
        public static LineSeries CreateLineSeries(PlotData data, string title)
        {
            if (data == null) return null;
            OxyColor color;
            if (title == "Actual Track")
            {
                color = SelectPlotColor(TrajectoryType.ActualTrack);
            }
            else if (title == "Desired Track")
            {
                color = SelectPlotColor(TrajectoryType.DesiredTrack);
            }
            else
            {
                color = SelectPlotColor(data.character);
            }
            LineSeries series = new LineSeries()
            {
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None,
                Color = color,
                Title = title
            };
            foreach (DataPoint point in data.values)
            {
                series.Points.Add(point);
            }
            return series;
        }
        private static OxyColor SelectPlotColor(PlotCharacter character)
        {
            if (character == PlotCharacter.Ideal || character == PlotCharacter.Estimate)
                return OxyColors.Blue;
            else if (character == PlotCharacter.Real || character == PlotCharacter.Error)
                return OxyColors.Red;
            else if (character == PlotCharacter.CorrectError || character == PlotCharacter.CorrectTrajectory)
                return OxyColors.Green;
            else
                return OxyColors.Black;

        }
        public static OxyColor SelectPlotColor(TrajectoryType type)
        {
            switch (type)
            {
                case TrajectoryType.DesiredTrack:
                    return OxyColors.Blue;
                case TrajectoryType.ActualTrack:
                    return OxyColors.Red;
                default : return OxyColors.Black;
            }
        }
        public static Source SelectSource(string sourceName)
        {
            switch (sourceName)
            {
                case "INS":
                    return Source.INS;
                case "GNSS":
                    return Source.GNSS;
                case "SVS":
                    return Source.KVS;
                default:
                    return Source.INS;
            }
        }
        public static string SelectSource(Source source)
        {
            switch (source)
            {
                case Source.INS:
                    return "INS";
                case Source.GNSS:
                    return "GNSS";
                case Source.KVS:
                    return "SVS";
                default:
                    return "";
            }
        }
        public static PlotCharacter SelectPlotCharacter(string character)
        {
            switch (character)
            {
                case "Ideal":
                    return PlotCharacter.Ideal;
                case "Error":
                    return PlotCharacter.Error;
                case "Estimate":
                    return PlotCharacter.Estimate;
                case "Error - Estimate":
                    return PlotCharacter.CorrectError;
                case "Ideal + Error":
                    return PlotCharacter.Real;
                case "P":
                    return PlotCharacter.P;
                default:
                    return PlotCharacter.None;
            }
        }

        public static string SelectPlotCharacter(PlotCharacter character)
        {
            switch (character)
            {
                case PlotCharacter.Ideal:
                    return "Ideal";
                case PlotCharacter.Real:
                    return "Real";
                case PlotCharacter.CorrectError:
                    return "Correct";
                case PlotCharacter.CorrectTrajectory:
                    return "Correct";
                case PlotCharacter.Error:
                    return "Error";
                case PlotCharacter.Estimate:
                    return "Estimate";
                case PlotCharacter.P:
                    return "P";
                default:
                    return "";
            }
        }
        public static PlotName SelectPlotName(string name)
        {
            switch (name)
            {
                case "Latitude":
                    return PlotName.Latitude;

                case "Longitude":
                    return PlotName.Longitude;

                case "Altitude":
                    return PlotName.Altitude;

                case "East Velocity":
                    return PlotName.VelocityEast;

                case "North Velocity":
                    return PlotName.VelocityNorth;

                case "Vertical Velocity":
                    return PlotName.VelocityH;

                case "Heading":
                    return PlotName.Heading;

                case "Roll":
                    return PlotName.Roll;
                case "Pitch":
                    return PlotName.Pitch;

                default:
                    return PlotName.None;
            }
        }
        public static string SelectPlotName(PlotName name)
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
                case PlotName.None:
                    return "";
                default:
                    return "";
            }
        }
        public static string SelectPlotDimension(PlotName name, PlotCharacter character)
        {
            if (name == PlotName.VelocityEast || name == PlotName.VelocityH || name == PlotName.VelocityNorth)
                return "[m/sec]";
            else
            {
                if (name == PlotName.Heading || name == PlotName.Pitch || name == PlotName.Roll)
                    return "[deg]";
                else
                {
                    if (character == PlotCharacter.Ideal || character == PlotCharacter.Real || character == PlotCharacter.CorrectTrajectory)
                    {
                        if (name == PlotName.Altitude)
                            return "[m]";
                        else
                            return "[deg]";
                    }
                       
                    else
                        return "[m]";
                }
            }

        }


    }
}

