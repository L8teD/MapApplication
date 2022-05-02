/*
 * Dryden.c
 *
 * Code generation for model "Dryden".
 *
 * Model version              : 1.3
 * Simulink Coder version : 9.2 (R2019b) 18-Jul-2019
 * C source code generated on : Mon May  2 22:21:08 2022
 *
 * Target selection: grt.tlc
 * Note: GRT includes extra infrastructure and instrumentation for prototyping
 * Embedded hardware selection: Intel->x86-64 (Windows64)
 * Code generation objectives: Unspecified
 * Validation result: Not run
 */

#include "Dryden.h"
#include "Dryden_private.h"

/* Block signals (default storage) */
B_Dryden_T Dryden_B;

/* Continuous states */
X_Dryden_T Dryden_X;

/* Block states (default storage) */
DW_Dryden_T Dryden_DW;

/* External outputs (root outports fed by signals with default storage) */
ExtY_Dryden_T Dryden_Y;

/* Real-time model */
RT_MODEL_Dryden_T Dryden_M_;
RT_MODEL_Dryden_T *const Dryden_M = &Dryden_M_;

/*
 * This function updates continuous states using the ODE3 fixed-step
 * solver algorithm
 */
static void rt_ertODEUpdateContinuousStates(RTWSolverInfo *si )
{
  /* Solver Matrices */
  static const real_T rt_ODE3_A[3] = {
    1.0/2.0, 3.0/4.0, 1.0
  };

  static const real_T rt_ODE3_B[3][3] = {
    { 1.0/2.0, 0.0, 0.0 },

    { 0.0, 3.0/4.0, 0.0 },

    { 2.0/9.0, 1.0/3.0, 4.0/9.0 }
  };

  time_T t = rtsiGetT(si);
  time_T tnew = rtsiGetSolverStopTime(si);
  time_T h = rtsiGetStepSize(si);
  real_T *x = rtsiGetContStates(si);
  ODE3_IntgData *id = (ODE3_IntgData *)rtsiGetSolverData(si);
  real_T *y = id->y;
  real_T *f0 = id->f[0];
  real_T *f1 = id->f[1];
  real_T *f2 = id->f[2];
  real_T hB[3];
  int_T i;
  int_T nXc = 5;
  rtsiSetSimTimeStep(si,MINOR_TIME_STEP);

  /* Save the state values at time t in y, we'll use x as ynew. */
  (void) memcpy(y, x,
                (uint_T)nXc*sizeof(real_T));

  /* Assumes that rtsiSetT and ModelOutputs are up-to-date */
  /* f0 = f(t,y) */
  rtsiSetdX(si, f0);
  Dryden_derivatives();

  /* f(:,2) = feval(odefile, t + hA(1), y + f*hB(:,1), args(:)(*)); */
  hB[0] = h * rt_ODE3_B[0][0];
  for (i = 0; i < nXc; i++) {
    x[i] = y[i] + (f0[i]*hB[0]);
  }

  rtsiSetT(si, t + h*rt_ODE3_A[0]);
  rtsiSetdX(si, f1);
  Dryden_step();
  Dryden_derivatives();

  /* f(:,3) = feval(odefile, t + hA(2), y + f*hB(:,2), args(:)(*)); */
  for (i = 0; i <= 1; i++) {
    hB[i] = h * rt_ODE3_B[1][i];
  }

  for (i = 0; i < nXc; i++) {
    x[i] = y[i] + (f0[i]*hB[0] + f1[i]*hB[1]);
  }

  rtsiSetT(si, t + h*rt_ODE3_A[1]);
  rtsiSetdX(si, f2);
  Dryden_step();
  Dryden_derivatives();

  /* tnew = t + hA(3);
     ynew = y + f*hB(:,3); */
  for (i = 0; i <= 2; i++) {
    hB[i] = h * rt_ODE3_B[2][i];
  }

  for (i = 0; i < nXc; i++) {
    x[i] = y[i] + (f0[i]*hB[0] + f1[i]*hB[1] + f2[i]*hB[2]);
  }

  rtsiSetT(si, tnew);
  rtsiSetSimTimeStep(si,MAJOR_TIME_STEP);
}

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
void Dryden_step(void)
{
  real_T Out2_tmp;
  real_T Out2_tmp_0;
  real_T Out2_tmp_tmp;
  if (rtmIsMajorTimeStep(Dryden_M)) {
    /* set solver stop time */
    if (!(Dryden_M->Timing.clockTick0+1)) {
      rtsiSetSolverStopTime(&Dryden_M->solverInfo,
                            ((Dryden_M->Timing.clockTickH0 + 1) *
        Dryden_M->Timing.stepSize0 * 4294967296.0));
    } else {
      rtsiSetSolverStopTime(&Dryden_M->solverInfo, ((Dryden_M->Timing.clockTick0
        + 1) * Dryden_M->Timing.stepSize0 + Dryden_M->Timing.clockTickH0 *
        Dryden_M->Timing.stepSize0 * 4294967296.0));
    }
  }                                    /* end MajorTimeStep */

  /* Update absolute time of base rate at minor time step */
  if (rtmIsMinorTimeStep(Dryden_M)) {
    Dryden_M->Timing.t[0] = rtsiGetT(&Dryden_M->solverInfo);
  }

  /* Outport: '<Root>/Out1' incorporates:
   *  TransferFcn: '<Root>/Transfer Fcn'
   */
  Dryden_Y.Out1 = sqrt(2.0 * Dryden_P.WIND.Va0 / Dryden_P.WIND.L_u) *
    Dryden_P.WIND.sigma_u / Dryden_P.TransferFcn_Denominator[0] *
    Dryden_X.TransferFcn_CSTATE;
  if (rtmIsMajorTimeStep(Dryden_M)) {
    /* RandomNumber: '<Root>/Random Number' */
    Dryden_B.RandomNumber = Dryden_DW.NextOutput;

    /* RandomNumber: '<Root>/Random Number1' */
    Dryden_B.RandomNumber1 = Dryden_DW.NextOutput_g;

    /* RandomNumber: '<Root>/Random Number2' */
    Dryden_B.RandomNumber2 = Dryden_DW.NextOutput_j;
  }

  /* TransferFcn: '<Root>/Transfer Fcn1' incorporates:
   *  TransferFcn: '<Root>/Transfer Fcn2'
   */
  Out2_tmp_tmp = 3.0 * Dryden_P.WIND.Va0;
  Out2_tmp = sqrt(Out2_tmp_tmp / Dryden_P.WIND.L_v) * Dryden_P.WIND.sigma_v;
  Out2_tmp_0 = Out2_tmp * 0.0 / Dryden_P.TransferFcn1_Denominator[0];

  /* Outport: '<Root>/Out2' incorporates:
   *  TransferFcn: '<Root>/Transfer Fcn1'
   */
  Dryden_Y.Out2 = ((Out2_tmp / Dryden_P.TransferFcn1_Denominator[0] - Out2_tmp_0
                    * (Dryden_P.TransferFcn1_Denominator[1] /
                       Dryden_P.TransferFcn1_Denominator[0])) *
                   Dryden_X.TransferFcn1_CSTATE[0] + Out2_tmp_0 *
                   Dryden_B.RandomNumber1) + (Out2_tmp * 0.14433756729740646 /
    Dryden_P.TransferFcn1_Denominator[0] - Out2_tmp_0 *
    (Dryden_P.TransferFcn1_Denominator[2] / Dryden_P.TransferFcn1_Denominator[0]))
    * Dryden_X.TransferFcn1_CSTATE[1];

  /* TransferFcn: '<Root>/Transfer Fcn2' */
  Out2_tmp_tmp = sqrt(Out2_tmp_tmp / Dryden_P.WIND.L_w) * Dryden_P.WIND.sigma_w;
  Out2_tmp = Out2_tmp_tmp * 0.0 / Dryden_P.TransferFcn2_Denominator[0];

  /* Outport: '<Root>/Out3' incorporates:
   *  TransferFcn: '<Root>/Transfer Fcn2'
   */
  Dryden_Y.Out3 = ((Out2_tmp_tmp / Dryden_P.TransferFcn2_Denominator[0] -
                    Out2_tmp * (Dryden_P.TransferFcn2_Denominator[1] /
    Dryden_P.TransferFcn2_Denominator[0])) * Dryden_X.TransferFcn2_CSTATE[0] +
                   Out2_tmp * Dryden_B.RandomNumber2) + (Out2_tmp_tmp *
    0.57735026918962584 / Dryden_P.TransferFcn2_Denominator[0] - Out2_tmp *
    (Dryden_P.TransferFcn2_Denominator[2] / Dryden_P.TransferFcn2_Denominator[0]))
    * Dryden_X.TransferFcn2_CSTATE[1];
  if (rtmIsMajorTimeStep(Dryden_M)) {
    /* Matfile logging */
    rt_UpdateTXYLogVars(Dryden_M->rtwLogInfo, (Dryden_M->Timing.t));
  }                                    /* end MajorTimeStep */

  if (rtmIsMajorTimeStep(Dryden_M)) {
    if (rtmIsMajorTimeStep(Dryden_M)) {
      /* Update for RandomNumber: '<Root>/Random Number' */
      Dryden_DW.NextOutput = rt_nrand_Upu32_Yd_f_pw_snf(&Dryden_DW.RandSeed) *
        Dryden_P.RandomNumber_StdDev + Dryden_P.RandomNumber_Mean;

      /* Update for RandomNumber: '<Root>/Random Number1' */
      Dryden_DW.NextOutput_g = rt_nrand_Upu32_Yd_f_pw_snf(&Dryden_DW.RandSeed_e)
        * Dryden_P.RandomNumber1_StdDev + Dryden_P.RandomNumber1_Mean;

      /* Update for RandomNumber: '<Root>/Random Number2' */
      Dryden_DW.NextOutput_j = rt_nrand_Upu32_Yd_f_pw_snf(&Dryden_DW.RandSeed_m)
        * Dryden_P.RandomNumber2_StdDev + Dryden_P.RandomNumber2_Mean;
    }
  }                                    /* end MajorTimeStep */

  if (rtmIsMajorTimeStep(Dryden_M)) {
    /* signal main to stop simulation */
    {                                  /* Sample time: [0.0s, 0.0s] */
      if ((rtmGetTFinal(Dryden_M)!=-1) &&
          !((rtmGetTFinal(Dryden_M)-(((Dryden_M->Timing.clockTick1+
               Dryden_M->Timing.clockTickH1* 4294967296.0)) * 1.0)) >
            (((Dryden_M->Timing.clockTick1+Dryden_M->Timing.clockTickH1*
               4294967296.0)) * 1.0) * (DBL_EPSILON))) {
        rtmSetErrorStatus(Dryden_M, "Simulation finished");
      }
    }

    rt_ertODEUpdateContinuousStates(&Dryden_M->solverInfo);

    /* Update absolute time for base rate */
    /* The "clockTick0" counts the number of times the code of this task has
     * been executed. The absolute time is the multiplication of "clockTick0"
     * and "Timing.stepSize0". Size of "clockTick0" ensures timer will not
     * overflow during the application lifespan selected.
     * Timer of this task consists of two 32 bit unsigned integers.
     * The two integers represent the low bits Timing.clockTick0 and the high bits
     * Timing.clockTickH0. When the low bit overflows to 0, the high bits increment.
     */
    if (!(++Dryden_M->Timing.clockTick0)) {
      ++Dryden_M->Timing.clockTickH0;
    }

    Dryden_M->Timing.t[0] = rtsiGetSolverStopTime(&Dryden_M->solverInfo);

    {
      /* Update absolute timer for sample time: [1.0s, 0.0s] */
      /* The "clockTick1" counts the number of times the code of this task has
       * been executed. The resolution of this integer timer is 1.0, which is the step size
       * of the task. Size of "clockTick1" ensures timer will not overflow during the
       * application lifespan selected.
       * Timer of this task consists of two 32 bit unsigned integers.
       * The two integers represent the low bits Timing.clockTick1 and the high bits
       * Timing.clockTickH1. When the low bit overflows to 0, the high bits increment.
       */
      Dryden_M->Timing.clockTick1++;
      if (!Dryden_M->Timing.clockTick1) {
        Dryden_M->Timing.clockTickH1++;
      }
    }
  }                                    /* end MajorTimeStep */
}

