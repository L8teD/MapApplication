using DebugApp.Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DebugApp.Model.Types;

namespace DebugApp.ViewModel
{
    public class PlotWindowVM : BaseViewModel
    {
        public PlotControlVM plotWindowControlVM { get; set; }
        public PlotWindowVM(PlotName name, PlotControlVM plotControlVM, MainModel model)
        {
            plotWindowControlVM = new PlotControlVM(name, model);
            plotWindowControlVM.Plot("x", "y", plotControlVM.IndicatedSeries);
        }
    }
}
