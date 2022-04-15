using MapApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapApplication.ViewModel
{
    public class DataTableVM : BaseViewModel
    {
        private string tableTitle;
        public string TableTitle
        {
            get { return tableTitle; }
            set
            {
                if (tableTitle == null)
                    tableTitle = value;
            }
        }
        public DataRowVM LongitudeDataRowVM { get; set; }
        public DataRowVM LatitudeDataRowVM { get; set; }
        public DataRowVM AltitudeDataRowVM { get; set; }
        public DataRowVM V_EastDataRowVM { get; set; }
        public DataRowVM V_NorthDataRowVM { get; set; }
        public DataRowVM V_VerticalDataRowVM { get; set; }
        public DataRowVM HeadingDataRowVM { get; set; }
        public DataRowVM PitchDataRowVM { get; set; }
        public DataRowVM RollDataRowVM { get; set; }

        public DataTableVM(PlotCharacter character)
        {
            TableTitle = PlotWorker.SelectPlotCharacter(character);

            LongitudeDataRowVM = new DataRowVM(PlotName.Longitude, character);
            LatitudeDataRowVM = new DataRowVM(PlotName.Latitude, character);
            AltitudeDataRowVM = new DataRowVM(PlotName.Altitude, character);
            V_EastDataRowVM = new DataRowVM(PlotName.VelocityEast, character);
            V_NorthDataRowVM = new DataRowVM(PlotName.VelocityNorth, character);
            V_VerticalDataRowVM = new DataRowVM(PlotName.VelocityH, character);
            HeadingDataRowVM = new DataRowVM(PlotName.Heading, character);
            RollDataRowVM = new DataRowVM(PlotName.Roll, character);
            PitchDataRowVM = new DataRowVM(PlotName.Pitch, character);
        }
        public void LongitudeValueMessage(string obj)
        {
            LongitudeDataRowVM.UpdateValueMessage(obj);
        }
        public void LatitudeValueMessage(string obj)
        {
            LatitudeDataRowVM.UpdateValueMessage(obj);
        }
        public void AltitudeValueMessage(string obj)
        {
            AltitudeDataRowVM.UpdateValueMessage(obj);
        }
        public void V_EastValueMessage(string obj)
        {
            V_EastDataRowVM.UpdateValueMessage(obj);
        }
        public void V_NortheValueMessage(string obj)
        {
            V_NorthDataRowVM.UpdateValueMessage(obj);
        }
        public void V_VerticalValueMessage(string obj)
        {
            V_VerticalDataRowVM.UpdateValueMessage(obj);
        }
        public void HeadingValueMessage(string obj)
        {
            HeadingDataRowVM.UpdateValueMessage(obj);
        }
        public void RollValueMessage(string obj)
        {
            RollDataRowVM.UpdateValueMessage(obj);
        }
        public void PitchValueMessage(string obj)
        {
            PitchDataRowVM.UpdateValueMessage(obj);
        }
    }
}