/* Derivatives for root system: '<Root>' */
void Dryden_derivatives(void)
{
  XDot_Dryden_T *_rtXdot;
  _rtXdot = ((XDot_Dryden_T *) Dryden_M->derivs);

  /* Derivatives for TransferFcn: '<Root>/Transfer Fcn' */
  _rtXdot->TransferFcn_CSTATE = 0.0;
  _rtXdot->TransferFcn_CSTATE += -Dryden_P.TransferFcn_Denominator[1] /
    Dryden_P.TransferFcn_Denominator[0] * Dryden_X.TransferFcn_CSTATE;
  _rtXdot->TransferFcn_CSTATE += Dryden_B.RandomNumber;

  /* Derivatives for TransferFcn: '<Root>/Transfer Fcn1' */
  _rtXdot->TransferFcn1_CSTATE[0] = 0.0;
  _rtXdot->TransferFcn1_CSTATE[0] += -Dryden_P.TransferFcn1_Denominator[1] /
    Dryden_P.TransferFcn1_Denominator[0] * Dryden_X.TransferFcn1_CSTATE[0];
  _rtXdot->TransferFcn1_CSTATE[1] = 0.0;
  _rtXdot->TransferFcn1_CSTATE[0] += -Dryden_P.TransferFcn1_Denominator[2] /
    Dryden_P.TransferFcn1_Denominator[0] * Dryden_X.TransferFcn1_CSTATE[1];
  _rtXdot->TransferFcn1_CSTATE[1] += Dryden_X.TransferFcn1_CSTATE[0];
  _rtXdot->TransferFcn1_CSTATE[0] += Dryden_B.RandomNumber1;

  /* Derivatives for TransferFcn: '<Root>/Transfer Fcn2' */
  _rtXdot->TransferFcn2_CSTATE[0] = 0.0;
  _rtXdot->TransferFcn2_CSTATE[0] += -Dryden_P.TransferFcn2_Denominator[1] /
    Dryden_P.TransferFcn2_Denominator[0] * Dryden_X.TransferFcn2_CSTATE[0];
  _rtXdot->TransferFcn2_CSTATE[1] = 0.0;
  _rtXdot->TransferFcn2_CSTATE[0] += -Dryden_P.TransferFcn2_Denominator[2] /
    Dryden_P.TransferFcn2_Denominator[0] * Dryden_X.TransferFcn2_CSTATE[1];
  _rtXdot->TransferFcn2_CSTATE[1] += Dryden_X.TransferFcn2_CSTATE[0];
  _rtXdot->TransferFcn2_CSTATE[0] += Dryden_B.RandomNumber2;
}

