using CommonLib;
using CommonLib.Params;
using ModellingErrorsLib3;
using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimateLib
{
    public class KalmanFeedbackModel : BaseKalman, IKalman
    {
        public new Vector X {
            get
            {
                return base.X_error - base.X_estimate;
            }
            set
            {
                X = value;
            } 
        }
    }
}
