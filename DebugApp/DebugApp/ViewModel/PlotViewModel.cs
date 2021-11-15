using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugApp
{
    public class PlotViewModel : BaseViewModel
    {
        PlotControllerModel m_PlotModel;
        public PlotModel MyPlotModel { get; set; }
        public PlotController MyPlotController { get; set; }

        public string Title { get; set; }
        private RelayCommand cmd_Home;
        public RelayCommand Cmd_Home
        {
            get
            {
                return cmd_Home ??
                (cmd_Home = new RelayCommand(obj =>
                {
                    m_PlotModel.Home();

                }));
            }
        }
        private RelayCommand cmd_Switch;
        public RelayCommand Cmd_Switch
        {
            get
            {
                return cmd_Switch ??
                (cmd_Switch = new RelayCommand(obj =>
                {
                    m_PlotModel.Switch();

                }));
            }
        }
        public PlotViewModel(string title)
        {
            m_PlotModel = new PlotControllerModel(title);

            MyPlotModel = m_PlotModel.MyPlotModel;
            MyPlotController = m_PlotModel.MyPlotController;
        }


    }
}
