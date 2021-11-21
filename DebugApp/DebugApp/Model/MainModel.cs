using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DebugApp.Logger;
using static DebugApp.Types;

namespace DebugApp
{
    class MainModel
    {
        OutputData outputData;
        List<DebugInfo> infoList;
        DebugInfo selectedInfo;
        public void AddRTP(ObservableCollection<RouteTurningPoint> rtpList, RouteTurningPoint RTP)
        {
            ListViewWorker.UpdateData(rtpList, RTP);
        }
        public void RemoveRTP(ObservableCollection<RouteTurningPoint> rtpList, Button button)
        {
            int id = (int)button.Tag;
            ListViewWorker.RemoveElement(rtpList, id);         
        }
        public void Compute(InitData initData)
        {
            outputData = new OutputData();
            try
            {
                Execute.CreateTrajectory(initData, ref outputData);
                CreatePlotData();
            }
            catch(Exception ex)
            {
                Logger.PrintErrorInfo(ex.Message, initData);
            }

            
        }
        private void CreatePlotData()
        {
            PlotWorker.InitListOfPlotData();
            for (int i = 0; i < outputData.FullDisplayedData.DisplayedDatasIdeal.Count; i++)
            {
                PlotWorker.AddPlotDataToStruct(outputData.FullDisplayedData, i);
            }
        }
        public MainModel()
        {
            
        }
        public void SetDataFromLogger(LogInfo info, ObservableCollection<RouteTurningPoint> rtpList)
        {
            string[] infoString = info.Element.Split('|');
            int ID = Convert.ToInt32(infoString[7].Split(' ')[1]);
            selectedInfo = infoList.Find(item => item.id == ID);
            string[] latitude = selectedInfo.input.latitude.Split(' ');
            string[] longitude = selectedInfo.input.longitude.Split(' ');
            string[] altitude = selectedInfo.input.altitude.Split(' ');
            string[] velocity = selectedInfo.input.velocity.Split(' ');
            rtpList.Clear();
            for (int i = 0; i < selectedInfo.CountOfPoints; i++)
            {
                RouteTurningPoint RTP = new RouteTurningPoint();
                RTP.Latitude = Convert.ToDouble(latitude[i]);
                RTP.Longitude = Convert.ToDouble(longitude[i]);
                RTP.Altitude = Convert.ToDouble(altitude[i]);
                RTP.Velocity = Convert.ToDouble(velocity[i]);
                AddRTP(rtpList, RTP);
            }
        }
        public void RemoveDataFromLogger()
        {
            Logger.RemoveDataFromDB(selectedInfo.id);
        }
        public ObservableCollection<LogInfo> GetInfoFromLogger()
        {
            ObservableCollection<LogInfo> logInfo = new ObservableCollection<LogInfo>();
            infoList = ReadInfoFromDB();

            foreach (DebugInfo info in infoList)
            {
                string temp = "Date: " + info.Date + "\n|"
                    +"Error: "+ info.Message + "\n|"
                    + "Input:\n|" + "Lat: " + info.input.latitude + "\n|" + "Lon: " + info.input.longitude + "\n|"
                    + "Alt: "+ info.input.altitude + "\n|" + "Vel: " + info.input.velocity + "\n|" + "ID: " + info.id.ToString();
                
                logInfo.Add(new LogInfo() {Element=temp });
            }
            return logInfo;
        }
        public RouteTurningPoint SetRTP()
        {
            return new RouteTurningPoint() { Latitude = 55, Longitude = 37, Altitude = 3000, Velocity = 1224 };
        }
        public (ObservableCollection<InputError>, ObservableCollection<InputError>) SetInputErrors()
        {
            ObservableCollection<InputError> insErrors = new ObservableCollection<InputError>();
            ObservableCollection<InputError> sensorErrors = new ObservableCollection<InputError>();

            insErrors.Add(new InputError() { Name = "α", Value = 0.25, Dimension = "[deg/h]" });
            insErrors.Add(new InputError() { Name = "β", Value = 0.03, Dimension = "[deg/h]" });
            insErrors.Add(new InputError() { Name = "γ", Value = 0.03, Dimension = "[deg/h]" });

            insErrors.Add(new InputError() { Name = "Δλ", Value = 15, Dimension = "[m]" });
            insErrors.Add(new InputError() { Name = "Δφ", Value = 15, Dimension = "[m]" });
            insErrors.Add(new InputError() { Name = "ΔH", Value = 15, Dimension = "[m]" });

            insErrors.Add(new InputError() { Name = "ΔVe", Value = 0.5, Dimension = "[m/s]" });
            insErrors.Add(new InputError() { Name = "ΔVn", Value = 0.5, Dimension = "[m/s]" });
            insErrors.Add(new InputError() { Name = "ΔVh", Value = 0.5, Dimension = "[m/s]" });

            sensorErrors.Add(new InputError() { Name = "Δn1", Value = 6E-06, Dimension = "[g]" });
            sensorErrors.Add(new InputError() { Name = "Δn2", Value = 6E-06, Dimension = "[g]" });
            sensorErrors.Add(new InputError() { Name = "Δn3", Value = 6E-06, Dimension = "[g]" });

            sensorErrors.Add(new InputError() { Name = "ΔΩ1", Value = 0.001, Dimension = "[deg/h]" });
            sensorErrors.Add(new InputError() { Name = "ΔΩ2", Value = 0.001, Dimension = "[deg/h]" });
            sensorErrors.Add(new InputError() { Name = "ΔΩ3", Value = 0.001, Dimension = "[deg/h]" });

            //items.Add(new InputError() { Name = "ΔXc", Value = 10, Dimension = "[m]" });
            //items.Add(new InputError() { Name = "ΔVc", Value = 0.1, Dimension = "[m/s]" });

            //SaveInitDataHandler += ListViewWorker.SaveInitDataHandler;

            return (insErrors, sensorErrors);
        }
    }
}
