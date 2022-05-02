/*
 * Dryden_data.c
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

/* Block parameters (default storage) */
P_Dryden_T Dryden_P = {
  /* Variable: WIND
   * Referenced by:
   *   '<Root>/Transfer Fcn'
   *   '<Root>/Transfer Fcn1'
   *   '<Root>/Transfer Fcn2'
   */
  {
    1.0,
    1.0,
    1.0,
    200.0,
    200.0,
    50.0,
    3.0,
    1.05,
    0.7,
    35.0
  },

  /* Expression: [1 WIND.Va0/WIND.L_u]
   * Referenced by: '<Root>/Transfer Fcn'
   */
  { 1.0, 0.175 },

  /* Expression: poly([-WIND.Va0/WIND.L_v, -WIND.Va0/WIND.L_v])
   * Referenced by: '<Root>/Transfer Fcn1'
   */
  { 1.0, 0.35, 0.030624999999999996 },

  /* Expression: poly([-WIND.Va0/WIND.L_w, -WIND.Va0/WIND.L_w])
   * Referenced by: '<Root>/Transfer Fcn2'
   */
  { 1.0, 1.4, 0.48999999999999994 }
};
