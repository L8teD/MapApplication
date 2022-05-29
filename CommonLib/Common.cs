using CsvHelper;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Common
    {
        public static bool IsGardenRingRoad(double lat, double lon)
        {
            return lat > 55.73 && lat < 55.768 && lon > 37.594 && lon < 37.6278;
        }

        public static void WriteCSV<T>(List<T> data, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(data);
                    csvWriter.Flush();
                }
            }
        }
        public static double[] Polyfit(double[] x, double[] y, int degree)
        {
            // Vandermonde matrix
            var v = new DenseMatrix(x.Length, degree + 1);
            for (int i = 0; i < v.RowCount; i++)
                for (int j = 0; j <= degree; j++) v[i, j] = Math.Pow(x[i], j);
            var yv = new DenseVector(y).ToColumnMatrix();
            QR<double> qr = v.QR();
            // Math.Net doesn't have an "economy" QR, so:
            // cut R short to square upper triangle, then recompute Q
            var r = qr.R.SubMatrix(0, degree + 1, 0, degree + 1);
            var q = v.Multiply(r.Inverse());
            var p = r.Inverse().Multiply(q.TransposeThisAndMultiply(yv));
            return p.Column(0).ToArray();
        }
        public static MyMatrix.Vector Aproximate(List<double> values, int degree)
        {
            double[] v = values.ToArray();
            double[] x = new double[v.Length];
            for (int i = 0; i < x.Length; i++)
                x[i] = i+1;

            var coeff = Polyfit(x, v, degree);
            MyMatrix.Vector output = MyMatrix.Vector.Zero(values.Count);
            MyMatrix.Vector x_vector = new MyMatrix.Vector(x);
            for (int k = 0; k < degree + 1; k++)
            {
                MyMatrix.Vector d = x_vector ^ k;
                MyMatrix.Vector j = d * coeff[k];
                output = output + j;
            }
            return output;
        }
    }
}
