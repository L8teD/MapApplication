using MapApplication.Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MapApplication.Model.Types;

namespace MapApplication.ViewModel
{
    public class PlotTableVM
    {
        public PlotControlVM LongitudePlotControlVM { get; set; }
        public PlotControlVM LatitudePlotControlVM { get; set; }
        public PlotControlVM AltitudePlotControlVM { get; set; }
        public PlotControlVM V_EastPlotControlVM { get; set; }
        public PlotControlVM V_NorthPlotControlVM { get; set; }
        public PlotControlVM V_VerticalPlotControlVM { get; set; }
        public PlotControlVM HeadingPlotControlVM { get; set; }
        public PlotControlVM PitchPlotControlVM { get; set; }
        public PlotControlVM RollPlotControlVM { get; set; }

        public PlotTableVM(MainModel m_Model)
        {
            LongitudePlotControlVM = new PlotControlVM(PlotName.Longitude, m_Model);
            LatitudePlotControlVM = new PlotControlVM(PlotName.Latitude, m_Model);
            AltitudePlotControlVM = new PlotControlVM(PlotName.Altitude, m_Model);
            V_EastPlotControlVM = new PlotControlVM(PlotName.VelocityEast, m_Model);
            V_NorthPlotControlVM = new PlotControlVM(PlotName.VelocityNorth, m_Model);
            V_VerticalPlotControlVM = new PlotControlVM(PlotName.VelocityH, m_Model);
            HeadingPlotControlVM = new PlotControlVM(PlotName.Heading, m_Model);
            RollPlotControlVM = new PlotControlVM(PlotName.Roll, m_Model);
            PitchPlotControlVM = new PlotControlVM(PlotName.Pitch, m_Model);

            m_Model.RefreshLongitudePlot += M_Model_RefreshLongitudePlot;
            m_Model.RefreshLatitudePlot += M_Model_RefreshLatitudePlot;
            m_Model.RefreshAltitudePlot += M_Model_RefreshAltitudePlot;
            m_Model.RefreshV_EastPlot += M_Model_RefreshV_EastPlot;
            m_Model.RefreshV_NorthPlot += M_Model_RefreshV_NorthPlot;
            m_Model.RefreshV_VerticalPlot += M_Model_RefreshV_VerticalPlot;
            m_Model.RefreshHeadingPlot += M_Model_RefreshHeadingPlot;
            m_Model.RefreshPitchPlot += M_Model_RefreshPitchPlot;
            m_Model.RefreshRollPlot += M_Model_RefreshRollPlot;
        }
        private void M_Model_RefreshRollPlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            RollPlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshPitchPlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            PitchPlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshHeadingPlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            HeadingPlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshV_VerticalPlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            V_VerticalPlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshV_NorthPlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            V_NorthPlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshV_EastPlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            V_EastPlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshAltitudePlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            AltitudePlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshLatitudePlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            LatitudePlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }

        private void M_Model_RefreshLongitudePlot(string xAxisName, string yAxisName, List<LineSeries> seriesList)
        {
            LongitudePlotControlVM.Plot(xAxisName, yAxisName, seriesList);
        }
    }
}
