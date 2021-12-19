using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace DebugApp.ViewModel
{
    public class PlotVM : BaseViewModel
    {
        public PlotController MyPlotController { get; set; }
        public PlotModel MyPlotModel { get; set; }
        string lastXAxesName;
        string lastYAxesName;
        string lastTitle;
        string lastDimension;
        LinearAxis yAxis;
        LinearAxis xAxis;
        List<LineSeries> lastSeriesData = new List<LineSeries>();

        public PlotVM(string title)
        {
            MyPlotController = new PlotController();
            MyPlotModel = new PlotModel();

            MyPlotModel.Title = title;

        }
        
        public void Home()
        {
            if (lastXAxesName != null && lastYAxesName != null)
                Plot(lastXAxesName, lastYAxesName, lastSeriesData);
        }
        public void Plot(string xAxisName, string yAxisName, List<LineSeries> lineSeriesData)
        {
            lastXAxesName = xAxisName;
            lastYAxesName = yAxisName;
            Plot(lineSeriesData);
        }
       
        public void Plot(List<LineSeries> lineSeriesData)
        {
            MyPlotModel.Series.Clear();
            if (lastXAxesName != null && lastYAxesName != null)
            {
                RefreshAxes(lastYAxesName, lastYAxesName);
                foreach (Series serie in lineSeriesData)
                {
                    if (serie != null)
                        if (serie.PlotModel != null)
                        {
                            serie.PlotModel.Series.Clear();
                        }
                }
                foreach (Series serie in lineSeriesData)
                    MyPlotModel.Series.Add(serie);
                MyPlotModel.InvalidatePlot(true);
                lastSeriesData = lineSeriesData;
            }
        }
        private void RefreshAxes(string xAxisTitle, string yAxisTitle)
        {
            MyPlotModel.Axes.Clear();
            CreateAxes(xAxisTitle, yAxisTitle);
        }
        private void CreateAxes(string xAxisTitle, string yAxisTitle)
        {
            yAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Dash,
                MajorGridlineColor = OxyColors.Gray,
                Position = AxisPosition.Left,
                Title = yAxisTitle
            };
            yAxis.AxisChanged += Axis_AxisChanged;

            xAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Dash,
                MajorGridlineColor = OxyColors.Gray,
                Position = AxisPosition.Bottom,
                Title = xAxisTitle
            };
            xAxis.AxisChanged += Axis_AxisChanged;
            MyPlotModel.Axes.Add(yAxis);
            MyPlotModel.Axes.Add(xAxis);
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
        private void Axis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            RefreshGrid();
        }
    }
}
