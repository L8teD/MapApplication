﻿

namespace ModellingErrorsLib3
{
    #region Input Struct
    public struct InitErrors
    {
        public AngleAccuracy angleAccuracy;
        public CoordAccuracy coordAccuracy;
        public VelocityAccuracy velocityAccuracy;
        public GyroError gyroError;
        public AccError accelerationError;
        public double accNoise;
        public double gyroNoise;
        public double snsNoise;
        public double sateliteErrorCoord { get; set; }
        public double sateliteErrorVelocity { get; set; }

        public double dt; //вынести отсюда
    }
    public struct AngleAccuracy
    {
        public double heading { get; set; }
        public double pitch { get; set; }
        public double roll { get; set; }
    }
    public struct CoordAccuracy
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double altitude { get; set; }
    }
    public struct VelocityAccuracy
    {
        public double east { get; set; }
        public double north { get; set; }
        public double H { get; set; }
    }
    public struct GyroError
    {
        public double first { get; set; }
        public double second { get; set; }
        public double third { get; set; }
    }
    public struct AccError
    {
        public double first { get; set; }
        public double second { get; set; }
        public double third { get; set; }
    }
    #endregion

}
