using MapApplicationWPF.ExternalResourses;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapApplicationWPF.UserControls
{
    public partial class Plot : UserControl
    {

        public PlotModel MyPlotModel { get; set; }
        public PlotController MyPlotController { get; set; }

        string lastXAxesName;
        string lastYAxesName;
        string lastTitle;
        LinearAxis yAxis;
        LinearAxis xAxis;
        List<LineSeries> lastSeriesData = new List<LineSeries>();

        public Plot()
        {
            InitializeComponent();
            DataContext = this;
            MyPlotModel = new PlotModel();
            MyPlotController = new PlotController();
            //MyPlotController.BindMouseDown(OxyMouseButton.Right, PlotCommands.PanAt);

            yAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Dash,
                MajorGridlineColor = OxyColors.Gray,
                //MinorTickSize = 0.1,
                Position = AxisPosition.Left,
            };
            yAxis.AxisChanged += Y_Axis_AxisChanged;

            xAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Dash,
                MajorGridlineColor = OxyColors.Gray,
                Position = AxisPosition.Bottom
            };
            xAxis.AxisChanged += X_Axis_AxisChanged;
            MyPlotModel.Axes.Add(yAxis);
            MyPlotModel.Axes.Add(xAxis);
        }
        public void RefreshAxis(string xAxisTitle, string yAxisTitle)
        {
            MyPlotModel.Axes.Clear();
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
            MyPlotModel.Axes.Add(yAxis);
            MyPlotModel.Axes.Add(xAxis);
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
        public void SetPlotState(string xAxisName, string yAxisName, string title, List<LineSeries> lineSeriesData)
        {
            MyPlotModel.Series.Clear();
            MyPlotModel.Title = title;
            RefreshAxis(xAxisName, yAxisName);
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
        public void FixAxes(List<LineSeries> lineSeries)
        {
            double xMax = 0;
            double yMax = 0;
            double xMin = 0;
            double yMin = 0;
            double xMaxTemp, yMaxTemp, xMinTemp, yMinTemp;
            foreach (LineSeries series in lineSeries)
            {
                xMaxTemp = series.Points.Max(point => point.X);
                yMaxTemp = series.Points.Max(point => point.Y);
                xMinTemp = series.Points.Min(point => point.X);
                yMinTemp = series.Points.Min(point => point.Y);

                CompareValueMore(xMaxTemp, ref xMax);
                CompareValueMore(yMaxTemp, ref yMax);
                CompareValueLess(xMinTemp, ref xMin);
                CompareValueLess(yMinTemp, ref yMin);
            }


            double xIncreaseCoeff = Math.Abs(xMax - xMin) * 0.1;
            double yIncreaseCoeff = Math.Abs(yMax - yMin) * 0.1;

            xAxis.Maximum = xMax >= 0 ? xMax + xIncreaseCoeff : xMax - xIncreaseCoeff;
            xAxis.Minimum = xMin >= 0 ? xMax + xIncreaseCoeff : xMax - xIncreaseCoeff;
            yAxis.Maximum = yMax >= 0 ? yMax + yIncreaseCoeff : yMax - yIncreaseCoeff;
            yAxis.Minimum = yMin >= 0 ? yMin + yIncreaseCoeff : yMin - yIncreaseCoeff;
        }
        private void CompareValueMore(double temp, ref double extremum)
        {
            if (Math.Abs(temp) > Math.Abs(extremum) || extremum == 0)
                extremum = temp;
        }

        private void CompareValueLess(double temp, ref double extremum)
        {
            if (Math.Abs(temp) < Math.Abs(extremum) || extremum == 0)
                extremum = temp;
        }

        private void btn_Home_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (lastXAxesName != null && lastYAxesName != null)
                SetPlotState(lastXAxesName, lastYAxesName, lastTitle, lastSeriesData);
        }
    }
}
