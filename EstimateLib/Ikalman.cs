using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using MyMatrix;

namespace EstimateLib
{
    public interface IKalman
    {
        Vector X { get; set; }
        Vector X_estimate { get; set; }
        Matrix P { get; set; }
        //void InitX(InitErrors initErrors, Point point, AbsoluteOmega absOmega, EarthModel earthModel, Velocity velocity, Angles angles, bool Init = false);
        //void InitF(OmegaGyro omegaGyro, AbsoluteOmega absOmega, EarthModel earthModel, Acceleration acceleration, Matrix C);
        //void InitG(Matrix C);
        //void InitW(InitErrors initErrors);
        //void InitH(Point point, EarthModel earth, AbsoluteOmega absOmega, Velocity velocity);
        //void InitZ(Point point, Velocity velocity, EarthModel earth);
        //void InitAnglesError(InitErrors initErrors);
        //void Kalman();
        void Model(InitErrors initErrors, Parameters parameters, Matrix C, double dt);
    }

}
