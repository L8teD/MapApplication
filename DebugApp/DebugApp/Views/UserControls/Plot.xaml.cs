using DebugApp.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DebugApp.View
{
    public partial class Plot : UserControl
    {
        public string Title { get; set; }
        public Plot()
        {
            InitializeComponent();
            //this.Loaded += Plot_Loaded;
        }

        private void Plot_Loaded(object sender, RoutedEventArgs e)
        {
            //this.DataContext = new PlotViewModel(Title);
        }
    }
}
