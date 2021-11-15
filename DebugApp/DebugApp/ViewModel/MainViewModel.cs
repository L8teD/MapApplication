using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DebugApp.Types;

namespace DebugApp
{
    public class MainViewModel : DependencyObject
    {
        private MainModel m_Model;
        private RouteTurningPoint rtp;
        public RouteTurningPoint RTP
        {
            get { return rtp; }
            set
            {
                rtp = value;
                OnPropertyChanged("RTP");
            }
        }
        public InitData initData { get; set; }
        public ObservableCollection<LogInfo> loggerInfoList { get; set; }

        #region Commands
        private RelayCommand cmd_AddRTP;
        public RelayCommand Cmd_AddRTP
        {
            get
            {
                return cmd_AddRTP ??
                (cmd_AddRTP = new RelayCommand(obj =>
                {
                    if (obj is ObservableCollection<RouteTurningPoint>);
                        m_Model.AddRTP((ObservableCollection<RouteTurningPoint> )obj, RTP);
                }));
            }
        }
        private RelayCommand cmd_RemoveRTP;
        public RelayCommand Cmd_RemoveRTP
        {
            get
            {
                return cmd_RemoveRTP ??
                (cmd_RemoveRTP = new RelayCommand(obj =>
                {
                    Button button = obj as Button;
                    m_Model.RemoveRTP(initData.rtpList, button);

                }));
            }
        }
        private RelayCommand cmd_SelectionChanged;
        public RelayCommand Cmd_SelectionChanged
        {
            get
            {
                return cmd_SelectionChanged ??
                (cmd_SelectionChanged = new RelayCommand(obj =>
                {

                    m_Model.SetDataFromLogger((LogInfo)obj, initData.rtpList);

                }));
            }
        }
        private RelayCommand cmd_DeleteLogElement;
        public RelayCommand Cmd_DeleteLogElement
        {
            get
            {
                return cmd_DeleteLogElement ??
                (cmd_DeleteLogElement = new RelayCommand(obj =>
                {
                    m_Model.RemoveDataFromLogger();
                    loggerInfoList = m_Model.GetInfoFromLogger();
                }));
            }
        }
        private RelayCommand cmd_Compute;
        public RelayCommand Cmd_Compute
        {
            get
            {
                return cmd_Compute ??
                (cmd_Compute = new RelayCommand(obj =>
                {
                    m_Model.Compute(initData);

                }));
            }
        }
        #endregion

        public MainViewModel()
        {
            m_Model = new MainModel();
            //RTP = new RouteTurningPoint();
            RTP = m_Model.SetRTP();

            initData = new InitData();
            initData.rtpList = new ObservableCollection<RouteTurningPoint>();
            (initData.insErrors, initData.sensorErrors) = m_Model.SetInputErrors();
            loggerInfoList = m_Model.GetInfoFromLogger();
            
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
