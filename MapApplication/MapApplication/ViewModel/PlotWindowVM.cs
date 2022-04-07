using MapApplication.Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MapApplication.Model.Types;

namespace MapApplication.ViewModel
{
    public class PlotWindowVM : BaseViewModel
    {
        public PlotControlVM plotWindowControlVM { get; set; }
        public PlotWindowVM(PlotName name, ActivePlotState plotState, PlotControlVM plotControlVM, MainModel model)
        {
            plotWindowControlVM = new PlotControlVM(name, model, true);
            plotWindowControlVM.RefreshPlot(plotState);
        }
    }
}
