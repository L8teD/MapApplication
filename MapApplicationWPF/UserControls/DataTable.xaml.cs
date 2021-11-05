using MapApplicationWPF.Graphic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;

using static CommonLib.Types;
using static ModellingTrajectoryLib.Types;

namespace MapApplicationWPF.UserControls
{

    public partial class DataTable : UserControl
    {
        public string TableTitle { get; set; }

        public SynchronizationContext m_SyncContext;
        #region DataRows Actions
        public event Action<string> LatitudeMessage;
        public event Action<SolidColorBrush> LatitudeColorMessage;
        public event Action<string> LongitudeMessage;
        public event Action<SolidColorBrush> LongitudeColorMessage;
        public event Action<string> AltitudeMessage;
        public event Action<SolidColorBrush> AltitudeColorMessage;
        public event Action<string> VelocityMessage;
        public event Action<SolidColorBrush> VelocityColorMessage;
        public event Action<string> VelocityEastMessage;
        public event Action<SolidColorBrush> VelocityEastColorMessage;
        public event Action<string> VelocityNorthMessage;
        public event Action<SolidColorBrush> VelocityNorthColorMessage;
        public event Action<string> VelocityHMessage;
        public event Action<SolidColorBrush> VelocityHColorMessage;
        public event Action<string> HeadingMessage;
        public event Action<SolidColorBrush> HeadingColorMessage;
        public event Action<string> PitchMessage;
        public event Action<SolidColorBrush> PitchColorMessage;
        public event Action<string> RollMessage;
        public event Action<SolidColorBrush> RollColorMessage;
        #endregion
        public DataTable()
        {
            InitializeComponent();
            DataContext = this;

            #region DataRows Actions Build Events
            LatitudeMessage += MainWindow_LatitudeMessage;
            LatitudeColorMessage += MainWindow_LatitudeColorMessage;
            LongitudeMessage += MainWindow_LongitudeMessage;
            LongitudeColorMessage += MainWindow_LongitudeColorMessage;
            AltitudeMessage += MainWindow_AltitudeMessage;
            AltitudeColorMessage += MainWindow_AltitudeColorMessage;
            VelocityMessage += MainWindow_VelocityMessage;
            VelocityColorMessage += MainWindow_VelocityColorMessage;
            VelocityEastMessage += MainWindow_VelocityEastMessage;
            VelocityEastColorMessage += MainWindow_VelocityEastColorMessage;
            VelocityNorthMessage += MainWindow_VelocityNorthMessage;
            VelocityNorthColorMessage += MainWindow_VelocityNorthColorMessage;
            VelocityHMessage += MainWindow_VelocityNorthMessage;
            VelocityHColorMessage += MainWindow_VelocityNorthColorMessage;
            HeadingMessage += MainWindow_HeadingMessage;
            HeadingColorMessage += MainWindow_HeadingColorMessage;
            PitchMessage += MainWindow_PitchMessage;
            PitchColorMessage += MainWindow_PitchColorMessage;
            RollMessage += MainWindow_RollMessage;
            RollColorMessage += MainWindow_RollColorMessage;
            #endregion

            //dr_Latitude.PlotEventHandler += SetPlotFactor;

        }
        public void SetPlotFactor()
        {
            PlotWorker.SetPlotFactor(TableTitle);
        }
        #region DataRows Action Events
        private void MainWindow_RollColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Roll.SendColorMessage, obj);
        }

        private void MainWindow_RollMessage(string obj)
        {
            m_SyncContext.Send(dr_Roll.SendMessage, obj);
        }

        private void MainWindow_PitchColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Pitch.SendColorMessage, obj);
        }

        private void MainWindow_PitchMessage(string obj)
        {
            m_SyncContext.Send(dr_Pitch.SendMessage, obj);
        }

        private void MainWindow_HeadingColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Heading.SendColorMessage, obj);
        }

        private void MainWindow_HeadingMessage(string obj)
        {
            m_SyncContext.Send(dr_Heading.SendMessage, obj);
        }
        private void MainWindow_VelocityHColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Velocity_North.SendColorMessage, obj);
        }

        private void MainWindow_VelocityHMessage(string obj)
        {
            m_SyncContext.Send(dr_Velocity_North.SendMessage, obj);
        }

        private void MainWindow_VelocityNorthColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Velocity_North.SendColorMessage, obj);
        }

        private void MainWindow_VelocityNorthMessage(string obj)
        {
            m_SyncContext.Send(dr_Velocity_North.SendMessage, obj);
        }

        private void MainWindow_VelocityEastColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Velocity_East.SendColorMessage, obj);
        }

        private void MainWindow_VelocityEastMessage(string obj)
        {
            m_SyncContext.Send(dr_Velocity_East.SendMessage, obj);
        }

        private void MainWindow_VelocityColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Velocity.SendColorMessage, obj);
        }

        private void MainWindow_VelocityMessage(string obj)
        {
            m_SyncContext.Send(dr_Velocity.SendMessage, obj);
        }
        private void MainWindow_AltitudeColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Altitude.SendColorMessage, obj);
        }

        private void MainWindow_AltitudeMessage(string obj)
        {
            m_SyncContext.Send(dr_Altitude.SendMessage, obj);
        }
        private void MainWindow_LongitudeColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Longitude.SendColorMessage, obj);
        }

        private void MainWindow_LongitudeMessage(string obj)
        {
            m_SyncContext.Send(dr_Longitude.SendMessage, obj);
        }

        private void MainWindow_LatitudeColorMessage(SolidColorBrush obj)
        {
            m_SyncContext.Send(dr_Latitude.SendColorMessage, obj);
        }

        private void MainWindow_LatitudeMessage(string obj)
        {
            m_SyncContext.Send(dr_Latitude.SendMessage, obj);
        }

        #endregion
        public void UpdateDisplayedData(DisplayedData displayedData)
        {

            LatitudeMessage.Invoke(displayedData.Latitude.ToString());
            LongitudeMessage.Invoke(displayedData.Longitude.ToString());
            AltitudeMessage.Invoke(displayedData.Altitude.ToString());
            VelocityMessage.Invoke(displayedData.Velocity.ToString());
            VelocityEastMessage.Invoke(displayedData.VelocityEast.ToString());
            VelocityNorthMessage.Invoke(displayedData.VelocityNorth.ToString());
            VelocityHMessage.Invoke(displayedData.VelocityH.ToString());
            HeadingMessage.Invoke(displayedData.Heading.ToString());
            PitchMessage.Invoke(displayedData.Pitch.ToString());
            RollMessage.Invoke(displayedData.Roll.ToString());


            LatitudeColorMessage.Invoke(Brushes.Green);
            LongitudeColorMessage.Invoke(Brushes.Green);
            AltitudeColorMessage.Invoke(Brushes.Green);
            VelocityColorMessage.Invoke(Brushes.Green);
            VelocityEastColorMessage.Invoke(Brushes.Green);
            VelocityNorthColorMessage.Invoke(Brushes.Green);
            VelocityHColorMessage.Invoke(Brushes.Green);
            HeadingColorMessage.Invoke(Brushes.Green);
            PitchColorMessage.Invoke(Brushes.Green);
            RollColorMessage.Invoke(Brushes.Green);
        }
    }
}
