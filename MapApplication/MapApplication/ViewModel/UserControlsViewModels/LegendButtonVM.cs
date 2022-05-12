using MapApplication.Model;
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
using static MapApplication.View.LegendButtonConverter;

namespace MapApplication.ViewModel
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
        public bool IsCheckedPropertyName
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsCheckedPropertyName", typeof(bool), typeof(LegendButtonVM), new PropertyMetadata(true));
        private IPlotControl m_PlotControlVM;
        private PlotVM m_plotVM;
        public LegendButtonVM(IPlotControl plotControlVM, PlotVM plotVM)
        {
            m_PlotControlVM = plotControlVM;
            m_plotVM = plotVM;
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
                        LineSeries lineSeries = m_PlotControlVM.IndicatedSeries.Find(item => item.Title == legBtnVal.seriesText);
                        m_PlotControlVM.IndicatedSeries.Remove(lineSeries);
                        m_PlotControlVM.RemovedSeries.Add(lineSeries);
                    }
                    else
                    {
                        LineSeries lineSeries = m_PlotControlVM.RemovedSeries.Find(item => item.Title == legBtnVal.seriesText);
                        m_PlotControlVM.IndicatedSeries.Add(lineSeries);
                        m_PlotControlVM.RemovedSeries.Remove(lineSeries);
                    }
                    m_plotVM.Plot(m_PlotControlVM.IndicatedSeries);
                }));
            }
        }
        public void UpdateLegendIsChecked(bool obj)
        {
            syncContext.Send(SendIsChecked, obj);
        }
        private void SendIsChecked(object isChecked)
        {
            IsCheckedPropertyName = (bool)isChecked;
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
