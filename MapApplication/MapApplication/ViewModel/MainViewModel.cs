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
using CommonLib;
using OxyPlot;

namespace MapApplication.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private Microsoft.Toolkit.Wpf.UI.Controls.MapControl Map;
        public InitData initData { get; set; }
        public PlotTableVM PlotTable { get; set; }

        private MapElementsLayer airportsLayer;
        private MainModel m_Model;

        private WayPoint waypoint;
        MapPolyline trajectoryLine;
        List<BasicGeoposition> trajectoryPoints = new List<BasicGeoposition>();

        public DataTableWithChangesVM IdealDataTable { get; set; }
        public DataTableVM ErrorDataTable { get; set; }
        public DataTableVM EstimateDataTable { get; set; }

        public PlotPageVM plotPageVM{ get; set; }

        public WayPoint Waypoint
        {
            get { return waypoint; }
            set
            {
                waypoint = value;
                OnPropertyChanged("Waypoint");
            }
        }
        public ObservableCollection<LogInfo> loggerInfoList { get; set; }

        #region Commands
        
        private RelayCommand cmd_AddWayPoint;
        public RelayCommand Cmd_AddWayPoint
        {
            get
            {
                return cmd_AddWayPoint ??
                (cmd_AddWayPoint = new RelayCommand(obj =>
                {
                    //if (obj is ObservableCollection<RouteTurningPoint>)
                    m_Model.AddWayPoint(initData.wayPointList, Waypoint);
                }));
            }
        }
        private RelayCommand cmd_RemoveWayPoint;
        public RelayCommand Cmd_RemoveWayPoint
        {
            get
            {
                return cmd_RemoveWayPoint ??
                (cmd_RemoveWayPoint = new RelayCommand(obj =>
                {
                    Button button = obj as Button;
                    
                    MapElement removedElement = airportsLayer.MapElements.Where(item => (item as MapIcon).Title ==
                                        initData.wayPointList.Where(X => X.ID == (int)button.Tag).FirstOrDefault().AirportName).FirstOrDefault();

                    if(removedElement != null)
                        UpdateMapElementOnClick(removedElement);
                    else
                        m_Model.RemoveWayPoint(initData.wayPointList, (int)button.Tag);
                }));
            }
        }
        private RelayCommand cmd_LoggerSelectionChanged;
        public RelayCommand Cmd_LoggerSelectionChanged
        {
            get
            {
                return cmd_LoggerSelectionChanged ??
                (cmd_LoggerSelectionChanged = new RelayCommand(obj =>
                {

                    m_Model.SetDataFromLogger((LogInfo)obj, initData.wayPointList);

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
                    m_Model.Simulate();

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
        private RelayCommand cmd_Channel2;
        public RelayCommand Cmd_Channel2
        {
            get
            {
                return cmd_Channel2 ??
                (cmd_Channel2 = new RelayCommand(obj =>
                {
                    m_Model.SwitchIndicatedData(DataSource.twoChannel);
                    m_Model.SwitchPlotData(DataSource.twoChannel);

                }));
            }
        }
        private RelayCommand cmd_Channel3;
        public RelayCommand Cmd_Channel3
        {
            get
            {
                return cmd_Channel3 ??
                (cmd_Channel3 = new RelayCommand(obj =>
                {
                    m_Model.SwitchIndicatedData(DataSource.threeChannel);
                    m_Model.SwitchPlotData(DataSource.threeChannel);

                }));
            }
        }
        private RelayCommand cmd_FeedbackChannel2;
        public RelayCommand Cmd_FeedbackChannel2
        {
            get
            {
                return cmd_FeedbackChannel2 ??
                (cmd_FeedbackChannel2 = new RelayCommand(obj =>
                {
                    m_Model.SwitchIndicatedData(DataSource.twoChannelFeedback);
                    m_Model.SwitchPlotData(DataSource.twoChannelFeedback);

                }));
            }
        }
        private RelayCommand cmd_FeedbackChannel3;
        public RelayCommand Cmd_FeedbackChannel3
        {
            get
            {
                return cmd_FeedbackChannel3 ??
                (cmd_FeedbackChannel3 = new RelayCommand(obj =>
                {
                    m_Model.SwitchIndicatedData(DataSource.threeChannelFeedback);
                    m_Model.SwitchPlotData(DataSource.threeChannelFeedback);

                }));
            }
        }
        private RelayCommand cmd_drawFull;
        public RelayCommand Cmd_drawFull
        {
            get
            {
                return cmd_drawFull ??
                (cmd_drawFull = new RelayCommand(obj =>
                {
                    m_Model.Stop();
                    m_Model.DrawFullTrajctory();

                }));
            }
        }
        #endregion

        public MainViewModel(Microsoft.Toolkit.Wpf.UI.Controls.MapControl map)
        {
            Map = map;
            m_Model = new MainModel();

            initData = new InitData();
            PlotTable = new PlotTableVM(m_Model);
            initData.wayPointList = new ObservableCollection<WayPoint>();
            m_Model.SetTemplateRoute(initData.wayPointList);

            plotPageVM = new PlotPageVM(m_Model);
           
            SetMapView(Map);

            Waypoint = m_Model.SetWayPoint();

            m_Model.DrawTrajectoryAction += DrawLine;

            m_Model.UpdateTableData += M_Model_UpdateTableData;


            initData = m_Model.SetInputErrors(initData);
            loggerInfoList = m_Model.GetInfoFromLogger();

            IdealDataTable = new DataTableWithChangesVM(PlotCharacter.Ideal);
            ErrorDataTable = new DataTableVM(PlotCharacter.Error);
            EstimateDataTable = new DataTableVM(PlotCharacter.Estimate);

            MapElementWorker.AddAirportsOnMap();
            airportsLayer = new MapElementsLayer() { MapElements = MapElementWorker.airportsMapElements };
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

                int id = initData.wayPointList.Where(item => item.AirportName == myclickedIcon.Title).FirstOrDefault().ID;

                ListViewWorker.RemoveElement(initData.wayPointList, id);

            }
            else if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Hover)
            {
                mapElement.MapStyleSheetEntryState = MapStyleSheetEntryStates.Selected;

                WayPoint RTP = new WayPoint();
                RTP.AirportName = myclickedIcon.Title;
                RTP.Longitude = myclickedIcon.Location.Position.Longitude;
                RTP.Latitude = myclickedIcon.Location.Position.Latitude;
                RTP.Velocity = 130;
                RTP.Altitude = 1000;

                ListViewWorker.UpdateData(initData.wayPointList, RTP);
            }
        }

        #endregion

        private async void DrawLine(List<BasicGeoposition> trajectoryPoints, int trChar)
        {
            try
            {
                await Dispatcher.InvokeAsync(new Action(delegate ()
                {
                    Map.MapElements.Clear();
                    if (trajectoryLine == null)
                    {
                        trajectoryLine = new MapPolyline();
                        trajectoryLine.StrokeColor = Windows.UI.Colors.Red;
                        trajectoryLine.StrokeThickness = 2;
                    }
                    if (trChar == 1)
                        trajectoryLine.StrokeColor = Windows.UI.Colors.Red;
                    else
                        trajectoryLine.StrokeColor = Windows.UI.Colors.Black;
                    trajectoryLine.Path = new Geopath(trajectoryPoints);
                    Map.MapElements.Add(trajectoryLine);
                    
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
