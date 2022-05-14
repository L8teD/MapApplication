using CommonLib;
using CommonLib.Params;
using MapApplication.Model.Helper;
using MapApplication.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using System.Windows.Controls;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using static MapApplication.Model.Helper.Logger;


namespace MapApplication.Model
{
    public class MainModel
    {
        T_OutputFull OutputData;
        TrackData indicatedData;
        List<DebugInfo> infoList;
        DebugInfo selectedInfo;
        public DisplayGraphicData indicatedPlotData;
        private DisplayGraphicData desiredPlotData;
        private DisplayGraphicData actualPlotData;
        private DisplayGraphicData desiredFeedbackPlotData;
        private DisplayGraphicData actualFeedbackPlotData;

        private List<BasicGeoposition> trajectoryPoints;

        public event Action<string, string, List<LineSeries>> RefreshLongitudePlot;
        public event Action<string, string, List<LineSeries>> RefreshLatitudePlot;
        public event Action<string, string, List<LineSeries>> RefreshAltitudePlot;
        public event Action<string, string, List<LineSeries>> RefreshV_EastPlot;
        public event Action<string, string, List<LineSeries>> RefreshV_NorthPlot;
        public event Action<string, string, List<LineSeries>> RefreshV_VerticalPlot;
        public event Action<string, string, List<LineSeries>> RefreshHeadingPlot;
        public event Action<string, string, List<LineSeries>> RefreshPitchPlot;
        public event Action<string, string, List<LineSeries>> RefreshRollPlot;

        public event Action<DisplayGraphicData, DisplayGraphicData> SetPlotData;

        public event Action<TrackData, int> UpdateTableData;

        public event Action<List<BasicGeoposition>, int> DrawTrajectoryAction;

        System.Timers.Timer timerTrajectory;
        int second = 1;
        public MainModel()
        {
            timerTrajectory = new System.Timers.Timer(1000);
            timerTrajectory.Elapsed += TimerTrajectory_Elapsed;
        }

        public void SwitchIndicatedData(DataSource source)
        {
            if (indicatedData.INS.points == null) return;
            switch (source)
            {
                case DataSource.threeChannel:
                    indicatedData = DublicateTrackData(OutputData.Default.ActualTrack);
                    break;
                case DataSource.twoChannel:
                    indicatedData = DublicateTrackData(OutputData.Default.DesiredTrack);
                    break;
                case DataSource.threeChannelFeedback:
                    indicatedData = DublicateTrackData(OutputData.Feedback.ActualTrack);
                    break;
                case DataSource.twoChannelFeedback:
                    indicatedData = DublicateTrackData(OutputData.Feedback.DesiredTrack);
                    break;
            }
        }
        public void DrawFullTrajctory()
        {
            RefreshDrawingTrajectoryParams();
            while (second < indicatedData.INS.points.Count)
            {
                trajectoryPoints.Add(new BasicGeoposition()
                {
                    Latitude = indicatedData.INS.points[second].CorrectTrajectory.GetValueOrDefault().Degrees.lat,
                    Longitude = indicatedData.INS.points[second].CorrectTrajectory.GetValueOrDefault().Degrees.lon,
                    Altitude = indicatedData.INS.points[second].CorrectTrajectory.GetValueOrDefault().Degrees.alt
                    //Latitude = indicatedData.airData[second].point.lat,
                    //Longitude = indicatedData.airData[second].point.lon,
                    //Altitude = indicatedData.airData[second].point.alt
                });
                
                MathTransformation.IncrementValue(ref second);
            }

            DrawTrajectoryAction?.Invoke(trajectoryPoints, 1);
        }
        private void RefreshDrawingTrajectoryParams()
        {
            second = 1;
            if (trajectoryPoints == null)
                trajectoryPoints = new List<BasicGeoposition>();
            trajectoryPoints.Clear();
        }
        private void TimerTrajectory_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (second < indicatedData.INS.points.Count)
            {
                trajectoryPoints.Add(new BasicGeoposition()
                {
                    Latitude = indicatedData.INS.points[second - 1].CorrectTrajectory.GetValueOrDefault().Degrees.lat,
                    Longitude = indicatedData.INS.points[second - 1].CorrectTrajectory.GetValueOrDefault().Degrees.lon
                });
                trajectoryPoints.Add(new BasicGeoposition()
                {
                    Latitude = indicatedData.INS.points[second].CorrectTrajectory.GetValueOrDefault().Degrees.lat,
                    Longitude = indicatedData.INS.points[second].CorrectTrajectory.GetValueOrDefault().Degrees.lon
                });

                DrawTrajectoryAction?.Invoke(trajectoryPoints, 1);
                MathTransformation.IncrementValue(ref second);

                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");

                UpdateTableData?.Invoke(indicatedData, second - 1);
            }
            else
                timerTrajectory.Enabled = false;
        }

