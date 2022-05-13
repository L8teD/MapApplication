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
        
    }
    public class Randomize
    {
        //Random random;
        public void Init(int seed)
        {
            //random = new Random();
            Random_initialize();
        }
        public double GetRandom()
        {
            double output = 0;
            //int i = random.Next(0, 100);
            //
            //for (int j = 0; j < i; j++)
            Random_step();

            GetOutput(ref output);
            return output;
        }

        [DllImport("Random.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Random_step();
        [DllImport("Random.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Random_initialize();
        [DllImport("Random.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetOutput(ref double output);
    }
    public class Dryden
    {
        public void Init()
        {
            Dryden_initialize();
        }
        public DrydenOutput Model(InputWindData windData, double airSpeed, Randomize randomize)
        {
            DrydenInput input = new DrydenInput();
            input.rand1 = randomize.GetRandom();
            input.rand2 = randomize.GetRandom();
            input.rand3 = randomize.GetRandom();
            DrydenLocal local = new DrydenLocal(windData, airSpeed);

            SetInput(ref input, ref local);

            for (int i = 0; i < 50; i++) //50 - эмпирически определено соответствие 1сек расчета симулинка
            {
                Dryden_step();
            }

            DrydenOutput output = new DrydenOutput();

            GetOutput(ref output);

            return output;
        }

        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Dryden_step();
        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Dryden_initialize();
        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetInput(ref DrydenInput input, ref DrydenLocal local);
        [DllImport("Dryden.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetOutput(ref DrydenOutput output);
    }

    #region Dryden Types

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
    public struct DrydenLocal
    {
        MatlabLocal local;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        double[] denominator;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        double[] denominator1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        double[] denominator2;
        public DrydenLocal(InputWindData windData, double airSpeed)
        {
            local.wind_n = windData.wind_n;
            local.wind_e = windData.wind_e;
            local.wind_d = windData.wind_d;
            local.L_u = windData.L_u;
            local.L_v = windData.L_v;
            local.L_w = windData.L_w;
            local.sigma_u = windData.sigma_u;
            local.sigma_v = windData.sigma_v;
            local.sigma_w = windData.sigma_w;
            local.Va0 = airSpeed;

            denominator = new double[] { 1.0, local.Va0 / local.L_v };
            denominator1 = new double[] { 1.0, 2 * local.Va0 / local.L_v, Math.Pow(local.Va0 / local.L_v, 2) };
            denominator2 = new double[] { 1.0, 2 * local.Va0 / local.L_w, Math.Pow(local.Va0 / local.L_w, 2) };

        }
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    internal struct MatlabLocal
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
    public struct DrydenOutput
    {
        public double windRand1;
        public double windRand2;
        public double windRand3;
    }
    #endregion

}
