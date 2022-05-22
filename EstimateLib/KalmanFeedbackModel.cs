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
        protected new Vector X_previous
        {
            get
            {
                if (base.X_estimate == null)
                {
                    return X.Dublicate();
                }
                else
                {
                    Vector _ = base.X_estimate.Dublicate();
                    _[1] = 0;
                    _[2] = 0;
                    _[3] = 0;
                    _[4] = 0;
                    _[5] = 0;
                    _[6] = 0;
                    return _;
                }
            }
            set
            {
                X_previous = value;
            }
        }
    }
}
