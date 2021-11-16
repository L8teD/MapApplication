using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DebugApp.Types;

namespace DebugApp
{
    public class PlotWorker
    {
        public static bool dataIsUpdated { get; set; }
        public static string fullOpenedTitle { get; set; }
        public static List<PlotData> plotDataList;
        public static void InitListOfPlotData()
        {
            plotDataList = new List<PlotData>();
        }
        public static void AddPlotDataToStruct(FullDisplayedData fullDisplayedData, int index)
        {
            plotDataList.Add(new PlotData("Latitude", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Latitude, "[deg]"));
            plotDataList.Add(new PlotData("Longitude", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Longitude, "[deg]"));
            plotDataList.Add(new PlotData("Altitude", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Altitude, "[m]"));
            plotDataList.Add(new PlotData("Velocity", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Velocity, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity East", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].VelocityEast, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity North", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].VelocityNorth, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity H", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].VelocityH, "[m/sec]"));
            plotDataList.Add(new PlotData("Heading", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Heading, "[deg]"));
            plotDataList.Add(new PlotData("Roll", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Roll, "[deg]"));
            plotDataList.Add(new PlotData("Pitch", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Pitch, "[deg]"));

            plotDataList.Add(new PlotData("Latitude", "Error Data", fullDisplayedData.DisplayedDatasError[index].Latitude, "[deg]"));
            plotDataList.Add(new PlotData("Longitude", "Error Data", fullDisplayedData.DisplayedDatasError[index].Longitude, "[deg]"));
            plotDataList.Add(new PlotData("Altitude", "Error Data", fullDisplayedData.DisplayedDatasError[index].Altitude, "[m]"));
            plotDataList.Add(new PlotData("Velocity", "Error Data", fullDisplayedData.DisplayedDatasError[index].Velocity, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity East", "Error Data", fullDisplayedData.DisplayedDatasError[index].VelocityEast, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity North", "Error Data", fullDisplayedData.DisplayedDatasError[index].VelocityNorth, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity H", "Error Data", fullDisplayedData.DisplayedDatasError[index].VelocityH, "[m/sec]"));
            plotDataList.Add(new PlotData("Heading", "Error Data", fullDisplayedData.DisplayedDatasError[index].Heading, "[deg]"));
            plotDataList.Add(new PlotData("Roll", "Error Data", fullDisplayedData.DisplayedDatasError[index].Roll, "[deg]"));
            plotDataList.Add(new PlotData("Pitch", "Error Data", fullDisplayedData.DisplayedDatasError[index].Pitch, "[deg]"));

            plotDataList.Add(new PlotData("Latitude", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].Latitude, "[deg]"));
            plotDataList.Add(new PlotData("Longitude", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].Longitude, "[deg]"));
            plotDataList.Add(new PlotData("Altitude", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].Altitude,"[m]"));
            plotDataList.Add(new PlotData("Velocity", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].Velocity, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity East", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].VelocityEast, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity North", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].VelocityNorth, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity H", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].VelocityH, "[m/sec]"));
            plotDataList.Add(new PlotData("Heading", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].Heading, "[deg]"));
            plotDataList.Add(new PlotData("Roll", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].Roll, "[deg]"));
            plotDataList.Add(new PlotData("Pitch", "Ideal+Error Data", fullDisplayedData.DisplayedDatasWithError[index].Pitch, "[deg]"));
        }

        public static List<DataPoint> CreateDatapointList(List<PlotData> currentPlotDataList)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            for (int i = 0; i < currentPlotDataList.Count; i++)
                dataPoints.Add(new DataPoint(i, currentPlotDataList[i].value));
            return dataPoints;
        }
        public static List<PlotData> FindRequiredData(string plotTitle, string plotFactor)
        {
            return plotDataList.FindAll(item => item.name == plotTitle && item.valid == plotFactor);
        }
        public static LineSeries CreateLineSeries(List<DataPoint> data, bool isBlue = true)
        {
            LineSeries series = new LineSeries()
            {
                //ItemsSource = data,
                DataFieldX = "x",
                DataFieldY = "Y",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = LineStyle.Solid,
                Color = isBlue ? OxyColors.Blue : OxyColors.Red,
                MarkerType = MarkerType.None,
            };
            foreach (DataPoint point in data)
            {
                series.Points.Add(point);
            }
            return series;
        }
    }
    public class PlotData
    {
        public string name;
        public string valid;
        public double value;
        public string dimension;
        public PlotData(string _name, string _valid, double _value, string _dimension)
        {
            name = _name;
            valid = _valid;
            value = _value;
            dimension = _dimension;
        }
    }
}

