using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DebugApp
{
    public class PlotControllerModel
    {
        Timer timer = new Timer();
        public PlotModel MyPlotModel { get; set; }
        public PlotController MyPlotController { get; set; }

        LinearAxis yAxis;
        LinearAxis xAxis;

        string mainTitle;

        string lastXAxesName;
        string lastYAxesName;
        string lastTitle;
        string dimension;

        ChoosenData choosenData;
        List<LineSeries> errorSeries;
        List<LineSeries> mainSeries;
        List<LineSeries> lastSeriesData = new List<LineSeries>();
        public PlotControllerModel(string title)
        {
            MyPlotModel = new PlotModel();
            MyPlotController = new PlotController();
            mainTitle = title;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();
            

            //CreateAxes("testX", "testY");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (PlotWorker.dataIsUpdated)
            {
                Plot();

                PlotWorker.dataIsUpdated = false;
            }
        }

        private void X_Axis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void Y_Axis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            RefreshGrid();
        }
        private void RefreshGrid()
        {
            double xStart = xAxis.ActualMinimum;
            double xEnd = xAxis.ActualMaximum;
            double yStart = yAxis.ActualMinimum;
            double yEnd = yAxis.ActualMaximum;

            double xGridStep = Math.Abs(xEnd - xStart) / 10;
            double yGridStep = Math.Abs(yEnd - yStart) / 10;

            xAxis.MajorStep = xGridStep;
            yAxis.MajorStep = yGridStep;
        }
        private void RefreshAxes(string xAxisTitle, string yAxisTitle, string title)
        {
            MyPlotModel.Axes.Clear();
            CreateAxes(xAxisTitle, yAxisTitle, title);
        }
        private void CreateAxes(string xAxisTitle, string yAxisTitle, string title)
        {
            yAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Dash,
                MajorGridlineColor = OxyColors.Gray,
                Position = AxisPosition.Left,
                Title = yAxisTitle
            };
            yAxis.AxisChanged += Y_Axis_AxisChanged;

            xAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Dash,
                MajorGridlineColor = OxyColors.Gray,
                Position = AxisPosition.Bottom,
                Title = xAxisTitle
            };
            xAxis.AxisChanged += X_Axis_AxisChanged;
            MyPlotModel.Title = title;
            MyPlotModel.Axes.Add(yAxis);
            MyPlotModel.Axes.Add(xAxis);
        }
        private void SetPlotState(string xAxisName, string yAxisName, string title, List<LineSeries> lineSeriesData)
        {
            MyPlotModel.Series.Clear();
            MyPlotModel.Title = title;
            RefreshAxes(xAxisName, yAxisName, title);
            foreach (Series serie in lineSeriesData)
            {
                MyPlotModel.Series.Add(serie);
            }
            MyPlotModel.InvalidatePlot(true);

            lastXAxesName = xAxisName;
            lastYAxesName = yAxisName;
            lastTitle = title;
            lastSeriesData = lineSeriesData;
            //plot.FixAxes(plotData.lineSeriesData[plotTitle]);

        }
        public void Plot()
        {
            List<PlotData> idealPlotData = PlotWorker.FindRequiredData(mainTitle, "Ideal Data");
            List<PlotData> errorPlotData = PlotWorker.FindRequiredData(mainTitle, "Error Data");
            List<PlotData> withErrorPlotData = PlotWorker.FindRequiredData(mainTitle, "Ideal+Error Data");


            List<DataPoint> idealDataPoints = PlotWorker.CreateDatapointList(idealPlotData);
            List<DataPoint> errorDataPoints = PlotWorker.CreateDatapointList(errorPlotData);
            List<DataPoint> withErrorDataPoints = PlotWorker.CreateDatapointList(withErrorPlotData);

            errorSeries = new List<LineSeries>() { PlotWorker.CreateLineSeries(errorDataPoints) };
            mainSeries = new List<LineSeries>() { PlotWorker.CreateLineSeries(idealDataPoints),
                                                                   PlotWorker.CreateLineSeries(withErrorDataPoints, false) };
            string lastDimension = idealPlotData[0].dimension;
            choosenData = ChoosenData.Full;
            SetPlotState("Time, [sec]", idealPlotData[0].name + " " + lastDimension, mainTitle, mainSeries);

            //OrdinaryPlotWindow ordinaryPlotWindow = new OrdinaryPlotWindow(message);
        }
        public void Home()
        {
            if (lastXAxesName != null && lastYAxesName != null)
                SetPlotState(lastXAxesName, lastYAxesName, lastTitle, lastSeriesData);
        }
        public void Switch()
        {
            if (choosenData == ChoosenData.Full)
            {
                SetPlotState("Time, [sec]", lastYAxesName + " " + dimension, mainTitle, errorSeries);
                choosenData = ChoosenData.Error;
            }
            else if(choosenData == ChoosenData.Error)
            {
                SetPlotState("Time, [sec]", lastYAxesName + " " + dimension, mainTitle, mainSeries);
                choosenData = ChoosenData.Full;
            }
        }
        enum ChoosenData
        {
            Full,
            Error
        }
    }

}
