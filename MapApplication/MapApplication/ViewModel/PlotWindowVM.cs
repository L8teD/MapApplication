using MapApplication.Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.ViewModel
{
    public class PlotWindowVM : BaseViewModel
    {
        public IPlotControl plotWindowControlVM { get; set; }
        public PlotWindowVM(PlotName name, PlotControlVM plotControlVM, MainModel model)
        {
            plotWindowControlVM = new PlotControlVM(name, plotControlVM.CurrentPlotState, model, true);
            plotWindowControlVM.RefreshPlot();
        }
        public PlotWindowVM(PlotPageVM plotPageVM, MainModel model)
        {
            plotWindowControlVM = new PlotPageVM(model);
            plotWindowControlVM.RefreshPlot();
        }
    }
}
