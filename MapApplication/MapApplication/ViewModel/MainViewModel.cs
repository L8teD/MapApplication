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
using System.Windows.Input;
using System.Diagnostics;
using System.IO;

namespace MapApplication.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private Microsoft.Toolkit.Wpf.UI.Controls.MapControl Map;
        public InitData initData { get; set; }
        public PlotTableVM PlotTable { get; set; }

        private MapElementsLayer airportsLayer;
        private MapElementsLayer elementsRunningAdded;
        private MainModel m_Model;

        private WayPoint waypoint;
        private Process process;

        //MapPolyline trajectoryLine;
        List<BasicGeoposition> trajectoryPoints = new List<BasicGeoposition>();


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
        public ObservableCollection<TableParameter> tableParameters { get; set; } = new ObservableCollection<TableParameter>();


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
        private RelayCommand cmd_ExecuteNdDisplay;
        public RelayCommand ExecuteNdDisplay
        {
            get
            {
                return cmd_ExecuteNdDisplay ??
                (cmd_ExecuteNdDisplay = new RelayCommand(obj =>
                {
                    string exePath = @"B:\Projects\GosNIIAS\Pages Model\studentsProject\CallingProgram\bin\Debug\";
                    string fileName = "CallingProgram.exe";
                    process = new Process();
                    process.StartInfo.FileName = Path.Combine(exePath, fileName);
                    process.StartInfo.WorkingDirectory = new FileInfo(exePath).DirectoryName;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    process.Start();
                }));
            }
        }
        private RelayCommand cmd_KillNdDisplay;
        public RelayCommand KillNdDisplay
        {
            get
            {
                return cmd_KillNdDisplay ??
                (cmd_KillNdDisplay = new RelayCommand(obj =>
                {
                    
                    if (!process.HasExited)
                        process.Kill();
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

            m_Model.SetTableData += M_Model_SetTableData;


            initData = m_Model.SetInputErrors(initData);
            loggerInfoList = m_Model.GetInfoFromLogger();

           
            MapElementWorker.AddAirportsOnMap();
            airportsLayer = new MapElementsLayer() { MapElements = MapElementWorker.airportsMapElements };
            //airportsLayer.MapElementPointerEntered += AirportsLayer_MapElementPointerEntered;
            //airportsLayer.MapElementPointerExited += AirportsLayer_MapElementPointerExited;
            //airportsLayer.MapElementClick += AirportsLayer_MapElementClick;

            AddMapElementEvents(airportsLayer);

            elementsRunningAdded = new MapElementsLayer() { MapElements = MapElementWorker.doubleClickElements };

            Map.Layers.Add(airportsLayer);

            map.MapRightTapped += Map_MapRightTapped;
        }

        private void M_Model_SetTableData(TrackData obj)
        {
            if (tableParameters == null)
                tableParameters = new ObservableCollection<TableParameter>();
            for (int i = 0; i < obj.INS.points.Count; i++)
            {
                TableParameter param = new TableParameter();

                param.Latitude = obj.INS.points[i].Ideal.Value.Degrees.lat;
                param.Longitude = obj.INS.points[i].Ideal.Value.Degrees.lon;
                param.Altitude = obj.INS.points[i].Ideal.Value.Degrees.alt;

                param.EastVelocity = obj.INS.velocities[i].Ideal.Value.E;
                param.NorthVelocity = obj.INS.velocities[i].Ideal.Value.N;
                param.VerticalVelocity = obj.INS.velocities[i].Ideal.Value.H;

                param.Heading = obj.INS.angles[i].Ideal.Value.Degrees.heading;
                param.Pitch = obj.INS.angles[i].Ideal.Value.Degrees.pitch;
                param.Roll = obj.INS.angles[i].Ideal.Value.Degrees.roll;

                tableParameters.Add(param);
            }


        }

        private void Map_MapRightTapped(object sender, Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.MapRightTappedEventArgs e)
        {
            if (MapElementWorker.doubleClickElements == null) MapElementWorker.doubleClickElements = new List<MapElement>();
            MapElementWorker.AddElement(MapElementWorker.doubleClickElements, e.Location.Position.Latitude, e.Location.Position.Longitude);

            //Map.Layers.Clear();
            if (Map.Layers.Contains(elementsRunningAdded))
                Map.Layers.Remove(elementsRunningAdded);

            elementsRunningAdded = new MapElementsLayer() { MapElements = MapElementWorker.doubleClickElements };
            AddMapElementEvents(elementsRunningAdded);
            Map.Layers.Add(elementsRunningAdded);
        }

        private void AddMapElementEvents(MapElementsLayer elementsLayer)
        {
            elementsLayer.MapElementPointerEntered += AirportsLayer_MapElementPointerEntered;
            elementsLayer.MapElementPointerExited += AirportsLayer_MapElementPointerExited;
            elementsLayer.MapElementClick += AirportsLayer_MapElementClick;
        }
       
        private async void SetMapView(Microsoft.Toolkit.Wpf.UI.Controls.MapControl Map)
        {
            Geopoint maiLocation = new Geopoint(new BasicGeoposition() { Latitude = 62.9/*55.811685*/, Longitude = 91.2/*37.502471*/ });
            await Map.TrySetViewAsync(maiLocation, 10);
        }
        #region MapElement Events
        private void AirportsLayer_MapElementClick(MapElementsLayer sender, MapElementsLayerClickEventArgs args)
        {
            if(sender.Equals(airportsLayer))
                UpdateMapElementOnClick(args.MapElements[0]);
            else
            {
                UpdateMapElementOnClick(args.MapElements[0]);
            }
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

                int id = initData.wayPointList.Where(item => 
                item.Latitude == myclickedIcon.Location.Position.Latitude &&
                item.Longitude == myclickedIcon.Location.Position.Longitude).FirstOrDefault().ID;

                ListViewWorker.RemoveElement(initData.wayPointList, id);

            }
            else if (mapElement.MapStyleSheetEntryState == MapStyleSheetEntryStates.Hover)
            {
                mapElement.MapStyleSheetEntryState = MapStyleSheetEntryStates.Selected;

                WayPoint wayPoint = new WayPoint();
                wayPoint.AirportName = myclickedIcon.Title;
                wayPoint.Longitude = myclickedIcon.Location.Position.Longitude;
                wayPoint.Latitude = myclickedIcon.Location.Position.Latitude;
                wayPoint.Velocity = 100;
                wayPoint.Altitude = 100;

                ListViewWorker.UpdateData(initData.wayPointList, wayPoint);
            }
        }

        #endregion
        
        private async void DrawLine(List<BasicGeoposition> trajectoryPoints, int trChar)
        {
            try
            { 

                await Dispatcher.InvokeAsync(new Action(delegate ()
                {
                    //Map.MapElements.Clear();
                    MapPolyline trajectoryLine = new MapPolyline();
                    trajectoryLine.StrokeThickness = 3; 
                    if (trajectoryLine == null)
                    {
                        trajectoryLine = new MapPolyline();
                        trajectoryLine.StrokeColor = Windows.UI.Colors.Blue;
                        trajectoryLine.StrokeThickness = 2;
                    }
                    if (trChar == 1)
                        trajectoryLine.StrokeColor = Windows.UI.Colors.Red;
                    else
                        trajectoryLine.StrokeColor = Windows.UI.Colors.Blue;
                    List<BasicGeoposition> geopositions = new List<BasicGeoposition>();
                    geopositions.AddRange(trajectoryPoints);
                    trajectoryLine.Path = new Geopath(geopositions);
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
