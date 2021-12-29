using MapApplication.Model;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using static CommonLib.Types;
using static MapApplication.Model.Types;
using Point = CommonLib.Params.Point;

namespace MapApplication.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private Microsoft.Toolkit.Wpf.UI.Controls.MapControl Map;
        public InitData initData { get; set; }

        private MainModel m_Model;

        private RouteTurningPoint rtp;

        public PlotControlVM LongitudePlotControlVM { get; set; }
        public PlotControlVM LatitudePlotControlVM { get; set; }
        public PlotControlVM AltitudePlotControlVM { get; set; }
        public PlotControlVM V_EastPlotControlVM { get; set; }
        public PlotControlVM V_NorthPlotControlVM { get; set; }
        public PlotControlVM V_VerticalPlotControlVM { get; set; }
        public PlotControlVM HeadingPlotControlVM { get; set; }
        public PlotControlVM PitchPlotControlVM { get; set; }
        public PlotControlVM RollPlotControlVM { get; set; }

        public DataTableWithChangesVM IdealDataTable { get; set; }
        public DataTableVM ErrorDataTable { get; set; }
        public DataTableVM EstimateDataTable { get; set; }

        public RouteTurningPoint RTP
        {
            get { return rtp; }
            set
            {
                rtp = value;
                OnPropertyChanged("RTP");
            }
        }
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
                    //if (obj is ObservableCollection<RouteTurningPoint>)
                    m_Model.AddRTP(initData.rtpList, RTP);
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
        private RelayCommand cmd_Simulation;
        public RelayCommand Cmd_Simulation
        {
            get
            {
                return cmd_Simulation ??
                (cmd_Simulation = new RelayCommand(obj =>
                {
                    m_Model.Start();

                }));
            }
        }
        private RelayCommand cmd_Start;
        public RelayCommand Cmd_Start
        {
            get
            {
                return cmd_Start ??
                (cmd_Start = new RelayCommand(obj =>
                {
                    m_Model.Start();

                }));
            }
        }
        private RelayCommand cmd_Pause;
        public RelayCommand Cmd_Pause
        {
            get
            {
                return cmd_Pause ??
                (cmd_Pause = new RelayCommand(obj =>
                {
                    m_Model.Pause();

                }));
            }
        }
        private RelayCommand cmd_Stop;
        public RelayCommand Cmd_Stop
        {
            get
            {
                return cmd_Stop ??
                (cmd_Stop = new RelayCommand(obj =>
                {
                    m_Model.Stop();

                }));
            }
        }
        private RelayCommand cmd_X10;
        public RelayCommand Cmd_X10
        {
            get
            {
                return cmd_X10 ??
                (cmd_X10 = new RelayCommand(obj =>
                {
                    m_Model.SetDrawingSpeed(100);

                }));
            }
        }
        private RelayCommand cmd_X100;
        public RelayCommand Cmd_X100
        {
            get
            {
                return cmd_X100 ??
                (cmd_X100 = new RelayCommand(obj =>
                {
                    m_Model.SetDrawingSpeed(10);

                }));
            }
        }
        private RelayCommand cmd_X1000;
        public RelayCommand Cmd_X1000
        {
            get
            {
                return cmd_X1000 ??
                (cmd_X1000 = new RelayCommand(obj =>
                {
                    m_Model.SetDrawingSpeed(1);

                }));
            }
        }
        #endregion
        public MainViewModel(Microsoft.Toolkit.Wpf.UI.Controls.MapControl map)
        {
            Map = map;
            m_Model = new MainModel();

            initData = new InitData();

            initData.rtpList = new ObservableCollection<RouteTurningPoint>();


            SetMapView(Map);

            RTP = m_Model.SetRTP();

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

            m_Model.DrawTrajectoryAction += DrawLine;

            m_Model.UpdateTableData += M_Model_UpdateTableData;

            (initData.insErrors, initData.sensorErrors) = m_Model.SetInputErrors();
            loggerInfoList = m_Model.GetInfoFromLogger();

            IdealDataTable = new DataTableWithChangesVM(PlotCharacter.Ideal);
            ErrorDataTable = new DataTableVM(PlotCharacter.Error);
            EstimateDataTable = new DataTableVM(PlotCharacter.Estimate);

            MapElementWorker.AddAirportsOnMAp();
            MapElementsLayer airportsLayer = new MapElementsLayer() { MapElements = MapElementWorker.airportsMapElements };
            airportsLayer.MapElementPointerEntered += AirportsLayer_MapElementPointerEntered;
            airportsLayer.MapElementPointerExited += AirportsLayer_MapElementPointerExited;
            airportsLayer.MapElementClick += AirportsLayer_MapElementClick;
            Map.Layers.Add(airportsLayer);
        }

        private void M_Model_UpdateTableData(OutputData outputData, int second)
        {
            IdealDataTable.LongitudeValueMessage(outputData.points[second].Ideal.Degrees.lat.ToString());
            IdealDataTable.LatitudeDataRowWithChangesVM.UpdateValueMessage(outputData.points[second].Ideal.Degrees.lon.ToString());
            IdealDataTable.AltitudeDataRowWithChangesVM.UpdateValueMessage(outputData.points[second].Ideal.Degrees.alt.ToString());
            IdealDataTable.V_EastDataRowWithChangesVM.UpdateValueMessage(outputData.velocities[second].Ideal.E.ToString());
            IdealDataTable.V_NorthDataRowWithChangesVM.UpdateValueMessage(outputData.velocities[second].Ideal.N.ToString());
            IdealDataTable.V_VerticalDataRowWithChangesVM.UpdateValueMessage(outputData.velocities[second].Ideal.H.ToString());
            IdealDataTable.HeadingDataRowWithChangesVM.UpdateValueMessage(outputData.angles[second].Ideal.Degrees.heading.ToString());
            IdealDataTable.PitchDataRowWithChangesVM.UpdateValueMessage(outputData.angles[second].Ideal.Degrees.pitch.ToString());
            IdealDataTable.RollDataRowWithChangesVM.UpdateValueMessage(outputData.angles[second].Ideal.Degrees.roll.ToString());

            ErrorDataTable.LongitudeDataRowVM.UpdateValueMessage(outputData.points[second].Error.Meters.lat.ToString());
            ErrorDataTable.LatitudeDataRowVM.UpdateValueMessage(outputData.points[second].Error.Meters.lon.ToString());
            ErrorDataTable.AltitudeDataRowVM.UpdateValueMessage(outputData.points[second].Error.Meters.alt.ToString());
            ErrorDataTable.V_EastDataRowVM.UpdateValueMessage(outputData.velocities[second].Error.E.ToString());
            ErrorDataTable.V_NorthDataRowVM.UpdateValueMessage(outputData.velocities[second].Error.N.ToString());
            ErrorDataTable.V_VerticalDataRowVM.UpdateValueMessage(outputData.velocities[second].Error.H.ToString());
            ErrorDataTable.HeadingDataRowVM.UpdateValueMessage(outputData.angles[second].Error.Degrees.heading.ToString());
            ErrorDataTable.PitchDataRowVM.UpdateValueMessage(outputData.angles[second].Error.Degrees.pitch.ToString());
            ErrorDataTable.RollDataRowVM.UpdateValueMessage(outputData.angles[second].Error.Degrees.roll.ToString());

            EstimateDataTable.LongitudeDataRowVM.UpdateValueMessage(outputData.points[second].Estimate.Meters.lat.ToString());
            EstimateDataTable.LatitudeDataRowVM.UpdateValueMessage(outputData.points[second].Estimate.Meters.lon.ToString());
            EstimateDataTable.AltitudeDataRowVM.UpdateValueMessage(outputData.points[second].Estimate.Meters.alt.ToString());
            EstimateDataTable.V_EastDataRowVM.UpdateValueMessage(outputData.velocities[second].Estimate.E.ToString());
            EstimateDataTable.V_NorthDataRowVM.UpdateValueMessage(outputData.velocities[second].Estimate.N.ToString());
            EstimateDataTable.V_VerticalDataRowVM.UpdateValueMessage(outputData.velocities[second].Estimate.H.ToString());
            //EstimateDataTable.HeadingDataRowVM.UpdateValueMessage(outputData.angles[second].Error.Estimate.heading.ToString());
            //EstimateDataTable.PitchDataRowVM.UpdateValueMessage(outputData.angles[second].Error.Estimate.pitch.ToString());
            //EstimateDataTable.RollDataRowVM.UpdateValueMessage(outputData.angles[second].Error.Estimate.roll.ToString());
        }

        private async void SetMapView(Microsoft.Toolkit.Wpf.UI.Controls.MapControl Map)
        {
            Geopoint maiLocation = new Geopoint(new BasicGeoposition() { Latitude = 55.811685, Longitude = 37.502471 });
            await Map.TrySetViewAsync(maiLocation, 6);

            
        }
        #region MapElement Events
        private void AirportsLayer_MapElementClick(MapElementsLayer sender, MapElementsLayerClickEventArgs args)
        {
            UpdateMapElementOnClick(args.MapElements[0]);
        }

        private void AirportsLayer_MapElementPointerExited(MapElementsLayer sender, MapElementsLayerPointerExitedEventArgs args)
        {
            UpdateMapElementOnPointerExited(args.MapElement);
        }

        private void AirportsLayer_MapElementPointerEntered(MapElementsLayer sender, MapElementsLayerPointerEnteredEventArgs args)
        {
            UpdateMapElementOnPointerEntered(args.MapElement);
        }
        #endregion
        private async void DrawLine(Point startPoint, Point endPoint, Windows.UI.Color color)
        {
            try
            {
                await Dispatcher.InvokeAsync(new Action(delegate ()
                {
                    MapPolyline mapPolyline = new MapPolyline();
                    mapPolyline.Path = new Geopath(new List<BasicGeoposition>() {
                    new BasicGeoposition() { Latitude = startPoint.lat, Longitude = startPoint.lon},
                    new BasicGeoposition() {Latitude = endPoint.lat, Longitude = endPoint.lon },
                });
                    //mapPolyline.StrokeDashed = strokeDashed;
                    mapPolyline.StrokeColor = color;
                    mapPolyline.StrokeThickness = 3;
                    Map.MapElements.Add(mapPolyline);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void UpdateMapElementOnPointerEntered(MapElement mapElement)
        {
            if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Disabled || mapElement.MapStyleSheetEntryState == "")
            {
                mapElement.MapStyleSheetEntryState = MapStyleSheetEntryStates.Hover;
            }
        }

        public void UpdateMapElementOnPointerExited(MapElement mapElement)
        {
            if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Hover)
            {
                mapElement.MapStyleSheetEntryState = "";
                mapElement.MapStyleSheetEntry = MapStyleSheetEntries.Forest;
            }
        }
        public void UpdateMapElementOnClick(MapElement mapElement)
        {
            MapIcon myclickedIcon = mapElement as MapIcon;
            if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Selected)
            {
                mapElement.MapStyleSheetEntryState = "";
                mapElement.MapStyleSheetEntry = MapStyleSheetEntries.Forest;

                int id = initData.rtpList.Where(item => item.AirportName == myclickedIcon.Title).FirstOrDefault().ID;

                ListViewWorker.RemoveElement(initData.rtpList, id);

            }
            else if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Hover)
            {
                mapElement.MapStyleSheetEntryState = MapStyleSheetEntryStates.Selected;

                RouteTurningPoint RTP = new RouteTurningPoint();
                RTP.AirportName = myclickedIcon.Title;
                RTP.Longitude = myclickedIcon.Location.Position.Longitude;
                RTP.Latitude = myclickedIcon.Location.Position.Latitude;
                RTP.Velocity = 850;
                RTP.Altitude = 1500;

                ListViewWorker.UpdateData(initData.rtpList, RTP);
            }
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
