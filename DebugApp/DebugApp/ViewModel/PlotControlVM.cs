using DebugApp.Model;
using DebugApp.View;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DebugApp.ViewModel
{
    public class PlotControlVM : BaseViewModel
    {
        public PlotVM plotVM { get; set; }
        public LegendVM legendControlVM { get; set; }
        private MainModel m_Model;
        private RelayCommand cmd_Home;
        private string currentTitle;
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
                    Plot("opa", "trajectory", m_Model.IndicatedSeries);

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
                    //m_Model.IndicatedSeries = m_Model.series2;
                    Plot("z", "j", m_Model.IndicatedSeries);
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
                    //m_Model.IndicatedSeries = m_Model.series3;
                    Plot("x", "y", m_Model.IndicatedSeries);

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
                    //plotWindowVM = new PlotControlVM(currentTitle, m_Model);
                    PlotWindow plotWindow = new PlotWindow();
                    plotWindow.DataContext = new PlotWindowVM(currentTitle, m_Model);
                    plotWindow.Show();
                    //plotWindowVM.plotVM.Plot(new List<LineSeries>());
                }));
            }
        }
        public PlotControlVM(string title, MainModel model)
        {
            m_Model = model;
            plotVM = new PlotVM(title);
            currentTitle = title;
            legendControlVM = new LegendVM(model, plotVM);
        }
        public void Plot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            plotVM.Plot(xAxisName, yAxisName, seriesList);
            for (int i = 0; i < 5; i++)
            {
                if (i < seriesList.Count)
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