/* Model initialize function */
void Dryden_initialize(void)
{
  /* Registration code */

  /* initialize non-finites */
  rt_InitInfAndNaN(sizeof(real_T));

  /* initialize real-time model */
  (void) memset((void *)Dryden_M, 0,
                sizeof(RT_MODEL_Dryden_T));

  {
    /* Setup solver object */
    rtsiSetSimTimeStepPtr(&Dryden_M->solverInfo, &Dryden_M->Timing.simTimeStep);
    rtsiSetTPtr(&Dryden_M->solverInfo, &rtmGetTPtr(Dryden_M));
    rtsiSetStepSizePtr(&Dryden_M->solverInfo, &Dryden_M->Timing.stepSize0);
    rtsiSetdXPtr(&Dryden_M->solverInfo, &Dryden_M->derivs);
    rtsiSetContStatesPtr(&Dryden_M->solverInfo, (real_T **)
                         &Dryden_M->contStates);
    rtsiSetNumContStatesPtr(&Dryden_M->solverInfo,
      &Dryden_M->Sizes.numContStates);
    rtsiSetNumPeriodicContStatesPtr(&Dryden_M->solverInfo,
      &Dryden_M->Sizes.numPeriodicContStates);
    rtsiSetPeriodicContStateIndicesPtr(&Dryden_M->solverInfo,
      &Dryden_M->periodicContStateIndices);
    rtsiSetPeriodicContStateRangesPtr(&Dryden_M->solverInfo,
      &Dryden_M->periodicContStateRanges);
    rtsiSetErrorStatusPtr(&Dryden_M->solverInfo, (&rtmGetErrorStatus(Dryden_M)));
    rtsiSetRTModelPtr(&Dryden_M->solverInfo, Dryden_M);
  }

  rtsiSetSimTimeStep(&Dryden_M->solverInfo, MAJOR_TIME_STEP);
  Dryden_M->intgData.y = Dryden_M->odeY;
  Dryden_M->intgData.f[0] = Dryden_M->odeF[0];
  Dryden_M->intgData.f[1] = Dryden_M->odeF[1];
  Dryden_M->intgData.f[2] = Dryden_M->odeF[2];
  Dryden_M->contStates = ((X_Dryden_T *) &Dryden_X);
  rtsiSetSolverData(&Dryden_M->solverInfo, (void *)&Dryden_M->intgData);
  rtsiSetSolverName(&Dryden_M->solverInfo,"ode3");
  rtmSetTPtr(Dryden_M, &Dryden_M->Timing.tArray[0]);
  rtmSetTFinal(Dryden_M, 1.0);
  Dryden_M->Timing.stepSize0 = 1.0;

  /* Setup for data logging */
  {
    static RTWLogInfo rt_DataLoggingInfo;
    rt_DataLoggingInfo.loggingInterval = NULL;
    Dryden_M->rtwLogInfo = &rt_DataLoggingInfo;
  }

  /* Setup for data logging */
  {
    rtliSetLogXSignalInfo(Dryden_M->rtwLogInfo, (NULL));
    rtliSetLogXSignalPtrs(Dryden_M->rtwLogInfo, (NULL));
    rtliSetLogT(Dryden_M->rtwLogInfo, "tout");
    rtliSetLogX(Dryden_M->rtwLogInfo, "");
    rtliSetLogXFinal(Dryden_M->rtwLogInfo, "");
    rtliSetLogVarNameModifier(Dryden_M->rtwLogInfo, "rt_");
    rtliSetLogFormat(Dryden_M->rtwLogInfo, 4);
    rtliSetLogMaxRows(Dryden_M->rtwLogInfo, 0);
    rtliSetLogDecimation(Dryden_M->rtwLogInfo, 1);
    rtliSetLogY(Dryden_M->rtwLogInfo, "");
    rtliSetLogYSignalInfo(Dryden_M->rtwLogInfo, (NULL));
    rtliSetLogYSignalPtrs(Dryden_M->rtwLogInfo, (NULL));
  }

  /* block I/O */
  (void) memset(((void *) &Dryden_B), 0,
                sizeof(B_Dryden_T));

  /* states (continuous) */
  {
    (void) memset((void *)&Dryden_X, 0,
                  sizeof(X_Dryden_T));
  }

  /* states (dwork) */
  (void) memset((void *)&Dryden_DW, 0,
                sizeof(DW_Dryden_T));

  /* external outputs */
  (void) memset((void *)&Dryden_Y, 0,
                sizeof(ExtY_Dryden_T));

  /* Matfile logging */
  rt_StartDataLoggingWithStartTime(Dryden_M->rtwLogInfo, 0.0, rtmGetTFinal
    (Dryden_M), Dryden_M->Timing.stepSize0, (&rtmGetErrorStatus(Dryden_M)));

  {
    uint32_T tseed;
    int32_T r;
    int32_T t;
    real_T tmp;

    /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn' */
    Dryden_X.TransferFcn_CSTATE = 0.0;

    /* InitializeConditions for RandomNumber: '<Root>/Random Number' */
    tmp = floor(Dryden_P.RandomNumber_Seed);
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

    Dryden_DW.RandSeed = tseed;
    Dryden_DW.NextOutput = rt_nrand_Upu32_Yd_f_pw_snf(&Dryden_DW.RandSeed) *
      Dryden_P.RandomNumber_StdDev + Dryden_P.RandomNumber_Mean;

    /* End of InitializeConditions for RandomNumber: '<Root>/Random Number' */

    /* InitializeConditions for RandomNumber: '<Root>/Random Number1' */
    tmp = floor(Dryden_P.RandomNumber1_Seed);
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

    Dryden_DW.RandSeed_e = tseed;
    Dryden_DW.NextOutput_g = rt_nrand_Upu32_Yd_f_pw_snf(&Dryden_DW.RandSeed_e) *
      Dryden_P.RandomNumber1_StdDev + Dryden_P.RandomNumber1_Mean;

    /* End of InitializeConditions for RandomNumber: '<Root>/Random Number1' */

    /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn1' */
    Dryden_X.TransferFcn1_CSTATE[0] = 0.0;
    Dryden_X.TransferFcn1_CSTATE[1] = 0.0;

    /* InitializeConditions for RandomNumber: '<Root>/Random Number2' */
    tmp = floor(Dryden_P.RandomNumber2_Seed);
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

    Dryden_DW.RandSeed_m = tseed;
    Dryden_DW.NextOutput_j = rt_nrand_Upu32_Yd_f_pw_snf(&Dryden_DW.RandSeed_m) *
      Dryden_P.RandomNumber2_StdDev + Dryden_P.RandomNumber2_Mean;

    /* End of InitializeConditions for RandomNumber: '<Root>/Random Number2' */

    /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn2' */
    Dryden_X.TransferFcn2_CSTATE[0] = 0.0;
    Dryden_X.TransferFcn2_CSTATE[1] = 0.0;
  }
}

/* Model terminate function */
void Dryden_terminate(void)
{
  /* (no terminate code required) */
}
