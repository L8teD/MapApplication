using MapApplicationWPF.Graphic;
using MapApplicationWPF.Helper;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapApplicationWPF.UserControls
{
    public delegate void PlotHandler();
    public partial class DataRow : UserControl
    {
        public event PlotHandler PlotEventHandler;
        public string ParameterName { get; set; }
        public string Dimension { get; set; }
        double lastValue = 0;
        double currValue;

        public readonly object objectLock = new object();
        public event PlotHandler SetPlotFactor
        {
            add { lock (objectLock) { PlotEventHandler += value; } }
            remove { lock (objectLock) { PlotEventHandler -= value; } }
        }
        public string ContentOperationName
        {
            get { return (string)GetValue(ContentOperationProperty); }
            set { SetValue(ContentOperationProperty, value); }
        }
        public static readonly DependencyProperty ContentOperationProperty =
            DependencyProperty.Register("ContentOperationName", typeof(string), typeof(DataRow), new PropertyMetadata(default(string)));

        public SolidColorBrush FillOperationName
        {
            get { return (SolidColorBrush)GetValue(FillOperationProperty); }
            set { SetValue(FillOperationProperty, value); }
        }
        public static readonly DependencyProperty FillOperationProperty =
            DependencyProperty.Register("FillOperationName", typeof(SolidColorBrush), typeof(DataRow), new PropertyMetadata(default(string)));
        public void SendMessage(object messageRaw)
        {
            ContentOperationName = (string)messageRaw;
            //try
            //{
            //    currValue = Convert.ToDouble(messageRaw, CultureInfo.InvariantCulture);
            //    if (lastValue != 0)
            //    {
            //        if (currValue > lastValue)
            //            label.Foreground = Brushes.LightGreen;
            //        else if(currValue < lastValue)
            //            label.Foreground = Brushes.Red;
            //    }
            //    lastValue = currValue;

            //}
            //catch (Exception ex)
            //{

            //}
        }
        public void SendColorMessage(object messageRaw)
        {
            FillOperationName = (SolidColorBrush)messageRaw;
        }
        public DataRow()
        {
            InitializeComponent();

            PlotEventHandler += SetPlotTitle;
            PlotEventHandler += SetPlotDimension;

            DataContext = this;
        }
        private void SetPlotDimension()
        {
            PlotWorker.SetPlotDimension(Dimension);
        }
        private void SetPlotTitle()
        {
            PlotWorker.SetPlotTitle(ParameterName);
        }
        private RelayCommand cmd_OpenPlot;
        public RelayCommand Cmd_OpenPlot
        {
            get
            {
                return cmd_OpenPlot ??
                (cmd_OpenPlot = new RelayCommand(obj =>
                {
                    PlotEventHandler.Invoke();
                    PlotWorker.Plot();

                }));
            }
        }
    }
}
