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
        OutputData threeChannelOutput;
        OutputData twoChannelOutput;
        OutputData indicatedData;
        List<DebugInfo> infoList;
        DebugInfo selectedInfo;
        public List<PlotData> indicatedListOfPlotData;
        private List<PlotData> twoChannelPlotData;
        private List<PlotData> threeChannelPlotData;

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

        public event Action<OutputData, int> UpdateTableData;

        public event Action<List<BasicGeoposition>> DrawTrajectoryAction;

        System.Timers.Timer timerTrajectory;
        int second = 1;
        public MainModel()
        {
            timerTrajectory = new System.Timers.Timer(1000);
            timerTrajectory.Elapsed += TimerTrajectory_Elapsed;
        }
        public void SwitchIndicatedData(DataSource source)
        {
            if (indicatedData.points == null) return;
            switch (source)
            {
                case DataSource.threeChannel:
                    indicatedData = DublicateOutputData(threeChannelOutput);
                    break;
                case DataSource.twoChannel:
                    indicatedData = DublicateOutputData(twoChannelOutput);
                    break;
            }
        }
        public void DrawFullTrajctory()
        {
            
            while(second < indicatedData.points.Count)
            {
                trajectoryPoints.Add(new BasicGeoposition()
                {
                    Latitude = indicatedData.points[second].CorrectTrajectory.Degrees.lat,
                    Longitude = indicatedData.points[second].CorrectTrajectory.Degrees.lon,
                    Altitude = indicatedData.points[second].CorrectTrajectory.Degrees.alt
                });
                MathTransformation.IncrementValue(ref second);
            }

            DrawTrajectoryAction?.Invoke(trajectoryPoints);
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
            if (second < indicatedData.points.Count)
            {
                trajectoryPoints.Add(new BasicGeoposition()
                {
                    Latitude = indicatedData.points[second - 1].CorrectTrajectory.Degrees.lat,
                    Longitude = indicatedData.points[second - 1].CorrectTrajectory.Degrees.lon
                });
                trajectoryPoints.Add(new BasicGeoposition()
                {
                    Latitude = indicatedData.points[second].CorrectTrajectory.Degrees.lat,
                    Longitude = indicatedData.points[second].CorrectTrajectory.Degrees.lon
                });

                DrawTrajectoryAction?.Invoke(trajectoryPoints);
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
        public void Start()
        {
            timerTrajectory.Start();
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
            threeChannelOutput = new OutputData();
            twoChannelOutput = new OutputData();
            try
            {
                Execute.CreateTrajectory(initData, ref threeChannelOutput, ref twoChannelOutput);
                indicatedData = DublicateOutputData(twoChannelOutput);
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
            plotData = PlotWorker.SelectData(name, PlotCharacter.Ideal, indicatedListOfPlotData);

            lineSeriesList.Add(PlotWorker.CreateLineSeries(plotData));

            plotData = PlotWorker.SelectData(name, PlotCharacter.Real, indicatedListOfPlotData);
            lineSeriesList.Add(PlotWorker.CreateLineSeries(plotData));

            if (name != PlotName.Pitch && name != PlotName.Heading && name != PlotName.Roll)
            {
                plotData = PlotWorker.SelectData(name, PlotCharacter.CorrectTrajectory, indicatedListOfPlotData);
                lineSeriesList.Add(PlotWorker.CreateLineSeries(plotData));
            }

            action.Invoke(plotData.xAxisName, plotData.yAxisName, lineSeriesList);
        }
        private void CreatePlotData()
        {
            indicatedListOfPlotData = PlotWorker.CreatePlotData(twoChannelOutput);
            twoChannelPlotData = PlotWorker.CreatePlotData(twoChannelOutput);
            threeChannelPlotData = PlotWorker.CreatePlotData(threeChannelOutput);
        }
        public void SwitchPlotData(DataSource source)
        {
            if (indicatedListOfPlotData == null) return;
            switch (source)
            {
                case DataSource.threeChannel:
                    indicatedListOfPlotData = DublicatePlotData(threeChannelPlotData);
                    break;
                case DataSource.twoChannel:
                    indicatedListOfPlotData = DublicatePlotData(twoChannelPlotData);
                    break;
            }
            RefreshPlots();
        }
        private List<PlotData> DublicatePlotData(List<PlotData> original)
        {
            if (original == null) return new List<PlotData>();
            List<PlotData> copy = new List<PlotData>();

            for (int i = 0; i < original.Count; i++)
            {
                List<double> doubleList = new List<double>();
                foreach (DataPoint point in original[i].values)
                    doubleList.Add(point.Y);
                PlotData data = new PlotData(original[i].name, original[i].character, doubleList);
                copy.Add(data);
            }
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
            return new WayPoint() { Latitude = 55, Longitude = 37, Altitude = 3000, Velocity = 1224 };
        }
        public (ObservableCollection<InputError>, ObservableCollection<InputError>) SetInputErrors()
        {
            ObservableCollection<InputError> insErrors = new ObservableCollection<InputError>();
            ObservableCollection<InputError> sensorErrors = new ObservableCollection<InputError>();

            insErrors.Add(new InputError() { Name = "α", Value = 0.25, Dimension = "[deg/h]" });
            insErrors.Add(new InputError() { Name = "β", Value = 0.03, Dimension = "[deg/h]" });
            insErrors.Add(new InputError() { Name = "γ", Value = 0.03, Dimension = "[deg/h]" });

            insErrors.Add(new InputError() { Name = "Δλ", Value = 15, Dimension = "[m]" });
            insErrors.Add(new InputError() { Name = "Δφ", Value = 15, Dimension = "[m]" });
            insErrors.Add(new InputError() { Name = "ΔH", Value = 15, Dimension = "[m]" });

            insErrors.Add(new InputError() { Name = "ΔVe", Value = 0.5, Dimension = "[m/s]" });
            insErrors.Add(new InputError() { Name = "ΔVn", Value = 0.5, Dimension = "[m/s]" });
            insErrors.Add(new InputError() { Name = "ΔVh", Value = 0.5, Dimension = "[m/s]" });

            sensorErrors.Add(new InputError() { Name = "Δn1", Value = 6E-06, Dimension = "[g]" });
            sensorErrors.Add(new InputError() { Name = "Δn2", Value = 6E-06, Dimension = "[g]" });
            sensorErrors.Add(new InputError() { Name = "Δn3", Value = 6E-06, Dimension = "[g]" });

            sensorErrors.Add(new InputError() { Name = "ΔΩ1", Value = 0.001, Dimension = "[deg/h]" });
            sensorErrors.Add(new InputError() { Name = "ΔΩ2", Value = 0.001, Dimension = "[deg/h]" });
            sensorErrors.Add(new InputError() { Name = "ΔΩ3", Value = 0.001, Dimension = "[deg/h]" });

            //items.Add(new InputError() { Name = "ΔXc", Value = 10, Dimension = "[m]" });
            //items.Add(new InputError() { Name = "ΔVc", Value = 0.1, Dimension = "[m/s]" });

            //SaveInitDataHandler += ListViewWorker.SaveInitDataHandler;

            return (insErrors, sensorErrors);
        }
        public List<LineSeries> DublicateLineSeriesList(List<LineSeries> seriesList)
        {
            List<LineSeries> copyList = new List<LineSeries>();
            foreach (LineSeries series in seriesList)
                copyList.Add(DublicateLineSeries(series));
            return copyList;
        }
        public LineSeries DublicateLineSeries(LineSeries series)
        {
            LineSeries lineSeries = new LineSeries()
            {
                DataFieldX = "x",
                DataFieldY = "Y",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = series.LineStyle,
                Color = series.Color,
                MarkerType = series.MarkerType,
                Title = series.Title
            };
            foreach (var point in series.Points)
                lineSeries.Points.Add(point);
            return lineSeries;
        }
    }
}
