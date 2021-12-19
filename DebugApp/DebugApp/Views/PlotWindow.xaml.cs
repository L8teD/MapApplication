using DebugApp.Model;
using DebugApp.ViewModel;
using OxyPlot;
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
using System.Windows.Shapes;

namespace DebugApp.View
{
    /// <summary>
    /// Логика взаимодействия для PlotWindow.xaml
    /// </summary>
    public partial class PlotWindow : Window
    {
        string m_title;
        MainModel m_Model;
        //public PlotControlVM plotWindowVM { get; set; }
        public PlotWindow()
        {
            InitializeComponent();
            //m_title = title;
            //m_Model = model;
            //this.Loaded += PlotWindow_Loaded;
        }

        private void PlotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //DataContext = new PlotControlVM(m_title, m_Model);
        }
    }
}
