using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using MapApplicationWPF.Graphic;
using MapApplicationWPF.Helper;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;


using static MapApplicationWPF.Helper.Types;
using MapApplicationWPF.ExternalResourses;
using System.Windows.Media;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using static CommonLib.Types;
using CommonLib.Params;
using Point = CommonLib.Params.Point;

namespace MapApplicationWPF
{
    public delegate void InitHandler(ref InitData _initData, ObservableCollection<InputError> inputErrors);


    public partial class MainWindow : Window
    {
        public event InitHandler SaveInitDataHandler;
        public System.Threading.SynchronizationContext m_SyncContext;
        public event Action<string> CurrentTimeMessage;
        public event Action<string> FlightTimeMessage;
        public MainWindow()
        {
            InitializeComponent();
            CurrentTimeMessage += MainWindow_CurrentTimeMessage;
            FlightTimeMessage += MainWindow_FlightTimeMessage;
        }

        //RelayCommand plotOpen = new RelayCommand()
        MapElementsLayer airportsLayer;
        InitData initData;
        OutputData outputData;
        public ObservableCollection<InputError> inputErrorsList;


        Timer timerTrajectory;
        Timer timerRefreshStartedTimes;
        int second = 1;
        DateTime startedTime;
        private async void myMap_Loaded(object sender, RoutedEventArgs e)
        {

            dt_Ideal.m_SyncContext = System.Threading.SynchronizationContext.Current;
            dt_Error.m_SyncContext = System.Threading.SynchronizationContext.Current;
            m_SyncContext = System.Threading.SynchronizationContext.Current;
            initData = new InitData
            {
                ppmList = new ObservableCollection<MapApplicationWPF.ExternalResourses.PPM>(),
                initErrors = new ModellingErrorsLib3.Types.InitErrors()
            };
            outputData = new OutputData()
            {
                Points = new List<PointSet>(),
                Velocities = new List<VelocitySet>()
            };

            Geopoint maiLocation = new Geopoint(new BasicGeoposition() { Latitude = 55.811685, Longitude = 37.502471 });

            await (sender as Microsoft.Toolkit.Wpf.UI.Controls.MapControl).TrySetViewAsync(maiLocation, 6);

            (sender as Microsoft.Toolkit.Wpf.UI.Controls.MapControl).MapProjection = Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.MapProjection.Globe;
            //myMap.Style = Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.MapStyle.Road;
            Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.Geopoint p = new Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT.Geopoint(new BasicGeoposition() { Latitude = 47.604, Longitude = -122.329 });
            myMap.Center = p;
            MapElementWorker.AddAirportsOnMAp();
            airportsLayer = new MapElementsLayer() { MapElements = MapElementWorker.airportsMapElements };
            airportsLayer.MapElementPointerEntered += AirportsLayer_MapElementPointerEntered;
            airportsLayer.MapElementPointerExited += AirportsLayer_MapElementPointerExited;
            airportsLayer.MapElementClick += AirportsLayer_MapElementClick;
            myMap.Layers.Add(airportsLayer);


            lv_Routes.ItemsSource = initData.ppmList;
            inputErrorsList = SetInputErrors();
            lv_InputErrors.ItemsSource = inputErrorsList;

            timerTrajectory = new Timer(1000);
            timerTrajectory.Elapsed += TimerTrajectory_Elapsed;

            timerRefreshStartedTimes = new Timer(1000);
            timerRefreshStartedTimes.Elapsed += TimerRefreshStartedTimes_Elapsed;
            timerRefreshStartedTimes.Start();

            PlotWorker.CreateListOfPlotData();
        }


        #region MapElement Events
        private void AirportsLayer_MapElementClick(MapElementsLayer sender, MapElementsLayerClickEventArgs args)
        {
            MapElementWorker.UpdateMapElementOnClick(args.MapElements[0], initData);
        }

        private void AirportsLayer_MapElementPointerExited(MapElementsLayer sender, MapElementsLayerPointerExitedEventArgs args)
        {
            MapElementWorker.UpdateMapElementOnPointerExited(args.MapElement);
        }

