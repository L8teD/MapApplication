using MapApplication.Model;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapApplication.ViewModel
{
    public class PlotPageVM : BaseViewModel, IPlotControl
    {
        public PlotVM plot { get; set; }
        public LegendVM legendControlVM { get; set; }
        public List<LineSeries> IndicatedSeries { get; set; }
        public List<LineSeries> RemovedSeries { get; set; }
        List<PlotData> desiredPlotData;
        List<PlotData> actualPlotData;

        #region Commands
        private RelayCommand cmd_Home;
        public RelayCommand Cmd_Home
        {
            get
            {
                return cmd_Home ??
                (cmd_Home = new RelayCommand(obj =>
                {
                    plot.Home();

                }));
            }
        }
        private RelayCommand cmd_ParamSelectionChanged;
        public RelayCommand ParamSelectionChanged
        {
            get
            {
                return cmd_ParamSelectionChanged ??
                (cmd_ParamSelectionChanged = new RelayCommand(obj =>
                {
                    string paramName = (obj as TextBlock).Text;
                    plot.ChangePlotTitle(paramName);

                    PlotName plotName = PlotWorker.SelectPlotName(paramName);
                    string yAxis = PlotWorker.SelectPlotDimension(plotName, PlotCharacter.Error);


                    FillSeries(plotName);
                    Plot("time, [sec]", yAxis);
                }));
            }
        }

        #endregion

        public PlotPageVM(MainModel m_Model)
        {
            plot = new PlotVM("Latitude");
            legendControlVM = new LegendVM(this, plot);

            m_Model.SetPlotData += M_Model_SetPlotData;


            IndicatedSeries = new List<LineSeries>();
            RemovedSeries = new List<LineSeries>();

        }

        private void M_Model_SetPlotData(List<PlotData> arg1, List<PlotData> arg2)
        {
            M_Model_SetDesiredData(arg1);
            M_Model_SetActualData(arg2);
        }
        private void FillSeries(PlotName name)
        {
            IndicatedSeries.Clear();
            RemovedSeries.Clear();

            IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                PlotWorker.SelectData(name, PlotCharacter.Error, desiredPlotData), 
                "Desired Track"));

            IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                PlotWorker.SelectData(name, PlotCharacter.Error, actualPlotData), 
                "Actual Track"));

        }

        private void M_Model_SetDesiredData(List<PlotData> obj)
        {
            desiredPlotData = obj;
        }
        private void M_Model_SetActualData(List<PlotData> obj)
        {
            actualPlotData = obj;
        }

        public void Plot(string xAxisName, string yAxisName)
        {
            plot.Plot(xAxisName, yAxisName, IndicatedSeries);

            legendControlVM.ClearLegendsVis();
            for (int i = 0; i < IndicatedSeries.Count; i++)
            {
                if (IndicatedSeries[i] != null)
                {

                    LineSeries series = IndicatedSeries[i];
                    SolidColorBrush legendElColor = new SolidColorBrush(Color.FromArgb(series.Color.A, series.Color.R, series.Color.G, series.Color.B));
                    string legendElText = series.Title;
                    legendControlVM.UpdateLegendElement(legendControlVM.legendBtns[i], legendElColor, legendElText);

                }
                else
                    legendControlVM.UpdateLegendElement(legendControlVM.legendBtns[i]);
            }
        }
    }
}
