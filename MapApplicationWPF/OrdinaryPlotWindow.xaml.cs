﻿using OxyPlot.Series;
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
using static MapApplicationWPF.Helper.Types;

namespace MapApplicationWPF
{
    public partial class OrdinaryPlotWindow : Window
    {
        public OrdinaryPlotWindow(string xAxisName, string yAxisName, string plotTitle, List<LineSeries> lineSeries)
        {
            InitializeComponent();
            plot.SetPlotState(xAxisName, yAxisName, plotTitle, lineSeries);
        }
    }
}
