using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib
{
    public partial class Matrix
    {
        public void Multiply(Matrix Matrix)
        {
            if (Columns != Matrix.Rows)
                throw new InvalidOperationException("Cannot multiply matrices of different sizes.");
            var _result = new double[Rows, Matrix.Columns];
            for (int _row = 0; _row < Rows; _row++)
                for (int _col = 0; _col < Matrix.Columns; _col++)
                {
                    double _sum = 0;
                    for (int i = 0; i < Columns; i++)
                        _sum += _matrix[_row, i] * Matrix[i + 1, _col + 1];
                    _result[_row, _col] = _sum;
                }
            _matrix = _result;
        }
        public Vector Multiply(Vector vector)
        {
            if (Columns != vector.Length)
                throw new InvalidOperationException("Cannot multiply matrices of different sizes.");
            var _result = new double[Rows];
            for (int _row = 0; _row < Rows; _row++)
            { 
                double _sum = 0;
                for (int i = 0; i < Columns; i++)
                    _sum += _matrix[_row, i] * vector[i + 1];
                _result[_row] = _sum;
            }
            return new Vector(_result);
            //_matrix = _result;
        }
        public void Multiply(double Scalar)
        {
            for (int _row = 0; _row < _matrix.GetLength(0); _row++)
                for (int _col = 0; _col < _matrix.GetLength(1); _col++)
                {
                    _matrix[_row, _col] *= Scalar;
                }
        }
        public void Pow(double degree)
        {
            for (int _row = 0; _row < _matrix.GetLength(0); _row++)
                for (int _col = 0; _col < _matrix.GetLength(1); _col++)
                {
                    _matrix[_row, _col] = Math.Pow(_matrix[_row, _col], degree);
                }
        }

        public void HadamardProduct(Matrix Matrix)
        {
            if (Columns != Matrix.Columns || Rows != Matrix.Rows)
                throw new InvalidOperationException("Cannot multiply matrices of different sizes.");
            for (int _row = 0; _row < Rows; _row++)
                for (int _col = 0; _col < Columns; _col++)
                {
                    _matrix[_row, _col] *= Matrix[_row + 1, _col + 1];
                }
        }



        public void Add(Matrix Matrix)
        {
            if (Rows != Matrix.Rows || Columns != Matrix.Columns)
                throw new InvalidOperationException("Cannot add matrices of different sizes.");
            for (int _row = 0; _row < _matrix.GetLength(0); _row++)
                for (int _col = 0; _col < _matrix.GetLength(1); _col++)
                {
                    _matrix[_row, _col] += Matrix[_row + 1, _col + 1];
                }
        }


        public void Sub(Matrix Matrix)
        {
            if (Rows != Matrix.Rows || Columns != Matrix.Columns)
                throw new InvalidOperationException("Cannot sub matrices of different sizes.");
            for (int _row = 0; _row < _matrix.GetLength(0); _row++)
                for (int _col = 0; _col < _matrix.GetLength(1); _col++)
                {
                    _matrix[_row, _col] -= Matrix[_row + 1, _col + 1];
                }
        }
        private Tuple<double[][], int[]> LUPDecomposition(double[][] A)
        {
            int n = A.Length - 1;

            int[] pi = new int[n + 1];
            double p = 0;
            int kp = 0;
            int pik = 0;
            int pikp = 0;
            double aki = 0;
            double akpi = 0;

            for (int j = 0; j <= n; j++)
            {
                pi[j] = j;
            }

            for (int k = 0; k <= n; k++)
            {
                p = 0;
                for (int i = k; i <= n; i++)
                {
                    if (Math.Abs(A[i][k]) > p)
                    {
                        p = Math.Abs(A[i][k]);
                        kp = i;
                    }
                }
                if (p == 0)
                {
                    throw new Exception("singular matrix");
                }
                pik = pi[k];
                pikp = pi[kp];
                pi[k] = pikp;
                pi[kp] = pik;

                for (int i = 0; i <= n; i++)
                {
                    aki = A[k][i];
                    akpi = A[kp][i];
                    A[k][i] = akpi;
                    A[kp][i] = aki;
                }

                for (int i = k + 1; i <= n; i++)
                {
                    A[i][k] = A[i][k] / A[k][k];
                    for (int j = k + 1; j <= n; j++)
                    {
                        A[i][j] = A[i][j] - (A[i][k] * A[k][j]);
                    }
                }
            }
            return Tuple.Create(A, pi);
        }
        private double[] LUPSolve(double[][] LU, int[] pi, double[] b)
        {
            int n = LU.Length - 1;
            double[] x = new double[n + 1];
            double[] y = new double[n + 1];
            double suml = 0;
            double sumu = 0;
            double lij = 0;

            /*
            * Solve for y using formward substitution
            * */
            for (int i = 0; i <= n; i++)
            {
                suml = 0;
                for (int j = 0; j <= i - 1; j++)
                {
                    /*
                    * Since we've taken L and U as a singular matrix as an input
                    * the value for L at index i and j will be 1 when i equals j, not LU[i][j], since
                    * the diagonal values are all 1 for L.
                    * */
                    if (i == j)
                    {
                        lij = 1;
                    }
                    else
                    {
                        lij = LU[i][j];
                    }
                    suml = suml + (lij * y[j]);
                }
                y[i] = b[pi[i]] - suml;
            }
            //Solve for x by using back substitution
            for (int i = n; i >= 0; i--)
            {
                sumu = 0;
                for (int j = i + 1; j <= n; j++)
                {
                    sumu = sumu + (LU[i][j] * x[j]);
                }
                x[i] = (y[i] - sumu) / LU[i][i];
            }
            return x;
        }
        public void InverseNumerics()
        {
            MathNet.Numerics.LinearAlgebra.Matrix<double> matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(_matrix);
            MathNet.Numerics.LinearAlgebra.Matrix<double> invertedMatrix = matrix.Inverse();
            _matrix = invertedMatrix.ToArray();
        }
        public void Inverse2()
        {
            double[] solve;
            double[] e;
            double[][] A = new double[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                A[i] = new double[Columns];
                for (int j = 0; j < Columns; j++)
                {
                    A[i][j] = _matrix[i, j];
                }
            }
            Tuple<double[][], int[]> results = LUPDecomposition(A);

            double[][] LU = results.Item1;
            int[] P = results.Item2;

            for (int i = 0; i < Rows; i++)
            {
                e = new double[A[i].Length];
                e[i] = 1;
                solve = LUPSolve(LU, P, e);
                for (int j = 0; j < solve.Length; j++)
                {
                    _matrix[j,i] = solve[j];
                }
            }
            
        }
        public void Inverse()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Only square matrices can be inverted.");
            int dimension = Rows;
            var result = _matrix.Clone() as double[,];
            var identity = _matrix.Clone() as double[,];
            //make identity matrix
            for (int _row = 0; _row < dimension; _row++)
                for (int _col = 0; _col < dimension; _col++)
                {
                    identity[_row, _col] = (_row == _col) ? 1.0 : 0.0;
                }
            //invert
            for (int i = 0; i < dimension; i++)
            {
                double temporary = result[i, i];
                if (temporary == 0)
                    continue;
                else
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        result[i, j] = result[i, j] / temporary;
                        identity[i, j] = identity[i, j] / temporary;
                    }
                    for (int k = 0; k < dimension; k++)
                    {
                        if (i != k)
                        {
                            temporary = result[k, i];
                            for (int n = 0; n < dimension; n++)
                            {
                                result[k, n] = result[k, n] - temporary * result[i, n];
                                identity[k, n] = identity[k, n] - temporary * identity[i, n];
                            }
                        }
                    }
                }
                
            }
            _matrix = identity;
        }

        public void Transpose()
        {
            var _result = new double[Columns, Rows];
            for (int _row = 0; _row < _matrix.GetLength(0); _row++)
                for (int _col = 0; _col < _matrix.GetLength(1); _col++)
                {
                    _result[_col, _row] = _matrix[_row, _col];
                }
            _matrix = _result;
        }

        public void Map(Func<double, double> func)
        {
            for (int _row = 0; _row < _matrix.GetLength(0); _row++)
                for (int _col = 0; _col < _matrix.GetLength(1); _col++)
                {
                    _matrix[_row, _col] = func(_matrix[_row, _col]);
                }
        }

        public void Randomize(double lowest, double highest)
        {
            Random _random = new Random();
            double _diff = highest - lowest;
            for (int _row = 0; _row < _matrix.GetLength(0); _row++)
                for (int _col = 0; _col < _matrix.GetLength(1); _col++)
                {
                    _matrix[_row, _col] = (_random.NextDouble() * _diff) + lowest;
                }
        }

        public void Randomize()
        {
            Randomize(0.0, 1.0);
        }

        public Matrix Duplicate()
        {
            var _result = new Matrix(Rows, Columns);
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                {
                    _result[row + 1, col + 1] = _matrix[row, col];
                }
            return _result;
        }
    }
}
