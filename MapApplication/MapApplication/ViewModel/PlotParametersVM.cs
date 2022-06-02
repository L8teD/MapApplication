using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapApplication.ViewModel
{
    public class PlotParametersVM : BaseViewModel
    {
        IPlotControl plotControl;
        public PlotParametersVM(IPlotControl _plotControl)
        {
            plotControl = _plotControl;
            //plotControl.plot.SaveToClipBoard();
        }
        private RelayCommand cmd_Execute;
        public RelayCommand Execute
        {
            get
            {
                return cmd_Execute ??
                (cmd_Execute = new RelayCommand(obj =>
                {
                    string title = TitleName;
                    int legendPos = LegendPosition;
                    string[] axisNames = new string[] {AxisName1, AxisName2};
                    string[] legendNames = new string[] {LegendName1, LegendName2, LegendName3, LegendName4 };
                    plotControl.plot.SaveToClipBoard(TitleName, axisNames, legendNames, legendPos);
                }));
            }
        }
        public string TitleName
        {
            get { return (string)GetValue(TitleNameProperty); }
            set { SetValue(TitleNameProperty, value); }
        }
        public static readonly DependencyProperty TitleNameProperty =
          DependencyProperty.Register("TitleName", typeof(string), typeof(PlotParametersVM), new PropertyMetadata(default(string))); 
        public int LegendPosition
        {
            get { return (int)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }
        public static readonly DependencyProperty LegendPositionProperty =
          DependencyProperty.Register("LegendPosition", typeof(int), typeof(PlotParametersVM), new PropertyMetadata(2));
        
        public string LegendName1
        {
            get { return (string)GetValue(LegendName1Property); }
            set { SetValue(LegendName1Property, value); }
        }
        public static readonly DependencyProperty LegendName1Property =
          DependencyProperty.Register("LegendName1", typeof(string), typeof(PlotParametersVM), new PropertyMetadata("Заданная траектория"));
        
        public string LegendName2
        {
            get { return (string)GetValue(LegendName2Property); }
            set { SetValue(LegendName2Property, value); }
        }
        public static readonly DependencyProperty LegendName2Property =
          DependencyProperty.Register("LegendName2", typeof(string), typeof(PlotParametersVM), new PropertyMetadata("Фактическая траектория"));
        
        public string LegendName3
        {
            get { return (string)GetValue(LegendName3Property); }
            set { SetValue(LegendName3Property, value); }
        }
        public static readonly DependencyProperty LegendName3Property =
          DependencyProperty.Register("LegendName3", typeof(string), typeof(PlotParametersVM), new PropertyMetadata(default(string)));
        
        public string LegendName4
        {
            get { return (string)GetValue(LegendName4Property); }
            set { SetValue(LegendName4Property, value); }
        }
        public static readonly DependencyProperty LegendName4Property =
          DependencyProperty.Register("LegendName4", typeof(string), typeof(PlotParametersVM), new PropertyMetadata(default(string)));


        public string AxisName1
        {
            get { return (string)GetValue(AxisName1Property); }
            set { SetValue(AxisName1Property, value); }
        }
        public static readonly DependencyProperty AxisName1Property =
          DependencyProperty.Register("AxisName1", typeof(string), typeof(PlotParametersVM), new PropertyMetadata(", [м]"));

        public string AxisName2
        {
            get { return (string)GetValue(AxisName2Property); }
            set { SetValue(AxisName2Property, value); }
        }
        public static readonly DependencyProperty AxisName2Property =
          DependencyProperty.Register("AxisName2", typeof(string), typeof(PlotParametersVM), new PropertyMetadata("Время, [сек]"));

    }
}
