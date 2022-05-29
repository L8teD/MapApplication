/*
 * AproximateModel.h
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

#ifndef RTW_HEADER_AproximateModel_h_
#define RTW_HEADER_AproximateModel_h_
#include <math.h>
#include <string.h>
#include <float.h>
#include <stddef.h>
#ifndef AproximateModel_COMMON_INCLUDES_
# define AproximateModel_COMMON_INCLUDES_
#include "rtwtypes.h"
#include "rtw_continuous.h"
#include "rtw_solver.h"
#include "rt_logging.h"
#endif                                 /* AproximateModel_COMMON_INCLUDES_ */

#include "AproximateModel_types.h"

/* Shared type includes */
#include "multiword_types.h"
#include "rt_nonfinite.h"

/* Macros for accessing real-time model data structure */
#ifndef rtmGetFinalTime
# define rtmGetFinalTime(rtm)          ((rtm)->Timing.tFinal)
#endif

#ifndef rtmGetRTWLogInfo
# define rtmGetRTWLogInfo(rtm)         ((rtm)->rtwLogInfo)
#endif

#ifndef rtmGetErrorStatus
# define rtmGetErrorStatus(rtm)        ((rtm)->errorStatus)
#endif

#ifndef rtmSetErrorStatus
# define rtmSetErrorStatus(rtm, val)   ((rtm)->errorStatus = (val))
#endif

#ifndef rtmGetStopRequested
# define rtmGetStopRequested(rtm)      ((rtm)->Timing.stopRequestedFlag)
#endif

#ifndef rtmSetStopRequested
# define rtmSetStopRequested(rtm, val) ((rtm)->Timing.stopRequestedFlag = (val))
#endif

#ifndef rtmGetStopRequestedPtr
# define rtmGetStopRequestedPtr(rtm)   (&((rtm)->Timing.stopRequestedFlag))
#endif

#ifndef rtmGetT
# define rtmGetT(rtm)                  ((rtm)->Timing.taskTime0)
#endif

#ifndef rtmGetTFinal
# define rtmGetTFinal(rtm)             ((rtm)->Timing.tFinal)
#endif

#ifndef rtmGetTPtr
# define rtmGetTPtr(rtm)               (&(rtm)->Timing.taskTime0)
#endif

/* External inputs (root inport signals with default storage) */
typedef struct {
  int16_T In1;                         /* '<Root>/In1' */
  int16_T In2;                         /* '<Root>/In2' */
} ExtU_AproximateModel_T;

/* External outputs (root outports fed by signals with default storage) */
typedef struct {
  int16_T Out1[6];                     /* '<Root>/Out1' */
} ExtY_AproximateModel_T;

/* Real-time Model Data Structure */
struct tag_RTM_AproximateModel_T {
  const char_T *errorStatus;
  RTWLogInfo *rtwLogInfo;

  /*
   * Timing:
   * The following substructure contains information regarding
   * the timing information for the model.
   */
  struct {
    time_T taskTime0;
    uint32_T clockTick0;
    uint32_T clockTickH0;
    time_T stepSize0;
    time_T tFinal;
    boolean_T stopRequestedFlag;
  } Timing;
};

/* External inputs (root inport signals with default storage) */
extern ExtU_AproximateModel_T AproximateModel_U;

/* External outputs (root outports fed by signals with default storage) */
extern ExtY_AproximateModel_T AproximateModel_Y;

/* Model entry point functions */
extern void AproximateModel_initialize(void);
extern void AproximateModel_step(void);
extern void AproximateModel_terminate(void);

/* Real-time Model object */
extern RT_MODEL_AproximateModel_T *const AproximateModel_M;

/*-
 * The generated code includes comments that allow you to trace directly
 * back to the appropriate location in the model.  The basic format
 * is <system>/block_name, where system is the system number (uniquely
 * assigned by Simulink) and block_name is the name of the block.
 *
 * Use the MATLAB hilite_system command to trace the generated code back
 * to the model.  For example,
 *
 * hilite_system('<S3>')    - opens system 3
 * hilite_system('<S3>/Kp') - opens and selects block Kp which resides in S3
 *
 * Here is the system hierarchy for this model
 *
 * '<Root>' : 'AproximateModel'
 * '<S1>'   : 'AproximateModel/MATLAB Function'
 */
#endif                                 /* RTW_HEADER_AproximateModel_h_ */
