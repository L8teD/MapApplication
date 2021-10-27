using MapApplicationWPF.ExternalResourses;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapApplicationWPF.UserControls
{
    public partial class PlotControl : UserControl
    {
        PlotFinalData plotFinalData = new PlotFinalData();
        string plotName;
        public PlotControl()
        {
            InitializeComponent();
            this.DataContext = this;

            plotFinalData.CreatePlotData();
            lb_PlotTitles.ItemsSource = plotFinalData.plotTitles;

            plot.SetPlotState(plotFinalData.axisNames["Траектория движения"][0], plotFinalData.axisNames["Траектория движения"][1],
                "Траектория движения", plotFinalData.lineSeriesData["Траектория движения"]);


        }
        private void lb_PlotTitles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            //MessageBox.Show((string)lb.SelectedItem);
            plotName = (string)lb.SelectedItem;
            plot.SetPlotState(plotFinalData.axisNames[plotName][0], plotFinalData.axisNames[plotName][1], plotName, plotFinalData.lineSeriesData[plotName]);

        }


    }
}
