using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModellingErrorsLib3;
using EstimateLib;
using CommonLib.Params;

namespace ModellingTrajectoryLib
{
    public class Modelling
    {
        public T_OutputFull Output = new T_OutputFull();
        private IKalman ActualFeedbackKalman;
        private IKalman ActualKalman;
        private IKalman AdditionalKalman;
        private IKalman DesiredFeedbackKalman;
        private IKalman DesiredKalman;

        int wayPointsCount;
        BaseTrajectory desiredTrack;
        BaseTrajectory actualTrack;
        //BaseTrajectory additionalTrack;

        Input input;

        Randomize randomize;
        ModellingFunctions functions;

        MeasurementsErrors gnssMeasurements;
        MeasurementsErrors svsMeasurements;
        int correctorCounter;
        public void GetOutputs(ref T_OutputFull output)
        {
            Model();
            output = Output;
        }

        public void Init(Input _input)
        {
            desiredTrack = new DesiredTrack();
            actualTrack = new ActualTrack();
            //additionalTrack = new DesiredTrack();
            input = _input;
            input.trajectory.latitude = Converter.DegToRad(input.trajectory.latitude);
            input.trajectory.longitude = Converter.DegToRad(input.trajectory.longitude);

            wayPointsCount = input.trajectory.latitude.Length;

            CreateOutputData(ref Output.Default);
            CreateOutputData(ref Output.Feedback);

            randomize = new Randomize();
            randomize.Init();
            functions = new ModellingFunctions();

            DesiredFeedbackKalman = new KalmanFeedbackModel();
            DesiredKalman = new KalmanModel();

            ActualFeedbackKalman = new KalmanFeedbackModel();
            ActualKalman = new KalmanModel();

            AdditionalKalman = new KalmanModel();

            InputAirData zeroInputAirData = input.air;
            InputWindData zeroInputWindData = input.wind;
            InsErrors zeroInsError = input.INS;
            SetZeroInputs(ref zeroInputAirData, ref zeroInputWindData, ref zeroInsError);
            desiredTrack.Init(input, zeroInputAirData, zeroInputWindData, zeroInsError);
            actualTrack.Init(input, input.air, input.wind, input.INS);
            //additionalTrack.Init(input, input.air, input.wind, input.INS);

            functions.InitStartedData(input.trajectory.latitude, input.trajectory.longitude, input.trajectory.altitude, input.trajectory.velocity);
            functions.InitParamsBetweenPPM();

            desiredTrack.FillOutputsData += FillDesiredTrackOutput;
            actualTrack.FillOutputsData += FillActualTrackOutput;
            //additionalTrack.FillOutputsData += FillAdditionalTrackOutput;

            correctorCounter = 0;
        }
        public void Model()
        {
            for (int wpNumber = 0; wpNumber < wayPointsCount - 1; wpNumber++)
            {
                double LUR_Distance = functions.GetLUR(wpNumber, wayPointsCount - 2);
                double PPM_Distance = functions.GetPPM(wpNumber);
                double PPM_DisctancePrev;
                while (LUR_Distance < PPM_Distance)
                {
                    DrydenInput drydenInput = new DrydenInput();
                    drydenInput.rand1 = randomize.GetRandom();
                    drydenInput.rand2 = randomize.GetRandom();
                    drydenInput.rand3 = randomize.GetRandom();

                    SetMeasurements();
                    desiredTrack.Track(wpNumber, functions, drydenInput, ref svsMeasurements);
                    actualTrack.Track(wpNumber, functions, drydenInput, ref svsMeasurements);
                    //additionalTrack.Track(wpNumber, functions, drydenInput, ref svsMeasurements);
                    Kalman();

                    PPM_DisctancePrev = PPM_Distance;
                    double ortDistAngleCurrent = functions.ComputeOrtDistAngle(desiredTrack.OutPoints.Ideal.GetValueOrDefault().Radians, wpNumber);
                    PPM_Distance = functions.GetPPM(ortDistAngleCurrent);

                    //if (counterAddPlotDataDes % 1 == 0)
                    //{
                    //    functions.CheckParamsBetweenPPM(wpNumber, desiredTrack.OutPoints.Ideal.GetValueOrDefault().Radians,
                    //    new Velocity(desiredTrack.OutVelocities.Ideal.GetValueOrDefault().E,
                    //                desiredTrack.OutVelocities.Ideal.GetValueOrDefault().N,
                    //                desiredTrack.OutVelocities.Ideal.GetValueOrDefault().H).module);
                    //}
                        

                    //LUR_Distance = functions.GetLUR(wpNumber, wayPointsCount - 2);
                    if (PPM_DisctancePrev <= PPM_Distance)
                        break;

                }
                if (functions.TurnIsAvailable(wpNumber, wayPointsCount - 2))
                {
                    functions.InitTurnVariables(wpNumber, input.INS.dt);
                    for (double j = 0; functions.TurnIsNotEnded(j); j += input.INS.dt)
                    {
                        DrydenInput drydenInput = new DrydenInput();
                        drydenInput.rand1 = randomize.GetRandom();
                        drydenInput.rand2 = randomize.GetRandom();
                        drydenInput.rand3 = randomize.GetRandom();
                        SetMeasurements();
                        functions.SetTurnAngles(wpNumber, input.INS.dt, desiredTrack.OutPoints.Ideal.GetValueOrDefault().Radians.alt);
                        desiredTrack.Track(wpNumber, functions, drydenInput, ref svsMeasurements);
                        actualTrack.Track(wpNumber, functions, drydenInput, ref svsMeasurements);
                        //additionalTrack.Track(wpNumber, functions, drydenInput, ref svsMeasurements);
                        Kalman();
                    }
                }
            }
        }
        private void SetMeasurements()
        {
            gnssMeasurements = new MeasurementsErrors();
            svsMeasurements = new MeasurementsErrors();

            gnssMeasurements.constant.lon = 0;
            gnssMeasurements.constant.lat = 0;
            gnssMeasurements.constant.alt = 0;
            gnssMeasurements.constant.E = 0;
            gnssMeasurements.constant.N = 0;
            gnssMeasurements.constant.H = 0;

            gnssMeasurements.SKO.lat = input.GNSS.coord;
            gnssMeasurements.SKO.lon = input.GNSS.coord;
            gnssMeasurements.SKO.alt = input.GNSS.coord;
            gnssMeasurements.SKO.E = input.GNSS.velocity;
            gnssMeasurements.SKO.N = input.GNSS.velocity;
            gnssMeasurements.SKO.H = input.GNSS.velocity;


            gnssMeasurements.noise.lon = randomize.GetRandom();
            gnssMeasurements.noise.lat = randomize.GetRandom();
            gnssMeasurements.noise.alt = randomize.GetRandom();
            gnssMeasurements.noise.E = randomize.GetRandom();
            gnssMeasurements.noise.N = randomize.GetRandom();
            gnssMeasurements.noise.H = randomize.GetRandom();


            svsMeasurements.SKO.lat = input.air.coordSKO;
            svsMeasurements.SKO.lon = input.air.coordSKO;
            svsMeasurements.SKO.alt = input.air.coordSKO;
            svsMeasurements.SKO.E = input.air.velSKO;
            svsMeasurements.SKO.N = input.air.velSKO;
            svsMeasurements.SKO.H = input.air.velSKO;

            svsMeasurements.noise.lat = randomize.GetRandom();
            svsMeasurements.noise.lon = randomize.GetRandom();
            svsMeasurements.noise.alt = randomize.GetRandom();
            svsMeasurements.noise.E = randomize.GetRandom();
            svsMeasurements.noise.N = randomize.GetRandom();
            svsMeasurements.noise.H = randomize.GetRandom();

        }
        private void SetZeroInputs(ref InputAirData airZero, ref InputWindData windZero, ref InsErrors insZero)
        {
            airZero.coordSKO = 2.5;
            airZero.velSKO = 0.3;
            airZero.pressureError = 5;
            airZero.tempratureError = 0.2;
            airZero.pressureIndicatorError = 0.1;
            airZero.isCompensation = 1;

            windZero.L_u = 200;
            windZero.L_v = 200;
            windZero.L_w = 50;
            windZero.sigma_u = 1.06;
            windZero.sigma_v = 1.06;
            windZero.sigma_w = 0.7;

            insZero.accelerationError.first = 0.02 * insZero.dt;
            insZero.accelerationError.second = 0.02 * insZero.dt;
            insZero.accelerationError.third = 0.02 * insZero.dt;

            insZero.accNoiseSKO.first = 0.005 * insZero.dt;
            insZero.accNoiseSKO.second = 0.005 * insZero.dt;
            insZero.accNoiseSKO.third = 0.005 * insZero.dt;

            insZero.gyroError.first = Converter.DegToRad(1) / 3600 * insZero.dt;
            insZero.gyroError.second = Converter.DegToRad(1) / 3600 * insZero.dt;
            insZero.gyroError.third = Converter.DegToRad(1) / 3600 * insZero.dt;

            insZero.gyroNoiseSKO.first = Converter.DegToRad(0.25) / 3600 * insZero.dt;
            insZero.gyroNoiseSKO.second = Converter.DegToRad(0.25) / 3600 * insZero.dt;
            insZero.gyroNoiseSKO.third = Converter.DegToRad(0.25) / 3600 * insZero.dt;

            insZero.accTemperatureKoef.first = 0.008;
            insZero.accTemperatureKoef.second = 0.008;
            insZero.accTemperatureKoef.third = 0.008;

            insZero.gyroTemperatureKoef.first = 0.03;
            insZero.gyroTemperatureKoef.second = 0.03;
            insZero.gyroTemperatureKoef.third = 0.03;
        }
        
