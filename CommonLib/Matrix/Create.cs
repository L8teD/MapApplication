using MyMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Create
    {
        public static Matrix MatrixC(Angles angles)
        {
            Matrix C = Matrix.Zero(3);
            
            double temp = angles.heading;
            angles.heading = MathTransformation.ReverseAngle(angles.heading);
            C[1,1] = -Math.Sin(angles.heading) * Math.Cos(angles.pitch);
            C[1,2] = Math.Sin(angles.heading) * Math.Sin(angles.pitch) * Math.Cos(angles.roll) + Math.Cos(angles.heading) * Math.Sin(angles.roll);
            C[1,3] = -Math.Sin(angles.heading) * Math.Sin(angles.pitch) * Math.Sin(angles.roll) + Math.Cos(angles.heading) * Math.Cos(angles.roll);
            C[2,1] = Math.Cos(angles.heading) * Math.Cos(angles.pitch);
            C[2,2] = -Math.Cos(angles.heading) * Math.Sin(angles.pitch) * Math.Cos(angles.roll) + Math.Sin(angles.heading) * Math.Sin(angles.roll);
            C[2,3] = Math.Cos(angles.heading) * Math.Sin(angles.pitch) * Math.Sin(angles.roll) + Math.Sin(angles.heading) * Math.Cos(angles.roll);
            C[3,1] = Math.Sin(angles.pitch);
            C[3,2] = Math.Cos(angles.pitch) * Math.Cos(angles.roll);
            C[3,3] = -Math.Cos(angles.pitch) * Math.Sin(angles.roll);

            for (int i = 0; i < C.Rows; i++)
            {
                for (int j = 0; j < C.Columns; j++)
                {
                    if (Math.Abs(C[i+1, j+1]) <= 0.000000001)
                        C[i + 1, j + 1] = 0;
                }
            }
            angles.heading = temp;
            return C;
        }
        public static Matrix MatrixM(double heading, double pitch)
        {
            Matrix M = Matrix.Zero(3);
            //double headingReverse = MathTransformation.ReverseAngle(heading);
            double headingReverse = heading;
            M[1,1] = Math.Sin(headingReverse) * Math.Tan(pitch);
            M[1,2] = Math.Cos(headingReverse) * Math.Tan(pitch);
            M[1,3] = -1;
            M[2,1] = Math.Cos(headingReverse);
            M[2,2] = -Math.Sin(headingReverse);
            M[2,3] = 0;
            M[3,1] = Math.Sin(headingReverse) / Math.Cos(pitch);
            M[3,2] = Math.Cos(-headingReverse) / Math.Cos(pitch);
            M[3,3] = 0;

            for (int i = 0; i < M.Rows; i++)
            {
                for (int j = 0; j < M.Columns; j++)
                {
                    if (Math.Abs(M[i+1, j+1]) <= 0.000000001)
                        M[i+1, j+1] = 0;
                }
            }

            return M;
        }
    }
}
