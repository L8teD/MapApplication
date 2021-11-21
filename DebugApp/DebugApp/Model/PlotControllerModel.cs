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
        Timer timer;
        Timer timerForLaunchMainTimer;
        private bool plotWindowIsCreated;
        private bool plotIsNeedToCreate;
        public PlotModel MyPlotModel { get; set; }
        public PlotController MyPlotController { get; set; }

        LinearAxis yAxis;
        LinearAxis xAxis;

        string mainTitle;

        string lastXAxesName;
        string lastYAxesName;
        string lastTitle;
        string lastDimension;

        ChoosenData choosenData;
        List<LineSeries> errorSeries;
        List<LineSeries> mainSeries;
        List<LineSeries> lastSeriesData = new List<LineSeries>();
        public PlotControllerModel(string title, bool newWindow = false)
        {
            timer = new Timer();
            timerForLaunchMainTimer = new Timer();
            plotIsNeedToCreate = newWindow;
            plotWindowIsCreated = false;
            MyPlotModel = new PlotModel();
            MyPlotController = new PlotController();
            mainTitle = title;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 500;
            timer.Start();
            timerForLaunchMainTimer.Elapsed += TimerForLaunchMainTimer_Elapsed;
            timerForLaunchMainTimer.Interval = 1000;
            timerForLaunchMainTimer.Start();
            
            //CreateAxes("testX", "testY");
        }

        private void TimerForLaunchMainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (PlotWorker.dataIsUpdated)
            {
                timer.Start();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (mainTitle != null && PlotWorker.plotDataList != null)
            {

                Plot();
                plotWindowIsCreated = false;
                plotIsNeedToCreate = false;
                timer.Stop();
                PlotWorker.dataIsUpdated = false;

                //else if (!plotWindowIsCreated && plotIsNeedToCreate)
                //{
                //    timer.Enabled = false;
                //    Plot();
                //    plotWindowIsCreated = true;
                //    timer.Enabled = true;
                //}
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
            if (PlotWorker.plotDataList != null)
            {
                List<PlotData> idealPlotData = PlotWorker.FindRequiredData(mainTitle, "Ideal Data");
                List<PlotData> errorPlotData = PlotWorker.FindRequiredData(mainTitle, "Error Data");
                List<PlotData> withErrorPlotData = PlotWorker.FindRequiredData(mainTitle, "Ideal+Error Data");


                List<DataPoint> idealDataPoints = PlotWorker.CreateDatapointList(idealPlotData);
                List<DataPoint> errorDataPoints = PlotWorker.CreateDatapointList(errorPlotData);
                List<DataPoint> withErrorDataPoints = PlotWorker.CreateDatapointList(withErrorPlotData);

                errorSeries = new List<LineSeries>() { PlotWorker.CreateLineSeries(errorDataPoints, false) };
                mainSeries = new List<LineSeries>() { PlotWorker.CreateLineSeries(idealDataPoints),
                                                                   PlotWorker.CreateLineSeries(withErrorDataPoints, false) };
                lastDimension = idealPlotData[0].dimension;
                choosenData = ChoosenData.Full;
                SetPlotState("Time, [sec]", idealPlotData[0].name + " " + lastDimension, mainTitle, mainSeries);

            }

            //OrdinaryPlotWindow ordinaryPlotWindow = new OrdinaryPlotWindow(message);
        }
        public void Home()
        {
            if (lastXAxesName != null && lastYAxesName != null)
                SetPlotState(lastXAxesName, lastYAxesName, lastTitle, lastSeriesData);
        }
        public void Full()
        {
            //PlotWorker.fullOpened = true;
            if (MyPlotModel != null)
            {
                PlotWindow plotWindow = new PlotWindow(MyPlotModel.Title);
                plotWindow.Show();
            }

        }
        public void Switch()
        {
            timer.Start();
            if (choosenData == ChoosenData.Full)
            {
                SetPlotState("Time, [sec]", lastYAxesName + " " + lastDimension, mainTitle, errorSeries);
                choosenData = ChoosenData.Error;
            }
            else if(choosenData == ChoosenData.Error)
            {
                SetPlotState("Time, [sec]", lastYAxesName + " " + lastDimension, mainTitle, mainSeries);
                choosenData = ChoosenData.Full;
            }
            timer.Stop();
        }
        enum ChoosenData
        {
            Full,
            Error
        }
    }

}
