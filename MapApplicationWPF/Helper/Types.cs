using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MapApplicationWPF.ExternalResourses;
using static CommonLib.Types;
using static ModellingErrorsLib.Types;

namespace MapApplicationWPF.Helper
{
    public class Types
    {
        public struct InitData
        {
            public ObservableCollection<PPM> ppmList;
            public InitErrors initErrors;
        }
        public struct OutputData
        {
            public List<PointSet> Points { get;  set; }
            public List<VelocitySet> Velocities { get;  set; }
            public FullDisplayedData FullDisplayedData{ get;  set; }
            //public OutputData(List<PointSet> _outputPoints, List<VelocitySet> _Velocities)
            //{
            //    Points = _outputPoints;
            //    Velocities = _Velocities;
            //}
        }
        public struct FullDisplayedData
        {
            public List<DisplayedData> DisplayedDatasIdeal { get; set; }
            public List<DisplayedData> DisplayedDatasError { get; set; }

        }

    }
}
