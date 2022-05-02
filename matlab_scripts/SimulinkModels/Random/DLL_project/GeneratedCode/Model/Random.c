/*
 * Random.c
 *
 * Code generation for model "Random".
 *
 * Model version              : 1.2
 * Simulink Coder version : 9.2 (R2019b) 18-Jul-2019
 * C source code generated on : Mon May  2 18:59:27 2022
 *
 * Target selection: grt.tlc
 * Note: GRT includes extra infrastructure and instrumentation for prototyping
 * Embedded hardware selection: Intel->x86-64 (Windows64)
 * Code generation objectives: Unspecified
 * Validation result: Not run
 */

#include "Random.h"
#include "Random_private.h"

/* Block states (default storage) */
DW_Random_T Random_DW;

/* External outputs (root outports fed by signals with default storage) */
ExtY_Random_T Random_Y;

/* Real-time model */
RT_MODEL_Random_T Random_M_;
RT_MODEL_Random_T *const Random_M = &Random_M_;
real_T rt_urand_Upu32_Yd_f_pw_snf(uint32_T *u)
{
  uint32_T lo;
  uint32_T hi;

  /* Uniform random number generator (random number between 0 and 1)

     #define IA      16807                      magic multiplier = 7^5
     #define IM      2147483647                 modulus = 2^31-1
     #define IQ      127773                     IM div IA
     #define IR      2836                       IM modulo IA
     #define S       4.656612875245797e-10      reciprocal of 2^31-1
     test = IA * (seed % IQ) - IR * (seed/IQ)
     seed = test < 0 ? (test + IM) : test
     return (seed*S)
   */
  lo = *u % 127773U * 16807U;
  hi = *u / 127773U * 2836U;
  if (lo < hi) {
    *u = 2147483647U - (hi - lo);
  } else {
    *u = lo - hi;
  }

  return (real_T)*u * 4.6566128752457969E-10;
}

real_T rt_nrand_Upu32_Yd_f_pw_snf(uint32_T *u)
{
  real_T y;
  real_T sr;
  real_T si;

  /* Normal (Gaussian) random number generator */
  do {
    sr = 2.0 * rt_urand_Upu32_Yd_f_pw_snf(u) - 1.0;
    si = 2.0 * rt_urand_Upu32_Yd_f_pw_snf(u) - 1.0;
    si = sr * sr + si * si;
  } while (si > 1.0);

  y = sqrt(-2.0 * log(si) / si) * sr;
  return y;
}

/* Model step function */
void Random_step(void)
{
  /* Outport: '<Root>/Out1' incorporates:
   *  RandomNumber: '<Root>/Random Number'
   */
  Random_Y.Out1 = Random_DW.NextOutput;

  /* Update for RandomNumber: '<Root>/Random Number' */
  Random_DW.NextOutput = rt_nrand_Upu32_Yd_f_pw_snf(&Random_DW.RandSeed) *
    Random_P.RandomNumber_StdDev + Random_P.RandomNumber_Mean;

  /* signal main to stop simulation */
  {                                    /* Sample time: [1.0s, 0.0s] */
    if ((rtmGetTFinal(Random_M)!=-1) &&
        !((rtmGetTFinal(Random_M)-Random_M->Timing.taskTime0) >
          Random_M->Timing.taskTime0 * (DBL_EPSILON))) {
      rtmSetErrorStatus(Random_M, "Simulation finished");
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
  if (!(++Random_M->Timing.clockTick0)) {
    ++Random_M->Timing.clockTickH0;
  }

  Random_M->Timing.taskTime0 = Random_M->Timing.clockTick0 *
    Random_M->Timing.stepSize0 + Random_M->Timing.clockTickH0 *
    Random_M->Timing.stepSize0 * 4294967296.0;
}

/* Model initialize function */
void Random_initialize(void)
{
  /* Registration code */

  /* initialize non-finites */
  rt_InitInfAndNaN(sizeof(real_T));

  /* initialize real-time model */
  (void) memset((void *)Random_M, 0,
                sizeof(RT_MODEL_Random_T));
  rtmSetTFinal(Random_M, 1.0);
  Random_M->Timing.stepSize0 = 1.0;

  /* Setup for data logging */
  {
    static RTWLogInfo rt_DataLoggingInfo;
    rt_DataLoggingInfo.loggingInterval = NULL;
    Random_M->rtwLogInfo = &rt_DataLoggingInfo;
  }

  /* Setup for data logging */
  {
    rtliSetLogXSignalInfo(Random_M->rtwLogInfo, (NULL));
    rtliSetLogXSignalPtrs(Random_M->rtwLogInfo, (NULL));
    rtliSetLogT(Random_M->rtwLogInfo, "tout");
    rtliSetLogX(Random_M->rtwLogInfo, "");
    rtliSetLogXFinal(Random_M->rtwLogInfo, "");
    rtliSetLogVarNameModifier(Random_M->rtwLogInfo, "rt_");
    rtliSetLogFormat(Random_M->rtwLogInfo, 4);
    rtliSetLogMaxRows(Random_M->rtwLogInfo, 0);
    rtliSetLogDecimation(Random_M->rtwLogInfo, 1);
    rtliSetLogY(Random_M->rtwLogInfo, "");
    rtliSetLogYSignalInfo(Random_M->rtwLogInfo, (NULL));
    rtliSetLogYSignalPtrs(Random_M->rtwLogInfo, (NULL));
  }

  /* states (dwork) */
  (void) memset((void *)&Random_DW, 0,
                sizeof(DW_Random_T));

  /* external outputs */
  Random_Y.Out1 = 0.0;

  {
    uint32_T tseed;
    int32_T r;
    int32_T t;
    real_T tmp;

    /* InitializeConditions for RandomNumber: '<Root>/Random Number' */
    tmp = floor(Random_P.RandomNumber_Seed);
    if (rtIsNaN(tmp) || rtIsInf(tmp)) {
      tmp = 0.0;
    } else {
      tmp = fmod(tmp, 4.294967296E+9);
    }

    tseed = tmp < 0.0 ? (uint32_T)-(int32_T)(uint32_T)-tmp : (uint32_T)tmp;
    r = (int32_T)(tseed >> 16U);
    t = (int32_T)(tseed & 32768U);
    tseed = ((((tseed - ((uint32_T)r << 16U)) + t) << 16U) + t) + r;
    if (tseed < 1U) {
      tseed = 1144108930U;
    } else {
      if (tseed > 2147483646U) {
        tseed = 2147483646U;
      }
    }

    Random_DW.RandSeed = tseed;
    Random_DW.NextOutput = rt_nrand_Upu32_Yd_f_pw_snf(&Random_DW.RandSeed) *
      Random_P.RandomNumber_StdDev + Random_P.RandomNumber_Mean;

    /* End of InitializeConditions for RandomNumber: '<Root>/Random Number' */
  }
}

/* Model terminate function */
void Random_terminate(void)
{
  /* (no terminate code required) */
}
