using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DebugApp.Types;

namespace DebugApp
{
    class MainModel
    {
        OutputData outputData;
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
            Execute.CreateTrajectory(initData, ref outputData);

            CreatePlotData();
        }
        private void CreatePlotData()
        {
            PlotWorker.InitListOfPlotData();
            PlotWorker.dataIsUpdated = true;
            for (int i = 0; i <outputData.FullDisplayedData.DisplayedDatasIdeal.Count; i++)
            {
                PlotWorker.AddPlotDataToStruct(outputData.FullDisplayedData, i);
            }
        }
        public MainModel()
        {
            
        }
        public RouteTurningPoint SetRTP()
        {
            return new RouteTurningPoint() { Latitude = 55, Longitude = 37, Altitude = 3000, Velocity = 1224 };
        }
        public (ObservableCollection<InputError>, ObservableCollection<InputError>) SetInputErrors()
        {
            ObservableCollection<InputError>  insErrors = new ObservableCollection<InputError>();
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
