using MapApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MapApplication.ViewModel
{
    public class DataRowVM : BaseViewModel
    {
        private string parameterName;
        public string ParameterName
        {
            get { return parameterName; }
            set
            {
                if (parameterName == null)
                    parameterName = value;
            }
        }
        private string dimension;
        public string Dimension
        {
            get { return dimension; }
            set
            {
                if (dimension == null)
                    dimension = value;
            }
        }
        public string UpdateValueName
        {
            get { return (string)GetValue(UpdateValueNameProperty); }
            set { SetValue(UpdateValueNameProperty, value); }
        }
        public static readonly DependencyProperty UpdateValueNameProperty =
          DependencyProperty.Register("UpdateValueName", typeof(string), typeof(DataRowVM), new PropertyMetadata(default(string)));

        public SolidColorBrush FillOperationName
        {
            get { return (SolidColorBrush)GetValue(FillOperationNameProperty); }
            set { SetValue(FillOperationNameProperty, value); }
        }
        public static readonly DependencyProperty FillOperationNameProperty =
            DependencyProperty.Register("FillOperationName", typeof(SolidColorBrush), typeof(DataRowVM), new PropertyMetadata(Brushes.Green));
        public DataRowVM(PlotName plotName, PlotCharacter character)
        {
            ParameterName = PlotWorker.SelectPlotName(plotName);
            Dimension = PlotWorker.SelectPlotDimension(plotName, character);
        }
        public void UpdateEllipseColor(SolidColorBrush color)
        {
            syncContext.Send(SendEllipseColor, color);
        }
        private void SendEllipseColor(object color)
        {
            FillOperationName = (SolidColorBrush)color;
        }
        public void UpdateValueMessage(string obj)
        {
            syncContext.Send(SendValueMessage, obj);
        }
        private void SendValueMessage(object text)
        {
            UpdateValueName = (string)text;
        }
    }
}