        public void AddWayPoint(ObservableCollection<WayPoint> wayPointList, WayPoint wayPoint)
        {
            ListViewWorker.UpdateData(wayPointList, wayPoint);
        }
        public void RemoveWayPoint(ObservableCollection<WayPoint> wayPointList, int id)
        {
            ListViewWorker.RemoveElement(wayPointList, id);
        }
        public void Simulate()
        {
            timerTrajectory.Start();
        }
        public void Start()
        {
            
        }
        public void Pause()
        {
            timerTrajectory.Enabled = false;
        }
        public void Stop()
        {
            timerTrajectory.Stop();
        }
        public void SetDrawingSpeed(int timerInterval)
        {
            timerTrajectory.Interval = timerInterval;
        }
        public void Compute(InitData initData)
        {
            RefreshDrawingTrajectoryParams();
            OutputData = new T_OutputFull();
            try
            {
                Execute.CreateTrajectory(initData, ref OutputData);
                indicatedData = DublicateTrackData(OutputData.Default.DesiredTrack);
                CreatePlotData();
                RefreshPlots();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                Logger.PrintErrorInfo(ex.Message, initData);
            }
        }
        public void RefreshPlots()
        {
            Plot(RefreshLongitudePlot, PlotName.Longitude);
            Plot(RefreshLatitudePlot, PlotName.Latitude);
            Plot(RefreshAltitudePlot, PlotName.Altitude);
            Plot(RefreshV_EastPlot, PlotName.VelocityEast);
            Plot(RefreshV_NorthPlot, PlotName.VelocityNorth);
            Plot(RefreshV_VerticalPlot, PlotName.VelocityH);
            Plot(RefreshHeadingPlot, PlotName.Heading);
            Plot(RefreshPitchPlot, PlotName.Pitch);
            Plot(RefreshRollPlot, PlotName.Roll);
        }
        private void Plot(Action<string, string, List<LineSeries>> action, PlotName name)
        {
            PlotData plotData;
            List<LineSeries> lineSeriesList = new List<LineSeries>();
            plotData = PlotWorker.SelectData(name, PlotCharacter.Ideal, indicatedPlotData.Display);

            lineSeriesList.Add(PlotWorker.CreateLineSeries(plotData, PlotWorker.SelectPlotCharacter(plotData.character)));

            plotData = PlotWorker.SelectData(name, PlotCharacter.Real, indicatedPlotData.Display);
            lineSeriesList.Add(PlotWorker.CreateLineSeries(plotData, PlotWorker.SelectPlotCharacter(plotData.character)));

            if (name != PlotName.Pitch && name != PlotName.Heading && name != PlotName.Roll)
            {
                plotData = PlotWorker.SelectData(name, PlotCharacter.CorrectTrajectory, indicatedPlotData.Display);
                lineSeriesList.Add(PlotWorker.CreateLineSeries(plotData, PlotWorker.SelectPlotCharacter(plotData.character)));
            }

            action.Invoke(plotData.xAxisName, plotData.yAxisName, lineSeriesList);
        }
        private void CreatePlotData()
        {
            PlotWorker.CreatePlotData(ref indicatedPlotData, OutputData.Default.DesiredTrack);
            PlotWorker.CreatePlotData(ref desiredPlotData, OutputData.Default.DesiredTrack);
            PlotWorker.CreatePlotData(ref desiredFeedbackPlotData, OutputData.Feedback.DesiredTrack);
            PlotWorker.CreatePlotData(ref actualPlotData, OutputData.Default.ActualTrack);
            PlotWorker.CreatePlotData(ref actualFeedbackPlotData, OutputData.Feedback.ActualTrack);


            SetPlotData.Invoke(desiredPlotData, actualPlotData);
        }
        public void SwitchPlotData(DataSource source)
        {
            if (indicatedPlotData == null) return;
            switch (source)
            {
                case DataSource.threeChannel:
                    indicatedPlotData = actualPlotData.Copy();
                    break;
                case DataSource.twoChannel:
                    indicatedPlotData = desiredPlotData.Copy();
                    break;
                case DataSource.threeChannelFeedback:
                    indicatedPlotData = actualFeedbackPlotData.Copy();
                    break;
                case DataSource.twoChannelFeedback:
                    indicatedPlotData = desiredFeedbackPlotData.Copy();
                    break;
            }
            RefreshPlots();
        }
        
