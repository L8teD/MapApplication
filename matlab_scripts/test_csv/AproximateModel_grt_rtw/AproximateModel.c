/*
 * AproximateModel.c
 *
 * Code generation for model "AproximateModel".
 *
 * Model version              : 1.17
 * Simulink Coder version : 9.2 (R2019b) 18-Jul-2019
 * C source code generated on : Sun May 22 21:56:25 2022
 *
 * Target selection: grt.tlc
 * Note: GRT includes extra infrastructure and instrumentation for prototyping
 * Embedded hardware selection: Intel->x86-64 (Windows64)
 * Code generation objectives: Unspecified
 * Validation result: Not run
 */

#include "AproximateModel.h"
#include "AproximateModel_private.h"

/* External inputs (root inport signals with default storage) */
ExtU_AproximateModel_T AproximateModel_U;

/* External outputs (root outports fed by signals with default storage) */
ExtY_AproximateModel_T AproximateModel_Y;

/* Real-time model */
RT_MODEL_AproximateModel_T AproximateModel_M_;
RT_MODEL_AproximateModel_T *const AproximateModel_M = &AproximateModel_M_;

/* Forward declaration for local functions */
static void AproximateModel_qrsolve(const real_T A_data[], const int32_T A_size
  [2], const int16_T B_data[], int16_T Y[6], int32_T *rankR);
static void AproximateModel_polyfit(const real_T x_data[], const int32_T *x_size,
  const int16_T y_data[], int16_T p[6]);
real_T rt_roundd_snf(real_T u)
{
  real_T y;
  if (fabs(u) < 4.503599627370496E+15) {
    if (u >= 0.5) {
      y = floor(u + 0.5);
    } else if (u > -0.5) {
      y = u * 0.0;
    } else {
      y = ceil(u - 0.5);
    }
  } else {
    y = u;
  }

  return y;
}

/* Function for MATLAB Function: '<Root>/MATLAB Function' */
static void AproximateModel_qrsolve(const real_T A_data[], const int32_T A_size
  [2], const int16_T B_data[], int16_T Y[6], int32_T *rankR)
{
  int32_T ma;
  int8_T jpvt[6];
  real_T b_A_data[6];
  real_T vn1[6];
  real_T vn2[6];
  int32_T pvt;
  real_T temp2;
  int32_T ix;
  real_T smax;
  int32_T iy;
  int32_T c_k;
  real_T temp2_tmp;
  ma = A_size[0];
  pvt = A_size[0] * A_size[1] - 1;
  if (0 <= pvt) {
    memcpy(&b_A_data[0], &A_data[0], (pvt + 1) * sizeof(real_T));
  }

  if (A_size[0] == 0) {
    for (ma = 0; ma < 6; ma++) {
      jpvt[ma] = (int8_T)(ma + 1);
    }
  } else {
    for (pvt = 0; pvt < 6; pvt++) {
      jpvt[pvt] = (int8_T)(pvt + 1);
      smax = 0.0;
      if (ma >= 1) {
        smax = A_data[pvt];
      }

      vn2[pvt] = smax;
      vn1[pvt] = smax;
    }

    pvt = 0;
    while (pvt <= ma - 1) {
      pvt = 0;
      ix = 0;
      smax = vn1[0];
      for (iy = 0; iy < 5; iy++) {
        ix++;
        if (vn1[ix] > smax) {
          pvt = iy + 1;
          smax = vn1[ix];
        }
      }

      if (pvt + 1 != 1) {
        ix = pvt;
        iy = 0;
        c_k = 0;
        while (c_k <= ma - 1) {
          smax = b_A_data[ix];
          b_A_data[ix] = b_A_data[iy];
          b_A_data[iy] = smax;
          ix++;
          iy++;
          c_k = 1;
        }

        ix = jpvt[pvt];
        jpvt[pvt] = jpvt[0];
        jpvt[0] = (int8_T)ix;
        vn1[pvt] = vn1[0];
        vn2[pvt] = vn2[0];
      }

      for (pvt = 0; pvt < 5; pvt++) {
        smax = vn1[pvt + 1];
        if (smax != 0.0) {
          smax = b_A_data[pvt + 1] / smax;
          smax = 1.0 - smax * smax;
          if (smax < 0.0) {
            smax = 0.0;
          }

          temp2_tmp = vn1[pvt + 1];
          temp2 = temp2_tmp / vn2[pvt + 1];
          temp2 = temp2 * temp2 * smax;
          if (temp2 <= 1.4901161193847656E-8) {
            vn1[pvt + 1] = 0.0;
            vn2[pvt + 1] = 0.0;
          } else {
            vn1[pvt + 1] = temp2_tmp * sqrt(smax);
          }
        }
      }

      pvt = 1;
    }
  }

  *rankR = 0;
  if (A_size[0] > 0) {
    while ((*rankR < 1) && (!(b_A_data[0] <= 1.3322676295501878E-14 * b_A_data[0])))
    {
      *rankR = 1;
    }
  }

  for (ma = 0; ma < 6; ma++) {
    Y[ma] = 0;
  }

  if (0 <= A_size[0] - 1) {
    Y[jpvt[0] - 1] = B_data[0];
  }

  ma = A_size[0];
  while (ma > 0) {
    ma = jpvt[0] - 1;
    smax = rt_roundd_snf((real_T)Y[ma] / b_A_data[0]);
    if (smax < 32768.0) {
      if (smax >= -32768.0) {
        Y[ma] = (int16_T)smax;
      } else {
        Y[ma] = MIN_int16_T;
      }
    } else {
      Y[ma] = MAX_int16_T;
    }

    ma = 0;
  }
}

