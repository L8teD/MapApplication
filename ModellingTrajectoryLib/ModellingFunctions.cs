using CommonLib;
using CommonLib.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModellingTrajectoryLib;
using MyMatrix;

namespace ModellingTrajectoryLib
{
    public class ModellingFunctions
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
                startedPoints[i] = new Point(
                    latArray[i],
                    lonArray[i],
                    altArray[i],
                    Dimension.Radians);
            }
        }
        private void InitStartedVelocitites(double[] velocity)
        {
            velAbs = new double[velocity.Length - 1];
            for (int i = 1; i < velocity.Length; i++)
            {
                if (velocity[i - 1] >= 222)
                    velocity[i - 1] = 222;
                else if (velocity[i] <= 50)
                    velocity[i - 1] = 50;
                velAbs[i - 1] = Converter.KmPerHourToMeterPerSec(velocity[i - 1]);
            }
        }
        private double[] MakeArray(int length)
        {
            return new double[length - 1];
        }
        internal double ComputeOrtDistAngle(Point point, int wpNumber)
        {
            return ComputeOrtDistAngle(point, startedPoints[wpNumber + 1]);
        }
        private double ComputeOrtDistAngle(Point currPoint, Point nextPoint)
        {
            return Math.Acos(Math.Sin(currPoint.lat) * Math.Sin(nextPoint.lat) +
                Math.Cos(currPoint.lat) * Math.Cos(nextPoint.lat) * Math.Cos(nextPoint.lon - currPoint.lon));
        }
        private double ComputeHeading(Point lastPoint, Point nextPoint, double dLon)
        {
            return Math.Atan2(Math.Cos(nextPoint.lat) * Math.Sin(dLon),
                 Math.Cos(lastPoint.lat) * Math.Sin(nextPoint.lat) - Math.Sin(lastPoint.lat) * Math.Cos(nextPoint.lat) * Math.Cos(dLon));
        }
        internal void InitParamsBetweenPPM()
        {
            dH = MakeArray(startedPoints.Length);

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
                ComputeParamsBetweenPPM(k, startedPoints[k]);
            }
        }
        internal void CheckParamsBetweenPPM(int k, Point lastPoint, double velocityValue)
        {
            ComputeParamsBetweenPPM(k, lastPoint, velocityValue);
            turnHappened = false;
            //if (turnHappened)
            //{
            //    ComputeParamsBetweenPPM(k);
            //    turnHappened = false;
            //}
        }
        private void ComputeParamsBetweenPPM(int wpNumber, Point lastPoint, double velocityValue = 0)
        {
            if (velocityValue != 0)
                velAbs[wpNumber] = velocityValue;
            dH[wpNumber] = startedPoints[wpNumber + 1].alt - lastPoint.alt;
            dLon[wpNumber] = startedPoints[wpNumber + 1].lon - lastPoint.lon;
            ortDistAngle[wpNumber] = ComputeOrtDistAngle(lastPoint, startedPoints[wpNumber + 1]);
            ortDist[wpNumber] = Rz * ortDistAngle[wpNumber];
            distance[wpNumber] = Math.Sqrt(Math.Pow(ortDist[wpNumber], 2)) + dH[wpNumber];
            pitch[wpNumber] = Math.Atan2(dH[wpNumber], ortDist[wpNumber]);
            roll[wpNumber] = 0;
            heading[wpNumber] = ComputeHeading(lastPoint, startedPoints[wpNumber + 1], dLon[wpNumber]);
            //heading[wpNumber] += heading[wpNumber] <= 0 ? 2 * Math.PI : 0;
            //heading[wpNumber] -= heading[wpNumber] >= Converter.DegToRad(360) ? 2 * Math.PI : 0;
        }
        internal double GetLUR(int wpNumber, int limit)
        {
            if (wpNumber != limit)
                ComputeLUR(wpNumber);
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

        internal double GetPPM(int k)
        {
            return Rz * ortDistAngle[k];
        }
        internal double GetPPM(double ortDistAngle)
        {
            return Rz * ortDistAngle;
        }
        
        internal Matrix CreateMatrixC(Parameters parameters)
        {
            return Create.MatrixC(parameters.angles);
        }
        internal void SetAngles(ref Parameters parameters, int k)
        {
            Angles angles = new Angles();
            angles.heading = heading[k];
            angles.pitch = pitch[k];
            angles.roll = roll[k];
            angles.dimension = Dimension.Radians;
            parameters.angles = angles;
        }
        internal void SetVelocity(ref Parameters parameters, int k, double dt)
        {
            parameters.velocity = new Velocity(velAbs[k], parameters.angles, dt);
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

            //dRollOnTurn = (rollTarget / numberOfIterations) * 2;

            dVelocityOnFullTurn = velAbs[k + 1] - velAbs[k];
            dVelocityOnEveryIteration = dVelocityOnFullTurn != 0 ? dVelocityOnFullTurn / numberOfIterations : 0;
        }
        internal bool TurnIsNotEnded(double j)
        {
            return j <= timeTurnInt;
        }
        internal void SetTurnAngles(int k, double dt, double altitude)
        {
            roll[k] = rollTarget;
            double velocityValue = velAbs[k] + dVelocityOnEveryIteration;
            double distTurn = velocityValue * dt;
            pitch[k] = Math.Atan2((startedPoints[k+1].alt - altitude) /numberOfIterations, distTurn);
            heading[k] += dHeading;
        }

    }
}
