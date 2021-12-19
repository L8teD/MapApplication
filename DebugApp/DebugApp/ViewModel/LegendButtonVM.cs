using DebugApp.Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static DebugApp.Converters.LegendButtonConverter;

namespace DebugApp.ViewModel
{
    public class LegendButtonVM :BaseViewModel
    {
        public Visibility VisPropertyName
        {
            get { return (Visibility)GetValue(VisProperty); }
            set { SetValue(VisProperty, value); }
        }
        public static readonly DependencyProperty VisProperty =
          DependencyProperty.Register("VisPropertyName", typeof(Visibility), typeof(LegendButtonVM), new PropertyMetadata(Visibility.Hidden));
        public string TextPropertyName
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("TextPropertyName", typeof(string), typeof(LegendButtonVM), new PropertyMetadata(default(string)));

        public SolidColorBrush ColorPropertyName
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("ColorPropertyName", typeof(SolidColorBrush), typeof(LegendButtonVM), new PropertyMetadata(default(SolidColorBrush)));
        private MainModel m_Model;
        private PlotVM m_plotVM;
        private List<LineSeries> removedSeries;
        public LegendButtonVM(MainModel model, PlotVM plotVM)
        {
            m_Model = model;
            m_plotVM = plotVM;
            removedSeries = new List<LineSeries>();
        }
        private RelayCommand cmd_Pressed;
        public RelayCommand Cmd_Pressed
        {
            get
            {
                return cmd_Pressed ??
                (cmd_Pressed = new RelayCommand(obj =>
                {
                    LegendButtonValue legBtnVal = obj as LegendButtonValue;
                    if (!legBtnVal.btnIsChecked)
                    {
                        LineSeries lineSeries = m_Model.IndicatedSeries.Find(item => item.Title == legBtnVal.seriesText);
                        m_Model.IndicatedSeries.Remove(lineSeries);
                        removedSeries.Add(lineSeries);
                    }
                    else
                    {
                        LineSeries lineSeries = removedSeries.Find(item => item.Title == legBtnVal.seriesText);
                        m_Model.IndicatedSeries.Add(lineSeries);
                        removedSeries.Remove(lineSeries);
                    }
                    m_plotVM.Plot(m_Model.IndicatedSeries);
                }));
            }
        }
        public void UpdateLegendVis(Visibility obj)
        {
            syncContext.Send(SendVisibility, obj);
        }
        private void SendVisibility(object vis)
        {
            VisPropertyName = (Visibility)vis;
        }
        public void UpdateLegendColor(SolidColorBrush color)
        {
            syncContext.Send(SendEllipseColor, color);
        }
        private void SendEllipseColor(object color)
        {
            ColorPropertyName = (SolidColorBrush)color;
        }
        public void UpdateLegendText(string obj)
        {
            syncContext.Send(SendText, obj);
        }
        private void SendText(object text)
        {
            TextPropertyName = (string)text;
        }

        
        
    }
}
