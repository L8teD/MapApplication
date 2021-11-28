using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonLib.Types;

namespace CommonLib.Matrix
{
    public class Create
    {
        public static double[][] MatrixC(Angles angles)
        {
            double[][] C = MatrixOperations.Create(3, 3);

            C[0][0] = -Math.Sin(angles.heading) * Math.Cos(angles.pitch);
            C[0][1] = Math.Sin(angles.heading) * Math.Sin(angles.pitch) * Math.Cos(angles.roll) + Math.Cos(angles.heading) * Math.Sin(angles.roll);
            C[0][2] = -Math.Sin(angles.heading) * Math.Sin(angles.pitch) * Math.Sin(angles.roll) + Math.Cos(angles.heading) * Math.Cos(angles.roll);
            C[1][0] = Math.Cos(angles.heading) * Math.Cos(angles.pitch);
            C[1][1] = -Math.Cos(angles.heading) * Math.Sin(angles.pitch) * Math.Cos(angles.roll) + Math.Sin(angles.heading) * Math.Sin(angles.roll);
            C[1][2] = Math.Cos(angles.heading) * Math.Sin(angles.pitch) * Math.Sin(angles.roll) + Math.Sin(angles.heading) * Math.Cos(angles.roll);
            C[2][0] = Math.Sin(angles.pitch);
            C[2][1] = Math.Cos(angles.pitch) * Math.Cos(angles.roll);
            C[2][2] = -Math.Cos(angles.pitch) * Math.Sin(angles.roll);

            return C;
        }
    }
}
