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
        List<PlotData> desiredPlotData;
        List<PlotData> actualPlotData;

        PlotName currentPlotName = PlotName.Longitude;
        PlotCharacter currentCharacter = PlotCharacter.Ideal;

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

                        currentPlotName = PlotWorker.SelectPlotName(paramName);

                        string yAxis = PlotWorker.SelectPlotDimension(currentPlotName, currentCharacter);

                        FillSeries(currentPlotName, currentCharacter);
                        Plot("time, [sec]", paramName + " " + yAxis);
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

                    currentCharacter = PlotWorker.SelectPlotCharacter(characterName);

                    string yAxis = PlotWorker.SelectPlotDimension(currentPlotName, currentCharacter);

                    FillSeries(currentPlotName, currentCharacter);
                    Plot("time, [sec]", PlotWorker.SelectPlotName(currentPlotName) + " " + yAxis);


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

                    //currentCharacter = PlotWorker.SelectPlotCharacter(characterName);

                    //string yAxis = PlotWorker.SelectPlotDimension(currentPlotName, currentCharacter);

                    //FillSeries(currentPlotName, currentCharacter);
                    //Plot("time, [sec]", yAxis);


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
            string yAxis = PlotWorker.SelectPlotDimension(currentPlotName, currentCharacter);

            FillSeries(currentPlotName, currentCharacter);
            Plot("time, [sec]", yAxis);
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
        private void M_Model_SetPlotData(List<PlotData> arg1, List<PlotData> arg2)
        {
            M_Model_SetDesiredData(arg1);
            M_Model_SetActualData(arg2);
        }
        private void FillSeries(PlotName name, PlotCharacter character)
        {
            IndicatedSeries.Clear();
            RemovedSeries.Clear();

            IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                PlotWorker.SelectData(name, character, desiredPlotData), 
                "Desired Track"));

            IndicatedSeries.Add(PlotWorker.CreateLineSeries(
                PlotWorker.SelectData(name, character, actualPlotData), 
                "Actual Track"));
        }

        private void M_Model_SetDesiredData(List<PlotData> obj)
        {
            desiredPlotData = obj;
        }
        private void M_Model_SetActualData(List<PlotData> obj)
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