/* Function for MATLAB Function: '<Root>/MATLAB Function' */
static void AproximateModel_polyfit(const real_T x_data[], const int32_T *x_size,
  const int16_T y_data[], int16_T p[6])
{
  real_T V_data[6];
  int32_T n;
  int32_T c_k;
  int32_T V_size[2];
  n = *x_size - 1;
  V_size[0] = *x_size;
  V_size[1] = 6;
  if (*x_size != 0) {
    if (0 <= n) {
      V_data[*x_size * 5] = 1.0;
      V_data[*x_size << 2] = x_data[0];
    }

    c_k = 0;
    while (c_k <= n) {
      V_data[V_size[0] * 3] = V_data[V_size[0] << 2] * x_data[0];
      c_k = 1;
    }

    c_k = 0;
    while (c_k <= n) {
      V_data[V_size[0] << 1] = V_data[V_size[0] * 3] * x_data[0];
      c_k = 1;
    }

    c_k = 0;
    while (c_k <= n) {
      V_data[V_size[0]] = V_data[V_size[0] << 1] * x_data[0];
      c_k = 1;
    }

    c_k = 0;
    while (c_k <= n) {
      V_data[0] = x_data[0] * V_data[V_size[0]];
      c_k = 1;
    }
  }

  AproximateModel_qrsolve(V_data, V_size, y_data, p, &n);
}

