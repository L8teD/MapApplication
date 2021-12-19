using DebugApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DebugApp.ViewModel
{
    public class LegendVM : BaseViewModel
    {
        public LegendButtonVM legendBtn1 { get; set; }
        public LegendButtonVM legendBtn2 { get; set; }
        public LegendButtonVM legendBtn3 { get; set; }
        public LegendButtonVM legendBtn4 { get; set; }
        public LegendButtonVM legendBtn5 { get; set; }
        public List<LegendButtonVM> legendBtns;
        public LegendVM(MainModel model, PlotVM plotVM)
        {
            legendBtn1 = new LegendButtonVM(model, plotVM);
            legendBtn2 = new LegendButtonVM(model, plotVM);
            legendBtn3 = new LegendButtonVM(model, plotVM);
            legendBtn4 = new LegendButtonVM(model, plotVM);
            legendBtn5 = new LegendButtonVM(model, plotVM);
            legendBtns = new List<LegendButtonVM>() { legendBtn1, legendBtn2, legendBtn3, legendBtn4, legendBtn5 };
        }
        public void UpdateLegendElement(LegendButtonVM legendElement, SolidColorBrush color, string text, Visibility vis = Visibility.Visible)
        {
            legendElement.UpdateLegendColor(color);
            legendElement.UpdateLegendText(text);
            legendElement.UpdateLegendVis(vis);
        }
        public void UpdateLegendElement(LegendButtonVM legendElement, Visibility vis = Visibility.Hidden)
        {
            legendElement.UpdateLegendVis(vis);
        }

    }
}