        private TrackData DublicateTrackData(TrackData original)
        {
            TrackData copy = new TrackData();

            copy.INS = DublicateOutputData(original.INS);
            copy.GNSS = DublicateOutputData(original.GNSS);
            copy.KVS = DublicateOutputData(original.KVS);

            return copy;
        }
        private OutputData DublicateOutputData(OutputData original)
        {
            OutputData copy = new OutputData();

            if (original.points != null)
            {
                copy.points = new List<PointSet>();
                copy.velocities = new List<VelocitySet>();
                copy.angles = new List<AnglesSet>();
                copy.p_OutList = new List<P_out>();
                copy.points.AddRange(original.points);
                copy.velocities.AddRange(original.velocities);
                copy.angles.AddRange(original.angles);
                copy.p_OutList.AddRange(original.p_OutList);
            }
            
            return copy;
        }

        public void SetTemplateRoute(ObservableCollection<WayPoint> wayPointList)
        {
            WayPoint wayPoint = new WayPoint();
            #region wp#1
            wayPoint.Latitude = 55;
            wayPoint.Longitude = 37;
            wayPoint.Altitude = 600;
            wayPoint.Velocity = 80;
            AddWayPoint(wayPointList, wayPoint);
            #endregion

            #region wp#2
            wayPoint.Latitude = 55.5;
            wayPoint.Longitude = 37.5;
            wayPoint.Altitude = 600;
            wayPoint.Velocity = 80;
            AddWayPoint(wayPointList, wayPoint);
            #endregion

            //#region wp#3
            //wayPoint.Latitude = 55.2;
            //wayPoint.Longitude = 37.2;
            //wayPoint.Altitude = 600;
            //wayPoint.Velocity = 80;
            //AddWayPoint(wayPointList, wayPoint);
            //#endregion

            //#region wp#4
            //wayPoint.Latitude = 55.2;
            //wayPoint.Longitude = 37;
            //wayPoint.Altitude = 600;
            //wayPoint.Velocity = 80;
            //AddWayPoint(wayPointList, wayPoint);
            //#endregion

            //#region wp#5
            //wayPoint.Latitude = 55;
            //wayPoint.Longitude = 37;
            //wayPoint.Altitude = 600;
            //wayPoint.Velocity = 80;
            //AddWayPoint(wayPointList, wayPoint);
            //#endregion
        }
        public void SetDataFromLogger(LogInfo info, ObservableCollection<WayPoint> wayPointList)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            string[] infoString = info.Element.Split('|');
            int ID = Convert.ToInt32(infoString[7].Split(' ')[1]);
            selectedInfo = infoList.Find(item => item.id == ID);
            string[] latitude = selectedInfo.input.latitude.Split(' ');
            string[] longitude = selectedInfo.input.longitude.Split(' ');
            string[] altitude = selectedInfo.input.altitude.Split(' ');
            string[] velocity = selectedInfo.input.velocity.Split(' ');
            wayPointList.Clear();
            for (int i = 0; i < selectedInfo.CountOfPoints; i++)
            {
                WayPoint wayPoint = new WayPoint();
                wayPoint.Latitude = Convert.ToDouble(latitude[i]);
                wayPoint.Longitude = Convert.ToDouble(longitude[i]);
                wayPoint.Altitude = Convert.ToDouble(altitude[i]);
                wayPoint.Velocity = Convert.ToDouble(velocity[i]);
                AddWayPoint(wayPointList, wayPoint);
            }
        }
        public void RemoveDataFromLogger()
        {
            Logger.RemoveDataFromDB(selectedInfo.id);
        }
        public ObservableCollection<LogInfo> GetInfoFromLogger()
        {
            ObservableCollection<LogInfo> logInfo = new ObservableCollection<LogInfo>();
            infoList = ReadInfoFromDB();

            foreach (DebugInfo info in infoList)
            {
                string temp = "Date: " + info.Date + "\n|"
                    + "Error: " + info.Message + "\n|"
                    + "Input:\n|" + "Lat: " + info.input.latitude + "\n|" + "Lon: " + info.input.longitude + "\n|"
                    + "Alt: " + info.input.altitude + "\n|" + "Vel: " + info.input.velocity + "\n|" + "ID: " + info.id.ToString();

                logInfo.Add(new LogInfo() { Element = temp });
            }
            return logInfo;
        }
        public WayPoint SetWayPoint()
        {
            return new WayPoint() { Latitude = 55, Longitude = 37, Altitude = 1000, Velocity = 130 };
        }
        public InitData SetInputErrors(InitData initData)
        {
            initData.insErrors = new ObservableCollection<EquipmentData>();
            initData.sensorErrors = new ObservableCollection<EquipmentData>();
            initData.airInfo = new ObservableCollection<EquipmentData>();
            initData.windInfo = new ObservableCollection<EquipmentData>();
            initData.windInfoDryden = new ObservableCollection<EquipmentData>();
            initData.gnssErrors = new ObservableCollection<EquipmentData>();


            #region InsErrors
            initData.insErrors.Add(new EquipmentData() { Name = "α", Value = 0.25, Dimension = "[deg/h]" });
            initData.insErrors.Add(new EquipmentData() { Name = "β", Value = 0.03, Dimension = "[deg/h]" });
            initData.insErrors.Add(new EquipmentData() { Name = "γ", Value = 0.03, Dimension = "[deg/h]" });

            initData.insErrors.Add(new EquipmentData() { Name = "Δλ", Value = 15, Dimension = "[m]" });
            initData.insErrors.Add(new EquipmentData() { Name = "Δφ", Value = 15, Dimension = "[m]" });
            initData.insErrors.Add(new EquipmentData() { Name = "ΔH", Value = 15, Dimension = "[m]" });

            initData.insErrors.Add(new EquipmentData() { Name = "ΔVe", Value = 0.5, Dimension = "[m/s]" });
            initData.insErrors.Add(new EquipmentData() { Name = "ΔVn", Value = 0.5, Dimension = "[m/s]" });
            initData.insErrors.Add(new EquipmentData() { Name = "ΔVh", Value = 0.5, Dimension = "[m/s]" });

            initData.insErrors.Add(new EquipmentData() { Name = "dt", Value = 0.5, Dimension = "" });
            #endregion

            #region SensorErrors
            initData.sensorErrors.Add(new EquipmentData() { Name = "Δn1", Value = 6E-06, Dimension = "[g]" });
            initData.sensorErrors.Add(new EquipmentData() { Name = "Δn2", Value = 6E-06, Dimension = "[g]" });
            initData.sensorErrors.Add(new EquipmentData() { Name = "Δn3", Value = 6E-06, Dimension = "[g]" });

            initData.sensorErrors.Add(new EquipmentData() { Name = "ΔΩ1", Value = 0.001, Dimension = "[deg/h]" });
            initData.sensorErrors.Add(new EquipmentData() { Name = "ΔΩ2", Value = 0.001, Dimension = "[deg/h]" });
            initData.sensorErrors.Add(new EquipmentData() { Name = "ΔΩ3", Value = 0.001, Dimension = "[deg/h]" });
            
            initData.sensorErrors.Add(new EquipmentData() { Name = "acc noise", Value = 0.001, Dimension = "" });
            initData.sensorErrors.Add(new EquipmentData() { Name = "gyro noise", Value = 0.001, Dimension = "" });

            initData.sensorErrors.Add(new EquipmentData() { Name = "Kt", Value = 1.2E-5, Dimension = "1/`C" });
            #endregion

            #region AirInfo
            initData.airInfo.Add(new EquipmentData() { Name = "H0", Value = 0, Dimension = "[m]" });
            #endregion

            #region WindInfo
            initData.windInfo.Add(new EquipmentData() { Name = "wind_E", Value = 3, Dimension = "[m/s]" });
            initData.windInfo.Add(new EquipmentData() { Name = "wind_N", Value = 2, Dimension = "[m/s]" });
            initData.windInfo.Add(new EquipmentData() { Name = "wind_H", Value = 1, Dimension = "[m/s]" });

            initData.windInfo.Add(new EquipmentData() { Name = "ΔP", Value = 0, Dimension = "[P]" });
            initData.windInfo.Add(new EquipmentData() { Name = "ΔT", Value = 0, Dimension = "[K]" });

            initData.windInfoDryden.Add(new EquipmentData() { Name = "sigma_u", Value = 1.1, Dimension = "[m/s]" });
            initData.windInfoDryden.Add(new EquipmentData() { Name = "sigma_v", Value = 1.1, Dimension = "[m/s]" });
            initData.windInfoDryden.Add(new EquipmentData() { Name = "sigma_w", Value = 0.7, Dimension = "[m/s]" });

            initData.windInfoDryden.Add(new EquipmentData() { Name = "L_u", Value = 200, Dimension = "[m]" });
            initData.windInfoDryden.Add(new EquipmentData() { Name = "L_v", Value = 200, Dimension = "[m]" });
            initData.windInfoDryden.Add(new EquipmentData() { Name = "L_w", Value = 50, Dimension = "[m]" });
            #endregion


            #region GnssInfo
            initData.gnssErrors.Add(new EquipmentData() { Name = "ΔXc", Value = 10, Dimension = "[m]" });
            initData.gnssErrors.Add(new EquipmentData() { Name = "ΔVc", Value = 0.1, Dimension = "[m/s]" });
            initData.gnssErrors.Add(new EquipmentData() { Name = "noise", Value = 1, Dimension = "" });
            #endregion


            return initData;
        }
    }
}
