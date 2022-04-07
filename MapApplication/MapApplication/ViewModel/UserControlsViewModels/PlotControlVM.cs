using MapApplication.Model;
using MapApplication.View;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static MapApplication.Model.Types;

namespace MapApplication.ViewModel
{
    public class PlotControlVM : BaseViewModel
    {
        private bool isWindow;
        public ActivePlotState CurrentPlotState { get; set; }
        public PlotVM plotVM { get; set; }
        public LegendVM legendControlVM { get; set; }
        private MainModel m_Model;
        private RelayCommand cmd_Home;
        private PlotName currentTitle;
        public List<LineSeries> IndicatedSeries;
        public List<LineSeries> RemovedSeries;
        public RelayCommand Cmd_Home
        {
            get
            {
                return cmd_Home ??
                (cmd_Home = new RelayCommand(obj =>
                {
                    plotVM.Home();

                }));
            }
        }
        private RelayCommand cmd_Trajectory;
        public RelayCommand Cmd_Trajectory
        {
            get
            {
                return cmd_Trajectory ??
                (cmd_Trajectory = new RelayCommand(obj =>
                {
                    RefreshPlot(ActivePlotState.Trajectory);
                }));
            }
        }
        private RelayCommand cmd_Error;
        public RelayCommand Cmd_Error
        {
            get
            {
                return cmd_Error ??
                (cmd_Error = new RelayCommand(obj =>
                {
                    RefreshPlot(ActivePlotState.Error);
                }));
            }
        }
        private RelayCommand cmd_Covar;
        public RelayCommand Cmd_Covar
        {
            get
            {
                return cmd_Covar ??
                (cmd_Covar = new RelayCommand(obj =>
                {
                    RefreshPlot(ActivePlotState.P);
                }));
            }
        }
        private RelayCommand cmd_Full;

        public RelayCommand Cmd_Full
        {
            get
            {
                return cmd_Full ??
                (cmd_Full = new RelayCommand(obj =>
                {
                    if (!isWindow)
                    {
                        PlotWindow plotWindow = new PlotWindow();
                        plotWindow.DataContext = new PlotWindowVM(currentTitle, CurrentPlotState, this, m_Model);
                        plotWindow.Show();
                    }
                }));
            }
        }


        public PlotControlVM(PlotName plotName, MainModel model, bool fromWindow = false)
        {
            m_Model = model;
            plotVM = new PlotVM(PlotWorker.SelectPlotName(plotName));
            currentTitle = plotName;
            legendControlVM = new LegendVM(this, plotVM);
            IndicatedSeries = new List<LineSeries>();
            RemovedSeries = new List<LineSeries>();

            isWindow = fromWindow;
        }
        public void RefreshPlot(ActivePlotState plotState)
        {
            CurrentPlotState = plotState;
            switch (plotState)
            {
                case ActivePlotState.Trajectory:
                    RefreshPlot(PlotCharacter.Ideal, PlotCharacter.Real, PlotCharacter.CorrectTrajectory);
                    break;
                case ActivePlotState.Error:
                    RefreshPlot(PlotCharacter.Error, PlotCharacter.Estimate, PlotCharacter.CorrectError);
                    break;
                case ActivePlotState.P:
                    RefreshPlot(PlotCharacter.P);
                    break;
            }
        }
        private void RefreshPlot(params PlotCharacter[] characters)
        {
            IndicatedSeries.Clear();
            PlotData plotData = null;
            string xAxisName = "";
            string yAxisName = "";
            foreach (PlotCharacter character in characters)
            {
                plotData = PlotWorker.SelectData(currentTitle, character, m_Model.indicatedListOfPlotData);
                IndicatedSeries.Add(PlotWorker.CreateLineSeries(plotData));

                if (plotData != null)
                {
                    xAxisName = plotData.xAxisName;
                    yAxisName = plotData.yAxisName;
                }
            }
           

            //plotData = PlotWorker.SelectData(currentTitle, characters[1], m_Model.indicatedListOfPlotData);
            //IndicatedSeries.Add(PlotWorker.CreateLineSeries(plotData));
            //if (currentTitle != PlotName.Pitch && currentTitle != PlotName.Heading && currentTitle != PlotName.Roll)
            //{
            //    plotData = PlotWorker.SelectData(currentTitle, characters[2], m_Model.indicatedListOfPlotData);
            //    IndicatedSeries.Add(PlotWorker.CreateLineSeries(plotData));
            //}
            Plot(xAxisName, yAxisName, IndicatedSeries);
        }
        public void Plot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            plotVM.Plot(xAxisName, yAxisName, seriesList);
            IndicatedSeries = seriesList;

            legendControlVM.ClearLegendsVis();
            for (int i = 0; i < seriesList.Count; i++)
            {
                if (seriesList[i] != null)
                {
                    
                    LineSeries series = seriesList[i];
                    OxyColor lineColor = series.Color;
                    SolidColorBrush legendElColor = new SolidColorBrush(Color.FromArgb(lineColor.A, lineColor.R, lineColor.G, lineColor.B));
                    string legendElText = series.Title;
                    legendControlVM.UpdateLegendElement(legendControlVM.legendBtns[i], legendElColor, legendElText);
                    
                    
                }
                else
                    legendControlVM.UpdateLegendElement(legendControlVM.legendBtns[i]);
            }
            
            
        }
    }
}
