using CommonLib;
using CommonLib.Matrix;
using CommonLib.Params;
using ModellingErrorsLib;
using ModellingTrajectoryLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;
using static ModellingErrorsLib.Types;
using static ModellingTrajectoryLib.Types;

namespace ModellingTrajectoryLib
{
    class ModellingFunctions
    {
        double Rz = 6371000;
        double g = 9.81;

        double[] dLon;
        double[] ortDist;
        double[] ortDistAngle;
        double[] heading;
        double[] distance;
        double[] pitch;
        double[] dH;
        double[] roll;

        double rollTarget = Converter.DegToRad(-20);
        double UR;
        double radiusTurn;
        double timeTurn;
        double LUR_Distance;
        double[] velAbs;
        int timeTurnInt;
        int numberOfIterations;
        double dHeading;
        double dVelocityOnFullTurn;
        double dVelocityOnEveryIteration;


        int CountOfWindCall = 0;
        Point[] startedPoints;

        bool turnHappened = false;

        internal void InitStartedData(double[] latArray, double[] lonArray, double[] altArray, double[] velocity)
        {
            InitStartedCoords(latArray, lonArray, altArray);
            InitStartedVelocitites(velocity);
        }
        private void InitStartedCoords(double[] latArray, double[] lonArray, double[] altArray)
        {
            startedPoints = new Point[latArray.Length];
            for (int i = 0; i < latArray.Length; i++)
            {
                startedPoints[i] = new Point(latArray[i], lonArray[i], altArray[i]);
            }
        }
        private void InitStartedVelocitites(double[] velocity)
        {
            velAbs = new double[velocity.Length - 1];
            for (int i = 1; i < velocity.Length; i++)
            {
                if (velocity[i] >= 1224)
                    velAbs[i - 1] = 1224 / 3.6;
                else if (velocity[i] <= 500)
                    velAbs[i - 1] = 500 / 3.6;
                else
                    velAbs[i - 1] = velocity[i] / 3.6;
            }
        }
        private double[] MakeArray(int length)
        {
            return new double[length - 1];
        }
        internal double ComputeOrtDistAngle(Parameters parameters, int k)
        {
            return ComputeOrtDistAngle(parameters.point, startedPoints[k + 1]);
        }
        private double ComputeOrtDistAngle(Point lastPoint, Point nextPoint)
        {
            return Math.Acos(Math.Sin(lastPoint.lat) * Math.Sin(nextPoint.lat) +
                Math.Cos(lastPoint.lat) * Math.Cos(nextPoint.lat) * Math.Cos(nextPoint.lon - lastPoint.lon));
        }
        private double ComputeHeading(Point lastPoint, Point nextPoint, double dLon)
        {
            return Math.Atan2(Math.Cos(nextPoint.lat) * Math.Sin(dLon),
                 Math.Cos(lastPoint.lat) * Math.Sin(nextPoint.lat) - Math.Sin(lastPoint.lat) * Math.Cos(nextPoint.lat) * Math.Cos(dLon));
        }
        internal void InitParamsBetweenPPM()
        {
            dH = MakeArray(startedPoints.Length);
            for (int i = 0; i < dH.Length; i++)
            {
                dH[i] = startedPoints[i+1].alt - startedPoints[i].alt;
            }
            dLon = MakeArray(startedPoints.Length);
            ortDist = MakeArray(startedPoints.Length);
            ortDistAngle = MakeArray(startedPoints.Length);
            heading = MakeArray(startedPoints.Length);
            distance = MakeArray(startedPoints.Length);
            pitch = MakeArray(startedPoints.Length);
            roll = MakeArray(startedPoints.Length);
            //roll = MakeArray(startedPoints.Length);
            for (int k = 0; k < startedPoints.Length - 1; k++)
            {
                ComputeParamsBetweenPPM(k);
            }
        }
        internal void CheckParamsBetweenPPM(int k)
        {
            if (turnHappened)
            {
                ComputeParamsBetweenPPM(k);
                turnHappened = false;
            }
        }
        private void ComputeParamsBetweenPPM(int k)
        {
            dLon[k] = startedPoints[k + 1].lon - startedPoints[k].lon;
            ortDistAngle[k] = ComputeOrtDistAngle(startedPoints[k], startedPoints[k + 1]);
            ortDist[k] = Rz * ortDistAngle[k];
            distance[k] = Math.Sqrt(Math.Pow(ortDist[k], 2)) + dH[k];
            pitch[k] = Math.Atan2(dH[k], ortDist[k]);
            roll[k] = 0;
            heading[k] = ComputeHeading(startedPoints[k], startedPoints[k + 1], dLon[k]);
            heading[k] += heading[k] <= 0 ? 2 * Math.PI : 0;
            heading[k] -= heading[k] >= Converter.DegToRad(360) ? 2 * Math.PI : 0;
        }
        internal double GetLUR(int k, int limit)
        {
            if (k != limit)
                ComputeLUR(k);
            else
                LUR_Distance = -1;
            return LUR_Distance;
        }
        private void ComputeLUR(int k)
        {
            UR = heading[k + 1] - heading[k];
            UR -= UR >= Converter.DegToRad(180) ? Converter.DegToRad(360) : 0;
            UR += UR <= Converter.DegToRad(-180) ? Converter.DegToRad(360) : 0;

            rollTarget = UR >= 0 ? Math.Abs(rollTarget) : rollTarget;

            radiusTurn = Math.Pow(velAbs[k], 2) / (g * Math.Tan(rollTarget));
            timeTurn = radiusTurn * UR / velAbs[k];
            LUR_Distance = radiusTurn * Math.Tan(0.5 * UR);
        }
        private double RecountHeading(double velocityRoute, double velocityAbs, Wind wind, double heading)
        {
            double a1 = Math.Pow(velocityRoute, 2) + Math.Pow(velocityAbs, 2) - Math.Pow(wind.speed, 2);
            double a2 = (2.0 * velocityAbs * velocityRoute);
            double a = Math.Acos(a1 / a2);
            return a + heading;
        }
        private double RecountVelocity(double velocityAbs, Wind wind, double heading)
        {
            return Math.Sqrt(Math.Pow(velocityAbs, 2) + Math.Pow(wind.speed, 2) - 2.0 * velocityAbs * wind.speed * Math.Cos(180 - heading - wind.angle));
        }
        internal double GetPPM(int k)
        {
            return Rz * ortDistAngle[k];
        }
        internal double GetPPM(double ortDistAngle)
        {
            return Rz * ortDistAngle;
        }
        internal void RecountWind(Parameters parameters, int k)
        {
            if (CountOfWindCall % 100 == 0)
            {
                Wind wind = Weather.Query(parameters.point);
                velAbs[k] = RecountVelocity(velAbs[k], wind, heading[k]);
                //heading[k] = RecountHeading(velAbs[k], velAbs[k], wind, heading[k]);
            }
            CountOfWindCall++;
        }
        internal double[][] CreateMatrixC(Parameters parameters)
        {
            return Create.MatrixC(parameters.angles.heading, parameters.angles.pitch, parameters.angles.roll);
        }
        internal void SetAngles(ref Parameters parameters, int k)
        {
            Angles angles = new Angles();
            angles.heading = heading[k];
            angles.pitch = pitch[k];
            angles.roll = roll[k];
            parameters.angles = angles;
        }
        internal void SetVelocity(ref Parameters parameters, int k)
        {
            parameters.velocity = new Velocity(velAbs[k], parameters);
        }
        internal bool TurnIsAvailable(int k, int length)
        {
            return (k != length) && (timeTurn >= 0.51);
        }
        internal void InitTurnVariables(int k, double dt)
        {
            timeTurnInt = (int)Math.Round(timeTurn);
            numberOfIterations = (int)(timeTurnInt / dt);
            dHeading = UR / numberOfIterations;


            turnHappened = true;

            dVelocityOnFullTurn = velAbs[k + 1] - velAbs[k];
            dVelocityOnEveryIteration = dVelocityOnFullTurn != 0 ? dVelocityOnFullTurn / numberOfIterations : 0;
        }
        internal bool TurnIsNotEnded(double j)
        {
            return j <= timeTurnInt;
        }
        internal void SetTurnAngles(int k, double dt)
        {
            roll[k] = rollTarget;
            double velocityValue = velAbs[k] + dVelocityOnEveryIteration;
            double distTurn = velocityValue * dt;
            pitch[k] = Math.Atan2(0, distTurn);
            heading[k] += dHeading;
        }

    }
}
