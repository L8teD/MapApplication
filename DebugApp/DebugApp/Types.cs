using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static ModellingErrorsLib3.Types;

namespace DebugApp
{
    public class Types
    {
        public class InitData
        {
            public ObservableCollection<RouteTurningPoint> rtpList { get; set; }
            public ObservableCollection<InputError> insErrors { get; set; }
            public ObservableCollection<InputError> sensorErrors { get; set; }
        }
        public struct OutputData
        {
            public List<PointSet> Points { get; set; }
            public List<VelocitySet> Velocities { get; set; }
            public List<AnglesSet> Angles { get; set; }
            public FullDisplayedData FullDisplayedData { get; set; }

        }
        public struct FullDisplayedData
        {
            public List<DisplayedData> DisplayedDatasIdeal { get; set; }
            public List<DisplayedData> DisplayedDatasError { get; set; }
            public List<DisplayedData> DisplayedDatasWithError { get; set; }

        }
    }
}
