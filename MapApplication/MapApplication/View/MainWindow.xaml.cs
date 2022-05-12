using MapApplication.ViewModel;
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

namespace MapApplication.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FlightPlan flightPlan;
        FlightData flightData;
        DebugMode debugMode;
        EquipmentPage equipment;
        PlotPage plotPage;
        public MainWindow()
        {
            InitializeComponent();

            flightPlan = new FlightPlan();
            flightData = new FlightData();
            debugMode = new DebugMode();
            equipment = new EquipmentPage();
            plotPage = new PlotPage();

            MainViewModel mainViewModel = new MainViewModel(flightPlan.Map);

            flightPlan.DataContext = mainViewModel;
            flightData.DataContext = mainViewModel;
            debugMode.DataContext = mainViewModel;
            equipment.DataContext = mainViewModel;
            plotPage.DataContext = mainViewModel.plotPageVM;

            Uri iconUri = new Uri("pack://application:,,,/AnotherFiles/Images/MAI_logo.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(flightPlan);
        }

        private void btn_FlightPlan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(flightPlan);
        }

        private void btn_FlightData_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(flightData);
        }
        private void btn_DebugMode_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(debugMode);
        }
        private void btn_EquipmentPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(equipment);
        }

        private void btn_PlotPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(plotPage);
        }
    }
}
