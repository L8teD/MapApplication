using MapApplication.Model;
using MapApplication.View;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapApplication.ViewModel
{
    public class PlotPageVM : BaseViewModel, IPlotControl
    {
        public PlotVM plot { get; set; }
        public LegendVM legendControlVM { get; set; }

        public Dictionary<string, string[][]> cb_Data { get; set; }
        public List<LineSeries> IndicatedSeries { get; set; }
        public List<LineSeries> RemovedSeries { get; set; }
        DisplayGraphicData desiredPlotData;
        DisplayGraphicData actualPlotData;
        //DisplayGraphicData additionalPlotData;
        private List<LineSeries> savedSeries;

        string activePlotTitle;
        PlotName activeParameter = PlotName.Latitude;
        PlotCharacter activeCharacter = PlotCharacter.Ideal;
        Source activeSource = Source.INS;
        string activeVerticalAxis = "";
        List<OxyColor> colors = new List<OxyColor>()
        {
            OxyColors.Green,
            OxyColors.Red,
            OxyColors.Blue,
            OxyColors.Black,
            OxyColors.Orange,
            OxyColors.Purple,
            OxyColors.Pink
        };
        int colorIndex = 0;
        #region Commands
        private RelayCommand cmd_SavePlot;
        public RelayCommand SavePlot
        {
            get
            {
                return cmd_SavePlot ??
                (cmd_SavePlot = new RelayCommand(obj =>
                {
                    if (savedSeries == null)
                        savedSeries = new List<LineSeries>();
                    
                    foreach (LineSeries series in IndicatedSeries)
                    {
                        LineSeries newSeries = new LineSeries()
                        {
                            StrokeThickness = 2,
                            MarkerSize = 0,
                            LineStyle = LineStyle.Solid,
                            MarkerType = MarkerType.None,
                            Color = colors[colorIndex],
                            Title = activePlotTitle + " | " + series.Title
                        };
                        newSeries.Points.AddRange(series.Points);
                        colorIndex++;
                        
                        if (!savedSeries.Contains(newSeries))
                            savedSeries.Add(newSeries);

                    }
                    UpdateToolTipText();
                    //savedSeries.AddRange(IndicatedSeries);
                }));
            }
        }
        private RelayCommand cmd_OpenSavedPlot;
        public RelayCommand OpenSavedPlot
        {
            get
            {
                return cmd_OpenSavedPlot ??
                (cmd_OpenSavedPlot = new RelayCommand(obj =>
                {
                    if (savedSeries == null) return;

                    IndicatedSeries.Clear();
                    RemovedSeries.Clear();

                    IndicatedSeries.AddRange(savedSeries);

                    RefreshPlot(true);

                }));
            }
        }
        private RelayCommand cmd_ClearSavedCharts;
        public RelayCommand ClearSavedCharts
        {
            get
            {
                return cmd_ClearSavedCharts ??
                (cmd_ClearSavedCharts = new RelayCommand(obj =>
                {
                    if (savedSeries == null) return;

                    savedSeries.Clear();
                    UpdateToolTipText();
                    colorIndex = 0;
                }));
            }
        }
        private RelayCommand cmd_SaveToClipBoard;
        public RelayCommand SaveToClipBoard
        {
            get
            {
                return cmd_SaveToClipBoard ??
                (cmd_SaveToClipBoard = new RelayCommand(obj =>
                {
                    plot.SaveToClipBoard();
                }));
            }
        }
        private RelayCommand cmd_OpenPlotSettings;
        public RelayCommand OpenPlotSettings
        {
            get
            {
                return cmd_OpenPlotSettings ??
                (cmd_OpenPlotSettings = new RelayCommand(obj =>
                {
                    PlotParameters settingWindow = new PlotParameters();
                    settingWindow.DataContext = new PlotParametersVM(this);
                    settingWindow.ShowDialog();
                }));
            }
        }
        private RelayCommand cmd_Home;
        public RelayCommand Cmd_Home
        {
            get
            {
                return cmd_Home ??
                (cmd_Home = new RelayCommand(obj =>
                {
                    FillSeries(activeParameter, activeCharacter);
                    RefreshPlot();
                    plot.Home();

                }));
            }
        }
        private RelayCommand cmd_Difference;
        public RelayCommand DifferenceClick
        {
            get
            {
                return cmd_Difference ??
                (cmd_Difference = new RelayCommand(obj =>
                {
                    if (IndicatedSeries == null) return;

                    string yAxis = PlotWorker.SelectPlotDimension(activeParameter, activeCharacter);

                    if (IndicatedSeries.Count == 2 )
                    {
                        LineSeries diffSerie = new LineSeries()
                        {
                            StrokeThickness = 2,
                            MarkerSize = 0,
                            LineStyle = LineStyle.Solid,
                            MarkerType = MarkerType.None,
                            Color = OxyColors.Green,
                            Title = IndicatedSeries[0].Title + " - " + IndicatedSeries[1].Title
                        };
                        for (int i = 0; i < IndicatedSeries[0].Points.Count; i++)
                        {
                            diffSerie.Points.Add(new DataPoint(
                                i,
                                IndicatedSeries[1].Points[i].Y - IndicatedSeries[0].Points[i].Y));
                        }
                        IndicatedSeries.Clear();
                        IndicatedSeries.Add(diffSerie);
                    }
                    

                    Plot("time, [sec]", PlotWorker.SelectPlotName(activeParameter) + " " + yAxis);
                }));
            }
        }

        private RelayCommand cmd_ParamSelectionChanged;
        public RelayCommand ParamSelectionChanged
        {
            get
            {
                return cmd_ParamSelectionChanged ??
                (cmd_ParamSelectionChanged = new RelayCommand(obj =>
                {
                    string paramName = ((KeyValuePair<string, string[][]>)obj).Key;

                    if(paramName != null)
                    {
                        

                        activeParameter = PlotWorker.SelectPlotName(paramName);

                        FillSeries(activeParameter, activeCharacter);
                        RefreshPlot();
                    }
                    
                }));
            }
        }
        private RelayCommand cmd_CharacterSelectionChanged;
        public RelayCommand CharacterSelectionChanged
        {
            get
            {
                return cmd_CharacterSelectionChanged ??
                (cmd_CharacterSelectionChanged = new RelayCommand(obj =>
                {
                    string characterName = obj as string;

                    activeCharacter = PlotWorker.SelectPlotCharacter(characterName);

                    activeVerticalAxis = PlotWorker.SelectPlotDimension(activeParameter, activeCharacter);

                    FillSeries(activeParameter, activeCharacter);
                    RefreshPlot();
                }));
            }
        }
        private RelayCommand cmd_SourceSelectionChanged;
        public RelayCommand SourceSelectionChanged
        {
            get
            {
                return cmd_SourceSelectionChanged ??
                (cmd_SourceSelectionChanged = new RelayCommand(obj =>
                {
                    string sourceName = obj as string;

                    activeSource = PlotWorker.SelectSource(sourceName);

                    desiredPlotData.SwitchSource(activeSource);
                    actualPlotData.SwitchSource(activeSource);
                    //additionalPlotData.SwitchSource(activeSource);

                    FillSeries(activeParameter, activeCharacter);
                    RefreshPlot();
                }));
            }
        }

        #endregion

        public string SavedSeriesNames
        {
            get { return (string)GetValue(SavedSeriesNamesProperty); }
            set { SetValue(SavedSeriesNamesProperty, value); }
        }
        public static readonly DependencyProperty SavedSeriesNamesProperty =
          DependencyProperty.Register("SavedSeriesNames", typeof(string), typeof(PlotPageVM), new PropertyMetadata(default(string)));


        private void UpdateToolTipText()
        {
            
            if (savedSeries != null)
            {
                string text = "";
                for (int i = 0; i < savedSeries.Count; i++)
                {
                    text += savedSeries[i].Title;

                    if (i != (savedSeries.Count - 1))
                        text += "\n";
                }
                syncContext.Send(SendValueMessage, text);
            }
        }
        private void SendValueMessage(object text)
        {
            SavedSeriesNames = (string)text;
        }

        public PlotPageVM(MainModel m_Model)
        {
            plot = new PlotVM("Latitude");
            legendControlVM = new LegendVM(this, plot);
            SetComboBoxData();

            m_Model.SetPlotData += M_Model_SetPlotData;

            IndicatedSeries = new List<LineSeries>();
            RemovedSeries = new List<LineSeries>();
        }
        public void RefreshPlot(bool isMerge = false)
        {
            if (isMerge)
                activePlotTitle = "Merging Charts";
            else
            {
                activePlotTitle = PlotWorker.SelectPlotName(activeParameter)
                                            + " " + PlotWorker.SelectSource(activeSource)
                                            + " " + PlotWorker.SelectPlotCharacter(activeCharacter);
            }
           
            activeVerticalAxis = PlotWorker.SelectPlotDimension(activeParameter, activeCharacter);
            Plot("time, [sec]", PlotWorker.SelectPlotName(activeParameter) + " " + activeVerticalAxis);
            plot.ChangePlotTitle(activePlotTitle);
        }
        private void SetComboBoxData()
        {
            cb_Data = new Dictionary<string, string[][]>();

            string[] paramNames = new string[]
            {
                "Latitude",
                "Longitude",
                "Altitude",
                "East Velocity",
                "North Velocity",
                "Vertical Velocity",
                "Heading",
                "Roll",
                "Pitch"
            };
            string[] sources = new string[]
            {
                "BINS",
                "GNSS",
                "SVS"
            };
            string[] angleSources = new string[]
            {
                "BINS"
            };
            string[] fullCharacter = new string[]
            {
                "Ideal",
                "Error",
                "Estimate",
                "Error - Estimate",
                "Ideal + Error",
                "P"
            };
            string[] angleCharacter = new string[]
            {
                "Ideal",
                "Error",
                "Ideal + Error"
            };
            cb_Data.Add(paramNames[0], new string[][] { sources, fullCharacter });
            cb_Data.Add(paramNames[1], new string[][] { sources, fullCharacter });
            cb_Data.Add(paramNames[2], new string[][] { sources, fullCharacter });
            cb_Data.Add(paramNames[3], new string[][] { sources, fullCharacter });
            cb_Data.Add(paramNames[4], new string[][] { sources, fullCharacter });
            cb_Data.Add(paramNames[5], new string[][] { sources, fullCharacter });
            cb_Data.Add(paramNames[6], new string[][] { angleSources, angleCharacter });
            cb_Data.Add(paramNames[7], new string[][] { angleSources, angleCharacter });
            cb_Data.Add(paramNames[8], new string[][] { angleSources, angleCharacter });
        }
        private void M_Model_SetPlotData(DisplayGraphicData arg1, DisplayGraphicData arg2/*, DisplayGraphicData arg3*/)
        {
            M_Model_SetDesiredData(arg1);
            M_Model_SetActualData(arg2);
            //M_Model_SetAdditionalData(arg3);
        }
        private void FillSeries(PlotName name, PlotCharacter character)
        {
            IndicatedSeries.Clear();
            RemovedSeries.Clear();

            if (desiredPlotData != null)
            {
                IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                    PlotWorker.SelectData(name, character, desiredPlotData.Display),
                    "Desired Track"));
            }

            if(actualPlotData != null)
            {
                IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                    PlotWorker.SelectData(name, character, actualPlotData.Display),
                    "Actual Track"));
            }
            //if (additionalPlotData != null)
            //{
            //    IndicatedSeries.Add(PlotWorker.CreateLineSeries(
            //       PlotWorker.SelectData(name, character, additionalPlotData.Display),
            //       "Additional Track"));
            //}

        }

        private void M_Model_SetDesiredData(DisplayGraphicData obj)
        {
            desiredPlotData = obj;
        }
        private void M_Model_SetActualData(DisplayGraphicData obj)
        {
            actualPlotData = obj;
        }
        private void M_Model_SetAdditionalData(DisplayGraphicData obj)
        {
            //additionalPlotData = obj;
        }

        public void Plot(string xAxisName, string yAxisName)
        {
            plot.Plot(xAxisName, yAxisName, IndicatedSeries);

            legendControlVM.ClearLegendsVis();
            for (int i = 0; i < IndicatedSeries.Count; i++)
            {
                if (IndicatedSeries[i] != null)
                {
                    
                    LineSeries series = IndicatedSeries[i];

                    SolidColorBrush legendElColor = new SolidColorBrush(Color.FromArgb(series.Color.A, series.Color.R, series.Color.G, series.Color.B));
                    string legendElText = series.Title;
                    legendControlVM.UpdateLegendElement(legendControlVM.legendBtns[i], legendElColor, legendElText);

                }
                else
                    legendControlVM.UpdateLegendElement(legendControlVM.legendBtns[i]);
            }
        }

    }
}