/* Model step function */
void AproximateModel_step(void)
{
  real_T b_x_data;
  int32_T b;
  int16_T tmp_data;
  int32_T x_size_idx_1;

  /* MATLAB Function: '<Root>/MATLAB Function' incorporates:
   *  Inport: '<Root>/In1'
   *  Inport: '<Root>/In2'
   *  Outport: '<Root>/Out1'
   */
  if (1 > AproximateModel_U.In2) {
    b = -1;
  } else {
    b = 0;
  }

  b++;
  if (b < 1) {
    x_size_idx_1 = 0;
  } else {
    x_size_idx_1 = 1;
  }

  if (0 <= x_size_idx_1 - 1) {
    b_x_data = 1.0;
  }

  if (0 <= b - 1) {
    tmp_data = AproximateModel_U.In1;
  }

  AproximateModel_polyfit(&b_x_data, &x_size_idx_1, &tmp_data,
    AproximateModel_Y.Out1);

  /* End of MATLAB Function: '<Root>/MATLAB Function' */

  /* Matfile logging */
  rt_UpdateTXYLogVars(AproximateModel_M->rtwLogInfo,
                      (&AproximateModel_M->Timing.taskTime0));

  /* signal main to stop simulation */
  {                                    /* Sample time: [0.02s, 0.0s] */
    if ((rtmGetTFinal(AproximateModel_M)!=-1) &&
        !((rtmGetTFinal(AproximateModel_M)-AproximateModel_M->Timing.taskTime0) >
          AproximateModel_M->Timing.taskTime0 * (DBL_EPSILON))) {
      rtmSetErrorStatus(AproximateModel_M, "Simulation finished");
    }
  }

  /* Update absolute time for base rate */
  /* The "clockTick0" counts the number of times the code of this task has
   * been executed. The absolute time is the multiplication of "clockTick0"
   * and "Timing.stepSize0". Size of "clockTick0" ensures timer will not
   * overflow during the application lifespan selected.
   * Timer of this task consists of two 32 bit unsigned integers.
   * The two integers represent the low bits Timing.clockTick0 and the high bits
   * Timing.clockTickH0. When the low bit overflows to 0, the high bits increment.
   */
  if (!(++AproximateModel_M->Timing.clockTick0)) {
    ++AproximateModel_M->Timing.clockTickH0;
  }

  AproximateModel_M->Timing.taskTime0 = AproximateModel_M->Timing.clockTick0 *
    AproximateModel_M->Timing.stepSize0 + AproximateModel_M->Timing.clockTickH0 *
    AproximateModel_M->Timing.stepSize0 * 4294967296.0;
}

/* Model initialize function */
void AproximateModel_initialize(void)
{
  /* Registration code */

  /* initialize non-finites */
  rt_InitInfAndNaN(sizeof(real_T));

  /* initialize real-time model */
  (void) memset((void *)AproximateModel_M, 0,
                sizeof(RT_MODEL_AproximateModel_T));
  rtmSetTFinal(AproximateModel_M, 1.0);
  AproximateModel_M->Timing.stepSize0 = 0.02;

  /* Setup for data logging */
  {
    static RTWLogInfo rt_DataLoggingInfo;
    rt_DataLoggingInfo.loggingInterval = NULL;
    AproximateModel_M->rtwLogInfo = &rt_DataLoggingInfo;
  }

  /* Setup for data logging */
  {
    rtliSetLogXSignalInfo(AproximateModel_M->rtwLogInfo, (NULL));
    rtliSetLogXSignalPtrs(AproximateModel_M->rtwLogInfo, (NULL));
    rtliSetLogT(AproximateModel_M->rtwLogInfo, "tout");
    rtliSetLogX(AproximateModel_M->rtwLogInfo, "");
    rtliSetLogXFinal(AproximateModel_M->rtwLogInfo, "");
    rtliSetLogVarNameModifier(AproximateModel_M->rtwLogInfo, "rt_");
    rtliSetLogFormat(AproximateModel_M->rtwLogInfo, 4);
    rtliSetLogMaxRows(AproximateModel_M->rtwLogInfo, 0);
    rtliSetLogDecimation(AproximateModel_M->rtwLogInfo, 1);
    rtliSetLogY(AproximateModel_M->rtwLogInfo, "");
    rtliSetLogYSignalInfo(AproximateModel_M->rtwLogInfo, (NULL));
    rtliSetLogYSignalPtrs(AproximateModel_M->rtwLogInfo, (NULL));
  }

  /* external inputs */
  (void)memset(&AproximateModel_U, 0, sizeof(ExtU_AproximateModel_T));

  /* external outputs */
  (void) memset(&AproximateModel_Y.Out1[0], 0,
                6U*sizeof(int16_T));

  /* Matfile logging */
  rt_StartDataLoggingWithStartTime(AproximateModel_M->rtwLogInfo, 0.0,
    rtmGetTFinal(AproximateModel_M), AproximateModel_M->Timing.stepSize0,
    (&rtmGetErrorStatus(AproximateModel_M)));
}

/* Model terminate function */
void AproximateModel_terminate(void)
{
  /* (no terminate code required) */
}
