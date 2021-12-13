﻿using DebugApp.ViewModel;
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
        public PlotWindow(string title)
        {
            InitializeComponent();
            plot.Title = title;
            this.Loaded += PlotWindow_Loaded;
        }

        private void PlotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new PlotViewModel(plot.Title, true);
        }
    }
}
