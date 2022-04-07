using MapApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MapApplication.Model.Types;

namespace MapApplication.ViewModel
{
    public class DataTableWithChangesVM : BaseViewModel
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
        public DataRowWithChangesVM LongitudeDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM LatitudeDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM AltitudeDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM V_EastDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM V_NorthDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM V_VerticalDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM HeadingDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM PitchDataRowWithChangesVM { get; set; }
        public DataRowWithChangesVM RollDataRowWithChangesVM { get; set; }

        public DataTableWithChangesVM(PlotCharacter character)
        {
            TableTitle = PlotWorker.SelectPlotCharacter(character);

            LongitudeDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.Longitude, character);
            LatitudeDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.Latitude, character);
            AltitudeDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.Altitude, character);
            V_EastDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.VelocityEast, character);
            V_NorthDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.VelocityNorth, character);
            V_VerticalDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.VelocityH, character);
            HeadingDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.Heading, character);
            RollDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.Roll, character);
            PitchDataRowWithChangesVM = new DataRowWithChangesVM(PlotName.Pitch, character);
        }
        public void LongitudeValueMessage(string obj)
        {
            LongitudeDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void LatitudeValueMessage(string obj)
        {
            LatitudeDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void AltitudeValueMessage(string obj)
        {
            AltitudeDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void V_EastValueMessage(string obj)
        {
            V_EastDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void V_NortheValueMessage(string obj)
        {
            V_NorthDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void V_VerticalValueMessage(string obj)
        {
            V_VerticalDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void HeadingValueMessage(string obj)
        {
            HeadingDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void RollValueMessage(string obj)
        {
            RollDataRowWithChangesVM.UpdateValueMessage(obj);
        }
        public void PitchValueMessage(string obj)
        {
            PitchDataRowWithChangesVM.UpdateValueMessage(obj);
        }
    }
}
