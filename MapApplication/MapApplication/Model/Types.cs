using CommonLib;
using MapApplication.ViewModel;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.Model
{
    public class DisplayGraphicData
    {
        private Source activeSource;
        private List<PlotData> display;
        public List<PlotData> Display
        {
            get { return display; }
            set
            {
                if (value == null) return;

                if (display == null)
                    display = new List<PlotData>();

                else if (display.Equals(value)) return;

                display.Clear();
                display.AddRange(value);
            }
        }
        public List<PlotData> INS;
        public List<PlotData> GNSS;
        public List<PlotData> KVS;
        public void SwitchSource(Source source)
        {
            switch (source)
            {
                case Source.INS:
                    display = INS;
                    break;
                case Source.GNSS:
                    display = GNSS;
                    break;
                case Source.KVS:
                    display = KVS;
                    break;
                default:
                    display = INS;
                    break;
            }
            activeSource = source;
        }
        public DisplayGraphicData Copy()
        {
            DisplayGraphicData copy = new DisplayGraphicData(activeSource);
            copy.INS = PlotWorker.DublicatePlotData(INS);
            copy.GNSS = PlotWorker.DublicatePlotData(GNSS);
            copy.KVS = PlotWorker.DublicatePlotData(KVS);
            copy.SwitchSource(activeSource);
            return copy;
        }
        public DisplayGraphicData()
        {
            SwitchSource(Source.INS);
        }
        public DisplayGraphicData(Source source)
        {
            SwitchSource(source);
        }



    }
    public class InitData
    {
        public ObservableCollection<WayPoint> wayPointList { get; set; }

        public ObservableCollection<EquipmentData> insErrors { get; set; }
        public ObservableCollection<EquipmentData> sensorErrors { get; set; }

        public ObservableCollection<EquipmentData> gnssErrors { get; set; }

        public ObservableCollection<EquipmentData> airInfo { get; set; }

        public ObservableCollection<EquipmentData> windInfoDryden { get; set; }
        public ObservableCollection<EquipmentData> windInfo { get; set; }
    }
    public class ReportData
    {
        public ObservableCollection<EquipmentData> trajectorySettings { get;set; }
        public ObservableCollection<EquipmentData> weatherSettings { get;set; }
    }
    public enum DataSource
    {
        twoChannel,
        threeChannel,
        twoChannelFeedback,
        threeChannelFeedback
    }
    public class PlotData
    {
        public PlotName name;
        public PlotCharacter character;
        public Source source;
        public List<DataPoint> values;
        public string xAxisName;
        public string yAxisName;
        public PlotData(PlotName _name, PlotCharacter _character,Source _source, List<double> _values)
        {
            name = _name;
            character = _character;
            source = _source;
            xAxisName = "time, sec";
            yAxisName = PlotWorker.SelectPlotName(_name) + ", " + PlotWorker.SelectPlotDimension(_name, character);
            values = new List<DataPoint>();
            //if (_name == PlotName.VelocityEast && _source == Source.KVS && _character == PlotCharacter.Estimate && _values.Count > 0)
            //{
            //    MyMatrix.Vector x = Common.Aproximate(_values, 3);
            //    _values.Clear();
            //    for (int i = 0; i < x.Length; i++)
            //        _values.Add(x[i+1]);
            //}
            for (int i = 0; i < _values.Count; i++)
            {
                values.Add(new DataPoint(i, _values[i]));
            }
        }
    }
    public enum TrajectoryType
    {
        DesiredTrack,
        ActualTrack
        //AdditionalTrack
    }
    public enum Source
    {
        INS,
        GNSS,
        KVS
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
        Pitch,
        None
    }
    public enum ActivePlotState
    {
        Trajectory,
        Error,
        P
    }
}
