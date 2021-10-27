﻿using MapApplicationWPF.Helper;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using static CommonLib.Types;
using static MapApplicationWPF.Helper.Types;
using static ModellingTrajectoryLib.Types;

namespace MapApplicationWPF.ExternalResourses
{

    public class PlotFinalData
    {
        public List<string> plotTitles;
        public Dictionary<string, List<LineSeries>> lineSeriesData;
        public Dictionary<string, string[]> axisNames;
        public static OutputData outputData { private get; set; }
        public void CreatePlotData()
        {
            #region Create List
            List<DataPoint> trajectoryPointsList = new List<DataPoint>();
            List<DataPoint> trajectoryPointsErrorList = new List<DataPoint>();
            List<DataPoint> trajectoryPointsWithErrorList = new List<DataPoint>();

            List<DataPoint> latitudeList = new List<DataPoint>();
            List<DataPoint> longitudeList = new List<DataPoint>();
            List<DataPoint> latitudeErrorList = new List<DataPoint>();

            List<DataPoint> longitudeErrorList = new List<DataPoint>();
            List<DataPoint> latitudeWithErrorsList = new List<DataPoint>();
            List<DataPoint> longitudeWithErrorsList = new List<DataPoint>();

            List<DataPoint> velocityEastList = new List<DataPoint>();
            List<DataPoint> velocityNorthList = new List<DataPoint>();
            List<DataPoint> velocityEastErrorList = new List<DataPoint>();

            List<DataPoint> velocityNorthErrorList = new List<DataPoint>();
            List<DataPoint> velocityEastWithErrorsList = new List<DataPoint>();
            List<DataPoint> velocityNorthWithErrorsList = new List<DataPoint>();
            #endregion

            #region Create DataPoint List
            CreateDataPointList(outputData.Points, ref trajectoryPointsList, ref trajectoryPointsErrorList, 
                ref trajectoryPointsWithErrorList, ref latitudeList, ref longitudeList, ref latitudeErrorList,
                ref longitudeErrorList, ref latitudeWithErrorsList, ref longitudeWithErrorsList);
            CreateDataPointList(outputData.Velocities, ref velocityEastList,
            ref velocityNorthList, ref velocityEastErrorList,
            ref velocityNorthErrorList,
            ref velocityEastWithErrorsList, ref velocityNorthWithErrorsList);
            #endregion
            #region Create Line Series
            LineSeries trajectoryPointsSeries = CreateLineSeries(trajectoryPointsList, false);
            LineSeries trajectoryPointsErrorSeries = CreateLineSeries(trajectoryPointsErrorList);
            LineSeries trajectoryPointsWithErrorSeries = CreateLineSeries(trajectoryPointsWithErrorList);

            LineSeries latitudeSeries = CreateLineSeries(latitudeList, false);
            LineSeries longitudeSeries = CreateLineSeries(longitudeList, false);

            LineSeries latitudeErrorSeries = CreateLineSeries(latitudeErrorList);
            LineSeries longitudeErrorSeries = CreateLineSeries(longitudeErrorList);

            LineSeries latitudeWithErrorsSeries = CreateLineSeries(latitudeWithErrorsList);
            LineSeries longitudeWithErrorsSeries = CreateLineSeries(longitudeWithErrorsList);
           
            LineSeries velocityEastSeries = CreateLineSeries(velocityEastList, false);
            LineSeries velocityNorthSeries = CreateLineSeries(velocityNorthList, false);

            LineSeries velocityEastErrorSeries = CreateLineSeries(velocityEastErrorList);
            LineSeries velocityNorthErrorSeries = CreateLineSeries(velocityNorthErrorList);

            LineSeries velocityEastWithErrorsSeries = CreateLineSeries(velocityEastWithErrorsList);
            LineSeries velocityNorthWithErrorsSeries = CreateLineSeries(velocityNorthWithErrorsList);
            #endregion

            #region Create Data Dict
            lineSeriesData = new Dictionary<string, List<LineSeries>>();
            axisNames = new Dictionary<string, string[]>();
            plotTitles = new List<string>()
            {
                "Траектория движения",
                "Ошибка на траектории",
                "Траектория движения с ошибкой",
                "Изменение долготы",
                "Изменение ошибки по долготе",
                "Изменение долготы с ошибкой",
                "Изменение широты",
                "Изменение ошибки по широте",
                "Изменение широты с ошибкой",
                "Изменение восточной скорости",
                "Ошибка по восточной скорости",
                "Изменение восточной скорости с ошибкой",
                "Изменение северной скорости",
                "Ошибка по северной скорости",
                "Изменение северной скорости с ошибкой"
            };
            lineSeriesData.Add("Траектория движения", new List<LineSeries> { trajectoryPointsSeries });
            axisNames.Add("Траектория движения", new string[] { "Долгота, [град]", "Широта, [град]" });
            lineSeriesData.Add("Ошибка на траектории", new List<LineSeries> { trajectoryPointsErrorSeries});
            axisNames.Add("Ошибка на траектории", new string[] { "Долгота, [град]", "Широта, [град]" });
            lineSeriesData.Add("Траектория движения с ошибкой", new List<LineSeries>
            {
                trajectoryPointsSeries,
                trajectoryPointsWithErrorSeries
            });
            axisNames.Add("Траектория движения с ошибкой", new string[] { "Долгота, [град]", "Широта, [град]" });

            lineSeriesData.Add("Изменение долготы", new List<LineSeries> { longitudeSeries });
            axisNames.Add("Изменение долготы", new string[] { "Время, [с]", "Долгота, [град]" });
            lineSeriesData.Add("Изменение ошибки по долготе", new List<LineSeries> { longitudeErrorSeries });
            axisNames.Add("Изменение ошибки по долготе", new string[] { "Время, [с]", "Долгота, [град]" });
            lineSeriesData.Add("Изменение долготы с ошибкой", new List<LineSeries>
            {
                longitudeSeries,
                longitudeWithErrorsSeries
            });
            axisNames.Add("Изменение долготы с ошибкой", new string[] { "Время, [с]", "Долгота, [град]" });

            lineSeriesData.Add("Изменение широты", new List<LineSeries> { latitudeSeries });
            axisNames.Add("Изменение широты", new string[] { "Время, [с]", "Широта, [град]" });
            lineSeriesData.Add("Изменение ошибки по широте", new List<LineSeries> { latitudeErrorSeries });
            axisNames.Add("Изменение ошибки по широте", new string[] { "Время, [с]", "Широта, [град]" });
            lineSeriesData.Add("Изменение широты с ошибкой", new List<LineSeries>
            {
                latitudeSeries,
                latitudeWithErrorsSeries
            });
            axisNames.Add("Изменение широты с ошибкой", new string[] { "Время, [с]", "Широта, [град]" });

            lineSeriesData.Add("Изменение восточной скорости", new List<LineSeries> { velocityEastSeries });
            axisNames.Add("Изменение восточной скорости", new string[] { "Время, [с]", "Восточная скорость, [м/с]" });
            lineSeriesData.Add("Ошибка по восточной скорости", new List<LineSeries> { velocityEastErrorSeries});
            axisNames.Add("Ошибка по восточной скорости", new string[] { "Время, [с]", "Восточная скорость, [м/с]" });
            lineSeriesData.Add("Изменение восточной скорости с ошибкой", new List<LineSeries>
            {
                velocityEastSeries,
                velocityEastWithErrorsSeries
            });
            axisNames.Add("Изменение восточной скорости с ошибкой", new string[] { "Время, [с]", "Восточная скорость, [м/с]" });

            lineSeriesData.Add("Изменение северной скорости", new List<LineSeries> { velocityNorthSeries });
            axisNames.Add("Изменение северной скорости", new string[] { "Время, [с]", "Северная скорость, [м/с]" });
            lineSeriesData.Add("Ошибка по северной скорости", new List<LineSeries> { velocityNorthErrorSeries});
            axisNames.Add("Ошибка по северной скорости", new string[] { "Время, [с]", "Северная скорость, [м/с]" });
            lineSeriesData.Add("Изменение северной скорости с ошибкой", new List<LineSeries>
            {
                velocityNorthSeries,
                velocityNorthWithErrorsSeries
            });
            axisNames.Add("Изменение северной скорости с ошибкой", new string[] { "Время, [с]", "Северная скорость, [м/с]" });


            #endregion
        }
        public static LineSeries CreateLineSeries(List<DataPoint> data, bool isBlue = true)
        {
            LineSeries series = new LineSeries()
            {
                //ItemsSource = data,
                DataFieldX = "x",
                DataFieldY = "Y",
                StrokeThickness = 2,
                MarkerSize = 0,
                LineStyle = LineStyle.Solid,
                Color = !isBlue ? OxyColors.Blue : OxyColors.Red,
                MarkerType = MarkerType.None,
            };
            foreach(DataPoint point in data)
            {
                series.Points.Add(point);
            }
            return series;
        }
        private static void CreateDataPointList(List<PointSet> inputData, ref List<DataPoint> trajectoryPointsList, 
            ref List<DataPoint> trajectoryPointsErrorsList, ref List<DataPoint> trajectoryPointsWithErrorsList,
            ref List<DataPoint> latitudeList, ref List<DataPoint> longitudeList, ref List<DataPoint> latitudeErrorList,
            ref List<DataPoint> longitudeErrorList, ref List<DataPoint> latitudeWithErrorList, ref List<DataPoint> longitudeWithErrorList)
        {
            int i = 0;
            foreach (PointSet pointSet in inputData)
            {
                trajectoryPointsList.Add(new DataPoint(pointSet.InDegrees.lat, pointSet.InDegrees.lon));
                trajectoryPointsErrorsList.Add(new DataPoint(pointSet.ErrorInDegrees.lat, pointSet.ErrorInDegrees.lon));
                trajectoryPointsWithErrorsList.Add(new DataPoint(pointSet.InDegreesWithError.lat, pointSet.InDegreesWithError.lon));
                latitudeList.Add(new DataPoint(i, pointSet.InMeters.lat));
                longitudeList.Add(new DataPoint(i, pointSet.InMeters.lon));
                latitudeErrorList.Add(new DataPoint(i, pointSet.ErrorInMeters.lat));
                longitudeErrorList.Add(new DataPoint(i, pointSet.ErrorInMeters.lon));
                latitudeWithErrorList.Add(new DataPoint(i, pointSet.InMetersWithError.lat));
                longitudeWithErrorList.Add(new DataPoint(i, pointSet.InMetersWithError.lon));
                i++;
            }
        }
        private static void CreateDataPointList(List<VelocitySet> inputData, ref List<DataPoint> velocityEastList, 
            ref List<DataPoint> velocityNorthList, ref List<DataPoint> velocityEastErrorsList,
            ref List<DataPoint> velocityNortherrorsList,
            ref List<DataPoint> velocityEastWithErrorsList, ref List<DataPoint> velocityNorthWithErrorsList)
        {
            int i = 0;
            foreach (VelocitySet velocitySet in inputData)
            {
                velocityEastList.Add(new DataPoint(i, velocitySet.Value.E));
                velocityNorthList.Add(new DataPoint(i, velocitySet.Value.N));
                velocityEastErrorsList.Add(new DataPoint(i, velocitySet.Error.E));
                velocityNortherrorsList.Add(new DataPoint(i, velocitySet.Error.N));
                velocityEastWithErrorsList.Add(new DataPoint(i, velocitySet.ValueWithError.E));
                velocityNorthWithErrorsList.Add(new DataPoint(i, velocitySet.ValueWithError.N));

                i++;
            }
        }
        
    }
    
}
