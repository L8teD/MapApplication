using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace MapApplication.ViewModel
{
    public class PlotVM : BaseViewModel
    {
        public PlotController MyPlotController { get; set; }
        public PlotModel MyPlotModel { get; set; }
        string lastXAxesName;
        string lastYAxesName;
        LinearAxis yAxis;
        LinearAxis xAxis;
        List<LineSeries> lastSeriesData = new List<LineSeries>();

        public PlotVM(string title)
        {
            MyPlotController = new PlotController();
            MyPlotModel = new PlotModel();

            MyPlotModel.Title = title;

        }
        public void SaveToClipBoard()
        {
            var pngExporter = new PngExporter { Width = 1460, Height = 900};
            MyPlotModel.Background = OxyColors.White;
            MyPlotModel.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.RightTop,
                LegendBorder = OxyColors.Black,
                LegendSize = new OxySize(1000, 800),
                LegendBorderThickness = 2,
                LegendFontWeight = 12,
                LegendFontSize = 26,
            });
            var bitmap = pngExporter.ExportToBitmap(MyPlotModel);
            Clipboard.SetImage(bitmap);
            MyPlotModel.Legends.Clear();
        }
        private void SetPlotParameter(ref string item, string value)
        {
            if(value != default(string))
                item = value;
        }
        public void SaveToClipBoard(string title, string[] axesNames, string[] seriesNames, int legendPosition)
        {
            var pngExporter = new PngExporter { Width = 1460, Height = 900 };
            MyPlotModel.Background = OxyColors.White;
            for (int i = 0; i < MyPlotModel.Axes.Count; i++)
            {
                if (axesNames[i] != default(string))
                    MyPlotModel.Axes[i].Title = axesNames[i];
            }
            for (int i = 0; i < MyPlotModel.Series.Count; i++)
            {
                if (seriesNames[i] != default(string))
                    MyPlotModel.Series[i].Title = seriesNames[i];
            }
            MyPlotModel.Title = title;
            MyPlotModel.TitleFontSize = 50;
            MyPlotModel.Legends.Add(new Legend
            {
                LegendPosition = (LegendPosition)legendPosition,
                LegendBorder = OxyColors.Black,
                LegendSize = new OxySize(1000, 800),
                LegendBorderThickness = 2,
                LegendFontWeight = 16,
                LegendFontSize = 36,
            });
            var bitmap = pngExporter.ExportToBitmap(MyPlotModel);
            Clipboard.SetImage(bitmap);
            MyPlotModel.Legends.Clear();
        }
        public void ChangePlotTitle(string plotName)
        {
            MyPlotModel.Title = plotName;
            MyPlotModel.InvalidatePlot(true);

        }
        public void Home()
        {
            if (lastXAxesName != null && lastYAxesName != null)
                Plot(lastXAxesName, lastYAxesName, lastSeriesData);
        }
        public void Plot(string xAxisName, string yAxisName, List<LineSeries> lineSeriesData)
        {
            if (xAxisName == default(string))
                lastXAxesName = "time, [sec]";
            else
                lastXAxesName = xAxisName;

            if (yAxisName != default(string))
                lastYAxesName = yAxisName;
            lastYAxesName = yAxisName;
            Plot(lineSeriesData);
        }
       
        public void Plot(List<LineSeries> lineSeriesData)
        {
            MyPlotModel.Series.Clear();
            if (lastXAxesName != null && lastYAxesName != null)
            {
                RefreshAxes(lastXAxesName, lastYAxesName);
                foreach (Series serie in lineSeriesData)
                {
                    if (serie != null)
                        if (serie.PlotModel != null)
                        {
                            serie.PlotModel.Series.Clear();
                            //MyPlotModel.Series.Add(serie);
                        }
                }
                foreach (Series serie in lineSeriesData)
                {
                    if (serie != null)
                        MyPlotModel.Series.Add(serie);
                }

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
                Title = yAxisTitle,
                FontSize = 40,
                FontWeight = 18
            };
            yAxis.AxisChanged += Y_Axis_AxisChanged;

            xAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Dash,
                MajorGridlineColor = OxyColors.Gray,
                Position = AxisPosition.Bottom,
                Title = xAxisTitle,
                FontSize = 40,
                FontWeight = 18,
                MajorStep = 200
            };
            xAxis.AxisChanged += X_Axis_AxisChanged;
            MyPlotModel.Axes.Add(yAxis);
            MyPlotModel.Axes.Add(xAxis);
            MyPlotModel.TitleFontSize = 28;
        }
        private void RefreshGrid(LinearAxis axis)
        {
            axis.MajorStep = Math.Abs(Math.Round(axis.ActualMaximum, MidpointRounding.ToEven) - 
                Math.Round(axis.ActualMinimum, MidpointRounding.ToEven)) / 10;
        }
        private void X_Axis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            RefreshGrid(xAxis);
        }
        private void Y_Axis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            RefreshGrid(xAxis);
        }
    }
}
