using MapApplicationWPF.ExternalResourses;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MapApplicationWPF.Helper.Types;

namespace MapApplicationWPF.Graphic
{
    class PlotWorker
    {

        private static string plotTitle;
        private static string plotFactor;
        private static string plotDimension;
        public static List<PlotData> plotDataList;
        public static void CreateListOfPlotData()
        {
            plotDataList = new List<PlotData>();
        }
        public static void AddPlotDataToStruct(FullDisplayedData fullDisplayedData, int index)
        {
            plotDataList.Add(new PlotData("Latitude", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Latitude));
            plotDataList.Add(new PlotData("Longitude", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Longitude));
            plotDataList.Add(new PlotData("Altitude", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Altitude));
            plotDataList.Add(new PlotData("Velocity", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Velocity));
            plotDataList.Add(new PlotData("Velocity East", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].VelocityEast));
            plotDataList.Add(new PlotData("Velocity North", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].VelocityNorth));
            plotDataList.Add(new PlotData("Velocity H", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].VelocityH));
            plotDataList.Add(new PlotData("Heading", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Heading));
            plotDataList.Add(new PlotData("Roll", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Roll));
            plotDataList.Add(new PlotData("Pitch", "Ideal Data", fullDisplayedData.DisplayedDatasIdeal[index].Pitch));

            plotDataList.Add(new PlotData("Latitude", "Error Data", fullDisplayedData.DisplayedDatasError[index].Latitude));
            plotDataList.Add(new PlotData("Longitude", "Error Data", fullDisplayedData.DisplayedDatasError[index].Longitude));
            plotDataList.Add(new PlotData("Altitude", "Error Data", fullDisplayedData.DisplayedDatasError[index].Altitude));
            plotDataList.Add(new PlotData("Velocity", "Error Data", fullDisplayedData.DisplayedDatasError[index].Velocity));
            plotDataList.Add(new PlotData("Velocity East", "Error Data", fullDisplayedData.DisplayedDatasError[index].VelocityEast));
            plotDataList.Add(new PlotData("Velocity North", "Error Data", fullDisplayedData.DisplayedDatasError[index].VelocityNorth));
            plotDataList.Add(new PlotData("Velocity H", "Error Data", fullDisplayedData.DisplayedDatasError[index].VelocityH));
            plotDataList.Add(new PlotData("Heading", "Error Data", fullDisplayedData.DisplayedDatasError[index].Heading));
            plotDataList.Add(new PlotData("Roll", "Error Data", fullDisplayedData.DisplayedDatasError[index].Roll));
            plotDataList.Add(new PlotData("Pitch", "Error Data", fullDisplayedData.DisplayedDatasError[index].Pitch));
        }

        public static void SetPlotTitle(string _plotTitle)
        {
            plotTitle = _plotTitle;
        }
        public static void SetPlotFactor(string _PlotFactor)
        {
            plotFactor = _PlotFactor;
        }
        public static void SetPlotDimension(string _plotDimension)
        {
            plotDimension = _plotDimension;
        }
        public static void Plot()
        {
            List<PlotData> currentPlotDataList = plotDataList.FindAll(item =>  item.name == plotTitle && item.valid == plotFactor);

            List<DataPoint> dataPoints = new List<DataPoint>();
            for (int i = 0; i < currentPlotDataList.Count; i++)
                dataPoints.Add(new DataPoint(i, currentPlotDataList[i].value));
            
            OrdinaryPlotWindow ordinaryPlotWindow = new OrdinaryPlotWindow(
                "Time, [sec]",
                plotTitle + ", " + plotDimension,
                plotTitle,
                new List<LineSeries>() { PlotFinalData.CreateLineSeries(dataPoints) });
            ordinaryPlotWindow.Show();
            //OrdinaryPlotWindow ordinaryPlotWindow = new OrdinaryPlotWindow(message);
        }
    }
    class PlotData
    {
        public string name;
        public string valid; 
        public double value;
        public string dimension;
        public PlotData(string _name, string _valid, double _value)
        {
            name = _name;
            valid = _valid;
            value = _value;
        }
    }
}
