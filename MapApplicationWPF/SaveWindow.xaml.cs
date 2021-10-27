using MapApplicationWPF.Helper;
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
using static CommonLib.Types;
using static MapApplicationWPF.Helper.Types;

namespace MapApplicationWPF
{
    /// <summary>
    /// Логика взаимодействия для SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        int csvFileId = 1;
        int dbFileId = 1;
        OutputData outputData;
        public SaveWindow(OutputData _outputData)
        {
            InitializeComponent();
            outputData = _outputData;
        }

        private void btnCSV_Click(object sender, RoutedEventArgs e)
        {
            Saver.WriteCSV(outputData.FullDisplayedData.DisplayedDatasIdeal, "ideal" + csvFileId.ToString() + ".csv");
            Saver.WriteCSV(outputData.FullDisplayedData.DisplayedDatasError, "error" + csvFileId.ToString() + ".csv");
            csvFileId++;
        }

        private void btnDb_Click(object sender, RoutedEventArgs e)
        {
            Saver.WriteDB(outputData, "data" + dbFileId.ToString() + ".db");
            dbFileId++;
        }
        private void btnDisk_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
