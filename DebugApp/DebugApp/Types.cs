using DebugApp.ViewModel;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static ModellingErrorsLib3.Types;

namespace DebugApp.Model
{
    public class Types
    {
        public class InitData
        {
            public ObservableCollection<RouteTurningPoint> rtpList { get; set; }
            public ObservableCollection<InputError> insErrors { get; set; }
            public ObservableCollection<InputError> sensorErrors { get; set; }
        }
        public class PlotData
        {
            public PlotName name;
            public PlotCharacter character;
            public List<DataPoint> values;
            public string xAxisName;
            public string yAxisName;
            public PlotData(PlotName _name, PlotCharacter _character, List<double> _values)
            {
                name = _name;
                character = _character;
                xAxisName = "time, sec";
                yAxisName = PlotWorker.SelectPlotName(_name) + ", " + PlotWorker.SelectPlotDimension(_name, character);
                values = new List<DataPoint>();
                for (int i = 0; i < _values.Count; i++)
                {
                    values.Add(new DataPoint(i, _values[i]));
                }
            }
        }
        public enum PlotCharacter
        {
            Ideal,
            Error,
            Real,
            Estimate,
            CorrectError,
            CorrectTrajectory,
            P,
            None
        }
        public enum PlotName
        {
            Longitude,
            Latitude,
            Altitude,
            VelocityEast,
            VelocityNorth,
            VelocityH,
            Heading,
            Roll,
            Pitch
        }
    }
}
