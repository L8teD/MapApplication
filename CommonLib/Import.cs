using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Import
    {
        #region Rand
        [DllImport("RandomSample.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void random_sample_step();
        [DllImport("RandomSample.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void random_sample_initialize();
        [DllImport("RandomSample.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetOutput(ref double output);

        public static double GetRandom()
        {
            double output = 0;
            random_sample_step();
            GetOutput(ref output);
            return output;
        }
        #endregion

        #region dryden
        //[DllImport("TestSimulink.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void test_step();
        //[DllImport("TestSimulink.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void test_initialize();
        //[DllImport("TestSimulink.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void SetInput(ref ExtU_test_T input);       
        //[DllImport("TestSimulink.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void GetOutput(ref ExtY_test_T output);
        #endregion
    }
}