        private void Kalman()
        {
            input.INS.accNoiseValue.first = randomize.GetRandom();
            input.INS.accNoiseValue.second = randomize.GetRandom();
            input.INS.accNoiseValue.third = randomize.GetRandom();
            input.INS.gyroNoiseValue.first = randomize.GetRandom();
            input.INS.gyroNoiseValue.second = randomize.GetRandom();
            input.INS.gyroNoiseValue.third = randomize.GetRandom();


            bool isGardenRoad = false;
            if (desiredTrack.kalmanModel != null)
            {
                isGardenRoad = correctorCounter > 500 && correctorCounter < 1000 || correctorCounter > 1600 && correctorCounter < 2400;
                //isGardenRoad = Common.IsGardenRingRoad(desiredTrack.OutPoints.Ideal.GetValueOrDefault().Degrees.lat,
                //                                                desiredTrack.OutPoints.Ideal.GetValueOrDefault().Degrees.lon);
            }

            desiredTrack.Estimation(DesiredKalman, correctorCounter % 2 == 0, gnssMeasurements, svsMeasurements, input.INS);
            //desiredTrack.Estimation(DesiredFeedbackKalman, !isGardenRoad, gnssMeasurements, svsMeasurements, input.INS);
            actualTrack.Estimation(ActualKalman, correctorCounter % 2 == 0, gnssMeasurements, svsMeasurements, input.INS);
            //additionalTrack.Estimation(AdditionalKalman, !isGardenRoad, gnssMeasurements, svsMeasurements, input.INS);
            //actualTrack.Estimation(ActualFeedbackKalman, correctorCounter % 2 == 0, gnssMeasurements, svsMeasurements, input.INS);
            correctorCounter++;
        }
        private int counterAddPlotDataDes = 0;
        private int counterAddPlotDataAct = 0;
        private int counterAddPlotDataAdd = 0;
        public void FillDesiredTrackOutput(IKalman kalman)
        {
            if (counterAddPlotDataDes % (1.0 / input.INS.dt) == 0)
            {
                if (kalman is KalmanFeedbackModel)
                    FillOutputData(Output.Feedback.DesiredTrack, desiredTrack);
                else if (kalman is KalmanModel)
                    FillOutputData(Output.Default.DesiredTrack, desiredTrack);
            }
            if (kalman is KalmanModel)
                counterAddPlotDataDes++;

        }

