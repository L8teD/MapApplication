using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Matrix
{
    public class Create
    {
        public static double[][] MatrixC(double heading, double pitch, double roll)
        {
            double[][] C = MatrixOperations.Create(3, 3);

            C[0][0] = -Math.Sin(heading) * Math.Cos(pitch);
            C[0][1] = Math.Sin(heading) * Math.Sin(pitch) * Math.Cos(roll) + Math.Cos(heading) * Math.Sin(roll);
            C[0][2] = -Math.Sin(heading) * Math.Sin(pitch) * Math.Sin(roll) + Math.Cos(heading) * Math.Cos(roll);
            C[1][0] = Math.Cos(heading) * Math.Cos(pitch);
            C[1][1] = -Math.Cos(heading) * Math.Sin(pitch) * Math.Cos(roll) + Math.Sin(heading) * Math.Sin(roll);
            C[1][2] = Math.Cos(heading) * Math.Sin(pitch) * Math.Sin(roll) + Math.Sin(heading) * Math.Cos(roll);
            C[2][0] = Math.Sin(pitch);
            C[2][1] = Math.Cos(pitch) * Math.Cos(roll);
            C[2][2] = -Math.Cos(pitch) * Math.Sin(roll);

            return C;
        }
    }
}
