using DebugApp.Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugApp.ViewModel
{
    public class PlotWindowVM : BaseViewModel
    {
        public PlotControlVM plotWindowControlVM { get; set; }
        public PlotWindowVM(string title, MainModel model)
        {
            plotWindowControlVM = new PlotControlVM(title+" jopa", model);
            plotWindowControlVM.Plot("x", "y", model.IndicatedSeries);
        }
    }
}
