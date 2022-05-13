using MapApplication.Model;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        PlotName activeParameter = PlotName.Latitude;
        PlotCharacter activeCharacter = PlotCharacter.Ideal;
        Source activeSource = Source.INS;
        string activeVerticalAxis = "";

        #region Commands
        private RelayCommand cmd_Home;
        public RelayCommand Cmd_Home
        {
            get
            {
                return cmd_Home ??
                (cmd_Home = new RelayCommand(obj =>
                {
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
                   
                    if (IndicatedSeries.Count > 1)
                    {
                        LineSeries diffSerie = new LineSeries()
                        {
                            StrokeThickness = 2,
                            MarkerSize = 0,
                            LineStyle = LineStyle.Solid,
                            MarkerType = MarkerType.None,
                            Color = OxyColors.Green
                        };
                        for (int i = 0; i < IndicatedSeries[0].Points.Count; i++)
                        {
                            diffSerie.Points.Add(new DataPoint(
                                i,
                                IndicatedSeries[0].Points[i].Y - IndicatedSeries[1].Points[i].Y));
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
                        plot.ChangePlotTitle(paramName);

                        activeParameter = PlotWorker.SelectPlotName(paramName);

                        activeVerticalAxis = PlotWorker.SelectPlotDimension(activeParameter, activeCharacter);

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

                    RefreshPlot();
                }));
            }
        }

        #endregion

        public PlotPageVM(MainModel m_Model)
        {
            plot = new PlotVM("Latitude");
            legendControlVM = new LegendVM(this, plot);
            SetComboBoxData();

            m_Model.SetPlotData += M_Model_SetPlotData;

            IndicatedSeries = new List<LineSeries>();
            RemovedSeries = new List<LineSeries>();
        }
        public void RefreshPlot()
        {
            activeVerticalAxis = PlotWorker.SelectPlotDimension(activeParameter, activeCharacter);

            FillSeries(activeParameter, activeCharacter);
            Plot("time, [sec]", PlotWorker.SelectPlotName(activeParameter) + " " + activeVerticalAxis);
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
        private void M_Model_SetPlotData(DisplayGraphicData arg1, DisplayGraphicData arg2)
        {
            M_Model_SetDesiredData(arg1);
            M_Model_SetActualData(arg2);
        }
        private void FillSeries(PlotName name, PlotCharacter character)
        {
            IndicatedSeries.Clear();
            RemovedSeries.Clear();

            IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                PlotWorker.SelectData(name, character, desiredPlotData.Display), 
                "Desired Track"));

            IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                PlotWorker.SelectData(name, character, actualPlotData.Display), 
                "Actual Track"));
        }

        private void M_Model_SetDesiredData(DisplayGraphicData obj)
        {
            desiredPlotData = obj;
        }
        private void M_Model_SetActualData(DisplayGraphicData obj)
        {
            actualPlotData = obj;
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