        private void AirportsLayer_MapElementPointerEntered(MapElementsLayer sender, MapElementsLayerPointerEnteredEventArgs args)
        {
            MapElementWorker.UpdateMapElementOnPointerEntered(args.MapElement);
        }
        #endregion
        private void TimerRefreshStartedTimes_Elapsed(object sender, ElapsedEventArgs e)
        {
            //ListViewWorker.RefreshTimes(initData);
        }
        private void TimerTrajectory_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (second < outputData.Points.Count)
            {
                DrawLine(outputData.Points[second - 1].InDegrees, outputData.Points[second].InDegrees, Windows.UI.Colors.Blue);
                Operations.IncreaseValue(ref second);
                if (second < outputData.Points.Count)
                {
                    DrawLine(outputData.Points[second - 1].InDegreesWithError, outputData.Points[second].InDegreesWithError, Windows.UI.Colors.Red, true);
                }
                dt_Ideal.UpdateDisplayedData(outputData.FullDisplayedData.DisplayedDatasIdeal[second - 1]);
                dt_Error.UpdateDisplayedData(outputData.FullDisplayedData.DisplayedDatasError[second - 1]);

                System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ru-RU");

                CurrentTimeMessage.Invoke(Operations.AccelerateTime(startedTime, second));
                FlightTimeMessage.Invoke(second.ToString() + "sec");
            }
            else
                timerTrajectory.Enabled = false;
        }
        private ObservableCollection<InputError> SetInputErrors()
        {
            ObservableCollection<InputError> items = new ObservableCollection<InputError>();
            items.Add(new InputError() { Name = "α", Value = 0.25, Dimension = "[deg/h]" });
            items.Add(new InputError() { Name = "β", Value = 0.03, Dimension = "[deg/h]" });
            items.Add(new InputError() { Name = "γ", Value = 0.03, Dimension = "[deg/h]" });

            items.Add(new InputError() { Name = "Δλ", Value = 15, Dimension = "[m]" });
            items.Add(new InputError() { Name = "Δφ", Value = 15, Dimension = "[m]" });
            items.Add(new InputError() { Name = "ΔH", Value = 15, Dimension = "[m]" });

            items.Add(new InputError() { Name = "ΔVe", Value = 0.5, Dimension = "[m/s]" });
            items.Add(new InputError() { Name = "ΔVn", Value = 0.5, Dimension = "[m/s]" });
            items.Add(new InputError() { Name = "ΔVh", Value = 0.5, Dimension = "[m/s]" });

            items.Add(new InputError() { Name = "Δn1", Value = 6E-06, Dimension = "[g]" });
            items.Add(new InputError() { Name = "Δn2", Value = 6E-06, Dimension = "[g]" });
            items.Add(new InputError() { Name = "Δn3", Value = 6E-06, Dimension = "[g]" });

            items.Add(new InputError() { Name = "ΔΩ1", Value = 0.001, Dimension = "[deg/h]" });
            items.Add(new InputError() { Name = "ΔΩ2", Value = 0.001, Dimension = "[deg/h]" });
            items.Add(new InputError() { Name = "ΔΩ3", Value = 0.001, Dimension = "[deg/h]" });

            items.Add(new InputError() { Name = "ΔXc", Value = 10, Dimension = "[m]" });
            items.Add(new InputError() { Name = "ΔVc", Value = 0.1, Dimension = "[m/s]" });

            SaveInitDataHandler += ListViewWorker.SaveInitDataHandler;

            return items;
        }
        private async void DrawLine(Point startPoint, Point endPoint, Windows.UI.Color color, bool strokeDashed = false)
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
                    mapPolyline.StrokeDashed = strokeDashed;
                    mapPolyline.StrokeColor = color;
                    mapPolyline.StrokeThickness = 3;
                    myMap.MapElements.Add(mapPolyline);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void btn_ComputeTrajectory_Click(object sender, RoutedEventArgs e)
        {
            timerRefreshStartedTimes.Stop();

            SaveInitDataHandler.Invoke(ref initData, inputErrorsList);

            Execute.CreateTrajectory(initData, ref outputData);
            Operations.ChangeEnableStates(btn_Start, btn_Pause, btn_Stop, btn_X10, btn_X100, btn_X1000);

            tElem_Current.Visibility = Visibility.Visible;
            tElem_Flight.Visibility = Visibility.Visible;

            btn_ShowPlottings.IsEnabled = true;
            btn_SaveData.IsEnabled = true;

            for (int i = 0; i < outputData.Points.Count; i++)
                PlotWorker.AddPlotDataToStruct(outputData.FullDisplayedData, i);
        }
        private void btn_X10_Click(object sender, RoutedEventArgs e)
        {
            timerTrajectory.Interval = 100;
        }

        private void btn_X100_Click(object sender, RoutedEventArgs e)
        {
            timerTrajectory.Interval = 10;
        }

        private void btn_X1000_Click(object sender, RoutedEventArgs e)
        {
            timerTrajectory.Interval = 1;
        }

        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            timerTrajectory.Enabled = false;
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            timerTrajectory.Enabled = false;
            second = 1;
            Operations.ChangeEnableStates(btn_Start, btn_Pause, btn_Stop, btn_X10, btn_X100, btn_X1000);

        }

        private void btn_DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)(sender as Button).Tag;
            MapElementWorker.choosenAirportsIcons[id - 1].MapStyleSheetEntryState = "";
            MapElementWorker.choosenAirportsIcons[id - 1].MapStyleSheetEntry = MapStyleSheetEntries.Forest;
            MapElementWorker.choosenAirportsIcons.RemoveAt(id - 1);
            ListViewWorker.UpdateData(initData);
        }
        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            timerTrajectory.Enabled = true;

            startedTime = DateTime.Now;

        }

        //private void btn_InputBinsErrors_Click(object sender, RoutedEventArgs e)
        //{
        //    //InputErrors inputErrors = new InputErrors();


        //    //inputErrors.Closed += InputErrors_Closed;
        //    //inputErrors.ShowDialog();
        //}

        private void btn_ShowPlottings_Click(object sender, RoutedEventArgs e)
        {
            PlotFinalData.outputData = outputData;
            PlotWindow plotWindow = new PlotWindow();
            plotWindow.ShowDialog();
        }
        private void MainWindow_CurrentTimeMessage(string obj)
        {
            m_SyncContext.Send(tElem_Current.SendTimeMessage, obj);
        }
        private void MainWindow_FlightTimeMessage(string obj)
        {
            m_SyncContext.Send(tElem_Flight.SendTimeMessage, obj);
        }

        private void btn_SaveData_Click(object sender, RoutedEventArgs e)
        {
            SaveWindow saveWindow = new SaveWindow(outputData);
            saveWindow.ShowDialog();
        }
        private RelayCommand cmd_SaveCSV;
        public RelayCommand Cmd_SaveCSV
        {
            get
            {
                return cmd_SaveCSV ??
                (cmd_SaveCSV = new RelayCommand(obj =>
                {
                    Saver.WriteCSV(outputData.FullDisplayedData.DisplayedDatasIdeal, "ideal.csv");
                }));
            }
        }
    }

}
