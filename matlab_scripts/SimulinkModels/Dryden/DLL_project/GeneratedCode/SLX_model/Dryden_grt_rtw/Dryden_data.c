/*
 * Dryden_data.c
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
    1.0,
    1.0,
    1.0,
    50.0
  },

  /* Expression: [1 WIND.Va0/WIND.L_u]
   * Referenced by: '<Root>/Transfer Fcn'
   */
  { 1.0, 0.25 },

  /* Expression: 0
   * Referenced by: '<Root>/Random Number'
   */
  0.0,

  /* Computed Parameter: RandomNumber_StdDev
   * Referenced by: '<Root>/Random Number'
   */
  1.0,

  /* Expression: 0
   * Referenced by: '<Root>/Random Number'
   */
  0.0,

  /* Expression: 0
   * Referenced by: '<Root>/Random Number1'
   */
  0.0,

  /* Computed Parameter: RandomNumber1_StdDev
   * Referenced by: '<Root>/Random Number1'
   */
  1.0,

  /* Expression: 0
   * Referenced by: '<Root>/Random Number1'
   */
  0.0,

  /* Expression: poly([-WIND.Va0/WIND.L_v, -WIND.Va0/WIND.L_v])
   * Referenced by: '<Root>/Transfer Fcn1'
   */
  { 1.0, 0.5, 0.0625 },

  /* Expression: 0
   * Referenced by: '<Root>/Random Number2'
   */
  0.0,

  /* Computed Parameter: RandomNumber2_StdDev
   * Referenced by: '<Root>/Random Number2'
   */
  1.0,

  /* Expression: 0
   * Referenced by: '<Root>/Random Number2'
   */
  0.0,

  /* Expression: poly([-WIND.Va0/WIND.L_w, -WIND.Va0/WIND.L_w])
   * Referenced by: '<Root>/Transfer Fcn2'
   */
  { 1.0, 2.0, 1.0 }
};