        public void FillActualTrackOutput(IKalman kalman)
        {
            if (counterAddPlotDataAct % (1.0 / input.INS.dt) == 0)
            {
                if (kalman is KalmanFeedbackModel)
                    FillOutputData(Output.Feedback.ActualTrack, actualTrack);
                else if (kalman is KalmanModel)
                    FillOutputData(Output.Default.ActualTrack, actualTrack);
            }
            if (kalman is KalmanModel)
                counterAddPlotDataAct++;
        }
        public void FillAdditionalTrackOutput(IKalman kalman)
        {
            //if (counterAddPlotDataAdd % (1.0 / input.INS.dt) == 0)
            //{
            //    if (kalman is KalmanFeedbackModel)
            //        FillOutputData(Output.Feedback.AdditionalTrack, additionalTrack);
            //    else if (kalman is KalmanModel)
            //        FillOutputData(Output.Default.AdditionalTrack, additionalTrack);
            //}
            //if (kalman is KalmanModel)
            //    counterAddPlotDataAdd++;
        }

        private void FillOutputData(TrackData trackData, ITrajectory trajectory)
        {
            trackData.INS.points.Add(trajectory.OutPoints);
            trackData.INS.velocities.Add(trajectory.OutVelocities);
            trackData.INS.angles.Add(trajectory.OutAngles);
            trackData.INS.p_OutList.Add(trajectory.OutCovar);

            trackData.GNSS.points.Add(trajectory.GnssPoints);
            trackData.GNSS.velocities.Add(trajectory.GnssVelocities);

            trackData.KVS.points.Add(trajectory.KvsPoints);


            trackData.KVS.velocities.Add(trajectory.KvsVelocities);
            
        }
        private void CreateOutputData(ref OutputData data)
        {
            data = new OutputData();
            data.points = new List<PointSet>();
            data.velocities = new List<VelocitySet>();
            data.angles = new List<AnglesSet>();
            data.p_OutList = new List<P_out>();
        }
        private void CreateOutputData(ref TrackData data)
        {
            data = new TrackData();
            CreateOutputData(ref data.INS);
            CreateOutputData(ref data.GNSS);
            CreateOutputData(ref data.KVS);
        }
        private void CreateOutputData(ref T_Output data)
        {
            data = new T_Output();
            CreateOutputData(ref data.DesiredTrack);
            CreateOutputData(ref data.ActualTrack);
            //CreateOutputData(ref data.AdditionalTrack);
        }

    }
}
