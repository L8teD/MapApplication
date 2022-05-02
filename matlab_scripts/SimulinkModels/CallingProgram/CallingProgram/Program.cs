using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CallingProgram
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            //TestRandom();
            TestDryden();
        }

        #region Random
        //static void testrandom()
        //{
        //    random_initialize();
        //    double x = 0;
        //    for (int i = 0; i < 100; i++)
        //    {
        //        random_step();
        //        getoutput(ref x);
        //        console.writeline(x);
        //    }

        //    console.readkey();
        //}
        //[dllimport("random.dll", callingconvention = callingconvention.cdecl)]
        //private static extern void random_step();
        //[dllimport("random.dll", callingconvention = callingconvention.cdecl)]
        //private static extern void random_initialize();
        //[dllimport("random.dll", callingconvention = callingconvention.cdecl)]
        //private static extern void getoutput(ref double output);
        #endregion

        #region Dryden

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct DrydenInput
        {
            public double rand1;
            public double rand2;
            public double rand3;
        }
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct DrydenLocal
        {
            public DrydenWind wind;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public double[] denominator;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] denominator1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] denominator2;
        }

       
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct DrydenWind
        {
            public double wind_n;
            public double wind_e;
            public double wind_d;
            public double L_u;
            public double L_v;
            public double L_w;
            public double sigma_u;
            public double sigma_v;
            public double sigma_w;
            public double Va0;
        }
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct DrydenOutput
        {
            public double windRand1;
            public double windRand2;
            public double windRand3;
        }

        static void TestDryden()
        {
            Dryden_initialize();

            DrydenInput input = new DrydenInput();
            DrydenOutput output = new DrydenOutput();
            input.rand1 = 1.35;
            input.rand2 = 1.07;
            input.rand3 = 0.7;

            #region wind
            DrydenWind wind = new DrydenWind();
            wind.wind_n = 1;
            wind.wind_e = 1;
            wind.wind_d = 1;

            wind.L_u = 200;
            wind.L_v = 200;
            wind.L_w = 50;

            wind.sigma_u = 1.35;
            wind.sigma_v = 1.1;
            wind.sigma_w = 0.9;

            wind.Va0 = 50;
            #endregion

            DrydenLocal local = new DrydenLocal();
            local.wind = wind;
            local.denominator = new double[] { 1.0, wind.Va0 / wind.L_v };
            local.denominator1 = new double[] {1.0, 2 * wind.Va0 / wind.L_v,  Math.Pow(wind.Va0 / wind.L_v, 2) };
            local.denominator2 = new double[] {1.0, 2 * wind.Va0 / wind.L_w,  Math.Pow(wind.Va0 / wind.L_w, 2) };

            SetInput(ref input, ref local);

            for (int i = 0; i < 50; i++)
            {
                Dryden_step();
            }

            GetOutput(ref output);



            Console.ReadKey();
        }
        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Dryden_step();
        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Dryden_initialize();
        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetInput(ref DrydenInput input, ref DrydenLocal local);
        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetOutput(ref DrydenOutput output);

        #endregion
    }
}
