/*
 * Dryden_types.h
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

#ifndef RTW_HEADER_Dryden_types_h_
#define RTW_HEADER_Dryden_types_h_
#include "rtwtypes.h"
#include "builtin_typeid_types.h"
#include "multiword_types.h"
#ifndef DEFINED_TYPEDEF_FOR_struct_BioeieTGRXo2RBFnHAgUoD_
#define DEFINED_TYPEDEF_FOR_struct_BioeieTGRXo2RBFnHAgUoD_

typedef struct {
  real_T wind_n;
  real_T wind_e;
  real_T wind_d;
  real_T L_u;
  real_T L_v;
  real_T L_w;
  real_T sigma_u;
  real_T sigma_v;
  real_T sigma_w;
  real_T Va0;
} struct_BioeieTGRXo2RBFnHAgUoD;

#endif

/* Parameters (default storage) */
typedef struct P_Dryden_T_ P_Dryden_T;

/* Forward declaration for rtModel */
typedef struct tag_RTM_Dryden_T RT_MODEL_Dryden_T;

#endif                                 /* RTW_HEADER_Dryden_types_h_ */
