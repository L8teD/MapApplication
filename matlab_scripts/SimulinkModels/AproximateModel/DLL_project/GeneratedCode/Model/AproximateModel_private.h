/*
 * AproximateModel_private.h
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

#ifndef RTW_HEADER_AproximateModel_private_h_
#define RTW_HEADER_AproximateModel_private_h_
#include "rtwtypes.h"
#include "builtin_typeid_types.h"
#include "multiword_types.h"

/* Private macros used by the generated code to access rtModel */
#ifndef rtmSetTFinal
# define rtmSetTFinal(rtm, val)        ((rtm)->Timing.tFinal = (val))
#endif

extern real_T rt_roundd_snf(real_T u);

#endif                               /* RTW_HEADER_AproximateModel_private_h_ */
