/*
 * Dryden.c
 *
 * Code generation for model "Dryden".
 *
 * Model version              : 1.2
 * Simulink Coder version : 9.2 (R2019b) 18-Jul-2019
 * C source code generated on : Mon May  2 21:36:59 2022
 *
 * Target selection: grt.tlc
 * Note: GRT includes extra infrastructure and instrumentation for prototyping
 * Embedded hardware selection: Intel->x86-64 (Windows64)
 * Code generation objectives: Unspecified
 * Validation result: Not run
 */

#include "Dryden.h"
#include "Dryden_private.h"

/* Continuous states */
X_Dryden_T Dryden_X;

/* External inputs (root inport signals with default storage) */
ExtU_Dryden_T Dryden_U;

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

  /* TransferFcn: '<Root>/Transfer Fcn1' incorporates:
   *  TransferFcn: '<Root>/Transfer Fcn2'
   */
  Out2_tmp_tmp = 3.0 * Dryden_P.WIND.Va0;
  Out2_tmp = sqrt(Out2_tmp_tmp / Dryden_P.WIND.L_v) * Dryden_P.WIND.sigma_v;
  Out2_tmp_0 = Out2_tmp * 0.0 / Dryden_P.TransferFcn1_Denominator[0];

  /* Outport: '<Root>/Out2' incorporates:
   *  Inport: '<Root>/In2'
   *  TransferFcn: '<Root>/Transfer Fcn1'
   */
  Dryden_Y.Out2 = ((Out2_tmp / Dryden_P.TransferFcn1_Denominator[0] - Out2_tmp_0
                    * (Dryden_P.TransferFcn1_Denominator[1] /
                       Dryden_P.TransferFcn1_Denominator[0])) *
                   Dryden_X.TransferFcn1_CSTATE[0] + Out2_tmp_0 * Dryden_U.In2)
    + (Out2_tmp * 0.10103629710818451 / Dryden_P.TransferFcn1_Denominator[0] -
       Out2_tmp_0 * (Dryden_P.TransferFcn1_Denominator[2] /
                     Dryden_P.TransferFcn1_Denominator[0])) *
    Dryden_X.TransferFcn1_CSTATE[1];

  /* TransferFcn: '<Root>/Transfer Fcn2' */
  Out2_tmp_tmp = sqrt(Out2_tmp_tmp / Dryden_P.WIND.L_w) * Dryden_P.WIND.sigma_w;
  Out2_tmp = Out2_tmp_tmp * 0.0 / Dryden_P.TransferFcn2_Denominator[0];

  /* Outport: '<Root>/Out3' incorporates:
   *  Inport: '<Root>/In3'
   *  TransferFcn: '<Root>/Transfer Fcn2'
   */
  Dryden_Y.Out3 = ((Out2_tmp_tmp / Dryden_P.TransferFcn2_Denominator[0] -
                    Out2_tmp * (Dryden_P.TransferFcn2_Denominator[1] /
    Dryden_P.TransferFcn2_Denominator[0])) * Dryden_X.TransferFcn2_CSTATE[0] +
                   Out2_tmp * Dryden_U.In3) + (Out2_tmp_tmp *
    0.40414518843273806 / Dryden_P.TransferFcn2_Denominator[0] - Out2_tmp *
    (Dryden_P.TransferFcn2_Denominator[2] / Dryden_P.TransferFcn2_Denominator[0]))
    * Dryden_X.TransferFcn2_CSTATE[1];

  if (rtmIsMajorTimeStep(Dryden_M)) {
    /* signal main to stop simulation */
    {                                  /* Sample time: [0.0s, 0.0s] */
      if ((rtmGetTFinal(Dryden_M)!=-1) &&
          !((rtmGetTFinal(Dryden_M)-Dryden_M->Timing.t[0]) > Dryden_M->Timing.t
            [0] * (DBL_EPSILON))) {
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
  }                                    /* end MajorTimeStep */
}

/* Derivatives for root system: '<Root>' */
void Dryden_derivatives(void)
{
  XDot_Dryden_T *_rtXdot;
  _rtXdot = ((XDot_Dryden_T *) Dryden_M->derivs);

  /* Derivatives for TransferFcn: '<Root>/Transfer Fcn' incorporates:
   *  Inport: '<Root>/In1'
   */
  _rtXdot->TransferFcn_CSTATE = 0.0;
  _rtXdot->TransferFcn_CSTATE += -Dryden_P.TransferFcn_Denominator[1] /
    Dryden_P.TransferFcn_Denominator[0] * Dryden_X.TransferFcn_CSTATE;
  _rtXdot->TransferFcn_CSTATE += Dryden_U.In1;

  /* Derivatives for TransferFcn: '<Root>/Transfer Fcn1' incorporates:
   *  Inport: '<Root>/In2'
   */
  _rtXdot->TransferFcn1_CSTATE[0] = 0.0;
  _rtXdot->TransferFcn1_CSTATE[0] += -Dryden_P.TransferFcn1_Denominator[1] /
    Dryden_P.TransferFcn1_Denominator[0] * Dryden_X.TransferFcn1_CSTATE[0];
  _rtXdot->TransferFcn1_CSTATE[1] = 0.0;
  _rtXdot->TransferFcn1_CSTATE[0] += -Dryden_P.TransferFcn1_Denominator[2] /
    Dryden_P.TransferFcn1_Denominator[0] * Dryden_X.TransferFcn1_CSTATE[1];
  _rtXdot->TransferFcn1_CSTATE[1] += Dryden_X.TransferFcn1_CSTATE[0];
  _rtXdot->TransferFcn1_CSTATE[0] += Dryden_U.In2;

  /* Derivatives for TransferFcn: '<Root>/Transfer Fcn2' incorporates:
   *  Inport: '<Root>/In3'
   */
  _rtXdot->TransferFcn2_CSTATE[0] = 0.0;
  _rtXdot->TransferFcn2_CSTATE[0] += -Dryden_P.TransferFcn2_Denominator[1] /
    Dryden_P.TransferFcn2_Denominator[0] * Dryden_X.TransferFcn2_CSTATE[0];
  _rtXdot->TransferFcn2_CSTATE[1] = 0.0;
  _rtXdot->TransferFcn2_CSTATE[0] += -Dryden_P.TransferFcn2_Denominator[2] /
    Dryden_P.TransferFcn2_Denominator[0] * Dryden_X.TransferFcn2_CSTATE[1];
  _rtXdot->TransferFcn2_CSTATE[1] += Dryden_X.TransferFcn2_CSTATE[0];
  _rtXdot->TransferFcn2_CSTATE[0] += Dryden_U.In3;
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
  Dryden_M->Timing.stepSize0 = 0.02;

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

  /* states (continuous) */
  {
    (void) memset((void *)&Dryden_X, 0,
                  sizeof(X_Dryden_T));
  }

  /* external inputs */
  (void)memset(&Dryden_U, 0, sizeof(ExtU_Dryden_T));

  /* external outputs */
  (void) memset((void *)&Dryden_Y, 0,
                sizeof(ExtY_Dryden_T));


  /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn' */
  Dryden_X.TransferFcn_CSTATE = 0.0;

  /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn1' */
  Dryden_X.TransferFcn1_CSTATE[0] = 0.0;

  /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn2' */
  Dryden_X.TransferFcn2_CSTATE[0] = 0.0;

  /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn1' */
  Dryden_X.TransferFcn1_CSTATE[1] = 0.0;

  /* InitializeConditions for TransferFcn: '<Root>/Transfer Fcn2' */
  Dryden_X.TransferFcn2_CSTATE[1] = 0.0;
}

/* Model terminate function */
void Dryden_terminate(void)
{
  /* (no terminate code required) */
}
