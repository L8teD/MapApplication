using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DebugApp.Model.Types;

namespace DebugApp.Model
{
    public class PlotWorker
    {
        public static List<PlotData> plotDataList;
        public static bool dataIsUpdated { get; set; }
        public static void InitListOfPlotData()
        {
            plotDataList = new List<PlotData>();
        }
        public static void AddPlotDataToStruct(FullDisplayedData fullDisplayedData, int index)
        {
            plotDataList.Add(new PlotData("Latitude", "Ideal Data", fullDisplayedData.ideal[index].Latitude, "[deg]"));
            plotDataList.Add(new PlotData("Longitude", "Ideal Data", fullDisplayedData.ideal[index].Longitude, "[deg]"));
            plotDataList.Add(new PlotData("Altitude", "Ideal Data", fullDisplayedData.ideal[index].Altitude, "[m]"));
            plotDataList.Add(new PlotData("Velocity", "Ideal Data", fullDisplayedData.ideal[index].Velocity, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity East", "Ideal Data", fullDisplayedData.ideal[index].VelocityEast, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity North", "Ideal Data", fullDisplayedData.ideal[index].VelocityNorth, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity H", "Ideal Data", fullDisplayedData.ideal[index].VelocityH, "[m/sec]"));
            plotDataList.Add(new PlotData("Heading", "Ideal Data", fullDisplayedData.ideal[index].Heading, "[deg]"));
            plotDataList.Add(new PlotData("Roll", "Ideal Data", fullDisplayedData.ideal[index].Roll, "[deg]"));
            plotDataList.Add(new PlotData("Pitch", "Ideal Data", fullDisplayedData.ideal[index].Pitch, "[deg]"));

            plotDataList.Add(new PlotData("Latitude", "Error Data", fullDisplayedData.error[index].Latitude, "[deg]"));
            plotDataList.Add(new PlotData("Longitude", "Error Data", fullDisplayedData.error[index].Longitude, "[deg]"));
            plotDataList.Add(new PlotData("Altitude", "Error Data", fullDisplayedData.error[index].Altitude, "[m]"));
            plotDataList.Add(new PlotData("Velocity", "Error Data", fullDisplayedData.error[index].Velocity, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity East", "Error Data", fullDisplayedData.error[index].VelocityEast, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity North", "Error Data", fullDisplayedData.error[index].VelocityNorth, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity H", "Error Data", fullDisplayedData.error[index].VelocityH, "[m/sec]"));
            plotDataList.Add(new PlotData("Heading", "Error Data", fullDisplayedData.error[index].Heading, "[deg]"));
            plotDataList.Add(new PlotData("Roll", "Error Data", fullDisplayedData.error[index].Roll, "[deg]"));
            plotDataList.Add(new PlotData("Pitch", "Error Data", fullDisplayedData.error[index].Pitch, "[deg]"));

            plotDataList.Add(new PlotData("Latitude", "Ideal+Error Data", fullDisplayedData.real[index].Latitude, "[deg]"));
            plotDataList.Add(new PlotData("Longitude", "Ideal+Error Data", fullDisplayedData.real[index].Longitude, "[deg]"));
            plotDataList.Add(new PlotData("Altitude", "Ideal+Error Data", fullDisplayedData.real[index].Altitude,"[m]"));
            plotDataList.Add(new PlotData("Velocity", "Ideal+Error Data", fullDisplayedData.real[index].Velocity, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity East", "Ideal+Error Data", fullDisplayedData.real[index].VelocityEast, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity North", "Ideal+Error Data", fullDisplayedData.real[index].VelocityNorth, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity H", "Ideal+Error Data", fullDisplayedData.real[index].VelocityH, "[m/sec]"));
            plotDataList.Add(new PlotData("Heading", "Ideal+Error Data", fullDisplayedData.real[index].Heading, "[deg]"));
            plotDataList.Add(new PlotData("Roll", "Ideal+Error Data", fullDisplayedData.real[index].Roll, "[deg]"));
            plotDataList.Add(new PlotData("Pitch", "Ideal+Error Data", fullDisplayedData.real[index].Pitch, "[deg]"));

            plotDataList.Add(new PlotData("Latitude", "Estimate Data", fullDisplayedData.estimated[index].Latitude, "[deg]"));
            plotDataList.Add(new PlotData("Longitude", "Estimate Data", fullDisplayedData.estimated[index].Longitude, "[deg]"));
            //plotDataList.Add(new PlotData("Altitude", "Estimate Data", fullDisplayedData.estimated[index].Altitude, "[m]"));
            plotDataList.Add(new PlotData("Velocity", "Estimate Data", fullDisplayedData.estimated[index].Velocity, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity East", "Estimate Data", fullDisplayedData.estimated[index].VelocityEast, "[m/sec]"));
            plotDataList.Add(new PlotData("Velocity North", "Estimate Data", fullDisplayedData.estimated[index].VelocityNorth, "[m/sec]"));
            //plotDataList.Add(new PlotData("Velocity H", "Estimate Data", fullDisplayedData.estimated[index].VelocityH, "[m/sec]"));
            //plotDataList.Add(new PlotData("Heading", "Estimate Data", fullDisplayedData.estimated[index].Heading, "[deg]"));
            //plotDataList.Add(new PlotData("Roll", "Estimate Data", fullDisplayedData.estimated[index].Roll, "[deg]"));
            //plotDataList.Add(new PlotData("Pitch", "Estimate Data", fullDisplayedData.estimated[index].Pitch, "[deg]"));
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
            if (plotDataList == null)
                return new List<PlotData>();
            else
                return plotDataList.FindAll(item => item.name == plotTitle && item.valid == plotFactor);
        }
        public static LineSeries CreateLineSeries(List<DataPoint> data, string title, bool isBlue = true)
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
                Title = title,
                LegendKey = title,
                LineLegendPosition = LineLegendPosition.End
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

